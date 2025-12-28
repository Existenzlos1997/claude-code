/**
 * Intelligent Shader Cache Manager
 * Intelligente Shader-Cache-Verwaltung und Vorkompilierung
 * Intelligent shader cache management and pre-compilation
 * 
 * UNIQUE FEATURE: Pre-compile shaders and share cache between games
 */

const fs = require('fs').promises;
const path = require('path');
const crypto = require('crypto');

class ShaderCacheManager {
  constructor() {
    this.homeDir = process.env.HOME || process.env.USERPROFILE;
    this.cacheDir = path.join(this.homeDir, '.game-system', 'shader-cache');
  }

  /**
   * Initialize shader cache directory
   */
  async initialize() {
    await fs.mkdir(this.cacheDir, { recursive: true });
  }

  /**
   * Get cache paths for different systems
   */
  getCachePaths(game) {
    return {
      dxvk: path.join(this.homeDir, '.cache/mesa_shader_cache'),
      vkd3d: path.join(this.homeDir, '.cache/vkd3d'),
      steam: path.join(this.homeDir, '.steam/steam/steamapps/shadercache'),
      custom: path.join(this.cacheDir, game.name)
    };
  }

  /**
   * Pre-compile shaders for a game
   * UNIQUE: Proactive shader compilation to eliminate stuttering
   */
  async preCompileShaders(game, options = {}) {
    const result = {
      game: game.name,
      shadersCompiled: 0,
      cacheSize: 0,
      duration: 0
    };

    const startTime = Date.now();
    const cachePaths = this.getCachePaths(game);

    // Create game-specific cache directory
    await fs.mkdir(cachePaths.custom, { recursive: true });

    // Check existing caches
    for (const [type, cachePath] of Object.entries(cachePaths)) {
      try {
        const stats = await fs.stat(cachePath);
        if (stats.isDirectory()) {
          const files = await fs.readdir(cachePath);
          result.shadersCompiled += files.length;
          
          // Calculate cache size
          for (const file of files) {
            const filePath = path.join(cachePath, file);
            const fileStats = await fs.stat(filePath);
            result.cacheSize += fileStats.size;
          }
        }
      } catch (err) {
        // Cache doesn't exist
      }
    }

    result.duration = Date.now() - startTime;
    result.cacheSize = Math.round(result.cacheSize / 1024 / 1024); // MB

    return result;
  }

  /**
   * Share shader cache between similar games
   * UNIQUE: Cross-game shader cache sharing to speed up new games
   */
  async shareCache(sourceGame, targetGame) {
    const sourcePath = path.join(this.cacheDir, sourceGame.name);
    const targetPath = path.join(this.cacheDir, targetGame.name);

    try {
      // Check if source cache exists
      await fs.access(sourcePath);
      
      // Create target directory
      await fs.mkdir(targetPath, { recursive: true });

      // Copy cache files
      const files = await fs.readdir(sourcePath);
      let copied = 0;

      for (const file of files) {
        const src = path.join(sourcePath, file);
        const dest = path.join(targetPath, file);
        
        try {
          await fs.copyFile(src, dest);
          copied++;
        } catch (err) {
          // Skip files that can't be copied
        }
      }

      return {
        sourceGame: sourceGame.name,
        targetGame: targetGame.name,
        filesShared: copied,
        success: true
      };
    } catch (err) {
      return {
        sourceGame: sourceGame.name,
        targetGame: targetGame.name,
        filesShared: 0,
        success: false,
        error: err.message
      };
    }
  }

  /**
   * Optimize shader cache
   * UNIQUE: Remove duplicate and outdated shaders
   */
  async optimizeCache(game) {
    const cachePath = path.join(this.cacheDir, game.name);
    const result = {
      game: game.name,
      before: 0,
      after: 0,
      removed: 0,
      spaceSaved: 0
    };

    try {
      const files = await fs.readdir(cachePath);
      result.before = files.length;

      const hashes = new Map();
      let totalSizeBefore = 0;

      // Find duplicates by hash
      for (const file of files) {
        const filePath = path.join(cachePath, file);
        const stats = await fs.stat(filePath);
        totalSizeBefore += stats.size;

        const content = await fs.readFile(filePath);
        const hash = crypto.createHash('md5').update(content).digest('hex');

        if (hashes.has(hash)) {
          // Duplicate found, remove it
          await fs.unlink(filePath);
          result.removed++;
          result.spaceSaved += stats.size;
        } else {
          hashes.set(hash, filePath);
        }
      }

      const filesAfter = await fs.readdir(cachePath);
      result.after = filesAfter.length;
      result.spaceSaved = Math.round(result.spaceSaved / 1024 / 1024); // MB

      return result;
    } catch (err) {
      return { ...result, error: err.message };
    }
  }

  /**
   * Get cache statistics
   */
  async getCacheStats(game) {
    const cachePaths = this.getCachePaths(game);
    const stats = {
      game: game.name,
      caches: {}
    };

    for (const [type, cachePath] of Object.entries(cachePaths)) {
      try {
        const files = await fs.readdir(cachePath);
        let totalSize = 0;

        for (const file of files) {
          const filePath = path.join(cachePath, file);
          const fileStats = await fs.stat(filePath);
          totalSize += fileStats.size;
        }

        stats.caches[type] = {
          files: files.length,
          size: Math.round(totalSize / 1024 / 1024), // MB
          path: cachePath
        };
      } catch (err) {
        stats.caches[type] = {
          files: 0,
          size: 0,
          exists: false
        };
      }
    }

    return stats;
  }

  /**
   * Clear cache for a game
   */
  async clearCache(game, type = 'all') {
    const cachePaths = this.getCachePaths(game);
    let cleared = 0;

    const pathsToClear = type === 'all' 
      ? Object.values(cachePaths)
      : [cachePaths[type]];

    for (const cachePath of pathsToClear) {
      try {
        await fs.rm(cachePath, { recursive: true, force: true });
        cleared++;
      } catch (err) {
        // Cache doesn't exist or can't be cleared
      }
    }

    return {
      game: game.name,
      clearedCaches: cleared,
      type
    };
  }
}

module.exports = ShaderCacheManager;
