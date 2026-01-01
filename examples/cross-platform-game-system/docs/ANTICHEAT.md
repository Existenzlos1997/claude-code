# Anti-Cheat Kompatibilität / Anti-Cheat Compatibility

## Deutsch

### Das Problem

Viele moderne Spiele verwenden Anti-Cheat-Systeme (z.B. EasyAntiCheat, BattlEye), die auf Kernel-Ebene arbeiten und Windows-spezifische Komponenten benötigen. Diese funktionieren oft nicht unter Wine/Proton, da sie:
- Windows-Kernel-Treiber verwenden
- Systemintegrität auf niedrigster Ebene prüfen
- Virtuelle Maschinen und Emulatoren erkennen

### Lösungsansätze

#### 1. **Kernel-Ebene Emulation**
Unser System kann Windows-Kernel-Funktionen emulieren, die Anti-Cheat-Programme benötigen:
- Windows NT Kernel API
- Driver Loading (ntoskrnl.exe)
- System Call Tables
- Hardware-IDs und Registry-Einträge

#### 2. **EasyAntiCheat (EAC) Support**
EAC hat offiziellen Linux-Support über Proton seit 2021:
- Entwickler müssen EAC für Linux aktivieren
- Proton Experimental unterstützt EAC nativ
- Unser System kann automatisch erkennen, ob EAC-Support aktiv ist

#### 3. **BattlEye Support**
BattlEye bietet ebenfalls Linux-Support:
- Native Linux-Version für Proton
- Automatische Erkennung und Konfiguration
- Spiele mit aktiviertem Support funktionieren

### Implementierung

---

## English

### The Problem

Many modern games use anti-cheat systems (e.g., EasyAntiCheat, BattlEye) that operate at kernel level and require Windows-specific components. These often don't work under Wine/Proton because they:
- Use Windows kernel drivers
- Check system integrity at the lowest level
- Detect virtual machines and emulators

### Solution Approaches

#### 1. **Kernel-Level Emulation**
Our system can emulate Windows kernel functions that anti-cheat programs need:
- Windows NT Kernel API
- Driver Loading (ntoskrnl.exe)
- System Call Tables
- Hardware IDs and Registry entries

#### 2. **EasyAntiCheat (EAC) Support**
EAC has official Linux support via Proton since 2021:
- Developers must enable EAC for Linux
- Proton Experimental supports EAC natively
- Our system can automatically detect if EAC support is active

#### 3. **BattlEye Support**
BattlEye also offers Linux support:
- Native Linux version for Proton
- Automatic detection and configuration
- Games with enabled support work

### Implementation

## Unterstützte Anti-Cheat Systeme / Supported Anti-Cheat Systems

| System | Status | Methode / Method |
|--------|--------|------------------|
| EasyAntiCheat (EAC) | ✅ Unterstützt / Supported | Proton mit EAC-Modul |
| BattlEye | ✅ Unterstützt / Supported | Proton mit BattlEye-Modul |
| Valve Anti-Cheat (VAC) | ✅ Funktioniert / Works | Native Linux-Unterstützung |
| Denuvo Anti-Tamper | ⚠️ Teilweise / Partial | Wine/Proton kompatibel |
| Riot Vanguard | ❌ Nicht unterstützt / Not supported | Kernel-Treiber erforderlich |
| FACEIT Anti-Cheat | ❌ Nicht unterstützt / Not supported | Kernel-Treiber erforderlich |

## Technische Details / Technical Details

### Kernel-Mode Driver Emulation

Wine/Proton kann begrenzte Kernel-Treiber-Funktionalität emulieren:

```
Wine Kernel Emulation Stack:
┌─────────────────────────────┐
│   Game + Anti-Cheat DLL     │
├─────────────────────────────┤
│   Wine System32/SysWOW64    │
├─────────────────────────────┤
│   Wine Kernel32/NTDLL       │
├─────────────────────────────┤
│   Wine Driver Support       │
│   (ntoskrnl.exe emulation)  │
├─────────────────────────────┤
│   Linux Kernel Interface    │
└─────────────────────────────┘
```

### Automatische EAC/BattlEye Erkennung

Das System erkennt automatisch, ob ein Spiel Anti-Cheat verwendet und ob Support verfügbar ist.

## Konfiguration / Configuration

### config/anticheat.json

```json
{
  "enabled": true,
  "protonGE": true,
  "easyAntiCheat": {
    "enabled": true,
    "useNativeModule": true,
    "fallbackToProton": true
  },
  "battleEye": {
    "enabled": true,
    "useNativeModule": true
  },
  "kernelEmulation": {
    "enabled": true,
    "emulateDriverLoading": true,
    "spoofHardwareIDs": false
  },
  "compatibility": {
    "hideWineEnvironment": true,
    "emulateWindowsVersion": "win10",
    "disableVirtualizationDetection": true
  }
}
```

## Verwendung / Usage

### Spiel mit EAC starten / Launch Game with EAC

```bash
# Automatische Erkennung und Konfiguration
npm start launch "Apex Legends" -- --anticheat eac

# Mit Proton GE (empfohlen für Anti-Cheat)
npm start launch "Apex Legends" -- --layer proton-ge --anticheat eac
```

### Status prüfen / Check Status

```bash
# Anti-Cheat Kompatibilität prüfen
npm run check-anticheat "Game Name"
```

## Workarounds für nicht unterstützte Systeme

### 1. Dual Boot
- Windows für Spiele mit inkompatiblem Anti-Cheat
- Linux für alles andere

### 2. GPU Passthrough (VM)
- Windows-VM mit direktem GPU-Zugriff
- Nahezu native Performance
- Komplex einzurichten

### 3. Cloud Gaming
- GeForce NOW, Stadia, etc.
- Spiele laufen auf Remote-Servern
- Keine lokale Anti-Cheat-Probleme

## Entwickler-Ressourcen / Developer Resources

### EAC Linux Support aktivieren
Entwickler können EAC für Linux aktivieren über:
- Epic Games Developer Portal
- Aktivierung dauert nur wenige Minuten
- Keine Code-Änderungen erforderlich

### BattlEye Linux Support
Kontakt: support@battleye.com
- Kostenlose Linux-Unterstützung verfügbar
- Einfache Integration

## Testing

```bash
# Test EAC Erkennung
npm run test:anticheat -- --type eac

# Test BattlEye Erkennung  
npm run test:anticheat -- --type battleye

# Test Kernel Emulation
npm run test:kernel-emu
```

## Bekannte kompatible Spiele / Known Compatible Games

**EasyAntiCheat (funktioniert):**
- Apex Legends
- Dead by Daylight
- Rust
- War Thunder
- Fall Guys

**BattlEye (funktioniert):**
- Rainbow Six Siege
- ARMA 3
- DayZ
- Destiny 2

**Nicht kompatibel:**
- Valorant (Riot Vanguard)
- FACEIT-enabled games
- Genshin Impact (kernel anti-cheat)

## Zukunft / Future

Epic Games und andere Entwickler arbeiten an verbessertem Linux-Support. Die Situation wird sich mit der Zeit verbessern, besonders durch:
- Steam Deck Popularität
- Proton Weiterentwicklung
- Entwickler-Unterstützung

## Weitere Informationen / More Information

- [ProtonDB](https://www.protondb.com/) - Spielkompatibilität prüfen
- [Are We Anti-Cheat Yet?](https://areweanticheatyet.com/) - Anti-Cheat Status
- [Proton GE](https://github.com/GloriousEggroll/proton-ge-custom) - Enhanced Proton
