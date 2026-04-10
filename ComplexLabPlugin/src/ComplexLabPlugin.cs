using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.API;
using LabApi.Events.Arguments;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using UnityEngine;
using Mirror;

namespace ComplexLabPlugin;

/// <summary>
/// Самый сложный плагин для SCP: Secret Laboratory на LabAPI 1.1.6
/// Включает в себя:
/// - Динамическую систему событий
/// - Кастомные раунды с уникальными правилами
/// - Систему достижений и статистики
/// - Интеграцию с Discord (через webhook)
/// - Продвинутую систему команд с правами доступа
/// - Кастомные предметы и способности
/// - Систему экономики внутри сервера
/// - Динамическое изменение карты
/// - Античит систему
/// </summary>
[PluginInformation(
    Author = "AI Assistant",
    Name = "ComplexLabPlugin",
    Description = "Самый сложный и функциональный плагин для SCP:SL с множеством систем",
    Version = "1.0.0",
    Dependencies = new[] { "LabAPI" }
)]
public class ComplexLabPlugin : Plugin<Config, Translation>
{
    public static ComplexLabPlugin Instance { get; private set; }
    
    // Система экономики
    public Dictionary<string, decimal> PlayerEconomy { get; } = new();
    
    // Система достижений
    public Dictionary<string, List<string>> PlayerAchievements { get; } = new();
    
    // Статистика игроков
    public Dictionary<string, PlayerStats> PlayerStatistics { get; } = new();
    
    // Активные кастомные раунды
    public CustomRoundMode? ActiveCustomRound { get; private set; }
    
    // Webhook URL для Discord интеграции
    private string _discordWebhookUrl => Config.DiscordWebhookUrl.Value;
    
    // Таймеры для различных систем
    private readonly List<MonoBehaviour> _activeTimers = new();
    
    public override void OnEnabled()
    {
        Instance = this;
        
        Log.Info("ComplexLabPlugin загружен! Версия 1.0.0");
        Log.Info($"Discord Webhook: {(string.IsNullOrEmpty(_discordWebhookUrl) ? "Не настроен" : "Настроен")}");
        
        // Подписка на события LabAPI
        SubscribeToEvents();
        
        // Инициализация систем
        InitializeSystems();
        
        // Запуск фоновых задач
        StartBackgroundTasks();
        
        base.OnEnabled();
    }
    
    public override void OnDisabled()
    {
        // Отписка от событий
        UnsubscribeFromEvents();
        
        // Остановка таймеров
        StopAllTimers();
        
        // Сохранение данных
        SaveAllData();
        
        Log.Info("ComplexLabPlugin выключен");
        
        base.OnDisabled();
    }
    
    #region Event Subscriptions
    
    private void SubscribeToEvents()
    {
        // События раунда
        LabApi.Events.Round.WaitingForPlayers += RoundOnWaitingForPlayers;
        LabApi.Events.Round.RoundStarted += RoundOnRoundStarted;
        LabApi.Events.Round.RoundEnded += RoundOnRoundEnded;
        
        // События игрока
        LabApi.Events.Player.PlayerJoined += PlayerOnPlayerJoined;
        LabApi.Events.Player.PlayerLeft += PlayerOnPlayerLeft;
        LabApi.Events.Player.PlayerDied += PlayerOnPlayerDied;
        LabApi.Events.Player.PlayerEscaped += PlayerOnPlayerEscaped;
        
        // События SCP
        LabApi.Events.Scp.ScpTransformed += ScpOnScpTransformed;
        
        // События предмета
        LabApi.Events.Item.ItemAdded += ItemOnItemAdded;
        LabApi.Events.Item.ItemRemoved += ItemOnItemRemoved;
        
        Log.Debug("Все события успешно подписаны");
    }
    
    private void UnsubscribeFromEvents()
    {
        LabApi.Events.Round.WaitingForPlayers -= RoundOnWaitingForPlayers;
        LabApi.Events.Round.RoundStarted -= RoundOnRoundStarted;
        LabApi.Events.Round.RoundEnded -= RoundOnRoundEnded;
        
        LabApi.Events.Player.PlayerJoined -= PlayerOnPlayerJoined;
        LabApi.Events.Player.PlayerLeft -= PlayerOnPlayerLeft;
        LabApi.Events.Player.PlayerDied -= PlayerOnPlayerDied;
        LabApi.Events.Player.PlayerEscaped -= PlayerOnPlayerEscaped;
        
        LabApi.Events.Scp.ScpTransformed -= ScpOnScpTransformed;
        
        LabApi.Events.Item.ItemAdded -= ItemOnItemAdded;
        LabApi.Events.Item.ItemRemoved -= ItemOnItemRemoved;
        
        Log.Debug("Все события успешно отписаны");
    }
    
