/**
 * Native Games Plugin
 * For games that run natively on the current platform
 */

const { execa } = require('execa');

module.exports = {
  name: 'native',
  platform: 'native',
  description: 'Run native games',

  async canRun(game) {
    return game.platform === 'native' || game.platform === process.platform;
  },

  async launch(game, options = {}) {
    console.log(`Launching ${game.name} (native)`);
    console.log(`Executable: ${game.executable}`);

    const gameProcess = execa(game.executable, options.args || [], {
      cwd: game.path,
      stdio: 'inherit'
    });

    return gameProcess;
  }
};
