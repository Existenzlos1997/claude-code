/**
 * Game Detection
 * Automatische Erkennung von installierten Spielen
 */

const fs = require('fs').promises;
const path = require('path');
const { glob } = require('glob');

class GameDetector {
  constructor() {
    this.scanPaths = [
      path.join(process.env.HOME || '', '.steam/steam/steamapps/common'),
      path.join(process.env.HOME || '', '.local/share/lutris/games'),
      '/opt/games'
    ];

    // Windows paths (if running on Windows or Wine)
    if (process.platform === 'win32') {
      this.scanPaths.push(
        'C:\\Program Files',
        'C:\\Program Files (x86)',
        'C:\\Games'
      );
    }
  }

  async detectSteamGames() {
    const games = [];
    const steamPath = path.join(process.env.HOME || '', '.steam/steam/steamapps/common');

    try {
      const dirs = await fs.readdir(steamPath);
      
      for (const dir of dirs) {
        const gamePath = path.join(steamPath, dir);
        const stat = await fs.stat(gamePath);
        
        if (stat.isDirectory()) {
          // Look for executables
          const executables = await glob('**/*.{exe,sh,x86_64}', {
            cwd: gamePath,
            maxDepth: 2,
            nocase: true
          });

          if (executables.length > 0) {
            games.push({
              name: dir,
              path: gamePath,
              executable: executables[0],
              platform: executables[0].endsWith('.exe') ? 'windows' : 'native',
              source: 'steam'
            });
          }
        }
      }
    } catch (err) {
      console.warn('Steam games detection failed:', err.message);
    }

    return games;
  }

  async detectAll() {
    const allGames = [];

    // Detect Steam games
    const steamGames = await this.detectSteamGames();
    allGames.push(...steamGames);

    return allGames;
  }
}

module.exports = GameDetector;
