/**
 * Error Handler - Graceful error handling with automatic recovery strategies
 */

class ErrorHandler {
  constructor() {
    this.recoveryStrategies = new Map();
    this.errorLog = [];
    this.maxLogSize = 1000;
  }

  /**
   * Register a recovery strategy for an error type
   */
  registerRecovery(errorType, strategy) {
    this.recoveryStrategies.set(errorType, strategy);
  }

  /**
   * Categorize error by type and severity
   */
  categorize(error) {
    const category = this._detectCategory(error);
    const severity = this._detectSeverity(error);
    const recoverable = this.recoveryStrategies.has(category);

    return { category, severity, recoverable };
  }

  /**
   * Detect error category
   */
  _detectCategory(error) {
    const message = error.message || '';

    if (message.includes('ENOENT') || message.includes('EACCES')) {
      return 'FILESYSTEM';
    }
    if (message.includes('ECONNREFUSED') || message.includes('ETIMEDOUT')) {
      return 'NETWORK';
    }
    if (message.includes('plugin') || message.includes('Plugin')) {
      return 'PLUGIN';
    }
    if (message.includes('save') || message.includes('sync')) {
      return 'SAVE_SYNC';
    }
    if (message.includes('shader') || message.includes('cache')) {
      return 'SHADER_CACHE';
    }
    
    return 'UNKNOWN';
  }

  /**
   * Detect error severity
   */
  _detectSeverity(error) {
    const fatal = ['FATAL', 'CRITICAL', 'segmentation fault'];
    const high = ['EACCES', 'permission denied', 'out of memory'];
    const medium = ['ENOENT', 'not found', 'timeout'];

    const message = error.message || '';

    if (fatal.some(keyword => message.toLowerCase().includes(keyword.toLowerCase()))) {
      return 'CRITICAL';
    }
    if (high.some(keyword => message.toLowerCase().includes(keyword.toLowerCase()))) {
      return 'HIGH';
    }
    if (medium.some(keyword => message.toLowerCase().includes(keyword.toLowerCase()))) {
      return 'MEDIUM';
    }

    return 'LOW';
  }

  /**
   * Handle error with automatic recovery
   */
  async handle(error, context = {}) {
    const categorized = this.categorize(error);
    
    this._logError(error, categorized, context);

    if (categorized.recoverable) {
      try {
        const strategy = this.recoveryStrategies.get(categorized.category);
        const result = await strategy(error, context);
        
        if (result && result.recovered) {
          return {
            recovered: true,
            message: result.message || 'Error recovered successfully',
            data: result.data
          };
        }
      } catch (recoveryError) {
        console.error('Recovery failed:', recoveryError);
      }
    }

    return {
      recovered: false,
      category: categorized.category,
      severity: categorized.severity,
      message: error.message
    };
  }

  /**
   * Graceful degradation when recovery fails
   */
  async degradeGracefully(error, context = {}) {
    const categorized = this.categorize(error);

    switch (categorized.category) {
      case 'PLUGIN':
        return {
          action: 'fallback',
          message: 'Using fallback plugin',
          degraded: true
        };

      case 'NETWORK':
        return {
          action: 'offline_mode',
          message: 'Continuing in offline mode',
          degraded: true
        };

      case 'SHADER_CACHE':
        return {
          action: 'skip_cache',
          message: 'Skipping shader cache optimization',
          degraded: true
        };

      default:
        return {
          action: 'continue',
          message: 'Continuing with degraded functionality',
          degraded: true
        };
    }
  }

  /**
   * Retry with exponential backoff
   */
  async retry(callback, options = {}) {
    const {
      maxAttempts = 3,
      backoff = 'exponential',
      initialDelay = 1000,
      maxDelay = 30000
    } = options;

    let attempt = 0;
    let delay = initialDelay;

    while (attempt < maxAttempts) {
      try {
        return await callback();
      } catch (error) {
        attempt++;
        
        if (attempt >= maxAttempts) {
          throw error;
        }

        await this._sleep(delay);

        if (backoff === 'exponential') {
          delay = Math.min(delay * 2, maxDelay);
        } else if (backoff === 'linear') {
          delay = Math.min(delay + initialDelay, maxDelay);
        }
      }
    }
  }

  /**
   * Log error
   */
  _logError(error, categorized, context) {
    const entry = {
      timestamp: new Date().toISOString(),
      error: {
        message: error.message,
        stack: error.stack
      },
      category: categorized.category,
      severity: categorized.severity,
      context
    };

    this.errorLog.push(entry);

    if (this.errorLog.length > this.maxLogSize) {
      this.errorLog.shift();
    }
  }

  /**
   * Get error statistics
   */
  getStats() {
    const byCategory = {};
    const bySeverity = {};

    this.errorLog.forEach(entry => {
      byCategory[entry.category] = (byCategory[entry.category] || 0) + 1;
      bySeverity[entry.severity] = (bySeverity[entry.severity] || 0) + 1;
    });

    return {
      total: this.errorLog.length,
      byCategory,
      bySeverity,
      recent: this.errorLog.slice(-10)
    };
  }

  /**
   * Clear error log
   */
  clearLog() {
    this.errorLog = [];
  }

  /**
   * Sleep utility
   */
  _sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}

module.exports = ErrorHandler;
