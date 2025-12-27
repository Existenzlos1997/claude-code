# ============================================================
# EarthUnderFreelancer - Complete Installation & Build Guide
# ============================================================

## 📦 Was ist enthalten?

Das EarthUnderFreelancer-Projekt ist ein vollständiges Unity MMORPG mit:

- **100+ C# Scripts** (40.000+ Zeilen Code)
- **45+ vollständige Gameplay-Systeme**
- **100+ echte Flugzeuge** (WWI bis 5. Generation)
- **Professioneller Installer** für Windows
- **Android APK/AAB** für Mobile und Play Store
- **Auto-Update System** für automatische Updates
- **14 Sprachen** lokalisiert

---

## 🎮 Wie installiere ich das fertige Spiel?

### Windows Installation:

1. **Download** die Datei `EarthUnderFreelancer_Setup_v1.0.0.exe`
2. **Doppelklick** auf die Setup-Datei
3. **Installation durchführen** (Standard-Verzeichnis empfohlen)
4. **Fertig!** Starte das Spiel über Desktop-Verknüpfung

### Android Installation:

1. **Download** die Datei `EarthUnderFreelancer.apk`
2. **Erlaube** Installation von unbekannten Quellen in den Einstellungen
3. **Tippe** auf die APK-Datei
4. **Installieren** und öffnen

---

## 🛠 Wie baue ich das Spiel selbst?

### Voraussetzungen:

1. **Unity 2022.3 LTS** (oder neuer)
   - Download: https://unity.com/download
   - Module: Android Build Support, iOS Build Support, Windows Build Support (IL2CPP)

2. **Inno Setup 6** (für Windows Installer)
   - Download: https://jrsoftware.org/isinfo.php

3. **Android SDK** (für APK)
   - Wird automatisch mit Unity installiert

### Build-Schritte:

#### Variante A: Über Unity Editor

1. Öffne das Projekt in Unity
2. Gehe zu **EarthUnderFreelancer** → **Build** im Menü
3. Wähle die gewünschte Plattform:
   - Windows (x64)
   - Android APK
   - Android AAB (für Play Store)
   - macOS
   - Linux

#### Variante B: Über Command Line

**Windows:**
```batch
cd EarthUnderFreelancer/Installer
build_installer.bat
```

**macOS/Linux:**
```bash
cd EarthUnderFreelancer/Installer
chmod +x build_all.sh
./build_all.sh
```

#### Variante C: Über Unity Command Line

```bash
# Windows Build
Unity.exe -projectPath "EarthUnderFreelancer" -executeMethod BuildScript.BuildWindowsFromCommandLine -batchmode -quit

# Android Build  
Unity.exe -projectPath "EarthUnderFreelancer" -executeMethod BuildScript.BuildAndroidFromCommandLine -batchmode -quit
```

---

## 📁 Projektstruktur

```
EarthUnderFreelancer/
├── Assets/
│   ├── Editor/                    # Build-Scripts
│   │   ├── BuildScript.cs
│   │   └── EnhancedBuildScript.cs
│   ├── Scripts/
│   │   ├── AI/                    # KI-Systeme
│   │   ├── Audio/                 # Audio-Manager
│   │   ├── AutoUpdate/            # Auto-Update System
│   │   ├── Character/             # Charakter-System
│   │   ├── Combat/                # Kampf-Systeme
│   │   ├── Core/                  # Kern-Manager
│   │   ├── Customization/         # Anpassungs-Systeme
│   │   ├── Data/                  # Datenbanken
│   │   ├── Economy/               # Wirtschafts-Systeme
│   │   ├── Events/                # Event-Systeme
│   │   ├── Graphics/              # Grafik/Shader
│   │   ├── Housing/               # Hangar-System
│   │   ├── Instances/             # Dungeon/Raid
│   │   ├── Launcher/              # Game Launcher
│   │   ├── Leaderboard/           # Ranglisten
│   │   ├── Localization/          # Sprachen
│   │   ├── MMO/                   # MMO-Backend
│   │   ├── Multiplayer/           # Multiplayer-Sync
│   │   ├── NPC/                   # NPC-Systeme
│   │   ├── Performance/           # LOD/Optimierung
│   │   ├── Player/                # Spieler-Controller
│   │   ├── PvP/                   # PvP-Systeme
│   │   ├── Social/                # Soziale Systeme
│   │   ├── Story/                 # Story-Kampagne
│   │   ├── Systems/               # Gameplay-Systeme
│   │   ├── UI/                    # UI-Systeme
│   │   ├── Vehicles/              # Fahrzeug-Controller
│   │   ├── VFX/                   # Effekte
│   │   ├── Weather/               # Wetter-System
│   │   ├── Weapons/               # Waffen-Systeme
│   │   └── World/                 # Welt-Systeme
│   ├── Prefabs/                   # Vorgefertigte Objekte
│   ├── Resources/                 # Ressourcen
│   ├── Scenes/                    # Szenen
│   └── StreamingAssets/           # Streaming-Assets
├── Documentation/
│   ├── BuildGuide.md
│   ├── LICENSE.txt
│   ├── README_INSTALL.txt
│   └── StoreDescriptions.md
├── Installer/
│   ├── EarthUnderFreelancer_Setup.iss
│   ├── build_installer.bat
│   └── build_all.sh
├── ProjectSettings/               # Unity-Einstellungen
└── README.md
```

---

## 🚀 Nach dem Build

### Windows Installer erstellen:

1. Baue das Spiel über Unity (Windows x64)
2. Öffne `Installer/EarthUnderFreelancer_Setup.iss` in Inno Setup
3. Klicke "Compile"
4. Der Installer wird in `Installer/Output/` erstellt

### Play Store Upload:

1. Baue Android AAB
2. Gehe zur Google Play Console
3. Erstelle neue App
4. Lade AAB hoch
5. Fülle Store-Listing aus (siehe StoreDescriptions.md)
6. Veröffentlichen

### Steam Upload:

1. Registriere bei Steamworks
2. Erstelle neue App
3. Konfiguriere Depots
4. Nutze SteamCMD für Upload
5. Fülle Store-Page aus
6. Veröffentlichen

---

## ❓ Häufige Fragen

### Das Spiel startet nicht?
- Prüfe Visual C++ Redistributable
- Prüfe DirectX 11 Unterstützung
- Aktualisiere Grafiktreiber

### Build schlägt fehl?
- Prüfe Unity Version (2022.3+)
- Prüfe Android SDK Installation
- Prüfe Konsole auf Fehler

### APK lässt sich nicht installieren?
- Erlaube "Unbekannte Quellen"
- Deinstalliere alte Version
- Prüfe Android Version (7.0+)

---

## 📞 Support

- Website: https://earthunderfreelancer.com
- Email: support@earthunderfreelancer.com
- Discord: discord.gg/earthunderfreelancer

---

© 2024 EarthUnder Studios. Alle Rechte vorbehalten.
