/**
 * Test Suite for Compatibility Manager
 */

const CompatibilityManager = require('../src/compatibility/manager');
const fs = require('fs').promises;
const path = require('path');

describe('CompatibilityManager', () => {
  let manager;

  beforeEach(() => {
    manager = new CompatibilityManager();
  });

  describe('loadPlugins()', () => {
    test('should load plugins without errors', async () => {
      await expect(manager.loadPlugins()).resolves.not.toThrow();
    });

    test('should load built-in plugins', async () => {
      await manager.loadPlugins();

      expect(manager.plugins.size).toBeGreaterThan(0);
    });

    test('should load wine plugin', async () => {
      await manager.loadPlugins();

      const wine = manager.getPlugin('wine');
      expect(wine).toBeDefined();
      expect(wine.name).toBe('wine');
    });

    test('should load proton plugin', async () => {
      await manager.loadPlugins();

      const proton = manager.getPlugin('proton');
      expect(proton).toBeDefined();
      expect(proton.name).toBe('proton');
    });

    test('should load native plugin', async () => {
      await manager.loadPlugins();

      const native = manager.getPlugin('native');
      expect(native).toBeDefined();
      expect(native.name).toBe('native');
    });
  });

  describe('getPlugin()', () => {
    beforeEach(async () => {
      await manager.loadPlugins();
    });

    test('should return plugin by name', () => {
      const wine = manager.getPlugin('wine');
      expect(wine).toBeDefined();
      expect(wine.name).toBe('wine');
    });

    test('should return undefined for non-existent plugin', () => {
      const plugin = manager.getPlugin('nonexistent');
      expect(plugin).toBeUndefined();
    });
  });

  describe('launch()', () => {
    beforeEach(async () => {
      await manager.loadPlugins();
    });

    test('should throw error for unsupported compatibility layer', async () => {
      const game = {
        name: 'Test Game',
        compatibilityLayer: 'nonexistent'
      };

      await expect(manager.launch(game)).rejects.toThrow('not found');
    });

    test('should use native layer by default', async () => {
      const game = {
        name: 'Test Game',
        path: '/test',
        executable: 'test'
      };

      // Mock the native plugin's launch method
      const nativePlugin = manager.getPlugin('native');
      nativePlugin.launch = jest.fn().mockResolvedValue({});

      await manager.launch(game);

      expect(nativePlugin.launch).toHaveBeenCalledWith(game);
    });
  });
});
