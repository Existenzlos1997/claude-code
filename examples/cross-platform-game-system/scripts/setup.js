/**
 * Setup Script
 * Initialisiert das System und überprüft Abhängigkeiten
 */

const { execa } = require('execa');
const chalk = require('chalk');
const fs = require('fs').promises;
const path = require('path');

async function checkDependency(command, name) {
  try {
    await execa(command, ['--version']);
    console.log(chalk.green(`✓ ${name} is installed`));
    return true;
  } catch (err) {
    console.log(chalk.yellow(`✗ ${name} is not installed`));
    return false;
  }
}

async function setup() {
  console.log(chalk.blue('🎮 Setting up Cross-Platform Game System\n'));

  // Check dependencies
  console.log(chalk.white('Checking dependencies:\n'));
  
  await checkDependency('node', 'Node.js');
  const hasWine = await checkDependency('wine', 'Wine');
  const hasWinetricks = await checkDependency('winetricks', 'Winetricks');

  console.log();

  if (!hasWine) {
    console.log(chalk.yellow('Wine is recommended for running Windows games.'));
    console.log(chalk.gray('Install with: sudo apt install wine-stable (Ubuntu/Debian)'));
    console.log(chalk.gray('             brew install wine-stable (macOS)'));
  }

  if (!hasWinetricks) {
    console.log(chalk.yellow('Winetricks is recommended for installing game dependencies.'));
    console.log(chalk.gray('Install with: sudo apt install winetricks (Ubuntu/Debian)'));
  }

  // Create config directory
  const configDir = path.join(process.env.HOME || process.env.USERPROFILE, '.game-system');
  await fs.mkdir(configDir, { recursive: true });
  console.log(chalk.green(`\n✓ Created config directory: ${configDir}`));

  console.log(chalk.green('\n✓ Setup complete!'));
  console.log(chalk.gray('\nRun "npm start list" to see available games.'));
}

if (require.main === module) {
  setup().catch(err => {
    console.error(chalk.red('Setup failed:'), err.message);
    process.exit(1);
  });
}
