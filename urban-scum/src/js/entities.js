/**
 * Urban Scum - Game Entities
 * Player, Enemies, Collectibles, Power-ups, Platforms
 */

// Base Entity class
class Entity {
    constructor(x, y, width, height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.velocityX = 0;
        this.velocityY = 0;
        this.active = true;
        this.sprite = null;
    }
    
    update(deltaTime) {
        this.x += this.velocityX * deltaTime;
        this.y += this.velocityY * deltaTime;
    }
    
    draw(ctx, cameraX = 0, cameraY = 0) {
        if (this.sprite) {
            ctx.drawImage(this.sprite, this.x - cameraX, this.y - cameraY, this.width, this.height);
        }
    }
    
    getBounds() {
        return {
            x: this.x,
            y: this.y,
            width: this.width,
            height: this.height
        };
    }
    
    collidesWith(other) {
        return Utils.rectCollision(this.getBounds(), other.getBounds());
    }
}

// Player class
class Player extends Entity {
    constructor(x, y) {
        super(x, y, 50, 70);
        this.speed = 300;
        this.jumpForce = -600;
        this.gravity = 1800;
        this.isGrounded = false;
        this.isJumping = false;
        this.direction = 1; // 1 = right, -1 = left
        this.lives = 3;
        this.isInvincible = false;
        this.invincibleTimer = 0;
        this.animFrame = 0;
        this.animTimer = 0;
        this.state = 'idle'; // idle, running, jumping, hurt
        
        // Power-up states
        this.hasShield = false;
        this.hasSpeedBoost = false;
        this.hasMagnet = false;
        this.hasDoublePoints = false;
        this.powerUpTimers = {
            shield: 0,
            speed: 0,
            magnet: 0,
            doublePoints: 0
        };
        
        this.sprite = Sprites.get('player');
    }
    
    update(deltaTime, input, platforms) {
        // Handle input
        if (input.left) {
            this.velocityX = -this.speed * (this.hasSpeedBoost ? 1.5 : 1);
            this.direction = -1;
            this.state = 'running';
        } else if (input.right) {
            this.velocityX = this.speed * (this.hasSpeedBoost ? 1.5 : 1);
            this.direction = 1;
            this.state = 'running';
        } else {
            this.velocityX = 0;
            if (this.isGrounded) {
                this.state = 'idle';
            }
        }
        
        // Jump
        if (input.jump && this.isGrounded) {
            this.velocityY = this.jumpForce;
            this.isGrounded = false;
            this.isJumping = true;
            this.state = 'jumping';
            Audio.play('jump');
            Utils.vibrate(10);
        }
        
        // Apply gravity
        this.velocityY += this.gravity * deltaTime;
        
        // Limit fall speed
        if (this.velocityY > 1000) {
            this.velocityY = 1000;
        }
        
        // Update position
        this.x += this.velocityX * deltaTime;
        this.y += this.velocityY * deltaTime;
        
        // Platform collision
        this.isGrounded = false;
        platforms.forEach(platform => {
            if (this.checkPlatformCollision(platform)) {
                this.isGrounded = true;
                this.isJumping = false;
            }
        });
        
        // Update animation
        this.animTimer += deltaTime;
        if (this.animTimer >= 0.1) {
            this.animTimer = 0;
            this.animFrame = (this.animFrame + 1) % 4;
        }
        
        // Update sprite based on state
        this.updateSprite();
        
        // Update invincibility
        if (this.isInvincible) {
            this.invincibleTimer -= deltaTime;
            if (this.invincibleTimer <= 0) {
                this.isInvincible = false;
            }
        }
        
        // Update power-up timers
        this.updatePowerUps(deltaTime);
        
        // Keep player in bounds
        if (this.x < 0) this.x = 0;
    }
    
