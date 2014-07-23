@echo off
..\..\NuGet\NuGet.exe pack Miracle.macros.csproj -prop Configuration=release
echo "run ..\..\NuGet\NuGet.exe push Miracle.Macros... to publish"
pause