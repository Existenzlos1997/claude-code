# Gameplay Scripts

This directory contains high-level gameplay management scripts that make it easy to set up and test the game.

## Scripts Overview

### 🚁 AircraftSpawner.cs
Flexible system for spawning aircraft (player or AI) with customizable settings.

**Features:**
- Multiple spawn modes: OnStart, OnTrigger, Continuous, WavesBased
- Configurable team types: Player, Friendly, Enemy, Neutral
- AI behavior types: Patrol, Guard, Attack, Escort, Intercept
- Placeholder aircraft creation if no prefab available
- Visual gizmos in editor for spawn points

**Usage:**
1. Add `AircraftSpawner` component to a GameObject
2. Configure spawn settings in inspector
3. Set team type and AI behavior
4. Optionally assign spawn points
5. Play - aircraft will spawn automatically!

**Example:**
```csharp
// Spawn an enemy aircraft manually
GameObject enemy = spawner.SpawnAircraft();

// Trigger spawn from code
spawner.TriggerSpawn();

// Despawn all aircraft
spawner.DespawnAll();
```

---

### 🎮 GameModeManager.cs
Manages different game modes and scenarios for quick setup.

**Supported Modes:**
- **FreeRoam**: Explore freely with no objectives
- **Combat**: Dogfight mode with enemy aircraft
- **Mission**: Play assigned missions
- **Training**: Tutorial and practice mode
- **Multiplayer**: PvP or co-op gameplay
- **Campaign**: Story-driven missions

**Usage:**
1. Add `GameModeManager` to scene
2. Select desired game mode in inspector
3. Configure mode settings
4. Enable autoStartOnLoad or switch manually

**Example:**
```csharp
// Switch mode during gameplay
gameModeManager.SwitchToCombat();
gameModeManager.SwitchToMission();
```

---

### 🌍 SimpleWorldSetup.cs
Creates a basic playable world with essential elements for testing.

**Creates:**
- Directional lighting (sun)
- Skybox and fog
- Ground terrain plane
- Player spawn point
- Enemy spawn points
- Navigation checkpoints

**Usage:**
1. Add `SimpleWorldSetup` to an empty scene
2. Configure what to create (lighting, terrain, etc.)
3. Set world size and enemy count
4. Press Play - instant playable world!

**Perfect for:**
- Quick prototyping
- Testing new features
- Demo scenes
- Tutorial levels

---

### 🛠️ DeveloperTools.cs
Debug and testing tools for developers.

**Features:**
- Performance monitoring (FPS counter)
- Time scale controls (speed up/slow down)
- Quick spawn/destroy enemies
- Scene reload
- Cheat codes support
- On-screen GUI menu

**Controls:**
- **F1**: Toggle developer menu
- **Ctrl + Plus**: Speed up time
- **Ctrl + Minus**: Slow down time
- **Ctrl + 0**: Reset time to normal

**Usage:**
1. Add `DeveloperTools` to scene
2. Press F1 during gameplay to toggle menu
3. Use GUI buttons or keyboard shortcuts

---

## Quick Start Examples

### Example 1: Basic Combat Scene

```csharp
// Add these to an empty scene:
1. SimpleWorldSetup (creates world)
2. GameModeManager (set to Combat mode)
3. DeveloperTools (for testing)
```

### Example 2: Custom Spawn Setup

```csharp
// Create GameObject with AircraftSpawner
var spawner = gameObject.AddComponent<AircraftSpawner>();
spawner.spawnMode = SpawnMode.Continuous;
spawner.team = TeamType.Enemy;
spawner.maxConcurrentSpawns = 10;
spawner.respawnDelay = 30f;
```

### Example 3: Training Mode

```csharp
// Set up training scene
var gameModeManager = FindObjectOfType<GameModeManager>();
gameModeManager.SwitchToTraining();
```

---

## Integration with Other Systems

These gameplay scripts work seamlessly with other EarthUnderFreelancer systems:

- **AI System**: AircraftSpawner configures AI behavior automatically
- **Combat System**: Spawned aircraft have team settings for targeting
- **Mission System**: GameModeManager integrates with mission objectives
- **UI System**: Developer tools can display custom info

---

## Tips & Best Practices

### For Scene Setup
1. Start with `SimpleWorldSetup` for instant playability
2. Use `GameModeManager` to define gameplay type
3. Add `DeveloperTools` for easy debugging

### For Testing
1. Enable DeveloperTools on startup
2. Use time controls to speed up long scenarios
3. Spawn extra enemies to test combat

### For Production
1. Disable DeveloperTools in builds
2. Configure AircraftSpawner for specific scenarios
3. Use GameModeManager for menu integration

---

## Future Enhancements

Potential additions to this module:
- [ ] Save/load spawn configurations
- [ ] Advanced AI waypoint editor
- [ ] Multiplayer lobby integration
- [ ] Campaign mission sequencing
- [ ] Performance profiler integration

---

## Need Help?

See main documentation:
- `EarthUnderFreelancer/README.md` - Full project overview
- `Documentation/COMPLETE_GUIDE.md` - Developer guide
- `QUICK_START.md` - Getting started guide
