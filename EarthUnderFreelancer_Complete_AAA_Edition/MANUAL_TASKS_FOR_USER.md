# 🔧 Manuelle Aufgaben - Was Nur DU Machen Kannst

**Für:** @Existenzlos1997
**Von:** @copilot
**Datum:** 01.01.2026

Diese Aufgaben erfordern Unity-Editor-Interaktion, Asset-Erstellung oder externe Tools. Ich kann den Code bereitstellen, aber die Integration musst Du machen.

---

## 📋 Phase 4A: GUI-Framework - Unity-Spezifische Aufgaben

### Task 1: Main Menu Scene Erstellen
**Dauer:** 30 Minuten
**Priorität:** 🔴 KRITISCH

#### Schritte:
1. Unity öffnen
2. **Neue Szene erstellen:**
   - File → New Scene
   - Name: `MainMenu.unity`
   - Speichern in: `Assets/Scenes/`

3. **Canvas erstellen:**
   ```
   - Rechtsklick in Hierarchy → UI → Canvas
   - Name: "MainMenuCanvas"
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080
   ```

4. **EventSystem prüfen:**
   - Sollte automatisch erstellt werden
   - Falls nicht: GameObject → UI → Event System

5. **Background hinzufügen:**
   - Im Canvas: Rechtsklick → UI → Image
   - Name: "Background"
   - Stretch to full screen (Anchors auf Stretch/Stretch)
   - Color: Schwarz oder Dein Design

6. **UnifiedMenuSystem Component hinzufügen:**
   - Leeres GameObject erstellen: "MenuSystem"
   - Add Component → UnifiedMenuSystem
   - Add Component → GUIManager
   - Add Component → MenuInputController

7. **Szene als Build-Scene hinzufügen:**
   - File → Build Settings
   - Add Open Scenes
   - MainMenu als Scene 0 (erste Szene)

**Ergebnis:** Main Menu Scene ist bereit für UI-Elements

---

### Task 2: UI-Prefabs Erstellen
**Dauer:** 1-2 Stunden
**Priorität:** 🔴 KRITISCH

#### Main Menu Button Panel:
1. **Panel erstellen:**
   - Im Canvas: UI → Panel
   - Name: "MainMenuPanel"
   - Anchors: Center
   - Size: 400x600

2. **Buttons hinzufügen:**
   ```
   Vertical Layout Group auf Panel
   - Child Alignment: Middle Center
   - Spacing: 20
   
   Buttons:
   - StartGameButton (Text: "Start Game")
   - MultiplayerButton (Text: "Multiplayer")
   - SettingsButton (Text: "Settings")
   - CreditsButton (Text: "Credits")
   - QuitButton (Text: "Quit")
   ```

3. **Button-Styling:**
   - Transition: Color Tint
   - Normal: Weiß
   - Highlighted: Hellgrau
   - Pressed: Dunkelgrau
   - Font: Arial 24pt (oder dein Font)

4. **Als Prefab speichern:**
   - Drag Panel in `Assets/Prefabs/UI/`
   - Name: "MainMenuPanel.prefab"

#### Pause Menu Prefab:
1. **Ähnlich wie Main Menu, aber mit:**
   ```
   Buttons:
   - ResumeButton
   - SettingsButton
   - MainMenuButton
   - QuitButton
   ```

2. **Semi-transparenter Background:**
   - Alpha: 0.8 (80%)
   - Color: Schwarz

3. **Speichern als:** `PauseMenuPanel.prefab`

#### Settings Menu Prefab:
1. **Tabs erstellen für:**
   - Graphics
   - Audio
   - Controls
   - Gameplay

2. **Per Tab Slider/Dropdowns/Toggles:**
   ```
   Graphics Tab:
   - Resolution Dropdown
   - Quality Dropdown
   - VSync Toggle
   - Fullscreen Toggle
   
   Audio Tab:
   - Master Volume Slider
   - Music Volume Slider
   - SFX Volume Slider
   - Voice Volume Slider
   ```

3. **Speichern als:** `SettingsMenuPanel.prefab`

**Tipp:** Nutze TextMeshPro statt Standard-Text für bessere Qualität!

---

### Task 3: UI-Assets Organisieren
**Dauer:** 30 Minuten
**Priorität:** 🟡 MEDIUM

