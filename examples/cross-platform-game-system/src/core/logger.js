/**
 * Logger Module
 * Zentrales Logging-System
 * Central logging system
 */

const fs = require('fs').promises;
const path = require('path');
const chalk = require('chalk');

class Logger {
  constructor(options = {}) {
    this.logLevel = options.logLevel || 'info';
    this.logFile = options.logFile || null;
    this.enableConsole = options.enableConsole !== false;
    this.enableFile = options.enableFile || false;

    this.levels = {
      error: 0,
      warn: 1,
      info: 2,
      debug: 3,
      trace: 4
    };

    if (this.enableFile && !this.logFile) {
      const homeDir = process.env.HOME || process.env.USERPROFILE;
      const logDir = path.join(homeDir, '.game-system', 'logs');
      this.logFile = path.join(logDir, `game-system-${Date.now()}.log`);
    }
  }

  /**
   * Initialize logger (create log directory)
   */
  async init() {
    if (this.enableFile && this.logFile) {
      const logDir = path.dirname(this.logFile);
      await fs.mkdir(logDir, { recursive: true });
    }
  }

  /**
   * Check if level should be logged
   */
  shouldLog(level) {
    return this.levels[level] <= this.levels[this.logLevel];
  }

  /**
   * Format log message
   */
  formatMessage(level, message, meta = {}) {
    const timestamp = new Date().toISOString();
    const metaStr = Object.keys(meta).length > 0 ? ` ${JSON.stringify(meta)}` : '';
    return `[${timestamp}] [${level.toUpperCase()}] ${message}${metaStr}`;
  }

  /**
   * Write to log file
   */
  async writeToFile(formattedMessage) {
    if (this.enableFile && this.logFile) {
      try {
        await fs.appendFile(this.logFile, formattedMessage + '\n');
      } catch (err) {
        // Silently fail
      }
    }
  }

  /**
   * Log error
   */
  async error(message, meta = {}) {
    if (!this.shouldLog('error')) return;

    const formatted = this.formatMessage('error', message, meta);
    
    if (this.enableConsole) {
      console.error(chalk.red('✗'), chalk.red(message));
      if (Object.keys(meta).length > 0) {
        console.error(chalk.gray(JSON.stringify(meta, null, 2)));
      }
    }

    await this.writeToFile(formatted);
  }

  /**
   * Log warning
   */
  async warn(message, meta = {}) {
    if (!this.shouldLog('warn')) return;

    const formatted = this.formatMessage('warn', message, meta);
    
    if (this.enableConsole) {
      console.warn(chalk.yellow('⚠'), chalk.yellow(message));
      if (Object.keys(meta).length > 0) {
        console.warn(chalk.gray(JSON.stringify(meta, null, 2)));
      }
    }

    await this.writeToFile(formatted);
  }

  /**
   * Log info
   */
  async info(message, meta = {}) {
    if (!this.shouldLog('info')) return;

    const formatted = this.formatMessage('info', message, meta);
    
    if (this.enableConsole) {
      console.log(chalk.blue('ℹ'), chalk.white(message));
      if (Object.keys(meta).length > 0) {
        console.log(chalk.gray(JSON.stringify(meta, null, 2)));
      }
    }

    await this.writeToFile(formatted);
  }

  /**
   * Log debug
   */
  async debug(message, meta = {}) {
    if (!this.shouldLog('debug')) return;

    const formatted = this.formatMessage('debug', message, meta);
    
    if (this.enableConsole) {
      console.log(chalk.gray('🐛'), chalk.gray(message));
      if (Object.keys(meta).length > 0) {
        console.log(chalk.gray(JSON.stringify(meta, null, 2)));
      }
    }

    await this.writeToFile(formatted);
  }

  /**
   * Log trace
   */
  async trace(message, meta = {}) {
    if (!this.shouldLog('trace')) return;

    const formatted = this.formatMessage('trace', message, meta);
    
    if (this.enableConsole) {
      console.log(chalk.gray('📍'), chalk.gray(message));
      if (Object.keys(meta).length > 0) {
        console.log(chalk.gray(JSON.stringify(meta, null, 2)));
      }
    }

    await this.writeToFile(formatted);
  }

  /**
   * Log success
   */
  async success(message, meta = {}) {
    const formatted = this.formatMessage('info', message, meta);
    
    if (this.enableConsole) {
      console.log(chalk.green('✓'), chalk.green(message));
      if (Object.keys(meta).length > 0) {
        console.log(chalk.gray(JSON.stringify(meta, null, 2)));
      }
    }

    await this.writeToFile(formatted);
  }

  // ===== PHASE 3 ENHANCEMENTS: Structured Logging & Aggregation =====

  /**
   * Get aggregated log statistics
   */
  getAggregatedLogs(options = {}) {
    const { groupBy = 'level', timeRange = null } = options;
    
    // In a real implementation, this would read from log files
    // For now, return mock aggregated data
    return {
      error: 5,
      warn: 12,
      info: 150,
      debug: 45,
      trace: 8
    };
  }

  /**
   * Search logs
   */
  async search(criteria = {}) {
    const { level, contains, after, before } = criteria;
    
    // In a real implementation, this would search log files
    // For now, return empty array
    return [];
  }

  /**
   * Export logs
   */
  async exportLogs(outputPath, options = {}) {
    const { format = 'json', filter = {} } = options;
    
    // In a real implementation, this would export logs
    // For now, just create empty file
    if (format === 'json') {
      await fs.writeFile(outputPath, JSON.stringify({ logs: [] }, null, 2));
    }
    
    return { exported: 0, path: outputPath };
  }
}

// Default logger instance
const defaultLogger = new Logger({
  logLevel: process.env.LOG_LEVEL || 'info',
  enableFile: process.env.ENABLE_FILE_LOGGING === 'true',
  enableConsole: true
});

module.exports = Logger;
module.exports.default = defaultLogger;
