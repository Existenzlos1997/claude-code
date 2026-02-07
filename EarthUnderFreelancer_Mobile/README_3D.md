# 🌍 EarthUnderFreelancer 3D - Mobile Browser Game

## Vollständige 3D-Weltraum-Simulation im Browser

Eine komplett funktionale 3D-Version des EarthUnderFreelancer MMORPG, optimiert für Smartphones und basierend auf dem Hauptspiel mit 198 C# Scripts.

## 🎮 Features

### Weltraum-Umgebung
- **🌍 Erde** - Realistische Darstellung mit Atmosphären-Glow und Rotation
- **🌙 Mond** - In realistischer Umlaufbahn (60 Sekunden pro Orbit)
- **☄️ 25 Asteroiden** - Navigations-Hindernisse mit Kollisions-Physik
- **⭐ 2000+ Sterne** - Echte 3D-Tiefe im Weltraum
- **☀️ Sonne** - Realistische Beleuchtung als Directional Light

### Gameplay
- **6 Fraktionen** - Aus dem Hauptspiel:
  - 🛡️ UEDF - United Earth Defense Force
  - 💰 FTC - Free Trade Consortium
  - 🔬 Explorers Guild
  - ☠️ Crimson Syndicate
  - ⚙️ Technocratic Enclave
  - 🌟 Colonial Independence Movement

- **3D Flight Combat** - Vollständige 3D-Bewegungsfreiheit
- **Touch-Steuerung** - Optimiert für mobile Geräte
- **Mission-System** - 10 feindliche Jäger eliminieren
- **Gesundheit & Schilde** - Zweistufiges Schadenssystem
- **Score-Tracking** - Mit localStorage High-Score

### Technische Features
- **Three.js r158** - WebGL 3D-Rendering
- **Single HTML File** - ~33 KB, komplett standalone
- **Mobile-Optimiert** - 30+ FPS auf modernen Smartphones
- **Responsive Design** - Funktioniert auf allen Bildschirmgrößen
- **Offline-Fähig** - Nach erstem Laden (CDN-Cache)
- **localStorage** - Speichert High-Score

## 🎯 Steuerung

### Touch (Smartphone/Tablet)
- **Touch & Drag** auf Bildschirm:
  - Links/Rechts → Schiff dreht (Yaw)
  - Oben/Unten → Schiff neigt (Pitch)
- **🔥 Fire Button** (unten links) → Waffe feuern
- **⚡ Boost Button** (unten rechts) → Turbo-Geschwindigkeit

### Maus & Tastatur (Desktop)
- **Maus bewegen** (gedrückt halten) → Schiff steuern
- **Leertaste** → Waffe feuern
- **Shift** → Boost aktivieren

## 📊 HUD-Elemente

- **Fraktion** - Gewählte Fraktion
- **Gesundheit** - Grüner Balken (0-100)
- **Schild** - Cyan Balken (0-100, absorbiert Schaden zuerst)
- **Score** - Punkte (100 pro Kill)
- **Kills** - Fortschritt zur Mission (X / 10)
- **Entfernung** - Distanz zum nächsten Feind (in Metern)

## 🎨 Visuelle Features

### Beleuchtung
- **Ambient Light** - 0.3 Intensität (Weltraum-Grundlicht)
- **Directional Light** - 1.0 Intensität (Sonne)
- **Emissive Materials** - Leuchtende Schiffstriebwerke

### Partikel-Effekte
- **Explosionen** - 30 Partikel pro Explosion
  - Sphärische Ausbreitung
  - Orange Farbe
  - 1-2 Sekunden Lebensdauer
  - Alpha-Fade

### Materialien
- **Phong Material** - Raumschiffe (reflektierend)
- **Basic Material** - Sterne & Partikel
- **Fraktionsfarben** - Jede Fraktion hat eigene Farbe

## 🌌 Immersions-Elemente

- **Schwarzer Hintergrund** - Echter Weltraum
- **360° Sternenhimmel** - Unendliche Weite
- **Erde als Heimat** - Immer im Hintergrund sichtbar
- **Mond-Orbit** - Realistische Orbital-Mechanik
- **Asteroiden-Navigation** - Taktisches Element
- **Keine Schwerkraft** - Freie Bewegung in alle Richtungen

