#!/usr/bin/env node
/**
 * System Validation Script
 * Überprüft die System-Konfiguration und Abhängigkeiten
 * Validates system configuration and dependencies
 */

const { execa } = require('execa');
const chalk = require('chalk');
const fs = require('fs').promises;
const path = require('path');
const Validator = require('../src/core/validator');

async function validateSystem() {
  console.log(chalk.blue('🔍 System Validation\n'));
  console.log(chalk.blue('='.repeat(60)));

  const issues = {
    errors: [],
    warnings: [],
    info: []
  };

  // 1. Environment Validation
  console.log(chalk.white('\n🌍 Checking Environment...\n'));
  const envCheck = Validator.validateEnvironment();
  
  if (envCheck.errors.length > 0) {
    issues.errors.push(...envCheck.errors);
    envCheck.errors.forEach(e => console.log(chalk.red(`  ✗ ${e}`)));
  } else {
    console.log(chalk.green('  ✓ Environment OK'));
  }
  
  if (envCheck.warnings.length > 0) {
    issues.warnings.push(...envCheck.warnings);
    envCheck.warnings.forEach(w => console.log(chalk.yellow(`  ⚠ ${w}`)));
  }

  // 2. Platform Check
  console.log(chalk.white('\n💻 Platform Information...\n'));
  console.log(chalk.gray(`  OS: ${process.platform}`));
  console.log(chalk.gray(`  Arch: ${process.arch}`));
  console.log(chalk.gray(`  Node: ${process.version}`));

  // 3. Check for Wine
  console.log(chalk.white('\n🍷 Checking Wine...\n'));
  try {
    const { stdout } = await execa('wine', ['--version']);
    console.log(chalk.green(`  ✓ Wine installed: ${stdout.trim()}`));
    issues.info.push('Wine is available');
  } catch (err) {
    console.log(chalk.yellow('  ⚠ Wine not found'));
    issues.warnings.push('Wine is not installed - required for Windows games');
  }

  // 4. Check for Winetricks
  console.log(chalk.white('\n🔧 Checking Winetricks...\n'));
  try {
    await execa('winetricks', ['--version']);
    console.log(chalk.green('  ✓ Winetricks installed'));
    issues.info.push('Winetricks is available');
  } catch (err) {
    console.log(chalk.yellow('  ⚠ Winetricks not found'));
    issues.warnings.push('Winetricks recommended for installing game dependencies');
  }

  // 5. Check for Proton
  console.log(chalk.white('\n⚡ Checking Proton...\n'));
  const homeDir = process.env.HOME || process.env.USERPROFILE;
  if (homeDir) {
    const protonPaths = [
      path.join(homeDir, '.steam/steam/steamapps/common/Proton 8.0'),
      path.join(homeDir, '.steam/steam/steamapps/common/Proton - Experimental'),
      path.join(homeDir, '.steam/steam/steamapps/common/Proton 7.0')
    ];

    let protonFound = false;
    for (const protonPath of protonPaths) {
      try {
        await fs.access(path.join(protonPath, 'proton'));
        console.log(chalk.green(`  ✓ Found: ${path.basename(protonPath)}`));
        protonFound = true;
      } catch (err) {
        // Continue checking
      }
    }

    if (!protonFound) {
      console.log(chalk.yellow('  ⚠ Proton not found'));
      issues.warnings.push('Proton not found - install Steam for better game compatibility');
    }
  }

  // 6. Check for Steam
  console.log(chalk.white('\n🎮 Checking Steam...\n'));
  if (homeDir) {
    const steamPath = path.join(homeDir, '.steam/steam');
    try {
      await fs.access(steamPath);
      console.log(chalk.green('  ✓ Steam directory found'));
      issues.info.push('Steam is installed');
    } catch (err) {
      console.log(chalk.yellow('  ⚠ Steam directory not found'));
      issues.warnings.push('Steam recommended for automatic game detection');
    }
  }

  // 7. Check Project Structure
  console.log(chalk.white('\n📁 Validating Project Structure...\n'));
  const requiredDirs = ['src', 'plugins', 'config', 'tests', 'docs'];
  
  for (const dir of requiredDirs) {
    const dirPath = path.join(__dirname, '..', dir);
    try {
      const stats = await fs.stat(dirPath);
      if (stats.isDirectory()) {
        console.log(chalk.green(`  ✓ ${dir}/`));
      }
    } catch (err) {
      console.log(chalk.red(`  ✗ ${dir}/ missing`));
      issues.errors.push(`Required directory missing: ${dir}`);
    }
  }

  // 8. Check Required Files
  console.log(chalk.white('\n📄 Checking Required Files...\n'));
  const requiredFiles = [
    'package.json',
    'README.md',
    'src/index.js',
    'src/core/game-library.js',
    'src/compatibility/manager.js'
  ];

  for (const file of requiredFiles) {
    const filePath = path.join(__dirname, '..', file);
    try {
      await fs.access(filePath);
      console.log(chalk.green(`  ✓ ${file}`));
    } catch (err) {
      console.log(chalk.red(`  ✗ ${file} missing`));
      issues.errors.push(`Required file missing: ${file}`);
    }
  }

  // 9. Check Plugins
  console.log(chalk.white('\n🔌 Validating Plugins...\n'));
  const requiredPlugins = ['wine', 'proton', 'native'];
  
  for (const plugin of requiredPlugins) {
    const pluginPath = path.join(__dirname, '..', 'plugins', plugin, 'index.js');
    try {
      await fs.access(pluginPath);
      console.log(chalk.green(`  ✓ ${plugin} plugin`));
    } catch (err) {
      console.log(chalk.red(`  ✗ ${plugin} plugin missing`));
      issues.errors.push(`Required plugin missing: ${plugin}`);
    }
  }

  // 10. Check Configuration
  console.log(chalk.white('\n⚙️  Validating Configuration...\n'));
  try {
    const configPath = path.join(__dirname, '..', 'config', 'settings.json');
    const config = JSON.parse(await fs.readFile(configPath, 'utf8'));
    
    const requiredKeys = ['defaultCompatibilityLayer', 'platforms'];
    const missingKeys = requiredKeys.filter(key => !config[key]);
    
    if (missingKeys.length > 0) {
      issues.errors.push(`Missing config keys: ${missingKeys.join(', ')}`);
      console.log(chalk.red(`  ✗ Missing keys: ${missingKeys.join(', ')}`));
    } else {
      console.log(chalk.green('  ✓ Configuration valid'));
    }
  } catch (err) {
    issues.errors.push('Configuration file invalid: ' + err.message);
    console.log(chalk.red('  ✗ Configuration invalid'));
  }

  // Print Summary
  console.log(chalk.blue('\n' + '='.repeat(60)));
  console.log(chalk.blue('\n📊 Validation Summary\n'));

  if (issues.errors.length > 0) {
    console.log(chalk.red(`❌ Errors: ${issues.errors.length}`));
    issues.errors.forEach(e => console.log(chalk.red(`  • ${e}`)));
  }

  if (issues.warnings.length > 0) {
    console.log(chalk.yellow(`\n⚠️  Warnings: ${issues.warnings.length}`));
    issues.warnings.forEach(w => console.log(chalk.yellow(`  • ${w}`)));
  }

  if (issues.info.length > 0) {
    console.log(chalk.blue(`\nℹ️  Info: ${issues.info.length}`));
    issues.info.forEach(i => console.log(chalk.gray(`  • ${i}`)));
  }

  console.log();

  if (issues.errors.length === 0) {
    console.log(chalk.green('✨ System validation passed!\n'));
    console.log(chalk.gray('You can start using the system with: npm start\n'));
    process.exit(0);
  } else {
    console.log(chalk.red('⚠️  System validation failed. Please fix errors above.\n'));
    process.exit(1);
  }
}

if (require.main === module) {
  validateSystem().catch(err => {
    console.error(chalk.red('Validation failed:'), err);
    process.exit(1);
  });
}

module.exports = validateSystem;
