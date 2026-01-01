#!/usr/bin/env node
/**
 * End-to-End Simulation Test
 * Vollständiger simulierter Systemtest
 * Complete simulated system test
 */

const chalk = require('chalk');
const fs = require('fs').promises;
const path = require('path');
const os = require('os');

// Import system modules
const GameLibrary = require('../src/core/game-library');
const CompatibilityManager = require('../src/compatibility/manager');
const AntiCheatManager = require('../src/compatibility/anticheat');
const Validator = require('../src/core/validator');
const GameDetector = require('../src/detection/detector');
const Logger = require('../src/core/logger');
const PerformanceMonitor = require('../src/core/performance-monitor');
const { getProfile, listProfiles, searchProfiles } = require('../src/core/game-profiles');

class SimulationTest {
  constructor() {
    this.results = {
      passed: 0,
      failed: 0,
      skipped: 0,
      tests: []
    };
    this.testDir = null;
    this.logger = new Logger({ logLevel: 'error', enableConsole: false });
  }

  async setup() {
    // Create temporary test directory
    this.testDir = await fs.mkdtemp(path.join(os.tmpdir(), 'game-system-simulation-'));
    console.log(chalk.gray(`Test directory: ${this.testDir}`));
  }

  async cleanup() {
    if (this.testDir) {
      try {
        await fs.rm(this.testDir, { recursive: true, force: true });
      } catch (err) {
        // Ignore cleanup errors
      }
    }
  }

  async runTest(name, testFn) {
    process.stdout.write(chalk.gray(`  ${name}... `));
    
    try {
      await testFn();
      console.log(chalk.green('✓ PASS'));
      this.results.passed++;
      this.results.tests.push({ name, status: 'passed' });
      return true;
    } catch (err) {
      console.log(chalk.red('✗ FAIL'));
      console.log(chalk.red(`    Error: ${err.message}`));
      this.results.failed++;
      this.results.tests.push({ name, status: 'failed', error: err.message });
      return false;
    }
  }

  async testGameLibrary() {
    console.log(chalk.blue('\n📚 Testing Game Library\n'));

    await this.runTest('Create library instance', async () => {
      const library = new GameLibrary();
      library.libraryPath = path.join(this.testDir, 'library.json');
      if (!library) throw new Error('Failed to create library');
    });

    await this.runTest('Add game to library', async () => {
      const library = new GameLibrary();
      library.libraryPath = path.join(this.testDir, 'library.json');
      
      await library.addGame({
        name: 'Test Game',
        path: '/test/game',
        platform: 'windows',
        executable: 'game.exe'
      });

      if (library.games.length !== 1) throw new Error('Game not added');
    });

    await this.runTest('Save and load library', async () => {
      const library1 = new GameLibrary();
      library1.libraryPath = path.join(this.testDir, 'library-persist.json');
      
      await library1.addGame({
        name: 'Persistent Game',
        path: '/test',
        platform: 'windows',
        executable: 'test.exe'
      });
      await library1.save();

      const library2 = new GameLibrary();
      library2.libraryPath = path.join(this.testDir, 'library-persist.json');
      await library2.load();

      if (library2.games.length !== 1) throw new Error('Library not persisted');
      if (library2.games[0].name !== 'Persistent Game') throw new Error('Game data corrupted');
    });

    await this.runTest('Find game by name', async () => {
      const library = new GameLibrary();
      library.libraryPath = path.join(this.testDir, 'library-find.json');
      
      await library.addGame({
        name: 'Findable Game',
        path: '/test',
        platform: 'windows',
        executable: 'game.exe'
      });

      const game = library.findGame('findable game');
      if (!game) throw new Error('Game not found');
      if (game.name !== 'Findable Game') throw new Error('Wrong game found');
    });

    await this.runTest('Remove game from library', async () => {
      const library = new GameLibrary();
      library.libraryPath = path.join(this.testDir, 'library-remove.json');
      
      await library.addGame({
        name: 'Removable Game',
        path: '/test',
        platform: 'windows',
        executable: 'game.exe'
      });

      const removed = library.removeGame('removable game');
      if (!removed) throw new Error('Game not removed');
      if (library.games.length !== 0) throw new Error('Library not empty');
    });
  }

