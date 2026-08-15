# PowerSound

PowerSound is a lightweight Windows tray app that plays customizable sounds when your computer is plugged into or unplugged from power.

It also includes optional battery alerts, Windows notifications, customizable battery thresholds, and other power-related features.

Created by Jacob Hack.

## System requirements

PowerSound is intended for **64-bit Windows 10 and Windows 11**.

PowerSound has been tested on **Windows 11**. Windows 10 should also work, but has not yet been tested.

## Download

For most users, download and run:

**[PowerSound-Setup.exe](https://github.com/Jacob-Hack/PowerSound/releases/latest/download/PowerSound-Setup.exe)**

PowerSound installs to the normal Windows Program Files location.

Prefer not to install it? A portable ZIP is also available on the [latest release page](https://github.com/Jacob-Hack/PowerSound/releases/latest).

Windows may show a SmartScreen warning because PowerSound is not digitally signed yet.

## Core feature

PowerSound's main purpose is to provide immediate audio feedback when your computer's power connection changes.

- Plays a sound when AC power is connected.
- Plays a different sound when AC power is disconnected.
- Includes built-in sounds for both events.
- Lets you replace either sound with your own `.wav` file.
- Includes Test buttons so you can preview your selected sounds.
- Copies custom sounds into `%APPDATA%\PowerSound\Sounds` so they continue working if the original file is moved or deleted.

## Additional features

### Battery alerts

PowerSound can also alert you at important battery levels:

- **Battery Low:** enabled by default at 20%.
- **Battery Critical:** enabled by default at 10%.
- **Battery Emergency:** enabled by default at 5%.
- **Battery Fully Charged:** optional and disabled by default. Alerts at 100% while connected to power.

Low, Critical, and Emergency thresholds can be customized.

Each battery alert can play a sound, show a Windows notification, or both. Each alert can also use its own custom `.wav` sound.

Low, Critical, and Emergency alerts trigger once when the battery reaches their configured threshold rather than repeating at every lower percentage. They reset after the battery rises above the threshold or AC power is connected.

The Fully Charged alert triggers once per charge cycle.

If Windows reports a large battery change, such as after waking from sleep below several thresholds, PowerSound uses only the most severe matching alert.

### Startup and updates

- PowerSound starts with Windows by default on fresh installs. This can be turned off in Settings.
- Automatic update checking at startup is enabled by default.
- You can manually check for updates at any time.
- When an update is available, PowerSound shows the release notes and can download and launch the latest installer.
- Settings can be reset to their defaults at any time.

## Release history

See [CHANGELOG.md](CHANGELOG.md) for notable changes in each release.

## Sound credits

The bundled default sounds were generated using ByteDance Seed Audio 1.0.

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
