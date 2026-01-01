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
    this.pluginMetrics = new Map();
    this.pluginConfigs = new Map();
  }

  async loadPlugins() {
    // Load built-in compatibility layers
    // Support both development and installed paths
    const pluginsDir = process.env.GAME_SYSTEM_PLUGINS || path.join(__dirname, '../../plugins');
    
    try {
      const entries = await fs.readdir(pluginsDir, { withFileTypes: true });
      
      for (const entry of entries) {
        if (entry.isDirectory()) {
          try {
            await this._loadPlugin(pluginsDir, entry.name);
          } catch (err) {
            console.warn(`Failed to load plugin ${entry.name}:`, err.message);
          }
        }
      }
    } catch (err) {
      console.warn('No plugins directory found');
    }
  }

  async _loadPlugin(pluginsDir, pluginName) {
    const pluginPath = path.join(pluginsDir, pluginName, 'index.js');
    const plugin = require(pluginPath);
    
    // Validate plugin structure
    if (!plugin.name || !plugin.launch) {
      throw new Error(`Invalid plugin structure: ${pluginName}`);
    }

    // Initialize plugin
    if (plugin.init) {
      await plugin.init();
    }

    this.plugins.set(plugin.name, plugin);
    this.pluginMetrics.set(plugin.name, {
      loadedAt: new Date().toISOString(),
      launches: 0,
      failures: 0,
      lastUsed: null
    });
  }

  getPlugin(name) {
    return this.plugins.get(name);
  }

  getAllPlugins() {
    return Array.from(this.plugins.values());
  }

  getPluginNames() {
    return Array.from(this.plugins.keys());
  }

  // Plugin Lifecycle Management
  async reloadPlugin(pluginName) {
    const plugin = this.plugins.get(pluginName);
    if (!plugin) {
      throw new Error(`Plugin not found: ${pluginName}`);
    }

    // Cleanup old plugin
    await this.cleanupPlugin(pluginName);

    // Remove from require cache
    const pluginsDir = process.env.GAME_SYSTEM_PLUGINS || path.join(__dirname, '../../plugins');
    const pluginPath = path.join(pluginsDir, pluginName, 'index.js');
    delete require.cache[require.resolve(pluginPath)];

    // Reload
    await this._loadPlugin(pluginsDir, pluginName);
    return true;
  }

  async cleanupPlugin(pluginName) {
    const plugin = this.plugins.get(pluginName);
    if (plugin && plugin.cleanup) {
      await plugin.cleanup();
    }
  }

  // Plugin Configuration
  configurePlugin(pluginName, config) {
    const plugin = this.plugins.get(pluginName);
    if (!plugin) {
      throw new Error(`Plugin not found: ${pluginName}`);
    }

    this.pluginConfigs.set(pluginName, config);

    if (plugin.configure) {
      plugin.configure(config);
    }

    return true;
  }

  getPluginConfig(pluginName) {
    return this.pluginConfigs.get(pluginName) || {};
  }

  // Plugin Health & Validation
  async getPluginHealth(pluginName) {
    const plugin = this.plugins.get(pluginName);
    if (!plugin) {
      return { status: 'not_found', healthy: false };
    }

    const health = {
      name: pluginName,
      status: 'loaded',
      healthy: true,
      version: plugin.version || 'unknown',
      metrics: this.pluginMetrics.get(pluginName)
    };

    // Check if plugin has health check
    if (plugin.healthCheck) {
      try {
        const checkResult = await plugin.healthCheck();
        health.healthy = checkResult.healthy;
        health.details = checkResult.details;
        if (!checkResult.healthy) {
          health.status = 'unhealthy';
        }
      } catch (err) {
        health.healthy = false;
        health.status = 'error';
        health.error = err.message;
      }
    }

    return health;
  }

  async validatePluginDependencies(pluginName) {
    const plugin = this.plugins.get(pluginName);
    if (!plugin) {
      throw new Error(`Plugin not found: ${pluginName}`);
    }

    if (!plugin.dependencies || plugin.dependencies.length === 0) {
      return { valid: true, missing: [] };
    }

    const missing = [];
    for (const dep of plugin.dependencies) {
      if (!this.plugins.has(dep)) {
        missing.push(dep);
      }
    }

    return {
      valid: missing.length === 0,
      missing,
      required: plugin.dependencies
    };
  }

  // Plugin Metrics
  getPluginMetrics(pluginName = null) {
    if (pluginName) {
      return this.pluginMetrics.get(pluginName);
    }
    
    const metrics = {};
    this.pluginMetrics.forEach((value, key) => {
      metrics[key] = value;
    });
    return metrics;
  }

  _updateMetrics(pluginName, success) {
    const metrics = this.pluginMetrics.get(pluginName);
    if (metrics) {
      metrics.launches++;
      if (!success) {
        metrics.failures++;
      }
      metrics.lastUsed = new Date().toISOString();
      metrics.successRate = ((metrics.launches - metrics.failures) / metrics.launches * 100).toFixed(2);
    }
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

    try {
      const result = await plugin.launch(game);
      this._updateMetrics(pluginName, true);
      return result;
    } catch (err) {
      this._updateMetrics(pluginName, false);
      
      // Auto-recovery: Try fallback if available
      if (plugin.fallback) {
        console.warn(`Plugin ${pluginName} failed, trying fallback...`);
        const fallbackPlugin = this.plugins.get(plugin.fallback);
        if (fallbackPlugin) {
          const result = await fallbackPlugin.launch(game);
          this._updateMetrics(plugin.fallback, true);
          return result;
        }
      }
      
      throw err;
    }
  }

  // System Health
  async getSystemHealth() {
    const health = {
      pluginCount: this.plugins.size,
      plugins: {},
      overall: 'healthy'
    };

    for (const pluginName of this.plugins.keys()) {
      health.plugins[pluginName] = await this.getPluginHealth(pluginName);
      if (!health.plugins[pluginName].healthy) {
        health.overall = 'degraded';
      }
    }

    return health;
  }
}

module.exports = CompatibilityManager;
