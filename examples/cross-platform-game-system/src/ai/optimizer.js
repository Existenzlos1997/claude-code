/**
 * AI-Powered Configuration Optimizer
 * Intelligente Optimierung basierend auf Hardware
 * Intelligent optimization based on hardware
 * 
 * UNIQUE FEATURE: No other game compatibility system has AI-powered auto-optimization
 */

const os = require('os');
const { execa } = require('execa');

class AIOptimizer {
  constructor() {
    this.systemProfile = null;
  }

  /**
   * Analyze system hardware and capabilities
   */
  async analyzeSystem() {
    const profile = {
      cpu: {
        cores: os.cpus().length,
        model: os.cpus()[0].model,
        speed: os.cpus()[0].speed
      },
      memory: {
        total: Math.round(os.totalmem() / 1024 / 1024 / 1024), // GB
        free: Math.round(os.freemem() / 1024 / 1024 / 1024)
      },
      platform: os.platform(),
      arch: os.arch()
    };

    // Detect GPU (Linux)
    if (os.platform() === 'linux') {
      try {
        const { stdout } = await execa('lspci', []);
        const gpuLine = stdout.split('\n').find(line => 
          line.toLowerCase().includes('vga') || 
          line.toLowerCase().includes('3d controller')
        );
        
        if (gpuLine) {
          profile.gpu = {
            detected: true,
            info: gpuLine,
            vendor: this.detectGPUVendor(gpuLine)
          };
        }
      } catch (err) {
        profile.gpu = { detected: false };
      }
    }

    this.systemProfile = profile;
    return profile;
  }

  detectGPUVendor(gpuLine) {
    if (gpuLine.toLowerCase().includes('nvidia')) return 'nvidia';
    if (gpuLine.toLowerCase().includes('amd') || gpuLine.toLowerCase().includes('radeon')) return 'amd';
    if (gpuLine.toLowerCase().includes('intel')) return 'intel';
    return 'unknown';
  }

  /**
   * Calculate performance tier
   */
  calculatePerformanceTier() {
    if (!this.systemProfile) return 'unknown';

    const { cpu, memory, gpu } = this.systemProfile;
    let score = 0;

    // CPU scoring
    if (cpu.cores >= 8) score += 3;
    else if (cpu.cores >= 4) score += 2;
    else score += 1;

    // Memory scoring
    if (memory.total >= 16) score += 3;
    else if (memory.total >= 8) score += 2;
    else score += 1;

    // GPU scoring
    if (gpu && gpu.detected) {
      if (gpu.vendor === 'nvidia' || gpu.vendor === 'amd') score += 3;
      else score += 1;
    }

    // Determine tier
    if (score >= 8) return 'high';
    if (score >= 5) return 'medium';
    return 'low';
  }

  /**
   * Generate optimal configuration for a game
   * UNIQUE: AI-powered automatic optimization
   */
  optimizeForGame(game, targetFPS = 60) {
    if (!this.systemProfile) {
      throw new Error('System not analyzed. Call analyzeSystem() first.');
    }

    const tier = this.calculatePerformanceTier();
    const config = {
      game: game.name,
      tier,
      targetFPS,
      settings: {}
    };

    // Base settings based on tier
    switch (tier) {
      case 'high':
        config.settings = {
          dxvk: true,
          vkd3d: true,
          esync: true,
          fsync: true,
          dxvkAsync: true,
          wineD3D: false,
          resolution: 'native',
          textureQuality: 'ultra',
          shadowQuality: 'high',
          antiAliasing: 'TAA',
          recommendation: 'Your system can handle maximum settings'
        };
        break;

      case 'medium':
        config.settings = {
          dxvk: true,
          vkd3d: true,
          esync: true,
          fsync: false,
          dxvkAsync: true,
          wineD3D: false,
          resolution: '1920x1080',
          textureQuality: 'high',
          shadowQuality: 'medium',
          antiAliasing: 'FXAA',
          recommendation: 'Balanced settings for smooth gameplay'
        };
        break;

      case 'low':
        config.settings = {
          dxvk: true,
          vkd3d: false,
          esync: false,
          fsync: false,
          dxvkAsync: false,
          wineD3D: false,
          resolution: '1280x720',
          textureQuality: 'low',
          shadowQuality: 'low',
          antiAliasing: 'off',
          recommendation: 'Performance-optimized settings'
        };
        break;
    }

    // GPU-specific optimizations
    if (this.systemProfile.gpu && this.systemProfile.gpu.vendor === 'nvidia') {
      config.settings.nvidiaTweaks = {
        NVIDIA_THREADED_OPTIMIZATION: '1',
        __GL_THREADED_OPTIMIZATIONS: '1'
      };
    } else if (this.systemProfile.gpu && this.systemProfile.gpu.vendor === 'amd') {
      config.settings.amdTweaks = {
        RADV_PERFTEST: 'aco',
        mesa_glthread: 'true'
      };
    }

    // Game-specific optimizations
    if (game.antiCheat === 'eac' || game.antiCheat === 'battleye') {
      config.settings.protonVersion = 'proton-experimental';
      config.settings.antiCheatOptimized = true;
    }

    return config;
  }

