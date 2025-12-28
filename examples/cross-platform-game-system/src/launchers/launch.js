#!/usr/bin/env node
/**
 * Launch Game CLI
 */

const { program } = require('commander');
const chalk = require('chalk');
const GameSystem = require('../index');

async function launch(gameName, options) {
  const system = new GameSystem();
  await system.initialize();
  await system.launch(gameName);
}

program
  .argument('<game>', 'Game name or ID')
  .option('--layer <layer>', 'Override compatibility layer')
  .parse();

const [gameName] = program.args;
const options = program.opts();

launch(gameName, options).catch(err => {
  console.error(chalk.red('Error:'), err.message);
  process.exit(1);
});
