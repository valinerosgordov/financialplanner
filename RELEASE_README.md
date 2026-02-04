# Release Build Instructions

## Quick Start

### Option 1: Framework-Dependent (Recommended)
Запускает `publish-release.bat` - создаёт легковесный exe (~50MB), требует .NET 8 Runtime на целевом компьютере.

```bash
.\publish-release.bat
```

### Option 2: Self-Contained
Запускает `publish-self-contained.bat` - создаёт полный exe со всеми зависимостями (~150MB), не требует установки .NET.

```bash
.\publish-self-contained.bat
```

## Output Location
```
bin\Release\net8.0-windows\win-x64\publish\NexusFinance.exe
```

## Adding Custom Icon

### Method 1: Replace app.ico
1. Создайте или скачайте иконку в формате `.ico` (рекомендуется 256x256)
2. Переименуйте в `app.ico`
3. Поместите в корень проекта: `c:\SideProjects\financialplanner\app.ico`
4. Пересоберите проект

### Method 2: Online Icon Generator
1. Создайте PNG изображение (256x256 или 512x512)
2. Конвертируйте в ICO: https://convertio.co/png-ico/
3. Сохраните как `app.ico` в корне проекта

### Method 3: Use Visual Studio
1. Project → Properties → Application → Icon
2. Browse и выберите `.ico` файл

## Distribution

### Framework-Dependent Build
**Pros:**
- Малый размер (~50MB)
- Быстрые обновления
- Меньше использует диск

**Cons:**
- Требует .NET 8 Desktop Runtime на целевом ПК
- Пользователь должен установить: https://dotnet.microsoft.com/download/dotnet/8.0

### Self-Contained Build
**Pros:**
- Работает на любом Windows без установки .NET
- Один файл, один клик

**Cons:**
- Больший размер (~150MB)
- Каждое обновление включает весь runtime

## Manual Build Commands

### Framework-Dependent
```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

### Self-Contained
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Version Management

Обновите версию в `NexusFinance.csproj`:
```xml
<AssemblyVersion>1.0.0.0</AssemblyVersion>
<FileVersion>1.0.0.0</FileVersion>
<Version>1.0.0</Version>
```

## Troubleshooting

### "Application requires .NET Runtime"
→ Используйте `publish-self-contained.bat` или установите .NET 8 Runtime

### "Icon not showing"
→ Убедитесь что `app.ico` находится в корне проекта и пересоберите

### Large file size
→ Framework-dependent build всегда меньше, но требует .NET Runtime
