/**
 * Configuration Management
 * Zentrale Konfigurationsverwaltung
 */

const Conf = require('conf');

const config = new Conf({
  projectName: 'cross-platform-game-system',
  defaults: {
    defaultCompatibilityLayer: 'proton',
    winePrefixPath: '~/.wine-games',
    autoDetectGames: true,
    platforms: ['windows', 'linux', 'native'],
    wine: {
      version: 'wine-stable',
      prefix: '~/.wine-games',
      dxvk: true,
      vkd3d: true
    },
    proton: {
      version: 'latest',
      experimentalFeatures: true
    },
    detection: {
      scanPaths: [
        '~/.steam/steam/steamapps/common',
        '~/.local/share/lutris/games',
        '/opt/games',
        'C:\\Program Files',
        'C:\\Program Files (x86)'
      ]
    }
  }
});

module.exports = config;