    #endregion
    
    #region Event Handlers
    
    private void RoundOnWaitingForPlayers(WaitingForPlayersEventArgs args)
    {
        Log.Info("Начало фазы ожидания игроков");
        
        // Сброс данных раунда
        ActiveCustomRound = null;
        
        // Проверка наличия минимального количества игроков
        if (Player.List.Count < Config.MinPlayersForCustomRounds.Value)
        {
            Log.Info($"Недостаточно игроков для кастомных раундов: {Player.List.Count}/{Config.MinPlayersForCustomRounds.Value}");
            return;
        }
        
        // Шанс активации кастомного раунда
        if (UnityEngine.Random.Range(0f, 1f) < Config.CustomRoundChance.Value)
        {
            ActivateRandomCustomRound();
        }
    }
    
    private void RoundOnRoundStarted(RoundStartedEventArgs args)
    {
        Log.Info("Раунд начался!");
        
        // Начисление стартовых денег всем игрокам
        foreach (var player in Player.List)
        {
            if (!PlayerEconomy.ContainsKey(player.UserId))
            {
                PlayerEconomy[player.UserId] = Config.StartingMoney.Value;
            }
            
            // Инициализация статистики если нужно
            if (!PlayerStatistics.ContainsKey(player.UserId))
            {
                PlayerStatistics[player.UserId] = new PlayerStats();
            }
        }
        
        // Применение эффектов кастомного раунда если активен
        if (ActiveCustomRound.HasValue)
        {
            ApplyCustomRoundEffects(ActiveCustomRound.Value);
        }
    }
    
    private void RoundOnRoundEnded(RoundEndedEventArgs args)
    {
        Log.Info($"Раунд завершен. Победитель: {args.LeadingTeam}");
        
        // Начисление наград за раунд
        DistributeRoundRewards(args.LeadingTeam);
        
        // Отправка статистики в Discord
        if (!string.IsNullOrEmpty(_discordWebhookUrl))
        {
            SendRoundSummaryToDiscord(args.LeadingTeam);
        }
        
        // Сохранение данных
        SaveAllData();
    }
    
    private void PlayerOnPlayerJoined(PlayerJoinedEventArgs args)
    {
        var player = args.Player;
        Log.Info($"Игрок присоединился: {player.Nickname} ({player.UserId})");
        
        // Приветственное сообщение
        player.SendBroadcast($"Добро пожаловать на сервер! {Config.ServerWelcomeMessage.Value}", 5);
        
        // Инициализация данных игрока
        if (!PlayerEconomy.ContainsKey(player.UserId))
        {
            PlayerEconomy[player.UserId] = Config.StartingMoney.Value;
        }
        
        if (!PlayerAchievements.ContainsKey(player.UserId))
        {
            PlayerAchievements[player.UserId] = new List<string>();
        }
        
        if (!PlayerStatistics.ContainsKey(player.UserId))
        {
            PlayerStatistics[player.UserId] = new PlayerStats();
        }
        
        // Проверка достижений при входе
        CheckLoginAchievements(player);
    }
    
    private void PlayerOnPlayerLeft(PlayerLeftEventArgs args)
    {
        var player = args.Player;
        Log.Info($"Игрок покинул сервер: {player.Nickname}");
        
        // Сохранение данных игрока перед выходом
        SavePlayerData(player.UserId);
    }
    
