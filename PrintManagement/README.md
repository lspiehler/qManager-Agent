https://thoughtbot.com/blog/using-httplistener-to-build-a-http-server-in-csharp
HttpServer.cs

https://stackoverflow.com/questions/4019466/httplistener-access-denied
netsh http add urlacl url="http://*:8080/" user=everyone
netsh http add urlacl url="https://*:8080/" user=everyone

netsh http add sslcert ipport=0.0.0.0:8080 certhash=ea45c4adc12e8e2de8c592c6ef660c05571918e0 appid={d6002c9f-2099-4690-82d1-49c5e3bf3abc} clientcertnegotiation=enable
netsh http delete sslcert ipport=0.0.0.0:8080


netsh http delete urlacl url="https://*:8080/"

https://volkanpaksoy.com/archive/2015/11/11/building-a-simple-http-server-with-nancy/
Nancy

https://docs.microsoft.com/en-us/dotnet/api/system.printing.printserver?view=windowsdesktop-6.0
getprintqueues


https://www.c-sharpcorner.com/UploadFile/b7531b/create-simple-window-service-and-setup-project-with-installa/
service setup


https://stackoverflow.com/questions/2991286/visual-studio-packaging-another-version-of-this-product-is-already-installed


https://coderead.wordpress.com/2014/08/07/enabling-ssl-for-self-hosted-nancy/

PrintManagement->Properties->AssemblyInfo.cs->AssemblyVersion and AssemblyFileVersion (code)

InstallPrintManagement->Version (UI)

C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe "C:\Users\LyasSpiehler\source\repos\PrintManagement\PrintManagement\bin\Release\PrintManagement.exe"

sc create "Print Management" binpath= "C:\Users\LyasSpiehler\source\repos\PrintManagement\PrintManagement\bin\Release\PrintManagement.exe"

sc delete "Print Management"

---
## Installation Notes

https://subscription.packtpub.com/book/application-development/9781782160427/2/ch02lvl1sec19/harvesting-files-with-heat-exe

cd "C:\Program Files (x86)\WiX Toolset v3.11\bin"
heat dir C:\Users\LyasSpiehler\source\repos\qManager-Agent\PrintManagement\bin\Debug -dr PrintManagement -cg SourceFilesGroup -ag -g1 -sfrag -srd -var "var.MyDir" -out "C:\Users\LyasSpiehler\source\repos\qManager-Agent\WixInstaller\bin\Debug\test.wxs"

#pre-build
"C:\Program Files (x86)\WiX Toolset v3.11\bin\heat.exe" dir $(SolutionDir)\PrintManagement\bin\Debug -dr PrintManagement -cg SourceFilesGroup -ag -g1 -sfrag -srd -var "var.MyDir" -out "$(SolutionDir)\WixInstaller\bin\Debug\test.wxs"

https://stackoverflow.com/questions/36756311/include-all-files-in-bin-folder-in-wix-installer

https://stackoverflow.com/questions/20748544/how-to-generate-consistent-guid-using-wix-harvest-tool
"Yes. -ag flag is used if you want to do minor upgrades. If you have used previously -gg flag, then switching to -ag flag wont help you anymore(if that's the case; you could write custom tool that processes your wxs to keep GUIDS consistent - you can read everything from MSI file). Note that Guid="*" on Component wont produce Guid randomly - it is calculated from the path. If you want to verify things, you should set up test environment and see how minor upgrades work. – 
Erti-Chris Eelmaa"
---