    checkPlatformCollision(platform) {
        const playerBottom = this.y + this.height;
        const platformTop = platform.y;
        
        if (this.velocityY > 0 &&
            playerBottom >= platformTop &&
            playerBottom <= platformTop + 20 &&
            this.x + this.width > platform.x &&
            this.x < platform.x + platform.width) {
            this.y = platformTop - this.height;
            this.velocityY = 0;
            return true;
        }
        return false;
    }
    
    updateSprite() {
        switch (this.state) {
            case 'running':
                this.sprite = Sprites.get(`player_run_${this.animFrame}`) || Sprites.get('player');
                break;
            case 'jumping':
                this.sprite = Sprites.get('player_jump') || Sprites.get('player');
                break;
            case 'hurt':
                this.sprite = Sprites.get('player_hurt') || Sprites.get('player');
                break;
            default:
                this.sprite = Sprites.get('player');
        }
    }
    
    updatePowerUps(deltaTime) {
        Object.keys(this.powerUpTimers).forEach(key => {
            if (this.powerUpTimers[key] > 0) {
                this.powerUpTimers[key] -= deltaTime;
                if (this.powerUpTimers[key] <= 0) {
                    this[`has${key.charAt(0).toUpperCase() + key.slice(1)}`] = false;
                }
            }
        });
    }
    
    takeDamage() {
        if (this.isInvincible) return false;
        
        if (this.hasShield) {
            this.hasShield = false;
            this.powerUpTimers.shield = 0;
            Audio.play('hit');
            return false;
        }
        
        this.lives--;
        this.isInvincible = true;
        this.invincibleTimer = 2;
        this.state = 'hurt';
        Audio.play('hit');
        Utils.vibrate([50, 30, 50]);
        
        return this.lives <= 0;
    }
    
    collectPowerUp(type) {
        const duration = 10; // seconds
        switch (type) {
            case 'speed':
                this.hasSpeedBoost = true;
                this.powerUpTimers.speed = duration;
                break;
            case 'shield':
                this.hasShield = true;
                this.powerUpTimers.shield = duration;
                break;
            case 'magnet':
                this.hasMagnet = true;
                this.powerUpTimers.magnet = duration;
                break;
            case 'double':
                this.hasDoublePoints = true;
                this.powerUpTimers.doublePoints = duration;
                break;
        }
        Audio.play('powerup');
    }
    
    draw(ctx, cameraX = 0, cameraY = 0) {
        // Blinking when invincible
        if (this.isInvincible && Math.floor(this.invincibleTimer * 10) % 2 === 0) {
            return;
        }
        
        ctx.save();
        
        // Flip sprite based on direction
        if (this.direction === -1) {
            ctx.translate(this.x - cameraX + this.width, this.y - cameraY);
            ctx.scale(-1, 1);
            ctx.drawImage(this.sprite, 0, 0, this.width, this.height);
        } else {
            ctx.drawImage(this.sprite, this.x - cameraX, this.y - cameraY, this.width, this.height);
        }
        
        // Draw shield effect
        if (this.hasShield) {
            ctx.strokeStyle = '#0984e3';
            ctx.lineWidth = 3;
            ctx.shadowColor = '#0984e3';
            ctx.shadowBlur = 15;
            ctx.beginPath();
            if (this.direction === -1) {
                ctx.ellipse(this.width / 2, this.height / 2, this.width / 2 + 10, this.height / 2 + 5, 0, 0, Math.PI * 2);
            } else {
                ctx.ellipse(this.x - cameraX + this.width / 2, this.y - cameraY + this.height / 2, this.width / 2 + 10, this.height / 2 + 5, 0, 0, Math.PI * 2);
            }
            ctx.stroke();
        }
        
        ctx.restore();
    }
}

// Enemy base class
class Enemy extends Entity {
    constructor(x, y, width, height, type = 'rat') {
        super(x, y, width, height);
        this.type = type;
        this.speed = 100;
        this.direction = -1;
        this.patrolRange = 200;
        this.startX = x;
        this.points = 100;
        this.animFrame = 0;
        this.animTimer = 0;
        
        this.setTypeProperties();
    }
    
