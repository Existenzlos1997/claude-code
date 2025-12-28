# Cross-Platform Game Compatibility System

Ein System zur plattformübergreifenden Ausführung von PC-Spielen auf Windows, Linux und anderen Betriebssystemen.

A system for running PC games across platforms - Windows, Linux, and other operating systems.

## Übersicht / Overview

Dieses Projekt ermöglicht es, Spiele (neu und alt) auf verschiedenen Betriebssystemen auszuführen, unabhängig von der ursprünglichen Zielplattform.

This project enables running games (new and old) on different operating systems, regardless of the original target platform.

## Hauptfunktionen / Key Features

- **Plattformübergreifende Kompatibilität** / Cross-platform compatibility
  - Windows-Spiele auf Linux ausführen / Run Windows games on Linux
  - Unterstützung für alte und neue Spiele / Support for old and new games
  - Mehrere Kompatibilitätsschichten / Multiple compatibility layers

- **Modulare Architektur** / Modular Architecture
  - Plugin-System für verschiedene Kompatibilitätstools / Plugin system for different compatibility tools
  - Flexible Konfiguration / Flexible configuration
  - Erweiterbare Plattformunterstützung / Extensible platform support

- **Spiele-Bibliotheksverwaltung** / Game Library Management
  - Zentrale Verwaltung installierter Spiele / Central management of installed games
  - Automatische Erkennung von Spielen / Automatic game detection
  - Profilverwaltung und Einstellungen / Profile management and settings

## Architektur / Architecture

```
cross-platform-game-system/
├── src/
│   ├── core/              # Kernsystem / Core system
│   ├── compatibility/     # Kompatibilitätsschichten / Compatibility layers
│   ├── launchers/         # Spiele-Launcher / Game launchers
│   ├── detection/         # Spiele-Erkennung / Game detection
│   └── config/            # Konfigurationsverwaltung / Configuration management
├── plugins/
│   ├── wine/              # Wine-Integration für Windows-Spiele
│   ├── proton/            # Proton-Unterstützung
│   └── native/            # Native Spiele
├── docs/                  # Dokumentation
└── tests/                 # Tests
```

## Technologie-Stack / Technology Stack

### Kompatibilitätsschichten / Compatibility Layers
- **Wine**: Windows-Anwendungen auf Unix-Systemen / Windows applications on Unix systems
- **Proton**: Valve's Wine-basierte Lösung für Spiele / Valve's Wine-based solution for games
- **DXVK**: DirectX zu Vulkan Übersetzung / DirectX to Vulkan translation
- **VKD3D**: Direct3D 12 zu Vulkan / Direct3D 12 to Vulkan

### Laufzeitumgebung / Runtime Environment
- **Node.js**: Backend und Orchestrierung / Backend and orchestration
- **Electron** (optional): Desktop-Anwendung / Desktop application
- **Python**: Skripte und Tools / Scripts and tools

## Installation

### Voraussetzungen / Prerequisites

```bash
# Linux
sudo apt-get install wine-stable winetricks

# macOS
brew install wine-stable

# Node.js dependencies
npm install
```

### Setup

```bash
# Clone repository
git clone <repository-url>
cd cross-platform-game-system

# Install dependencies
npm install

# Configure system
npm run setup

# Start the system
npm start
```

## Verwendung / Usage

### Spiel hinzufügen / Add a Game

```bash
# Automatische Erkennung / Automatic detection
npm run detect-games

# Manuell hinzufügen / Manual add
npm run add-game -- --path "/path/to/game" --platform windows
```

### Spiel starten / Launch a Game

```bash
# Via CLI
npm run launch -- --game "Game Name"

# Via UI
npm run gui
```

### Konfiguration / Configuration

Bearbeiten Sie `config/settings.json` für globale Einstellungen:
Edit `config/settings.json` for global settings:

```json
{
  "defaultCompatibilityLayer": "proton",
  "winePrefixPath": "~/.wine-games",
  "autoDetectGames": true,
  "platforms": ["windows", "linux", "native"]
}
```

## Unterstützte Plattformen / Supported Platforms

- ✅ Windows (via Wine/Proton)
- ✅ Linux (Native + Wine/Proton)
- ⚠️ macOS (Limited Wine support)
- 🔜 FreeBSD (Planned)

## Entwicklung / Development

```bash
# Development mode
npm run dev

# Run tests
npm test

# Build
npm run build
```

## Plugin-Entwicklung / Plugin Development

Erstellen Sie ein Plugin im `plugins/` Verzeichnis:
Create a plugin in the `plugins/` directory:

```javascript
// plugins/my-compatibility-layer/index.js
module.exports = {
  name: 'my-compatibility-layer',
  platform: 'windows',
  
  async canRun(game) {
    // Check if this plugin can run the game
    return true;
  },
  
  async launch(game, options) {
    // Launch the game
  }
};
```

## Roadmap

- [x] Grundlegende Architektur / Basic architecture
- [ ] Wine/Proton Integration
- [ ] Automatische Spielerkennung / Automatic game detection
- [ ] GUI mit Electron
- [ ] Steam/Epic/GOG Integration
- [ ] Performance-Optimierungen / Performance optimizations
- [ ] Cloud-Synchronisierung / Cloud sync
- [ ] Controller-Unterstützung / Controller support

## Bekannte Probleme / Known Issues

- Einige Anti-Cheat-Systeme funktionieren nicht unter Wine
  Some anti-cheat systems don't work under Wine
- DirectX 12 Unterstützung ist experimentell
  DirectX 12 support is experimental

## Mitwirken / Contributing

Siehe [CONTRIBUTING.md](CONTRIBUTING.md) für Details.
See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## Lizenz / License

MIT License - siehe [LICENSE](LICENSE)

## Ressourcen / Resources

- [Wine HQ](https://www.winehq.org/)
- [Proton](https://github.com/ValveSoftware/Proton)
- [DXVK](https://github.com/doitsujin/dxvk)
- [Lutris](https://lutris.net/) - Inspiration für Spiele-Management
