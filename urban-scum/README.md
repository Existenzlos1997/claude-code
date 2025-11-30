# Urban Scum 🎮

Ein süchtig machendes Mobile-Plattformspiel im Cartoon-Style!

![Urban Scum Banner](src/assets/banner.png)

## 🎯 Features

- **3 Abwechslungsreiche Levels:** Downtown, Industrial und Nightlife
- **Süchtig machender Spielloop:** Einfache Steuerung, schnelles Gameplay
- **Power-Ups:** Speed Boost, Schild, Magnet, Doppelpunkte
- **Punktesystem mit Combo:** Sammle Münzen und besiege Gegner
- **Highscore-System:** Speichere deine besten Ergebnisse
- **Level-Fortschritt:** Automatisches Speichern
- **AdMob-Integration:** Interstitial und Rewarded Ads
- **Modernes Cartoon-Design:** Bunte, ansprechende Grafiken

## 🚀 Schnellstart

### Im Browser spielen

1. Öffne `index.html` in deinem Browser
2. Oder starte einen lokalen Server:

```bash
npx serve .
```

### Android-App erstellen

1. Dependencies installieren:
```bash
npm install
```

2. Capacitor initialisieren:
```bash
npx cap init "Urban Scum" com.urbanscum.game --web-dir .
npx cap add android
npx cap sync
```

3. In Android Studio öffnen:
```bash
npx cap open android
```

4. Release-Build erstellen (Android Studio):
   - Build > Generate Signed Bundle / APK
   - Android App Bundle wählen
   - Keystore erstellen/auswählen
   - Build starten

## 🎮 Steuerung

### Touch-Steuerung (Mobile)
- **Links/Rechts:** Touch-Buttons am unteren Bildschirmrand
- **Springen:** Blitz-Button

### Tastatur (Desktop)
- **A/D oder Pfeiltasten:** Links/Rechts bewegen
- **W, Pfeil hoch oder Leertaste:** Springen

## 📁 Projektstruktur

```
urban-scum/
├── index.html              # Haupt-HTML-Datei
├── manifest.json           # PWA-Manifest
├── sw.js                   # Service Worker
├── package.json            # NPM-Konfiguration
├── capacitor.config.json   # Capacitor-Konfiguration
├── src/
│   ├── css/
│   │   └── style.css       # Alle Styles
│   ├── js/
│   │   ├── utils.js        # Hilfsfunktionen
│   │   ├── storage.js      # LocalStorage Manager
│   │   ├── audio.js        # Sound-Manager
│   │   ├── sprites.js      # Sprite-Generator
│   │   ├── entities.js     # Spielobjekte
│   │   ├── levels.js       # Level-Definitionen
│   │   ├── game.js         # Game-Engine
│   │   ├── ui.js           # UI-Controller
│   │   ├── ads.js          # AdMob-Integration
│   │   └── main.js         # Einstiegspunkt
│   └── assets/
│       └── (Icons & Bilder)
└── docs/
    └── PLAY_STORE_UPLOAD.md # Upload-Anleitung
```

## 🎨 Grafiken

Alle Grafiken werden programmatisch über Canvas generiert - keine externen Assets erforderlich! Dies umfasst:

- Spielercharakter (mit Animationen)
- 3 Gegnertypen (Ratte, Thug, Roboter)
- Sammelobjekte (Münzen, Edelsteine)
- Power-Ups (Speed, Schild, Magnet, x2)
- Plattformen (Normal, Gefahr, Bewegend)
- Hindernisse (Stacheln, Fass, Mülleimer)
- Hintergründe (3 verschiedene)
- Partikeleffekte

## 🔊 Audio

Audio wird ebenfalls programmatisch mit der Web Audio API erzeugt:

- Sprung-Sound
- Sammel-Sound
- Power-Up-Sound
- Treffer-Sound
- Gegner-Besiegt-Sound
- Game-Over-Sound
- Level-Complete-Sound
- Combo-Sound
- Hintergrundmusik

## 📱 AdMob-Einrichtung

1. Erstelle ein [AdMob-Konto](https://admob.google.com)
2. Füge eine neue Android-App hinzu
3. Erstelle Anzeigenblöcke:
   - Interstitial
   - Rewarded (Belohnungsvideo)
4. Ersetze die Test-IDs in `src/js/ads.js`:

```javascript
// Ersetze diese mit deinen echten IDs
appId: 'ca-app-pub-DEINE-APP-ID',
interstitialId: 'ca-app-pub-DEINE-INTERSTITIAL-ID',
rewardedId: 'ca-app-pub-DEINE-REWARDED-ID',
useTestAds: false // Auf false für Produktion
```

## 📦 Play Store veröffentlichen

Siehe [PLAY_STORE_UPLOAD.md](docs/PLAY_STORE_UPLOAD.md) für eine detaillierte Anleitung.

## 🛠️ Anpassung

### Neue Level hinzufügen

Bearbeite `src/js/levels.js` und füge ein neues Level-Objekt hinzu:

```javascript
4: {
    name: "DEIN LEVEL",
    icon: "🏰",
    background: "bg_custom",
    width: 5000,
    // ... weitere Konfiguration
}
```

### Neue Gegner hinzufügen

1. Erstelle ein neues Sprite in `src/js/sprites.js`
2. Füge den Typ in `src/js/entities.js` > `Enemy.setTypeProperties()` hinzu
3. Verwende den neuen Typ in Level-Definitionen

### Neue Power-Ups hinzufügen

1. Erstelle ein Sprite in `src/js/sprites.js`
2. Füge Logik in `src/js/entities.js` > `Player.collectPowerUp()` hinzu
3. Füge das Power-Up in Level-Definitionen ein

## 📄 Lizenz

MIT License - Frei verwendbar für kommerzielle und private Projekte.

## 🤝 Beitragen

Pull Requests sind willkommen! Für größere Änderungen bitte zuerst ein Issue öffnen.

---

Made with ❤️ for mobile gaming!
