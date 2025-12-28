/**
 * Compatibility Manager
 * Verwaltet verschiedene Kompatibilitätsschichten (Wine, Proton, etc.)
 */

const fs = require('fs').promises;
const path = require('path');
const { execa } = require('execa');

class CompatibilityManager {
  constructor() {
    this.plugins = new Map();
  }

  async loadPlugins() {
    // Load built-in compatibility layers
    const pluginsDir = path.join(__dirname, '../../plugins');
    
    try {
      const entries = await fs.readdir(pluginsDir, { withFileTypes: true });
      
      for (const entry of entries) {
        if (entry.isDirectory()) {
          try {
            const pluginPath = path.join(pluginsDir, entry.name, 'index.js');
            const plugin = require(pluginPath);
            this.plugins.set(plugin.name, plugin);
          } catch (err) {
            console.warn(`Failed to load plugin ${entry.name}:`, err.message);
          }
        }
      }
    } catch (err) {
      console.warn('No plugins directory found');
    }
  }

  getPlugin(name) {
    return this.plugins.get(name);
  }

  async launch(game) {
    const pluginName = game.compatibilityLayer || 'native';
    const plugin = this.plugins.get(pluginName);

    if (!plugin) {
      throw new Error(`Compatibility layer "${pluginName}" not found`);
    }

    if (plugin.canRun && !(await plugin.canRun(game))) {
      throw new Error(`Plugin "${pluginName}" cannot run this game`);
    }

    return await plugin.launch(game);
  }
}

module.exports = CompatibilityManager;