  async testCompatibilityManager() {
    console.log(chalk.blue('\n🔌 Testing Compatibility Manager\n'));

    await this.runTest('Load plugins', async () => {
      const manager = new CompatibilityManager();
      await manager.loadPlugins();
      
      if (manager.plugins.size === 0) throw new Error('No plugins loaded');
    });

    await this.runTest('Get Wine plugin', async () => {
      const manager = new CompatibilityManager();
      await manager.loadPlugins();
      
      const wine = manager.getPlugin('wine');
      if (!wine) throw new Error('Wine plugin not found');
      if (wine.name !== 'wine') throw new Error('Wrong plugin loaded');
    });

    await this.runTest('Get Proton plugin', async () => {
      const manager = new CompatibilityManager();
      await manager.loadPlugins();
      
      const proton = manager.getPlugin('proton');
      if (!proton) throw new Error('Proton plugin not found');
      if (proton.name !== 'proton') throw new Error('Wrong plugin loaded');
    });

    await this.runTest('Get Native plugin', async () => {
      const manager = new CompatibilityManager();
      await manager.loadPlugins();
      
      const native = manager.getPlugin('native');
      if (!native) throw new Error('Native plugin not found');
      if (native.name !== 'native') throw new Error('Wrong plugin loaded');
    });
  }

  async testAntiCheat() {
    console.log(chalk.blue('\n🛡️  Testing Anti-Cheat Detection\n'));

    await this.runTest('Create AntiCheatManager', async () => {
      const acManager = new AntiCheatManager();
      if (!acManager) throw new Error('Failed to create manager');
    });

    await this.runTest('Detect EasyAntiCheat', async () => {
      const acManager = new AntiCheatManager();
      const testDir = await fs.mkdtemp(path.join(this.testDir, 'eac-'));
      
      await fs.writeFile(path.join(testDir, 'EasyAntiCheat.exe'), '');
      const detected = await acManager.detectAntiCheat(testDir);
      
      if (detected.length === 0) throw new Error('EAC not detected');
      if (detected[0].type !== 'eac') throw new Error('Wrong anti-cheat detected');
    });

    await this.runTest('Detect BattlEye', async () => {
      const acManager = new AntiCheatManager();
      const testDir = await fs.mkdtemp(path.join(this.testDir, 'be-'));
      
      await fs.writeFile(path.join(testDir, 'BEClient.dll'), '');
      const detected = await acManager.detectAntiCheat(testDir);
      
      if (detected.length === 0) throw new Error('BattlEye not detected');
      if (detected[0].type !== 'battleye') throw new Error('Wrong anti-cheat detected');
    });

    await this.runTest('Check EAC compatibility', async () => {
      const acManager = new AntiCheatManager();
      const isCompatible = acManager.isCompatible('eac');
      
      if (!isCompatible) throw new Error('EAC should be compatible');
    });

    await this.runTest('Check Vanguard incompatibility', async () => {
      const acManager = new AntiCheatManager();
      const isCompatible = acManager.isCompatible('vanguard');
      
      if (isCompatible) throw new Error('Vanguard should not be compatible');
    });

    await this.runTest('Get Proton recommendation', async () => {
      const acManager = new AntiCheatManager();
      const proton = acManager.getRecommendedProton('eac');
      
      if (!proton) throw new Error('No Proton recommendation');
      if (proton !== 'proton-experimental') throw new Error('Wrong Proton version');
    });

    await this.runTest('Get environment variables', async () => {
      const acManager = new AntiCheatManager();
      const env = acManager.getAntiCheatEnvironment('eac', {});
      
      if (!env.PROTON_USE_EAC) throw new Error('EAC env not set');
      if (!env.WINE_HIDE_WINE) throw new Error('Wine hiding not set');
    });
  }