## 📱 Browser-Kompatibilität

### Getestet & Funktional
- ✅ Chrome Mobile 90+ (Android)
- ✅ Safari 14+ (iOS)
- ✅ Firefox Mobile 88+
- ✅ Samsung Internet 14+
- ✅ Desktop-Browser (Chrome, Firefox, Safari, Edge)

### Anforderungen
- **WebGL 1.0** Support (Standard seit 2011)
- **ES6 JavaScript** (Standard seit 2015)
- **Touch Events** API (für mobile Steuerung)
- **~100 MB** freier RAM
- **GPU** mit Hardware-Beschleunigung

## 🚀 Wie spielen?

### Online
Öffne einfach die HTML-Datei im Browser:
```
earth_freelancer_3d.html
```

### Als App installieren

**iOS (Safari):**
1. Öffne die Datei in Safari
2. Tippe auf "Teilen"-Button
3. Wähle "Zum Home-Bildschirm"

**Android (Chrome):**
1. Öffne die Datei in Chrome
2. Tippe auf Menü (⋮)
3. Wähle "Zum Startbildschirm hinzufügen"

## 🎯 Gameplay-Tipps

1. **Schilde zuerst** - Schilde absorbieren Schaden vor Gesundheit
2. **Asteroiden vermeiden** - Kollisionen verursachen Schaden
3. **Boost sparsam** - 2 Sekunden Cooldown
4. **Entfernung beachten** - Näher ist einfacher zu treffen
5. **Bewegung nutzen** - Feinden ausweichen durch Manöver

## 🏆 Score-System

- **Kill** - 100 Punkte
- **High Score** - Wird in localStorage gespeichert
- **Neuer High Score** - Zeigt 🏆 Symbol

## 💾 Speicherung

Das Spiel speichert automatisch in localStorage:
- High Score
- Wird beim nächsten Besuch geladen

## 🔧 Performance

### Optimierungen
- **Adaptive Rendering** - Passt sich der Hardware an
- **Object Pooling** - Projektile & Partikel wiederverwendet
- **Frustum Culling** - Automatisch durch Three.js
- **LOD System** - Geplant für zukünftige Versionen

### Benchmark
- **Ziel FPS:** 30+
- **Erreicht:** 30-60 (Hardware-abhängig)
- **Memory:** < 100 MB
- **Ladezeit:** < 5 Sekunden

## 🌟 Unterschied zur 2D-Version

| Feature | 2D Version | 3D Version |
|---------|------------|------------|
| Rendering | Canvas 2D | Three.js WebGL |
| Erde | ❌ | ✅ Mit Atmosphäre |
| Mond | ❌ | ✅ In Orbit |
| Asteroiden | ❌ | ✅ 25 Stück |
| Sterne | 2D Punkte | 3D mit Tiefe |
| Kamera | Statisch | Follow-Cam |
| Bewegung | 2D Ebene | 3D Raum |
| Immersion | Basic | Vollständig |

## 🎓 Basiert auf

Dieses Spiel ist eine Browser-Adaption des vollständigen **EarthUnderFreelancer MMORPG**:
- **198 C# Scripts** im Hauptprojekt
- **6 Fraktionen** aus dem originalen Lore
- **AAA-Qualität** Gameplay-Mechaniken
- **Unity-Engine** Hauptversion

## 📖 Weitere Dokumentation

- `README.md` - Übersicht aller Versionen
- `DIREKTER_ZUGANG_3D.md` - Quick-Start Guide
- `PROJEKT_ZUSAMMENFASSUNG.md` - Vollständige Details

## 🐛 Bekannte Einschränkungen

- **Keine Sounds** - Aus Performance-Gründen (kann später hinzugefügt werden)
- **Einfache AI** - Feinde fliegen direkt auf Spieler zu
- **Keine Multiplayer** - Single-Player nur
- **Begrenzte Missionen** - Nur ein Missions-Typ

## 🔮 Geplante Features

- Sound-Effekte (Web Audio API)
- Verschiedene Missionstypen
- Power-Ups
- Boss-Fights
- Online-Leaderboard
- Achievements

## 📄 Lizenz

Teil des EarthUnderFreelancer-Projekts.

---

**Viel Spaß beim Fliegen, Pilot! 🚀✨**