#### Folder-Struktur erstellen:
```
Assets/
├── UI/
│   ├── Sprites/
│   │   ├── Buttons/
│   │   ├── Panels/
│   │   ├── Icons/
│   │   └── Backgrounds/
│   ├── Fonts/
│   └── Prefabs/
│       ├── Menus/
│       ├── Dialogs/
│       └── HUD/
```

#### Placeholder-Assets nutzen:
1. **PlaceholderAssetGenerator nutzen:**
   - Unity Menu → EarthUnderFreelancer → Tools → Placeholder Asset Generator
   - "Generate UI Sprites" klicken
   - Assets werden generiert in `Assets/Placeholder/UI/`

2. **Kopieren zu UI/Sprites:**
   - Sprites aus Placeholder nach UI/Sprites kopieren

---

### Task 4: UI-Animation Setup
**Dauer:** 1 Stunde
**Priorität:** 🟢 NICE-TO-HAVE

#### DOTween installieren (empfohlen):
1. **Package Manager öffnen:**
   - Window → Package Manager

2. **DOTween importieren:**
   - Asset Store durchsuchen
   - ODER: https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

3. **Setup:**
   - DOTween Setup-Panel folgen
   - "Setup DOTween" klicken

#### Alternative (ohne DOTween):
- Meine UI-Systeme haben Fallback auf einfache Lerp-Animations
- Einfach ignorieren, sieht nur weniger smooth aus

---

### Task 5: Font-Setup (Optional)
**Dauer:** 15 Minuten
**Priorität:** 🟢 OPTIONAL

#### TextMeshPro einrichten:
1. **Erste Nutzung:**
   - Beim ersten TMP-Text wird Setup-Fenster angezeigt
   - "Import TMP Essentials" klicken

2. **Eigenen Font importieren:**
   - Font-Datei (.ttf, .otf) nach Assets/UI/Fonts/ kopieren
   - Window → TextMeshPro → Font Asset Creator
   - Source Font: Dein Font
   - "Generate Font Atlas" klicken
   - Save

3. **Als Default setzen:**
   - In TMP-Settings (Window → TextMeshPro → Settings)
   - Default Font Asset: Dein Font

---

### Task 6: Input-System Konfiguration
**Dauer:** 30 Minuten
**Priorität:** 🟡 MEDIUM

#### Neue Input-System (empfohlen):
1. **Package installieren:**
   - Window → Package Manager
   - Unity Registry
   - Suche: "Input System"
   - Install

2. **Input Actions Asset erstellen:**
   - Assets/Settings/
   - Rechtsklick → Create → Input Actions
   - Name: "UIControls"

3. **Actions definieren:**
   ```
   Action Maps:
   - UI
     - Navigate (Value: Vector2)
     - Submit (Button)
     - Cancel (Button)
     - Point (Value: Vector2)
     - Click (Button)
   ```

4. **Bindings:**
   ```
   Navigate:
   - Keyboard: WASD / Arrow Keys
   - Gamepad: Left Stick / D-Pad
   
   Submit:
   - Keyboard: Enter / Space
   - Gamepad: South Button (A/Cross)
   
   Cancel:
   - Keyboard: Escape
   - Gamepad: East Button (B/Circle)
   ```

5. **Auto-Generate Class:**
   - "Generate C# Class" klicken
   - Class Name: UIControls
   - Namespace: EarthUnderFreelancer.Input

6. **In MenuInputController referenzieren:**
   - Siehe Code-Kommentare im Script

#### Alternative (Old Input):
- Meine Scripts haben Fallback auf Input.GetKeyDown()
- Funktioniert, aber weniger flexibel

---

### Task 7: Scene-Transitionen Setup
**Dauer:** 15 Minuten
**Priorität:** 🟡 MEDIUM

#### Loading Screen Scene:
1. **Neue Scene erstellen:**
   - Name: "LoadingScreen.unity"
   - Einfacher Canvas mit:
     - "Loading..." Text
     - Progress Bar (Slider)
     - Rotating Icon (Animation)

2. **Als Scene 1 hinzufügen:**
   - Build Settings → Add Scene
   - Zwischen Main Menu und Game Scene

3. **Scene-Namen in GUIManager eintragen:**
   - Siehe `sceneNames` Dictionary im Code

---

### Task 8: Context-Menu-Testing
**Dauer:** 30 Minuten
**Priorität:** 🟢 TEST

#### Test-Scene erstellen:
1. **Neue Scene:** "UI_Test.unity"

