@echo off
setlocal enabledelayedexpansion

if "%1"=="" (
    echo Usage: setup-new-project.bat YourProjectName
    echo Example: setup-new-project.bat MyAwesomeAPI
    exit /b 1
)

set PROJECT_NAME=%1

echo Setting up new project: %PROJECT_NAME%
echo.

echo This batch file provides basic setup. For full automated setup, please use:
echo PowerShell: .\setup-new-project.ps1 -ProjectName "%PROJECT_NAME%"
echo.

echo Manual steps to complete setup:
echo 1. Rename all folders containing "ProjectTemplate" to "%PROJECT_NAME%"
echo 2. Rename all files containing "ProjectTemplate" to "%PROJECT_NAME%"
echo 3. Replace all "ProjectTemplate" text in files with "%PROJECT_NAME%"
echo 4. Update connection string in appsettings.json
echo 5. Update JWT secret in appsettings.json
echo 6. Run: dotnet restore
echo 7. Run: dotnet ef database update --project %PROJECT_NAME%.API
echo 8. Run: dotnet run --project %PROJECT_NAME%.API
echo.

echo For automated setup, please run the PowerShell script instead.
pause
