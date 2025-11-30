/**
 * Urban Scum - Main Game Engine
 */

class Game {
    constructor() {
        this.canvas = null;
        this.ctx = null;
        this.width = 0;
        this.height = 0;
        
        // Game state
        this.state = 'loading'; // loading, menu, playing, paused, gameover, levelcomplete
        this.currentLevel = 1;
        this.score = 0;
        this.combo = 1;
        this.comboTimer = 0;
        this.time = 0;
        this.isPaused = false;
        
        // Game objects
        this.player = null;
        this.platforms = [];
        this.enemies = [];
        this.collectibles = [];
        this.powerUps = [];
        this.obstacles = [];
        this.particles = [];
        
        // Camera
        this.cameraX = 0;
        this.cameraY = 0;
        
        // Input
        this.input = {
            left: false,
            right: false,
            jump: false,
            action: false
        };
        
        // Timing
        this.lastTime = 0;
        this.deltaTime = 0;
        this.gameTime = 0;
        
        // Level data
        this.levelData = null;
        this.levelWidth = 0;
        this.background = null;
        
        // Callbacks
        this.onScoreUpdate = null;
        this.onComboUpdate = null;
        this.onLivesUpdate = null;
        this.onPowerUpCollect = null;
        this.onGameOver = null;
        this.onLevelComplete = null;
    }
    
    init() {
        this.canvas = document.getElementById('game-canvas');
        this.ctx = this.canvas.getContext('2d');
        
        this.resize();
        window.addEventListener('resize', () => this.resize());
        
        this.setupInput();
        
        console.log('Game engine initialized');
    }
    
    resize() {
        this.width = window.innerWidth;
        this.height = window.innerHeight;
        this.canvas.width = this.width;
        this.canvas.height = this.height;
    }
    
    setupInput() {
        // Keyboard
        document.addEventListener('keydown', (e) => {
            switch (e.code) {
                case 'ArrowLeft':
                case 'KeyA':
                    this.input.left = true;
                    break;
                case 'ArrowRight':
                case 'KeyD':
                    this.input.right = true;
                    break;
                case 'ArrowUp':
                case 'KeyW':
                case 'Space':
                    this.input.jump = true;
                    e.preventDefault();
                    break;
                case 'KeyE':
                case 'ShiftLeft':
                    this.input.action = true;
                    break;
            }
        });
        
        document.addEventListener('keyup', (e) => {
            switch (e.code) {
                case 'ArrowLeft':
                case 'KeyA':
                    this.input.left = false;
                    break;
                case 'ArrowRight':
                case 'KeyD':
                    this.input.right = false;
                    break;
                case 'ArrowUp':
                case 'KeyW':
                case 'Space':
                    this.input.jump = false;
                    break;
                case 'KeyE':
                case 'ShiftLeft':
                    this.input.action = false;
                    break;
            }
        });
        
        // Touch controls
        const touchLeft = document.getElementById('touch-left');
        const touchRight = document.getElementById('touch-right');
        const touchAction = document.getElementById('touch-action');
        
        // Left button
        touchLeft.addEventListener('touchstart', (e) => {
            e.preventDefault();
            this.input.left = true;
        });
        touchLeft.addEventListener('touchend', (e) => {
            e.preventDefault();
            this.input.left = false;
        });
        
        // Right button
        touchRight.addEventListener('touchstart', (e) => {
            e.preventDefault();
            this.input.right = true;
        });
        touchRight.addEventListener('touchend', (e) => {
            e.preventDefault();
            this.input.right = false;
        });
        
        // Action button (jump)
        touchAction.addEventListener('touchstart', (e) => {
            e.preventDefault();
            this.input.jump = true;
        });
        touchAction.addEventListener('touchend', (e) => {
            e.preventDefault();
            this.input.jump = false;
        });
        
        // Swipe up for jump on canvas
        let touchStartY = 0;
        this.canvas.addEventListener('touchstart', (e) => {
            touchStartY = e.touches[0].clientY;
        });
        
        this.canvas.addEventListener('touchmove', (e) => {
            const touchY = e.touches[0].clientY;
            const deltaY = touchStartY - touchY;
            
            if (deltaY > 50) { // Swipe up
                this.input.jump = true;
                touchStartY = touchY;
            }
        });
        
        this.canvas.addEventListener('touchend', () => {
            this.input.jump = false;
        });
    }
    
