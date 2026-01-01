/**
 * Test Suite for Validator
 */

const Validator = require('../src/core/validator');
const fs = require('fs').promises;
const path = require('path');
const os = require('os');

describe('Validator', () => {
  describe('validateGame()', () => {
    test('should pass for valid game', () => {
      const game = {
        name: 'Test Game',
        path: '/path/to/game',
        platform: 'windows',
        executable: 'game.exe'
      };

      const errors = Validator.validateGame(game);
      expect(errors).toEqual([]);
    });

    test('should fail when game is null', () => {
      const errors = Validator.validateGame(null);
      expect(errors).toContain('Game object is required');
    });

    test('should fail when name is missing', () => {
      const game = {
        path: '/path',
        platform: 'windows',
        executable: 'test.exe'
      };

      const errors = Validator.validateGame(game);
      expect(errors.length).toBeGreaterThan(0);
      expect(errors[0]).toContain('name');
    });

    test('should fail when name is empty string', () => {
      const game = {
        name: '   ',
        path: '/path',
        platform: 'windows',
        executable: 'test.exe'
      };

      const errors = Validator.validateGame(game);
      expect(errors.length).toBeGreaterThan(0);
    });

    test('should fail for invalid platform', () => {
      const game = {
        name: 'Test',
        path: '/path',
        platform: 'invalid',
        executable: 'test.exe'
      };

      const errors = Validator.validateGame(game);
      expect(errors.length).toBeGreaterThan(0);
      expect(errors.some(e => e.includes('platform'))).toBe(true);
    });

    test('should pass for all valid platforms', () => {
      const platforms = ['windows', 'linux', 'native', 'macos'];

      platforms.forEach(platform => {
        const game = {
          name: 'Test',
          path: '/path',
          platform,
          executable: 'test'
        };

        const errors = Validator.validateGame(game);
        expect(errors).toEqual([]);
      });
    });
  });

  describe('validateCompatibilityLayer()', () => {
    test('should pass for valid layers', () => {
      const layers = ['wine', 'proton', 'proton-ge', 'native'];

      layers.forEach(layer => {
        const errors = Validator.validateCompatibilityLayer(layer);
        expect(errors).toEqual([]);
      });
    });

    test('should fail for invalid layer', () => {
      const errors = Validator.validateCompatibilityLayer('invalid');
      expect(errors.length).toBeGreaterThan(0);
    });

    test('should fail for non-string layer', () => {
      const errors = Validator.validateCompatibilityLayer(123);
      expect(errors.length).toBeGreaterThan(0);
    });
  });

  describe('validateGamePath()', () => {
    let testDir;

    beforeEach(async () => {
      testDir = await fs.mkdtemp(path.join(os.tmpdir(), 'validator-test-'));
    });

    afterEach(async () => {
      try {
        await fs.rm(testDir, { recursive: true, force: true });
      } catch (err) {
        // Ignore
      }
    });

    test('should pass for existing directory', async () => {
      const errors = await Validator.validateGamePath(testDir);
      expect(errors).toEqual([]);
    });

    test('should fail for non-existent path', async () => {
      const errors = await Validator.validateGamePath('/nonexistent/path');
      expect(errors.length).toBeGreaterThan(0);
    });

    test('should fail for file instead of directory', async () => {
      const testFile = path.join(testDir, 'test.txt');
      await fs.writeFile(testFile, 'test');

      const errors = await Validator.validateGamePath(testFile);
      expect(errors.length).toBeGreaterThan(0);
    });
  });

  describe('validateExecutable()', () => {
    let testDir;

    beforeEach(async () => {
      testDir = await fs.mkdtemp(path.join(os.tmpdir(), 'validator-test-'));
    });

    afterEach(async () => {
      try {
        await fs.rm(testDir, { recursive: true, force: true });
      } catch (err) {
        // Ignore
      }
    });

    test('should pass for existing executable', async () => {
      const execFile = 'game.exe';
      await fs.writeFile(path.join(testDir, execFile), '');

      const errors = await Validator.validateExecutable(testDir, execFile);
      expect(errors).toEqual([]);
    });

    test('should fail for non-existent executable', async () => {
      const errors = await Validator.validateExecutable(testDir, 'nonexistent.exe');
      expect(errors.length).toBeGreaterThan(0);
    });
  });

  describe('sanitizeGameName()', () => {
    test('should remove invalid characters', () => {
      const name = 'Game: <Name> "Test" | Part?';
      const sanitized = Validator.sanitizeGameName(name);
      
      expect(sanitized).not.toContain(':');
      expect(sanitized).not.toContain('<');
      expect(sanitized).not.toContain('>');
      expect(sanitized).not.toContain('"');
      expect(sanitized).not.toContain('|');
      expect(sanitized).not.toContain('?');
    });

    test('should replace spaces with underscores', () => {
      const sanitized = Validator.sanitizeGameName('Game Name Test');
      expect(sanitized).toBe('game_name_test');
    });

    test('should convert to lowercase', () => {
      const sanitized = Validator.sanitizeGameName('GameName');
      expect(sanitized).toBe('gamename');
    });
  });

  describe('validateEnvironment()', () => {
    test('should validate environment', () => {
      const result = Validator.validateEnvironment();
      
      expect(result).toHaveProperty('errors');
      expect(result).toHaveProperty('warnings');
      expect(Array.isArray(result.errors)).toBe(true);
      expect(Array.isArray(result.warnings)).toBe(true);
    });
  });
});
