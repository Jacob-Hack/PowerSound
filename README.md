# PowerSound

PowerSound is a lightweight Windows 11 tray app that adds customizable sounds and optional notifications for charger and battery events.

Created by Jacob Hack.

## Download

For most users, download and run:

**[PowerSound-Setup.exe](https://github.com/Jacob-Hack/PowerSound/releases/latest/download/PowerSound-Setup.exe)**

PowerSound installs to the normal Windows Program Files location and includes everything needed to run.

Prefer not to install it? A portable ZIP is also available on the [latest release page](https://github.com/Jacob-Hack/PowerSound/releases/latest).

Windows may show a SmartScreen warning because PowerSound is not digitally signed yet.

## Features

- Plays distinct sounds when AC power is connected or disconnected.
- Includes Battery Low, Critical, Emergency, and Fully Charged alerts.
- Lets each battery alert play a sound, show a Windows notification, or both.
- Lets you customize Low, Critical, and Emergency battery thresholds.
- Includes built-in default sounds, with support for custom `.wav` files.
- Copies selected custom sounds into `%APPDATA%\PowerSound\Sounds` so they continue working if the original file is moved or deleted.
- Includes test buttons for each configurable sound.
- Can start automatically with Windows.
- Can check for updates automatically at startup or manually from the tray menu and Settings.
- Shows release notes before installing an update.
- Includes a Reset All Settings option.
- Uses standard Windows controls, keyboard navigation, and accessibility names for screen readers.

## Battery alerts

PowerSound includes four battery alerts:

- **Battery Low:** enabled by default at 20%.
- **Battery Critical:** enabled by default at 10%.
- **Battery Emergency:** enabled by default at 5%.
- **Battery Fully Charged:** disabled by default and triggers at 100% while connected to AC power.

Each alert can independently play a sound and show a Windows notification. Low, Critical, and Emergency alerts trigger once when the battery crosses the configured threshold and reset after the battery rises above that threshold or AC power is connected.

If Windows reports a large battery change, such as after waking from sleep below several thresholds, PowerSound uses only the most severe matching alert.

## Using PowerSound

After installation, PowerSound runs in the Windows notification area. Double-click the tray icon or open its context menu to access Settings, Check for Updates, or Exit.

Settings and custom sounds are stored under `%APPDATA%\PowerSound` so they remain separate from the installed program files.

## Release history

See [CHANGELOG.md](CHANGELOG.md) for notable changes in each release.

## Sound credits

The bundled default sounds were generated using ByteDance Seed Audio 1.0 via fal.ai.

## Build from source

PowerSound is built with C# and .NET 8 using Windows Forms.

Install the .NET 8 SDK, then run:

```powershell
dotnet build
```

To publish a self-contained Windows build:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

To build the Inno Setup installer locally, install Inno Setup 6, publish the app, then compile `Installer\PowerSound.iss`:

```powershell
dotnet publish PowerSound.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:DebugType=None /p:DebugSymbols=false -o publish\win-x64
iscc Installer\PowerSound.iss
```

GitHub Actions can also build the installer and portable ZIP automatically from the **Build release** workflow.
