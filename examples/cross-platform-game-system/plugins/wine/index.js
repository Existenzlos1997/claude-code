/**
 * Wine Compatibility Plugin
 * Ermöglicht das Ausführen von Windows-Spielen unter Linux/Unix
 */

const { execa } = require('execa');
const path = require('path');

module.exports = {
  name: 'wine',
  platform: 'windows',
  description: 'Run Windows games using Wine',

  async canRun(game) {
    // Check if wine is installed
    try {
      await execa('wine', ['--version']);
      return game.platform === 'windows';
    } catch (err) {
      return false;
    }
  },

  async launch(game, options = {}) {
    const prefix = options.winePrefix || process.env.WINEPREFIX || path.join(process.env.HOME, '.wine');
    
    const env = {
      ...process.env,
      WINEPREFIX: prefix
    };

    // Enable DXVK if available
    if (options.dxvk) {
      env.DXVK_HUD = '1';
    }

    console.log(`Launching ${game.name} with Wine`);
    console.log(`Prefix: ${prefix}`);
    console.log(`Executable: ${game.executable}`);

    const wineProcess = execa('wine', [game.executable], {
      cwd: game.path,
      env,
      stdio: 'inherit'
    });

    return wineProcess;
  },

  async configure(options) {
    // Run winecfg for configuration
    await execa('winecfg', [], { stdio: 'inherit' });
  },

  async installDependencies(game) {
    // Use winetricks to install common dependencies
    const tricks = ['vcrun2019', 'dotnet48', 'dxvk'];
    console.log(`Installing dependencies: ${tricks.join(', ')}`);
    
    for (const trick of tricks) {
      try {
        await execa('winetricks', [trick], { stdio: 'inherit' });
      } catch (err) {
        console.warn(`Failed to install ${trick}:`, err.message);
      }
    }
  }
};
