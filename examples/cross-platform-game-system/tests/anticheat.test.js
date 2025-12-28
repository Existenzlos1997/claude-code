/**
 * Test Suite for Anti-Cheat Manager
 */

const AntiCheatManager = require('../src/compatibility/anticheat');
const fs = require('fs').promises;
const path = require('path');
const os = require('os');

describe('AntiCheatManager', () => {
  let manager;
  let testGameDir;

  beforeEach(async () => {
    manager = new AntiCheatManager();
    testGameDir = await fs.mkdtemp(path.join(os.tmpdir(), 'game-test-'));
  });

  afterEach(async () => {
    try {
      await fs.rm(testGameDir, { recursive: true, force: true });
    } catch (err) {
      // Ignore cleanup errors
    }
  });

  describe('detectAntiCheat()', () => {
    test('should detect EasyAntiCheat', async () => {
      // Create EAC file
      await fs.writeFile(path.join(testGameDir, 'EasyAntiCheat.exe'), '');

      const detected = await manager.detectAntiCheat(testGameDir);

      expect(detected).toHaveLength(1);
      expect(detected[0].type).toBe('eac');
      expect(detected[0].name).toBe('EasyAntiCheat');
    });

    test('should detect BattlEye', async () => {
      await fs.writeFile(path.join(testGameDir, 'BEClient.dll'), '');

      const detected = await manager.detectAntiCheat(testGameDir);

      expect(detected).toHaveLength(1);
      expect(detected[0].type).toBe('battleye');
      expect(detected[0].name).toBe('BattlEye');
    });

    test('should detect multiple anti-cheat systems', async () => {
      await fs.writeFile(path.join(testGameDir, 'EasyAntiCheat.exe'), '');
      await fs.writeFile(path.join(testGameDir, 'BEClient.dll'), '');

      const detected = await manager.detectAntiCheat(testGameDir);

      expect(detected.length).toBeGreaterThanOrEqual(2);
    });

    test('should return empty array when no anti-cheat detected', async () => {
      const detected = await manager.detectAntiCheat(testGameDir);

      expect(detected).toEqual([]);
    });

    test('should detect Riot Vanguard', async () => {
      await fs.writeFile(path.join(testGameDir, 'vgk.sys'), '');

      const detected = await manager.detectAntiCheat(testGameDir);

      expect(detected).toHaveLength(1);
      expect(detected[0].type).toBe('vanguard');
      expect(detected[0].supported).toBe(false);
    });
  });

  describe('isCompatible()', () => {
    test('should return true for supported anti-cheat systems', () => {
      expect(manager.isCompatible('eac')).toBe(true);
      expect(manager.isCompatible('battleye')).toBe(true);
      expect(manager.isCompatible('vac')).toBe(true);
    });

    test('should return false for unsupported anti-cheat systems', () => {
      expect(manager.isCompatible('vanguard')).toBe(false);
      expect(manager.isCompatible('faceit')).toBe(false);
    });

    test('should return false for unknown anti-cheat systems', () => {
      expect(manager.isCompatible('unknown')).toBe(false);
    });
  });

  describe('getRecommendedProton()', () => {
    test('should recommend proton-experimental for EAC', () => {
      const proton = manager.getRecommendedProton('eac');
      expect(proton).toBe('proton-experimental');
    });

    test('should recommend proton-experimental for BattlEye', () => {
      const proton = manager.getRecommendedProton('battleye');
      expect(proton).toBe('proton-experimental');
    });

    test('should return null for unsupported systems', () => {
      const proton = manager.getRecommendedProton('vanguard');
      expect(proton).toBeNull();
    });

    test('should return null for unknown systems', () => {
      const proton = manager.getRecommendedProton('unknown');
      expect(proton).toBeNull();
    });
  });

  describe('getAntiCheatEnvironment()', () => {
    test('should set EAC environment variables', () => {
      const env = manager.getAntiCheatEnvironment('eac', { TEST: 'value' });

      expect(env.PROTON_USE_EAC).toBe('1');
      expect(env.WINE_HIDE_WINE).toBe('1');
      expect(env.TEST).toBe('value'); // Should preserve existing env
    });

    test('should set BattlEye environment variables', () => {
      const env = manager.getAntiCheatEnvironment('battleye');

      expect(env.PROTON_USE_BATTLEYE).toBe('1');
      expect(env.WINE_HIDE_WINE).toBe('1');
    });

    test('should return base environment for unsupported systems', () => {
      const baseEnv = { TEST: 'value' };
      const env = manager.getAntiCheatEnvironment('vanguard', baseEnv);

      expect(env.TEST).toBe('value');
      expect(env.WINE_HIDE_WINE).toBe('1');
    });
  });

  describe('getWorkarounds()', () => {
    test('should return null for supported systems', () => {
      const workarounds = manager.getWorkarounds('eac');
      expect(workarounds).toBeNull();
    });

    test('should return workarounds for unsupported systems', () => {
      const workarounds = manager.getWorkarounds('vanguard');

      expect(workarounds).toBeDefined();
      expect(workarounds).toHaveProperty('dualBoot');
      expect(workarounds).toHaveProperty('gpuPassthrough');
      expect(workarounds).toHaveProperty('cloudGaming');
      expect(workarounds).toHaveProperty('contactDev');
    });
  });

  describe('printCompatibilityReport()', () => {
    test('should handle empty detection array', () => {
      // Should not throw
      expect(() => {
        manager.printCompatibilityReport([]);
      }).not.toThrow();
    });

    test('should handle detection with supported systems', () => {
      const detected = [{
        type: 'eac',
        name: 'EasyAntiCheat',
        supported: true,
        notes: 'Native Linux support'
      }];

      expect(() => {
        manager.printCompatibilityReport(detected);
      }).not.toThrow();
    });
  });
});
