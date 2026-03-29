@echo off
:: ============================================================
::  MQG Trading Bot – Automatischer Installer (Windows)
::  Installationsbefehl (PowerShell):
::  irm https://raw.githubusercontent.com/Existenzlos1997/claude-code/main/TradingBot/install.bat | iex
:: ============================================================
setlocal EnableDelayedExpansion

set "REPO=https://github.com/Existenzlos1997/claude-code"
set "INSTALL_DIR=%USERPROFILE%\mqg-trading-bot"
set "BOT_DIR=%INSTALL_DIR%\TradingBot"

echo.
echo  ══════════════════════════════════════════════
echo     MQG Trading Bot ^– Windows Installer
echo  ══════════════════════════════════════════════
echo.

:: ── 1. Python prüfen ──────────────────────────────────────
where python >nul 2>&1
if %errorlevel% neq 0 (
    echo [X] Python nicht gefunden.
    echo     Bitte Python 3.10+ installieren: https://www.python.org/downloads/
    echo     Wichtig: "Add python.exe to PATH" aktivieren!
    pause
    exit /b 1
)
for /f "tokens=*" %%v in ('python --version 2^>^&1') do echo [+] %%v gefunden

:: ── 2. git prüfen / klonen ──────────────────────────────
where git >nul 2>&1
if %errorlevel% equ 0 (
    if exist "%INSTALL_DIR%\.git" (
        echo [+] Aktualisiere vorhandene Installation...
        git -C "%INSTALL_DIR%" pull --ff-only
    ) else (
        echo [+] Klone Repository nach %INSTALL_DIR% ...
        git clone --depth=1 "%REPO%" "%INSTALL_DIR%"
    )
) else (
    echo [!] git nicht gefunden – lade ZIP-Archiv herunter...
    if not exist "%TEMP%\mqg-bot.zip" (
        powershell -Command "Invoke-WebRequest -Uri '%REPO%/archive/refs/heads/main.zip' -OutFile '%TEMP%\mqg-bot.zip'"
    )
    powershell -Command "Expand-Archive -Path '%TEMP%\mqg-bot.zip' -DestinationPath '%TEMP%\mqg-bot-src' -Force"
    if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
    robocopy "%TEMP%\mqg-bot-src\claude-code-main" "%INSTALL_DIR%" /E /NFL /NDL /NJH /NJS >nul
)

:: ── 3. Abhängigkeiten installieren ────────────────────────
echo [+] Installiere Python-Abhängigkeiten...
python -m pip install -r "%BOT_DIR%\requirements.txt" --quiet

if %errorlevel% neq 0 (
    echo [X] Fehler beim Installieren der Abhängigkeiten.
    pause
    exit /b 1
)

:: ── 4. Fertig ──────────────────────────────────────────────
echo.
echo  ══════════════════════════════════════════════
echo     Installation abgeschlossen!
echo  ══════════════════════════════════════════════
echo.
echo  Verzeichnis : %BOT_DIR%
echo.
echo  Naechste Schritte:
echo  ────────────────────────────────────────────
echo  1) API-Key konfigurieren:
echo     notepad "%BOT_DIR%\config.json"
echo.
echo  2) Backtest (erst testen!):
echo     cd "%BOT_DIR%" ^&^& python backtest.py --symbol BTC/EUR --days 90 --plot
echo.
echo  3) Bot starten (Sandbox):
echo     cd "%BOT_DIR%" ^&^& python main.py
echo     Dashboard: http://localhost:5050
echo.
echo  4) Smartphone-App:
echo     Oeffne http://DEINE-IP:5050 im Handy-Browser
echo     Menue ^> "Zum Startbildschirm hinzufuegen"
echo.
echo  [!] Der Bot laeuft standardmaessig im SANDBOX-Modus (kein echtes Geld).
echo  [!] config.json: "sandbox": false nur NACH ausgiebigem Testen setzen!
echo.
pause