2. **GameObject mit Context-Menu:**
   - 3D-Objekt (z.B. Cube)
   - Add Component → Beispiel:
   ```csharp
   public class TestContextMenu : MonoBehaviour
   {
       void OnMouseOver()
       {
           if (Input.GetMouseButtonDown(1)) // Right-click
           {
               var menu = FindObjectOfType<ContextMenuSystem>();
               menu.ShowContextMenu(new string[] {
                   "Option 1",
                   "Option 2",
                   "---", // Separator
                   "Cancel"
               }, (index) => {
                   Debug.Log("Selected: " + index);
               });
           }
       }
   }
   ```

3. **Testen:**
   - Play-Mode
   - Right-Click auf Objekt
   - Menu sollte erscheinen

---

## 📋 Phase 4B: Quest-System - Manuelle Aufgaben

### Task 9: Quest-Content Erstellen
**Dauer:** 2-4 Stunden
**Priorität:** 🔴 KRITISCH

#### Quest-Datenbank befüllen:
1. **ScriptableObjects erstellen:**
   - Assets/Data/Quests/
   - Rechtsklick → Create → EarthUnderFreelancer → Quest

2. **Beispiel-Quest:**
   ```
   Name: "First Flight"
   Description: "Complete your first training flight"
   
   Objectives:
   - Reach altitude 1000m (Type: ReachAltitude, Value: 1000)
   - Fly for 2 minutes (Type: TimeElapsed, Value: 120)
   - Return to base (Type: ReachLocation, Value: "Base")
   
   Rewards:
   - Credits: 1000
   - XP: 500
   - Item: "Pilot License"
   ```

3. **10-20 Quests erstellen:**
   - Tutorial-Quests (5)
   - Story-Quests (10)
   - Side-Quests (5)

#### Quest-Giver-NPCs platzieren:
1. **In GameScene:**
   - Vorhandene NPCs finden (NPCBehaviorSystem)
   - Add Component → QuestGiverNPC
   - Quests zuweisen

2. **Visual-Indicator:**
   - "!" Symbol über NPC (Billboard)
   - Gelb: Quest verfügbar
   - Grau: Quest in Progress
   - Gold: Quest abschließbar

---

### Task 10: Quest-UI Design
**Dauer:** 2 Stunden
**Priorität:** 🔴 KRITISCH

#### Quest-Log-Window:
1. **Panel erstellen:**
   - Größe: 800x600
   - Tabs: Active / Completed / Failed

2. **Quest-List (Scroll-View):**
   - Vertical Layout Group
   - Quest-Item-Prefab:
     ```
     - Quest-Title (Text)
     - Quest-Description (Text)
     - Progress-Bar (Slider)
     - Track-Button (Toggle)
     ```

3. **Quest-Details-Panel:**
   - Rechte Seite
   - Shows:
     - Full Description
     - Objectives (mit Checkboxen)
     - Rewards
     - Accept/Abandon Buttons

#### Quest-Tracker (HUD):
1. **Kleines Panel:**
   - Top-Right Corner
   - Zeigt nur getrackte Quest
   - Live-Updates bei Objective-Progress

2. **Objective-Markers (3D-World):**
   - Billboard-Sprites
   - Zeigen auf Quest-Objectives
   - Distance-Display

---

## 📋 Phase 4C: Anti-Cheat - Externe Aufgaben

### Task 11: Dedicated Server Setup
**Dauer:** 1 Tag
**Priorität:** 🔴 KRITISCH (vor Launch)

#### Server-Build erstellen:
1. **Build-Settings:**
   - Platform: Linux (für Server)
   - Server Build: Enable
   - Headless Mode: Enable

2. **Separate Build-Configuration:**
   - Scripting Define: SERVER_BUILD
   - Strip UI-Components

3. **Server-Executable:**
   - Build → Linux
   - Name: "EarthUnderFreelancer_Server.x86_64"

#### Server-Hosting:
1. **Cloud-Provider wählen:**
   - AWS GameLift (empfohlen)
   - ODER: Google Cloud
   - ODER: Azure Playfab

2. **Docker-Container:**
   ```dockerfile
   FROM ubuntu:20.04
   COPY EarthUnderFreelancer_Server /app/
   EXPOSE 7777/udp
   CMD ["/app/EarthUnderFreelancer_Server.x86_64", "-batchmode", "-nographics"]
   ```

3. **Auto-Scaling einrichten:**
   - Min: 2 Server
   - Max: 100 Server
   - Scale on Player-Count

**Ich kann nicht:** Server deployen, nur Code schreiben

