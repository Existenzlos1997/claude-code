/**
 * Proton Compatibility Plugin
 * Valve's Wine-based compatibility layer optimized for games
 */

const { execa } = require('execa');
const path = require('path');
const fs = require('fs').promises;

module.exports = {
  name: 'proton',
  platform: 'windows',
  description: 'Run Windows games using Proton (Steam Play)',

  async canRun(game) {
    // Check if Proton is available
    const protonPaths = [
      path.join(process.env.HOME, '.steam/steam/steamapps/common/Proton 8.0'),
      path.join(process.env.HOME, '.steam/steam/steamapps/common/Proton - Experimental'),
      '/usr/share/steam/compatibilitytools.d/proton'
    ];

    for (const protonPath of protonPaths) {
      try {
        await fs.access(path.join(protonPath, 'proton'));
        return game.platform === 'windows';
      } catch (err) {
        continue;
      }
    }
    return false;
  },

  async findProton() {
    const protonPaths = [
      path.join(process.env.HOME, '.steam/steam/steamapps/common/Proton 8.0'),
      path.join(process.env.HOME, '.steam/steam/steamapps/common/Proton - Experimental')
    ];

    for (const protonPath of protonPaths) {
      try {
        await fs.access(path.join(protonPath, 'proton'));
        return protonPath;
      } catch (err) {
        continue;
      }
    }
    throw new Error('Proton not found. Please install Steam and Proton.');
  },

  async launch(game, options = {}) {
    const protonPath = await this.findProton();
    const protonBinary = path.join(protonPath, 'proton');
    
    const prefix = options.prefix || path.join(process.env.HOME, '.proton-games', game.name);

    const env = {
      ...process.env,
      STEAM_COMPAT_DATA_PATH: prefix,
      STEAM_COMPAT_CLIENT_INSTALL_PATH: path.join(process.env.HOME, '.steam/steam'),
      PROTON_USE_WINED3D: options.useWineD3D ? '1' : '0',
      PROTON_NO_ESYNC: options.noEsync ? '1' : '0',
      PROTON_NO_FSYNC: options.noFsync ? '1' : '0',
      DXVK_HUD: options.dxvkHud ? 'fps,devinfo' : '0'
    };

    // Anti-cheat support
    if (options.anticheat) {
      const AntiCheatManager = require('../../src/compatibility/anticheat');
      const acManager = new AntiCheatManager();
      
      // Detect anti-cheat in game directory
      const detected = await acManager.detectAntiCheat(game.path);
      
      if (detected.length > 0) {
        console.log(`Detected anti-cheat: ${detected.map(d => d.name).join(', ')}`);
        
        // Apply anti-cheat environment settings
        detected.forEach(ac => {
          const acEnv = acManager.getAntiCheatEnvironment(ac.type, env);
          Object.assign(env, acEnv);
        });
      }
    }

    // EasyAntiCheat runtime
    const eacRuntime = path.join(process.env.HOME, '.steam/steam/steamapps/common/Proton EasyAntiCheat Runtime');
    try {
      await fs.access(eacRuntime);
      env.PROTON_EAC_RUNTIME = eacRuntime;
    } catch (err) {
      // EAC runtime not installed
    }

    // BattlEye runtime
    const beRuntime = path.join(process.env.HOME, '.steam/steam/steamapps/common/Proton BattlEye Runtime');
    try {
      await fs.access(beRuntime);
      env.PROTON_BATTLEYE_RUNTIME = beRuntime;
    } catch (err) {
      // BattlEye runtime not installed
    }

    console.log(`Launching ${game.name} with Proton`);
    console.log(`Proton: ${protonPath}`);
    console.log(`Prefix: ${prefix}`);
    console.log(`Executable: ${game.executable}`);

    // Create prefix directory
    await fs.mkdir(prefix, { recursive: true });

    const protonProcess = execa('python3', [protonBinary, 'run', game.executable], {
      cwd: game.path,
      env,
      stdio: 'inherit'
    });

    return protonProcess;
  }
};
