rem
rem	https://docs.nuget.org/create/creating-and-publishing-a-package
rem 

if not exist NuGet mkdir NuGet

del /Q NuGet\*.*

.nuget\NuGet.exe pack ..\sources\NFile.Framework\NFile.Framework.csproj -OutputDirectory NuGet -Prop Configuration=Release -Symbols

pause
