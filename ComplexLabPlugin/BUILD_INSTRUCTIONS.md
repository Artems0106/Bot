# Инструкция по компиляции ComplexLabPlugin

## Предварительные требования

1. **Установите .NET SDK** (версия 6.0 или выше):
   - Windows: https://dotnet.microsoft.com/download
   - Linux: `sudo apt-get install dotnet-sdk-6.0` или используйте snap

2. **Получите необходимые DLL библиотеки** из вашего сервера SCP:SL:
   - LabApi.dll
   - Assembly-CSharp.dll
   - UnityEngine.CoreModule.dll
   - UnityEngine.PhysicsModule.dll
   - Mirror.dll

## Шаг 1: Размещение библиотек

Скопируйте все необходимые DLL файлы в папку `libs/`:

```bash
# Пример для Linux сервера
cp /path/to/server/LabApi.dll /workspace/ComplexLabPlugin/libs/
cp /path/to/server/Assembly-CSharp.dll /workspace/ComplexLabPlugin/libs/
cp /path/to/server/UnityEngine.CoreModule.dll /workspace/ComplexLabPlugin/libs/
cp /path/to/server/UnityEngine.PhysicsModule.dll /workspace/ComplexLabPlugin/libs/
cp /path/to/server/Mirror.dll /workspace/ComplexLabPlugin/libs/
```

## Шаг 2: Проверка структуры проекта

Убедитесь, что структура папок выглядит так:

```
ComplexLabPlugin/
├── src/
│   ├── ComplexLabPlugin.cs
│   ├── Config.cs
│   ├── Translation.cs
│   └── Commands.cs
├── libs/
│   ├── LabApi.dll ✓
│   ├── Assembly-CSharp.dll ✓
│   ├── UnityEngine.CoreModule.dll ✓
│   ├── UnityEngine.PhysicsModule.dll ✓
│   └── Mirror.dll ✓
├── config/
├── ComplexLabPlugin.csproj
└── README.md
```

## Шаг 3: Компиляция

### Вариант A: Компиляция через командную строку

```bash
cd /workspace/ComplexLabPlugin

# Отладочная версия
dotnet build --configuration Debug

# Или релизная версия
dotnet build --configuration Release
```

### Вариант B: Компиляция через Visual Studio / Rider

1. Откройте файл `ComplexLabPlugin.csproj` в вашей IDE
2. Выберите конфигурацию (Debug/Release)
3. Нажмите Build (Ctrl+Shift+B)

## Шаг 4: Получение DLL файла

После успешной компиляции DLL файл будет находиться:

- **Debug**: `bin/Debug/net48/ComplexLabPlugin.dll`
- **Release**: `bin/Release/net48/ComplexLabPlugin.dll`

## Шаг 5: Установка на сервер

```bash
# Скопируйте DLL в папку плагинов вашего сервера
cp bin/Release/net48/ComplexLabPlugin.dll /path/to/your/server/plugins/

# Или для Windows сервера
copy bin\Release\net48\ComplexLabPlugin.dll C:\path\to\server\plugins\
```

## Решение проблем

### Ошибка: "Reference hint path не найден"

Убедитесь, что все DLL файлы находятся в папке `libs/` и имена файлов совпадают с указанными в `.csproj`.

### Ошибка: "Namespace 'LabApi' not found"

Проверьте, что вы используете правильную версию LabAPI (1.1.6 или совместимую).

### Ошибка компиляции на Linux

Если возникают проблемы с путями, попробуйте:

```bash
# Очистить и пересобрать
dotnet clean
dotnet restore
dotnet build --configuration Release
```

### Ошибка: ".NET Framework 4.8 не доступен"

На Linux используйте mono или измените target framework в `.csproj`:

```xml
<TargetFramework>net6.0</TargetFramework>
```

Но это может потребовать изменений в коде для совместимости.

## Проверка сборки

После компиляции проверьте DLL файл:

```bash
# Проверка размера файла (должен быть > 10 KB)
ls -lh bin/Release/net48/ComplexLabPlugin.dll

# Проверка зависимостей (требуется ILSpy или dnSpy)
```

## Автоматическая сборка (опционально)

Создайте скрипт для автоматической сборки:

```bash
#!/bin/bash
# build.sh

cd /workspace/ComplexLabPlugin

echo "Cleaning..."
dotnet clean

echo "Restoring packages..."
dotnet restore

echo "Building Release..."
dotnet build --configuration Release

if [ $? -eq 0 ]; then
    echo "Build successful!"
    echo "DLL location: bin/Release/net48/ComplexLabPlugin.dll"
else
    echo "Build failed!"
    exit 1
fi
```

Сделайте его исполняемым:
```bash
chmod +x build.sh
./build.sh
```

## Обновление плагина

Для обновления существующего плагина:

1. Остановите сервер
2. Замените DLL файл в папке plugins
3. Запустите сервер заново

Конфигурация и данные сохранятся автоматически.
