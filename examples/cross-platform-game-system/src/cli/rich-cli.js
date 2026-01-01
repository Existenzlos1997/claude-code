/**
 * Rich CLI - Enhanced command-line interface with progress bars, spinners, and interactive prompts
 */

class RichCLI {
  constructor(options = {}) {
    this.options = {
      colors: options.colors !== false,
      interactive: options.interactive !== false,
      ...options
    };

    this.colors = {
      reset: '\x1b[0m',
      bright: '\x1b[1m',
      dim: '\x1b[2m',
      red: '\x1b[31m',
      green: '\x1b[32m',
      yellow: '\x1b[33m',
      blue: '\x1b[34m',
      cyan: '\x1b[36m'
    };

    this.progressChars = {
      complete: '█',
      incomplete: '░'
    };
  }

  colorize(text, color) {
    if (!this.options.colors) return text;
    return `${this.colors[color]}${text}${this.colors.reset}`;
  }

  success(message) {
    console.log(`${this.colorize('✓', 'green')} ${message}`);
  }

  error(message) {
    console.error(`${this.colorize('✗', 'red')} ${message}`);
  }

  warning(message) {
    console.warn(`${this.colorize('⚠', 'yellow')} ${message}`);
  }

  info(message) {
    console.log(`${this.colorize('ℹ', 'blue')} ${message}`);
  }

  createProgressBar(total = 100) {
    let current = 0;
    const width = 40;

    const render = (value, message = '') => {
      const percent = Math.floor((value / total) * 100);
      const filled = Math.floor((value / total) * width);
      const empty = width - filled;

      const bar = 
        this.colorize(this.progressChars.complete.repeat(filled), 'green') +
        this.colorize(this.progressChars.incomplete.repeat(empty), 'dim');

      process.stdout.write(`\r${bar} ${percent}% ${message}`.padEnd(80));
    };

    return {
      update: (value, message) => {
        current = value;
        render(current, message);
      },
      complete: (message = 'Done') => {
        render(total, message);
        process.stdout.write('\n');
      }
    };
  }

  async withProgress(message, callback) {
    console.log(this.colorize(message, 'cyan'));
    const progress = this.createProgressBar(100);
    
    try {
      await callback(progress);
      progress.complete();
    } catch (error) {
      process.stdout.write('\n');
      throw error;
    }
  }

  createSpinner(message) {
    const frames = ['⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏'];
    let frameIndex = 0;
    let interval = null;

    const render = () => {
      const frame = this.colorize(frames[frameIndex], 'cyan');
      process.stdout.write(`\r${frame} ${message}`.padEnd(80));
      frameIndex = (frameIndex + 1) % frames.length;
    };

    return {
      start: () => {
        interval = setInterval(render, 80);
      },
      stop: (finalMessage) => {
        if (interval) {
          clearInterval(interval);
          interval = null;
        }
        process.stdout.write('\r' + ' '.repeat(80) + '\r');
        if (finalMessage) {
          this.success(finalMessage);
        }
      }
    };
  }

  async withSpinner(message, callback) {
    const spinner = this.createSpinner(message);
    spinner.start();
    
    try {
      const result = await callback();
      spinner.stop(message + ' - Done');
      return result;
    } catch (error) {
      spinner.stop();
      this.error(message + ' - Failed');
      throw error;
    }
  }

  table(rows, headers) {
    if (rows.length === 0) {
      this.info('No data to display');
      return;
    }

    const keys = headers || Object.keys(rows[0]);
    const widths = keys.map(key => {
      const values = rows.map(row => String(row[key] || ''));
      const maxContentWidth = Math.max(...values.map(v => v.length));
      return Math.max(key.length, maxContentWidth);
    });

    const topBorder = '┌' + widths.map(w => '─'.repeat(w + 2)).join('┬') + '┐';
    console.log(this.colorize(topBorder, 'dim'));

    const headerRow = '│' + keys.map((key, i) => 
      ` ${this.colorize(key.padEnd(widths[i]), 'bright')} `
    ).join('│') + '│';
    console.log(headerRow);

    const midBorder = '├' + widths.map(w => '─'.repeat(w + 2)).join('┼') + '┤';
    console.log(this.colorize(midBorder, 'dim'));

    rows.forEach(row => {
      const dataRow = '│' + keys.map((key, i) => 
        ` ${String(row[key] || '').padEnd(widths[i])} `
      ).join('│') + '│';
      console.log(dataRow);
    });

    const bottomBorder = '└' + widths.map(w => '─'.repeat(w + 2)).join('┴') + '┘';
    console.log(this.colorize(bottomBorder, 'dim'));
  }

  async confirm(message, defaultValue = false) {
    if (!this.options.interactive) {
      return defaultValue;
    }

    const defaultText = defaultValue ? 'Y/n' : 'y/N';
    const question = `${this.colorize('?', 'cyan')} ${message} (${defaultText}): `;
    
    return new Promise((resolve) => {
      process.stdout.write(question);
      process.stdin.once('data', (data) => {
        const answer = data.toString().trim().toLowerCase();
        if (answer === '') {
          resolve(defaultValue);
        } else {
          resolve(answer === 'y' || answer === 'yes');
        }
      });
    });
  }

  list(items, options = {}) {
    const { symbol = '•', indent = 2 } = options;
    const prefix = ' '.repeat(indent);
    
    items.forEach(item => {
      console.log(`${prefix}${this.colorize(symbol, 'cyan')} ${item}`);
    });
  }

  box(title, content, options = {}) {
    const { width = 60, color = 'cyan' } = options;
    const lines = Array.isArray(content) ? content : [content];

    console.log(this.colorize('┌─ ' + title + ' ' + '─'.repeat(Math.max(0, width - title.length - 4)) + '┐', color));

    lines.forEach(line => {
      const padding = ' '.repeat(Math.max(0, width - line.length - 2));
      console.log(this.colorize('│', color) + ' ' + line + padding + ' ' + this.colorize('│', color));
    });

    console.log(this.colorize('└' + '─'.repeat(width) + '┘', color));
  }

  divider(char = '─', length = 60) {
    console.log(this.colorize(char.repeat(length), 'dim'));
  }
}

module.exports = RichCLI;