---

### Task 12: Ban-Database Setup
**Dauer:** 2 Stunden
**Priorität:** 🟡 MEDIUM

#### Database-Schema:
```sql
CREATE TABLE banned_players (
    player_id VARCHAR(64) PRIMARY KEY,
    ban_reason TEXT,
    ban_date TIMESTAMP,
    ban_duration INT, -- Minuten, -1 = permanent
    banned_by VARCHAR(64)
);

CREATE TABLE cheat_reports (
    report_id INT AUTO_INCREMENT PRIMARY KEY,
    reporter_id VARCHAR(64),
    reported_id VARCHAR(64),
    reason TEXT,
    evidence TEXT, -- JSON
    timestamp TIMESTAMP,
    status ENUM('pending', 'reviewed', 'actioned', 'dismissed')
);
```

#### Backend-API erstellen:
- REST-API für:
  - `POST /api/report` - Report-Player
  - `GET /api/bans/:playerId` - Check if Banned
  - `POST /api/ban` - Ban-Player (Admin)

**Ich kann nicht:** Backend-Server programmieren (außerhalb Unity)

---

## 📋 Phase 4E: Server-Infrastructure - Externe Aufgaben

### Task 13: Master-Server Setup
**Dauer:** 1 Tag
**Priorität:** 🔴 KRITISCH

#### Master-Server-API:
```
Endpoints:
- POST /api/servers/register
- POST /api/servers/heartbeat
- GET /api/servers/list
- DELETE /api/servers/unregister
```

#### Technologie:
- Node.js + Express (empfohlen)
- ODER: ASP.NET Core
- ODER: Python Flask

**Ich kann nicht:** Separate Web-Services erstellen

---

### Task 14: CDN für Assets
**Dauer:** 1 Tag
**Priorität:** 🟡 MEDIUM

#### Asset-Hosting:
1. **CDN wählen:**
   - AWS CloudFront
   - ODER: Cloudflare
   - ODER: Azure CDN

2. **Upload-Pipeline:**
   - Asset-Bundles hochladen
   - Versioning
   - Caching-Headers

3. **Download-URLs:**
   - In AssetStreamingManager konfigurieren

**Ich kann nicht:** CDN-Setup, nur Code für Download

---

## 📋 Phase 4H: Launcher - Komplett Extern

### Task 15: Standalone-Launcher Programmieren
**Dauer:** 1 Woche
**Priorität:** 🟢 OPTIONAL

#### Technologie-Stack:
- Electron.js (empfohlen für Cross-Platform)
- ODER: .NET WPF (nur Windows)
- ODER: Qt (C++)

#### Features:
- News-Feed
- Play-Button
- Settings
- Auto-Update-Check
- Download-Manager
- Patch-Notes

**Ich kann NICHT:** Komplett außerhalb meiner Fähigkeiten

**Alternative:** In-Game-Updater (das kann ich)

---

## ✅ Zusammenfassung - Was DU Machen Musst

### Sofort (für Phase 4A):
- [ ] Task 1: Main Menu Scene (30 Min)
- [ ] Task 2: UI-Prefabs (1-2h)
- [ ] Task 3: UI-Assets organisieren (30 Min)

### Diese Woche (für Phase 4B):
- [ ] Task 9: Quest-Content erstellen (2-4h)
- [ ] Task 10: Quest-UI Design (2h)

### Vor Launch (für Phase 4C):
- [ ] Task 11: Dedicated Server (1 Tag)
- [ ] Task 12: Ban-Database (2h)

### Später (für Phase 4E):
- [ ] Task 13: Master-Server (1 Tag)
- [ ] Task 14: CDN Setup (1 Tag)

### Optional (für Phase 4H):
- [ ] Task 15: Standalone-Launcher (1 Woche)

---

## 📞 Support

**Falls Probleme:**
1. Frage mich im PR-Comment
2. Ich kann Code anpassen/fixen
3. Ich kann Erklärungen geben
4. Ich kann keine Unity-Editor-Actions ausführen

**Was ich helfen kann:**
- ✅ Code debuggen
- ✅ Systeme erklären
- ✅ Best-Practices
- ✅ Integration-Code

**Was ich NICHT kann:**
- ❌ Unity-Editor bedienen
- ❌ Assets erstellen
- ❌ Server deployen
- ❌ Externe Apps programmieren

---

**Viel Erfolg!** 🚀
Ich implementiere jetzt Phase 4A Code-Systems.
