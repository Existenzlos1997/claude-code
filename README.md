# PZR Assistent Pro

Praxis-Management-App für Zahnarztpraxen — läuft im Browser, als installierbare App auf Android/iOS und als Desktop-Anwendung (Windows/Mac/Linux).

---

## 🌐 Live-App (Browser)

**➡️ https://existenzlos1997.github.io/claude-code/**

Login: `MA-001` / `admin`

---

## 🚀 Neuen Installer bauen (nach Code-Änderungen)

Da Copilot-Bot Commits immer eine Genehmigung brauchen, gibt es zwei Wege:

### Weg 1 — Workflow selbst starten (empfohlen, kein Approve nötig)
1. Gehe zu: https://github.com/Existenzlos1997/claude-code/actions/workflows/build-electron-installer.yml
2. Klicke **"Run workflow"** (grüner Button rechts)
3. Branch `copilot/setup-installer-for-project` auswählen → **Run workflow**
4. Nach ~5 Minuten: neuer Installer im [nightly Release](https://github.com/Existenzlos1997/claude-code/releases/tag/nightly)

### Weg 2 — Bot-Run genehmigen
1. Gehe zu: https://github.com/Existenzlos1997/claude-code/actions
2. Letzten Run mit "Action required" anklicken → **Approve and run**

### ⚠️ Einmalige Neu-Installation erforderlich
Die aktuell installierte Version enthält **noch kein** Auto-Update.  
Du musst **einmal** den neuen Installer ausführen — **kein Deinstallieren nötig**, einfach über die bestehende Installation drüber installieren.  
**Danach:** Alle zukünftigen Updates passieren automatisch beim App-Start (Banner erscheint, 1 Klick → fertig).

---

## Plattformen & Installation

| Plattform | Methode |
|-----------|---------|
| Browser | Direkt öffnen — kein Download nötig, Auto-Update durch Service Worker |
| Android | APK aus Einstellungen → Downloaden & Installieren |
| iOS | Einstellungen → "Zum Home-Bildschirm" (Safari) |
| Windows | .exe Installer aus Einstellungen → Downloaden |
| macOS | .dmg Installer aus Einstellungen → Downloaden |
| Linux | .deb Paket aus Einstellungen → Downloaden |

---

## Rollen & Benutzer

| Rolle | Erstellt von | Kann | Kann nicht |
|-------|-------------|------|------------|
| `masterAdmin` (MA-001) | — | Neue Praxis-Admins anlegen | — |
| `admin` (Praxis) | masterAdmin | Eigene Mitarbeiter anlegen, Backend einrichten | Weitere Admins anlegen |
| `employee` (Mitarbeiter) | Praxis-Admin | App nutzen | Konten anlegen |

**Patienten-Vorregistrierung:** Jede Praxis hat einen einzigartigen Link (Einstellungen → "Registrierungslink").  
Patienten öffnen diesen Link → registrieren sich → Praxis-Admin bestätigt.

---

## Struktur

```
PZR_App/          → Haupt-App (pzr_app.html + manifest.json + sw.js)
PZR_App_Electron/ → Desktop-App (Electron + Node.js Backend + Auto-Updater)
PZR_App_Android/  → Android-Wrapper (Capacitor)
docs/             → GitHub Pages (Browser-App, identisch mit PZR_App/)
.github/workflows/
  deploy-pages.yml             → Deployed Browser-App bei jedem Push
  build-electron-installer.yml → Baut Windows/Mac/Linux Installer (nightly)
  build-android-apk.yml        → Baut Android APK (nightly)
```

---

## GitHub Pages einrichten (einmalig)

1. Gehe zu: https://github.com/Existenzlos1997/claude-code/settings/pages
2. Source → "Deploy from a branch"
3. Branch: `copilot/setup-installer-for-project` / Ordner: `/docs`
4. Save
