# PZR Assistent Pro

Praxis-Management-App für Zahnarztpraxen — läuft im Browser, als installierbare App auf Android/iOS und als Desktop-Anwendung (Windows/Mac/Linux).

---

## 🌐 Live-App (Browser)

**➡️ https://existenzlos1997.github.io/claude-code/**

Login: `MA-001` / `admin`

---

## Plattformen & Installation

| Plattform | Methode |
|-----------|---------|
| Browser | Direkt öffnen — kein Download nötig |
| Android | APK aus Einstellungen → Downloaden & Installieren |
| iOS | Einstellungen → "Zum Home-Bildschirm" (Safari) |
| Windows | .exe Installer aus Einstellungen → Downloaden |
| macOS | .dmg Installer aus Einstellungen → Downloaden |
| Linux | .deb Paket aus Einstellungen → Downloaden |

---

## Struktur

```
PZR_App/          → Haupt-App (pzr_app.html + manifest.json + sw.js)
PZR_App_Electron/ → Desktop-App (Electron + Node.js Backend)
PZR_App_Android/  → Android-Wrapper (Capacitor)
docs/             → GitHub Pages (Browser-App)
.github/workflows/
  deploy-pages.yml          → Deployed Browser-App
  build-electron-installer.yml → Baut Windows/Mac/Linux Installer
  build-android-apk.yml     → Baut Android APK
```

---

## GitHub Pages einrichten (einmalig)

1. Gehe zu: https://github.com/Existenzlos1997/claude-code/settings/pages
2. Source → "Deploy from a branch"
3. Branch: `copilot/setup-installer-for-project` / Ordner: `/docs`
4. Save
