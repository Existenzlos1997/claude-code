/**
 * Configuration Management
 * Zentrale Konfigurationsverwaltung mit Hot-Reload, Validation und Templates
 */

const Conf = require('conf');
const fs = require('fs').promises;
const path = require('path');

class ConfigManager {
  constructor() {
    this.config = new Conf({
      projectName: 'cross-platform-game-system',
      defaults: this._getDefaults()
    });
    
    this.configPath = this.config.path;
    this.backupDir = path.join(path.dirname(this.configPath), 'backups');
    this.history = [];
  }

  _getDefaults() {
    return {
      defaultCompatibilityLayer: 'proton',
      winePrefixPath: '~/.wine-games',
      autoDetectGames: true,
      platforms: ['windows', 'linux', 'native'],
      wine: {
        version: 'wine-stable',
        prefix: '~/.wine-games',
        dxvk: true,
        vkd3d: true,
        esync: true,
        fsync: true
      },
      proton: {
        version: 'latest',
        experimentalFeatures: true,
        eac: true,
        battleye: true
      },
      detection: {
        scanPaths: [
          '~/.steam/steam/steamapps/common',
          '~/.local/share/lutris/games',
          '/opt/games',
          'C:\\Program Files',
          'C:\\Program Files (x86)'
        ]
      },
      performance: {
        targetFPS: 60,
        enableMonitoring: true,
        logMetrics: true
      },
      ai: {
        autoOptimize: true,
        predictCompatibility: true,
        shaderPreCompile: true,
        crossSaveSync: true
      }
    };
  }

  // Basic Configuration
  get(key, defaultValue = undefined) {
    // Support environment variable overrides
    const envKey = `GAME_SYSTEM_${key.toUpperCase().replace(/\./g, '_')}`;
    if (process.env[envKey]) {
      try {
        return JSON.parse(process.env[envKey]);
      } catch {
        return process.env[envKey];
      }
    }
    return this.config.get(key, defaultValue);
  }

  set(key, value) {
    const oldValue = this.config.get(key);
    this.config.set(key, value);
    
    // Track change in history
    this.history.push({
      timestamp: new Date().toISOString(),
      key,
      oldValue,
      newValue: value
    });
    
    // Keep last 50 changes
    if (this.history.length > 50) {
      this.history = this.history.slice(-50);
    }
  }

  has(key) {
    return this.config.has(key);
  }

  delete(key) {
    this.config.delete(key);
  }

  clear() {
    this.config.clear();
  }

  getAll() {
    return this.config.store;
  }

  // Hot Reload
  async reload() {
    // Re-initialize config from disk
    const newConfig = new Conf({
      projectName: 'cross-platform-game-system',
      defaults: this._getDefaults()
    });
    this.config = newConfig;
    return true;
  }

  // Schema Validation
  validate(schema = null) {
    if (!schema) {
      schema = this._getDefaultSchema();
    }

    const errors = [];
    const config = this.getAll();

    // Simple validation
    for (const [key, rules] of Object.entries(schema)) {
      const value = this.get(key);
      
      if (rules.required && value === undefined) {
        errors.push(`Missing required field: ${key}`);
      }
      
      if (value !== undefined && rules.type) {
        const actualType = Array.isArray(value) ? 'array' : typeof value;
        if (actualType !== rules.type) {
          errors.push(`Invalid type for ${key}: expected ${rules.type}, got ${actualType}`);
        }
      }
    }

    return {
      valid: errors.length === 0,
      errors
    };
  }

  _getDefaultSchema() {
    return {
      'defaultCompatibilityLayer': { type: 'string', required: true },
      'autoDetectGames': { type: 'boolean', required: true },
      'platforms': { type: 'array', required: true },
      'wine.version': { type: 'string' },
      'proton.version': { type: 'string' }
    };
  }

