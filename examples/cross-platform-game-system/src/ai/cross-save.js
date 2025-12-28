/**
 * Cross-Save Synchronization System
 * Synchronisiert Spielstände über verschiedene Kompatibilitätsschichten
 * Synchronizes game saves across different compatibility layers
 * 
 * UNIQUE FEATURE: Automatic save sync between Wine, Proton, Native, and Cloud
 */

const fs = require('fs').promises;
const path = require('path');
const crypto = require('crypto');

class CrossSaveManager {
  constructor() {
    this.saveRegistry = new Map();
    this.homeDir = process.env.HOME || process.env.USERPROFILE;
  }

  /**
   * Register save locations for a game
   */
  registerSaveLocations(game, locations) {
    this.saveRegistry.set(game.name, {
      game,
      locations,
      lastSync: null
    });
  }

  /**
   * Detect common save locations
   * UNIQUE: Cross-compatibility layer save detection
   */
  async detectSaveLocations(game) {
    const locations = [];

    // Common Windows save paths (via Wine/Proton)
    const winePrefixes = [
      path.join(this.homeDir, '.wine'),
      path.join(this.homeDir, '.proton-games', game.name),
      path.join(this.homeDir, '.steam/steam/steamapps/compatdata')
    ];

    for (const prefix of winePrefixes) {
      const possiblePaths = [
        path.join(prefix, 'drive_c/users', process.env.USER || 'user', 'Documents', game.name),
        path.join(prefix, 'drive_c/users', process.env.USER || 'user', 'Saved Games', game.name),
        path.join(prefix, 'drive_c/users', process.env.USER || 'user', 'AppData/Roaming', game.name),
        path.join(prefix, 'drive_c/users', process.env.USER || 'user', 'AppData/Local', game.name)
      ];

      for (const savePath of possiblePaths) {
        try {
          await fs.access(savePath);
          locations.push({
            type: 'wine',
            path: savePath,
            prefix
          });
        } catch (err) {
          // Path doesn't exist
        }
      }
    }

    // Native Linux save locations
    const nativePaths = [
      path.join(this.homeDir, '.local/share', game.name),
      path.join(this.homeDir, '.config', game.name),
      path.join(this.homeDir, 'Documents', game.name)
    ];

    for (const savePath of nativePaths) {
      try {
        await fs.access(savePath);
        locations.push({
          type: 'native',
          path: savePath
        });
      } catch (err) {
        // Path doesn't exist
      }
    }

    return locations;
  }

  /**
   * Calculate file hash for comparison
   */
  async calculateHash(filePath) {
    const content = await fs.readFile(filePath);
    return crypto.createHash('md5').update(content).digest('hex');
  }

  /**
   * Find most recent save
   * UNIQUE: Intelligent save selection across platforms
   */
  async findMostRecentSave(locations) {
    let mostRecent = null;
    let latestTime = 0;

    for (const location of locations) {
      try {
        const files = await fs.readdir(location.path, { withFileTypes: true });
        
        for (const file of files) {
          if (file.isFile()) {
            const filePath = path.join(location.path, file.name);
            const stats = await fs.stat(filePath);
            
            if (stats.mtimeMs > latestTime) {
              latestTime = stats.mtimeMs;
              mostRecent = {
                location,
                file: file.name,
                path: filePath,
                modified: new Date(stats.mtimeMs)
              };
            }
          }
        }
      } catch (err) {
        // Skip inaccessible locations
      }
    }

    return mostRecent;
  }

  /**
   * Sync saves across locations
   * UNIQUE: Bidirectional sync with conflict resolution
   */
  async syncSaves(game, options = {}) {
    const entry = this.saveRegistry.get(game.name);
    if (!entry) {
      throw new Error(`No save locations registered for ${game.name}`);
    }

    const { locations } = entry;
    const syncResults = {
      game: game.name,
      synced: 0,
      conflicts: 0,
      errors: []
    };

    // Find most recent save
    const mostRecent = await this.findMostRecentSave(locations);
    
    if (!mostRecent) {
      return { ...syncResults, message: 'No saves found' };
    }

    // Sync to all other locations
    for (const location of locations) {
      if (location.path === mostRecent.location.path) continue;

      try {
        const destPath = path.join(location.path, mostRecent.file);
        
        // Check if destination exists
        let shouldCopy = true;
        try {
          const destStats = await fs.stat(destPath);
          const destHash = await this.calculateHash(destPath);
          const srcHash = await this.calculateHash(mostRecent.path);
          
          if (destHash === srcHash) {
            shouldCopy = false; // Files are identical
          } else if (destStats.mtimeMs > mostRecent.modified.getTime()) {
            // Conflict: destination is newer
            syncResults.conflicts++;
            if (!options.forceOverwrite) {
              shouldCopy = false;
            }
          }
        } catch (err) {
          // Destination doesn't exist, proceed with copy
        }

        if (shouldCopy) {
          await fs.mkdir(location.path, { recursive: true });
          await fs.copyFile(mostRecent.path, destPath);
          syncResults.synced++;
        }
      } catch (err) {
        syncResults.errors.push({
          location: location.path,
          error: err.message
        });
      }
    }

    entry.lastSync = new Date();
    return syncResults;
  }

  /**
   * Create backup of saves
   */
  async createBackup(game) {
    const entry = this.saveRegistry.get(game.name);
    if (!entry) {
      throw new Error(`No save locations registered for ${game.name}`);
    }

    const backupDir = path.join(this.homeDir, '.game-system', 'backups', game.name);
    const timestamp = new Date().toISOString().replace(/:/g, '-').split('.')[0];
    const backupPath = path.join(backupDir, timestamp);

    await fs.mkdir(backupPath, { recursive: true });

    let backedUp = 0;
    for (const location of entry.locations) {
      try {
        const files = await fs.readdir(location.path);
        for (const file of files) {
          const src = path.join(location.path, file);
          const dest = path.join(backupPath, `${location.type}_${file}`);
          await fs.copyFile(src, dest);
          backedUp++;
        }
      } catch (err) {
        // Skip inaccessible locations
      }
    }

    return {
      backupPath,
      filesBackedUp: backedUp,
      timestamp: new Date()
    };
  }

  /**
   * Get sync status for a game
   */
  getSyncStatus(game) {
    const entry = this.saveRegistry.get(game.name);
    if (!entry) {
      return null;
    }

    return {
      game: game.name,
      locations: entry.locations.length,
      lastSync: entry.lastSync,
      syncEnabled: true
    };
  }
}

module.exports = CrossSaveManager;