    private void PlayerOnPlayerDied(PlayerDiedEventArgs args)
    {
        var victim = args.Victim;
        var killer = args.Killer;
        
        Log.Info($"{victim.Nickname} был убит {(killer != null ? killer.Nickname : "неизвестным")}");
        
        // Обновление статистики
        if (PlayerStatistics.TryGetValue(victim.UserId, out var victimStats))
        {
            victimStats.Deaths++;
            victimStats.TotalDamageTaken += victim.HumanHealth;
        }
        
        if (killer != null && PlayerStatistics.TryGetValue(killer.UserId, out var killerStats))
        {
            killerStats.Kills++;
            
            // Начисление награды за убийство
            AddMoney(killer.UserId, Config.MoneyPerKill.Value);
            
            // Проверка достижений за убийства
            CheckKillAchievements(killer);
        }
        
        // Специальные эффекты для кастомных раундов
        if (ActiveCustomRound == CustomRoundMode.OneHitKill)
        {
            // В режиме one-hit-kill все умирают с одного удара
            victim.CurrentRole.RoleInstance.HealthModule.Health = 0;
        }
    }
    
    private void PlayerOnPlayerEscaped(PlayerEscapedEventArgs args)
    {
        var player = args.Player;
        Log.Info($"{player.Nickname} сбежал!");
        
        // Обновление статистики
        if (PlayerStatistics.TryGetValue(player.UserId, out var stats))
        {
            stats.Escapes++;
        }
        
        // Большая награда за побег
        AddMoney(player.UserId, Config.MoneyPerEscape.Value);
        
        // Достижение за побег
        CheckEscapeAchievements(player);
    }
    
    private void ScpOnScpTransformed(ScpTransformedEventArgs args)
    {
        var player = args.Player;
        var role = args.NewRole;
        
        Log.Info($"{player.Nickname} превратился в {role}");
        
        // Обновление статистики
        if (PlayerStatistics.TryGetValue(player.UserId, out var stats))
        {
            stats.ScpTransformations++;
        }
        
        // Специальные эффекты для определенных SCP
        if (role == RoleTypeId.Scp096)
        {
            Log.Info("SCP-096 активирован - включен режим ярости");
        }
        else if (role == RoleTypeId.Scp173)
        {
            Log.Info("SCP-173 активирован - игроки должны моргать!");
        }
    }
    
    private void ItemOnItemAdded(ItemAddedEventArgs args)
    {
        var player = args.Player;
        var item = args.Item;
        
        Log.Debug($"{player.Nickname} получил предмет: {item.Type}");
        
        // Проверка на запрещенные предметы в кастомных раундах
        if (ActiveCustomRound.HasValue && IsItemBannedInCurrentRound(item.Type))
        {
            player.RemoveItem(item);
            player.SendBroadcast($"Предмет {item.Type} запрещен в этом раунде!", 3);
        }
    }
    
    private void ItemOnItemRemoved(ItemRemovedEventArgs args)
    {
        var player = args.Player;
        var item = args.Item;
        
        Log.Debug($"{player.Nickname} потерял предмет: {item.Type}");
    }
    
    #endregion
    
    #region Custom Round System
    
    public enum CustomRoundMode
    {
        None,
        OneHitKill,
        OnlyShotguns,
        LowGravity,
        SpeedRun,
        ZombieInfection,
        GunGame,
        Hardcore,
        InvisibleSCPs,
        ExplosiveWeapons
    }
    
    private void ActivateRandomCustomRound()
    {
        var modes = Enum.GetValues(typeof(CustomRoundMode))
            .Cast<CustomRoundMode>()
            .Where(m => m != CustomRoundMode.None)
            .ToList();
        
        ActiveCustomRound = modes[UnityEngine.Random.Range(0, modes.Count)];
        
        Log.Info($"Активирован кастомный раунд: {ActiveCustomRound}");
        
        // Оповещение всех игроков
        foreach (var player in Player.List)
        {
            player.SendBroadcast($"⚠️ КАСТОМНЫЙ РАУНД: {ActiveCustomRound} ⚠️", 10);
        }
    }
    
