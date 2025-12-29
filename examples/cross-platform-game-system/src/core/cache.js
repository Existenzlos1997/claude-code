/**
 * Cache - LRU cache with TTL support
 */

class Cache {
  constructor(options = {}) {
    this.maxSize = options.maxSize || 100;
    this.defaultTTL = options.ttl || 3600; // 1 hour default
    this.cache = new Map();
    this.stats = {
      hits: 0,
      misses: 0
    };
    this.evictionCallbacks = [];
  }

  /**
   * Set cache entry
   */
  set(key, value, options = {}) {
    const ttl = options.ttl || this.defaultTTL;
    const expiresAt = Date.now() + (ttl * 1000);

    // Evict if at max size
    if (this.cache.size >= this.maxSize && !this.cache.has(key)) {
      this._evictOldest();
    }

    this.cache.set(key, {
      value,
      expiresAt,
      accessedAt: Date.now()
    });
  }

  /**
   * Get cache entry
   */
  get(key) {
    const entry = this.cache.get(key);

    if (!entry) {
      this.stats.misses++;
      return null;
    }

    // Check TTL
    if (Date.now() > entry.expiresAt) {
      this.cache.delete(key);
      this.stats.misses++;
      return null;
    }

    // Update access time (LRU)
    entry.accessedAt = Date.now();
    this.cache.set(key, entry);

    this.stats.hits++;
    return entry.value;
  }

  /**
   * Check if key exists and is valid
   */
  has(key) {
    const entry = this.cache.get(key);
    if (!entry) return false;
    
    if (Date.now() > entry.expiresAt) {
      this.cache.delete(key);
      return false;
    }

    return true;
  }

  /**
   * Delete cache entry
   */
  delete(key) {
    return this.cache.delete(key);
  }

  /**
   * Clear all cache
   */
  clear() {
    this.cache.clear();
    this.stats.hits = 0;
    this.stats.misses = 0;
  }

  /**
   * Get cache statistics
   */
  getStats() {
    const total = this.stats.hits + this.stats.misses;
    const hitRate = total > 0 ? (this.stats.hits / total * 100).toFixed(1) : 0;

    return {
      hits: this.stats.hits,
      misses: this.stats.misses,
      hitRate: parseFloat(hitRate),
      size: this.cache.size,
      maxSize: this.maxSize
    };
  }

  /**
   * Evict oldest entry (LRU)
   */
  _evictOldest() {
    let oldestKey = null;
    let oldestTime = Infinity;

    for (const [key, entry] of this.cache.entries()) {
      if (entry.accessedAt < oldestTime) {
        oldestTime = entry.accessedAt;
        oldestKey = key;
      }
    }

    if (oldestKey) {
      const value = this.cache.get(oldestKey).value;
      this.cache.delete(oldestKey);
      this._notifyEviction(oldestKey, value);
    }
  }

  /**
   * Register eviction callback
   */
  onEvict(callback) {
    this.evictionCallbacks.push(callback);
  }

  /**
   * Notify eviction callbacks
   */
  _notifyEviction(key, value) {
    this.evictionCallbacks.forEach(callback => {
      try {
        callback(key, value);
      } catch (error) {
        console.error('Eviction callback error:', error);
      }
    });
  }

  /**
   * Clean expired entries
   */
  cleanExpired() {
    const now = Date.now();
    const expiredKeys = [];

    for (const [key, entry] of this.cache.entries()) {
      if (now > entry.expiresAt) {
        expiredKeys.push(key);
      }
    }

    expiredKeys.forEach(key => this.cache.delete(key));
    
    return expiredKeys.length;
  }
}

module.exports = Cache;
