# ComplexLabPlugin для SCP: Secret Laboratory

Самый сложный и функциональный плагин для SCP:SL на LabAPI 1.1.6

## 📋 Возможности

### Основные системы:
- **Динамическая система событий** - полная интеграция с событиями LabAPI
- **Кастомные раунды** - 10 уникальных режимов игры
- **Система экономики** - внутренняя валюта, магазины (расширяемо)
- **Система достижений** - награды за различные действия
- **Статистика игроков** - подробный трекинг действий
- **Discord интеграция** - отправка уведомлений через webhook
- **Античит система** - базовая защита от читеров
- **Продвинутые команды** - система команд с правами доступа

### Кастомные режимы раундов:
1. **OneHitKill** - все умирают с одного удара
2. **OnlyShotguns** - только дробовики
3. **LowGravity** - низкая гравитация
4. **SpeedRun** - ускоренный режим
5. **ZombieInfection** - зомби инфекция
6. **GunGame** - гонка вооружений
7. **Hardcore** - хардкор режим
8. **InvisibleSCPs** - невидимые SCP
9. **ExplosiveWeapons** - взрывное оружие

## 📦 Установка

### Требования:
- SCP: Secret Laboratory сервер
- LabAPI 1.1.6 или выше
- .NET Framework 4.8

### Шаги установки:

1. **Скомпилируйте плагин:**
   ```bash
   cd /workspace/ComplexLabPlugin
   dotnet build --configuration Release
   ```

2. **Скопируйте DLL файл:**
   ```bash
   cp bin/Release/net48/ComplexLabPlugin.dll /path/to/your/server/plugins/
   ```

3. **Настройте конфигурацию:**
   После первого запуска в папке configs появится файл `ComplexLabPlugin_config.yml`
   
   Пример конфигурации:
   ```yaml
   DiscordWebhookUrl: ""
   MinPlayersForCustomRounds: 5
   CustomRoundChance: 0.3
   StartingMoney: 100
   MoneyPerKill: 25
   MoneyPerEscape: 500
   MoneyPerRound: 10
   MoneyPerWin: 50
   ServerWelcomeMessage: "Добро пожаловать на наш сервер SCP:SL!"
   EnableAntiCheat: true
   EnableAchievements: true
   EnableEconomy: true
   AutoSaveInterval: 300
   EnableCustomRounds: true
   ```

4. **Перезапустите сервер**

## 🎮 Команды

### Игровые команды:
- `.balance` или `.bal` - проверить баланс
- `.stats` - показать статистику
- `.achievements` или `.ach` - показать достижения
- `.complexhelp` - справка по командам

### Админ команды:
- `.givemoney <игрок> <сумма>` - выдать деньги игроку
- `.customround <режим>` - активировать кастомный раунд
- `.customround random` - случайный режим
- `.customround None` - отключить кастомные раунды

## 🔧 Разработка

### Структура проекта:
```
ComplexLabPlugin/
├── src/
│   ├── ComplexLabPlugin.cs    # Основной класс плагина
│   ├── Config.cs              # Конфигурация
│   ├── Translation.cs         # Переводы
│   └── Commands.cs            # Команды
├── libs/                      # Библиотеки (нужно добавить вручную)
│   ├── LabApi.dll
│   ├── Assembly-CSharp.dll
│   ├── UnityEngine.CoreModule.dll
│   └── Mirror.dll
├── config/                    # Конфигурационные файлы
└── ComplexLabPlugin.csproj    # Проект MSBuild
```

### Добавление библиотек:
Вам нужно скопировать следующие DLL из вашего сервера в папку `libs/`:
- LabApi.dll
- Assembly-CSharp.dll  
- UnityEngine.CoreModule.dll
- UnityEngine.PhysicsModule.dll
- Mirror.dll

### Компиляция:
```bash
dotnet build --configuration Release
```

### Отладка:
```bash
dotnet build --configuration Debug
```

## 📊 API для других плагинов

Плагин предоставляет статический экземпляр для доступа из других плагинов:

```csharp
// Получить экземпляр
var plugin = ComplexLabPlugin.Instance;

// Получить баланс игрока
var balance = plugin.PlayerEconomy[userId];

// Добавить деньги
plugin.AddMoney(userId, 100);

// Получить статистику
var stats = plugin.PlayerStatistics[userId];

// Проверить достижения
var achievements = plugin.PlayerAchievements[userId];
```

## ⚙️ Настройка

### Discord Webhook:
Для включения уведомлений в Discord:
1. Создайте webhook на вашем Discord сервере
2. Скопируйте URL webhook
3. Вставьте в конфиг: `DiscordWebhookUrl: "https://discord.com/api/webhooks/..."`

### Баланс экономики:
Настройте значения в конфиге для баланса:
- `StartingMoney` - стартовый капитал
- `MoneyPerKill` - награда за убийство
- `MoneyPerEscape` - награда за побег
- `MoneyPerRound` - участие в раунде
- `MoneyPerWin` - победа в раунде

### Кастомные раунды:
- `MinPlayersForCustomRounds` - минимальное кол-во игроков
- `CustomRoundChance` - шанс активации (0.0-1.0)
- `EnableCustomRounds` - включение/выключение

## 🛡️ Античит

Встроенная античит система проверяет:
- Подозрительные перемещения (телепортация)
- Ненормальную скорость движения
- Другие аномалии

При обнаружении нарушений игрок получает kick.

## 📝 Лицензия

Этот плагин создан в образовательных целях. Используйте на свой страх и риск.

## 🤝 Поддержка

Для вопросов и предложений создавайте issues в репозитории.

## 📈 Планы развития

- [ ] Добавить магазин предметов
- [ ] Интеграция с MySQL для хранения данных
- [ ] Больше кастомных режимов
- [ ] Расширенная Discord интеграция
- [ ] Веб-панель управления
- [ ] Система кланов
- [ ] Турнирный режим
