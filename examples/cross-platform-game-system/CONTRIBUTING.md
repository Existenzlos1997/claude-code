# Mitwirken / Contributing

Vielen Dank für Ihr Interesse an diesem Projekt!
Thank you for your interest in contributing to this project!

## Entwicklungsumgebung einrichten / Setting Up Development Environment

```bash
# Repository klonen / Clone repository
git clone <repository-url>
cd cross-platform-game-system

# Dependencies installieren / Install dependencies
npm install

# System testen / Test system
npm test
```

## Beitrag leisten / Making Contributions

1. Fork das Repository / Fork the repository
2. Erstellen Sie einen Feature-Branch / Create a feature branch
   ```bash
   git checkout -b feature/my-new-feature
   ```
3. Machen Sie Ihre Änderungen / Make your changes
4. Testen Sie Ihre Änderungen / Test your changes
   ```bash
   npm test
   ```
5. Commit Ihre Änderungen / Commit your changes
   ```bash
   git commit -m "Add some feature"
   ```
6. Push zum Branch / Push to the branch
   ```bash
   git push origin feature/my-new-feature
   ```
7. Erstellen Sie einen Pull Request / Create a Pull Request

## Code-Stil / Code Style

- Verwenden Sie klare, beschreibende Variablennamen
  Use clear, descriptive variable names
- Fügen Sie Kommentare für komplexe Logik hinzu (Deutsch oder Englisch)
  Add comments for complex logic (German or English)
- Folgen Sie dem bestehenden Code-Stil
  Follow existing code style

## Plugin-Entwicklung / Plugin Development

Wenn Sie ein neues Kompatibilitäts-Plugin erstellen möchten:
If you want to create a new compatibility plugin:

```javascript
// plugins/my-plugin/index.js
module.exports = {
  name: 'my-plugin',
  platform: 'target-platform',
  description: 'Description of the plugin',
  
  async canRun(game) {
    // Check if plugin can run this game
    return true;
  },
  
  async launch(game, options) {
    // Launch the game
  }
};
```

## Probleme melden / Reporting Issues

- Überprüfen Sie, ob das Problem bereits gemeldet wurde
  Check if the issue already exists
- Erstellen Sie ein neues Issue mit klarer Beschreibung
  Create a new issue with clear description
- Fügen Sie Schritte zur Reproduktion hinzu (für Bugs)
  Include steps to reproduce (for bugs)
- Fügen Sie relevante Systeminfo hinzu (OS, Node-Version, etc.)
  Add relevant system info (OS, Node version, etc.)

## Fragen? / Questions?

Öffnen Sie ein Issue für Fragen!
Feel free to open an issue for questions!
