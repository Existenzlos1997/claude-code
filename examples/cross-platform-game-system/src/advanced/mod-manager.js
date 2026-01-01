/**
 * Mod Management System
 * Phase 5: Advanced Features
 */

const fs = require('fs').promises;
const path = require('path');

class ModManager {
  constructor(gamePath) {
    this.gamePath = gamePath;
    this.modsPath = path.join(gamePath, 'mods');
    this.mods = new Map();
    this.loadOrder = [];
  }

  /**
   * Initialize mod manager
   */
  async initialize() {
    try {
      await fs.mkdir(this.modsPath, { recursive: true });
      await this.scanMods();
    } catch (error) {
      console.error('Failed to initialize mod manager:', error.message);
    }
  }

  /**
   * Scan for installed mods
   */
  async scanMods() {
    try {
      const entries = await fs.readdir(this.modsPath, { withFileTypes: true });
      
      for (const entry of entries) {
        if (entry.isDirectory()) {
          const modPath = path.join(this.modsPath, entry.name);
          const mod = await this.loadModInfo(modPath);
          if (mod) {
            this.mods.set(mod.id, mod);
          }
        }
      }
      
      return Array.from(this.mods.values());
    } catch (error) {
      console.error('Failed to scan mods:', error.message);
      return [];
    }
  }

  /**
   * Load mod information
   */
  async loadModInfo(modPath) {
    try {
      const infoPath = path.join(modPath, 'mod.json');
      const data = await fs.readFile(infoPath, 'utf8');
      const info = JSON.parse(data);
      
      return {
        id: info.id,
        name: info.name,
        version: info.version,
        author: info.author,
        description: info.description,
        dependencies: info.dependencies || [],
        conflicts: info.conflicts || [],
        path: modPath,
        enabled: info.enabled !== false
      };
    } catch (error) {
      return null;
    }
  }

  /**
   * Install mod from archive
   */
  async installMod(archivePath) {
    // Placeholder: would extract and validate mod
    const modId = path.basename(archivePath, path.extname(archivePath));
    
    const mod = {
      id: modId,
      name: modId,
      version: '1.0.0',
      author: 'Unknown',
      description: 'Installed mod',
      dependencies: [],
      conflicts: [],
      path: path.join(this.modsPath, modId),
      enabled: true
    };
    
    this.mods.set(mod.id, mod);
    
    return mod;
  }

  /**
   * Uninstall mod
   */
  async uninstallMod(modId) {
    const mod = this.mods.get(modId);
    if (!mod) {
      throw new Error(`Mod not found: ${modId}`);
    }
    
    // Remove from load order
    this.loadOrder = this.loadOrder.filter(id => id !== modId);
    
    // Remove from mods map
    this.mods.delete(modId);
    
    return true;
  }

  /**
   * Enable mod
   */
  enableMod(modId) {
    const mod = this.mods.get(modId);
    if (!mod) {
      throw new Error(`Mod not found: ${modId}`);
    }
    
    mod.enabled = true;
    
    if (!this.loadOrder.includes(modId)) {
      this.loadOrder.push(modId);
    }
    
    return true;
  }

  /**
   * Disable mod
   */
  disableMod(modId) {
    const mod = this.mods.get(modId);
    if (!mod) {
      throw new Error(`Mod not found: ${modId}`);
    }
    
    mod.enabled = false;
    this.loadOrder = this.loadOrder.filter(id => id !== modId);
    
    return true;
  }

  /**
   * Check mod dependencies
   */
  checkDependencies(modId) {
    const mod = this.mods.get(modId);
    if (!mod) {
      throw new Error(`Mod not found: ${modId}`);
    }
    
    const missing = [];
    const conflicts = [];
    
    // Check dependencies
    for (const dep of mod.dependencies) {
      if (!this.mods.has(dep) || !this.mods.get(dep).enabled) {
        missing.push(dep);
      }
    }
    
    // Check conflicts
    for (const conf of mod.conflicts) {
      if (this.mods.has(conf) && this.mods.get(conf).enabled) {
        conflicts.push(conf);
      }
    }
    
    return {
      satisfied: missing.length === 0 && conflicts.length === 0,
      missing,
      conflicts
    };
  }

  /**
   * Resolve load order
   */
  resolveLoadOrder() {
    const sorted = [];
    const visited = new Set();
    const visiting = new Set();
    
    const visit = (modId) => {
      if (visited.has(modId)) return;
      if (visiting.has(modId)) {
        throw new Error(`Circular dependency detected: ${modId}`);
      }
      
      visiting.add(modId);
      
      const mod = this.mods.get(modId);
      if (mod && mod.enabled) {
        for (const dep of mod.dependencies) {
          visit(dep);
        }
        
        sorted.push(modId);
      }
      
      visiting.delete(modId);
      visited.add(modId);
    };
    
    for (const modId of this.mods.keys()) {
      visit(modId);
    }
    
    this.loadOrder = sorted;
    return sorted;
  }

  /**
   * Get mod list
   */
  listMods(filter = {}) {
    let mods = Array.from(this.mods.values());
    
    if (filter.enabled !== undefined) {
      mods = mods.filter(m => m.enabled === filter.enabled);
    }
    
    if (filter.author) {
      mods = mods.filter(m => m.author === filter.author);
    }
    
    return mods;
  }

  /**
   * Get load order
   */
  getLoadOrder() {
    return this.loadOrder.slice();
  }

  /**
   * Set load order
   */
  setLoadOrder(order) {
    // Validate all mods exist
    for (const modId of order) {
      if (!this.mods.has(modId)) {
        throw new Error(`Invalid mod in load order: ${modId}`);
      }
    }
    
    this.loadOrder = order.slice();
    return true;
  }
}

module.exports = ModManager;
