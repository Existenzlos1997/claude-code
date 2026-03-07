# PZR App – Praxisverwaltung

![GitHub Pages](https://img.shields.io/badge/Browser--App-live-brightgreen?style=flat-square)
![Plattform](https://img.shields.io/badge/Plattform-Windows%20%7C%20macOS%20%7C%20Linux%20%7C%20Android-blue?style=flat-square)

---

## 🌐 Browser-App (sofort testen – keine Installation nötig)

> **[👉 https://existenzlos1997.github.io/claude-code/](https://existenzlos1997.github.io/claude-code/)**

Die App läuft direkt im Browser. Einfach den Link öffnen und loslegen.

---

## 📥 Installer herunterladen

> **[👉 https://github.com/Existenzlos1997/claude-code/releases](https://github.com/Existenzlos1997/claude-code/releases)**

Verfügbare Installer (neueste Version):

| Plattform | Datei |
|-----------|-------|
| 🪟 Windows | `.exe` Setup-Installer |
| 🍎 macOS | `.dmg` Disk Image |
| 🐧 Linux | `.AppImage` |
| 🤖 Android | `.apk` |

---

## 🔑 Standard-Login

Beim ersten Start:
- **Mitarbeiter-ID:** `MA-001`
- **Passwort:** `admin`

---

## ✨ Features

- 👥 Patienten- und Mitarbeiterverwaltung
- 📅 Terminkalender
- 💰 Abrechnung & Rechnungsstellung
- 📊 CSV-Import/Export (kompatibel mit Evident, Z1, Charly, Dampsoft)
- 🔄 Automatische Updates
- 🌐 Mehrmandantenfähig (mehrere Praxen)
- 📱 Funktioniert auf Desktop, Tablet und Smartphone

---

## 🏗️ Architektur

```
PZR_App/           → Browser-App (HTML/JS/CSS) + PWA
PZR_App_Electron/  → Desktop-App (Electron + lokaler Backend-Server)
PZR_App_Android/   → Android-App (Capacitor)
docs/              → GitHub Pages Deployment
```
