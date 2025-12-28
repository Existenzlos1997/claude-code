/**
 * Game Library Management
 * Verwaltet die Bibliothek der installierten Spiele
 */

const fs = require('fs').promises;
const path = require('path');

class GameLibrary {
  constructor() {
    this.games = [];
    this.libraryPath = path.join(process.env.HOME || process.env.USERPROFILE, '.game-system', 'library.json');
  }

  async load() {
    try {
      const data = await fs.readFile(this.libraryPath, 'utf8');
      this.games = JSON.parse(data);
    } catch (err) {
      // Library doesn't exist yet, start with empty array
      this.games = [];
    }
  }

  async save() {
    const dir = path.dirname(this.libraryPath);
    await fs.mkdir(dir, { recursive: true });
    await fs.writeFile(this.libraryPath, JSON.stringify(this.games, null, 2));
  }

  addGame(game) {
    this.games.push({
      id: Date.now().toString(),
      name: game.name,
      path: game.path,
      platform: game.platform,
      compatibilityLayer: game.compatibilityLayer || 'native',
      executable: game.executable,
      addedAt: new Date().toISOString()
    });
  }

  findGame(nameOrId) {
    return this.games.find(g => 
      g.name.toLowerCase() === nameOrId.toLowerCase() || 
      g.id === nameOrId
    );
  }

  removeGame(nameOrId) {
    const index = this.games.findIndex(g => 
      g.name.toLowerCase() === nameOrId.toLowerCase() || 
      g.id === nameOrId
    );
    if (index !== -1) {
      this.games.splice(index, 1);
      return true;
    }
    return false;
  }
}

module.exports = GameLibrary;
