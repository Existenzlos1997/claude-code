# 🎯 PZR APP - FEATURE MASTERPLAN
## Systematische Umsetzung aller geplanten Features

**Basisversion:** pzr_app.html (90KB) - Wiederhergestellt am 10. Februar 2026  
**Status:** ✅ Funktioniert perfekt - BESTÄTIGT vom User  
**Letzte Aktualisierung:** 2026-02-10

---

## 📚 ALLE GEPLANTEN FEATURES

### 🔴 KATEGORIE A: KRITISCHE KERN-FEATURES

#### A1. **Ampel-System für Terminabsagen** 🚦
**Beschreibung:**
- 🟢 **14+ Tage vorher:** Kostenlos absagen
- 🟠 **2-13 Tage vorher:** Warnung + evtl. Gebühr  
- 🔴 **0-1 Tag vorher:** ACHTUNG Ausfallgebühr!

**Funktion:** `getTrafficLight(appointmentDate)`  
**Komplexität:** Mittel  
**Geschätzte LOC:** ~50 Zeilen  
**Abhängigkeiten:** Termin-Verwaltung  
**Test-Kriterien:**
- Ampel zeigt korrekte Farbe basierend auf Tagen
- Warnungs-Text wird angezeigt
- Integration in Termin-Anzeige

---

#### A2. **Terminabsage-Flow (6 Schritte)** 📅
**Beschreibung:**
1. Ampel-Warnung anzeigen
2. Bestätigung vom Patient einholen
3. Status → 'cancelled-pending'
4. Folgetermin-Anfrage mit Zeitpräferenzen
5. Praxis-Benachrichtigung
6. Praxis bestätigt → Endgültige Löschung

**Funktion:** `cancelAppointment(appointmentId)`  
**Komplexität:** Hoch  
**Geschätzte LOC:** ~120 Zeilen  
**Abhängigkeiten:** Ampel-System, Termin-Verwaltung  
**Test-Kriterien:**
- Alle 6 Schritte durchlaufen korrekt
- Status-Updates funktionieren
- Folgetermin-Dialog erscheint
- Praxis sieht Benachrichtigung

---

#### A3. **PZR-Empfehlungen (KERNFEATURE!)** ⭐
**Beschreibung:**
- Automatische Empfehlungs-Generierung nach PZR-Termin
- Risiko-Bewertung: **NIEDRIG / MITTEL / HOCH**
- Individuelle Empfehlungen basierend auf:
  - Anamnese (Allergien, Medikamente, Krankheiten)
  - Zahnstatus (Karies, Parodontitis, Zahnstein)
  - Mundhygiene-Level
  - Letzter PZR-Termin
- Empfehlungs-Kategorien:
  - Nächster PZR-Termin (3/6/12 Monate)
  - Putz-Technik Tipps
  - Produkt-Empfehlungen (Zahnbürste, Zahnseide, Mundspülung)
  - Verhaltens-Tipps (Rauchen, Zucker, etc.)

**Funktionen:**
- `calculateRiskLevel(patient)` - Berechnet Risiko-Score
- `generatePZRRecommendations(patientId)` - Generiert Empfehlungen
- `calculateNextPZRDate(patient)` - Berechnet nächsten Termin

**Komplexität:** Sehr Hoch  
**Geschätzte LOC:** ~250 Zeilen  
**Abhängigkeiten:** Anamnese, Zahnstatus, Patient-Daten  
**Test-Kriterien:**
- Risiko wird korrekt berechnet (basierend auf Test-Szenarien)
- Empfehlungen sind individuell und sinnvoll
- Nächster PZR-Termin ist logisch (3/6/12 Monate)
- Patient sieht Empfehlungen nach PZR

---

#### A4. **Patient-Suche (EINGABEFELD!)** 🔍
**Beschreibung:**
- Live-Suche während Tippen
- Filter nach: Name, Patienten-Nummer, Telefon
- Highlighting der Treffer
- Schnellzugriff auf Patient-Profil

**Funktion:** `searchPatients(query)`  
**Komplexität:** Mittel  
**Geschätzte LOC:** ~60 Zeilen  
**Abhängigkeiten:** Patienten-Liste  
**Test-Kriterien:**
- Suche funktioniert live (während Tippen)
- Alle relevanten Felder werden durchsucht
- Treffer werden hervorgehoben
- Performance: < 100ms bei 1000 Patienten

---

