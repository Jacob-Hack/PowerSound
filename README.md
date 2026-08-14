# PowerSound

PowerSound is a simple Windows 11 C#/.NET tray app for power-related sounds and notifications.

## Download

Download the latest release from:

https://github.com/Jacob-Hack/PowerSound/releases

Most people should download `PowerSound-Setup.exe` and run it. The installer uses the normal Windows Program Files location and may ask for administrator permission.

Optional portable version: advanced users can download `PowerSound-Portable.zip`, unzip it, and run `PowerSound.exe` without installing.

Windows may show a SmartScreen warning because PowerSound is not code-signed.

## Features

- Runs as a tray app.
- Detects AC power connect and disconnect events.
- Includes battery low, critical, emergency, and fully charged alerts.
- Lets each battery alert play a sound, show a Windows notification, both, or neither.
- Uses configurable battery thresholds for low, critical, and emergency alerts.
- Includes built-in default sounds for AC power changes and battery alerts.
- Lets the user choose custom `.wav` files.
- Includes test buttons for both sounds.
- Saves settings to `%APPDATA%\PowerSound\settings.json`.
- Copies selected custom sounds to `%APPDATA%\PowerSound\Sounds` so they keep working if the original file moves.
- Can start automatically with Windows through the current user's Run registry key.
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

Run `PowerSound.exe`. Double-click the tray icon or open its tray menu to change settings, test sounds, or exit.
