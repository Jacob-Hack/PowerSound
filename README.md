# PowerSound

PowerSound is a simple Windows 11 C#/.NET tray app that plays one sound when AC power is connected and another sound when AC power is disconnected.

## Download

Download the latest release from:

https://github.com/Jacob-Hack/PowerSound/releases

Most people should download `PowerSound-Setup.exe` and run it.

For portable use, download `PowerSound-for-Windows.zip`, unzip it, and run `PowerSound.exe`.

Windows may show a SmartScreen warning because PowerSound is not code-signed.

## Features

- Runs as a tray app.
- Detects AC power connect and disconnect events.
- Includes two built-in default sounds.
- Lets the user choose custom `.wav` files.
- Includes test buttons for both sounds.
- Saves settings to `%APPDATA%\PowerSound\settings.json`.
- Can start automatically with Windows through the current user's Run registry key.
- Uses standard Windows controls with labels, keyboard access keys, and accessibility names for screen readers.

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

To build the installer:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:DebugType=None /p:DebugSymbols=false -o Installer\Payload
dotnet publish Installer\PowerSound.Installer.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:DebugType=None /p:DebugSymbols=false
```

## Use

Run `PowerSound.exe`. Double-click the tray icon or open its tray menu to change settings, test sounds, or exit.
