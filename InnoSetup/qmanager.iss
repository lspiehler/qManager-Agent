#define MyAppName "Print Management"
#define MyAppVersion "1.0"
#define MyAppPublisher "Sapphire Health"
#define MyAppURL "https://sapphirehealth.org/"
#define MyAppExeName "PrintManagement.exe"
#define SvcName "Print Management"
; #define MyAppAssocName MyAppName + " File"
; #define MyAppAssocExt ".myp"
; #define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
AppId={{89DEAD3D-AAB5-47FE-9C12-3210E9848A2D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppPublisher}\{#MyAppName}
UninstallDisplayIcon={app}\PrintManagement.ico
UninstallDisplayName={#MyAppName}
; DefaultGroupName=Print Management
; OutputDir=Output\Installer
; OutputBaseFilename=MyAppInstaller
; Compression=lzma
ArchitecturesAllowed=x64compatible
; "ArchitecturesInstallIn64BitMode=x64compatible" requests that the
; install be done in "64-bit mode" on x64 or Windows 11 on Arm,
; meaning it should use the native 64-bit Program Files directory and
; the 64-bit view of the registry.
ArchitecturesInstallIn64BitMode=x64compatible
ChangesAssociations=yes
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only).
;PrivilegesRequired=lowest
OutputBaseFilename=PrintManagementInstall
SolidCompression=yes
WizardStyle=modern
SetupIconFile=D:\a\qManager-Agent\qManager-Agent\PrintManagement\include\PrintManagement.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

; [Tasks]
; Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Source: "C:\Program Files (x86)\Inno Setup 6\Examples\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\a\qManager-Agent\qManager-Agent\PrintManagement\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "D:\a\qManager-Agent\qManager-Agent\PrintManagement\include\PrintManagement.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\a\qManager-Agent\qManager-Agent\PrintManagement\include\setprinter.exe"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
; Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{group}\Uninstall Print Management"; Filename: "{uninstallexe}"

[Code]

var
  qmanagerhost: string;
  qmanagergroups: string;
  qmanagercert: string;
  QManagerPage: TInputQueryWizardPage;
  ValuesInitialized: Boolean;
  SuppressMsgBoxes: Boolean;

function HasCmdLineSwitch(const Switch: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 1 to ParamCount do
  begin
    if CompareText(ParamStr(I), Switch) = 0 then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

function IsSilentInstall: Boolean;
begin
  Result := HasCmdLineSwitch('/SILENT') or HasCmdLineSwitch('/VERYSILENT');
end;

function IsSuppressMsgBoxes: Boolean;
begin
  Result := IsSilentInstall and HasCmdLineSwitch('/SUPPRESSMSGBOXES');
end;

function GetCmdLineArg(const ParamName: string): string;
var
  I: Integer;
  Arg, Key, Value: string;
  EqualPos: Integer;
begin
  Result := '';
  for I := 1 to ParamCount do
  begin
    Arg := ParamStr(I);
    if (Length(Arg) > 0) and ((Arg[1] = '/') or (Arg[1] = '-')) then
      Delete(Arg, 1, 1);

    EqualPos := Pos('=', Arg);
    if EqualPos > 0 then
    begin
      Key := Copy(Arg, 1, EqualPos - 1);
      Value := Copy(Arg, EqualPos + 1, MaxInt);

      if CompareText(Key, ParamName) = 0 then
      begin
        Result := Value;
        Exit;
      end;
    end;
  end;
end;

procedure InitializeWizard;
begin
  qmanagerhost := GetCmdLineArg('host');
  qmanagergroups := GetCmdLineArg('groups');
  qmanagercert := GetCmdLineArg('cert');
  SuppressMsgBoxes := IsSuppressMsgBoxes();

  QManagerPage := CreateInputQueryPage(wpWelcome,
    'qManager Agent Installation',
    'Please enter the required configuration values below.',
    'Setup needs some information to complete the installation.');

  QManagerPage.Add('qManager Server and Port:', False);
  QManagerPage.Add('qManager Groups (comma-separated):', False);
  QManagerPage.Add('qManager Certificate:', False);

  ValuesInitialized := False;
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if (CurPageID = QManagerPage.ID) and (not ValuesInitialized) then
  begin
    if qmanagerhost <> '' then
      QManagerPage.Values[0] := qmanagerhost;
    if qmanagergroups <> '' then
      QManagerPage.Values[1] := qmanagergroups;
    if qmanagercert <> '' then
      QManagerPage.Values[2] := qmanagercert;
    ValuesInitialized := True;
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;

  if CurPageID = QManagerPage.ID then
  begin
    qmanagerhost := QManagerPage.Values[0];
    qmanagergroups := QManagerPage.Values[1];
    qmanagercert := QManagerPage.Values[2];

    if qmanagerhost = '' then
    begin
      if not SuppressMsgBoxes then
      begin
        MsgBox('Please enter the QManager Server FQDN.', mbError, MB_OK);
        Result := False;
        Exit;
      end;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  binPath: string;
  resultCode: Integer;
  svcName: string;
begin
  if CurStep = ssPostInstall then
  begin
    svcName := '{#SvcName}';
    
    binPath := Format('\"%s\%s\" --server \"%s\" --groups \"%s\" --cert \"%s\"', [ExpandConstant('{app}'), '{#MyAppExeName}', qmanagerhost, qmanagergroups, qmanagercert]);

    // Try to delete existing service first (optional)
    // Exec('sc.exe', 'delete ' + svcName, '', SW_HIDE, ewWaitUntilTerminated, resultCode);

    // debug sc command args
    // if not SuppressMsgBoxes then
      // MsgBox(Format('sc create "%s" binPath= "%s" start= auto', [svcName, binPath]), mbError, MB_OK);

    // Create the service
    Exec('sc.exe', Format('create "%s" binPath= "%s" start= auto', [svcName, binPath]), '', SW_HIDE, ewWaitUntilTerminated, resultCode);

    if resultCode <> 0 then
    begin
      if not SuppressMsgBoxes then
        MsgBox('Failed to create service. Please run setup as administrator or check parameters.', mbError, MB_OK);
    end
    else
    begin
      // Set the service description
      Exec('sc.exe', Format('description "%s" "qManager print Management Service"', [svcName]), '', SW_HIDE, ewWaitUntilTerminated, resultCode);
      // Start the service
      Exec('sc.exe', Format('start "%s"', [svcName]), '', SW_HIDE, ewWaitUntilTerminated, resultCode);
    end;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
  svcName: string;
begin
  if CurUninstallStep = usUninstall then
  begin
    svcName := '{#SvcName}';
    // Stop the service before deleting it
    Exec('sc.exe', Format('stop "%s"', [svcName]), '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    // Delete the service
    Exec('sc.exe', Format('delete "%s"', [svcName]), '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    if ResultCode <> 0 then
      if not SuppressMsgBoxes then
        MsgBox(Format('Failed to remove the %s service. You may need to remove it manually.', [svcName]), mbError, MB_OK);
  end;
end;