  // Configuration Templates
  applyTemplate(templateName) {
    const templates = {
      gaming: {
        defaultCompatibilityLayer: 'proton',
        wine: {
          dxvk: true,
          vkd3d: true,
          esync: true,
          fsync: true
        },
        performance: {
          targetFPS: 60,
          enableMonitoring: true
        },
        ai: {
          autoOptimize: true,
          shaderPreCompile: true
        }
      },
      performance: {
        defaultCompatibilityLayer: 'native',
        wine: {
          dxvk: true,
          vkd3d: true,
          esync: true,
          fsync: true
        },
        performance: {
          targetFPS: 144,
          enableMonitoring: true,
          logMetrics: false
        },
        ai: {
          autoOptimize: true,
          shaderPreCompile: true,
          crossSaveSync: false
        }
      },
      compatibility: {
        defaultCompatibilityLayer: 'wine',
        wine: {
          dxvk: false,
          vkd3d: false,
          esync: false,
          fsync: false
        },
        performance: {
          targetFPS: 30,
          enableMonitoring: false
        },
        ai: {
          autoOptimize: false,
          shaderPreCompile: false
        }
      }
    };

    const template = templates[templateName];
    if (!template) {
      throw new Error(`Template not found: ${templateName}. Available: ${Object.keys(templates).join(', ')}`);
    }

    this.merge(template);
    return true;
  }

  getTemplates() {
    return ['gaming', 'performance', 'compatibility'];
  }

  // Configuration Merging
  merge(overrides) {
    const current = this.getAll();
    const merged = this._deepMerge(current, overrides);
    
    // Apply merged config
    for (const [key, value] of Object.entries(merged)) {
      this.set(key, value);
    }
    
    return merged;
  }

  _deepMerge(target, source) {
    const result = { ...target };
    
    for (const [key, value] of Object.entries(source)) {
      if (value && typeof value === 'object' && !Array.isArray(value)) {
        result[key] = this._deepMerge(result[key] || {}, value);
      } else {
        result[key] = value;
      }
    }
    
    return result;
  }

  // Backup & Restore
  async backup() {
    await fs.mkdir(this.backupDir, { recursive: true });
    
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    const backupPath = path.join(this.backupDir, `config-${timestamp}.json`);
    
    const data = JSON.stringify(this.getAll(), null, 2);
    await fs.writeFile(backupPath, data);
    
    return backupPath;
  }

  async restore(timestamp) {
    const backupPath = path.join(this.backupDir, `config-${timestamp}.json`);
    
    try {
      const data = await fs.readFile(backupPath, 'utf8');
      const config = JSON.parse(data);
      
      // Clear current config
      this.clear();
      
      // Restore
      for (const [key, value] of Object.entries(config)) {
        this.set(key, value);
      }
      
      return true;
    } catch (err) {
      throw new Error(`Failed to restore backup: ${err.message}`);
    }
  }

  async listBackups() {
    try {
      const files = await fs.readdir(this.backupDir);
      return files
        .filter(f => f.startsWith('config-') && f.endsWith('.json'))
        .map(f => f.replace('config-', '').replace('.json', ''))
        .sort()
        .reverse();
    } catch {
      return [];
    }
  }

  // Configuration History
  getHistory(limit = 10) {
    return this.history.slice(-limit).reverse();
  }

  clearHistory() {
    this.history = [];
  }

  // Export/Import
  export() {
    return {
      version: '1.2.0',
      exportedAt: new Date().toISOString(),
      config: this.getAll()
    };
  }

  async import(data) {
    if (!data.config) {
      throw new Error('Invalid import data');
    }

    // Validate before import
    const tempConfig = new ConfigManager();
    for (const [key, value] of Object.entries(data.config)) {
      tempConfig.set(key, value);
    }
    
    const validation = tempConfig.validate();
    if (!validation.valid) {
      throw new Error(`Invalid configuration: ${validation.errors.join(', ')}`);
    }

    // Apply
    for (const [key, value] of Object.entries(data.config)) {
      this.set(key, value);
    }

    return true;
  }
}

// Export singleton instance
const configManager = new ConfigManager();
module.exports = configManager;
