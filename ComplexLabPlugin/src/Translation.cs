using System.ComponentModel;
using LabApi.API.Features.Configs;

namespace ComplexLabPlugin;

/// <summary>
/// Переводы для плагина ComplexLabPlugin
/// </summary>
public class Translation : PluginTranslation
{
    [Description("Сообщение при загрузке плагина")]
    public string PluginLoaded { get; set; } = "ComplexLabPlugin загружен!";

    [Description("Сообщение при выключении плагина")]
    public string PluginDisabled { get; set; } = "ComplexLabPlugin выключен";

    [Description("Префикс для сообщений плагина")]
    public string PluginPrefix { get; set; } = "[ComplexLab]";

    [Description("Сообщение о недостаточном количестве игроков")]
    public string NotEnoughPlayers { get; set; } = "Недостаточно игроков для кастомного раунда";

    [Description("Сообщение об активации кастомного раунда")]
    public string CustomRoundActivated { get; set; } = "⚠️ КАСТОМНЫЙ РАУНД: {0} ⚠️";

    [Description("Сообщение о получении достижения")]
    public string AchievementUnlocked { get; set; } = "🏆 Достижение: {0}!";

    [Description("Сообщение о получении денег")]
    public string MoneyReceived { get; set; } = "💰 +{0} монет. Баланс: {1}";

    [Description("Сообщение о запрете предмета")]
    public string ItemBanned { get; set; } = "Предмет {0} запрещен в этом раунде!";

    [Description("Сообщение о кике за читы")]
    public string KickForCheating { get; set; } = "Подозрительная активность (читы)";

    [Description("Название достижения 'Первый вход'")]
    public string AchievementFirstLogin { get; set; } = "Первый вход";

    [Description("Название достижения '10 убийств'")]
    public string AchievementKills10 { get; set; } = "10 убийств";

    [Description("Название достижения '50 убийств'")]
    public string AchievementKills50 { get; set; } = "50 убийств";

    [Description("Название достижения '100 убийств'")]
    public string AchievementKills100 { get; set; } = "100 убийств";

    [Description("Название достижения '5 побегов'")]
    public string AchievementEscapes5 { get; set; } = "5 побегов";

    [Description("Режим OneHitKill")]
    public string ModeOneHitKill { get; set; } = "Один удар - одна смерть";

    [Description("Режим OnlyShotguns")]
    public string ModeOnlyShotguns { get; set; } = "Только дробовики";

    [Description("Режим LowGravity")]
    public string ModeLowGravity { get; set; } = "Низкая гравитация";

    [Description("Режим SpeedRun")]
    public string ModeSpeedRun { get; set; } = "Спидран";

    [Description("Режим ZombieInfection")]
    public string ModeZombieInfection { get; set; } = "Зомби инфекция";

    [Description("Режим GunGame")]
    public string ModeGunGame { get; set; } = "Гонка вооружений";

    [Description("Режим Hardcore")]
    public string ModeHardcore { get; set; } = "Хардкор";

    [Description("Режим InvisibleSCPs")]
    public string ModeInvisibleSCPs { get; set; } = "Невидимые SCP";

    [Description("Режим ExplosiveWeapons")]
    public string ModeExplosiveWeapons { get; set; } = "Взрывное оружие";
}
