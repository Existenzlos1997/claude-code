/**
 * Achievement Tracking System
 * Phase 5: Advanced Features
 */

const fs = require('fs').promises;
const path = require('path');

class AchievementTracker {
  constructor(dataPath) {
    this.dataPath = dataPath || path.join(require('os').homedir(), '.game-system', 'achievements.json');
    this.achievements = new Map();
    this.unlocked = new Set();
  }

  /**
   * Initialize achievement tracker
   */
  async initialize() {
    try {
      const data = await fs.readFile(this.dataPath, 'utf8');
      const saved = JSON.parse(data);
      
      for (const [id, achievement] of Object.entries(saved.achievements || {})) {
        this.achievements.set(id, achievement);
      }
      
      this.unlocked = new Set(saved.unlocked || []);
    } catch (error) {
      // File doesn't exist yet, start fresh
    }
  }

  /**
   * Register achievement
   */
  registerAchievement(id, achievement) {
    this.achievements.set(id, {
      id,
      name: achievement.name,
      description: achievement.description,
      points: achievement.points || 10,
      hidden: achievement.hidden || false,
      icon: achievement.icon || null,
      game: achievement.game
    });
  }

  /**
   * Unlock achievement
   */
  async unlockAchievement(id) {
    const achievement = this.achievements.get(id);
    if (!achievement) {
      throw new Error(`Achievement not found: ${id}`);
    }
    
    if (this.unlocked.has(id)) {
      return { unlocked: false, reason: 'already_unlocked' };
    }
    
    this.unlocked.add(id);
    achievement.unlockedAt = new Date();
    
    await this.save();
    
    console.log(`🏆 Achievement unlocked: ${achievement.name} (+${achievement.points} points)`);
    
    return { unlocked: true, achievement };
  }

  /**
   * Check if achievement is unlocked
   */
  isUnlocked(id) {
    return this.unlocked.has(id);
  }

  /**
   * Get achievement progress
   */
  getProgress(game = null) {
    let achievements = Array.from(this.achievements.values());
    
    if (game) {
      achievements = achievements.filter(a => a.game === game);
    }
    
    const total = achievements.length;
    const unlocked = achievements.filter(a => this.unlocked.has(a.id)).length;
    const totalPoints = achievements.reduce((sum, a) => sum + a.points, 0);
    const earnedPoints = achievements
      .filter(a => this.unlocked.has(a.id))
      .reduce((sum, a) => sum + a.points, 0);
    
    return {
      total,
      unlocked,
      locked: total - unlocked,
      percentage: total > 0 ? (unlocked / total * 100).toFixed(1) : 0,
      totalPoints,
      earnedPoints
    };
  }

  /**
   * Get achievements
   */
  getAchievements(filter = {}) {
    let achievements = Array.from(this.achievements.values());
    
    if (filter.game) {
      achievements = achievements.filter(a => a.game === filter.game);
    }
    
    if (filter.unlocked !== undefined) {
      achievements = achievements.filter(a => this.unlocked.has(a.id) === filter.unlocked);
    }
    
    if (filter.hidden !== undefined) {
      achievements = achievements.filter(a => a.hidden === filter.hidden);
    }
    
    return achievements;
  }

  /**
   * Get recent unlocks
   */
  getRecentUnlocks(limit = 10) {
    const unlocked = Array.from(this.achievements.values())
      .filter(a => this.unlocked.has(a.id) && a.unlockedAt)
      .sort((a, b) => b.unlockedAt - a.unlockedAt)
      .slice(0, limit);
    
    return unlocked;
  }

  /**
   * Save achievements
   */
  async save() {
    const data = {
      achievements: Object.fromEntries(this.achievements),
      unlocked: Array.from(this.unlocked)
    };
    
    const dir = path.dirname(this.dataPath);
    await fs.mkdir(dir, { recursive: true });
    await fs.writeFile(this.dataPath, JSON.stringify(data, null, 2));
  }
}

module.exports = AchievementTracker;