    loadLevel(levelNumber) {
        this.currentLevel = levelNumber;
        this.levelData = Levels.get(levelNumber);
        
        if (!this.levelData) {
            console.error('Level not found:', levelNumber);
            return false;
        }
        
        // Reset game state
        this.score = 0;
        this.combo = 1;
        this.comboTimer = 0;
        this.time = 0;
        this.gameTime = 0;
        this.particles = [];
        
        // Set level properties
        this.levelWidth = this.levelData.width;
        this.background = Sprites.get(this.levelData.background);
        
        // Create player
        this.player = new Player(this.levelData.startX, this.levelData.startY);
        
        // Create platforms
        this.platforms = this.levelData.platforms.map(p => 
            new Platform(p.x, p.y, p.width, p.type)
        );
        
        // Create enemies
        this.enemies = this.levelData.enemies.map(e => 
            new Enemy(e.x, e.y, 50, 50, e.type)
        );
        
        // Create collectibles
        this.collectibles = this.levelData.collectibles.map(c => 
            new Collectible(c.x, c.y, c.type)
        );
        
        // Create power-ups
        this.powerUps = this.levelData.powerUps.map(p => 
            new PowerUp(p.x, p.y, p.type)
        );
        
        // Create obstacles
        this.obstacles = this.levelData.obstacles.map(o => 
            new Obstacle(o.x, o.y, o.type)
        );
        
        // Reset camera
        this.cameraX = 0;
        this.cameraY = 0;
        
        console.log('Level loaded:', this.levelData.name);
        return true;
    }
    
    start() {
        this.state = 'playing';
        this.isPaused = false;
        this.lastTime = performance.now();
        Audio.startMusic();
        requestAnimationFrame((time) => this.gameLoop(time));
    }
    
    pause() {
        this.state = 'paused';
        this.isPaused = true;
        Audio.stopMusic();
    }
    
    resume() {
        this.state = 'playing';
        this.isPaused = false;
        this.lastTime = performance.now();
        Audio.startMusic();
        requestAnimationFrame((time) => this.gameLoop(time));
    }
    
    restart() {
        this.loadLevel(this.currentLevel);
        this.start();
    }
    
    gameLoop(currentTime) {
        if (this.state !== 'playing') return;
        
        // Calculate delta time
        this.deltaTime = Math.min((currentTime - this.lastTime) / 1000, 0.1);
        this.lastTime = currentTime;
        this.gameTime += this.deltaTime;
        this.time += this.deltaTime;
        
        // Update
        this.update();
        
        // Draw
        this.draw();
        
        // Continue loop
        requestAnimationFrame((time) => this.gameLoop(time));
    }
    