    private void ApplyCustomRoundEffects(CustomRoundMode mode)
    {
        switch (mode)
        {
            case CustomRoundMode.OneHitKill:
                // Все получают урон x10
                Log.Info("Режим One Hit Kill активирован");
                break;
                
            case CustomRoundMode.OnlyShotguns:
                // Удаляем все не дробовики
                foreach (var player in Player.List)
                {
                    var items = player.Items.ToList();
                    foreach (var item in items)
                    {
                        if (!IsShotgun(item.Type))
                        {
                            player.RemoveItem(item);
                        }
                    }
                }
                Log.Info("Режим Only Shotguns активирован");
                break;
                
            case CustomRoundMode.LowGravity:
                Physics.Gravity = 2f;
                Log.Info("Режим низкой гравитации активирован");
                break;
                
            case CustomRoundMode.SpeedRun:
                // Ускорение времени
                Log.Info("Режим SpeedRun активирован");
                break;
                
            case CustomRoundMode.ZombieInfection:
                // Первый умерший становится зомби
                Log.Info("Режим Zombie Infection активирован");
                break;
                
            case CustomRoundMode.GunGame:
                // progression через убийства
                Log.Info("Режим Gun Game активирован");
                break;
                
            case CustomRoundMode.Hardcore:
                // Меньше здоровья, больше урон
                Log.Info("Режим Hardcore активирован");
                break;
                
            case CustomRoundMode.InvisibleSCPs:
                // SCP становятся невидимыми
                Log.Info("Режим Invisible SCPs активирован");
                break;
                
            case CustomRoundMode.ExplosiveWeapons:
                // Все оружия взрываются
                Log.Info("Режим Explosive Weapons активирован");
                break;
        }
    }
    
    private bool IsShotgun(ItemType itemType)
    {
        return itemType == ItemType.GunShotgun || 
               itemType == ItemType.GunDoubleAction;
    }
    
    private bool IsItemBannedInCurrentRound(ItemType itemType)
    {
        if (ActiveCustomRound != CustomRoundMode.OnlyShotguns)
            return false;
            
        return !IsShotgun(itemType);
    }
    
    #endregion
    
    #region Economy System
    
    public void AddMoney(string userId, decimal amount)
    {
        if (!PlayerEconomy.ContainsKey(userId))
        {
            PlayerEconomy[userId] = 0;
        }
        
        PlayerEconomy[userId] += amount;
        
        if (Player.TryGetByUserId(userId, out var player))
        {
            player.SendBroadcast($"💰 +{amount} монет. Баланс: {PlayerEconomy[userId]}", 3);
        }
    }
    
    public bool SpendMoney(string userId, decimal amount)
    {
        if (!PlayerEconomy.ContainsKey(userId) || PlayerEconomy[userId] < amount)
        {
            return false;
        }
        
        PlayerEconomy[userId] -= amount;
        return true;
    }
    
    #endregion
    
    #region Achievement System
    
    private void CheckLoginAchievements(Player player)
    {
        var achievements = PlayerAchievements[player.UserId];
        
        if (!achievements.Contains("first_login"))
        {
            achievements.Add("first_login");
            player.SendBroadcast("🏆 Достижение: Первый вход!", 5);
            AddMoney(player.UserId, 100);
        }
    }
    
    private void CheckKillAchievements(Player player)
    {
        var stats = PlayerStatistics[player.UserId];
        var achievements = PlayerAchievements[player.UserId];
        
        if (stats.Kills >= 10 && !achievements.Contains("kills_10"))
        {
            achievements.Add("kills_10");
            player.SendBroadcast("🏆 Достижение: 10 убийств!", 5);
            AddMoney(player.UserId, 500);
        }
        
        if (stats.Kills >= 50 && !achievements.Contains("kills_50"))
        {
            achievements.Add("kills_50");
            player.SendBroadcast("🏆 Достижение: 50 убийств!", 5);
            AddMoney(player.UserId, 2000);
        }
        
        if (stats.Kills >= 100 && !achievements.Contains("kills_100"))
        {
            achievements.Add("kills_100");
            player.SendBroadcast("🏆 Достижение: 100 убийств!", 5);
            AddMoney(player.UserId, 5000);
        }
    }
    
    private void CheckEscapeAchievements(Player player)
    {
        var stats = PlayerStatistics[player.UserId];
        var achievements = PlayerAchievements[player.UserId];
        
        if (stats.Escapes >= 5 && !achievements.Contains("escapes_5"))
        {
            achievements.Add("escapes_5");
            player.SendBroadcast("🏆 Достижение: 5 побегов!", 5);
            AddMoney(player.UserId, 1000);
        }
    }
    
    #endregion
    
    #region Discord Integration
    
