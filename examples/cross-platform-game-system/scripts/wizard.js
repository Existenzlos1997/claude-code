#!/usr/bin/env node
/**
 * Configuration Wizard
 * Interaktiver Einrichtungsassistent
 * Interactive setup wizard
 */

const { program } = require('commander');
const chalk = require('chalk');
const fs = require('fs').promises;
const path = require('path');
const { execa } = require('execa');

async function runWizard() {
  console.log(chalk.blue('🧙 Cross-Platform Game System Configuration Wizard\n'));
  console.log(chalk.blue('='.repeat(60)));

  const config = {
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
    performance: {
      monitoring: false,
      reportGeneration: false
    },
    logging: {
      level: 'info',
      enableFile: false
    }
  };

  // Step 1: Detect System
  console.log(chalk.white('\n📋 Step 1: System Detection\n'));
  
  console.log(chalk.gray(`OS: ${process.platform}`));
  console.log(chalk.gray(`Architecture: ${process.arch}`));
  console.log(chalk.gray(`Node.js: ${process.version}`));

  // Check Wine
  try {
    const { stdout } = await execa('wine', ['--version']);
    console.log(chalk.green(`✓ Wine: ${stdout.trim()}`));
    config.wine.detected = true;
  } catch (err) {
    console.log(chalk.yellow('⚠ Wine not detected'));
    config.wine.detected = false;
  }

  // Check Proton
  const homeDir = process.env.HOME || process.env.USERPROFILE;
  const protonPath = path.join(homeDir, '.steam/steam/steamapps/common');
  try {
    const entries = await fs.readdir(protonPath);
    const protonDirs = entries.filter(e => e.startsWith('Proton'));
    if (protonDirs.length > 0) {
      console.log(chalk.green(`✓ Proton: Found ${protonDirs.length} version(s)`));
      config.proton.detected = true;
      config.proton.availableVersions = protonDirs;
    }
  } catch (err) {
    console.log(chalk.yellow('⚠ Proton not detected (Steam not installed?)'));
    config.proton.detected = false;
  }

  // Step 2: Recommendations
  console.log(chalk.white('\n📋 Step 2: Recommendations\n'));

  if (!config.wine.detected && !config.proton.detected) {
    console.log(chalk.red('⚠️  Neither Wine nor Proton detected!'));
    console.log(chalk.yellow('\nRecommendations:'));
    console.log(chalk.gray('  1. Install Wine: sudo apt install wine-stable'));
    console.log(chalk.gray('  2. Install Steam for Proton support'));
    console.log(chalk.gray('  3. Run this wizard again after installation'));
  } else if (config.proton.detected) {
    console.log(chalk.green('✓ Recommended: Use Proton (best compatibility)'));
    config.defaultCompatibilityLayer = 'proton';
  } else {
    console.log(chalk.yellow('⚠ Using Wine (Proton recommended for games)'));
    config.defaultCompatibilityLayer = 'wine';
  }

  // Step 3: Performance Settings
  console.log(chalk.white('\n📋 Step 3: Performance Optimization\n'));

  console.log(chalk.gray('Enable performance monitoring? (Recommended for troubleshooting)'));
  config.performance.monitoring = true;
  console.log(chalk.green('✓ Enabled'));

  console.log(chalk.gray('Enable DXVK (DirectX to Vulkan)? (Recommended)'));
  config.wine.dxvk = true;
  console.log(chalk.green('✓ Enabled'));

  console.log(chalk.gray('Enable VKD3D (DirectX 12 to Vulkan)? (Recommended)'));
  config.wine.vkd3d = true;
  console.log(chalk.green('✓ Enabled'));

  // Step 4: Logging
  console.log(chalk.white('\n📋 Step 4: Logging Configuration\n'));

  console.log(chalk.gray('Log Level: info (change to debug for troubleshooting)'));
  console.log(chalk.gray('File Logging: Disabled (set ENABLE_FILE_LOGGING=true to enable)'));

  // Step 5: Save Configuration
  console.log(chalk.white('\n📋 Step 5: Saving Configuration\n'));

  const configPath = path.join(__dirname, '..', 'config', 'settings.json');
  const configDir = path.dirname(configPath);
  await fs.mkdir(configDir, { recursive: true });
  await fs.writeFile(configPath, JSON.stringify(config, null, 2));
  console.log(chalk.green(`✓ Configuration saved: ${configPath}`));

  // Step 6: Next Steps
  console.log(chalk.blue('\n' + '='.repeat(60)));
  console.log(chalk.blue('\n✨ Configuration Complete!\n'));

  console.log(chalk.white('Next Steps:'));
  console.log(chalk.gray('  1. Detect games: npm run detect-games'));
  console.log(chalk.gray('  2. Add a game: npm run add-game'));
  console.log(chalk.gray('  3. Launch a game: npm start launch "Game Name"'));
  console.log(chalk.gray('  4. Check system: npm run validate'));

  console.log(chalk.white('\nDocumentation:'));
  console.log(chalk.gray('  - Quick Start: docs/QUICKSTART.md'));
  console.log(chalk.gray('  - Anti-Cheat: docs/ANTICHEAT.md'));
  console.log(chalk.gray('  - Troubleshooting: docs/TROUBLESHOOTING.md'));

  console.log();
}

program
  .description('Interactive configuration wizard')
  .action(runWizard);

program.parse();

if (require.main === module && program.args.length === 0) {
  runWizard().catch(err => {
    console.error(chalk.red('Wizard failed:'), err.message);
    process.exit(1);
  });
}
