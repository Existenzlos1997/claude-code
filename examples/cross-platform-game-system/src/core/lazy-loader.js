/**
 * Lazy Loader - Load modules on-demand to reduce startup time
 */

class LazyLoader {
  constructor() {
    this.modules = new Map();
    this.loaded = new Map();
  }

  /**
   * Register a module for lazy loading
   */
  register(name, loader) {
    this.modules.set(name, loader);
  }

  /**
   * Load a module
   */
  async load(name) {
    if (this.loaded.has(name)) {
      return this.loaded.get(name);
    }

    const loader = this.modules.get(name);
    if (!loader) {
      throw new Error(`Module "${name}" not registered for lazy loading`);
    }

    try {
      const module = await Promise.resolve(loader());
      this.loaded.set(name, module);
      return module;
    } catch (error) {
      throw new Error(`Failed to load module "${name}": ${error.message}`);
    }
  }

  /**
   * Check if module is loaded
   */
  isLoaded(name) {
    return this.loaded.has(name);
  }

  /**
   * Unload a module
   */
  unload(name) {
    this.loaded.delete(name);
  }

  /**
   * Get all registered modules
   */
  getRegistered() {
    return Array.from(this.modules.keys());
  }

  /**
   * Get all loaded modules
   */
  getLoaded() {
    return Array.from(this.loaded.keys());
  }
}

module.exports = LazyLoader;
