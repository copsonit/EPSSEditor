; -- installer.iss --
; Guide on how to integrate it in a VS solution:
; https://www.technical-recipes.com/2017/creating-installations-using-inno-setup-in-visual-studio/#:~:text=Creating%20installations%20using%20Inno%20Setup%20in%20Visual%20Studio,to%20add%20a%20new%20Installer%20project%20%28class...%20

 
[Setup]
AppName=EPSS Editor
AppVersion=1.16
WizardStyle=modern
DefaultDirName={autopf}\EPSS Editor
DefaultGroupName=EPSS Editor
Compression=lzma2
SolidCompression=yes
OutputBaseFilename=EPSSEditorInstaller
OutputDir=.
; "ArchitecturesAllowed=x64" specifies that Setup cannot run on
; anything but x64.
ArchitecturesAllowed=x64
; "ArchitecturesInstallIn64BitMode=x64" requests that the install be
; done in "64-bit mode" on x64, meaning it should use the native
; 64-bit Program Files directory and the 64-bit view of the registry.
ArchitecturesInstallIn64BitMode=x64

UninstallDisplayIcon={app}\EPSSEditor.exe
UninstallDisplayName=EPSS Editor
 
[Files]
Source: "..\EPSSEditor\bin\Debug\EPSSEditor.exe"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\EPSSEditor.pdb"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\EPSSEditor.exe.config"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\EPSSEditor.exe.manifest"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\NAudio.Asio.dll"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\NAudio.Core.dll"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\NAudio.dll"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\NAudio.Midi.dll"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\NAudio.Wasapi.dll"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\NAudio.WinForms.dll"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\NAudio.WinMM.dll"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\NAudio.xml"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\MidiUI.dll"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\MidiUI.pdb"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\MidiUI.xml"; DestDir: "{app}"
Source: "..\EPSSEditor\bin\Debug\drumMappings.xml"; DestDir: "{app}"
 
[Icons]
Name: "{group}\EPSS Editor"; Filename: "{app}\EPSSEditor.exe"
Name: "{group}\Uninstall"; Filename: "{uninstallexe}"