@echo off
REM ============================================================
REM EarthUnderFreelancer - Installer Build Script
REM ============================================================
REM This script builds the Windows installer using Inno Setup
REM and the Android APK with proper signing
REM ============================================================

echo ============================================================
echo   EarthUnderFreelancer Installer Builder
echo ============================================================
echo.

set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2022.3.0f1\Editor\Unity.exe"
set INNO_PATH="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
set PROJECT_PATH=%~dp0..
set BUILD_OUTPUT=%PROJECT_PATH%\Builds

REM Check if Unity exists
if not exist %UNITY_PATH% (
    echo [ERROR] Unity not found at %UNITY_PATH%
    echo Please update UNITY_PATH in this script
    pause
    exit /b 1
)

echo [1/5] Building Windows Player...
echo ------------------------------------------------------------

%UNITY_PATH% ^
    -projectPath "%PROJECT_PATH%" ^
    -executeMethod BuildScript.BuildWindowsFromCommandLine ^
    -batchmode ^
    -quit ^
    -logFile "%BUILD_OUTPUT%\build_windows.log"

if errorlevel 1 (
    echo [ERROR] Windows build failed. Check build_windows.log
    pause
    exit /b 1
)

echo [OK] Windows build complete!
echo.

echo [2/5] Building Android APK...
echo ------------------------------------------------------------

%UNITY_PATH% ^
    -projectPath "%PROJECT_PATH%" ^
    -executeMethod BuildScript.BuildAndroidFromCommandLine ^
    -batchmode ^
    -quit ^
    -logFile "%BUILD_OUTPUT%\build_android.log"

if errorlevel 1 (
    echo [ERROR] Android build failed. Check build_android.log
    pause
    exit /b 1
)

echo [OK] Android build complete!
echo.

echo [3/5] Creating Windows Installer...
echo ------------------------------------------------------------

if not exist %INNO_PATH% (
    echo [WARNING] Inno Setup not found. Skipping installer creation.
    echo Download from: https://jrsoftware.org/isinfo.php
    goto :SKIP_INSTALLER
)

%INNO_PATH% "%PROJECT_PATH%\Installer\EarthUnderFreelancer_Setup.iss"

if errorlevel 1 (
    echo [ERROR] Installer creation failed.
    pause
    exit /b 1
)

echo [OK] Windows installer created!

:SKIP_INSTALLER
echo.

echo [4/5] Signing Android APK...
echo ------------------------------------------------------------

REM Check if keystore exists
if not exist "%PROJECT_PATH%\Keystore\earthunderfreelancer.keystore" (
    echo [INFO] Keystore not found. Creating new keystore...
    
    mkdir "%PROJECT_PATH%\Keystore" 2>nul
    
    keytool -genkey ^
        -v ^
        -keystore "%PROJECT_PATH%\Keystore\earthunderfreelancer.keystore" ^
        -alias earthunderfreelancer ^
        -keyalg RSA ^
        -keysize 2048 ^
        -validity 10000
        
    if errorlevel 1 (
        echo [WARNING] Could not create keystore. APK will be unsigned.
        goto :SKIP_SIGNING
    )
)

REM Sign the APK
jarsigner ^
    -verbose ^
    -sigalg SHA256withRSA ^
    -digestalg SHA-256 ^
    -keystore "%PROJECT_PATH%\Keystore\earthunderfreelancer.keystore" ^
    "%BUILD_OUTPUT%\Android\EarthUnderFreelancer.apk" ^
    earthunderfreelancer

if errorlevel 1 (
    echo [WARNING] APK signing failed.
    goto :SKIP_SIGNING
)

REM Align the APK
zipalign -v 4 ^
    "%BUILD_OUTPUT%\Android\EarthUnderFreelancer.apk" ^
    "%BUILD_OUTPUT%\Android\EarthUnderFreelancer-aligned.apk"

move /Y "%BUILD_OUTPUT%\Android\EarthUnderFreelancer-aligned.apk" "%BUILD_OUTPUT%\Android\EarthUnderFreelancer.apk"

echo [OK] APK signed and aligned!

:SKIP_SIGNING
echo.

echo [5/5] Creating Release Package...
echo ------------------------------------------------------------

set RELEASE_DIR=%BUILD_OUTPUT%\Release
mkdir "%RELEASE_DIR%" 2>nul

REM Copy Windows installer
if exist "%PROJECT_PATH%\Installer\Output\*.exe" (
    copy "%PROJECT_PATH%\Installer\Output\*.exe" "%RELEASE_DIR%\"
)

REM Copy Android APK
if exist "%BUILD_OUTPUT%\Android\*.apk" (
    copy "%BUILD_OUTPUT%\Android\*.apk" "%RELEASE_DIR%\"
)

REM Create checksums
echo Generating checksums...
certutil -hashfile "%RELEASE_DIR%\EarthUnderFreelancer_Setup_v1.0.0.exe" SHA256 > "%RELEASE_DIR%\checksums.txt"
certutil -hashfile "%RELEASE_DIR%\EarthUnderFreelancer.apk" SHA256 >> "%RELEASE_DIR%\checksums.txt"

echo [OK] Release package created!
echo.

echo ============================================================
echo   BUILD COMPLETE!
echo ============================================================
echo.
echo Output files:
echo   Windows: %RELEASE_DIR%\EarthUnderFreelancer_Setup_v1.0.0.exe
echo   Android: %RELEASE_DIR%\EarthUnderFreelancer.apk
echo.
echo Next steps:
echo   1. Test both installers
echo   2. Upload to distribution platforms
echo   3. Update website download links
echo.

pause
