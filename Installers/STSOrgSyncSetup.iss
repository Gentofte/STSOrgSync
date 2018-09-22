; This file is a script that allows to build the OrgSyncer instalation package
; To generate the installer, define the variable MyAppSourceDir MUST point to the Directory where the dll's should be copied from
; The script may be executed from the console-mode compiler - iscc "c:\isetup\samples\my script.iss" or from the Inno Setup Compiler UI
#define AppId "{{0a90431b-110a-462f-8b19-b1edffda64a7}"
#define AppSourceDir "\\VBOXSVR\brian\projects\organisation-synchronisation-component\WindowsService\bin\Debug"
#define AppName "STSOrgSync"
#define AppVersion "1.1"
#define AppPublisher "Digital Identity"
#define AppURL "http://digital-identity.dk/"
#define AppExeName "STSOrgSync.exe"

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
OutputBaseFilename=STSOrgSyncSetup
Compression=lzma
SolidCompression=yes
SourceDir={#AppSourceDir}
OutputDir=..\..\..\Installers
SetupIconFile={#AppSourceDir}\..\..\..\Resources\di.ico
UninstallDisplayIcon={#AppSourceDir}\..\..\..\Resources\di.ico

[Registry]
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}";
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}";
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; ValueType: string; ValueName: "ClientCertThumbprint"; ValueData: "xx xx xx xx xx xx xx xx xx xx xx"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; ValueType: string; ValueName: "DBConnectionString"; ValueData: "SQLITE"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; ValueType: string; ValueName: "DisableRevocationCheck"; ValueData: "true"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; ValueType: string; ValueName: "Environment"; ValueData: "TEST"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; ValueType: string; ValueName: "LogRequestResponse"; ValueData: "false"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; ValueType: string; ValueName: "Municipality"; ValueData: "xxxxxxxx"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; ValueType: string; ValueName: "OrganisationUUID"; ValueData: "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"; Flags: createvalueifdoesntexist
Root: HKLM; Subkey: "SOFTWARE\{#AppPublisher}\{#AppName}"; ValueType: string; ValueName: "UseSSL"; ValueData: "false"; Flags: createvalueifdoesntexist

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "BusinessLayer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Common.Logging.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Common.Logging.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "IntegrationLayer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Log.config"; DestDir: "{app}"; Flags: onlyifdoesntexist
Source: "log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Microsoft.Owin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Microsoft.Owin.Host.HttpListener.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Microsoft.Owin.Hosting.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Owin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Quartz.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "SchedulingLayer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "ServiceLayer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Net.Http.Formatting.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Web.Http.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Web.Http.Owin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "WindowsService.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "WindowsService.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Data.SQLite.EF6.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Data.SQLite.Linq.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "EntityFramework.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "EntityFramework.SqlServer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "OrganisationInspector.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "x64/SQLite.Interop.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "x86/SQLite.Interop.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\OrganisationInspector\bin\Debug\OrganisationInspector.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\di.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\sts.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\oces-test-intermedicate-ca.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\oces-test-root-ca.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\serviceplatform.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\organisation.cer"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppSourceDir}\..\..\..\Resources\cert\InstallCert.bat"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"; IconFilename: "{app}/di.ico";

[Run]
Filename: "{app}\WindowsService.exe"; Parameters: "--install";
Filename: "InstallCert.bat"; WorkingDir: "{app}";

[UninstallRun]
Filename: "{app}\WindowsService.exe"; Parameters: "--uninstall"

[Code]
 function InitializeSetup: Boolean;
  begin
    Result := True;
  end;
