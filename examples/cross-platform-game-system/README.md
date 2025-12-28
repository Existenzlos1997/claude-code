# Cross-Platform Game Compatibility System

Ein System zur plattformübergreifenden Ausführung von PC-Spielen auf Windows, Linux und anderen Betriebssystemen **mit einzigartigen KI-Funktionen**.

A system for running PC games across platforms - Windows, Linux, and other operating systems **with unique AI-powered features**.

## 🚀 Was macht dieses System einzigartig? / What Makes This Unique?

**Keine andere Lösung (Lutris, PlayOnLinux, Bottles) bietet:**
**No other solution (Lutris, PlayOnLinux, Bottles) offers:**

- 🤖 **AI-Powered Auto-Optimization** - Hardware-Analyse und automatische Spiel-Optimierung
- 💾 **Cross-Save Synchronization** - Spielstände über Wine/Proton/Native synchronisieren  
- 🎨 **Intelligent Shader Management** - Pre-Compilation und Cache-Sharing zwischen Spielen
- 📊 **Compatibility Prediction** - KI sagt vorher ob Spiel läuft vor Installation

➡️ Siehe [INNOVATIVE_FEATURES.md](docs/INNOVATIVE_FEATURES.md) für Details

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
  - Vordefinierte Profile für beliebte Spiele / Predefined profiles for popular games

- **Anti-Cheat-Unterstützung** / Anti-Cheat Support
  - EasyAntiCheat (EAC) und BattlEye Unterstützung / EasyAntiCheat and BattlEye support
  - Automatische Erkennung von Anti-Cheat-Systemen / Automatic anti-cheat detection
  - Kernel-Ebene Emulation für Kompatibilität / Kernel-level emulation for compatibility
  - Windows-Komponenten-Imitation / Windows component imitation

- **Performance & Monitoring** / Performance & Monitoring
  - Performance-Überwachung während des Spielens / Performance monitoring during gameplay
  - CPU/RAM-Metriken / CPU/RAM metrics
  - Benchmark-Berichte / Benchmark reports
  - Zentrale Logging / Centralized logging

- **🤖 AI-Powered Features** / AI-Powered Features ⭐ UNIQUE
  - **Automatische Optimierung**: KI analysiert Hardware und optimiert Einstellungen / AI analyzes hardware and optimizes settings
  - **Kompatibilitäts-Vorhersage**: Vorhersage ob Spiel läuft vor Installation / Predict if game works before install
  - **Cross-Save Sync**: Spielstände über Wine/Proton/Native synchronisieren / Sync saves across Wine/Proton/Native
  - **Shader Pre-Compilation**: Shader vorab kompilieren, Stottern eliminieren / Pre-compile shaders, eliminate stuttering
  - **Intelligente Cache-Verwaltung**: Shader-Cache zwischen Spielen teilen / Share shader cache between games

- **Benutzerfreundlichkeit** / User Experience
  - Interaktiver Einrichtungsassistent / Interactive setup wizard
  - Vordefinierte Spiel-Profile / Predefined game profiles
  - Umfassende Fehlerbehebung / Comprehensive troubleshooting
  - Mehrsprachige Dokumentation / Multilingual documentation

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

# Run interactive configuration wizard (recommended)
npm run wizard

# Or configure manually
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

# Mit Anti-Cheat-Unterstützung / With anti-cheat support
npm start launch "Apex Legends" -- --anticheat eac

# Via UI
npm run gui
```

### Spiel-Profile / Game Profiles

```bash
# Liste aller verfügbaren Profile / List all available profiles
npm run list-profiles

# Zeige Details für ein Spiel / Show details for a game
npm run show-profile apex-legends

