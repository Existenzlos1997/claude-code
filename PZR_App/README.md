# PZR Assistent Pro - Installierbare Progressive Web App

## 🌐 LIVE APP — Direkt im Browser öffnen!

> **➡️ https://existenzlos1997.github.io/claude-code/**

---

### 🚀 Einmalige Einrichtung (2 Schritte)

#### Schritt 1 — Pages-Quelle auf "Branch: gh-pages" stellen

1. Gehe zu: **https://github.com/Existenzlos1997/claude-code/settings/pages**
2. Unter **Source** → wähle **"Deploy from a branch"**
3. Branch: **`gh-pages`** / Ordner: **`/ (root)`**
4. Speichern ✅

#### Schritt 2 — Deploy-Workflow manuell starten

**👉 https://github.com/Existenzlos1997/claude-code/actions/workflows/deploy-pages.yml**

1. **"Run workflow"** klicken (rechts oben)
2. Branch: `copilot/setup-installer-for-project`
3. **"Run workflow"** bestätigen → nach ~1 Minute ist die App live ✅

> Ab sofort läuft der Workflow **automatisch** bei jedem Push — kein manuelles Starten mehr nötig.

---

### 🔐 Demo-Zugangsdaten

| Rolle | Benutzername | Passwort |
|-------|-------------|----------|
| Behandler | `MA-001` | `admin` |
| Patient | Patientennummer: `PAT-12345` | PIN: `1234` |

---

### 📲 Android APK testen

1. App im Browser öffnen (URL oben)
2. Als **Behandler** anmelden (MA-001 / admin)
3. Tab **⚙️ Einstellungen** öffnen
4. Abschnitt **"🤖 Android-App installieren"** → **"⬇️ Android APK herunterladen"** klicken
5. APK auf Android-Gerät installieren (Einstellungen → Unbekannte Quellen erlauben)

> **Android APK bauen:** **👉 https://github.com/Existenzlos1997/claude-code/actions/workflows/build-android-apk.yml** → "Run workflow" → nach ~5 Min unter [Nightly Release](https://github.com/Existenzlos1997/claude-code/releases/tag/nightly) verfügbar

---

## 📱 INSTALLIERBAR AUF ALLEN PLATTFORMEN!

Die PZR App ist jetzt eine **Progressive Web App (PWA)** - installierbar wie eine native App auf Android, iOS und Desktop!

---

## 🔗 App-Links

| Plattform | URL |
|-----------|-----|
| **Browser (Live)** | https://existenzlos1997.github.io/claude-code/ |
| **Direktdatei** | https://existenzlos1997.github.io/claude-code/pzr_app.html |

---

## 🦷 Was ist PZR Assistent Pro?

Eine professionelle Lösung für Zahnarztpraxen zur Verwaltung von PZR-Terminen mit **Login-System**, **Praxis-Registrierung** und **medizinischer Anamnese**.

### 🆕 Hauptfunktionen:

- 🏥 **Praxis-Registrierung:** Praxis kann sich selbst registrieren
- 👥 **Multi-User:** Mehrere Mitarbeiter pro Praxis
- 🔐 **Admin-System:** Erster Benutzer wird automatisch Administrator
- 👨‍⚕️ **Mitarbeiter-Verwaltung:** Admins können Mitarbeiter hinzufügen/verwalten
- 📅 **Termine:** Behandler können Termine für Patienten erstellen
- 🏥 **Anamnese:** Vollständige medizinische Vorgeschichte
- 🎤 **Sprachaufnahme:** Empfehlungen per Sprache erfassen
- 📱 **PWA-Installation:** Auf allen Plattformen installierbar

---

## 📲 Installation (Empfohlen!)

### 🤖 Android Installation

1. Link in Chrome/Edge/Brave öffnen
2. **Install-Banner** erscheint → "Jetzt installieren"
3. Oder: Menü (⋮) → "App installieren"
4. ✅ App erscheint auf dem Startbildschirm

### 🍎 iOS Installation

1. Link in **Safari** öffnen (wichtig!)
2. Teilen (⬆️) → "Zum Home-Bildschirm"
3. "Hinzufügen" bestätigen
4. ✅ App öffnet wie eine native App

### 💻 Desktop Installation

1. Chrome/Edge öffnen
2. Install-Symbol in Adressleiste (⊕)
3. "Installieren" klicken
4. ✅ App öffnet in eigenem Fenster

---

## 🔐 Anmeldung

### Demo-Zugangsdaten:

**Patient:**
- Patientennummer: `PAT-12345`
- PIN: `1234`

