# Troubleshooting Guide / Fehlerbehebung

## Deutsch

### Häufige Probleme und Lösungen

#### 1. Spiel startet nicht

**Problem**: Das Spiel startet nicht oder stürzt sofort ab.

**Lösungen**:
```bash
# Überprüfen Sie die Logs
cat ~/.game-system/logs/game-system-*.log

# Validieren Sie das System
npm run validate

# Überprüfen Sie Anti-Cheat-Kompatibilität
npm run check-anticheat "Spielname"

# Versuchen Sie einen anderen Proton-Version
# Bearbeiten Sie die Spiel-Einstellungen und ändern Sie compatibilityLayer
```

#### 2. Wine/Proton nicht gefunden

**Problem**: `Proton not found` oder `Wine not found` Fehler.

**Lösungen**:
```bash
# Linux (Ubuntu/Debian)
sudo apt update
sudo apt install wine-stable winetricks

# Arch Linux
sudo pacman -S wine winetricks

# Für Proton: Installieren Sie Steam
# Steam wird Proton automatisch herunterladen
```

#### 3. Schlechte Performance

**Problem**: Das Spiel läuft langsam oder ruckelt.

**Lösungen**:
```bash
# Aktivieren Sie DXVK (DirectX zu Vulkan)
# In config/settings.json:
{
  "wine": {
    "dxvk": true,
    "vkd3d": true
  }
}

# Aktivieren Sie Esync/Fsync
export PROTON_NO_ESYNC=0
export PROTON_NO_FSYNC=0

# Verwenden Sie Proton GE für bessere Performance
# Download von: https://github.com/GloriousEggroll/proton-ge-custom
```

#### 4. Anti-Cheat-Fehler

**Problem**: Anti-Cheat verhindert den Spielstart.

**Lösungen**:
```bash
# Überprüfen Sie, ob Anti-Cheat unterstützt wird
npm run check-anticheat "Spielname"

# Für EasyAntiCheat:
# Stellen Sie sicher, dass Proton EAC Runtime installiert ist
ls ~/.steam/steam/steamapps/common/Proton\ EasyAntiCheat\ Runtime/

# Für BattlEye:
ls ~/.steam/steam/steamapps/common/Proton\ BattlEye\ Runtime/

# Falls nicht vorhanden, installieren Sie über Steam
```

#### 5. Audio-Probleme

**Problem**: Kein Ton oder verzerrter Sound.

**Lösungen**:
```bash
# Überprüfen Sie PulseAudio/PipeWire
pactl info

# Wine Audio-Treiber ändern
winecfg
# Audio-Tab -> Treiber auswählen

# ALSA verwenden (falls PulseAudio Probleme macht)
export WINE_ALSA_DRIVER=1
```

---

## English

### Common Problems and Solutions

#### 1. Game Won't Start

**Problem**: Game doesn't start or crashes immediately.

**Solutions**:
```bash
# Check the logs
cat ~/.game-system/logs/game-system-*.log

# Validate the system
npm run validate

# Check anti-cheat compatibility
npm run check-anticheat "Game Name"

# Try a different Proton version
# Edit game settings and change compatibilityLayer
```

#### 2. Wine/Proton Not Found

**Problem**: `Proton not found` or `Wine not found` error.

**Solutions**:
```bash
# Linux (Ubuntu/Debian)
sudo apt update
sudo apt install wine-stable winetricks

# Arch Linux
sudo pacman -S wine winetricks

# For Proton: Install Steam
# Steam will download Proton automatically
```

#### 3. Poor Performance

**Problem**: Game runs slowly or stutters.

**Solutions**:
```bash
# Enable DXVK (DirectX to Vulkan)
# In config/settings.json:
{
  "wine": {
    "dxvk": true,
    "vkd3d": true
  }
}

# Enable Esync/Fsync
export PROTON_NO_ESYNC=0
export PROTON_NO_FSYNC=0

# Use Proton GE for better performance
# Download from: https://github.com/GloriousEggroll/proton-ge-custom
```

