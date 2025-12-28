# Testing Guide / Test-Anleitung

## Deutsch

### Übersicht

Das System verfügt über ein umfassendes Test-Framework:
- **Unit Tests**: Testen einzelner Komponenten
- **Integration Tests**: Testen des Zusammenspiels
- **Validierung**: Systemprüfung und Konfiguration
- **End-to-End Tests**: Vollständige Workflows

### Tests ausführen

```bash
# Alle Unit Tests
npm test

# Mit Code Coverage
npm run test:coverage

# Im Watch-Modus (für Entwicklung)
npm run test:watch

# Vollständige Test-Suite
npm run test:all

# System-Validierung
npm run validate
```

### Test-Struktur

```
tests/
├── game-library.test.js        # Game Library Tests
├── anticheat.test.js           # Anti-Cheat Detection Tests
├── compatibility-manager.test.js # Plugin Manager Tests
├── validator.test.js           # Input Validation Tests
└── integration.test.js         # End-to-End Tests
```

### Code Coverage Ziele

- Branches: 70%+
- Functions: 70%+
- Lines: 70%+
- Statements: 70%+

### Manuelle Tests

#### 1. System Setup Test
```bash
npm run setup
# Erwartung: Konfigurationsverzeichnis wird erstellt
```

#### 2. Spiel hinzufügen
```bash
npm run add-game -- --name "Test Game" --path "/path/to/game" --executable "game.exe" --platform windows
# Erwartung: Spiel wird zur Bibliothek hinzugefügt
```

#### 3. Spiele auflisten
```bash
npm start list
# Erwartung: Liste aller Spiele wird angezeigt
```

#### 4. Anti-Cheat Check
```bash
npm run check-anticheat "Test Game"
# Erwartung: Anti-Cheat-Analyse wird angezeigt
```

#### 5. Spiel-Erkennung
```bash
npm run detect-games
# Erwartung: Steam-Spiele werden erkannt
```

---

## English

### Overview

The system features a comprehensive test framework:
- **Unit Tests**: Test individual components
- **Integration Tests**: Test component interactions
- **Validation**: System checks and configuration
- **End-to-End Tests**: Complete workflows

### Running Tests

```bash
# All unit tests
npm test

# With code coverage
npm run test:coverage

# Watch mode (for development)
npm run test:watch

# Complete test suite
npm run test:all

# System validation
npm run validate
```

### Test Structure

```
tests/
├── game-library.test.js        # Game Library Tests
├── anticheat.test.js           # Anti-Cheat Detection Tests
├── compatibility-manager.test.js # Plugin Manager Tests
├── validator.test.js           # Input Validation Tests
└── integration.test.js         # End-to-End Tests
```

### Code Coverage Goals

- Branches: 70%+
- Functions: 70%+
- Lines: 70%+
- Statements: 70%+

### Manual Tests

#### 1. System Setup Test
```bash
npm run setup
# Expected: Configuration directory is created
```

#### 2. Add Game
```bash
npm run add-game -- --name "Test Game" --path "/path/to/game" --executable "game.exe" --platform windows
# Expected: Game is added to library
```

#### 3. List Games
```bash
npm start list
# Expected: List of all games is displayed
```

#### 4. Anti-Cheat Check
```bash
npm run check-anticheat "Test Game"
# Expected: Anti-cheat analysis is displayed
```

#### 5. Game Detection
```bash
npm run detect-games
# Expected: Steam games are detected
```

## Test-Abdeckung / Test Coverage

### Aktueller Status / Current Status

| Modul | Coverage | Status |
|-------|----------|--------|
| Game Library | 85%+ | ✅ |
| Anti-Cheat | 80%+ | ✅ |
| Compatibility Manager | 75%+ | ✅ |
| Validator | 90%+ | ✅ |
| Integration | 70%+ | ✅ |

## Continuous Integration

Die Tests können in CI/CD-Pipelines integriert werden:

```yaml
# .github/workflows/test.yml
name: Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-node@v2
        with:
          node-version: '16'
      - run: npm install
      - run: npm run test:all
      - run: npm run validate
```

## Fehlersuche / Troubleshooting

### Tests schlagen fehl
1. Überprüfen Sie Node.js-Version (>=16 erforderlich)
2. Führen Sie `npm install` aus
3. Prüfen Sie Abhängigkeiten mit `npm run validate`

### Coverage zu niedrig
1. Führen Sie `npm run test:coverage` aus
2. Überprüfen Sie `coverage/lcov-report/index.html`
3. Fügen Sie fehlende Tests hinzu

### Integration Tests fehl
1. Überprüfen Sie Systemkonfiguration
2. Führen Sie `npm run validate` aus
3. Stellen Sie sicher, dass alle Plugins vorhanden sind

## Best Practices

1. **Immer Tests schreiben** für neue Features
2. **Test-First Development** verwenden
3. **Mocks verwenden** für externe Abhängigkeiten
4. **Tests isoliert** halten
5. **Aussagekräftige Namen** für Tests verwenden
