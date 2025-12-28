/**
 * Add Game CLI
 * Manuelles Hinzufügen von Spielen zur Bibliothek
 */

const { program } = require('commander');
const chalk = require('chalk');
const GameLibrary = require('../core/game-library');

async function addGame(options) {
  const library = new GameLibrary();
  await library.load();

  const game = {
    name: options.name,
    path: options.path,
    platform: options.platform || 'windows',
    executable: options.executable || 'game.exe',
    compatibilityLayer: options.layer || 'proton'
  };

  library.addGame(game);
  await library.save();

  console.log(chalk.green(`✓ Added game: ${game.name}`));
  console.log(chalk.gray(`  Path: ${game.path}`));
  console.log(chalk.gray(`  Platform: ${game.platform}`));
  console.log(chalk.gray(`  Layer: ${game.compatibilityLayer}`));
}

program
  .option('-n, --name <name>', 'Game name')
  .option('-p, --path <path>', 'Game directory path')
  .option('-e, --executable <file>', 'Executable file', 'game.exe')
  .option('--platform <platform>', 'Platform (windows/linux/native)', 'windows')
  .option('--layer <layer>', 'Compatibility layer (wine/proton/native)', 'proton')
  .parse();

const options = program.opts();

if (!options.name || !options.path) {
  console.error(chalk.red('Error: --name and --path are required'));
  program.help();
} else {
  addGame(options).catch(err => {
    console.error(chalk.red('Error:'), err.message);
    process.exit(1);
  });
}
