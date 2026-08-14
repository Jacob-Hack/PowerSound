# PowerSound

PowerSound is a simple Windows 11 tray app that plays sounds and shows optional notifications for power events, like plugging in your charger, unplugging it, or reaching a low battery level.

Created by Jacob Hack.

## Download

Most people should download and run the installer:

**[Download PowerSound-Setup.exe](https://github.com/Jacob-Hack/PowerSound/releases/latest/download/PowerSound-Setup.exe)**

The installer uses the normal Windows Program Files location and may ask for administrator permission.

Advanced users can also download the optional portable ZIP from the [latest release page](https://github.com/Jacob-Hack/PowerSound/releases/latest).

Windows may show a SmartScreen warning because PowerSound is not code-signed.

## Features

- Runs as a tray app.
- Uses a custom PowerSound app, tray, and installer icon.
- Detects AC power connect and disconnect events.
- Includes battery low, critical, emergency, and fully charged alerts.
- Lets each battery alert play a sound, show a Windows notification, both, or neither.
- Uses configurable battery thresholds for low, critical, and emergency alerts.
- Includes built-in default sounds for AC power changes and battery alerts.
- Lets the user choose custom `.wav` files.
- Includes test buttons for each configurable sound.
- Saves settings to `%APPDATA%\PowerSound\settings.json`.
- Copies selected custom sounds to `%APPDATA%\PowerSound\Sounds` so they keep working if the original file moves.
- Can start automatically with Windows through the current user's Run registry key.
- Can check GitHub Releases for updates on startup or on demand, show release notes, and launch the latest installer.
- Uses standard Windows controls with labels, keyboard access keys, and accessibility names for screen readers.

## Battery alerts

PowerSound includes these battery alerts:

- Battery Low: enabled by default at 20%.
- Battery Critical: enabled by default at 10%.
- Battery Emergency: enabled by default at 5%.
- Battery Fully Charged: disabled by default and triggers at 100% while connected to AC power.

Each battery alert can independently play a sound and show a Windows notification. Notifications are shown through the standard Windows notification area and follow Windows notification behavior, including Focus Assist / Do Not Disturb.

Low, Critical, and Emergency alerts trigger once when the battery crosses down to the configured threshold. They reset after the battery rises above the configured threshold or AC power is connected. If Windows reports a large battery change, such as after waking from sleep below multiple thresholds, PowerSound shows only the most severe matching alert.

## Sound credits

The bundled default sounds were generated using ByteDance Seed Audio 1.0 via fal.ai.

## Build

Install the .NET 8 SDK, then run:

```powershell
dotnet build
```

To publish a single Windows executable:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

The self-contained build does not require the .NET runtime or SDK on the target computer. Share the contents of:

```text
bin\Release\net8.0-windows\win-x64\publish
```

To build the Inno Setup installer locally, install Inno Setup 6, publish the app, then compile `Installer\PowerSound.iss`:

```powershell
dotnet publish PowerSound.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:DebugType=None /p:DebugSymbols=false -o publish\win-x64
iscc Installer\PowerSound.iss
```

GitHub Actions can also build the installer and optional portable ZIP automatically from the **Build release** workflow.

## Use

After installing, PowerSound runs from the Windows tray. Double-click the tray icon or open its tray menu to change settings, test sounds, check for updates, or exit.
