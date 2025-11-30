/**
 * Urban Scum - Sprite Generator
 * Creates all game graphics programmatically using Canvas
 */

// Polyfill for roundRect (older browsers)
if (!CanvasRenderingContext2D.prototype.roundRect) {
    CanvasRenderingContext2D.prototype.roundRect = function(x, y, width, height, radii) {
        const radius = typeof radii === 'number' ? radii : (radii ? radii[0] : 0);
        this.beginPath();
        this.moveTo(x + radius, y);
        this.lineTo(x + width - radius, y);
        this.quadraticCurveTo(x + width, y, x + width, y + radius);
        this.lineTo(x + width, y + height - radius);
        this.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
        this.lineTo(x + radius, y + height);
        this.quadraticCurveTo(x, y + height, x, y + height - radius);
        this.lineTo(x, y + radius);
        this.quadraticCurveTo(x, y, x + radius, y);
        this.closePath();
        return this;
    };
}

const Sprites = {
    cache: {},
    
    // Create a cached canvas sprite
    createSprite: (name, width, height, drawFunction) => {
        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext('2d');
        drawFunction(ctx, width, height);
        Sprites.cache[name] = canvas;
        return canvas;
    },
    
    // Get cached sprite or create it
    get: (name) => {
        return Sprites.cache[name] || null;
    },
    
    // Initialize all game sprites
    init: () => {
        // Player character - Urban hero
        Sprites.createPlayer();
        Sprites.createPlayerRun();
        Sprites.createPlayerJump();
        Sprites.createPlayerHurt();
        
        // Enemies
        Sprites.createEnemy1(); // Rat
        Sprites.createEnemy2(); // Thug
        Sprites.createEnemy3(); // Robot
        
        // Collectibles
        Sprites.createCoin();
        Sprites.createGem();
        
        // Power-ups
        Sprites.createPowerUpSpeed();
        Sprites.createPowerUpShield();
        Sprites.createPowerUpMagnet();
        Sprites.createPowerUpDoublePoints();
        
        // Platforms
        Sprites.createPlatform();
        Sprites.createPlatformDanger();
        Sprites.createPlatformMoving();
        
        // Obstacles
        Sprites.createSpikes();
        Sprites.createBarrel();
        Sprites.createTrash();
        
        // Backgrounds
        Sprites.createBackground1(); // Downtown
        Sprites.createBackground2(); // Industrial
        Sprites.createBackground3(); // Nightlife
        
        // Effects
        Sprites.createParticle();
        Sprites.createStarBurst();
        
        console.log('Sprites initialized:', Object.keys(Sprites.cache).length, 'sprites created');
    },
    
    // Player sprites
    createPlayer: () => {
        Sprites.createSprite('player', 60, 80, (ctx, w, h) => {
            // Body
            ctx.fillStyle = '#4ecdc4';
            ctx.beginPath();
            ctx.roundRect(10, 25, 40, 45, 8);
            ctx.fill();
            
            // Head
            ctx.fillStyle = '#ffeaa7';
            ctx.beginPath();
            ctx.arc(30, 18, 18, 0, Math.PI * 2);
            ctx.fill();
            
            // Hair (spiky)
            ctx.fillStyle = '#2d3436';
            ctx.beginPath();
            ctx.moveTo(15, 12);
            ctx.lineTo(22, 0);
            ctx.lineTo(28, 10);
            ctx.lineTo(35, 2);
            ctx.lineTo(40, 12);
            ctx.lineTo(45, 5);
            ctx.lineTo(45, 20);
            ctx.lineTo(15, 20);
            ctx.closePath();
            ctx.fill();
            
            // Sunglasses
            ctx.fillStyle = '#1a1a2e';
            ctx.fillRect(18, 14, 10, 6);
            ctx.fillRect(32, 14, 10, 6);
            ctx.fillRect(28, 16, 4, 2);
            
            // Smile
            ctx.strokeStyle = '#d63031';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(30, 24, 6, 0.2, Math.PI - 0.2);
            ctx.stroke();
            
            // Arms
            ctx.fillStyle = '#ffeaa7';
            ctx.fillRect(2, 30, 10, 25);
            ctx.fillRect(48, 30, 10, 25);
            
            // Legs
            ctx.fillStyle = '#636e72';
            ctx.fillRect(14, 68, 12, 12);
            ctx.fillRect(34, 68, 12, 12);
            
            // Shoes
            ctx.fillStyle = '#d63031';
            ctx.fillRect(12, 75, 15, 5);
            ctx.fillRect(33, 75, 15, 5);
        });
    },
    
    createPlayerRun: () => {
        // Running animation frames
        for (let frame = 0; frame < 4; frame++) {
            Sprites.createSprite(`player_run_${frame}`, 60, 80, (ctx, w, h) => {
                const legOffset = Math.sin(frame * Math.PI / 2) * 8;
                
                // Body (slight lean)
                ctx.save();
                ctx.translate(30, 45);
                ctx.rotate(0.1);
                ctx.translate(-30, -45);
                
                // Body
                ctx.fillStyle = '#4ecdc4';
                ctx.beginPath();
                ctx.roundRect(10, 25, 40, 45, 8);
                ctx.fill();
                
                // Head
                ctx.fillStyle = '#ffeaa7';
                ctx.beginPath();
                ctx.arc(30, 18, 18, 0, Math.PI * 2);
                ctx.fill();
                
                // Hair
                ctx.fillStyle = '#2d3436';
                ctx.beginPath();
                ctx.moveTo(15, 12);
                ctx.lineTo(22, 0);
                ctx.lineTo(28, 10);
                ctx.lineTo(35, 2);
                ctx.lineTo(40, 12);
                ctx.lineTo(45, 5);
                ctx.lineTo(48, 20);
                ctx.lineTo(12, 20);
                ctx.closePath();
                ctx.fill();
                
                // Sunglasses
                ctx.fillStyle = '#1a1a2e';
                ctx.fillRect(18, 14, 10, 6);
                ctx.fillRect(32, 14, 10, 6);
                ctx.fillRect(28, 16, 4, 2);
                
                ctx.restore();
                
                // Arms (pumping)
                ctx.fillStyle = '#ffeaa7';
                ctx.save();
                ctx.translate(6, 35);
                ctx.rotate(Math.sin(frame * Math.PI / 2) * 0.5);
                ctx.fillRect(-3, 0, 10, 20);
                ctx.restore();
                
                ctx.save();
                ctx.translate(54, 35);
                ctx.rotate(-Math.sin(frame * Math.PI / 2) * 0.5);
                ctx.fillRect(-7, 0, 10, 20);
                ctx.restore();
                
                // Legs (running)
                ctx.fillStyle = '#636e72';
                ctx.fillRect(14, 68 + legOffset, 12, 10);
                ctx.fillRect(34, 68 - legOffset, 12, 10);
                
                // Shoes
                ctx.fillStyle = '#d63031';
                ctx.fillRect(12, 75 + legOffset, 15, 5);
                ctx.fillRect(33, 75 - legOffset, 15, 5);
            });
        }
    },
    
    createPlayerJump: () => {
        Sprites.createSprite('player_jump', 60, 80, (ctx, w, h) => {
            // Body (arms up)
            ctx.fillStyle = '#4ecdc4';
            ctx.beginPath();
            ctx.roundRect(10, 25, 40, 45, 8);
            ctx.fill();
            
            // Head
            ctx.fillStyle = '#ffeaa7';
            ctx.beginPath();
            ctx.arc(30, 18, 18, 0, Math.PI * 2);
            ctx.fill();
            
            // Hair
            ctx.fillStyle = '#2d3436';
            ctx.beginPath();
            ctx.moveTo(15, 12);
            ctx.lineTo(22, -3);
            ctx.lineTo(28, 8);
            ctx.lineTo(35, 0);
            ctx.lineTo(40, 10);
            ctx.lineTo(45, 2);
            ctx.lineTo(45, 20);
            ctx.lineTo(15, 20);
            ctx.closePath();
            ctx.fill();
            
            // Sunglasses
            ctx.fillStyle = '#1a1a2e';
            ctx.fillRect(18, 14, 10, 6);
            ctx.fillRect(32, 14, 10, 6);
            ctx.fillRect(28, 16, 4, 2);
            
            // Arms (raised)
            ctx.fillStyle = '#ffeaa7';
            ctx.save();
            ctx.translate(5, 30);
            ctx.rotate(-0.7);
            ctx.fillRect(0, -5, 10, 25);
            ctx.restore();
            
            ctx.save();
            ctx.translate(55, 30);
            ctx.rotate(0.7);
            ctx.fillRect(-10, -5, 10, 25);
            ctx.restore();
            
            // Legs (tucked)
            ctx.fillStyle = '#636e72';
            ctx.fillRect(18, 65, 10, 8);
            ctx.fillRect(32, 65, 10, 8);
            
            // Shoes
            ctx.fillStyle = '#d63031';
            ctx.fillRect(16, 70, 13, 5);
            ctx.fillRect(31, 70, 13, 5);
        });
    },
    
    createPlayerHurt: () => {
        Sprites.createSprite('player_hurt', 60, 80, (ctx, w, h) => {
            ctx.globalAlpha = 0.8;
            
            // Body (red tint)
            ctx.fillStyle = '#e74c3c';
            ctx.beginPath();
            ctx.roundRect(10, 25, 40, 45, 8);
            ctx.fill();
            
            // Head
            ctx.fillStyle = '#f39c12';
            ctx.beginPath();
            ctx.arc(30, 18, 18, 0, Math.PI * 2);
            ctx.fill();
            
            // Hair
            ctx.fillStyle = '#2d3436';
            ctx.beginPath();
            ctx.moveTo(15, 12);
            ctx.lineTo(22, 0);
            ctx.lineTo(28, 10);
            ctx.lineTo(35, 2);
            ctx.lineTo(40, 12);
            ctx.lineTo(45, 5);
            ctx.lineTo(45, 20);
            ctx.lineTo(15, 20);
            ctx.closePath();
            ctx.fill();
            
            // X eyes
            ctx.strokeStyle = '#1a1a2e';
            ctx.lineWidth = 3;
            ctx.beginPath();
            ctx.moveTo(20, 12);
            ctx.lineTo(28, 20);
            ctx.moveTo(28, 12);
            ctx.lineTo(20, 20);
            ctx.stroke();
            
            ctx.beginPath();
            ctx.moveTo(32, 12);
            ctx.lineTo(40, 20);
            ctx.moveTo(40, 12);
            ctx.lineTo(32, 20);
            ctx.stroke();
            
            // Sad mouth
            ctx.strokeStyle = '#d63031';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(30, 30, 6, Math.PI + 0.2, -0.2);
            ctx.stroke();
            
            // Arms
            ctx.fillStyle = '#f39c12';
            ctx.fillRect(2, 30, 10, 25);
            ctx.fillRect(48, 30, 10, 25);
            
            // Legs
            ctx.fillStyle = '#636e72';
            ctx.fillRect(14, 68, 12, 12);
            ctx.fillRect(34, 68, 12, 12);
            
            // Shoes
            ctx.fillStyle = '#d63031';
            ctx.fillRect(12, 75, 15, 5);
            ctx.fillRect(33, 75, 15, 5);
        });
    },
    
    // Enemy sprites
    createEnemy1: () => {
        // Rat enemy
        Sprites.createSprite('enemy_rat', 50, 40, (ctx, w, h) => {
            // Body
            ctx.fillStyle = '#636e72';
            ctx.beginPath();
            ctx.ellipse(25, 25, 20, 15, 0, 0, Math.PI * 2);
            ctx.fill();
            
            // Head
            ctx.fillStyle = '#636e72';
            ctx.beginPath();
            ctx.ellipse(45, 20, 10, 10, 0.3, 0, Math.PI * 2);
            ctx.fill();
            
            // Ears
            ctx.fillStyle = '#b2bec3';
            ctx.beginPath();
            ctx.arc(40, 8, 6, 0, Math.PI * 2);
            ctx.arc(50, 8, 6, 0, Math.PI * 2);
            ctx.fill();
            
            // Eyes (evil)
            ctx.fillStyle = '#e74c3c';
            ctx.beginPath();
            ctx.arc(45, 18, 3, 0, Math.PI * 2);
            ctx.fill();
            
            ctx.fillStyle = '#000';
            ctx.beginPath();
            ctx.arc(45, 18, 1.5, 0, Math.PI * 2);
            ctx.fill();
            
            // Nose
            ctx.fillStyle = '#d63031';
            ctx.beginPath();
            ctx.arc(50, 22, 3, 0, Math.PI * 2);
            ctx.fill();
            
            // Tail
            ctx.strokeStyle = '#b2bec3';
            ctx.lineWidth = 3;
            ctx.beginPath();
            ctx.moveTo(5, 25);
            ctx.quadraticCurveTo(-5, 15, 0, 5);
            ctx.stroke();
            
            // Legs
            ctx.fillStyle = '#b2bec3';
            ctx.fillRect(15, 35, 5, 5);
            ctx.fillRect(30, 35, 5, 5);
        });
    },
    
    createEnemy2: () => {
        // Thug enemy
        Sprites.createSprite('enemy_thug', 55, 70, (ctx, w, h) => {
            // Body
            ctx.fillStyle = '#2d3436';
            ctx.beginPath();
            ctx.roundRect(8, 25, 40, 35, 6);
            ctx.fill();
            
            // Head
            ctx.fillStyle = '#dfe6e9';
            ctx.beginPath();
            ctx.arc(28, 18, 16, 0, Math.PI * 2);
            ctx.fill();
            
            // Ski mask
            ctx.fillStyle = '#1a1a2e';
            ctx.beginPath();
            ctx.arc(28, 18, 16, 0, Math.PI * 2);
            ctx.fill();
            
            // Eye holes
            ctx.fillStyle = '#dfe6e9';
            ctx.fillRect(18, 12, 8, 8);
            ctx.fillRect(30, 12, 8, 8);
            
            // Evil eyes
            ctx.fillStyle = '#e74c3c';
            ctx.beginPath();
            ctx.arc(22, 16, 3, 0, Math.PI * 2);
            ctx.arc(34, 16, 3, 0, Math.PI * 2);
            ctx.fill();
            
            // Arms
            ctx.fillStyle = '#2d3436';
            ctx.fillRect(0, 28, 10, 25);
            ctx.fillRect(46, 28, 10, 25);
            
            // Fists
            ctx.fillStyle = '#dfe6e9';
            ctx.beginPath();
            ctx.arc(5, 55, 6, 0, Math.PI * 2);
            ctx.arc(51, 55, 6, 0, Math.PI * 2);
            ctx.fill();
            
            // Legs
            ctx.fillStyle = '#636e72';
            ctx.fillRect(12, 58, 12, 12);
            ctx.fillRect(32, 58, 12, 12);
        });
    },
    
    createEnemy3: () => {
        // Robot enemy
        Sprites.createSprite('enemy_robot', 60, 70, (ctx, w, h) => {
            // Body (metallic)
            ctx.fillStyle = '#74b9ff';
            ctx.beginPath();
            ctx.roundRect(10, 22, 40, 38, 5);
            ctx.fill();
            
            // Body details
            ctx.fillStyle = '#0984e3';
            ctx.fillRect(20, 30, 20, 10);
            ctx.beginPath();
            ctx.arc(30, 50, 8, 0, Math.PI * 2);
            ctx.fill();
            
            // Head
            ctx.fillStyle = '#dfe6e9';
            ctx.beginPath();
            ctx.roundRect(12, 2, 36, 22, 4);
            ctx.fill();
            
            // Antenna
            ctx.fillStyle = '#e74c3c';
            ctx.fillRect(28, -5, 4, 8);
            ctx.beginPath();
            ctx.arc(30, -5, 4, 0, Math.PI * 2);
            ctx.fill();
            
            // Eyes (LED)
            ctx.fillStyle = '#e74c3c';
            ctx.shadowColor = '#e74c3c';
            ctx.shadowBlur = 10;
            ctx.fillRect(18, 8, 10, 6);
            ctx.fillRect(32, 8, 10, 6);
            ctx.shadowBlur = 0;
            
            // Arms (mechanical)
            ctx.fillStyle = '#636e72';
            ctx.fillRect(2, 25, 10, 30);
            ctx.fillRect(48, 25, 10, 30);
            
            // Claws
            ctx.fillStyle = '#2d3436';
            ctx.beginPath();
            ctx.moveTo(2, 55);
            ctx.lineTo(-2, 65);
            ctx.lineTo(6, 65);
            ctx.closePath();
            ctx.fill();
            ctx.beginPath();
            ctx.moveTo(8, 55);
            ctx.lineTo(4, 65);
            ctx.lineTo(12, 65);
            ctx.closePath();
            ctx.fill();
            
            ctx.beginPath();
            ctx.moveTo(52, 55);
            ctx.lineTo(48, 65);
            ctx.lineTo(56, 65);
            ctx.closePath();
            ctx.fill();
            ctx.beginPath();
            ctx.moveTo(58, 55);
            ctx.lineTo(54, 65);
            ctx.lineTo(62, 65);
            ctx.closePath();
            ctx.fill();
            
            // Legs
            ctx.fillStyle = '#636e72';
            ctx.fillRect(15, 58, 10, 12);
            ctx.fillRect(35, 58, 10, 12);
        });
    },
    
    // Collectibles
    createCoin: () => {
        Sprites.createSprite('coin', 30, 30, (ctx, w, h) => {
            // Outer glow
            ctx.shadowColor = '#f39c12';
            ctx.shadowBlur = 10;
            
            // Coin
            ctx.fillStyle = '#f1c40f';
            ctx.beginPath();
            ctx.arc(15, 15, 12, 0, Math.PI * 2);
            ctx.fill();
            
            // Inner
            ctx.fillStyle = '#f39c12';
            ctx.beginPath();
            ctx.arc(15, 15, 9, 0, Math.PI * 2);
            ctx.fill();
            
            // Dollar sign
            ctx.fillStyle = '#f1c40f';
            ctx.font = 'bold 14px Arial';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText('$', 15, 15);
            
            ctx.shadowBlur = 0;
        });
    },
    
    createGem: () => {
        Sprites.createSprite('gem', 30, 30, (ctx, w, h) => {
            ctx.shadowColor = '#9b59b6';
            ctx.shadowBlur = 10;
            
            // Diamond shape
            ctx.fillStyle = '#9b59b6';
            ctx.beginPath();
            ctx.moveTo(15, 2);
            ctx.lineTo(28, 12);
            ctx.lineTo(15, 28);
            ctx.lineTo(2, 12);
            ctx.closePath();
            ctx.fill();
            
            // Shine
            ctx.fillStyle = 'rgba(255, 255, 255, 0.4)';
            ctx.beginPath();
            ctx.moveTo(15, 2);
            ctx.lineTo(22, 12);
            ctx.lineTo(15, 18);
            ctx.lineTo(8, 12);
            ctx.closePath();
            ctx.fill();
            
            ctx.shadowBlur = 0;
        });
    },
    
    // Power-ups
    createPowerUpSpeed: () => {
        Sprites.createSprite('powerup_speed', 40, 40, (ctx, w, h) => {
            ctx.shadowColor = '#00cec9';
            ctx.shadowBlur = 15;
            
            // Circle background
            ctx.fillStyle = '#00cec9';
            ctx.beginPath();
            ctx.arc(20, 20, 18, 0, Math.PI * 2);
            ctx.fill();
            
            // Lightning bolt
            ctx.fillStyle = '#fff';
            ctx.beginPath();
            ctx.moveTo(25, 5);
            ctx.lineTo(12, 20);
            ctx.lineTo(18, 20);
            ctx.lineTo(15, 35);
            ctx.lineTo(28, 18);
            ctx.lineTo(22, 18);
            ctx.closePath();
            ctx.fill();
            
            ctx.shadowBlur = 0;
        });
    },
    
    createPowerUpShield: () => {
        Sprites.createSprite('powerup_shield', 40, 40, (ctx, w, h) => {
            ctx.shadowColor = '#0984e3';
            ctx.shadowBlur = 15;
            
            // Shield shape
            ctx.fillStyle = '#0984e3';
            ctx.beginPath();
            ctx.moveTo(20, 5);
            ctx.lineTo(35, 12);
            ctx.lineTo(35, 22);
            ctx.quadraticCurveTo(35, 38, 20, 38);
            ctx.quadraticCurveTo(5, 38, 5, 22);
            ctx.lineTo(5, 12);
            ctx.closePath();
            ctx.fill();
            
            // Star
            ctx.fillStyle = '#fff';
            ctx.beginPath();
            ctx.moveTo(20, 12);
            ctx.lineTo(23, 18);
            ctx.lineTo(30, 19);
            ctx.lineTo(25, 24);
            ctx.lineTo(26, 31);
            ctx.lineTo(20, 27);
            ctx.lineTo(14, 31);
            ctx.lineTo(15, 24);
            ctx.lineTo(10, 19);
            ctx.lineTo(17, 18);
            ctx.closePath();
            ctx.fill();
            
            ctx.shadowBlur = 0;
        });
    },
    
    createPowerUpMagnet: () => {
        Sprites.createSprite('powerup_magnet', 40, 40, (ctx, w, h) => {
            ctx.shadowColor = '#e74c3c';
            ctx.shadowBlur = 15;
            
            // Circle
            ctx.fillStyle = '#e74c3c';
            ctx.beginPath();
            ctx.arc(20, 20, 18, 0, Math.PI * 2);
            ctx.fill();
            
            // Magnet U shape
            ctx.strokeStyle = '#fff';
            ctx.lineWidth = 5;
            ctx.lineCap = 'round';
            ctx.beginPath();
            ctx.arc(20, 18, 10, 0, Math.PI);
            ctx.stroke();
            
            // Poles
            ctx.fillStyle = '#e74c3c';
            ctx.fillRect(8, 8, 6, 12);
            ctx.fillRect(26, 8, 6, 12);
            
            ctx.fillStyle = '#fff';
            ctx.fillRect(8, 12, 6, 6);
            ctx.fillRect(26, 12, 6, 6);
            
            ctx.shadowBlur = 0;
        });
    },
    
    createPowerUpDoublePoints: () => {
        Sprites.createSprite('powerup_double', 40, 40, (ctx, w, h) => {
            ctx.shadowColor = '#f39c12';
            ctx.shadowBlur = 15;
            
            // Circle
            ctx.fillStyle = '#f39c12';
            ctx.beginPath();
            ctx.arc(20, 20, 18, 0, Math.PI * 2);
            ctx.fill();
            
            // x2 text
            ctx.fillStyle = '#fff';
            ctx.font = 'bold 18px Arial';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText('x2', 20, 20);
            
            ctx.shadowBlur = 0;
        });
    },
    
    // Platforms
    createPlatform: () => {
        Sprites.createSprite('platform', 120, 30, (ctx, w, h) => {
            // Main platform
            ctx.fillStyle = '#636e72';
            ctx.beginPath();
            ctx.roundRect(0, 5, 120, 25, 5);
            ctx.fill();
            
            // Top surface
            ctx.fillStyle = '#b2bec3';
            ctx.fillRect(5, 5, 110, 8);
            
            // Details
            ctx.fillStyle = '#2d3436';
            for (let i = 0; i < 4; i++) {
                ctx.fillRect(15 + i * 30, 18, 15, 5);
            }
        });
    },
    
    createPlatformDanger: () => {
        Sprites.createSprite('platform_danger', 120, 30, (ctx, w, h) => {
            // Main platform
            ctx.fillStyle = '#e74c3c';
            ctx.beginPath();
            ctx.roundRect(0, 5, 120, 25, 5);
            ctx.fill();
            
            // Warning stripes
            ctx.fillStyle = '#f39c12';
            for (let i = 0; i < 8; i++) {
                ctx.save();
                ctx.translate(i * 20, 0);
                ctx.beginPath();
                ctx.moveTo(0, 5);
                ctx.lineTo(10, 5);
                ctx.lineTo(20, 30);
                ctx.lineTo(10, 30);
                ctx.closePath();
                ctx.fill();
                ctx.restore();
            }
        });
    },
    
    createPlatformMoving: () => {
        Sprites.createSprite('platform_moving', 100, 25, (ctx, w, h) => {
            // Glowing platform
            ctx.shadowColor = '#00cec9';
            ctx.shadowBlur = 10;
            
            ctx.fillStyle = '#00cec9';
            ctx.beginPath();
            ctx.roundRect(0, 3, 100, 20, 5);
            ctx.fill();
            
            // Arrows
            ctx.fillStyle = '#fff';
            ctx.beginPath();
            ctx.moveTo(20, 13);
            ctx.lineTo(30, 8);
            ctx.lineTo(30, 18);
            ctx.closePath();
            ctx.fill();
            
            ctx.beginPath();
            ctx.moveTo(80, 13);
            ctx.lineTo(70, 8);
            ctx.lineTo(70, 18);
            ctx.closePath();
            ctx.fill();
            
            ctx.shadowBlur = 0;
        });
    },
    
    // Obstacles
    createSpikes: () => {
        Sprites.createSprite('spikes', 60, 30, (ctx, w, h) => {
            ctx.fillStyle = '#636e72';
            
            for (let i = 0; i < 4; i++) {
                ctx.beginPath();
                ctx.moveTo(i * 15, 30);
                ctx.lineTo(i * 15 + 7.5, 5);
                ctx.lineTo(i * 15 + 15, 30);
                ctx.closePath();
                ctx.fill();
            }
            
            // Tips
            ctx.fillStyle = '#b2bec3';
            for (let i = 0; i < 4; i++) {
                ctx.beginPath();
                ctx.moveTo(i * 15 + 5, 15);
                ctx.lineTo(i * 15 + 7.5, 5);
                ctx.lineTo(i * 15 + 10, 15);
                ctx.closePath();
                ctx.fill();
            }
        });
    },
    
    createBarrel: () => {
        Sprites.createSprite('barrel', 40, 50, (ctx, w, h) => {
            // Barrel body
            ctx.fillStyle = '#d35400';
            ctx.beginPath();
            ctx.ellipse(20, 45, 18, 8, 0, 0, Math.PI * 2);
            ctx.fill();
            
            ctx.fillStyle = '#e67e22';
            ctx.fillRect(2, 8, 36, 37);
            
            ctx.fillStyle = '#d35400';
            ctx.beginPath();
            ctx.ellipse(20, 8, 18, 8, 0, 0, Math.PI * 2);
            ctx.fill();
            
            // Metal bands
            ctx.fillStyle = '#636e72';
            ctx.fillRect(2, 15, 36, 4);
            ctx.fillRect(2, 35, 36, 4);
            
            // Hazard symbol
            ctx.fillStyle = '#2d3436';
            ctx.beginPath();
            ctx.arc(20, 25, 8, 0, Math.PI * 2);
            ctx.fill();
            ctx.fillStyle = '#f39c12';
            ctx.beginPath();
            ctx.moveTo(20, 17);
            ctx.lineTo(27, 30);
            ctx.lineTo(13, 30);
            ctx.closePath();
            ctx.fill();
        });
    },
    
    createTrash: () => {
        Sprites.createSprite('trash', 50, 60, (ctx, w, h) => {
            // Trash can body
            ctx.fillStyle = '#2d3436';
            ctx.beginPath();
            ctx.moveTo(5, 15);
            ctx.lineTo(10, 55);
            ctx.lineTo(40, 55);
            ctx.lineTo(45, 15);
            ctx.closePath();
            ctx.fill();
            
            // Lid
            ctx.fillStyle = '#636e72';
            ctx.beginPath();
            ctx.ellipse(25, 15, 22, 8, 0, 0, Math.PI * 2);
            ctx.fill();
            
            // Handle
            ctx.fillStyle = '#b2bec3';
            ctx.fillRect(20, 3, 10, 8);
            ctx.beginPath();
            ctx.arc(25, 5, 5, 0, Math.PI * 2);
            ctx.fill();
            
            // Lines on body
            ctx.strokeStyle = '#636e72';
            ctx.lineWidth = 2;
            for (let i = 0; i < 3; i++) {
                ctx.beginPath();
                ctx.moveTo(15 + i * 10, 20);
                ctx.lineTo(15 + i * 10, 50);
                ctx.stroke();
            }
        });
    },
    
    // Backgrounds
    createBackground1: () => {
        Sprites.createSprite('bg_downtown', 400, 800, (ctx, w, h) => {
            // Sky gradient
            const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
            skyGrad.addColorStop(0, '#1a1a2e');
            skyGrad.addColorStop(0.5, '#16213e');
            skyGrad.addColorStop(1, '#0f3460');
            ctx.fillStyle = skyGrad;
            ctx.fillRect(0, 0, w, h);
            
            // Stars
            ctx.fillStyle = '#fff';
            for (let i = 0; i < 50; i++) {
                const x = Math.random() * w;
                const y = Math.random() * h * 0.6;
                const size = Math.random() * 2;
                ctx.beginPath();
                ctx.arc(x, y, size, 0, Math.PI * 2);
                ctx.fill();
            }
            
            // Buildings (back layer)
            ctx.fillStyle = '#0f0f23';
            for (let i = 0; i < 8; i++) {
                const bw = 40 + Math.random() * 30;
                const bh = 200 + Math.random() * 300;
                const bx = i * 55;
                ctx.fillRect(bx, h - bh, bw, bh);
                
                // Windows
                ctx.fillStyle = '#f39c12';
                for (let wy = h - bh + 20; wy < h - 20; wy += 30) {
                    for (let wx = bx + 5; wx < bx + bw - 10; wx += 15) {
                        if (Math.random() > 0.3) {
                            ctx.fillRect(wx, wy, 8, 12);
                        }
                    }
                }
                ctx.fillStyle = '#0f0f23';
            }
        });
    },
    
    createBackground2: () => {
        Sprites.createSprite('bg_industrial', 400, 800, (ctx, w, h) => {
            // Sky gradient (smoggy)
            const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
            skyGrad.addColorStop(0, '#2d3436');
            skyGrad.addColorStop(0.5, '#636e72');
            skyGrad.addColorStop(1, '#b2bec3');
            ctx.fillStyle = skyGrad;
            ctx.fillRect(0, 0, w, h);
            
            // Smokestacks
            for (let i = 0; i < 4; i++) {
                const x = 50 + i * 100;
                ctx.fillStyle = '#2d3436';
                ctx.fillRect(x, h - 400, 30, 400);
                
                // Smoke
                ctx.fillStyle = 'rgba(100, 100, 100, 0.5)';
                for (let s = 0; s < 5; s++) {
                    ctx.beginPath();
                    ctx.arc(x + 15 + Math.random() * 20, h - 420 - s * 40, 20 + s * 10, 0, Math.PI * 2);
                    ctx.fill();
                }
            }
            
            // Factories
            ctx.fillStyle = '#1a1a1a';
            for (let i = 0; i < 6; i++) {
                const bw = 60 + Math.random() * 40;
                const bh = 150 + Math.random() * 100;
                const bx = i * 70;
                ctx.fillRect(bx, h - bh, bw, bh);
            }
            
            // Pipes
            ctx.strokeStyle = '#636e72';
            ctx.lineWidth = 8;
            for (let i = 0; i < 5; i++) {
                ctx.beginPath();
                ctx.moveTo(0, h - 100 - i * 80);
                ctx.lineTo(w, h - 120 - i * 80);
                ctx.stroke();
            }
        });
    },
    
    createBackground3: () => {
        Sprites.createSprite('bg_nightlife', 400, 800, (ctx, w, h) => {
            // Neon sky
            const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
            skyGrad.addColorStop(0, '#0a0a15');
            skyGrad.addColorStop(0.5, '#1a0a2e');
            skyGrad.addColorStop(1, '#2a1a3e');
            ctx.fillStyle = skyGrad;
            ctx.fillRect(0, 0, w, h);
            
            // Neon signs (glow effect)
            const neonColors = ['#ff6b6b', '#4ecdc4', '#ffe66d', '#9b59b6', '#ff9ff3'];
            ctx.shadowBlur = 20;
            
            for (let i = 0; i < 10; i++) {
                const x = Math.random() * w;
                const y = 100 + Math.random() * 400;
                ctx.shadowColor = neonColors[i % neonColors.length];
                ctx.fillStyle = neonColors[i % neonColors.length];
                ctx.fillRect(x, y, 30 + Math.random() * 50, 10);
            }
            
            // Buildings with neon
            ctx.shadowBlur = 0;
            ctx.fillStyle = '#0a0a15';
            for (let i = 0; i < 8; i++) {
                const bw = 45 + Math.random() * 35;
                const bh = 250 + Math.random() * 250;
                const bx = i * 55;
                ctx.fillRect(bx, h - bh, bw, bh);
                
                // Neon windows
                ctx.shadowBlur = 10;
                for (let wy = h - bh + 20; wy < h - 20; wy += 35) {
                    for (let wx = bx + 5; wx < bx + bw - 10; wx += 18) {
                        const color = neonColors[Math.floor(Math.random() * neonColors.length)];
                        ctx.shadowColor = color;
                        ctx.fillStyle = color;
                        ctx.fillRect(wx, wy, 10, 15);
                    }
                }
                ctx.shadowBlur = 0;
            }
        });
    },
    
    // Effects
    createParticle: () => {
        Sprites.createSprite('particle', 10, 10, (ctx, w, h) => {
            ctx.fillStyle = '#fff';
            ctx.beginPath();
            ctx.arc(5, 5, 4, 0, Math.PI * 2);
            ctx.fill();
        });
    },
    
    createStarBurst: () => {
        Sprites.createSprite('starburst', 60, 60, (ctx, w, h) => {
            ctx.fillStyle = '#ffe66d';
            ctx.shadowColor = '#ffe66d';
            ctx.shadowBlur = 15;
            
            ctx.beginPath();
            for (let i = 0; i < 8; i++) {
                const angle = (i * Math.PI * 2) / 8;
                const innerRadius = 10;
                const outerRadius = 25;
                
                const outerX = 30 + Math.cos(angle) * outerRadius;
                const outerY = 30 + Math.sin(angle) * outerRadius;
                const innerX = 30 + Math.cos(angle + Math.PI / 8) * innerRadius;
                const innerY = 30 + Math.sin(angle + Math.PI / 8) * innerRadius;
                
                if (i === 0) {
                    ctx.moveTo(outerX, outerY);
                } else {
                    ctx.lineTo(outerX, outerY);
                }
                ctx.lineTo(innerX, innerY);
            }
            ctx.closePath();
            ctx.fill();
            
            ctx.shadowBlur = 0;
        });
    }
};

// Export for use
window.Sprites = Sprites;