**Behandler:**
- Praxis: `Demo Zahnarztpraxis` wählen
- Mitarbeiter-ID: `MA-001`
- Passwort: `admin`

### Neue Praxis registrieren:

1. "Behandler" Rolle wählen
2. "Praxis registrieren" Button klicken
3. Praxis-Daten und Admin-Account eingeben
4. Zugangsdaten notieren!

---

## ✨ Hauptfunktionen

### 👤 Patienten-Interface

**Dashboard:**
- Behandler-Empfehlungen prominent angezeigt
- Nächste Termine im Überblick
- Profilinformationen

**Profil bearbeiten:**
- Name, E-Mail, Telefon selbst aktualisieren
- Sofortige Speicherung
- Immer aktuelle Kontaktdaten

**Medizinische Anamnese:**
- Vollständiges medizinisches Profil
- Allergien, Medikamente, Erkrankungen
- Kritische Infos werden rot markiert
- Jährliche Aktualisierungs-Erinnerung
- Warnung bei Login wenn > 1 Jahr alt

**Empfehlungen:**
- Individuelle PZR-Empfehlungen
- Putztechnik-Anweisungen
- Hilfsmittel und Verwendung
- Weitere Bemerkungen

**Termine:**
- Vom Behandler erstellte Termine
- Status-Anzeige (Heute, Bevorstehend, Vergangen)
- Automatische Aktualisierung

**Erinnerungen:**
- 24h vor Termin - mit Empfehlungen
- 1h vor Termin - mit Empfehlungen
- Nachsorge nach 6 Monaten
- Browser-Push-Benachrichtigungen

### 👨‍⚕️ Behandler-Interface

**Patienten verwalten:**
- Neue Patienten anlegen
- Patientennummern zuweisen
- Profile bearbeiten
- Suchfunktion
- Kritische medizinische Infos sehen

**Termine erstellen:**
- Termin für Patient anlegen
- Datum, Uhrzeit, Ort, Notizen
- Automatische Erinnerungen für Patient
- Kritische Anamnese-Infos werden angezeigt

**Empfehlungen erfassen:**
- PZR-Empfehlungen eingeben
- Putztechnik-Anpassungen
- Hilfsmittel und Anwendung
- Bemerkungen hinzufügen

**🎤 Sprachaufnahme:**
- Mikrofon-Symbol bei jedem Feld
- Automatische Sprache-zu-Text
- Deutsche Spracherkennung
- Echtzeit-Transkription

### 🏥 Admin-Interface (zusätzlich)

**Praxis-Verwaltung:**
- Praxis-Daten einsehen
- Mitarbeiter hinzufügen
- Rollen zuweisen
- Mitarbeiter deaktivieren

**Mitarbeiter-Rollen:**
- Administrator
- Behandler
- Dentalhygieniker/in
- Zahnarzthelfer/in
- Rezeption

---

## 🎯 Technische Features

### Progressive Web App (PWA)

- ✅ **Installierbar:** Wie eine native App
- ✅ **Offline-fähig:** Funktioniert ohne Internet
- ✅ **Schnell:** Service Worker für Performance
- ✅ **Sicher:** HTTPS, isoliert wie native App
- ✅ **Updates:** Automatisch über Browser
- ✅ **Plattformübergreifend:** Android, iOS, Desktop

### Datenspeicherung

- ✅ **LocalStorage:** Alle Daten lokal gespeichert
- ✅ **Keine Cloud:** Keine Serververbindung
- ✅ **Datenschutz:** Daten bleiben auf Gerät
- ✅ **DSGVO-konform:** Keine externe Verarbeitung
- ✅ **Multi-Practice:** Daten pro Praxis getrennt

### Medizinische Anamnese

**Kritische Felder (rot markiert):**
- Allergien
- Aktuelle Medikamente
- Blutungsneigung
- Herz-Kreislauf-Erkrankungen
- Diabetes
- Schwangerschaft

**Weitere Felder:**
- Chronische Erkrankungen
- Vorherige Operationen
- Raucher (Ja/Nein)
- Alkoholkonsum

**Auto-Reminder:**
- Warnung beim Login wenn > 1 Jahr alt
- Hinweis wenn nie ausgefüllt
- "Letztes Update" Anzeige

---

## 💡 Anwendungsfälle

### Für Zahnarztpraxen:

1. **Praxis registrieren**
2. **Admin-Account** erstellen
3. **Mitarbeiter** hinzufügen
4. **Patienten** anlegen
5. **Termine** erstellen
6. **Empfehlungen** erfassen