  async testValidator() {
    console.log(chalk.blue('\n✅ Testing Input Validation\n'));

    await this.runTest('Validate correct game', async () => {
      const errors = Validator.validateGame({
        name: 'Valid Game',
        path: '/valid/path',
        platform: 'windows',
        executable: 'game.exe'
      });
      
      if (errors.length !== 0) throw new Error(`Validation failed: ${errors.join(', ')}`);
    });

    await this.runTest('Reject invalid game (no name)', async () => {
      const errors = Validator.validateGame({
        path: '/path',
        platform: 'windows',
        executable: 'game.exe'
      });
      
      if (errors.length === 0) throw new Error('Should reject game without name');
    });

    await this.runTest('Reject invalid platform', async () => {
      const errors = Validator.validateGame({
        name: 'Test',
        path: '/path',
        platform: 'invalid',
        executable: 'game.exe'
      });
      
      if (errors.length === 0) throw new Error('Should reject invalid platform');
    });

    await this.runTest('Validate compatibility layer', async () => {
      const errors = Validator.validateCompatibilityLayer('wine');
      if (errors.length !== 0) throw new Error('Wine should be valid');
    });

    await this.runTest('Reject invalid compatibility layer', async () => {
      const errors = Validator.validateCompatibilityLayer('invalid');
      if (errors.length === 0) throw new Error('Should reject invalid layer');
    });

    await this.runTest('Sanitize game name', async () => {
      const sanitized = Validator.sanitizeGameName('Game: <Name> "Test"');
      if (sanitized.includes(':') || sanitized.includes('<') || sanitized.includes('>')) {
        throw new Error('Special characters not sanitized');
      }
    });
  }

  async testGameProfiles() {
    console.log(chalk.blue('\n🎮 Testing Game Profiles\n'));

    await this.runTest('List all profiles', async () => {
      const profiles = listProfiles();
      if (profiles.length === 0) throw new Error('No profiles found');
    });

    await this.runTest('Get Apex Legends profile', async () => {
      const profile = getProfile('apex-legends');
      if (!profile) throw new Error('Apex Legends profile not found');
      if (profile.name !== 'Apex Legends') throw new Error('Wrong profile');
      if (profile.antiCheat !== 'eac') throw new Error('Wrong anti-cheat');
    });

    await this.runTest('Get Witcher 3 profile', async () => {
      const profile = getProfile('witcher-3');
      if (!profile) throw new Error('Witcher 3 profile not found');
      if (profile.compatibilityLayer !== 'proton') throw new Error('Wrong compatibility layer');
    });

    await this.runTest('Search profiles', async () => {
      const results = searchProfiles('apex');
      if (results.length === 0) throw new Error('No search results');
      if (!results.some(r => r.name.includes('Apex'))) throw new Error('Apex not in results');
    });

    await this.runTest('Check incompatible game', async () => {
      const profile = getProfile('valorant');
      if (!profile) throw new Error('Valorant profile not found');
      if (profile.compatible !== false) throw new Error('Valorant should be incompatible');
    });
  }

  async testPerformanceMonitor() {
    console.log(chalk.blue('\n📊 Testing Performance Monitor\n'));

    await this.runTest('Create performance monitor', async () => {
      const monitor = new PerformanceMonitor();
      if (!monitor) throw new Error('Failed to create monitor');
    });

    await this.runTest('Generate empty report', async () => {
      const monitor = new PerformanceMonitor();
      const report = await monitor.generateReport();
      if (report !== null) throw new Error('Should return null for empty metrics');
    });

    await this.runTest('Add metrics manually', async () => {
      const monitor = new PerformanceMonitor();
      monitor.gameName = 'Test Game';
      monitor.startTime = Date.now();
      monitor.metrics.push({
        timestamp: Date.now(),
        cpu: 50,
        memory: 30,
        threads: 10
      });

      const report = await monitor.generateReport();
      if (!report) throw new Error('Report should be generated');
      if (report.cpu.average !== 50) throw new Error('Wrong CPU average');
    });
  }

