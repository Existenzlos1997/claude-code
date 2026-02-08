# PZR APP – MASTER PROMPT (NEUSTART)

**Lies dieses Dokument vollständig.**
Dieses Projekt startet bei **Null** – es existieren **keine Dateien**. Du musst **alles von Grund auf** neu erstellen.

## 1) Ziel & Kontext
Ich möchte eine **mobile PZR-App** (Prophylaxe/Zahnreinigung), die Patienten **Empfehlungen**, **Erinnerungen** und eine **Terminübersicht** bietet. Ich arbeite **nur am Smartphone**, daher muss die App so erstellt werden, dass ich sie **direkt im Browser öffnen, herunterladen und testen** kann.

**Kernziel:** Eine fertige, lauffähige HTML-App (Single-File), die lokal auf dem Smartphone funktioniert (offline nutzbar) und sich wie eine App installieren lässt (PWA/Shortcut).

## 2) Arbeitsweise (Pflicht)
### 2.1 Tracking-System (immer verwenden)
Nutze für jede Änderung das folgende Schema, damit jederzeit klar ist, **wann/wo/was/warum** etwas geändert wurde:

- **WANN:** Zeitpunkt / Schritt
- **WO:** Datei + konkrete Stelle (Zeile oder Abschnitt)
- **WAS:** konkrete Änderung (Code-Änderung)
- **WARUM:** welches Problem löst es
- **WESHALB:** Root Cause / Abhängigkeiten
- **ABHÄNGIGKEITEN:** was sonst betroffen ist

### 2.2 Kritische Regeln
1. **NIEMALS nur dokumentieren** – es müssen **echte Code-Änderungen** stattfinden.
2. **Ganzheitlich denken**: Änderungen dürfen keine Nebenwirkungen verursachen. Immer prüfen, ob alles zusammen funktioniert.
3. **Keine isolierten Schnipsel**: Änderungen müssen konsistent und kompatibel mit dem Rest sein.
4. **Sicherheit & Stabilität**: Keine neuen Fehler, keine Regressionen.
5. **Smartphone-Fokus**: UI muss mobil freundlich sein.

## 3) Lieferumfang (neue Dateien)
Erstelle ausschließlich folgende Dateien (minimal, aber vollständig):

- `PZR_App/pzr_app.html` – komplette App in **einer** HTML-Datei (inkl. CSS + JS)

Optional (nur wenn nötig):
- `PZR_App/manifest.json` (falls du PWA technisch sauberer unterstützen willst)

## 4) Datenmodell (lokal, persistent)
Speichere alle Daten in `localStorage`, damit die App auf dem Smartphone ohne Backend funktioniert.

### 4.1 Schlüssel
- `pzrPractices` – Praxen
- `pzrEmployees` – Mitarbeiter/Behandler
- `pzrPatients` – Patienten
- `pzrAppointments` – Termine
- `pzrRecommendations` – PZR-Empfehlungen
- `pzrSettings` – App-Settings (z.B. Erinnerungseinstellungen, Backend-PC Flag)

### 4.2 Wichtige Flags
- `isOriginalAdmin` (boolean): Nur der **erste Admin** darf Mitarbeiter verwalten.
- `isBackendPC` (boolean): Erst nach **erstem Admin-Login** auf installiertem Gerät setzen.
- `lastAnamneseUpdate` (Datum): Patienten müssen jährlich aktualisieren.

## 5) Rollen & Login-System
### 5.1 Rollen
- **Praxis-Admin** (Original-Admin)
- **Mitarbeiter/Behandler**
- **Patient**

### 5.2 Praxis-Registrierung
- Registrierung erzeugt Praxis & Admin-Konto.
- **Admin-Login-ID wird automatisch aus dem Administrator-Namen erstellt**, aber muss **editierbar** sein.
- Login-ID darf nicht zu lang sein.
- **Keine Praxis-Auswahl beim Login**: Login-ID ist automatisch mit der Praxis verknüpft.

### 5.3 Mitarbeiter-Login
- Mitarbeiter-IDs müssen **editierbar** sein.
- Nur Original-Admin sieht Mitarbeiter-Verwaltung.
- Mitarbeiter können Patienten über **Patienten-Nr** zuordnen und deren Oberfläche (Infos, Bemerkungen, Putztechnik, Produkte) anpassen.