    setTypeProperties() {
        switch (this.type) {
            case 'rat':
                this.width = 50;
                this.height = 40;
                this.speed = 80;
                this.points = 50;
                this.sprite = Sprites.get('enemy_rat');
                break;
            case 'thug':
                this.width = 55;
                this.height = 70;
                this.speed = 60;
                this.points = 100;
                this.sprite = Sprites.get('enemy_thug');
                break;
            case 'robot':
                this.width = 60;
                this.height = 70;
                this.speed = 120;
                this.points = 200;
                this.sprite = Sprites.get('enemy_robot');
                break;
        }
    }
    
    update(deltaTime, player) {
        // Patrol movement
        this.x += this.speed * this.direction * deltaTime;
        
        // Reverse at patrol range
        if (this.x <= this.startX - this.patrolRange) {
            this.direction = 1;
        } else if (this.x >= this.startX + this.patrolRange) {
            this.direction = -1;
        }
        
        // Animation
        this.animTimer += deltaTime;
        if (this.animTimer >= 0.2) {
            this.animTimer = 0;
            this.animFrame = (this.animFrame + 1) % 2;
        }
    }
    
    draw(ctx, cameraX = 0, cameraY = 0) {
        ctx.save();
        
        // Flip based on direction
        if (this.direction === 1) {
            ctx.translate(this.x - cameraX + this.width, this.y - cameraY);
            ctx.scale(-1, 1);
            ctx.drawImage(this.sprite, 0, 0, this.width, this.height);
        } else {
            ctx.drawImage(this.sprite, this.x - cameraX, this.y - cameraY, this.width, this.height);
        }
        
        ctx.restore();
    }
}

// Collectible class
class Collectible extends Entity {
    constructor(x, y, type = 'coin') {
        super(x, y, 30, 30);
        this.type = type;
        this.collected = false;
        this.bobOffset = Math.random() * Math.PI * 2;
        this.bobSpeed = 3;
        this.baseY = y;
        
        this.setTypeProperties();
    }
    
    setTypeProperties() {
        switch (this.type) {
            case 'coin':
                this.points = 10;
                this.sprite = Sprites.get('coin');
                break;
            case 'gem':
                this.points = 50;
                this.sprite = Sprites.get('gem');
                break;
        }
    }
    
    update(deltaTime, gameTime) {
        // Bobbing animation
        this.y = this.baseY + Math.sin(gameTime * this.bobSpeed + this.bobOffset) * 5;
    }
    
    collect(player) {
        if (this.collected) return 0;
        this.collected = true;
        this.active = false;
        Audio.play('collect');
        return this.points * (player.hasDoublePoints ? 2 : 1);
    }
}

// PowerUp class
class PowerUp extends Entity {
    constructor(x, y, type = 'speed') {
        super(x, y, 40, 40);
        this.type = type;
        this.collected = false;
        this.bobOffset = Math.random() * Math.PI * 2;
        this.rotationSpeed = 2;
        this.rotation = 0;
        
        this.setTypeProperties();
    }
    
    setTypeProperties() {
        switch (this.type) {
            case 'speed':
                this.sprite = Sprites.get('powerup_speed');
                this.name = 'SPEED BOOST';
                break;
            case 'shield':
                this.sprite = Sprites.get('powerup_shield');
                this.name = 'SHIELD';
                break;
            case 'magnet':
                this.sprite = Sprites.get('powerup_magnet');
                this.name = 'MAGNET';
                break;
            case 'double':
                this.sprite = Sprites.get('powerup_double');
                this.name = 'DOUBLE POINTS';
                break;
        }
    }
    
    update(deltaTime, gameTime) {
        // Floating animation
        this.rotation += this.rotationSpeed * deltaTime;
    }
    
    collect(player) {
        if (this.collected) return;
        this.collected = true;
        this.active = false;
        player.collectPowerUp(this.type);
        return this.name;
    }
    