#### 4. Anti-Cheat Errors

**Problem**: Anti-cheat prevents game from starting.

**Solutions**:
```bash
# Check if anti-cheat is supported
npm run check-anticheat "Game Name"

# For EasyAntiCheat:
# Make sure Proton EAC Runtime is installed
ls ~/.steam/steam/steamapps/common/Proton\ EasyAntiCheat\ Runtime/

# For BattlEye:
ls ~/.steam/steam/steamapps/common/Proton\ BattlEye\ Runtime/

# If not present, install via Steam
```

#### 5. Audio Issues

**Problem**: No sound or distorted audio.

**Solutions**:
```bash
# Check PulseAudio/PipeWire
pactl info

# Change Wine audio driver
winecfg
# Audio tab -> Select driver

# Use ALSA (if PulseAudio has issues)
export WINE_ALSA_DRIVER=1
```

## Performance Optimization / Performance-Optimierung

### Für bessere FPS / For Better FPS

```bash
# Deaktivieren Sie Compositor (KDE/XFCE)
# Disable compositor (KDE/XFCE)
export __GL_YIELD="USLEEP"

# Nutzen Sie Gamemode
# Use Gamemode
sudo apt install gamemode
gamemoderun npm start launch "Game"

# CPU Governor auf Performance
# CPU Governor to Performance
sudo cpupower frequency-set -g performance

# GPU-Übertaktung (NVIDIA)
# GPU Overclocking (NVIDIA)
nvidia-settings
```

### Speicher-Optimierung / Memory Optimization

```bash
# Erhöhen Sie VRAM-Limit für Wine
# Increase VRAM limit for Wine
export WINE_D3D12_VRAM_SIZE=8192  # 8GB

# Nutzen Sie Zram
# Use Zram
sudo apt install zram-tools
```

## Debug-Modus / Debug Mode

```bash
# Aktivieren Sie detailliertes Logging
# Enable detailed logging
export LOG_LEVEL=debug
export ENABLE_FILE_LOGGING=true
npm start launch "Game"

# Wine Debug-Output
export WINEDEBUG=+all
wine game.exe 2>&1 | tee wine-debug.log

# Proton Debug
export PROTON_LOG=1
```

## Nützliche Befehle / Useful Commands

```bash
# System-Informationen
# System Information
npm run validate

# Spiel-Profile auflisten
# List game profiles
npm run list-profiles

# Performance-Bericht
# Performance Report
npm run performance-report "Game"

# Logs anzeigen
# View logs
tail -f ~/.game-system/logs/game-system-*.log
```

## Bekannte Einschränkungen / Known Limitations

1. **Riot Vanguard**: Nicht kompatibel / Not compatible
2. **FACEIT Anti-Cheat**: Nicht kompatibel / Not compatible
3. **DirectX 12**: Experimentell via VKD3D / Experimental via VKD3D
4. **HDR**: Begrenzte Unterstützung / Limited support

## Weitere Hilfe / Additional Help

- [ProtonDB](https://www.protondb.com/) - Spiele-Kompatibilitätsdatenbank
- [WineHQ AppDB](https://appdb.winehq.org/) - Wine-Anwendungsdatenbank
- [Are We Anti-Cheat Yet?](https://areweanticheatyet.com/) - Anti-Cheat-Status
- [r/linux_gaming](https://reddit.com/r/linux_gaming) - Community-Support

## Support kontaktieren / Contact Support

Bei anhaltenden Problemen:
For persistent issues:

1. Sammeln Sie Informationen:
   ```bash
   npm run validate > system-info.txt
   cat ~/.game-system/logs/*.log > logs.txt
   ```

2. Öffnen Sie ein GitHub Issue mit:
   - System-Informationen
   - Fehlermeldungen
   - Schritte zur Reproduktion

3. Community-Foren:
   - Linux Gaming Discord
   - ProtonDB-Foren
   - Wine-Mailinglisten