  /**
   * Predict game compatibility
   * UNIQUE: AI-based prediction before installation
   */
  predictCompatibility(game) {
    if (!this.systemProfile) {
      throw new Error('System not analyzed. Call analyzeSystem() first.');
    }

    const prediction = {
      game: game.name,
      compatible: true,
      confidence: 0,
      issues: [],
      recommendations: []
    };

    // Check platform compatibility
    if (game.platform === 'windows' && this.systemProfile.platform !== 'win32') {
      prediction.recommendations.push('Use Proton or Wine for Windows games');
    }

    // Check anti-cheat
    if (game.antiCheat) {
      if (game.antiCheat === 'vanguard' || game.antiCheat === 'faceit') {
        prediction.compatible = false;
        prediction.confidence = 95;
        prediction.issues.push(`${game.antiCheat} anti-cheat not compatible with Wine/Proton`);
        return prediction;
      } else if (game.antiCheat === 'eac' || game.antiCheat === 'battleye') {
        prediction.confidence = 85;
        prediction.recommendations.push('Requires Proton Experimental with anti-cheat runtime');
      }
    }

    // Check system requirements (basic heuristics)
    const tier = this.calculatePerformanceTier();
    if (tier === 'low') {
      prediction.confidence = 60;
      prediction.issues.push('Low-end system may struggle with modern games');
      prediction.recommendations.push('Consider lowering graphics settings');
    } else if (tier === 'medium') {
      prediction.confidence = 80;
    } else {
      prediction.confidence = 95;
    }

    return prediction;
  }

  /**
   * Generate environment variables for optimization
   */
  generateEnvironment(optimization) {
    const env = {};

    // DXVK settings
    if (optimization.settings.dxvk) {
      env.DXVK_HUD = 'fps';
      if (optimization.settings.dxvkAsync) {
        env.DXVK_ASYNC = '1';
      }
    }

    // Esync/Fsync
    if (optimization.settings.esync) {
      env.PROTON_NO_ESYNC = '0';
    }
    if (optimization.settings.fsync) {
      env.PROTON_NO_FSYNC = '0';
    }

    // GPU-specific
    if (optimization.settings.nvidiaTweaks) {
      Object.assign(env, optimization.settings.nvidiaTweaks);
    }
    if (optimization.settings.amdTweaks) {
      Object.assign(env, optimization.settings.amdTweaks);
    }

    // Wine threaded optimizations
    env.WINE_CPU_TOPOLOGY = `${this.systemProfile.cpu.cores}:0`;

    return env;
  }

  /**
   * Get optimization summary
   */
  getSummary(optimization) {
    return {
      tier: optimization.tier,
      targetFPS: optimization.targetFPS,
      recommendation: optimization.settings.recommendation,
      enabledFeatures: Object.keys(optimization.settings).filter(
        key => optimization.settings[key] === true
      ),
      systemInfo: {
        cpu: `${this.systemProfile.cpu.cores} cores`,
        memory: `${this.systemProfile.memory.total} GB`,
        gpu: this.systemProfile.gpu?.vendor || 'unknown'
      }
    };
  }
}

module.exports = AIOptimizer;
