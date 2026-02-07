# 🎮 WIE BEKOMME ICH DAS SPIEL AUF MEINEN PC?

## Schritt 1: Repository herunterladen

### Option A: Als ZIP herunterladen (einfachste Methode)
1. Öffne deinen Browser
2. Gehe zu: **https://github.com/Existenzlos1997/claude-code**
3. Klicke auf den grünen **"Code"** Button
4. Klicke auf **"Download ZIP"**
5. Speichere die ZIP-Datei (z.B. auf dem Desktop)
6. Rechtsklick auf die ZIP → **"Alle extrahieren"**
7. Das Spiel-Projekt ist jetzt im Ordner: `claude-code-main/EarthUnderFreelancer/`

### Option B: Mit Git klonen
```bash
git clone https://github.com/Existenzlos1997/claude-code.git
cd claude-code/EarthUnderFreelancer
```

---

## Schritt 2: Unity installieren

1. Gehe zu: **https://unity.com/download**
2. Lade **Unity Hub** herunter und installiere es
3. Öffne Unity Hub
4. Klicke links auf **"Installs"**
5. Klicke auf **"Install Editor"**
6. Wähle **Unity 2022.3 LTS** (Long Term Support)
7. Bei den Modulen aktiviere:
   - ✅ Windows Build Support
   - ✅ Android Build Support (optional, für Android)
8. Klicke auf **"Install"** und warte (ca. 5-10 GB Download)

---

## Schritt 3: Projekt in Unity öffnen

1. Öffne **Unity Hub**
2. Klicke auf **"Open"** (oben rechts)
3. Navigiere zu dem Ordner wo du das Projekt entpackt hast:
   - `claude-code-main/EarthUnderFreelancer/`
4. Wähle den Ordner **"EarthUnderFreelancer"** aus
5. Klicke auf **"Open"**
6. Unity fragt vielleicht nach der Version - wähle **Unity 2022.3 LTS**
7. **Warte** - der erste Import dauert einige Minuten!

---

## Schritt 4: Das Spiel als EXE erstellen

Wenn Unity das Projekt geöffnet hat:

1. Gehe im Menü zu: **EarthUnderFreelancer → 🚀 Build Portable Windows EXE**
2. Wähle einen Speicherort (z.B. Desktop)
3. Klicke auf **"Build"**
4. Warte bis der Build fertig ist

**FERTIG!** Die spielbare EXE findest du jetzt im Ordner `Builds/Windows/`

---

## Schritt 5: Spiel starten

1. Gehe in den Ordner `Builds/Windows/`
2. Doppelklicke auf **EarthUnderFreelancer.exe**
3. Spiel läuft! 🎮

---

## ❓ Häufige Fragen

### Wo finde ich das Spiel nach dem Download?
- Wenn du als ZIP heruntergeladen hast: Dort wo du es entpackt hast
- Standard: `Downloads/claude-code-main/EarthUnderFreelancer/`

### Wo ist die fertige EXE?
- Nach dem Build: `EarthUnderFreelancer/Builds/Windows/EarthUnderFreelancer.exe`

### Brauche ich Unity um zu spielen?
- **Zum Erstellen der EXE:** Ja, einmalig
- **Zum Spielen:** Nein, die EXE läuft ohne Unity

### Kann ich die EXE an Freunde schicken?
- Ja! Kopiere den ganzen `Windows` Ordner (EXE + alle anderen Dateien)

---

## 📁 Ordnerstruktur

```
claude-code/
└── EarthUnderFreelancer/          ← DAS IST DAS UNITY-PROJEKT
    ├── Assets/
    │   ├── Scripts/               ← Alle 105+ C# Scripts
    │   ├── Editor/                ← Build-Scripts
    │   └── ...
    ├── Documentation/             ← Anleitungen
    ├── Installer/                 ← Installer-Scripts
    ├── ProjectSettings/           ← Unity-Einstellungen
    └── README.md

Nach dem Build:
└── Builds/
    └── Windows/
        └── EarthUnderFreelancer.exe  ← DEIN FERTIGES SPIEL!
```

---

## 🆘 Hilfe

Falls etwas nicht funktioniert:
1. Stelle sicher dass Unity 2022.3 LTS installiert ist
2. Warte bis Unity alle Assets importiert hat (kann beim ersten Mal dauern)
3. Prüfe ob der `EarthUnderFreelancer` Ordner einen `Assets` Unterordner hat
