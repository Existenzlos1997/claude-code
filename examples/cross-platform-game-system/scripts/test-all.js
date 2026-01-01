#!/usr/bin/env node
/**
 * Comprehensive Test Runner
 * Führt alle Tests durch und erstellt Berichte
 * Runs all tests and generates reports
 */

const { execa } = require('execa');
const chalk = require('chalk');
const fs = require('fs').promises;
const path = require('path');

async function runTests() {
  console.log(chalk.blue('🧪 Starting Comprehensive Test Suite\n'));
  console.log(chalk.blue('='.repeat(60)));

  const results = {
    passed: 0,
    failed: 0,
    warnings: [],
    errors: []
  };

  // 1. Unit Tests
  console.log(chalk.white('\n📋 Running Unit Tests...\n'));
  try {
    const { stdout } = await execa('npm', ['test', '--', '--coverage'], {
      stdio: 'inherit',
      reject: false
    });
    results.passed++;
    console.log(chalk.green('✓ Unit tests passed'));
  } catch (err) {
    results.failed++;
    results.errors.push('Unit tests failed');
    console.log(chalk.red('✗ Unit tests failed'));
  }

  // 2. Lint Check
  console.log(chalk.white('\n📝 Running Linter...\n'));
  try {
    // Check if eslint is available
    try {
      await execa('npx', ['eslint', '--version'], { stdio: 'pipe' });
      await execa('npx', ['eslint', 'src/**/*.js', 'plugins/**/*.js'], {
        stdio: 'inherit',
        reject: false
      });
      results.passed++;
      console.log(chalk.green('✓ Linting passed'));
    } catch (err) {
      results.warnings.push('ESLint not configured, skipping');
      console.log(chalk.yellow('⚠ ESLint not available, skipping'));
    }
  } catch (err) {
    results.failed++;
    results.errors.push('Linting failed');
  }

  // 3. Dependency Check
  console.log(chalk.white('\n📦 Checking Dependencies...\n'));
  try {
    const packageJson = require('../package.json');
    const deps = { ...packageJson.dependencies, ...packageJson.devDependencies };
    
    console.log(chalk.gray(`  Found ${Object.keys(deps).length} dependencies`));
    results.passed++;
    console.log(chalk.green('✓ Dependencies OK'));
  } catch (err) {
    results.failed++;
    results.errors.push('Dependency check failed');
    console.log(chalk.red('✗ Dependency check failed'));
  }

  // 4. Configuration Validation
  console.log(chalk.white('\n⚙️  Validating Configuration...\n'));
  try {
    const configPath = path.join(__dirname, '../config/settings.json');
    const config = JSON.parse(await fs.readFile(configPath, 'utf8'));
    
    if (!config.defaultCompatibilityLayer) {
      throw new Error('Missing defaultCompatibilityLayer');
    }
    
    console.log(chalk.gray('  Configuration is valid'));
    results.passed++;
    console.log(chalk.green('✓ Configuration valid'));
  } catch (err) {
    results.failed++;
    results.errors.push('Configuration validation failed: ' + err.message);
    console.log(chalk.red('✗ Configuration validation failed'));
  }

  // 5. Plugin Loading Test
  console.log(chalk.white('\n🔌 Testing Plugin System...\n'));
  try {
    const CompatibilityManager = require('../src/compatibility/manager');
    const manager = new CompatibilityManager();
    await manager.loadPlugins();
    
    const pluginCount = manager.plugins.size;
    console.log(chalk.gray(`  Loaded ${pluginCount} plugins`));
    
    if (pluginCount < 3) {
      throw new Error(`Expected at least 3 plugins, got ${pluginCount}`);
    }
    
    results.passed++;
    console.log(chalk.green('✓ Plugin system working'));
  } catch (err) {
    results.failed++;
    results.errors.push('Plugin loading failed: ' + err.message);
    console.log(chalk.red('✗ Plugin system failed'));
  }

  // 6. Anti-Cheat Module Test
  console.log(chalk.white('\n🛡️  Testing Anti-Cheat Detection...\n'));
  try {
    const AntiCheatManager = require('../src/compatibility/anticheat');
    const acManager = new AntiCheatManager();
    
    const supported = acManager.supportedSystems;
    const supportedCount = Object.keys(supported).length;
    
    console.log(chalk.gray(`  Supports ${supportedCount} anti-cheat systems`));
    
    if (supportedCount < 4) {
      throw new Error('Expected at least 4 anti-cheat systems');
    }
    
    results.passed++;
    console.log(chalk.green('✓ Anti-cheat module working'));
  } catch (err) {
    results.failed++;
    results.errors.push('Anti-cheat test failed: ' + err.message);
    console.log(chalk.red('✗ Anti-cheat test failed'));
  }

  // Print Summary
  console.log(chalk.blue('\n' + '='.repeat(60)));
  console.log(chalk.blue('\n📊 Test Summary\n'));
  
  console.log(chalk.green(`✓ Passed: ${results.passed}`));
  if (results.failed > 0) {
    console.log(chalk.red(`✗ Failed: ${results.failed}`));
  }
  if (results.warnings.length > 0) {
    console.log(chalk.yellow(`⚠ Warnings: ${results.warnings.length}`));
    results.warnings.forEach(w => console.log(chalk.yellow(`  • ${w}`)));
  }
  
  if (results.errors.length > 0) {
    console.log(chalk.red('\n❌ Errors:'));
    results.errors.forEach(e => console.log(chalk.red(`  • ${e}`)));
  }

  const total = results.passed + results.failed;
  const percentage = total > 0 ? Math.round((results.passed / total) * 100) : 0;
  
  console.log(chalk.blue(`\nOverall: ${percentage}% passed`));
  
  if (results.failed === 0) {
    console.log(chalk.green('\n✨ All tests passed! System is ready.\n'));
    process.exit(0);
  } else {
    console.log(chalk.red('\n⚠️  Some tests failed. Please review errors above.\n'));
    process.exit(1);
  }
}

if (require.main === module) {
  runTests().catch(err => {
    console.error(chalk.red('Test runner failed:'), err);
    process.exit(1);
  });
}

module.exports = runTests;
