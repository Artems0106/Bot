using System.ComponentModel;
using LabApi.API.Features.Configs;

namespace ComplexLabPlugin;

/// <summary>
/// Конфигурация плагина ComplexLabPlugin
/// </summary>
public class Config : PluginConfig
{
    [Description("URL Discord webhook для отправки уведомлений")]
    public string DiscordWebhookUrl { get; set; } = "";

    [Description("Минимальное количество игроков для активации кастомных раундов")]
    public int MinPlayersForCustomRounds { get; set; } = 5;

    [Description("Шанс активации кастомного раунда (0.0 - 1.0)")]
    public float CustomRoundChance { get; set; } = 0.3f;

    [Description("Стартовые деньги для игроков")]
    public decimal StartingMoney { get; set; } = 100;

    [Description("Деньги за убийство")]
    public decimal MoneyPerKill { get; set; } = 25;

    [Description("Деньги за побег")]
    public decimal MoneyPerEscape { get; set; } = 500;

    [Description("Деньги за участие в раунде")]
    public decimal MoneyPerRound { get; set; } = 10;

    [Description("Дополнительные деньги за победу в раунде")]
    public decimal MoneyPerWin { get; set; } = 50;

    [Description("Приветственное сообщение на сервере")]
    public string ServerWelcomeMessage { get; set; } = "Добро пожаловать на наш сервер SCP:SL!";

    [Description("Включить античит систему")]
    public bool EnableAntiCheat { get; set; } = true;

    [Description("Включить систему достижений")]
    public bool EnableAchievements { get; set; } = true;

    [Description("Включить экономику")]
    public bool EnableEconomy { get; set; } = true;

    [Description("Интервал авто-сохранения в секундах")]
    public int AutoSaveInterval { get; set; } = 300;

    [Description("Разрешить кастомные раунды")]
    public bool EnableCustomRounds { get; set; } = true;
}
