/**
 * Urban Scum - Main Entry Point
 */

// Global game instance
let game = null;

// Initialize the game when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    initGame();
});

// Also listen for Cordova device ready
document.addEventListener('deviceready', () => {
    console.log('Device ready (Cordova)');
    Ads.init();
}, false);

async function initGame() {
    console.log('🎮 Urban Scum - Initializing...');
    
    // Show loading screen
    UI.setLoadingProgress(0);
    UI.setLoadingText('Initializing...');
    
    try {
        // Step 1: Initialize Audio
        UI.setLoadingProgress(10);
        UI.setLoadingText('Loading audio...');
        await delay(100);
        Audio.init();
        
        // Step 2: Initialize Sprites
        UI.setLoadingProgress(30);
        UI.setLoadingText('Creating sprites...');
        await delay(100);
        Sprites.init();
        
        // Step 3: Initialize Game Engine
        UI.setLoadingProgress(50);
        UI.setLoadingText('Setting up game...');
        await delay(100);
        game = new Game();
        game.init();
        
        // Step 4: Initialize UI
        UI.setLoadingProgress(70);
        UI.setLoadingText('Preparing UI...');
        await delay(100);
        UI.init(game);
        
        // Step 5: Initialize Ads
        UI.setLoadingProgress(85);
        UI.setLoadingText('Loading ads...');
        await delay(100);
        Ads.init();
        
        // Step 6: Load saved data
        UI.setLoadingProgress(95);
        UI.setLoadingText('Loading saved data...');
        await delay(100);
        UI.updateBestScore();
        UI.updateLevelSelect();
        
        // Complete
        UI.setLoadingProgress(100);
        UI.setLoadingText('Ready!');
        await delay(500);
        
        // Show start screen
        UI.showScreen('start');
        
        console.log('🎮 Urban Scum - Ready!');
        
        // Resume audio context on first user interaction
        document.addEventListener('click', () => Audio.resume(), { once: true });
        document.addEventListener('touchstart', () => Audio.resume(), { once: true });
        
    } catch (error) {
        console.error('Failed to initialize game:', error);
        UI.setLoadingText('Error loading game. Please refresh.');
    }
}

// Helper delay function
function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

// Prevent default touch behaviors that could interfere with the game
document.addEventListener('touchmove', (e) => {
    if (e.target.closest('#game-canvas, .touch-controls')) {
        e.preventDefault();
    }
}, { passive: false });

// Prevent zoom on double tap
let lastTouchEnd = 0;
document.addEventListener('touchend', (e) => {
    const now = Date.now();
    if (now - lastTouchEnd <= 300) {
        e.preventDefault();
    }
    lastTouchEnd = now;
}, false);

// Handle visibility change (pause when tab is hidden)
document.addEventListener('visibilitychange', () => {
    if (document.hidden && game && game.state === 'playing') {
        UI.pauseGame();
    }
});

// Handle back button on Android
document.addEventListener('backbutton', (e) => {
    e.preventDefault();
    
    if (game) {
        switch (game.state) {
            case 'playing':
                UI.pauseGame();
                break;
            case 'paused':
                UI.quitGame();
                break;
            default:
                if (UI.currentScreen !== 'start') {
                    UI.showScreen('start');
                } else {
                    // Exit app
                    if (navigator.app && navigator.app.exitApp) {
                        navigator.app.exitApp();
                    }
                }
        }
    }
}, false);

// Service Worker registration for PWA support
if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
        navigator.serviceWorker.register('sw.js')
            .then(registration => {
                console.log('SW registered:', registration.scope);
            })
            .catch(error => {
                console.log('SW registration failed:', error);
            });
    });
}

// Export game for debugging
window.urbanScum = {
    game: () => game,
    Storage,
    Audio,
    Sprites,
    UI,
    Ads,
    Levels,
    Utils
};
