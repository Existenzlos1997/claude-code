/**
 * Cross-Platform Game System - Main Entry Point
 * 
 * Haupteinstiegspunkt für das plattformübergreifende Spielesystem
 */

const chalk = require('chalk');
const { program } = require('commander');
const GameLibrary = require('./core/game-library');
const CompatibilityManager = require('./compatibility/manager');
const config = require('./config/config');

class GameSystem {
  constructor() {
    this.library = new GameLibrary();
    this.compatibilityManager = new CompatibilityManager();
  }

  async initialize() {
    console.log(chalk.blue('🎮 Cross-Platform Game System'));
    console.log(chalk.gray('Initializing system...'));
    
    await this.compatibilityManager.loadPlugins();
    await this.library.load();
    
    console.log(chalk.green('✓ System initialized'));
    console.log(chalk.gray(`Found ${this.library.games.length} games`));
  }

  async launch(gameName) {
    const game = this.library.findGame(gameName);
    if (!game) {
      console.error(chalk.red(`Game "${gameName}" not found`));
      return;
    }

    console.log(chalk.blue(`Launching: ${game.name}`));
    await this.compatibilityManager.launch(game);
  }

  listGames() {
    console.log(chalk.blue('\n📚 Game Library:\n'));
    this.library.games.forEach(game => {
      console.log(chalk.white(`  • ${game.name}`));
      console.log(chalk.gray(`    Platform: ${game.platform} | Layer: ${game.compatibilityLayer || 'native'}`));
    });
  }
}

// CLI Interface
async function main() {
  program
    .name('game-system')
    .description('Cross-platform game compatibility system')
    .version('0.1.0');

  program
    .command('launch <game>')
    .description('Launch a game')
    .action(async (gameName) => {
      const system = new GameSystem();
      await system.initialize();
      await system.launch(gameName);
    });

  program
    .command('list')
    .description('List all games')
    .action(async () => {
      const system = new GameSystem();
      await system.initialize();
      system.listGames();
    });

  program
    .command('detect')
    .description('Detect installed games')
    .action(async () => {
      const system = new GameSystem();
      await system.initialize();
      console.log(chalk.yellow('Game detection not yet implemented'));
    });

  program.parse();
}

if (require.main === module) {
  main().catch(err => {
    console.error(chalk.red('Error:'), err.message);
    process.exit(1);
  });
}

module.exports = GameSystem;
