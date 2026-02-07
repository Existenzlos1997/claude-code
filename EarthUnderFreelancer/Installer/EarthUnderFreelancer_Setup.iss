; ============================================================
; EarthUnderFreelancer - Inno Setup Script
; Professional Windows Installer
; ============================================================

#define MyAppName "EarthUnderFreelancer"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "EarthUnder Studios"
#define MyAppURL "https://earthunderfreelancer.com"
#define MyAppExeName "EarthUnderFreelancer.exe"
#define MyAppAssocName MyAppName + " Save File"
#define MyAppAssocExt ".eufsave"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
AppId={{A8B7C6D5-E4F3-2A1B-9C8D-7E6F5A4B3C2D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/support
AppUpdatesURL={#MyAppURL}/updates
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
; Remove the following line to run in administrative install mode
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=Output
OutputBaseFilename=EarthUnderFreelancer_Setup_v{#MyAppVersion}
SetupIconFile=..\Assets\StreamingAssets\Icons\game_icon.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
WizardImageFile=..\Assets\StreamingAssets\Installer\wizard_image.bmp
WizardSmallImageFile=..\Assets\StreamingAssets\Installer\wizard_small.bmp
UninstallDisplayIcon={app}\{#MyAppExeName}
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=EarthUnderFreelancer - The Ultimate Flight Combat MMORPG
VersionInfoCopyright=Copyright (C) 2024 {#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
DisableWelcomePage=no
DisableDirPage=no
DisableProgramGroupPage=yes
LicenseFile=..\Documentation\LICENSE.txt
InfoBeforeFile=..\Documentation\README_INSTALL.txt

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode
Name: "associatefiles"; Description: "Associate .eufsave files with {#MyAppName}"; GroupDescription: "File Associations:"; Flags: unchecked

[Files]
; Main game executable and Unity files
Source: "..\Builds\Windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Builds\Windows\EarthUnderFreelancer_Data\*"; DestDir: "{app}\EarthUnderFreelancer_Data"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Builds\Windows\MonoBleedingEdge\*"; DestDir: "{app}\MonoBleedingEdge"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Builds\Windows\UnityPlayer.dll"; DestDir: "{app}"; Flags: ignoreversion

; Launcher (optional)
Source: "..\Builds\Windows\Launcher\EUFLauncher.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Builds\Windows\Launcher\updater.exe"; DestDir: "{app}"; Flags: ignoreversion

; Documentation
Source: "..\Documentation\README.md"; DestDir: "{app}\Documentation"; Flags: ignoreversion
Source: "..\Documentation\BuildGuide.md"; DestDir: "{app}\Documentation"; Flags: ignoreversion
Source: "..\Documentation\Controls.pdf"; DestDir: "{app}\Documentation"; Flags: ignoreversion skipifsourcedoesntexist

; DirectX and Visual C++ Redistributables
Source: "..\Redist\vc_redist.x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "..\Redist\dxwebsetup.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall skipifsourcedoesntexist

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{#MyAppName} Launcher"; Filename: "{app}\EUFLauncher.exe"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Registry]
; File association
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue; Tasks: associatefiles
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey; Tasks: associatefiles
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"; Tasks: associatefiles
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""; Tasks: associatefiles

; App settings
Root: HKCU; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"
Root: HKCU; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"

[Run]
; Install Visual C++ Redistributable if needed
Filename: "{tmp}\vc_redist.x64.exe"; Parameters: "/quiet /norestart"; StatusMsg: "Installing Visual C++ Redistributable..."; Flags: waituntilterminated skipifnotsilent

; DirectX (optional)
Filename: "{tmp}\dxwebsetup.exe"; Parameters: "/Q"; StatusMsg: "Installing DirectX..."; Flags: waituntilterminated skipifnotsilent skipifdoesntexist

; Launch game after install
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}\Saves"
Type: filesandordirs; Name: "{app}\Cache"
Type: filesandordirs; Name: "{app}\Logs"
Type: dirifempty; Name: "{app}"

[Code]
// ============================================================
// Custom Pascal Script for Enhanced Installer
// ============================================================

var
  DownloadPage: TDownloadWizardPage;
  SystemCheckPage: TWizardPage;
  SystemCheckMemo: TMemo;

function GetSystemInfo: String;
var
  WinVer: TWindowsVersion;
  RAM: Int64;
begin
  GetWindowsVersionEx(WinVer);
  RAM := 0; // Would need WMI for actual RAM
  
  Result := 'System Information:' + #13#10 +
            '===================' + #13#10 +
            'Windows Version: ' + IntToStr(WinVer.Major) + '.' + IntToStr(WinVer.Minor) + #13#10 +
            'Build: ' + IntToStr(WinVer.Build) + #13#10 +
            'Architecture: x64' + #13#10#13#10 +
            'Minimum Requirements:' + #13#10 +
            '- Windows 10 (64-bit)' + #13#10 +
            '- 8 GB RAM' + #13#10 +
            '- GTX 1050 / RX 560 or better' + #13#10 +
            '- 20 GB free disk space' + #13#10 +
            '- Broadband internet connection' + #13#10#13#10 +
            'Recommended Requirements:' + #13#10 +
            '- Windows 11 (64-bit)' + #13#10 +
            '- 16 GB RAM' + #13#10 +
            '- RTX 2060 / RX 5700 or better' + #13#10 +
            '- 50 GB SSD space' + #13#10 +
            '- High-speed internet connection';
end;

procedure InitializeWizard;
begin
  // Create system check page
  SystemCheckPage := CreateCustomPage(wpWelcome, 'System Requirements', 'Check if your system meets the requirements');
  
  SystemCheckMemo := TMemo.Create(WizardForm);
  SystemCheckMemo.Parent := SystemCheckPage.Surface;
  SystemCheckMemo.Left := 0;
  SystemCheckMemo.Top := 0;
  SystemCheckMemo.Width := SystemCheckPage.SurfaceWidth;
  SystemCheckMemo.Height := SystemCheckPage.SurfaceHeight;
  SystemCheckMemo.ScrollBars := ssVertical;
  SystemCheckMemo.ReadOnly := True;
  SystemCheckMemo.Lines.Text := GetSystemInfo;
  SystemCheckMemo.Font.Name := 'Consolas';
  SystemCheckMemo.Font.Size := 9;
end;

function CheckDiskSpace: Boolean;
var
  FreeMB: Cardinal;
begin
  Result := True;
  FreeMB := GetSpaceOnDisk(ExpandConstant('{app}'), True, FreeMB);
  if FreeMB < 20480 then
  begin
    if MsgBox('Warning: Less than 20 GB of free disk space detected. The game may not install properly. Continue anyway?', 
              mbConfirmation, MB_YESNO) = IDNO then
      Result := False;
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  
  if CurPageID = wpSelectDir then
  begin
    Result := CheckDiskSpace;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Create default config if needed
    SaveStringToFile(ExpandConstant('{app}\config.ini'),
      '[Graphics]' + #13#10 +
      'Quality=High' + #13#10 +
      'Resolution=1920x1080' + #13#10 +
      'Fullscreen=True' + #13#10 +
      'VSync=True' + #13#10#13#10 +
      '[Audio]' + #13#10 +
      'MasterVolume=100' + #13#10 +
      'MusicVolume=80' + #13#10 +
      'SFXVolume=100' + #13#10#13#10 +
      '[Network]' + #13#10 +
      'Region=Auto' + #13#10 +
      'Language=Auto' + #13#10,
      False);
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  Result := '';
  NeedsRestart := False;
end;
