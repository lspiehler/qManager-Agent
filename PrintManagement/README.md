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