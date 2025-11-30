/**
 * Urban Scum - Local Storage Manager
 */

const Storage = {
    prefix: 'urbanscum_',
    
    // Get value from storage
    get: (key, defaultValue = null) => {
        try {
            const value = localStorage.getItem(Storage.prefix + key);
            if (value === null) return defaultValue;
            return JSON.parse(value);
        } catch (e) {
            console.warn('Storage get error:', e);
            return defaultValue;
        }
    },
    
    // Set value in storage
    set: (key, value) => {
        try {
            localStorage.setItem(Storage.prefix + key, JSON.stringify(value));
            return true;
        } catch (e) {
            console.warn('Storage set error:', e);
            return false;
        }
    },
    
    // Remove value from storage
    remove: (key) => {
        try {
            localStorage.removeItem(Storage.prefix + key);
            return true;
        } catch (e) {
            console.warn('Storage remove error:', e);
            return false;
        }
    },
    
    // Clear all game storage
    clear: () => {
        try {
            Object.keys(localStorage)
                .filter(key => key.startsWith(Storage.prefix))
                .forEach(key => localStorage.removeItem(key));
            return true;
        } catch (e) {
            console.warn('Storage clear error:', e);
            return false;
        }
    },
    
    // Game-specific storage methods
    
    // Highscores
    getHighscores: () => {
        return Storage.get('highscores', []);
    },
    
    saveHighscore: (score) => {
        const highscores = Storage.getHighscores();
        highscores.push({
            score: score,
            date: Date.now()
        });
        // Sort by score descending and keep top 10
        highscores.sort((a, b) => b.score - a.score);
        const topScores = highscores.slice(0, 10);
        Storage.set('highscores', topScores);
        return topScores;
    },
    
    getBestScore: () => {
        const highscores = Storage.getHighscores();
        return highscores.length > 0 ? highscores[0].score : 0;
    },
    
    // Level progress
    getLevelProgress: () => {
        return Storage.get('levelProgress', {
            1: { unlocked: true, stars: 0, bestScore: 0, completed: false },
            2: { unlocked: false, stars: 0, bestScore: 0, completed: false },
            3: { unlocked: false, stars: 0, bestScore: 0, completed: false }
        });
    },
    
    saveLevelProgress: (level, stars, score) => {
        const progress = Storage.getLevelProgress();
        if (!progress[level]) {
            progress[level] = { unlocked: true, stars: 0, bestScore: 0, completed: false };
        }
        
        progress[level].completed = true;
        if (stars > progress[level].stars) {
            progress[level].stars = stars;
        }
        if (score > progress[level].bestScore) {
            progress[level].bestScore = score;
        }
        
        // Unlock next level
        const nextLevel = level + 1;
        if (nextLevel <= 3 && progress[nextLevel]) {
            progress[nextLevel].unlocked = true;
        } else if (nextLevel <= 3) {
            progress[nextLevel] = { unlocked: true, stars: 0, bestScore: 0, completed: false };
        }
        
        Storage.set('levelProgress', progress);
        return progress;
    },
    
    isLevelUnlocked: (level) => {
        const progress = Storage.getLevelProgress();
        return progress[level]?.unlocked || level === 1;
    },
    
    // Settings
    getSettings: () => {
        return Storage.get('settings', {
            sound: true,
            music: true,
            vibration: true
        });
    },
    
    saveSetting: (key, value) => {
        const settings = Storage.getSettings();
        settings[key] = value;
        Storage.set('settings', settings);
        return settings;
    },
    
    // Game state (for continue feature)
    saveGameState: (state) => {
        Storage.set('gameState', state);
    },
    
    getGameState: () => {
        return Storage.get('gameState', null);
    },
    
    clearGameState: () => {
        Storage.remove('gameState');
    },
    
    // Statistics
    getStats: () => {
        return Storage.get('stats', {
            totalGames: 0,
            totalScore: 0,
            totalTime: 0,
            enemiesDefeated: 0,
            powerUpsCollected: 0
        });
    },
    
    updateStats: (updates) => {
        const stats = Storage.getStats();
        Object.keys(updates).forEach(key => {
            if (stats[key] !== undefined) {
                stats[key] += updates[key];
            }
        });
        Storage.set('stats', stats);
        return stats;
    }
};

// Export for use
window.Storage = Storage;
