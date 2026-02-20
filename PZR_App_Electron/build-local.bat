@echo off
echo ========================================
echo    PZR App Installer Builder
echo ========================================
echo.

REM Check for Node.js
where node >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Node.js ist nicht installiert!
    echo Bitte installieren Sie Node.js von https://nodejs.org/
    pause
    exit /b 1
)

echo [OK] Node.js version:
node --version
echo.

REM Check for npm
where npm >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] npm ist nicht installiert!
    pause
    exit /b 1
)

echo [OK] npm version:
npm --version
echo.

REM Install dependencies
echo [1/2] Installiere Abhaengigkeiten...
call npm install

if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Installation fehlgeschlagen!
    pause
    exit /b 1
)

echo.
echo [2/2] Baue Windows Installer...
call npm run build:win

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo  [SUCCESS] Build erfolgreich!
    echo ========================================
    echo.
    echo Installer befinden sich im 'dist' Ordner:
    dir dist\*.exe
    echo.
    echo Sie koennen den Installer jetzt verteilen!
    echo ========================================
) else (
    echo.
    echo [ERROR] Build fehlgeschlagen!
    pause
    exit /b 1
)

pause
