#define MyAppName "PowerSound"
#define MyAppVersion GetEnv("POWERSOUND_VERSION")
#if MyAppVersion == ""
  #define MyAppVersion "0.1.0"
#endif
#define MyAppPublisher "PowerSound Project"
#define MyAppExeName "PowerSound.exe"
#define SourceDir "..\publish\win-x64"
#define OutputDir "..\artifacts"

[Setup]
AppId={{B14308D4-6330-443A-BF9D-AE22F9AD63E7}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
AppPublisherURL=https://github.com/Jacob-Hack/PowerSound
AppSupportURL=https://github.com/Jacob-Hack/PowerSound/issues
AppUpdatesURL=https://github.com/Jacob-Hack/PowerSound/releases
OutputDir={#OutputDir}
OutputBaseFilename=PowerSound-Setup
SetupIconFile=..\Assets\PowerSound.ico
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
UninstallDisplayIcon={app}\{#MyAppExeName}
CloseApplications=yes
CloseApplicationsFilter={#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[InstallDelete]
Type: files; Name: "{app}\{#MyAppExeName}"

[UninstallDelete]
Type: files; Name: "{app}\{#MyAppExeName}"

[UninstallRun]
Filename: "{cmd}"; Parameters: "/c taskkill /IM {#MyAppExeName} /F /T"; Flags: runhidden waituntilterminated; RunOnceId: "StopPowerSound"

[Code]
procedure StopPowerSound();
var
  ResultCode: Integer;
begin
  Exec(ExpandConstant('{cmd}'), '/c taskkill /IM {#MyAppExeName} /F /T', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
end;

procedure DeleteStartupEntry();
begin
  RegDeleteValue(HKEY_CURRENT_USER, 'Software\Microsoft\Windows\CurrentVersion\Run', '{#MyAppName}');
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssInstall then
    StopPowerSound();
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then
  begin
    StopPowerSound();
    DeleteStartupEntry();
  end;
end;

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
