# Examples Directory

This directory contains examples and templates for using Claude Code.

## Available Examples

### 1. New Project Template
**Location:** `new-project-template/`

A complete starter template for creating new Node.js projects. This template includes:
- Basic project structure with `src/` and `tests/` directories
- Package configuration (`package.json`)
- Git ignore patterns (`.gitignore`)
- Environment variables example (`.env.example`)
- Documentation (README.md, CONTRIBUTING.md, LICENSE)
- Sample code and test files

**Usage:**
```bash
# Copy the template to your new project location
cp -r examples/new-project-template /path/to/your-new-project

# Navigate to your new project
cd /path/to/your-new-project

# Customize the template
# - Edit package.json with your project details
# - Update README.md with your project information
# - Modify src/index.js with your application code

# Install dependencies (if any)
npm install

# Run your project
npm start
```

### 2. Cross-Platform Game System
**Location:** `cross-platform-game-system/`

Ein System zur plattformübergreifenden Ausführung von PC-Spielen / A system for running PC games across platforms (Windows, Linux, etc.).

This advanced example demonstrates:
- Wine and Proton integration for Windows game compatibility
- Plugin architecture for compatibility layers
- Game library management
- Automatic game detection
- Multi-platform support

**Features:**
- Run Windows games on Linux using Wine or Proton
- Support for native games
- Automatic detection of Steam games
- Extensible plugin system
- CLI and (planned) GUI interface

**Usage:**
```bash
# Copy and set up the system
cp -r examples/cross-platform-game-system /path/to/your-location
cd /path/to/your-location

# Install dependencies
npm install

# Run setup
npm run setup

# Detect games
npm run detect-games

# Launch a game
npm start launch "Game Name"
```

**Requirements:**
- Node.js 16+
- Wine (for Windows games on Linux)
- Optional: Proton, winetricks

See the project's README for detailed documentation in German and English.

### 3. Hooks
**Location:** `hooks/`

Examples of Claude Code hooks for customizing behavior.

- **bash_command_validator_example.py** - Pre-tool use hook that validates bash commands

## Creating Your Own Examples

If you have created a useful example or template that could benefit others:
1. Add it to this `examples/` directory
2. Document it in this README.md
3. Include clear instructions for usage
4. Add comments to explain key concepts

## More Resources

- [Claude Code Documentation](https://docs.anthropic.com/en/docs/claude-code/overview)
- [Official Repository](https://github.com/anthropics/claude-code)
