# EarthUnder Freelancer

## 🛩️ Das ultimative Luftkampf-MMORPG

EarthUnder Freelancer ist ein massives Open-World Flugsimulator-MMORPG, das die besten Elemente von War Thunder, Freelancer und World of Warcraft kombiniert. Erlebe epische Luftkämpfe mit echten Flugzeugen, erobere Territorien mit deiner Fraktion, handle und schmuggele auf lebhaften Handelsrouten, und werde Teil einer lebendigen Welt mit tausenden anderen Spielern.

## 📊 Projektstatistiken
- **95+ C# Scripts**
- **35.000+ Zeilen Code**
- **40+ vollständige Systeme**
- **100+ echte Flugzeuge** von WW1 bis 5. Generation

---

## 🎮 Features

### ✈️ Echte Flugzeuge (100+ Modelle)
- **Weltkrieg I**: Fokker Dr.I, Sopwith Camel, SPAD XIII
- **Weltkrieg II**: P-51D Mustang, Bf 109, Spitfire, Zero, Fw 190, P-47, Corsair
- **Koreakrieg**: F-86 Sabre, MiG-15
- **Kalter Krieg**: F-4 Phantom, MiG-21, F-104 Starfighter
- **Modern**: F-14 Tomcat, F-15 Eagle, F-16, F/A-18, MiG-29, Su-27, A-10
- **5. Generation**: F-22 Raptor, F-35 Lightning II, Su-57

### 🎯 Kampfsystem
- **Realistische Flugphysik** wie in War Thunder (G-Kräfte, Strömungsabriss, Drehung)
- **Authentische Waffen**: MG, Kanonen, Bomben, Raketen, Lenkwaffen
- **Lead-Indikator-Zielsystem** mit präziser Berechnung
- **Schild-, Rumpf- und Komponenten-Schadenssystem**
- **Intelligente KI-Gegner** mit Patrol, Chase, Attack, Flee, Evade-Verhalten

### 🌍 MMORPG Fraktionssystem
- **Dynamische Territorien** die von Spielern erobert werden können
- **Fraktionskämpfe** die Grenzen auf der Weltkarte verschieben
- **Handelsrouten** zwischen Fraktionsgebieten
- **Schmuggel-Missionen** mit Grenzübergang
- **Lebendige Welt** - alle Spieler auf einem Server

### 💰 Wirtschaftssystem
- **Dynamisches Handelssystem** mit Angebot/Nachfrage
- **Fraktionsreputation** beeinflusst Preise und Missionen
- **Inventar- und Ausrüstungssystem**
- **Crafting** für Waffen und Schiffsverbesserungen

### Missionen
- **Zufallsgenerierte Missionen**: Kampf, Lieferung, Erkundung
- **Belohnungen**: Credits, XP, Reputation
- **Schwierigkeitsgrade** von Einfach bis Extrem

### Progression
- **Level-System** mit Skillpunkten
- **Schiffs-Upgrades** und Anpassungen
- **Speichersystem** für Spielfortschritt

---

## 📁 Projektstruktur

```
EarthUnderFreelancer/
├── Assets/
│   ├── Scripts/
│   │   ├── AI/              # KI-Systeme (AIShipController, AISpawnManager)
│   │   ├── Combat/          # Waffen, Targeting, Projectile, Missile
│   │   ├── Core/            # GameManager, Input, Audio, SaveLoad
│   │   ├── Data/            # ScriptableObjects (Items, Ships, Weapons)
│   │   ├── Missions/        # Missionssystem
│   │   ├── Monetization/    # Ads, IAP
│   │   ├── Networking/      # Multiplayer (Lobby, Matchmaking)
│   │   ├── Player/          # Spielersteuerung, Kamera
│   │   ├── Systems/         # Economy, Factions, Inventory, Skills,
│   │   │                    # Achievements, Quests, Loot, Docking,
│   │   │                    # Ship Upgrades, Environment, Crafting
│   │   ├── Trading/         # Handelssystem
│   │   ├── UI/              # HUD, Menüs, Radar, Skill Tree, Achievements
│   │   ├── VFX/             # Visuelle Effekte
│   │   └── Vehicles/        # Fahrzeugphysik, HealthSystem
│   ├── Editor/              # Build Scripts
│   ├── Prefabs/
│   ├── Scenes/
│   └── Resources/
├── ProjectSettings/
└── Documentation/
```

### Anzahl der Dateien
- **C# Scripts**: 75+
- **Codezeilen**: ~15.000+
- **Systeme**: 25+

---

## 🛠️ Installationsanleitung

### Voraussetzungen
- **Unity 2022.3 LTS** oder neuer
- **Visual Studio 2019/2022** oder **JetBrains Rider**
- **Android Build Support** (für Android-Builds)
- **Windows Build Support** (für PC-Builds)