### 🟡 KATEGORIE B: WICHTIGE ZUSATZ-FEATURES

#### B1. **Anamnese-Fragebogen** 📝
**Beschreibung:**
- Standard-Fragen (Allergien, Medikamente, Krankheiten, OP's)
- Zahn-spezifische Fragen (Schmerzen, Bluten, Empfindlichkeit)
- Lebensstil-Fragen (Rauchen, Zucker-Konsum, Sport)
- Aktualisierungs-Tracking (wann zuletzt aktualisiert)
- Warnung bei kritischen Antworten (z.B. Blutverdünner)

**Funktion:** `updateAnamnese(patientId, anamneseData)`  
**Komplexität:** Mittel  
**Geschätzte LOC:** ~100 Zeilen  
**Abhängigkeiten:** Patient-Profil  
**Test-Kriterien:**
- Alle Fragen werden angezeigt
- Antworten werden gespeichert
- Aktualisierungs-Datum wird gesetzt
- Warnungen erscheinen bei kritischen Antworten

---

#### B2. **Zahnstatus-Erfassung** 🦷
**Beschreibung:**
- Erfassung des aktuellen Zahnzustands
- Felder:
  - `cariesCount` - Anzahl Karies
  - `periodontitis` - Parodontitis ja/nein
  - `tartarLevel` - Zahnstein-Level (leicht/mittel/stark)
  - `gumBleeding` - Zahnfleischbluten
  - `toothMobility` - Zahnlockerung
  - `lastPZRDate` - Letzter PZR-Termin
  - `findings` - Zusätzliche Befunde (Freitext)

**Funktion:** `updateDentalStatus(patientId, dentalStatus)`  
**Komplexität:** Mittel  
**Geschätzte LOC:** ~80 Zeilen  
**Abhängigkeiten:** Patient-Profil, PZR-Empfehlungen  
**Test-Kriterien:**
- Formular ist übersichtlich
- Alle Felder werden gespeichert
- Historie wird angelegt (mehrere Status-Updates)
- Integration in PZR-Empfehlungen funktioniert

---

#### B3. **Recall-System** 🔔
**Beschreibung:**
- Automatische Erinnerungen an fällige PZR
- Warteschlange für Praxis (wer ist fällig?)
- Status-Tracking:
  - `due` - Recall fällig
  - `invited` - Einladung verschickt
  - `confirmed` - Patient hat bestätigt
  - `scheduled` - Termin vereinbart
  - `completed` - PZR durchgeführt
- Email/SMS Benachrichtigung (optional)

**Funktionen:**
- `checkRecallsDue()` - Prüft fällige Recalls
- `sendRecallInvitation(patientId)` - Verschickt Einladung
- `updateRecallStatus(patientId, status)` - Aktualisiert Status

**Komplexität:** Hoch  
**Geschätzte LOC:** ~150 Zeilen  
**Abhängigkeiten:** PZR-Empfehlungen (nächster Termin)  
**Test-Kriterien:**
- Recalls werden korrekt berechnet (basierend auf letztem PZR + Intervall)
- Warteschlange zeigt alle fälligen Patienten
- Status-Änderungen werden gespeichert
- Praxis kann Recalls verwalten

---

#### B4. **Empfehlungs-Nachrichtensystem** 💬
**Beschreibung:**
- Behandler kann Empfehlung per Text eingeben
- Optional: Spracheingabe (Web Speech API)
- Nachricht wird auf Patient-Login angezeigt
- Eigene Erinnerungsfunktion einstellbar:
  - Täglich
  - Wöchentlich
  - Monatlich
- Patient kann Nachricht als "gelesen" markieren

**Funktionen:**
- `sendRecommendationToPatient(patientId, message, reminderSettings)`
- `startVoiceInput()` - Aktiviert Mikrofon
- `checkForNewRecommendations()` - Prüft bei Patient-Login
- `showRecommendationPopup(recommendations)` - Zeigt Popup
- `markRecommendationAsRead(recId)` - Markiert als gelesen

**Komplexität:** Hoch  
**Geschätzte LOC:** ~200 Zeilen  
**Abhängigkeiten:** Patient-Login, Web Speech API  
**Test-Kriterien:**
- Text-Eingabe funktioniert
- Spracheingabe funktioniert (Browser-Support prüfen!)
- Patient sieht Popup bei Login
- Erinnerungen erscheinen zur richtigen Zeit
- "Gelesen" markieren funktioniert

---

### 🟢 KATEGORIE C: SYSTEM-VERBESSERUNGEN

#### C1. **PWA Installation (Multi-Platform)** 📱
**Beschreibung:**
- Windows: .exe Download (HTML als Starter)
- Android: APK Download (PWA Wrapper)
- iOS: PWA Installation (Add to Home Screen)
- Desktop: Browser PWA (Chrome/Edge/Firefox)
- Platform Detection (erkennt OS automatisch)

**Funktion:** `downloadApp(platform)`  
**Komplexität:** Mittel  
**Geschätzte LOC:** ~100 Zeilen  
**Abhängigkeiten:** Manifest, Service Worker  
**Test-Kriterien:**
- Platform wird korrekt erkannt
- Download funktioniert für alle Plattformen
- PWA ist installierbar
- Offline-Funktion funktioniert

---

#### C2. **Backend-Initialisierung** 💻
**Beschreibung:**
- Backend-PC wird nur gesetzt wenn ALLE Bedingungen erfüllt:
  1. App ist installiert (als PWA)
  2. User ist Admin (role === 'admin')
  3. Erste Anmeldung auf diesem PC (localStorage-Flag)
- Nur EIN PC pro Praxis ist Backend
- Speichert: `localStorage.isBackendPC = true`

**Funktion:** `initializeBackendOnFirstAdminLogin()`  
**Komplexität:** Mittel  
**Geschätzte LOC:** ~70 Zeilen  
**Abhängigkeiten:** PWA Installation, Admin-Login  
**Test-Kriterien:**
- Wird nur bei PWA aktiviert (nicht im Browser)
- Nur Admin kann Backend initialisieren
- Nur einmal pro PC
- Flag wird gespeichert

---

#### C3. **Admin-ID Auto-Generierung** 🔑
**Beschreibung:**
- Generiert Login-ID aus Administrator-Name
- Beispiel: "Dr. Müller" → "DR-MUELLER"
- Transformationen:
  - Großbuchstaben
  - Ä→AE, Ö→OE, Ü→UE, ß→SS
  - Leerzeichen → Bindestrich
  - Sonderzeichen entfernen
  - Max. 20 Zeichen

**Funktion:** `generateAdminLoginId(administratorName)`  
**Komplexität:** Niedrig  
**Geschätzte LOC:** ~30 Zeilen  
**Abhängigkeiten:** Praxis-Registrierung  
**Test-Kriterien:**
- Umlaute werden korrekt ersetzt
- Sonderzeichen werden entfernt
- Länge max. 20 Zeichen
- ID ist eindeutig

---

#### C4. **Statistiken Dashboard** 📊
**Beschreibung:**
- **Für Praxis:**
  - PZR pro Monat (Chart)
  - Recall-Quote (%)
  - Risiko-Verteilung (Niedrig/Mittel/Hoch)
  - Top 10 fällige Patienten
  
- **Für Patient:**
  - PZR-Historie (Timeline)
  - Verbesserung-Trend (besser/schlechter)
  - Nächster empfohlener Termin
  - Persönliche Statistik

**Funktionen:**
- `loadPracticeStatistics()` - Berechnet Praxis-Stats
- `loadPatientStatistics(patientId)` - Berechnet Patient-Stats
- `generateChart(data, type)` - Erstellt Chart (CSS/HTML)

**Komplexität:** Mittel  
**Geschätzte LOC:** ~120 Zeilen  
**Abhängigkeiten:** PZR-Daten, Recall-System  
**Test-Kriterien:**
- Alle Statistiken werden korrekt berechnet
- Charts sind übersichtlich (nur CSS/HTML, kein Framework!)
- Daten sind aktuell
- Performance: < 500ms bei 1000 Patienten

---

### 🔵 KATEGORIE D: OPTIONAL/ZUKUNFT

#### D1. **Multi-User Verwaltung** 👥
**Beschreibung:**
- `isOriginalAdmin` Flag (nur Registrierungs-Admin)
- Mitarbeiter-Rollen (Admin / Behandler / Rezeption)
- Berechtigungen pro Rolle
- Nur Original-Admin sieht "Mitarbeiter"-Tab

**Komplexität:** Sehr Hoch  
**Geschätzte LOC:** ~200 Zeilen  
**Status:** Später, wenn Basis stabil

---

#### D2. **Cloud-Sync** ☁️
**Beschreibung:**
- Firebase Integration
- Echtzeit-Synchronisation
- Offline-First mit automatischem Sync
- Konflikt-Auflösung

**Komplexität:** Extrem Hoch  
**Geschätzte LOC:** ~400+ Zeilen  
**Status:** Später, wenn Basis stabil

---

## 🎯 MASTERPLAN - PHASENWEISE UMSETZUNG

### **PHASE 1: Kritische Features** 🔴
**Ziel:** Ampel, Absage, Suche, Anamnese  
**Geschätzte Dauer:** 3-4 Sessions  
**Geschätzte LOC:** ~330 Zeilen

#### Session 1.1: Patient-Suche + Ampel-System
**Scope:**
- ✅ Patient-Suche (Eingabefeld mit Live-Filter)
- ✅ Ampel-System (getTrafficLight Funktion)
- ✅ Integration in Termin-Anzeige

**Output:** ~110 Zeilen Code  
**Test:** 
- Suche: "Max" → zeigt Max Mustermann
- Ampel: Termin in 20 Tagen → 🟢 Grün
- Ampel: Termin in 5 Tagen → 🟠 Orange
- Ampel: Termin morgen → 🔴 Rot

**Abhängigkeiten:** Keine (nutzt bestehende Patienten-Liste)

---

#### Session 1.2: Terminabsage-Flow
**Scope:**
- ✅ 6-Schritte Workflow implementieren
- ✅ Folgetermin-Anfrage Dialog
- ✅ Praxis-Benachrichtigung System
- ✅ Status-Updates (cancelled-pending → cancelled)

**Output:** ~120 Zeilen Code  
**Test:**
- Absage-Button klicken → Ampel-Warnung
- Bestätigen → Folgetermin-Dialog
- Zeitpräferenzen angeben → gespeichert
- Praxis sieht Benachrichtigung
- Praxis bestätigt → Termin gelöscht

**Abhängigkeiten:** Session 1.1 (Ampel-System)

---

#### Session 1.3: Anamnese + Zahnstatus
**Scope:**
- ✅ Anamnese-Fragebogen (alle Fragen)
- ✅ Zahnstatus-Erfassung (alle Felder)
- ✅ Aktualisierungs-Logik
- ✅ Warnungen bei kritischen Werten

**Output:** ~180 Zeilen Code  
**Test:**
- Anamnese ausfüllen → gespeichert
- Zahnstatus eingeben → gespeichert
- Aktualisierungs-Datum korrekt
- Warnung bei "Blutverdünner" → erscheint

**Abhängigkeiten:** Keine (erweitert Patient-Profil)

---

#### Session 1.4: PZR-Empfehlungen (Teil 1 - Risiko)
**Scope:**
- ✅ Risiko-Bewertung Algorithmus (calculateRiskLevel)
- ✅ Datenstruktur für Empfehlungen
- ✅ Basis-Empfehlungs-Generierung
- ✅ Test-Szenarien durchspielen

**Output:** ~130 Zeilen Code  
**Test:**
- Patient mit Diabetes + Karies → HOCH
- Patient mit Parodontitis → MITTEL
- Gesunder Patient → NIEDRIG
- Empfehlungs-Objekt wird erstellt

**Abhängigkeiten:** Session 1.3 (Anamnese + Zahnstatus)

---

### **PHASE 2: Empfehlungen & Kommunikation** 🟡
**Ziel:** PZR-Empfehlungen komplett, Nachrichtensystem  
**Geschätzte Dauer:** 2-3 Sessions  
**Geschätzte LOC:** ~420 Zeilen

#### Session 2.1: PZR-Empfehlungen (Teil 2 - Komplett)
**Scope:**
- ✅ Vollständige Empfehlungs-Generierung
- ✅ Produkt-Empfehlungen (Zahnbürste, Zahnseide, etc.)
- ✅ Verhaltens-Tipps (Rauchen, Zucker)
- ✅ Anzeige-Komponenten für Patient
- ✅ Print/Export Funktion

**Output:** ~120 Zeilen Code  
**Test:**
- PZR-Empfehlung generieren → alle Kategorien gefüllt
- Patient sieht Empfehlungen im Dashboard
- Print-Funktion → PDF/Druck möglich

**Abhängigkeiten:** Session 1.4 (Risiko-Bewertung)

---

#### Session 2.2: Empfehlungs-Nachrichtensystem
**Scope:**
- ✅ Text-Eingabe Dialog (Behandler-Seite)
- ✅ Spracheingabe (Web Speech API)
- ✅ Nachricht an Patient senden
- ✅ Patient-Login Integration
- ✅ Popup bei neuen Nachrichten

**Output:** ~200 Zeilen Code  
**Test:**
- Behandler: Text eingeben → Senden → gespeichert
- Behandler: 🎤 Sprechen → Text erscheint → Senden
- Patient Login → Popup mit Nachricht
- "Als gelesen" → Popup verschwindet

**Abhängigkeiten:** Keine (eigenständiges Feature)

---

#### Session 2.3: Erinnerungs-System
**Scope:**
- ✅ Erinnerungs-Einstellungen (täglich/wöchentlich/monatlich)
- ✅ Zeitgesteuerte Benachrichtigungen
- ✅ Popup bei fälliger Erinnerung
- ✅ "Später erinnern" Funktion

**Output:** ~100 Zeilen Code  
**Test:**
- Erinnerung einstellen → gespeichert
- Nach X Tagen → Erinnerungs-Popup
- "Später" → Popup in 1 Tag wieder
- "Erledigt" → Erinnerung entfernt

**Abhängigkeiten:** Session 2.2 (Nachrichtensystem)

---

### **PHASE 3: Recall & Statistiken** 🟢
**Ziel:** Automatisches Recall, Dashboard  
**Geschätzte Dauer:** 2 Sessions  
**Geschätzte LOC:** ~270 Zeilen

#### Session 3.1: Recall-System
**Scope:**
- ✅ Automatische Recall-Prüfung (basierend auf letztem PZR)
- ✅ Einladungs-Versand Funktion
- ✅ Status-Tracking (due/invited/confirmed/scheduled/completed)
- ✅ Warteschlangen-Ansicht für Praxis

**Output:** ~150 Zeilen Code  
**Test:**
- Patient mit PZR vor 6 Monaten → "due"
- Einladung senden → Status "invited"
- Patient bestätigt → Status "confirmed"
- Warteschlange zeigt alle "due" Patienten

**Abhängigkeiten:** Session 2.1 (PZR-Empfehlungen für Intervall)

---

#### Session 3.2: Statistiken Dashboard
**Scope:**
- ✅ Praxis-Dashboard (PZR/Monat, Recall-Quote, Risiko-Verteilung)
- ✅ Patient-Dashboard (Historie, Trend, nächster Termin)
- ✅ Grafische Anzeige (CSS/HTML Charts)
- ✅ Export-Funktion

**Output:** ~120 Zeilen Code  
**Test:**
- Praxis: Chart zeigt PZR pro Monat
- Praxis: Recall-Quote berechnet korrekt
- Patient: Historie zeigt alle PZR
- Patient: Trend-Anzeige funktioniert

**Abhängigkeiten:** Session 3.1 (Recall für Quote)

---

### **PHASE 4: System-Features & Polish** 🔵
**Ziel:** PWA, Backend, Final Testing  
**Geschätzte Dauer:** 2 Sessions  
**Geschätzte LOC:** ~200+ Zeilen

#### Session 4.1: PWA & Backend
**Scope:**
- ✅ Multi-Platform Download (Windows/Android/iOS)
- ✅ Backend-Initialisierung (nur Admin, nur PWA, nur einmal)
- ✅ Admin-ID Generierung (bei Registrierung)
- ✅ Manifest & Service Worker

**Output:** ~200 Zeilen Code  
**Test:**
- Platform Detection funktioniert
- Download für alle Plattformen
- Backend wird nur bei Admin+PWA initialisiert
- Admin-ID wird korrekt generiert

**Abhängigkeiten:** Keine (System-Features)

---

#### Session 4.2: Polish & Final Testing
**Scope:**
- ✅ Bug-Fixes (alles was gefunden wurde)
- ✅ UI-Verbesserungen (Design-Tweaks)
- ✅ Performance-Optimierung
- ✅ Vollständige End-to-End Tests
- ✅ Dokumentation aktualisieren

**Output:** Variable  
**Test:**
- Alle Features funktionieren zusammen
- Keine JavaScript-Fehler
- Performance: < 1s Ladezeit
- Browser-Kompatibilität (Chrome/Edge/Firefox/Safari)

**Abhängigkeiten:** Alle vorherigen Sessions

---

## 📊 GESAMT-ÜBERSICHT

### Komplexität nach Kategorie:
| Kategorie | Features | LOC | Sessions |
|-----------|----------|-----|----------|
| A - Kritisch | 4 | ~480 | 4 |
| B - Wichtig | 4 | ~530 | 3 |
| C - System | 4 | ~320 | 2 |
| D - Optional | 2 | ~600+ | Später |
| **GESAMT (A-C)** | **12** | **~1330** | **9** |
| **+ Polish** | - | Variable | 2 |
| **TOTAL** | **12+** | **~1330+** | **11** |

### Zeitschätzung (Phasen 1-4):
- **Pro Session:** 1-2 Stunden Entwicklung + Testing
- **Gesamt:** 11 Sessions = 11-22 Stunden
- **Kalender:** 2-3 Wochen (bei 3-5 Sessions/Woche)

---

## ⚠️ KRITISCHE REGELN FÜR JEDE SESSION

### ✅ VOR jeder Session:
1. **BACKUP erstellen:** `pzr_app.html` → `pzr_app_backup_YYYYMMDD.html`
2. **Basisversion prüfen:** Funktioniert alles?
3. **Session-Ziel klar:** Was genau wird implementiert?
4. **Abhängigkeiten prüfen:** Welche Features werden benötigt?

### ✅ WÄHREND der Session:
1. **NUR mit edit Tool arbeiten** - NIEMALS nur dokumentieren!
2. **Systematisch vorgehen** - Alle Abhängigkeiten zusammen
3. **Code-Qualität:** Keine Template-Strings mit `<script>`
4. **Sichere Zugriffe:** `obj && obj.prop` statt `obj.prop`
5. **Keine doppelten Funktionen** - Vorher mit grep prüfen

### ✅ NACH jeder Session:
1. **Testen:** Demo-Logins durchspielen
2. **Browser Console:** Keine Fehler?
3. **Commit:** Mit aussagekräftiger Message
4. **Test-Link:** User zum Testen geben
5. **Feedback:** User bestätigt → nächste Session

---

## 🎯 WORKFLOW PRO SESSION

### 1. SESSION STARTEN
```
User sagt: "Los mit Phase X.Y"
```

### 2. TRACKING ERSTELLEN
```
WANN: [Jetzt]
WO: [pzr_app.html, Zeile X]
WAS: [Feature X implementieren]
WARUM: [Welches Problem lösen]
WESHALB: [Root-Cause]
ABHÄNGIGKEITEN: [Feature Y, Z]
```

### 3. CODE IMPLEMENTIEREN
- Mit **edit Tool** echten Code schreiben
- Alle Funktionen vollständig
- Keine Placeholder/TODOs
- Kommentare nur wo nötig

### 4. TESTEN
- Demo-Logins
- Neue Features verifizieren
- Browser Console
- Edge Cases

### 5. COMMIT & SHARE
```bash
git add .
git commit -m "SESSION X.Y: Feature X implemented and tested"
git push
```

### 6. TEST-LINK TEILEN
```
https://raw.githubusercontent.com/Existenzlos1997/claude-code/copilot/add-app-for-pzr-recommendations/PZR_App/pzr_app.html
```

### 7. FEEDBACK EINHOLEN
```
User testet → Bestätigung → Nächste Session
User findet Bug → Fix → Re-Test → Bestätigung
```

---

## 🚀 NÄCHSTER SCHRITT

**Basisversion bestätigt:** ✅ pzr_app.html (90KB)  
**Masterplan erstellt:** ✅ Dieses Dokument

**Bereit zum Start!**

### Womit sollen wir beginnen?

**Empfehlung:** PHASE 1, Session 1.1
- Patient-Suche (Live-Filter)
- Ampel-System (14/2/0 Tage)
- Scope: Überschaubar (~110 LOC)
- Keine komplexen Abhängigkeiten
- Sichtbare Verbesserung für User

**Alternative Starts:**
- Session 1.3: Anamnese + Zahnstatus (unabhängig)
- Session 2.2: Nachrichtensystem (standalone Feature)
- Session 4.1: PWA Installation (System-Feature)

**User entscheidet:**
- "Los mit Phase 1.1" → Patient-Suche + Ampel
- "Ich möchte mit Feature X starten" → Andere Priorität
- "Zeig mir erst Feature Y genauer" → Detailplanung

---

**MASTERPLAN BEREIT - WARTE AUF START-SIGNAL!** 🚀📋
