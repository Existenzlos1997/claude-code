#!/usr/bin/env node
/**
 * Game Detection CLI
 */

const GameDetector = require('./detector');
const chalk = require('chalk');

async function main() {
  console.log(chalk.blue('🔍 Detecting installed games...\n'));

  const detector = new GameDetector();
  const games = await detector.detectAll();

  if (games.length === 0) {
    console.log(chalk.yellow('No games found.'));
    console.log(chalk.gray('Make sure games are installed in common locations.'));
    return;
  }

  console.log(chalk.green(`Found ${games.length} game(s):\n`));

  games.forEach((game, index) => {
    console.log(chalk.white(`${index + 1}. ${game.name}`));
    console.log(chalk.gray(`   Path: ${game.path}`));
    console.log(chalk.gray(`   Platform: ${game.platform}`));
    console.log(chalk.gray(`   Source: ${game.source}`));
    console.log();
  });
}

if (require.main === module) {
  main().catch(err => {
    console.error(chalk.red('Error:'), err.message);
    process.exit(1);
  });
}
