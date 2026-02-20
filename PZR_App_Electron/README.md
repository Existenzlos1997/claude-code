# PZR Praxis Manager - Desktop App

Professionelle Praxisverwaltungs-Software mit integriertem Backend-Server.

## 🚀 Installation

### Voraussetzungen
- Node.js 18+ installiert
- npm oder yarn

### Schritt 1: Dependencies installieren
```bash
cd PZR_App_Electron
npm install
```

### Schritt 2: App starten
```bash
npm start
```

## 📦 Installer erstellen

### Windows (.exe)
```bash
npm run build:win
```
Installer wird erstellt in: `dist/PZR Praxis Manager Setup.exe`

### Mac (.dmg)
```bash
npm run build:mac
```
Installer wird erstellt in: `dist/PZR Praxis Manager.dmg`

### Linux (.deb)
```bash
npm run build:linux
```
Installer wird erstellt in: `dist/pzr-praxis-manager.deb`

## 🎯 Features

✅ Desktop-App für Windows, Mac & Linux
✅ Integrierter Backend-Server
✅ Automatische Live-Synchronisation
✅ Offline-Funktionalität
✅ Multi-Device Support
✅ Professioneller Installer

## 💡 Verwendung

1. **App installieren** - Installer ausführen
2. **Admin-Login** - Mit Admin-Credentials anmelden
3. **Backend aktivieren** - In Einstellungen "Backend aktivieren"
4. **QR-Code scannen** - Andere Geräte verbinden sich via QR-Code/URL
5. **Live-Sync** - Alle Änderungen werden sofort synchronisiert

## 🔧 Technologie

- **Frontend:** HTML5, CSS3, JavaScript
- **Desktop:** Electron
- **Backend:** Node.js + Express
- **Realtime:** WebSocket (ws)
- **Datenbank:** SQLite3
- **Build:** electron-builder

## 📱 Geräte-Kompatibilität

**Desktop (Installiert):**
- ✅ Windows 10/11
- ✅ macOS 10.14+
- ✅ Linux (Ubuntu, Debian, etc.)

**Mobile/Tablet (Browser):**
- ✅ Verbindung via Server-URL
- ✅ iOS Safari
- ✅ Android Chrome
- ✅ Live-Sync funktioniert

## 📞 Support

Bei Fragen oder Problemen wenden Sie sich an die Praxis Burgau.
