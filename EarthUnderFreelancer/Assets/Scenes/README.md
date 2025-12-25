# Unity Scenes

Diese Ordner enthält alle spielbaren Unity-Szenen für EarthUnderFreelancer.

## Verfügbare Szenen

### GameScene.unity
Die Haupt-Spielszene mit vollständigem Setup.

**Enthält:**
- Main Camera (konfiguriert für große Sichtweiten)
- Directional Light (Sonne)
- WorldSetup GameObject mit SimpleWorldSetup-Script
- Vorkonfiguriertes Lighting und Fog

**Verwendung:**
1. Öffne die Szene in Unity
2. Drücke Play
3. Das WorldSetup-Script erstellt automatisch:
   - Terrain/Boden
   - Spawn-Punkte
   - Checkpoints
   - Manager-GameObjects

**Kamera-Steuerung im Editor:**
- Rechtsklick + WASD = Freie Bewegung
- Mittelklick + Ziehen = Pan
- Alt + Linksklick = Orbit

---

## Zukünftige Szenen (geplant)

### MainMenu.unity (TODO)
Hauptmenü mit Login und Optionen.

**Features:**
- Title Screen
- Login/Registration UI
- Settings Menu
- Credits

### TrainingScene.unity (TODO)
Tutorial-Level für neue Spieler.

**Features:**
- Schritt-für-Schritt Tutorial
- Übungs-Ziele
- Keine Feinde
- Reduzierte Welt-Größe

### CombatDemo.unity (TODO)
Schnelle Kampf-Demo für Testing.

**Features:**
- Vorgeladene Feinde
- Kleine Arena
- Schneller Einstieg
- Ideal für Combat-Testing

### MultiplayerLobby.unity (TODO)
Lobby für Multiplayer-Matches.

**Features:**
- Server-Browser
- Matchmaking
- Lobby-Chat
- Team-Auswahl

---

## Szenen erstellen

### Neue Szene von Grund auf:

1. **File → New Scene**
2. **Speichern als:** `Assets/Scenes/MeineScene.unity`
3. **WorldSetup hinzufügen:**
   ```
   GameObject → Create Empty
   Name: "WorldSetup"
   Add Component → SimpleWorldSetup
   ```
4. **Kamera anpassen:**
   - Far Clipping Plane: 100000
   - Position: (0, 200, -500)

### Template verwenden:

1. **GameScene.unity duplizieren**
2. **Umbenennen**
3. **Anpassen nach Bedarf**

---

## Best Practices

### Performance
- Verwende Occlusion Culling für große Szenen
- LOD-System für Flugzeuge aktivieren
- Object Pooling für Projektile nutzen

### Organization
- Nutze leere GameObjects als "Ordner"
- Benenne logisch: `_Managers`, `_Environment`, `_Gameplay`
- Verwende Layer für verschiedene Object-Typen

### Testing
- Erstelle separate Test-Szenen
- Halte Produktions-Szenen sauber
- Nutze Scene Templates für häufige Setups

---

## Szenen-Workflow

### Entwicklung
1. Arbeite in Test-Szenen
2. Teste Features isoliert
3. Merge in Main-Szene wenn stabil

### Build
1. **Build Settings → Scenes in Build:**
   - 0: MainMenu
   - 1: GameScene
   - 2: TrainingScene
   - etc.

2. **Start-Szene:** MainMenu

---

## Troubleshooting

### "Script missing" Fehler?
- Überprüfe ob alle Scripts kompiliert sind
- Reimport Assets: Assets → Reimport All

### Szene lädt nicht?
- Überprüfe Build Settings
- Szene muss in Build-Liste sein

### Performance-Probleme?
- F1 drücken für Developer Tools
- FPS-Counter überprüfen
- Profiler nutzen: Window → Analysis → Profiler

---

## Weitere Infos

Siehe auch:
- `/Assets/Scripts/Gameplay/README.md` - Gameplay-Scripts
- `/QUICK_START.md` - Schnellstart-Guide
- `/Documentation/COMPLETE_GUIDE.md` - Vollständige Doku