### 5.4 Patienten-Login
- Patient loggt sich mit **Patienten-Nr** und ggf. Passwort ein.
- Patienten können ihre Stammdaten (Name, E-Mail etc.) aktualisieren.

### 5.5 Demo-Daten
Erstelle Demo-Daten (lokal), damit man die App sofort testen kann:
- Demo-Praxis mit Admin & Mitarbeiter
- 2 Patienten mit vollständiger Anamnese
- 1–2 Termine

## 6) Termin-System
### 6.1 Termin-Erstellung (Behandler)
- Behandler kann Termine hinzufügen.
- Patientenauswahl **nicht** per Dropdown, sondern **Suchfeld mit Patientennummer**.

### 6.2 Termin-Anzeige (Patient)
- Patient sieht Termine & Erinnerungseinstellungen.

### 6.3 Termin-Absage mit Ampel-System
Ampel-Regeln:
- 🟢 **14+ Tage**: kostenlos stornierbar
- 🟠 **2–13 Tage**: Warnung (mögliche Gebühr)
- 🔴 **0–1 Tag**: Ausfallgebühr Warnung

**Flow bei Absage:**
1. Ampel anzeigen
2. Bestätigungsdialog
3. Status → `cancelled-pending`
4. **Folgetermin-Wunsch** (Zeitpräferenzen) erfassen
5. Praxis bestätigt → endgültige Löschung

## 7) PZR-Empfehlungen (KERNFEATURE)
Die App muss automatisch Empfehlungen basierend auf Patientendaten generieren.

### 7.1 Risiko-Bewertung
Beispiel-Logik:
- Diabetes, Parodontitis, Karies, Rauchen etc. erhöhen Risiko
- Risiko-Stufen: `NIEDRIG`, `MITTEL`, `HOCH`

### 7.2 Empfehlungsausgabe
- Nächstes PZR-Intervall (3/6/12 Monate)
- Putzttechnik (z.B. modifizierte Bass-Technik)
- Produktempfehlungen (Zahnbürste, Zahnseide, Interdentalbürste)
- Verhaltens-Tipps (Ernährung, Rauchverhalten)

### 7.3 Anzeige
- Patienten sehen eigene Empfehlungen prominent
- Behandler können Empfehlungen bearbeiten & ergänzen
- Empfehlungen & Hinweise müssen **visuell hervorgehoben** sein (hohe Präsenz) und direkt in den Erinnerungen erscheinen.

## 8) Erinnerungs-System (Patient)
- Erinnerungen zu Terminen (z.B. 4 Wochen + 1 Woche vorher)
- Erinnerungs-Intensität einstellbar
- Erinnerung an **jährliche Anamnese-Aktualisierung**
- Wenn Anamnese abgelaufen → **kein Zugriff** bis aktualisiert

## 9) Anamnese & Gesundheitsdaten
- Vollständiger Anamnese-Fragebogen
- Wichtige Einträge auf Behandler-Seite **automatisch rot markiert**

## 10) Download & Installation
### 10.1 Install-Button (PWA)
- `beforeinstallprompt` nutzen
- Falls iOS: manuelle „Zum Home-Bildschirm“ Anleitung

### 10.2 Download-Auswahl
- Auf Login-Seite **Download/Installieren** Bereich mit Plattform-Auswahl (Windows, macOS, Linux, Android, iOS)
- **Kein „nicht verfügbar“** – jede Auswahl muss eine funktionierende Aktion auslösen

### 10.3 Backend-PC Logik
- Jeder kann installieren
- **Nur bei erster Admin-Anmeldung** auf installiertem Gerät wird `isBackendPC = true`
- Warnhinweis darf übersprungen werden

## 11) UI / Startseite
- Startseite muss „**Patient**“ und „**Behandler**“ korrekt anzeigen (keine falschen Labels)
- Mobile-first Design

## 12) Qualitäts-Sicherungen
- App muss vollständig lauffähig sein
- Keine Features dürfen nur dokumentiert sein – **alles muss im Code implementiert werden**
- Demo-Login muss funktionieren

---

**Ziel:** Eine komplette, echte, testbare PZR-App als Single-File HTML, die ich direkt auf dem Smartphone öffnen und nutzen kann.
