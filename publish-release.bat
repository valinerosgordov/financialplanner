@echo off
echo ================================================
echo  Nexus Finance - Release Build Script
echo ================================================
echo.

REM Clean previous builds
echo [1/4] Cleaning previous builds...
dotnet clean -c Release
if exist "bin\Release\net8.0-windows\win-x64\publish" rmdir /s /q "bin\Release\net8.0-windows\win-x64\publish"

REM Restore dependencies
echo [2/4] Restoring dependencies...
dotnet restore

REM Build Release configuration
echo [3/4] Building Release configuration...
dotnet build -c Release --no-restore

REM Publish as single executable
echo [4/4] Publishing single-file executable...
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:PublishReadyToRun=true

echo.
echo ================================================
echo Build completed!
echo Location: bin\Release\net8.0-windows\win-x64\publish\NexusFinance.exe
echo ================================================
echo.
pause
