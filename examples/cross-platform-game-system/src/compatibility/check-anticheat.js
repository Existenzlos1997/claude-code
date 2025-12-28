#!/usr/bin/env node
/**
 * Anti-Cheat Compatibility Checker
 * Überprüft Anti-Cheat-Kompatibilität für Spiele
 */

const { program } = require('commander');
const chalk = require('chalk');
const GameLibrary = require('../core/game-library');
const AntiCheatManager = require('../compatibility/anticheat');

async function checkAntiCheat(gameName) {
  console.log(chalk.blue('🛡️  Anti-Cheat Compatibility Checker\n'));

  const library = new GameLibrary();
  await library.load();

  const game = library.findGame(gameName);
  
  if (!game) {
    console.error(chalk.red(`Game "${gameName}" not found in library`));
    console.log(chalk.gray('\nUse "npm start list" to see available games'));
    process.exit(1);
  }

  console.log(chalk.white(`Checking: ${game.name}`));
  console.log(chalk.gray(`Path: ${game.path}\n`));

  const acManager = new AntiCheatManager();
  const detected = await acManager.detectAntiCheat(game.path);

  acManager.printCompatibilityReport(detected);

  // Print recommendations
  if (detected.length > 0) {
    console.log(chalk.blue('📋 Recommendations:\n'));

    detected.forEach(system => {
      if (system.supported) {
        const proton = acManager.getRecommendedProton(system.type);
        console.log(chalk.green(`✓ ${system.name}:`));
        console.log(chalk.gray(`  Recommended Proton: ${proton}`));
        console.log(chalk.gray(`  Command: npm start launch "${game.name}" -- --layer proton`));
      } else {
        console.log(chalk.red(`✗ ${system.name}:`));
        console.log(chalk.yellow('  This anti-cheat is not compatible with Linux'));
        
        const workarounds = acManager.getWorkarounds(system.type);
        console.log(chalk.gray('\n  Possible workarounds:'));
        Object.entries(workarounds).forEach(([key, value]) => {
          console.log(chalk.gray(`    • ${value}`));
        });
      }
      console.log();
    });
  }

  // ProtonDB link
  console.log(chalk.blue('🔗 Additional Resources:\n'));
  console.log(chalk.gray(`  ProtonDB: https://www.protondb.com/search?q=${encodeURIComponent(game.name)}`));
  console.log(chalk.gray('  Are We Anti-Cheat Yet: https://areweanticheatyet.com/'));
}

program
  .argument('<game>', 'Game name or ID')
  .description('Check anti-cheat compatibility for a game')
  .parse();

const [gameName] = program.args;

checkAntiCheat(gameName).catch(err => {
  console.error(chalk.red('Error:'), err.message);
  process.exit(1);
});
