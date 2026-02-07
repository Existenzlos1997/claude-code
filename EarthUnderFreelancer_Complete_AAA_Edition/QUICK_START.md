# 🚀 EarthUnderFreelancer - Quick Start Guide

## Was ist EarthUnderFreelancer?

Ein massives Open-World-Flugsimulator-MMORPG mit:
- **500+ echte Flugzeuge** (WWI bis 5. Generation)
- **15.000+ Orte** (Städte, Flughäfen weltweit)
- **1.000+ Missionen** mit vollständigen Dialogen
- **50+ Gameplay-Systeme** (Clans, Handel, Kämpfe, Upgrades)

## 🎯 Schnellstart (für Spieler)

### Option 1: Unity direkt spielen (Einfachst!)
1. Installiere **Unity Hub** und **Unity 2022.3 LTS**
2. Öffne das Projekt im Unity Hub
3. Öffne die Szene `Assets/Scenes/GameScene` (oder lass GameSceneSetup eine erstellen)
4. Drücke **Play** ▶️
5. **Fertig! Spielen!**

### Option 2: Executable bauen
1. Öffne das Projekt in Unity
2. Menü: **EarthUnderFreelancer** → **🚀 Build Portable Windows EXE**
3. Die EXE liegt in `Builds/Windows/`
4. Doppelklick zum Spielen!

## 🎮 Steuerung

### Flugzeug
- **W** = Schub erhöhen
- **S** = Schub verringern
- **A/D** = Rollen links/rechts
- **Maus** = Zielen/Kamera
- **Linke Maustaste** = Schießen
- **Rechte Maustaste** = Rakete
- **Shift** = Boost
- **Leertaste** = Bremsen
- **Tab** = Nächstes Ziel

### UI & Menüs
- **F** = Andocken (bei Stationen)
- **I** = Inventar
- **M** = Missionen
- **K** = Skill-Baum
- **J** = Achievements
- **ESC** = Pause

## 🛠️ Entwickler-Informationen

### Projekt-Struktur
```
EarthUnderFreelancer/
├── Assets/
│   ├── Scripts/        # 120+ C# Skripte
│   │   ├── AI/         # KI-Systeme
│   │   ├── Combat/     # Kampfsysteme
│   │   ├── Core/       # Kern-Manager
│   │   ├── Data/       # Flugzeug/Welt-Datenbanken
│   │   ├── Systems/    # Gameplay-Systeme
│   │   └── UI/         # Benutzeroberfläche
│   ├── Scenes/         # Unity-Szenen
│   ├── Prefabs/        # Wiederverwendbare Objekte
│   └── Resources/      # Laufzeit-Assets
├── ProjectSettings/    # Unity-Projekteinstellungen
└── Documentation/      # Vollständige Dokumentation
```

### Wichtige Skripte
- `GameManager.cs` - Haupt-Spielmanager
- `GameSceneSetup.cs` - Automatisches Szenen-Setup
- `PlayerController.cs` - Spielersteuerung
- `RealAircraftDatabase.cs` - 500+ Flugzeuge
- `WorldGeographyDatabase.cs` - 15.000+ Orte

### Nächste Schritte für Entwickler
1. **Assets hinzufügen**: 3D-Modelle für Flugzeuge (`.fbx` oder `.obj`)
2. **Audio hinzufügen**: Sound-Effekte und Musik
3. **Texturen hinzufügen**: Skins für Flugzeuge
4. **Testen**: Spiel in Unity spielen und debuggen
5. **Bauen**: Portable EXE oder Android APK erstellen

## 📚 Weiterführende Dokumentation

- `README.md` - Vollständige Projektbeschreibung
- `Documentation/COMPLETE_GUIDE.md` - Detailliertes Entwickler-Handbuch
- `Documentation/BuildGuide.md` - Build-Anleitung für alle Plattformen
- `Documentation/WIE_BEKOMME_ICH_DAS_SPIEL.md` - Download & Installation (Deutsch)

## 🎯 Aktueller Status

✅ **Komplett** (Code-seitig):
- 120+ Skripte, 55.000+ Zeilen Code
- Alle Gameplay-Systeme implementiert
- 500+ Flugzeuge in Datenbank
- 15.000+ Weltorte
- 1.000+ Missionen
- 50 fortgeschrittene Systeme

⏳ **Benötigt noch**:
- 3D-Assets (Flugzeuge, Umgebung)
- Audio-Dateien (Motor, Waffen, Musik)
- Texturen und Materialien

## 💡 Tipps

### Für Spieler
- Starte im **Anfängergebiet** (sichere PVE-Zone)
- Wähle deinen **Karrierepfad**: Händler, Schmuggler, Militär oder Pirat
- Nutze **Auto-Pilot** für lange Flüge
- Trete einer **Gilde** bei für Clan-Kämpfe

### Für Entwickler
- Nutze `GameSceneSetup` um schnell eine spielbare Szene zu erstellen
- Alle Manager sind Singletons - greife über `.Instance` zu
- Flugzeuge werden per `AircraftData` ScriptableObject definiert
- Verwende `ObjectPool` für Performance (Projektile, Effekte)

## 🚀 Los geht's!

1. **Unity öffnen**
2. **Play drücken** ▶️
3. **Spaß haben!** 🛩️

Bei Fragen: Siehe vollständige Dokumentation in `Documentation/`
