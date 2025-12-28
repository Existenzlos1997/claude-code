/**
 * Game Profiles
 * Vordefinierte Konfigurationen für beliebte Spiele
 * Predefined configurations for popular games
 */

const gameProfiles = {
  // EasyAntiCheat Games
  'apex-legends': {
    name: 'Apex Legends',
    platform: 'windows',
    compatibilityLayer: 'proton',
    antiCheat: 'eac',
    recommended: {
      protonVersion: 'proton-experimental',
      dxvk: true,
      esync: true,
      fsync: true
    },
    notes: 'Works well with Proton Experimental. EAC support enabled.'
  },

  'dead-by-daylight': {
    name: 'Dead by Daylight',
    platform: 'windows',
    compatibilityLayer: 'proton',
    antiCheat: 'eac',
    recommended: {
      protonVersion: 'proton-experimental',
      dxvk: true,
      esync: true
    },
    notes: 'Requires Proton Experimental with EAC runtime.'
  },

  'rust': {
    name: 'Rust',
    platform: 'windows',
    compatibilityLayer: 'proton',
    antiCheat: 'eac',
    recommended: {
      protonVersion: 'proton-experimental',
      dxvk: true,
      esync: true,
      fsync: true
    },
    notes: 'Works with EAC support. May need PROTON_USE_WINED3D=0.'
  },

  // BattlEye Games
  'rainbow-six-siege': {
    name: 'Rainbow Six Siege',
    platform: 'windows',
    compatibilityLayer: 'proton',
    antiCheat: 'battleye',
    recommended: {
      protonVersion: 'proton-experimental',
      dxvk: true,
      vkd3d: true
    },
    notes: 'Requires BattlEye runtime. Check ProtonDB for latest status.'
  },

  'arma-3': {
    name: 'ARMA 3',
    platform: 'windows',
    compatibilityLayer: 'proton',
    antiCheat: 'battleye',
    recommended: {
      protonVersion: 'proton-ge',
      dxvk: true,
      largeAddressAware: true
    },
    notes: 'Works best with Proton GE. Enable LAA for better performance.'
  },

  'destiny-2': {
    name: 'Destiny 2',
    platform: 'windows',
    compatibilityLayer: 'proton',
    antiCheat: 'battleye',
    recommended: {
      protonVersion: 'proton-experimental',
      dxvk: true,
      vkd3d: true
    },
    notes: 'Officially supported on Steam Deck. Works with BattlEye.'
  },

  // No Anti-Cheat / VAC
  'witcher-3': {
    name: 'The Witcher 3',
    platform: 'windows',
    compatibilityLayer: 'proton',
    antiCheat: null,
    recommended: {
      protonVersion: 'proton-ge',
      dxvk: true,
      esync: true,
      fsync: true
    },
    notes: 'Runs perfectly. Consider using mods for enhanced graphics.'
  },

  'cyberpunk-2077': {
    name: 'Cyberpunk 2077',
    platform: 'windows',
    compatibilityLayer: 'proton',
    antiCheat: null,
    recommended: {
      protonVersion: 'proton-ge',
      dxvk: true,
      vkd3d: true,
      esync: true,
      fsync: true
    },
    notes: 'Excellent performance with Proton GE. Enable FSR for better FPS.'
  },

  'elden-ring': {
    name: 'Elden Ring',
    platform: 'windows',
    compatibilityLayer: 'proton',
    antiCheat: 'eac',
    recommended: {
      protonVersion: 'proton-experimental',
      dxvk: true,
      esync: true
    },
    notes: 'Works well with EAC support. May need PROTON_USE_WINED3D=0.'
  },

  // Native Linux Games
  'dota-2': {
    name: 'Dota 2',
    platform: 'native',
    compatibilityLayer: 'native',
    antiCheat: 'vac',
    recommended: {},
    notes: 'Native Linux version available. Runs perfectly.'
  },

  'counter-strike-2': {
    name: 'Counter-Strike 2',
    platform: 'native',
    compatibilityLayer: 'native',
    antiCheat: 'vac',
    recommended: {},
    notes: 'Native Linux support. VAC works natively.'
  },

  // Incompatible (for reference)
  'valorant': {
    name: 'Valorant',
    platform: 'windows',
    compatibilityLayer: null,
    antiCheat: 'vanguard',
    recommended: null,
    compatible: false,
    notes: 'Not compatible. Riot Vanguard requires kernel-level access. Use Windows or dual-boot.'
  }
};

/**
 * Get profile for a game
 */
function getProfile(gameNameOrId) {
  const id = gameNameOrId.toLowerCase().replace(/\s+/g, '-');
  return gameProfiles[id] || null;
}

/**
 * List all available profiles
 */
function listProfiles() {
  return Object.keys(gameProfiles).map(id => ({
    id,
    name: gameProfiles[id].name,
    compatible: gameProfiles[id].compatible !== false
  }));
}

/**
 * Search profiles by name
 */
function searchProfiles(query) {
  const lowerQuery = query.toLowerCase();
  return Object.keys(gameProfiles)
    .filter(id => {
      const profile = gameProfiles[id];
      return profile.name.toLowerCase().includes(lowerQuery) || id.includes(lowerQuery);
    })
    .map(id => ({
      id,
      ...gameProfiles[id]
    }));
}

/**
 * Apply profile to game
 */
function applyProfile(game, profileId) {
  const profile = getProfile(profileId);
  if (!profile) {
    return null;
  }

  return {
    ...game,
    compatibilityLayer: profile.compatibilityLayer,
    antiCheat: profile.antiCheat,
    settings: profile.recommended,
    notes: profile.notes
  };
}

module.exports = {
  gameProfiles,
  getProfile,
  listProfiles,
  searchProfiles,
  applyProfile
};
