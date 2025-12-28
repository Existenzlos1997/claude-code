#!/usr/bin/env node
/**
 * Game Profiles CLI
 * Zeigt und wendet Spiel-Profile an
 * Shows and applies game profiles
 */

const { program } = require('commander');
const chalk = require('chalk');
const {
  getProfile,
  listProfiles,
  searchProfiles
} = require('../src/core/game-profiles');

async function showProfile(gameId) {
  const profile = getProfile(gameId);

  if (!profile) {
    console.log(chalk.red(`Profile not found for: ${gameId}`));
    console.log(chalk.gray('\nUse "npm run list-profiles" to see available profiles'));
    return;
  }

  console.log(chalk.blue('\n🎮 Game Profile\n'));
  console.log(chalk.white(`Name: ${profile.name}`));
  console.log(chalk.gray(`Platform: ${profile.platform}`));
  console.log(chalk.gray(`Compatibility Layer: ${profile.compatibilityLayer || 'N/A'}`));
  
  if (profile.antiCheat) {
    console.log(chalk.yellow(`Anti-Cheat: ${profile.antiCheat.toUpperCase()}`));
  }

  if (profile.compatible === false) {
    console.log(chalk.red('\n⚠️  NOT COMPATIBLE'));
  } else {
    console.log(chalk.green('\n✓ Compatible'));
  }

  if (profile.recommended) {
    console.log(chalk.blue('\nRecommended Settings:'));
    Object.entries(profile.recommended).forEach(([key, value]) => {
      console.log(chalk.gray(`  ${key}: ${value}`));
    });
  }

  if (profile.notes) {
    console.log(chalk.blue('\nNotes:'));
    console.log(chalk.gray(`  ${profile.notes}`));
  }

  console.log();
}

async function listAll() {
  const profiles = listProfiles();

  console.log(chalk.blue('\n🎮 Available Game Profiles\n'));

  const compatible = profiles.filter(p => p.compatible);
  const incompatible = profiles.filter(p => !p.compatible);

  if (compatible.length > 0) {
    console.log(chalk.green('Compatible Games:'));
    compatible.forEach(p => {
      console.log(chalk.white(`  • ${p.name} (${p.id})`));
    });
  }

  if (incompatible.length > 0) {
    console.log(chalk.red('\nIncompatible Games:'));
    incompatible.forEach(p => {
      console.log(chalk.gray(`  • ${p.name} (${p.id})`));
    });
  }

  console.log(chalk.gray(`\nTotal: ${profiles.length} profiles`));
  console.log(chalk.gray('Use "npm run show-profile <game-id>" for details\n'));
}

async function search(query) {
  const results = searchProfiles(query);

  if (results.length === 0) {
    console.log(chalk.yellow(`No profiles found for: ${query}`));
    return;
  }

  console.log(chalk.blue(`\n🔍 Search Results for "${query}"\n`));

  results.forEach(profile => {
    const status = profile.compatible === false ? 
      chalk.red('✗ Incompatible') : 
      chalk.green('✓ Compatible');
    
    console.log(`${status} ${chalk.white(profile.name)} (${profile.id})`);
    if (profile.notes) {
      console.log(chalk.gray(`  ${profile.notes}`));
    }
    console.log();
  });
}

program
  .name('game-profiles')
  .description('Manage game profiles');

program
  .command('list')
  .description('List all available profiles')
  .action(listAll);

program
  .command('show <game-id>')
  .description('Show profile details')
  .action(showProfile);

program
  .command('search <query>')
  .description('Search for profiles')
  .action(search);

program.parse();

if (program.args.length === 0) {
  listAll();
}
