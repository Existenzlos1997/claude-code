/**
 * Test Suite for Game Library
 */

const GameLibrary = require('../src/core/game-library');
const fs = require('fs').promises;
const path = require('path');
const os = require('os');

describe('GameLibrary', () => {
  let library;
  let testDir;
  let testLibraryPath;

  beforeEach(async () => {
    // Create temporary test directory
    testDir = await fs.mkdtemp(path.join(os.tmpdir(), 'game-system-test-'));
    testLibraryPath = path.join(testDir, 'library.json');
    
    library = new GameLibrary();
    library.libraryPath = testLibraryPath;
  });

  afterEach(async () => {
    // Clean up test directory
    try {
      await fs.rm(testDir, { recursive: true, force: true });
    } catch (err) {
      // Ignore cleanup errors
    }
  });

  describe('load()', () => {
    test('should load empty library when file does not exist', async () => {
      await library.load();
      expect(library.games).toEqual([]);
    });

    test('should load existing library from file', async () => {
      const testGames = [
        { id: '1', name: 'Test Game', platform: 'windows' }
      ];
      await fs.mkdir(path.dirname(testLibraryPath), { recursive: true });
      await fs.writeFile(testLibraryPath, JSON.stringify(testGames));

      await library.load();
      expect(library.games).toHaveLength(1);
      expect(library.games[0].name).toBe('Test Game');
    });
  });

  describe('save()', () => {
    test('should save library to file', async () => {
      library.games = [
        { id: '1', name: 'Test Game', platform: 'windows' }
      ];

      await library.save();

      const saved = await fs.readFile(testLibraryPath, 'utf8');
      const parsed = JSON.parse(saved);
      expect(parsed).toHaveLength(1);
      expect(parsed[0].name).toBe('Test Game');
    });

    test('should create directory if it does not exist', async () => {
      library.libraryPath = path.join(testDir, 'subdir', 'library.json');
      library.games = [{ id: '1', name: 'Test' }];

      await library.save();

      const exists = await fs.access(library.libraryPath)
        .then(() => true)
        .catch(() => false);
      expect(exists).toBe(true);
    });
  });

  describe('addGame()', () => {
    test('should add a new game to the library', () => {
      const game = {
        name: 'My Game',
        path: '/path/to/game',
        platform: 'windows',
        executable: 'game.exe'
      };

      library.addGame(game);

      expect(library.games).toHaveLength(1);
      expect(library.games[0].name).toBe('My Game');
      expect(library.games[0]).toHaveProperty('id');
      expect(library.games[0]).toHaveProperty('addedAt');
    });

    test('should set default compatibility layer', () => {
      library.addGame({
        name: 'Test',
        path: '/test',
        platform: 'windows',
        executable: 'test.exe'
      });

      expect(library.games[0].compatibilityLayer).toBe('native');
    });
  });

  describe('findGame()', () => {
    beforeEach(() => {
      library.games = [
        { id: '1', name: 'Game One', platform: 'windows' },
        { id: '2', name: 'Game Two', platform: 'linux' }
      ];
    });

    test('should find game by name (case insensitive)', () => {
      const game = library.findGame('game one');
      expect(game).toBeDefined();
      expect(game.name).toBe('Game One');
    });

    test('should find game by ID', () => {
      const game = library.findGame('2');
      expect(game).toBeDefined();
      expect(game.name).toBe('Game Two');
    });

    test('should return undefined for non-existent game', () => {
      const game = library.findGame('nonexistent');
      expect(game).toBeUndefined();
    });
  });

  describe('removeGame()', () => {
    beforeEach(() => {
      library.games = [
        { id: '1', name: 'Game One', platform: 'windows' },
        { id: '2', name: 'Game Two', platform: 'linux' }
      ];
    });

    test('should remove game by name', () => {
      const result = library.removeGame('game one');
      expect(result).toBe(true);
      expect(library.games).toHaveLength(1);
      expect(library.games[0].name).toBe('Game Two');
    });

    test('should remove game by ID', () => {
      const result = library.removeGame('2');
      expect(result).toBe(true);
      expect(library.games).toHaveLength(1);
    });

    test('should return false for non-existent game', () => {
      const result = library.removeGame('nonexistent');
      expect(result).toBe(false);
      expect(library.games).toHaveLength(2);
    });
  });
});