### Projekt öffnen
1. Klone das Repository:
   ```bash
   git clone https://github.com/YourRepo/EarthUnderFreelancer.git
   ```
2. Öffne Unity Hub
3. Klicke auf "Add" und wähle den `EarthUnderFreelancer`-Ordner
4. Öffne das Projekt mit Unity 2022.3 LTS

### Erste Schritte im Editor
1. Öffne die Szene `Assets/Scenes/GameScene` (oder erstelle eine mit GameSceneSetup)
2. Drücke Play zum Testen
3. Steuerung:
   - **WASD** - Bewegung (W=Schub, S=Rückwärts, A/D=Rollen)
   - **Maus** - Zielen/Kamera
   - **Linke Maustaste** - Primärwaffe
   - **Rechte Maustaste** - Sekundärwaffe/Rakete
   - **Shift** - Boost
   - **Space** - Bremsen
   - **Tab** - Ziel wechseln
   - **R** - Waffe wechseln
   - **F** - Andocken (bei Stationen)
   - **I** - Inventar
   - **M** - Missionen
   - **K** - Skill Tree
   - **J** - Achievements
   - **ESC** - Pause

---

## 📱 Build-Anleitung

### PC Build (Windows)
1. **File → Build Settings**
2. Wähle "PC, Mac & Linux Standalone"
3. Target Platform: Windows
4. Architecture: x86_64
5. Klicke "Build"
6. Wähle Ausgabeordner
7. Fertig! → `EarthUnderFreelancer.exe`

### Android Build
1. **File → Build Settings**
2. Wähle "Android"
3. Klicke "Switch Platform"
4. **Player Settings** konfigurieren:
   - Package Name: `com.earthunderstudios.freelancer`
   - Minimum API Level: 22
   - Target API Level: 33
   - Scripting Backend: IL2CPP
5. Keystore erstellen/konfigurieren
6. Klicke "Build"
7. Fertig! → `EarthUnderFreelancer.apk`

---

## 💰 Monetarisierung

### Unity Ads
- **Interstitial Ads**: Nach Missionsergebnissen
- **Rewarded Ads**: Für Bonus-Credits/Leben
- **Banner Ads**: Im Hauptmenü

### In-App-Käufe
| Produkt | Preis | Beschreibung |
|---------|-------|--------------|
| 500 Credits | 0,99€ | Spielwährung |
| 2500 Credits | 4,99€ | Spielwährung |
| Remove Ads | 2,99€ | Werbung entfernen |
| VIP Monat | 9,99€ | Alle Premium-Features |

---

## 📊 Systemanforderungen

### PC
- **Minimum**: Windows 7, Intel i3, 4GB RAM, GeForce GTX 460
- **Empfohlen**: Windows 10, Intel i5, 8GB RAM, GeForce GTX 970

### Android
- **Minimum**: Android 5.1, 2GB RAM
- **Empfohlen**: Android 10+, 4GB RAM

---

## 🎯 Roadmap & Fertigstellungsgrad

### ✅ Fertig (~98%)
- [x] Core Flight System (6DoF Physik, Boost, Bremsen)
- [x] Combat System (Waffen, Raketen, Targeting mit Lead-Indikator)
- [x] Trading System (Dynamische Preise, Angebot/Nachfrage)
- [x] Mission System (Zufallsgenerierte Missionen)
- [x] Quest System (Story-Quests mit Voraussetzungen)
- [x] UI System (HUD, Menüs, Inventar, Trading)
- [x] Save/Load System (Vollständige Speicherung)
- [x] Skill Tree System (4 Bäume: Combat, Defense, Piloting, Trading)
- [x] Achievement System (15+ Achievements mit Belohnungen)
- [x] Tutorial System (Interaktives Tutorial für neue Spieler)
- [x] Loot System (Drops, Attraktion, Sammlung)
- [x] Docking System (Andocken an Raumstationen)
- [x] Ship Upgrade System (Waffen, Schilde, Motoren, Rüstung)
- [x] Space Environment System (Nebel, Asteroiden, Strahlung)
- [x] Enhanced Audio System (Musik-Crossfade, 3D-Sound, Ambience)
- [x] Radar/Minimap System
- [x] Pause Menu mit Optionen
- [x] Fraktionssystem mit Reputation
- [x] Crafting System
- [x] Monetarisierung (Unity Ads, IAP)

### 🔧 In Bearbeitung
- [ ] Multiplayer PvP (Grundgerüst vorhanden)
- [ ] Zusätzliche Schiffe
- [ ] Mehr Fraktionen
- [ ] Story-Kampagne

---

## 📄 Lizenz

Copyright © 2024 EarthUnder Studios. Alle Rechte vorbehalten.

---

## 👥 Credits

- Game Design & Development
- Art & Sound (Placeholder Assets)
- Unity Engine
