/**
 * Integration Tests
 * End-to-end tests for the game system
 */

const GameSystem = require('../src/index');
const GameLibrary = require('../src/core/game-library');
const fs = require('fs').promises;
const path = require('path');
const os = require('os');

describe('Integration Tests', () => {
  let testDir;
  let testLibraryPath;

  beforeEach(async () => {
    testDir = await fs.mkdtemp(path.join(os.tmpdir(), 'game-system-integration-'));
    testLibraryPath = path.join(testDir, 'library.json');
  });

  afterEach(async () => {
    try {
      await fs.rm(testDir, { recursive: true, force: true });
    } catch (err) {
      // Ignore cleanup errors
    }
  });

  describe('GameSystem Integration', () => {
    test('should initialize system successfully', async () => {
      const system = new GameSystem();
      
      await expect(system.initialize()).resolves.not.toThrow();
      expect(system.library).toBeDefined();
      expect(system.compatibilityManager).toBeDefined();
    });

    test('should list games from library', async () => {
      const library = new GameLibrary();
      library.libraryPath = testLibraryPath;
      library.addGame({
        name: 'Test Game',
        path: '/test',
        platform: 'windows',
        executable: 'test.exe'
      });
      await library.save();

      const system = new GameSystem();
      system.library.libraryPath = testLibraryPath;
      await system.initialize();

      expect(system.library.games).toHaveLength(1);
      expect(system.library.games[0].name).toBe('Test Game');
    });
  });

  describe('Complete Workflow', () => {
    test('should add, find, and remove a game', async () => {
      const library = new GameLibrary();
      library.libraryPath = testLibraryPath;

      // Add game
      library.addGame({
        name: 'Workflow Test Game',
        path: '/workflow/test',
        platform: 'windows',
        executable: 'game.exe'
      });

      expect(library.games).toHaveLength(1);

      // Save library
      await library.save();

      // Load in new instance
      const library2 = new GameLibrary();
      library2.libraryPath = testLibraryPath;
      await library2.load();

      expect(library2.games).toHaveLength(1);

      // Find game
      const game = library2.findGame('workflow test game');
      expect(game).toBeDefined();
      expect(game.name).toBe('Workflow Test Game');

      // Remove game
      const removed = library2.removeGame('workflow test game');
      expect(removed).toBe(true);
      expect(library2.games).toHaveLength(0);
    });
  });

  describe('Plugin System', () => {
    test('should load all plugins successfully', async () => {
      const system = new GameSystem();
      await system.initialize();

      const wine = system.compatibilityManager.getPlugin('wine');
      const proton = system.compatibilityManager.getPlugin('proton');
      const native = system.compatibilityManager.getPlugin('native');

      expect(wine).toBeDefined();
      expect(proton).toBeDefined();
      expect(native).toBeDefined();
    });
  });
});
