# Schnellstart / Quick Start Guide

## Deutsch

### Installation

```bash
# 1. Template kopieren
cp -r examples/cross-platform-game-system mein-game-system
cd mein-game-system

# 2. Dependencies installieren
npm install

# 3. System einrichten
npm run setup
```

### Spiel hinzufügen

```bash
# Automatische Erkennung (Steam Spiele)
npm run detect-games

# Manuell hinzufügen
npm run add-game -- \
  --name "Mein Spiel" \
  --path "/pfad/zum/spiel" \
  --executable "spiel.exe" \
  --platform windows \
  --layer proton
```

### Spiel starten

```bash
npm start launch "Mein Spiel"
```

### Verfügbare Spiele anzeigen

```bash
npm start list
```

---

## English

### Installation

```bash
# 1. Copy template
cp -r examples/cross-platform-game-system my-game-system
cd my-game-system

# 2. Install dependencies
npm install

# 3. Run setup
npm run setup
```

### Add a Game

```bash
# Automatic detection (Steam games)
npm run detect-games

# Add manually
npm run add-game -- \
  --name "My Game" \
  --path "/path/to/game" \
  --executable "game.exe" \
  --platform windows \
  --layer proton
```

### Launch a Game

```bash
npm start launch "My Game"
```

### List Available Games

```bash
npm start list
```

## Unterstützte Kompatibilitätsschichten / Supported Compatibility Layers

- **proton**: Valve's optimierte Wine-Version für Spiele / Valve's optimized Wine for games
- **wine**: Standard Wine für Windows-Anwendungen / Standard Wine for Windows applications
- **native**: Native Linux/Unix Spiele / Native Linux/Unix games

## Beispiel / Example

```javascript
// Programmatically add and launch a game
const GameSystem = require('./src');

async function example() {
  const system = new GameSystem();
  await system.initialize();
  
  // Add a game
  system.library.addGame({
    name: 'The Witcher 3',
    path: '/games/witcher3',
    executable: 'witcher3.exe',
    platform: 'windows',
    compatibilityLayer: 'proton'
  });
  
  await system.library.save();
  
  // Launch the game
  await system.launch('The Witcher 3');
}

example();
```

## Konfiguration anpassen / Customize Configuration

Bearbeiten Sie `config/settings.json`:
Edit `config/settings.json`:

```json
{
  "defaultCompatibilityLayer": "proton",
  "wine": {
    "dxvk": true,
    "vkd3d": true
  }
}
```