    draw(ctx, cameraX = 0, cameraY = 0) {
        ctx.save();
        ctx.translate(this.x - cameraX + this.width / 2, this.y - cameraY + this.height / 2);
        
        // Pulsing scale effect
        const scale = 1 + Math.sin(this.rotation * 2) * 0.1;
        ctx.scale(scale, scale);
        
        ctx.drawImage(this.sprite, -this.width / 2, -this.height / 2, this.width, this.height);
        ctx.restore();
    }
}

// Platform class
class Platform extends Entity {
    constructor(x, y, width = 120, type = 'normal') {
        super(x, y, width, 30);
        this.type = type;
        this.isMoving = type === 'moving';
        this.isDanger = type === 'danger';
        this.moveRange = 150;
        this.moveSpeed = 100;
        this.moveDirection = 1;
        this.startX = x;
        
        this.setTypeProperties();
    }
    
    setTypeProperties() {
        switch (this.type) {
            case 'normal':
                this.sprite = Sprites.get('platform');
                break;
            case 'danger':
                this.sprite = Sprites.get('platform_danger');
                break;
            case 'moving':
                this.sprite = Sprites.get('platform_moving');
                break;
        }
    }
    
    update(deltaTime) {
        if (this.isMoving) {
            this.x += this.moveSpeed * this.moveDirection * deltaTime;
            
            if (this.x <= this.startX - this.moveRange) {
                this.moveDirection = 1;
            } else if (this.x >= this.startX + this.moveRange) {
                this.moveDirection = -1;
            }
        }
    }
    
    draw(ctx, cameraX = 0, cameraY = 0) {
        // Repeat sprite for longer platforms
        const spriteWidth = 120;
        let drawX = this.x - cameraX;
        let remaining = this.width;
        
        while (remaining > 0) {
            const drawWidth = Math.min(spriteWidth, remaining);
            ctx.drawImage(this.sprite, 0, 0, drawWidth, 30, drawX, this.y - cameraY, drawWidth, 30);
            drawX += drawWidth;
            remaining -= drawWidth;
        }
    }
}

// Obstacle class
class Obstacle extends Entity {
    constructor(x, y, type = 'spikes') {
        super(x, y, 60, 30);
        this.type = type;
        
        this.setTypeProperties();
    }
    
    setTypeProperties() {
        switch (this.type) {
            case 'spikes':
                this.width = 60;
                this.height = 30;
                this.sprite = Sprites.get('spikes');
                break;
            case 'barrel':
                this.width = 40;
                this.height = 50;
                this.sprite = Sprites.get('barrel');
                break;
            case 'trash':
                this.width = 50;
                this.height = 60;
                this.sprite = Sprites.get('trash');
                break;
        }
    }
}

// Particle effect
class Particle {
    constructor(x, y, color = '#fff') {
        this.x = x;
        this.y = y;
        this.velocityX = (Math.random() - 0.5) * 200;
        this.velocityY = (Math.random() - 0.5) * 200;
        this.size = 3 + Math.random() * 5;
        this.color = color;
        this.life = 1;
        this.decay = 2 + Math.random();
        this.active = true;
    }
    
    update(deltaTime) {
        this.x += this.velocityX * deltaTime;
        this.y += this.velocityY * deltaTime;
        this.life -= this.decay * deltaTime;
        
        if (this.life <= 0) {
            this.active = false;
        }
    }
    
    draw(ctx, cameraX = 0, cameraY = 0) {
        ctx.globalAlpha = this.life;
        ctx.fillStyle = this.color;
        ctx.beginPath();
        ctx.arc(this.x - cameraX, this.y - cameraY, this.size, 0, Math.PI * 2);
        ctx.fill();
        ctx.globalAlpha = 1;
    }
}

// Export classes
window.Entity = Entity;
window.Player = Player;
window.Enemy = Enemy;
window.Collectible = Collectible;
window.PowerUp = PowerUp;
window.Platform = Platform;
window.Obstacle = Obstacle;
window.Particle = Particle;
