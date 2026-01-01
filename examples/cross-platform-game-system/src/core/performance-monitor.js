/**
 * Performance Monitor
 * Überwacht Spiel-Performance und FPS
 * Monitors game performance and FPS
 */

const { execa } = require('execa');
const fs = require('fs').promises;
const path = require('path');

class PerformanceMonitor {
  constructor() {
    this.metrics = [];
    this.isMonitoring = false;
  }

  /**
   * Start monitoring a game process
   */
  async startMonitoring(gameName, pid) {
    this.isMonitoring = true;
    this.gameName = gameName;
    this.pid = pid;
    this.startTime = Date.now();

    console.log(`📊 Started performance monitoring for ${gameName} (PID: ${pid})`);

    // Monitor in background
    this.monitorInterval = setInterval(async () => {
      if (!this.isMonitoring) return;

      try {
        const metrics = await this.collectMetrics(pid);
        this.metrics.push({
          timestamp: Date.now(),
          ...metrics
        });
      } catch (err) {
        // Process might have ended
        this.stopMonitoring();
      }
    }, 5000); // Every 5 seconds
  }

  /**
   * Collect current metrics
   */
  async collectMetrics(pid) {
    const metrics = {
      cpu: 0,
      memory: 0,
      threads: 0
    };

    try {
      // Get process info using ps
      const { stdout } = await execa('ps', ['-p', pid.toString(), '-o', '%cpu,%mem,nlwp', '--no-headers']);
      const parts = stdout.trim().split(/\s+/);
      
      if (parts.length >= 3) {
        metrics.cpu = parseFloat(parts[0]) || 0;
        metrics.memory = parseFloat(parts[1]) || 0;
        metrics.threads = parseInt(parts[2]) || 0;
      }
    } catch (err) {
      // Process not found
      throw new Error('Process not found');
    }

    return metrics;
  }

  /**
   * Stop monitoring
   */
  stopMonitoring() {
    if (this.monitorInterval) {
      clearInterval(this.monitorInterval);
      this.monitorInterval = null;
    }
    this.isMonitoring = false;
  }

  /**
   * Generate performance report
   */
  async generateReport() {
    if (this.metrics.length === 0) {
      return null;
    }

    const duration = (Date.now() - this.startTime) / 1000; // seconds

    // Calculate averages
    const avgCpu = this.metrics.reduce((sum, m) => sum + m.cpu, 0) / this.metrics.length;
    const avgMemory = this.metrics.reduce((sum, m) => sum + m.memory, 0) / this.metrics.length;
    const maxCpu = Math.max(...this.metrics.map(m => m.cpu));
    const maxMemory = Math.max(...this.metrics.map(m => m.memory));

    const report = {
      game: this.gameName,
      duration: Math.round(duration),
      samples: this.metrics.length,
      cpu: {
        average: Math.round(avgCpu * 10) / 10,
        max: Math.round(maxCpu * 10) / 10
      },
      memory: {
        average: Math.round(avgMemory * 10) / 10,
        max: Math.round(maxMemory * 10) / 10
      },
      metrics: this.metrics
    };

    return report;
  }

  /**
   * Save report to file
   */
  async saveReport(outputPath) {
    const report = await this.generateReport();
    if (!report) {
      return false;
    }

    const homeDir = process.env.HOME || process.env.USERPROFILE;
    const reportDir = path.join(homeDir, '.game-system', 'reports');
    await fs.mkdir(reportDir, { recursive: true });

    const filename = outputPath || path.join(reportDir, `${this.gameName}-${Date.now()}.json`);
    await fs.writeFile(filename, JSON.stringify(report, null, 2));

    console.log(`📊 Performance report saved: ${filename}`);
    return filename;
  }

  /**
   * Print summary
   */
  printSummary() {
    if (this.metrics.length === 0) {
      console.log('No performance data collected');
      return;
    }

    const report = {
      game: this.gameName,
      samples: this.metrics.length,
      cpu: {
        avg: this.metrics.reduce((sum, m) => sum + m.cpu, 0) / this.metrics.length,
        max: Math.max(...this.metrics.map(m => m.cpu))
      },
      memory: {
        avg: this.metrics.reduce((sum, m) => sum + m.memory, 0) / this.metrics.length,
        max: Math.max(...this.metrics.map(m => m.memory))
      }
    };

    console.log('\n📊 Performance Summary:');
    console.log(`   Game: ${report.game}`);
    console.log(`   Samples: ${report.samples}`);
    console.log(`   CPU: ${report.cpu.avg.toFixed(1)}% avg, ${report.cpu.max.toFixed(1)}% max`);
    console.log(`   Memory: ${report.memory.avg.toFixed(1)}% avg, ${report.memory.max.toFixed(1)}% max`);
  }
}

module.exports = PerformanceMonitor;
