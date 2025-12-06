# Urban Scum - Mobile Game

![GDevelop](https://img.shields.io/badge/GDevelop-5.3+-blue?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Android-green?style=flat-square)
![Version](https://img.shields.io/badge/Version-1.0.0-orange?style=flat-square)

## 🎮 About the Game

**Urban Scum** is an action-packed mobile platformer game set in a vibrant urban environment. Navigate through challenging levels, collect coins, defeat enemies, and become the ultimate urban champion!

### Features

- 🏃 **3 Unique Levels** - Each with increasing difficulty
- 💰 **Coin Collection System** - Collect coins to boost your score
- ⚡ **Power-Ups** - Speed boost, shield protection, coin magnet, and double coins
- 🏆 **Highscore System** - Compete against yourself for the best score
- 💾 **Progress Saving** - Your level progress and highscores are saved automatically
- 📺 **AdMob Integration** - Interstitial and rewarded ads ready to implement
- 🎨 **Cartoon Style Graphics** - Modern, colorful UI and visuals

## 📱 Screenshots

The game features:
- Colorful menu screen with animated logo
- Level selection with locked/unlocked states
- Dynamic gameplay with platform mechanics
- Game over screen with restart and ad options
- Level complete celebration with star ratings

## 🚀 Getting Started

### Prerequisites

- [GDevelop 5](https://gdevelop.io/) (version 5.3 or higher)
- Android SDK (for Android export)
- AdMob account (for ad integration)

### Installation

1. Download and install GDevelop 5 from [gdevelop.io](https://gdevelop.io/)
2. Open GDevelop and select "Open a project"
3. Navigate to the `urban-scum-game` folder and open `game.json`
4. The game will load with all scenes, assets, and events

### Running the Game

1. In GDevelop, click the "Preview" button (play icon) in the toolbar
2. Select "Start preview" to test the game in your browser
3. For mobile testing, use the "Export" function

## 📤 Exporting for Android

### Export Steps

1. In GDevelop, go to **File > Export**
2. Select **Android (& iOS)**
3. Choose **Manual build (with Cordova)**
4. Configure the following settings:
   - Package name: `com.urbanscum.game`
   - Version: `1.0.0`
   - Orientation: `Portrait`
5. Click **Export** and follow the build instructions

### AdMob Setup

Before exporting for production, update the AdMob IDs in the game:

1. Open `game.json` in a text editor
2. Search for `ca-app-pub-XXXXXXXXXXXXXXXX/XXXXXXXXXX`
3. Replace with your actual AdMob ad unit IDs:
   - Interstitial Ad ID
   - Rewarded Ad ID

## 🛒 Play Store Submission

### App Details

**Title:** Urban Scum - Urban Runner

**Short Description:**
Run, jump, and conquer the urban jungle! Collect coins, power-ups, and defeat enemies in this addictive platformer.

**Full Description:**
🏃 URBAN SCUM - The Ultimate Urban Adventure! 🏃

Dive into the vibrant world of Urban Scum, where every jump counts and every coin matters!

⭐ FEATURES:
• 3 Challenging Levels with unique urban environments
• Smooth platformer controls optimized for mobile
• Exciting power-ups: Speed Boost, Shield, Coin Magnet, Double Coins
• Global highscore system - compete against your best!
• Beautiful cartoon-style graphics
• Addictive gameplay that keeps you coming back

🎮 HOW TO PLAY:
• Tap left side of screen to move left
• Tap right side of screen to move right
• Tap upper area to jump
• Collect coins and avoid enemies
• Reach the goal to complete each level

💡 PRO TIPS:
• Use power-ups strategically to maximize your score
• Shield power-up protects you from one enemy hit
• Speed boost helps you reach tricky platforms
• Watch rewarded ads for extra lives!

Download now and prove you're the ultimate Urban Scum champion!

### Keywords

```
urban runner, platformer, mobile game, arcade, jumping game, coin collector, 
endless runner, action game, casual game, cartoon game, adventure, 
urban adventure, street runner, city game, 2D platformer
```

### Graphics Requirements

| Asset | Size | Format |
|-------|------|--------|
| App Icon | 512x512 | PNG |
| Feature Graphic | 1024x500 | PNG |
| Screenshots | 1080x1920 | PNG (portrait) |
| Promo Video | - | YouTube URL (optional) |

### Content Rating

- **ESRB:** Everyone
- **PEGI:** 3
- **Content Descriptors:** Mild Fantasy Violence

## 📁 Project Structure

```
urban-scum-game/
├── game.json              # Main GDevelop project file
├── README.md              # This documentation
├── assets/
│   ├── sprites/           # Character and object sprites
│   │   ├── player_*.png   # Player animations
│   │   ├── enemy_*.png    # Enemy sprites
│   │   ├── coin.png       # Collectible coin
│   │   ├── powerup_*.png  # Power-up items
│   │   └── platform.png   # Platform tiles
│   ├── backgrounds/       # Level backgrounds
│   │   ├── bg_level1.png  # City night theme
│   │   ├── bg_level2.png  # Neon city theme
│   │   ├── bg_level3.png  # Space city theme
│   │   └── bg_menu.png    # Menu background
│   ├── ui/                # User interface elements
│   │   ├── btn_*.png      # Button assets
│   │   ├── icon_*.png     # Icon assets
│   │   ├── logo.png       # Game logo
│   │   └── level_*.png    # Level selection icons
│   └── audio/             # Sound effects and music
└── docs/                  # Additional documentation
```

## 🎮 Game Scenes

| Scene | Description |
|-------|-------------|
| StartScreen | Main menu with Play and Level Select buttons |
| LevelSelect | Level selection with locked/unlocked states |
| Level1 | First level - City Night |
| Level2 | Second level - Neon City |
| Level3 | Third level - Space City |
| GameOver | Game over screen with restart/home options |
| LevelComplete | Victory screen with star rating |
| PauseMenu | In-game pause screen |

## 🔧 Customization

### Adding New Levels

1. In GDevelop, right-click on an existing level scene
2. Select "Duplicate"
3. Rename to `Level4` (or next number)
4. Modify the level layout in the scene editor
5. Update the background image reference
6. Add the new level button in `LevelSelect` scene

### Modifying Power-Ups

Power-up durations and effects can be modified in the level events:
- Speed Boost: Increases player max speed to 800
- Shield: Protects from one enemy hit
- Power-up timer is set to 10 seconds by default

### Changing Game Variables

Global variables can be modified in the project settings:
- `Lives`: Starting lives (default: 3)
- `Score`: Starting score (default: 0)
- `PowerUpTimer`: Power-up duration in seconds

## 📋 Technical Specifications

- **Resolution:** 480x800 (portrait)
- **Target FPS:** 60
- **Scale Mode:** Linear
- **Orientation:** Portrait only
- **Min Android Version:** 5.0 (API 21)

## 🐛 Troubleshooting

### Common Issues

**Game doesn't load:**
- Ensure GDevelop 5.3+ is installed
- Check that all asset files are in the correct folders

**Ads not showing:**
- Replace placeholder AdMob IDs with your actual IDs
- Ensure AdMob Cordova plugin is properly configured
- Test ads may take a few minutes to load initially

**Controls not responsive:**
- Check that touch events are properly configured
- Ensure no UI elements are blocking the touch areas

## 📄 License

This game project is provided as-is for educational and commercial purposes. 
The placeholder assets included can be replaced with your own custom graphics.

## 🤝 Support

For questions, issues, or feature requests:
- Create an issue in the repository
- Check the GDevelop documentation at [wiki.gdevelop.io](https://wiki.gdevelop.io/)

---

Made with ❤️ using GDevelop
