/**
 * Anti-Cheat Detection and Compatibility Module
 * Erkennt und konfiguriert Anti-Cheat-Systeme
 */

const fs = require('fs').promises;
const path = require('path');
const { execa } = require('execa');
const chalk = require('chalk');

class AntiCheatManager {
  constructor() {
    this.supportedSystems = {
      eac: {
        name: 'EasyAntiCheat',
        files: ['EasyAntiCheat.exe', 'EasyAntiCheat_x64.dll', 'EasyAntiCheat_x86.dll'],
        supported: true,
        requiresProtonGE: false,
        notes: 'Native Linux support via Proton since 2021'
      },
      battleye: {
        name: 'BattlEye',
        files: ['BEClient.dll', 'BEClient_x64.dll', 'BattlEye.exe'],
        supported: true,
        requiresProtonGE: false,
        notes: 'Native Linux support available'
      },
      vac: {
        name: 'Valve Anti-Cheat',
        files: [],
        supported: true,
        requiresProtonGE: false,
        notes: 'Works natively on Linux via Steam'
      },
      denuvo: {
        name: 'Denuvo Anti-Tamper',
        files: [],
        supported: true,
        requiresProtonGE: false,
        notes: 'Generally works with Wine/Proton'
      },
      vanguard: {
        name: 'Riot Vanguard',
        files: ['vgk.sys', 'vgc.exe'],
        supported: false,
        requiresProtonGE: false,
        notes: 'Requires kernel-mode driver, not compatible with Wine/Proton'
      },
      faceit: {
        name: 'FACEIT Anti-Cheat',
        files: ['FACEIT.exe'],
        supported: false,
        requiresProtonGE: false,
        notes: 'Kernel-mode driver required, not compatible'
      }
    };
  }

  /**
   * Detect anti-cheat system in game directory
   */
  async detectAntiCheat(gamePath) {
    const detected = [];

    for (const [key, system] of Object.entries(this.supportedSystems)) {
      if (system.files.length === 0) continue;

      for (const file of system.files) {
        try {
          const filePath = path.join(gamePath, file);
          await fs.access(filePath);
          detected.push({
            type: key,
            ...system,
            filePath
          });
          break; // Found one file, that's enough
        } catch (err) {
          // File doesn't exist, continue
        }
      }
    }

    return detected;
  }

  /**
   * Check if anti-cheat is compatible with current setup
   */
  isCompatible(antiCheatType) {
    const system = this.supportedSystems[antiCheatType];
    return system ? system.supported : false;
  }

  /**
   * Get recommended Proton version for anti-cheat
   */
  getRecommendedProton(antiCheatType) {
    const system = this.supportedSystems[antiCheatType];
    
    if (!system || !system.supported) {
      return null;
    }

    if (system.requiresProtonGE) {
      return 'proton-ge';
    }

    if (antiCheatType === 'eac' || antiCheatType === 'battleye') {
      return 'proton-experimental';
    }

    return 'proton-latest';
  }

  /**
   * Configure environment for anti-cheat
   */
  getAntiCheatEnvironment(antiCheatType, baseEnv = {}) {
    const env = { ...baseEnv };

    switch (antiCheatType) {
      case 'eac':
        env.PROTON_EAC_RUNTIME = path.join(process.env.HOME, '.steam/steam/steamapps/common/Proton EasyAntiCheat Runtime');
        env.PROTON_USE_EAC = '1';
        break;

      case 'battleye':
        env.PROTON_BATTLEYE_RUNTIME = path.join(process.env.HOME, '.steam/steam/steamapps/common/Proton BattlEye Runtime');
        env.PROTON_USE_BATTLEYE = '1';
        break;

      case 'vanguard':
      case 'faceit':
        console.warn(chalk.yellow(`Warning: ${this.supportedSystems[antiCheatType].name} is not supported on Linux`));
        break;
    }

    // General anti-cheat compatibility settings
    env.WINE_HIDE_WINE = '1';
    env.WINE_SIMULATE_WRITECOPY = '1';
    
    return env;
  }

  /**
   * Print compatibility report
   */
  printCompatibilityReport(detectedSystems) {
    console.log(chalk.blue('\n🛡️  Anti-Cheat Detection Report\n'));

    if (detectedSystems.length === 0) {
      console.log(chalk.green('✓ No anti-cheat systems detected'));
      return;
    }

    detectedSystems.forEach(system => {
      const status = system.supported ? 
        chalk.green('✓ Supported') : 
        chalk.red('✗ Not Supported');
      
      console.log(`${status} ${chalk.white(system.name)}`);
      console.log(chalk.gray(`  Type: ${system.type}`));
      console.log(chalk.gray(`  File: ${system.filePath}`));
      console.log(chalk.gray(`  Notes: ${system.notes}`));
      
      if (system.supported && system.requiresProtonGE) {
        console.log(chalk.yellow('  ⚠️  Requires Proton GE'));
      }
      
      console.log();
    });
  }

  /**
   * Get workaround suggestions for unsupported anti-cheat
   */
  getWorkarounds(antiCheatType) {
    if (this.isCompatible(antiCheatType)) {
      return null;
    }

    const homeDir = process.env.HOME || process.env.USERPROFILE || '~';

    return {
      dualBoot: 'Install Windows alongside Linux for games with incompatible anti-cheat',
      gpuPassthrough: 'Use Windows VM with GPU passthrough for near-native performance',
      cloudGaming: 'Use cloud gaming services (GeForce NOW, etc.) that run on remote servers',
      contactDev: 'Contact game developer to request Linux anti-cheat support'
    };
  }
}

module.exports = AntiCheatManager;