  async testLogger() {
    console.log(chalk.blue('\n📝 Testing Logger\n'));

    await this.runTest('Create logger', async () => {
      const logger = new Logger({ enableConsole: false });
      if (!logger) throw new Error('Failed to create logger');
    });

    await this.runTest('Log levels', async () => {
      const logger = new Logger({ enableConsole: false, logLevel: 'debug' });
      
      if (!logger.shouldLog('error')) throw new Error('Should log errors');
      if (!logger.shouldLog('debug')) throw new Error('Should log debug');
      if (logger.shouldLog('trace')) throw new Error('Should not log trace');
    });

    await this.runTest('Format message', async () => {
      const logger = new Logger({ enableConsole: false });
      const formatted = logger.formatMessage('info', 'Test message', { key: 'value' });
      
      if (!formatted.includes('INFO')) throw new Error('Level not in message');
      if (!formatted.includes('Test message')) throw new Error('Message not in formatted');
    });
  }

  async testIntegration() {
    console.log(chalk.blue('\n🔗 Testing Integration\n'));

    await this.runTest('Complete workflow: Add, find, remove', async () => {
      const library = new GameLibrary();
      library.libraryPath = path.join(this.testDir, 'workflow.json');

      // Add
      await library.addGame({
        name: 'Workflow Game',
        path: '/workflow',
        platform: 'windows',
        executable: 'game.exe'
      });

      // Find
      const game = library.findGame('workflow game');
      if (!game) throw new Error('Game not found after adding');

      // Remove
      const removed = library.removeGame('workflow game');
      if (!removed) throw new Error('Game not removed');
      if (library.games.length !== 0) throw new Error('Library not empty');
    });

    await this.runTest('Compatibility manager with game', async () => {
      const manager = new CompatibilityManager();
      await manager.loadPlugins();

      const game = {
        name: 'Test Game',
        path: '/test',
        platform: 'native',
        executable: 'game',
        compatibilityLayer: 'native'
      };

      const plugin = manager.getPlugin(game.compatibilityLayer);
      if (!plugin) throw new Error('Plugin not found for game');
    });

    await this.runTest('Anti-cheat detection + compatibility check', async () => {
      const acManager = new AntiCheatManager();
      const testDir = await fs.mkdtemp(path.join(this.testDir, 'ac-int-'));
      
      await fs.writeFile(path.join(testDir, 'EasyAntiCheat.exe'), '');
      const detected = await acManager.detectAntiCheat(testDir);
      
      if (detected.length === 0) throw new Error('No anti-cheat detected');
      
      const compatible = acManager.isCompatible(detected[0].type);
      if (!compatible) throw new Error('EAC should be compatible');
    });
  }

  async runAll() {
    console.log(chalk.blue('\n🧪 Cross-Platform Game System - Simulation Test\n'));
    console.log(chalk.blue('='.repeat(60)));
    console.log(chalk.gray('Testing all system components without requiring actual games\n'));

    await this.setup();

    try {
      await this.testGameLibrary();
      await this.testCompatibilityManager();
      await this.testAntiCheat();
      await this.testValidator();
      await this.testGameProfiles();
      await this.testPerformanceMonitor();
      await this.testLogger();
      await this.testIntegration();
    } finally {
      await this.cleanup();
    }

    // Print summary
    console.log(chalk.blue('\n' + '='.repeat(60)));
    console.log(chalk.blue('\n📊 Test Results\n'));

    const total = this.results.passed + this.results.failed + this.results.skipped;
    const passRate = total > 0 ? Math.round((this.results.passed / total) * 100) : 0;

    console.log(chalk.green(`✓ Passed: ${this.results.passed}`));
    if (this.results.failed > 0) {
      console.log(chalk.red(`✗ Failed: ${this.results.failed}`));
    }
    if (this.results.skipped > 0) {
      console.log(chalk.yellow(`⊘ Skipped: ${this.results.skipped}`));
    }

    console.log(chalk.blue(`\nTotal: ${total} tests`));
    console.log(chalk.blue(`Pass Rate: ${passRate}%`));

    if (this.results.failed > 0) {
      console.log(chalk.red('\n❌ Some tests failed. See details above.\n'));
      process.exit(1);
    } else {
      console.log(chalk.green('\n✨ All simulated tests passed! System is working correctly.\n'));
      process.exit(0);
    }
  }
}

if (require.main === module) {
  const simulation = new SimulationTest();
  simulation.runAll().catch(err => {
    console.error(chalk.red('Simulation test crashed:'), err);
    process.exit(1);
  });
}

module.exports = SimulationTest;