### Für Patienten:

1. **Zugangsdaten** vom Behandler erhalten
2. **App installieren** auf Smartphone
3. **Anmelden** mit Patientennummer + PIN
4. **Anamnese** ausfüllen
5. **Termine** einsehen
6. **Empfehlungen** befolgen

---

## 🔒 Datenschutz & Sicherheit

### DSGVO-konform:

- ✅ Daten nur lokal gespeichert
- ✅ Keine Cloud-Synchronisation
- ✅ Keine Datenübertragung an Server
- ✅ Volle Kontrolle über Daten
- ✅ Kein Account bei Drittanbietern
- ✅ Keine Tracking-Cookies
- ✅ Keine externe Datenverarbeitung

### Sicherheit:

- 🔐 PIN-geschützte Patientendaten
- 🔐 Passwort-geschützte Mitarbeiter-Accounts
- 🔐 Rollenbasierte Zugriffskontrolle
- 🔐 Daten bleiben auf dem Gerät

---

## 📱 Kompatibilität

### Voll unterstützt:

- ✅ Android 5.0+ (Chrome, Edge, Brave)
- ✅ iOS 11.3+ (Safari)
- ✅ Windows 10+ (Chrome, Edge)
- ✅ macOS (Chrome, Edge, Safari)
- ✅ Linux (Chrome, Firefox, Edge)

### Browser-Features:

| Feature | Chrome/Edge | Safari | Firefox |
|---------|-------------|--------|---------|
| PWA Installation | ✅ | ✅ | ⚠️ |
| Offline-Modus | ✅ | ✅ | ✅ |
| Spracherkennung | ✅ | ✅ | ❌ |
| Push-Benachrichtigungen | ✅ | ✅ | ✅ |
| LocalStorage | ✅ | ✅ | ✅ |

---

## 🚀 Schnellstart

1. **Link öffnen** im Browser
2. **"Jetzt installieren"** klicken
3. **Anmelden:**
   - Als Patient: PAT-12345 / 1234
   - Als Behandler: MA-001 / admin
4. **Erkunden:**
   - Dashboard ansehen
   - Profil bearbeiten
   - Anamnese ausfüllen
5. **App nutzen** wie eine native App!

---

## 📚 Dokumentation

- **DIREKTER_ZUGANG.md:** Installations-Anleitung
- **VERBESSERUNGSVORSCHLAEGE.md:** Feature-Vorschläge
- **README.md:** Diese Datei

---

## 🆕 Changelog

### Version 3.0 (Aktuell)
- ✅ PWA-Installation auf allen Plattformen
- ✅ Praxis-Registrierung
- ✅ Multi-User-System
- ✅ Admin-Mitarbeiter-Verwaltung
- ✅ Multi-Practice Support
- ✅ Verbesserte Datensynchronisation

### Version 2.0
- ✅ Medizinische Anamnese
- ✅ Profil-Bearbeitung für Patienten
- ✅ Terminverwaltung für Behandler
- ✅ Sprachaufnahme für Empfehlungen
- ✅ Auto-Markierung kritischer Infos

### Version 1.0
- ✅ Login-System
- ✅ Behandler-Empfehlungen
- ✅ Termin-Übersicht
- ✅ Erinnerungen

---

## 💪 Vorteile der PWA

### Für Praxen:
- ⚡ Keine App-Store-Gebühren
- 📱 Sofortige Updates
- 🔒 Volle Datenkontrolle
- 💾 Keine Server-Kosten
- 🎯 Einfache Bereitstellung

### Für Patienten:
- 📲 Einfache Installation
- 📴 Offline nutzbar
- 🔔 Push-Benachrichtigungen
- ⚡ Schneller Start
- 💾 Datenschutz

---

## 🛠️ Technischer Stack

- **Frontend:** Pure HTML/CSS/JavaScript
- **Storage:** LocalStorage API
- **Speech:** Web Speech API (de-DE)
- **PWA:** Service Worker, Web App Manifest
- **Notifications:** Notification API
- **Offline:** Service Worker Cache
- **Size:** ~105KB single-file
- **Dependencies:** Keine!

---

## 📞 Support

Bei Fragen oder Problemen:
- GitHub Issues erstellen
- Dokumentation lesen
- Demo-Zugänge testen

---

**Viel Erfolg mit der PZR Assistent Pro App! 🦷✨**

Entwickelt für professionelle Zahnarztpraxen mit Fokus auf Datenschutz und Benutzerfreundlichkeit.