    update() {
        // Update player
        this.player.update(this.deltaTime, this.input, this.platforms);
        
        // Update camera to follow player
        this.updateCamera();
        
        // Update platforms
        this.platforms.forEach(platform => platform.update(this.deltaTime));
        
        // Update enemies
        this.enemies.forEach(enemy => {
            if (enemy.active) {
                enemy.update(this.deltaTime, this.player);
                
                // Check collision with player
                if (enemy.collidesWith(this.player)) {
                    // Check if player is jumping on top of enemy
                    const playerBottom = this.player.y + this.player.height;
                    const enemyTop = enemy.y;
                    
                    if (this.player.velocityY > 0 && playerBottom < enemyTop + 20) {
                        // Defeat enemy
                        enemy.active = false;
                        this.player.velocityY = -400; // Bounce
                        this.addScore(enemy.points);
                        this.spawnParticles(enemy.x + enemy.width / 2, enemy.y + enemy.height / 2, '#ff6b6b', 10);
                        Audio.play('defeat');
                        Utils.vibrate(30);
                    } else {
                        // Player takes damage
                        const gameOver = this.player.takeDamage();
                        if (gameOver) {
                            this.handleGameOver();
                        } else if (this.onLivesUpdate) {
                            this.onLivesUpdate(this.player.lives);
                        }
                    }
                }
            }
        });
        
        // Update collectibles
        this.collectibles.forEach(collectible => {
            if (collectible.active) {
                collectible.update(this.deltaTime, this.gameTime);
                
                // Magnet effect
                if (this.player.hasMagnet) {
                    const dx = this.player.x - collectible.x;
                    const dy = this.player.y - collectible.y;
                    const dist = Math.sqrt(dx * dx + dy * dy);
                    
                    if (dist < 200) {
                        collectible.x += (dx / dist) * 300 * this.deltaTime;
                        collectible.y += (dy / dist) * 300 * this.deltaTime;
                    }
                }
                
                // Check collision
                if (collectible.collidesWith(this.player)) {
                    const points = collectible.collect(this.player);
                    this.addScore(points);
                    this.spawnParticles(collectible.x, collectible.y, '#f1c40f', 5);
                }
            }
        });
        
        // Update power-ups
        this.powerUps.forEach(powerUp => {
            if (powerUp.active) {
                powerUp.update(this.deltaTime, this.gameTime);
                
                if (powerUp.collidesWith(this.player)) {
                    const name = powerUp.collect(this.player);
                    if (this.onPowerUpCollect) {
                        this.onPowerUpCollect(name);
                    }
                    this.spawnParticles(powerUp.x + powerUp.width / 2, powerUp.y + powerUp.height / 2, '#4ecdc4', 15);
                }
            }
        });
        
        // Update obstacles
        this.obstacles.forEach(obstacle => {
            if (obstacle.active && obstacle.collidesWith(this.player)) {
                const gameOver = this.player.takeDamage();
                if (gameOver) {
                    this.handleGameOver();
                } else if (this.onLivesUpdate) {
                    this.onLivesUpdate(this.player.lives);
                }
            }
        });
        
        // Update particles
        this.particles = this.particles.filter(p => {
            p.update(this.deltaTime);
            return p.active;
        });
        
        // Update combo timer
        if (this.comboTimer > 0) {
            this.comboTimer -= this.deltaTime;
            if (this.comboTimer <= 0) {
                this.combo = 1;
                if (this.onComboUpdate) {
                    this.onComboUpdate(this.combo);
                }
            }
        }
        
        // Check if player fell off the map
        if (this.player.y > this.height + 100) {
            const gameOver = this.player.takeDamage();
            if (gameOver) {
                this.handleGameOver();
            } else {
                // Respawn at last safe position
                this.player.x = this.levelData.startX;
                this.player.y = this.levelData.startY;
                this.player.velocityY = 0;
                if (this.onLivesUpdate) {
                    this.onLivesUpdate(this.player.lives);
                }
            }
        }
        
        // Check level completion
        if (this.player.x >= this.levelWidth - 100) {
            this.handleLevelComplete();
        }
        
        // Update score display
        if (this.onScoreUpdate) {
            this.onScoreUpdate(this.score);
        }
    }
    
    updateCamera() {
        // Follow player with smooth lerp
        const targetX = this.player.x - this.width / 3;
        this.cameraX = Utils.lerp(this.cameraX, targetX, 0.1);
        
        // Clamp camera to level bounds
        this.cameraX = Utils.clamp(this.cameraX, 0, this.levelWidth - this.width);
    }
    
