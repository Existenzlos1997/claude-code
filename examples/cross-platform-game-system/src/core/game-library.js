/**
 * Game Library Management
 * Verwaltet die Bibliothek der installierten Spiele
 */

const fs = require('fs').promises;
const path = require('path');
const Validator = require('./validator');

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

  async addGame(game) {
    // Validate game data
    const errors = Validator.validateGame(game);
    if (errors.length > 0) {
      throw new Error(`Invalid game data: ${errors.join(', ')}`);
    }

    this.games.push({
      id: Date.now().toString(),
      name: game.name,
      path: game.path,
      platform: game.platform,
      compatibilityLayer: game.compatibilityLayer || 'native',
      executable: game.executable,
      addedAt: new Date().toISOString(),
      tags: game.tags || [],
      category: game.category || 'Uncategorized',
      favorite: false,
      playtime: 0, // in minutes
      lastPlayed: null,
      antiCheat: game.antiCheat || null
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

  // Advanced Querying
  filterByPlatform(platform) {
    return this.games.filter(g => g.platform === platform);
  }

  filterByCompatibilityLayer(layer) {
    return this.games.filter(g => g.compatibilityLayer === layer);
  }

  filterByAntiCheat(antiCheat) {
    return this.games.filter(g => g.antiCheat === antiCheat);
  }

  filterByTags(tags) {
    return this.games.filter(g => 
      tags.some(tag => g.tags && g.tags.includes(tag))
    );
  }

  filterByCategory(category) {
    return this.games.filter(g => g.category === category);
  }

  getFavorites() {
    return this.games.filter(g => g.favorite);
  }

  // Search
  search(query) {
    const lowerQuery = query.toLowerCase();
    return this.games.filter(g => 
      g.name.toLowerCase().includes(lowerQuery) ||
      g.platform.toLowerCase().includes(lowerQuery) ||
      (g.tags && g.tags.some(tag => tag.toLowerCase().includes(lowerQuery))) ||
      (g.category && g.category.toLowerCase().includes(lowerQuery))
    );
  }

  // Tag Management
  addTags(gameId, tags) {
    const game = this.findGame(gameId);
    if (!game) {
      throw new Error(`Game not found: ${gameId}`);
    }
    if (!game.tags) {
      game.tags = [];
    }
    tags.forEach(tag => {
      if (!game.tags.includes(tag)) {
        game.tags.push(tag);
      }
    });
    return game;
  }

  removeTags(gameId, tags) {
    const game = this.findGame(gameId);
    if (!game) {
      throw new Error(`Game not found: ${gameId}`);
    }
    if (game.tags) {
      game.tags = game.tags.filter(tag => !tags.includes(tag));
    }
    return game;
  }

  // Category Management
  setCategory(gameId, category) {
    const game = this.findGame(gameId);
    if (!game) {
      throw new Error(`Game not found: ${gameId}`);
    }
    game.category = category;
    return game;
  }

  getAllCategories() {
    const categories = new Set();
    this.games.forEach(g => {
      if (g.category) {
        categories.add(g.category);
      }
    });
    return Array.from(categories).sort();
  }

  // Favorites
  toggleFavorite(gameId) {
    const game = this.findGame(gameId);
    if (!game) {
      throw new Error(`Game not found: ${gameId}`);
    }
    game.favorite = !game.favorite;
    return game;
  }

  setFavorite(gameId, favorite) {
    const game = this.findGame(gameId);
    if (!game) {
      throw new Error(`Game not found: ${gameId}`);
    }
    game.favorite = favorite;
    return game;
  }

  // Playtime Tracking
  updatePlaytime(gameId, minutes) {
    const game = this.findGame(gameId);
    if (!game) {
      throw new Error(`Game not found: ${gameId}`);
    }
    game.playtime = (game.playtime || 0) + minutes;
    game.lastPlayed = new Date().toISOString();
    return game;
  }

  getPlaytimeStats() {
    const total = this.games.reduce((sum, g) => sum + (g.playtime || 0), 0);
    const byGame = this.games
      .filter(g => g.playtime > 0)
      .map(g => ({ name: g.name, playtime: g.playtime }))
      .sort((a, b) => b.playtime - a.playtime);
    return {
      totalMinutes: total,
      totalHours: Math.floor(total / 60),
      byGame
    };
  }

  // Statistics
  getStatistics() {
    const stats = {
      totalGames: this.games.length,
      byPlatform: {},
      byCompatibilityLayer: {},
      byAntiCheat: {},
      byCategory: {},
      favorites: this.games.filter(g => g.favorite).length,
      totalPlaytime: this.games.reduce((sum, g) => sum + (g.playtime || 0), 0)
    };

    this.games.forEach(g => {
      stats.byPlatform[g.platform] = (stats.byPlatform[g.platform] || 0) + 1;
      stats.byCompatibilityLayer[g.compatibilityLayer] = (stats.byCompatibilityLayer[g.compatibilityLayer] || 0) + 1;
      if (g.antiCheat) {
        stats.byAntiCheat[g.antiCheat] = (stats.byAntiCheat[g.antiCheat] || 0) + 1;
      }
      if (g.category) {
        stats.byCategory[g.category] = (stats.byCategory[g.category] || 0) + 1;
      }
    });

    return stats;
  }

  // Export/Import
  exportLibrary() {
    return {
      version: '1.2.0',
      exportedAt: new Date().toISOString(),
      games: this.games
    };
  }

  async importLibrary(data, merge = false) {
    if (!data.games || !Array.isArray(data.games)) {
      throw new Error('Invalid library data');
    }

    if (merge) {
      // Merge imported games with existing
      const existingIds = new Set(this.games.map(g => g.id));
      data.games.forEach(game => {
        if (!existingIds.has(game.id)) {
          this.games.push(game);
        }
      });
    } else {
      // Replace existing library
      this.games = data.games;
    }

    await this.save();
    return { imported: data.games.length, total: this.games.length };
  }

  // Utility
  getAllGames() {
    return [...this.games];
  }

  getGameCount() {
    return this.games.length;
  }
}

module.exports = GameLibrary;
