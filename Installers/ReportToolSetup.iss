; This file is a script that allows to build the OrgSyncer instalation package
; To generate the installer, define the variable MyAppSourceDir MUST point to the Directory where the dll's should be copied from
; The script may be executed from the console-mode compiler - iscc "c:\isetup\samples\my script.iss" or from the Inno Setup Compiler UI
#define AppId "{{99fc06ec-8285-4bd1-a018-58c74fca4987}"
#define AppSourceDir "\\VBOXSVR\brian\projects\organisation-synchronisation-component\ReportTool\bin\Release\"
#define AppName "ReportTool"
#define AppVersion "1.0"
#define AppPublisher "Digital Identity"
#define AppURL "http://digital-identity.dk/"
#define AppExeName "ReportTool.exe"

[Setup]
AppId={#AppId}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={pf}\{#AppPublisher}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
SetupLogging=yes
OutputBaseFilename=ReportToolSetup
Compression=lzma
SolidCompression=yes
SourceDir={#AppSourceDir}
OutputDir=..\..\..\Installers
SetupIconFile={#AppSourceDir}\..\..\..\Resources\di.ico
UninstallDisplayIcon={#AppSourceDir}\..\..\..\Resources\di.ico

[Registry]
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync";
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; ValueType: string; ValueName: "ClientCertThumbprint"; ValueData: "xx xx xx xx xx xx xx xx xx xx xx"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; ValueType: string; ValueName: "DBConnectionString"; ValueData: "SQLITE"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; ValueType: string; ValueName: "DisableRevocationCheck"; ValueData: "true"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; ValueType: string; ValueName: "Environment"; ValueData: "TEST"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; ValueType: string; ValueName: "LogRequestResponse"; ValueData: "false"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; ValueType: string; ValueName: "Municipality"; ValueData: "xxxxxxxx"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; ValueType: string; ValueName: "OrganisationUUID"; ValueData: "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\STSOrgSync"; ValueType: string; ValueName: "UseSSL"; ValueData: "false"; Flags: createvalueifdoesntexist

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "BusinessLayer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "IntegrationLayer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Log.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "ReportTool.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "ReportTool.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Web.Razor.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "RazorEngine.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "templates/template.html"; DestDir: "{app}\templates\"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\di.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\sts.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\organisation.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\oces-test-intermedicate-ca.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\oces-test-root-ca.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\serviceplatform.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\InstallCert.bat"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"; IconFilename: "{app}/di.ico";

[Run]
Filename: "InstallCert.bat"; WorkingDir: "{app}";

[CustomMessages]
AlreadyInstalled ={#AppName} is already installed. Please remove it using the Add or remove programs menu. 

[Code]
function InitializeSetup: Boolean;
begin
  if RegKeyExists(HKEY_LOCAL_MACHINE,'SOFTWARE\{#AppPublisher}\{#AppName}') then
      begin
        MsgBox(ExpandConstant('{cm:AlreadyInstalled}') , mbError, MB_OK);
        Result := False;
        exit;
      end
        Result := True;
end;

