/**
 * Urban Scum - UI Controller
 */

const UI = {
    screens: {},
    currentScreen: null,
    game: null,
    
    init: (game) => {
        UI.game = game;
        
        // Cache screen elements
        UI.screens = {
            loading: document.getElementById('loading-screen'),
            start: document.getElementById('start-screen'),
            level: document.getElementById('level-screen'),
            settings: document.getElementById('settings-screen'),
            highscore: document.getElementById('highscore-screen'),
            game: document.getElementById('game-screen'),
            pause: document.getElementById('pause-screen'),
            gameover: document.getElementById('gameover-screen'),
            levelcomplete: document.getElementById('levelcomplete-screen')
        };
        
        // Setup button handlers
        UI.setupButtons();
        
        // Setup settings
        UI.setupSettings();
        
        // Setup game callbacks
        UI.setupGameCallbacks();
        
        console.log('UI initialized');
    },
    
    setupButtons: () => {
        // Start screen buttons
        document.getElementById('btn-play').addEventListener('click', () => {
            Audio.play('click');
            UI.startGame(1);
        });
        
        document.getElementById('btn-levels').addEventListener('click', () => {
            Audio.play('click');
            UI.showScreen('level');
            UI.updateLevelSelect();
        });
        
        document.getElementById('btn-highscore').addEventListener('click', () => {
            Audio.play('click');
            UI.showScreen('highscore');
            UI.updateHighscoreList();
        });
        
        document.getElementById('btn-settings').addEventListener('click', () => {
            Audio.play('click');
            UI.showScreen('settings');
        });
        
        // Back buttons
        document.getElementById('btn-back-levels').addEventListener('click', () => {
            Audio.play('click');
            UI.showScreen('start');
        });
        
        document.getElementById('btn-back-settings').addEventListener('click', () => {
            Audio.play('click');
            UI.showScreen('start');
        });
        
        document.getElementById('btn-back-highscore').addEventListener('click', () => {
            Audio.play('click');
            UI.showScreen('start');
        });
        
        // Level select cards
        document.querySelectorAll('.level-card').forEach(card => {
            card.addEventListener('click', () => {
                const level = parseInt(card.dataset.level);
                if (Storage.isLevelUnlocked(level)) {
                    Audio.play('click');
                    UI.startGame(level);
                }
            });
        });
        
        // Game HUD
        document.getElementById('btn-pause').addEventListener('click', () => {
            Audio.play('click');
            UI.pauseGame();
        });
        
        // Pause screen buttons
        document.getElementById('btn-resume').addEventListener('click', () => {
            Audio.play('click');
            UI.resumeGame();
        });
        
        document.getElementById('btn-restart').addEventListener('click', () => {
            Audio.play('click');
            UI.restartGame();
        });
        
        document.getElementById('btn-quit').addEventListener('click', () => {
            Audio.play('click');
            UI.quitGame();
        });
        
        // Game over buttons
        document.getElementById('btn-retry').addEventListener('click', () => {
            Audio.play('click');
            UI.restartGame();
        });
        
        document.getElementById('btn-watch-ad').addEventListener('click', () => {
            Audio.play('click');
            Ads.showRewarded(() => {
                UI.game.giveExtraLife();
                UI.hideOverlay('gameover');
            });
        });
        
        document.getElementById('btn-home').addEventListener('click', () => {
            Audio.play('click');
            UI.quitGame();
        });
        
        // Level complete buttons
        document.getElementById('btn-next-level').addEventListener('click', () => {
            Audio.play('click');
            const nextLevel = UI.game.currentLevel + 1;
            if (Levels.exists(nextLevel)) {
                UI.startGame(nextLevel);
            } else {
                UI.quitGame();
            }
        });
        
        document.getElementById('btn-replay-level').addEventListener('click', () => {
            Audio.play('click');
            UI.restartGame();
        });
        
        document.getElementById('btn-levels-complete').addEventListener('click', () => {
            Audio.play('click');
            UI.showScreen('level');
            UI.updateLevelSelect();
        });
    },
    
    setupSettings: () => {
        const settings = Storage.getSettings();
        
        const soundToggle = document.getElementById('sound-toggle');
        const musicToggle = document.getElementById('music-toggle');
        const vibrationToggle = document.getElementById('vibration-toggle');
        
        soundToggle.checked = settings.sound;
        musicToggle.checked = settings.music;
        vibrationToggle.checked = settings.vibration;
        
        soundToggle.addEventListener('change', (e) => {
            Audio.toggleSound(e.target.checked);
        });
        
        musicToggle.addEventListener('change', (e) => {
            Audio.toggleMusic(e.target.checked);
        });
        
        vibrationToggle.addEventListener('change', (e) => {
            Storage.saveSetting('vibration', e.target.checked);
        });
    },
    
    setupGameCallbacks: () => {
        UI.game.onScoreUpdate = (score) => {
            document.getElementById('current-score').textContent = Utils.formatNumber(score);
        };
        
        UI.game.onComboUpdate = (combo) => {
            const comboDisplay = document.getElementById('combo-display');
            if (combo > 1) {
                comboDisplay.classList.add('active');
                comboDisplay.querySelector('.combo-value').textContent = `x${combo}`;
            } else {
                comboDisplay.classList.remove('active');
            }
        };
        
        UI.game.onLivesUpdate = (lives) => {
            const hearts = '❤️'.repeat(lives) + '🖤'.repeat(3 - lives);
            document.getElementById('lives-display').textContent = hearts;
        };
        
        UI.game.onPowerUpCollect = (name) => {
            const indicator = document.getElementById('power-up-indicator');
            indicator.textContent = name;
            indicator.classList.add('active');
            setTimeout(() => indicator.classList.remove('active'), 2000);
        };
        
        UI.game.onGameOver = (score, bestScore) => {
            UI.showOverlay('gameover');
            document.getElementById('final-score').textContent = Utils.formatNumber(score);
            document.getElementById('gameover-best').textContent = Utils.formatNumber(bestScore);
            
            const newRecord = document.getElementById('new-record');
            if (score >= bestScore && score > 0) {
                newRecord.classList.remove('hidden');
            } else {
                newRecord.classList.add('hidden');
            }
            
            // Show interstitial ad after game over
            setTimeout(() => Ads.showInterstitial(), 500);
        };
        
        UI.game.onLevelComplete = (score, stars, time, hasNextLevel) => {
            UI.showOverlay('levelcomplete');
            document.getElementById('level-score').textContent = Utils.formatNumber(score);
            document.getElementById('level-time').textContent = Utils.formatTime(time);
            
            // Show stars
            const starIcons = ['☆', '☆', '☆'];
            for (let i = 0; i < stars; i++) {
                starIcons[i] = '⭐';
            }
            document.getElementById('stars-earned').textContent = starIcons.join('');
            
            // Show/hide next level button
            const nextBtn = document.getElementById('btn-next-level');
            if (hasNextLevel) {
                nextBtn.classList.remove('hidden');
            } else {
                nextBtn.classList.add('hidden');
            }
        };
    },
    
    showScreen: (screenName) => {
        // Hide all screens
        Object.values(UI.screens).forEach(screen => {
            screen.classList.remove('active');
        });
        
        // Show target screen
        if (UI.screens[screenName]) {
            UI.screens[screenName].classList.add('active');
            UI.currentScreen = screenName;
        }
    },
    
    showOverlay: (overlayName) => {
        if (UI.screens[overlayName]) {
            UI.screens[overlayName].classList.add('active');
        }
    },
    
    hideOverlay: (overlayName) => {
        if (UI.screens[overlayName]) {
            UI.screens[overlayName].classList.remove('active');
        }
    },
    
    updateBestScore: () => {
        document.getElementById('best-score').textContent = Utils.formatNumber(Storage.getBestScore());
    },
    
    updateLevelSelect: () => {
        const progress = Storage.getLevelProgress();
        
        document.querySelectorAll('.level-card').forEach(card => {
            const level = parseInt(card.dataset.level);
            const levelProgress = progress[level] || { unlocked: false, stars: 0 };
            
            // Update locked state
            if (levelProgress.unlocked) {
                card.classList.remove('locked');
                card.querySelector('.level-locked').textContent = '';
            } else {
                card.classList.add('locked');
                card.querySelector('.level-locked').textContent = '🔒';
            }
            
            // Update stars
            const starIcons = ['☆', '☆', '☆'];
            for (let i = 0; i < levelProgress.stars; i++) {
                starIcons[i] = '⭐';
            }
            card.querySelector('.level-stars').textContent = starIcons.join('');
        });
    },
    
    updateHighscoreList: () => {
        const highscores = Storage.getHighscores();
        const list = document.getElementById('highscore-list');
        
        if (highscores.length === 0) {
            list.innerHTML = '<div class="highscore-item"><span>No scores yet!</span></div>';
            return;
        }
        
        list.innerHTML = highscores.map((entry, index) => `
            <div class="highscore-item">
                <span class="highscore-rank">${index + 1}.</span>
                <span class="highscore-score">${Utils.formatNumber(entry.score)}</span>
            </div>
        `).join('');
    },
    
    startGame: (level) => {
        UI.game.loadLevel(level);
        UI.showScreen('game');
        
        // Update HUD
        document.getElementById('level-indicator').textContent = `LEVEL ${level}`;
        document.getElementById('current-score').textContent = '0';
        document.getElementById('lives-display').textContent = '❤️❤️❤️';
        document.getElementById('combo-display').classList.remove('active');
        document.getElementById('power-up-indicator').classList.remove('active');
        
        // Start game
        setTimeout(() => UI.game.start(), 100);
    },
    
    pauseGame: () => {
        UI.game.pause();
        UI.showOverlay('pause');
    },
    
    resumeGame: () => {
        UI.hideOverlay('pause');
        setTimeout(() => UI.game.resume(), 100);
    },
    
    restartGame: () => {
        UI.hideOverlay('pause');
        UI.hideOverlay('gameover');
        UI.hideOverlay('levelcomplete');
        UI.game.restart();
    },
    
    quitGame: () => {
        UI.hideOverlay('pause');
        UI.hideOverlay('gameover');
        UI.hideOverlay('levelcomplete');
        UI.game.pause();
        UI.showScreen('start');
        UI.updateBestScore();
    },
    
    // Loading progress
    setLoadingProgress: (percent) => {
        const progressBar = document.querySelector('.loading-progress');
        if (progressBar) {
            progressBar.style.width = `${percent}%`;
        }
    },
    
    setLoadingText: (text) => {
        const loadingText = document.querySelector('.loading-text');
        if (loadingText) {
            loadingText.textContent = text;
        }
    }
};

// Export
window.UI = UI;
