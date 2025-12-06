# Build-Anleitung für Android und PC

## Voraussetzungen

### Software-Installation

1. **Unity Hub** herunterladen: https://unity.com/download
2. **Unity 2022.3 LTS** installieren mit folgenden Modulen:
   - ✅ Android Build Support
   - ✅ Android SDK & NDK Tools
   - ✅ OpenJDK
   - ✅ Windows Build Support (IL2CPP)

3. **Visual Studio 2022** oder **Rider** für Code-Bearbeitung

---

## PC Build (Windows)

### Schritt 1: Projekt öffnen
1. Unity Hub öffnen
2. "Add" klicken → EarthUnderFreelancer-Ordner auswählen
3. Projekt mit Unity 2022.3 LTS öffnen

### Schritt 2: Build Settings
1. **File → Build Settings** (Ctrl+Shift+B)
2. **Platform**: "PC, Mac & Linux Standalone" auswählen
3. **Target Platform**: Windows
4. **Architecture**: x86_64
5. Falls nicht ausgewählt: "Switch Platform" klicken

### Schritt 3: Player Settings
1. Klicke auf "Player Settings..."
2. Konfiguriere:
   - **Company Name**: EarthUnderStudios
   - **Product Name**: EarthUnderFreelancer
   - **Version**: 1.0.0
   - **Icon**: Dein App-Icon zuweisen
   - **Resolution and Presentation**:
     - Fullscreen Mode: Fullscreen Window
     - Default Screen Width: 1920
     - Default Screen Height: 1080
   - **Other Settings**:
     - Scripting Backend: IL2CPP (für bessere Performance)
     - API Compatibility Level: .NET Standard 2.1

### Schritt 4: Build erstellen
1. Zurück zu Build Settings
2. "Build" klicken
3. Zielordner wählen (z.B. `Builds/Windows`)
4. Warten bis Build abgeschlossen ist

### Ergebnis
```
Builds/Windows/
├── EarthUnderFreelancer.exe       ← Hauptprogramm
├── EarthUnderFreelancer_Data/     ← Spieldaten
├── MonoBleedingEdge/
└── UnityCrashHandler64.exe
```

### Distribution
Für Steam: Alle Dateien im Ordner zusammen verteilen.
Für eigenständige Distribution: Als ZIP packen.

---

## Android Build (APK/AAB)

### Schritt 1: Android Module installieren
Falls nicht bereits installiert:
1. Unity Hub → Installs → Unity 2022.3 → Add Modules
2. Aktivieren:
   - ✅ Android Build Support
   - ✅ Android SDK & NDK Tools
   - ✅ OpenJDK

### Schritt 2: Build Settings
1. **File → Build Settings**
2. **Platform**: "Android" auswählen
3. "Switch Platform" klicken (dauert einige Minuten)

### Schritt 3: Player Settings (Android)
1. "Player Settings..." klicken
2. Konfiguriere:
   
   **Allgemein**:
   - Company Name: EarthUnderStudios
   - Product Name: EarthUnderFreelancer
   - Version: 1.0.0

   **Icon**:
   - Default Icon zuweisen
   - Adaptive Icons konfigurieren

   **Resolution and Presentation**:
   - Default Orientation: Auto Rotation
   - ✅ Allowed Orientations: Landscape Left/Right

   **Other Settings**:
   - Package Name: `com.earthunderstudios.freelancer`
   - Minimum API Level: Android 5.1 (API 22)
   - Target API Level: Android 13 (API 33)
   - Scripting Backend: IL2CPP
   - Target Architectures: ✅ ARM64, ✅ ARMv7
   - Internet Access: Require (für Ads/IAP)

   **Publishing Settings**:
   - Keystore erstellen (für Release)

### Schritt 4: Keystore erstellen (für Release)
1. Publishing Settings → Keystore Manager
2. "Create New" → "Anywhere"
3. Keystore-Details eingeben:
   - Path: `Keys/earthunder.keystore`
   - Password: (sicheres Passwort)
   - Alias: earthunder
   - Validity: 50 Jahre
   - Organisationsdaten ausfüllen
4. Keystore in Player Settings aktivieren

### Schritt 5: Build erstellen

**Für Testing (APK)**:
1. Build Settings
2. Build System: Gradle
3. ☐ Build App Bundle (Google Play) - DEAKTIVIERT
4. "Build" klicken
5. Speicherort wählen → `Builds/Android/EarthUnderFreelancer.apk`

**Für Play Store (AAB)**:
1. Build Settings
2. Build System: Gradle
3. ✅ Build App Bundle (Google Play)
4. "Build" klicken
5. Speicherort wählen → `Builds/Android/EarthUnderFreelancer.aab`

### Schritt 6: APK installieren (Testing)
```bash
# Mit ADB
adb install EarthUnderFreelancer.apk

# Oder: APK auf Gerät kopieren und öffnen
```

---

## Optimierung für Mobile

### Performance-Einstellungen
1. **Quality Settings** (Edit → Project Settings → Quality):
   - Android Default: "Medium"
   - Deaktiviere: Shadows, Soft Particles
   - Texture Quality: Half Res

2. **Graphics Settings**:
   - Vulkan als primäre API
   - OpenGL ES 3.0 als Fallback

3. **Player Settings → Other**:
   - ✅ Optimize Mesh Data
   - ✅ Strip Engine Code
   - Managed Stripping Level: Medium

### Speicheroptimierung
1. Texturen komprimieren (ASTC für Android)
2. Audio komprimieren (Vorbis, Load Type: Streaming)
3. Mesh Compression aktivieren

---

## Häufige Probleme

### Problem: Build schlägt fehl
**Lösung**: 
- Konsole auf Fehler prüfen
- Android SDK/NDK Pfade in Preferences prüfen
- JDK Version verifizieren

### Problem: APK zu groß
**Lösung**:
- Unbenutzte Assets entfernen
- Texturen komprimieren
- Split APKs by Target Architecture aktivieren

### Problem: Spiel crasht auf Android
**Lösung**:
- adb logcat für Fehleranalyse
- Minification aktivieren (R8)
- Memory Budget prüfen

---

## Schnell-Befehle

### Unity Command Line Build
```bash
# Windows Build
Unity.exe -batchmode -quit -projectPath "C:/Projekte/EarthUnderFreelancer" -buildWindowsPlayer "Builds/Windows/EarthUnderFreelancer.exe"

# Android Build
Unity.exe -batchmode -quit -projectPath "C:/Projekte/EarthUnderFreelancer" -executeMethod BuildScript.BuildAndroid
```

### ADB Befehle
```bash
# Gerät prüfen
adb devices

# APK installieren
adb install -r EarthUnderFreelancer.apk

# Logs anzeigen
adb logcat -s Unity

# App deinstallieren
adb uninstall com.earthunderstudios.freelancer
```
