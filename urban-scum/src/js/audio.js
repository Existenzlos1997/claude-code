/**
 * Urban Scum - Audio Manager
 */

const Audio = {
    context: null,
    sounds: {},
    music: null,
    musicVolume: 0.5,
    sfxVolume: 0.7,
    enabled: {
        sound: true,
        music: true
    },
    
    // Initialize audio context
    init: () => {
        try {
            const AudioContext = window.AudioContext || window.webkitAudioContext;
            Audio.context = new AudioContext();
            
            // Load settings
            const settings = Storage.getSettings();
            Audio.enabled.sound = settings.sound;
            Audio.enabled.music = settings.music;
            
            // Create sounds programmatically
            Audio.createSounds();
            
            console.log('Audio initialized');
        } catch (e) {
            console.warn('Audio not supported:', e);
        }
    },
    
    // Resume audio context (needed for mobile browsers)
    resume: () => {
        if (Audio.context && Audio.context.state === 'suspended') {
            Audio.context.resume();
        }
    },
    
    // Create all game sounds programmatically
    createSounds: () => {
        // Jump sound - short rising tone
        Audio.sounds.jump = () => Audio.playTone(400, 0.1, 'sine', 0, 600);
        
        // Collect coin/item sound
        Audio.sounds.collect = () => Audio.playTone(800, 0.1, 'sine', 0, 1200);
        
        // Power-up sound - arpeggio
        Audio.sounds.powerup = () => {
            Audio.playTone(400, 0.1, 'sine', 0);
            Audio.playTone(500, 0.1, 'sine', 0.1);
            Audio.playTone(600, 0.1, 'sine', 0.2);
            Audio.playTone(800, 0.2, 'sine', 0.3);
        };
        
        // Hit/damage sound
        Audio.sounds.hit = () => Audio.playTone(150, 0.15, 'square', 0, 80);
        
        // Enemy defeat sound
        Audio.sounds.defeat = () => {
            Audio.playTone(600, 0.1, 'square', 0, 200);
            Audio.playTone(400, 0.1, 'square', 0.1, 150);
        };
        
        // Game over sound
        Audio.sounds.gameover = () => {
            Audio.playTone(400, 0.2, 'sine', 0, 300);
            Audio.playTone(300, 0.2, 'sine', 0.2, 200);
            Audio.playTone(200, 0.4, 'sine', 0.4, 100);
        };
        
        // Level complete sound
        Audio.sounds.levelcomplete = () => {
            [523, 659, 784, 1047].forEach((freq, i) => {
                Audio.playTone(freq, 0.2, 'sine', i * 0.15);
            });
        };
        
        // Button click sound
        Audio.sounds.click = () => Audio.playTone(600, 0.05, 'sine');
        
        // Combo sound
        Audio.sounds.combo = () => Audio.playTone(1000, 0.1, 'sine', 0, 1500);
    },
    
    // Play a synthesized tone
    playTone: (frequency, duration, type = 'sine', delay = 0, endFreq = null) => {
        if (!Audio.context || !Audio.enabled.sound) return;
        
        try {
            Audio.resume();
            
            const oscillator = Audio.context.createOscillator();
            const gainNode = Audio.context.createGain();
            
            oscillator.type = type;
            oscillator.frequency.setValueAtTime(frequency, Audio.context.currentTime + delay);
            
            if (endFreq) {
                oscillator.frequency.linearRampToValueAtTime(
                    endFreq, 
                    Audio.context.currentTime + delay + duration
                );
            }
            
            gainNode.gain.setValueAtTime(Audio.sfxVolume, Audio.context.currentTime + delay);
            gainNode.gain.exponentialRampToValueAtTime(
                0.01, 
                Audio.context.currentTime + delay + duration
            );
            
            oscillator.connect(gainNode);
            gainNode.connect(Audio.context.destination);
            
            oscillator.start(Audio.context.currentTime + delay);
            oscillator.stop(Audio.context.currentTime + delay + duration + 0.1);
        } catch (e) {
            console.warn('Error playing tone:', e);
        }
    },
    
    // Play a sound effect by name
    play: (name) => {
        if (Audio.sounds[name] && Audio.enabled.sound) {
            Audio.sounds[name]();
        }
    },
    
    // Start background music loop
    startMusic: () => {
        if (!Audio.context || !Audio.enabled.music) return;
        
        Audio.stopMusic();
        Audio.playBackgroundMusic();
    },
    
    // Play simple background music
    playBackgroundMusic: () => {
        if (!Audio.enabled.music || !Audio.context) return;
        
        const notes = [262, 294, 330, 349, 392, 440, 494, 523];
        let noteIndex = 0;
        
        const playNote = () => {
            if (!Audio.enabled.music) return;
            
            const oscillator = Audio.context.createOscillator();
            const gainNode = Audio.context.createGain();
            
            oscillator.type = 'sine';
            oscillator.frequency.value = notes[noteIndex % notes.length];
            
            gainNode.gain.setValueAtTime(Audio.musicVolume * 0.3, Audio.context.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.01, Audio.context.currentTime + 0.4);
            
            oscillator.connect(gainNode);
            gainNode.connect(Audio.context.destination);
            
            oscillator.start();
            oscillator.stop(Audio.context.currentTime + 0.4);
            
            noteIndex++;
        };
        
        Audio.music = setInterval(playNote, 500);
    },
    
    // Stop background music
    stopMusic: () => {
        if (Audio.music) {
            clearInterval(Audio.music);
            Audio.music = null;
        }
    },
    
    // Toggle sound effects
    toggleSound: (enabled) => {
        Audio.enabled.sound = enabled;
        Storage.saveSetting('sound', enabled);
    },
    
    // Toggle music
    toggleMusic: (enabled) => {
        Audio.enabled.music = enabled;
        Storage.saveSetting('music', enabled);
        
        if (enabled) {
            Audio.startMusic();
        } else {
            Audio.stopMusic();
        }
    }
};

// Export for use
window.Audio = Audio;
