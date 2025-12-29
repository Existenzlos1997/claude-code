/**
 * Memory Monitor - Monitor memory usage and alert on thresholds
 */

class MemoryMonitor {
  constructor(options = {}) {
    this.threshold = options.threshold || 0.8; // 80% default
    this.interval = null;
    this.callbacks = [];
  }

  /**
   * Start monitoring
   */
  start(intervalMs = 5000) {
    if (this.interval) {
      return;
    }

    this.interval = setInterval(() => {
      this._check();
    }, intervalMs);
  }

  /**
   * Stop monitoring
   */
  stop() {
    if (this.interval) {
      clearInterval(this.interval);
      this.interval = null;
    }
  }

  /**
   * Get current memory usage
   */
  getUsage() {
    const usage = process.memoryUsage();
    const totalHeap = usage.heapTotal;
    const usedHeap = usage.heapUsed;
    const percent = usedHeap / totalHeap;

    return {
      heapUsed: this._formatBytes(usedHeap),
      heapTotal: this._formatBytes(totalHeap),
      heapUsedRaw: usedHeap,
      heapTotalRaw: totalHeap,
      percent: (percent * 100).toFixed(1),
      percentRaw: percent,
      rss: this._formatBytes(usage.rss),
      external: this._formatBytes(usage.external)
    };
  }

  /**
   * Register threshold exceeded callback
   */
  onThresholdExceeded(callback) {
    this.callbacks.push(callback);
  }

  /**
   * Check memory and trigger callbacks
   */
  _check() {
    const usage = this.getUsage();

    if (usage.percentRaw > this.threshold) {
      this.callbacks.forEach(callback => {
        try {
          callback(usage);
        } catch (error) {
          console.error('Memory monitor callback error:', error);
        }
      });
    }
  }

  /**
   * Format bytes to human-readable string
   */
  _formatBytes(bytes) {
    const units = ['B', 'KB', 'MB', 'GB'];
    let value = bytes;
    let unitIndex = 0;

    while (value >= 1024 && unitIndex < units.length - 1) {
      value /= 1024;
      unitIndex++;
    }

    return `${value.toFixed(2)} ${units[unitIndex]}`;
  }
}

module.exports = MemoryMonitor;