# Suche nach Profilen / Search for profiles
npm run search-profiles "witcher"
```

Das System enthält vordefinierte Profile für beliebte Spiele mit optimierten Einstellungen.
The system includes predefined profiles for popular games with optimized settings.

### Anti-Cheat-Kompatibilität prüfen / Check Anti-Cheat Compatibility

```bash
# Überprüfe Anti-Cheat für ein Spiel / Check anti-cheat for a game
npm run check-anticheat "Game Name"
```

Das System erkennt automatisch Anti-Cheat-Systeme wie EasyAntiCheat und BattlEye und konfiguriert die notwendigen Komponenten.

The system automatically detects anti-cheat systems like EasyAntiCheat and BattlEye and configures the necessary components.

**Unterstützte Anti-Cheat-Systeme / Supported Anti-Cheat Systems:**
- ✅ EasyAntiCheat (EAC) - via Proton
- ✅ BattlEye - via Proton
- ✅ Valve Anti-Cheat (VAC)
- ⚠️ Denuvo Anti-Tamper - teilweise / partial
- ❌ Riot Vanguard - nicht unterstützt / not supported
- ❌ FACEIT AC - nicht unterstützt / not supported

Siehe [docs/ANTICHEAT.md](docs/ANTICHEAT.md) für Details.
See [docs/ANTICHEAT.md](docs/ANTICHEAT.md) for details.

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

# Simulierter End-to-End-Test (keine Spiele erforderlich)
# Simulated end-to-end test (no games required)
npm run simulate

# Run tests
npm test

# Run all tests
npm run test:all

# Build
npm run build
```

### Simulierter Systemtest / Simulated System Test

Das System verfügt über einen umfassenden Simulationstest, der alle Komponenten ohne echte Spiele testet:
The system includes a comprehensive simulation test that validates all components without real games:

```bash
npm run simulate
```

**Was wird getestet / What is tested:**
- ✅ Game Library (CRUD operations)
- ✅ Compatibility Manager (Plugin loading)
- ✅ Anti-Cheat Detection (EAC, BattlEye)
- ✅ Input Validation
- ✅ Game Profiles
- ✅ Performance Monitor
- ✅ Logger
- ✅ Integration workflows

**36 Tests in 8 Kategorien / 36 tests across 8 categories**

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

- **Anti-Cheat-Systeme / Anti-Cheat Systems**: Einige Anti-Cheat-Systeme wie Riot Vanguard funktionieren nicht unter Wine/Proton, da sie Kernel-Treiber benötigen. Siehe [docs/ANTICHEAT.md](docs/ANTICHEAT.md) für Lösungen.
  
  Some anti-cheat systems like Riot Vanguard don't work under Wine/Proton as they require kernel drivers. See [docs/ANTICHEAT.md](docs/ANTICHEAT.md) for solutions.

- **DirectX 12 Unterstützung / DirectX 12 Support**: DirectX 12 Unterstützung ist experimentell via VKD3D.
  
  DirectX 12 support is experimental via VKD3D.

## Mitwirken / Contributing

Siehe [CONTRIBUTING.md](CONTRIBUTING.md) für Details.
See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## Dokumentation / Documentation

- **[README.md](README.md)** - Hauptdokumentation / Main documentation
- **[INNOVATIVE_FEATURES.md](docs/INNOVATIVE_FEATURES.md)** - Einzigartige KI-Features / Unique AI features ⭐ NEW
- **[QUICKSTART.md](docs/QUICKSTART.md)** - Schnellstart-Anleitung / Quick start guide
- **[ANTICHEAT.md](docs/ANTICHEAT.md)** - Anti-Cheat-Unterstützung / Anti-cheat support
- **[TESTING.md](docs/TESTING.md)** - Test-Anleitung / Testing guide
- **[TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)** - Fehlerbehebung / Troubleshooting ⭐ NEW
- **[COMPLETE-GUIDE.md](docs/COMPLETE-GUIDE.md)** - Vollständige Referenz / Complete reference

## Lizenz / License

MIT License - siehe [LICENSE](LICENSE)

## Ressourcen / Resources

- [Wine HQ](https://www.winehq.org/)
- [Proton](https://github.com/ValveSoftware/Proton)
- [DXVK](https://github.com/doitsujin/dxvk)
- [ProtonDB](https://www.protondb.com/) - Spiele-Kompatibilitätsdatenbank
- [Lutris](https://lutris.net/) - Inspiration für Spiele-Management