    draw() {
        // Clear canvas
        this.ctx.clearRect(0, 0, this.width, this.height);
        
        // Draw background
        this.drawBackground();
        
        // Draw platforms
        this.platforms.forEach(platform => platform.draw(this.ctx, this.cameraX, this.cameraY));
        
        // Draw obstacles
        this.obstacles.forEach(obstacle => {
            if (obstacle.active) {
                obstacle.draw(this.ctx, this.cameraX, this.cameraY);
            }
        });
        
        // Draw collectibles
        this.collectibles.forEach(collectible => {
            if (collectible.active) {
                collectible.draw(this.ctx, this.cameraX, this.cameraY);
            }
        });
        
        // Draw power-ups
        this.powerUps.forEach(powerUp => {
            if (powerUp.active) {
                powerUp.draw(this.ctx, this.cameraX, this.cameraY);
            }
        });
        
        // Draw enemies
        this.enemies.forEach(enemy => {
            if (enemy.active) {
                enemy.draw(this.ctx, this.cameraX, this.cameraY);
            }
        });
        
        // Draw player
        this.player.draw(this.ctx, this.cameraX, this.cameraY);
        
        // Draw particles
        this.particles.forEach(particle => particle.draw(this.ctx, this.cameraX, this.cameraY));
    }
    
    drawBackground() {
        if (!this.background) return;
        
        // Parallax scrolling
        const parallaxX = this.cameraX * 0.3;
        const bgWidth = 400;
        
        // Calculate how many times we need to draw the background
        const startX = Math.floor(parallaxX / bgWidth) * bgWidth - parallaxX;
        
        for (let x = startX; x < this.width + bgWidth; x += bgWidth) {
            this.ctx.drawImage(this.background, x, 0, bgWidth, this.height);
        }
    }
    
    addScore(points) {
        const scoreToAdd = points * this.combo;
        this.score += scoreToAdd;
        
        // Update combo
        this.combo = Math.min(this.combo + 1, 10);
        this.comboTimer = 2; // 2 seconds to maintain combo
        
        if (this.onComboUpdate) {
            this.onComboUpdate(this.combo);
        }
        
        Audio.play('combo');
    }
    
    spawnParticles(x, y, color, count) {
        for (let i = 0; i < count; i++) {
            this.particles.push(new Particle(x, y, color));
        }
    }
    
    handleGameOver() {
        this.state = 'gameover';
        Audio.stopMusic();
        Audio.play('gameover');
        Utils.vibrate([100, 50, 100, 50, 100]);
        
        // Save highscore
        Storage.saveHighscore(this.score);
        
        // Update stats
        Storage.updateStats({
            totalGames: 1,
            totalScore: this.score,
            totalTime: Math.floor(this.time)
        });
        
        if (this.onGameOver) {
            this.onGameOver(this.score, Storage.getBestScore());
        }
    }
    
    handleLevelComplete() {
        this.state = 'levelcomplete';
        Audio.stopMusic();
        Audio.play('levelcomplete');
        Utils.vibrate([50, 30, 50, 30, 50, 30, 100]);
        
        // Calculate stars
        const stars = Levels.calculateStars(this.currentLevel, this.score);
        
        // Save progress
        Storage.saveLevelProgress(this.currentLevel, stars, this.score);
        Storage.saveHighscore(this.score);
        
        // Update stats
        Storage.updateStats({
            totalGames: 1,
            totalScore: this.score,
            totalTime: Math.floor(this.time)
        });
        
        if (this.onLevelComplete) {
            this.onLevelComplete(this.score, stars, this.time, this.currentLevel < 3);
        }
    }
    
    // Give player extra life (for rewarded ad)
    giveExtraLife() {
        this.player.lives = 1;
        this.player.isInvincible = true;
        this.player.invincibleTimer = 3;
        this.state = 'playing';
        this.lastTime = performance.now();
        Audio.startMusic();
        
        if (this.onLivesUpdate) {
            this.onLivesUpdate(this.player.lives);
        }
        
        requestAnimationFrame((time) => this.gameLoop(time));
    }
}

// Export
window.Game = Game;