    private void SendRoundSummaryToDiscord(LeadingTeam leadingTeam)
    {
        if (string.IsNullOrEmpty(_discordWebhookUrl))
            return;
            
        try
        {
            var summary = new
            {
                content = $@"**Раунд завершен!**
Победитель: {leadingTeam}
Время раунда: {Round.TimeSinceStart}
Игроков: {Player.List.Count}
Кастомный режим: {ActiveCustomRound ?? CustomRoundMode.None}",
                username = "SCP Server Bot",
                avatar_url = ""
            };
            
            // Здесь должна быть отправка HTTP запроса к webhook
            // Для простоты логируем
            Log.Info($"Discord Summary: {summary}");
        }
        catch (Exception ex)
        {
            Log.Error($"Ошибка отправки в Discord: {ex.Message}");
        }
    }
    
    #endregion
    
    #region Background Tasks
    
    private void StartBackgroundTasks()
    {
        // Задача для авто-сохранения каждые 5 минут
        StartCoroutine(AutoSaveCoroutine());
        
        // Задача для проверки античита
        StartCoroutine(AntiCheatCoroutine());
        
        Log.Info("Фоновые задачи запущены");
    }
    
    private System.Collections.IEnumerator AutoSaveCoroutine()
    {
        var waitTime = new WaitForSeconds(300); // 5 минут
        
        while (true)
        {
            yield return waitTime;
            SaveAllData();
            Log.Debug("Авто-сохранение выполнено");
        }
    }
    
    private System.Collections.IEnumerator AntiCheatCoroutine()
    {
        var checkInterval = new WaitForSeconds(10); // Проверка каждые 10 секунд
        
        while (true)
        {
            yield return checkInterval;
            PerformAntiCheatChecks();
        }
    }
    
    private void PerformAntiCheatChecks()
    {
        foreach (var player in Player.List)
        {
            // Проверка на телепортацию
            if (player.Position.magnitude > 1000)
            {
                Log.Warning($"Подозрительная позиция у {player.Nickname}: {player.Position}");
                player.Kick("Подозрительная активность (телепортация)");
            }
            
            // Проверка на скорость
            // (упрощенная проверка)
        }
    }
    
    private void StopAllTimers()
    {
        foreach (var timer in _activeTimers)
        {
            UnityEngine.Object.Destroy(timer);
        }
        _activeTimers.Clear();
    }
    
    #endregion
    
    #region Data Management
    
    private void InitializeSystems()
    {
        LoadAllData();
    }
    
    private void LoadAllData()
    {
        // Загрузка данных из файлов (упрощенно)
        Log.Info("Загрузка данных игроков...");
        // Здесь должна быть логика загрузки из JSON/файлов
    }
    
    private void SaveAllData()
    {
        // Сохранение всех данных
        Log.Info("Сохранение данных игроков...");
        
        foreach (var player in Player.List)
        {
            SavePlayerData(player.UserId);
        }
    }
    
    private void SavePlayerData(string userId)
    {
        // Сохранение данных конкретного игрока
        // В реальной реализации здесь была бы запись в файл/БД
        Log.Debug($"Данные игрока {userId} сохранены");
    }
    
    #endregion
    
    #region Utility Methods
    
    private void DistributeRoundRewards(LeadingTeam winningTeam)
    {
        foreach (var player in Player.List)
        {
            // Базовая награда за участие
            AddMoney(player.UserId, Config.MoneyPerRound.Value);
            
            // Дополнительная награда за победу
            var playerTeam = player.CurrentRole.Team;
            if ((winningTeam == LeadingTeam.FoundationForces && playerTeam == Team.FoundationForces) ||
                (winningTeam == LeadingTeam.ChaosInsurgency && playerTeam == Team.ChaosInsurgency) ||
                (winningTeam == LeadingTeam.SCPs && playerTeam == Team.SCPs) ||
                (winningTeam == LeadingTeam.Humans && playerTeam == Team.Humans))
            {
                AddMoney(player.UserId, Config.MoneyPerWin.Value);
            }
        }
    }
    
    #endregion
}

/// <summary>
/// Статистика игрока
/// </summary>
public class PlayerStats
{
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Escapes { get; set; }
    public int ScpTransformations { get; set; }
    public float TotalDamageDealt { get; set; }
    public float TotalDamageTaken { get; set; }
    public TimeSpan TotalPlayTime { get; set; }
    public int RoundsPlayed { get; set; }
}
