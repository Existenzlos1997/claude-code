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

### 2. Hooks
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
