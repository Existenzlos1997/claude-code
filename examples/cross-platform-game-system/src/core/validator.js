/**
 * Input Validation Module
 * Validates user input and game data
 */

class Validator {
  /**
   * Validate game object
   */
  static validateGame(game) {
    const errors = [];

    if (!game) {
      return ['Game object is required'];
    }

    if (!game.name || typeof game.name !== 'string' || game.name.trim() === '') {
      errors.push('Game name is required and must be a non-empty string');
    }

    if (!game.path || typeof game.path !== 'string') {
      errors.push('Game path is required and must be a string');
    }

    if (!game.executable || typeof game.executable !== 'string') {
      errors.push('Game executable is required and must be a string');
    }

    const validPlatforms = ['windows', 'linux', 'native', 'macos'];
    if (!game.platform || !validPlatforms.includes(game.platform)) {
      errors.push(`Game platform must be one of: ${validPlatforms.join(', ')}`);
    }

    return errors;
  }

  /**
   * Validate compatibility layer
   */
  static validateCompatibilityLayer(layer) {
    const validLayers = ['wine', 'proton', 'proton-ge', 'native'];
    
    if (!layer || typeof layer !== 'string') {
      return ['Compatibility layer must be a string'];
    }

    if (!validLayers.includes(layer)) {
      return [`Compatibility layer must be one of: ${validLayers.join(', ')}`];
    }

    return [];
  }

  /**
   * Validate game path exists
   */
  static async validateGamePath(gamePath) {
    const fs = require('fs').promises;
    const errors = [];

    try {
      const stats = await fs.stat(gamePath);
      if (!stats.isDirectory()) {
        errors.push('Game path must be a directory');
      }
    } catch (err) {
      errors.push(`Game path does not exist: ${gamePath}`);
    }

    return errors;
  }

  /**
   * Validate executable exists in game path
   */
  static async validateExecutable(gamePath, executable) {
    const fs = require('fs').promises;
    const path = require('path');
    const errors = [];

    try {
      const execPath = path.join(gamePath, executable);
      const stats = await fs.stat(execPath);
      if (!stats.isFile()) {
        errors.push('Executable must be a file');
      }
    } catch (err) {
      errors.push(`Executable not found: ${executable}`);
    }

    return errors;
  }

  /**
   * Sanitize game name for file system use
   */
  static sanitizeGameName(name) {
    return name
      .replace(/[<>:"/\\|?*]/g, '_')
      .replace(/\s+/g, '_')
      .toLowerCase();
  }

  /**
   * Validate environment variables
   */
  static validateEnvironment() {
    const warnings = [];
    const errors = [];

    // Check for home directory
    if (!process.env.HOME && !process.env.USERPROFILE) {
      errors.push('Cannot determine home directory (HOME or USERPROFILE not set)');
    }

    // Check platform
    const validPlatforms = ['linux', 'darwin', 'win32'];
    if (!validPlatforms.includes(process.platform)) {
      warnings.push(`Platform ${process.platform} may not be fully supported`);
    }

    return { errors, warnings };
  }
}

module.exports = Validator;
