# Compile Installer
. "C:\Program Files (x86)\Inno Setup 6\iscc.exe" qmanager.iss

# Silent Install
. "C:\Users\LyasSpiehler\source\repos\qManager-Agent-Installer\Output\PrintManagementInstall.exe" /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /host=qmanager-test.lcmchealth.org:3034 /groups="LCMC Test Cluster" /cert="qManager Websocket Agent"

# Silent Uninstall
. "C:\Program Files\Sapphire Health\Print Management\unins000.exe" /VERYSILENT /SUPPRESSMSGBOXES /NORESTART