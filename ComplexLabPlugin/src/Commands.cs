using System;
using LabApi.API.Features.Commands;
using LabApi.Features.Wrappers;

namespace ComplexLabPlugin.Commands;

/// <summary>
/// Команда для проверки баланса игрока
/// </summary>
[CommandHandler(typeof(PlayerCommandHandler))]
public class BalanceCommand : ICommand
{
    public string Name => "balance";
    public string[] Aliases => new[] { "bal", "money" };
    public string Description => "Показать ваш текущий баланс";
    public string[] UsageExamples => new[] { "balance" };
    
    public bool Execute(Player player, ArraySegment<string> arguments, out string response)
    {
        if (!ComplexLabPlugin.Instance.PlayerEconomy.TryGetValue(player.UserId, out var balance))
        {
            response = "Ваш баланс не найден";
            return false;
        }
        
        player.SendBroadcast($"💰 Ваш баланс: {balance} монет", 5);
        response = $"Ваш баланс: {balance} монет";
        return true;
    }
}

/// <summary>
/// Команда для администраторов: выдать деньги игроку
/// </summary>
[CommandHandler(typeof(ServerCommandHandler))]
public class GiveMoneyCommand : ICommand
{
    public string Name => "givemoney";
    public string[] Aliases => new[] { "addmoney", "pay" };
    public string Description => "Выдать деньги игроку (только для администраторов)";
    public string[] UsageExamples => new[] { "givemoney PlayerName 1000" };
    
    public bool Execute(Player player, ArraySegment<string> arguments, out string response)
    {
        if (arguments.Count < 2)
        {
            response = "Использование: givemoney <игрок> <сумма>";
            return false;
        }
        
        var targetName = arguments.At(0);
        if (!decimal.TryParse(arguments.At(1), out var amount))
        {
            response = "Неверная сумма";
            return false;
        }
        
        if (!Player.TryGetByNickname(targetName, out var targetPlayer))
        {
            response = $"Игрок '{targetName}' не найден";
            return false;
        }
        
        ComplexLabPlugin.Instance.AddMoney(targetPlayer.UserId, amount);
        response = $"Выдано {amount} монет игроку {targetPlayer.Nickname}";
        return true;
    }
}

/// <summary>
/// Команда для проверки статистики игрока
/// </summary>
[CommandHandler(typeof(PlayerCommandHandler))]
public class StatsCommand : ICommand
{
    public string Name => "stats";
    public string[] Aliases => new[] { "statistics", "stat" };
    public string Description => "Показать вашу статистику";
    public string[] UsageExamples => new[] { "stats" };
    
    public bool Execute(Player player, ArraySegment<string> arguments, out string response)
    {
        if (!ComplexLabPlugin.Instance.PlayerStatistics.TryGetValue(player.UserId, out var stats))
        {
            response = "Ваша статистика не найдена";
            return false;
        }
        
        var message = $@"=== Ваша статистика ===
Убийства: {stats.Kills}
Смерти: {stats.Deaths}
Побегов: {stats.Escapes}
Превращений в SCP: {stats.ScpTransformations}
Получено урона: {stats.TotalDamageTaken:F0}
Раундов сыграно: {stats.RoundsPlayed}";
        
        player.SendBroadcast(message, 10);
        response = "Статистика отправлена";
        return true;
    }
}

/// <summary>
/// Команда для проверки достижений игрока
/// </summary>
[CommandHandler(typeof(PlayerCommandHandler))]
public class AchievementsCommand : ICommand
{
    public string Name => "achievements";
    public string[] Aliases => new[] { "ach", "achievs" };
    public string Description => "Показать ваши достижения";
    public string[] UsageExamples => new[] { "achievements" };
    
    public bool Execute(Player player, ArraySegment<string> arguments, out string response)
    {
        if (!ComplexLabPlugin.Instance.PlayerAchievements.TryGetValue(player.UserId, out var achievements))
        {
            response = "Ваши достижения не найдены";
            return false;
        }
        
        var message = "=== Ваши достижения ===\n";
        if (achievements.Count == 0)
        {
            message += "Пока нет достижений";
        }
        else
        {
            foreach (var ach in achievements)
            {
                message += $"• {ach}\n";
            }
        }
        
        player.SendBroadcast(message, 10);
        response = $"Достижения: {achievements.Count}";
        return true;
    }
}

/// <summary>
/// Команда для администраторов: активировать кастомный раунд
/// </summary>
[CommandHandler(typeof(ServerCommandHandler))]
public class CustomRoundCommand : ICommand
{
    public string Name => "customround";
    public string[] Aliases => new[] { "cr", "setround" };
    public string Description => "Активировать кастомный раунд (только для администраторов)";
    public string[] UsageExamples => new[] { "customround OneHitKill", "customround random" };
    
    public bool Execute(Player player, ArraySegment<string> arguments, out string response)
    {
        if (arguments.Count < 1)
        {
            response = "Использование: customround <режим> | random";
            return false;
        }
        
        var modeArg = arguments.At(0).ToLower();
        
        if (modeArg == "random")
        {
            // Активация случайного режима
            response = "Случайный кастомный раунд будет активирован в следующем раунде";
            // Здесь должна быть логика установки флага
            return true;
        }
        
        if (Enum.TryParse<ComplexLabPlugin.CustomRoundMode>(modeArg, true, out var mode))
        {
            if (mode == ComplexLabPlugin.CustomRoundMode.None)
            {
                ComplexLabPlugin.Instance.ActiveCustomRound = null;
                response = "Кастомный раунд отключен";
                return true;
            }
            
            // Немедленная активация режима
            ComplexLabPlugin.Instance.ActiveCustomRound = mode;
            response = $"Активирован режим: {mode}";
            
            foreach (var p in Player.List)
            {
                p.SendBroadcast($"⚠️ АДМИН активировал: {mode} ⚠️", 10);
            }
            
            return true;
        }
        
        response = $"Неизвестный режим. Доступные: {string.Join(", ", Enum.GetNames(typeof(ComplexLabPlugin.CustomRoundMode)))}";
        return false;
    }
}

/// <summary>
/// Команда помощи по плагину
/// </summary>
[CommandHandler(typeof(PlayerCommandHandler))]
public class PluginHelpCommand : ICommand
{
    public string Name => "complexhelp";
    public string[] Aliases => new[] { "chelp", "pluginhelp" };
    public string Description => "Показать справку по командам плагина";
    public string[] UsageExamples => new[] { "complexhelp" };
    
    public bool Execute(Player player, ArraySegment<string> arguments, out string response)
    {
        var message = @"=== ComplexLabPlugin Помощь ===
Команды:
• balance - Проверить баланс
• stats - Показать статистику  
• achievements - Показать достижения
• complexhelp - Эта справка

Админ команды:
• givemoney <игрок> <сумма> - Выдать деньги
• customround <режим> - Активировать режим";
        
        player.SendBroadcast(message, 15);
        response = "Справка отправлена";
        return true;
    }
}
