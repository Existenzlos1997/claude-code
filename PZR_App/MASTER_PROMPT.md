# PZR APP - MASTER PROMPT FÜR NEUE IMPLEMENTATION

## 📋 LESE DIESE DATEI VOLLSTÄNDIG VOR DEM START!

Diese Datei ist deine KOMPLETTE Anleitung zur Erstellung der PZR (Professionelle Zahnreinigung) App VON GRUND AUF. Lies sie KOMPLETT bevor du beginnst!

---

## 🎯 PROJEKT-ÜBERSICHT

### Was ist die PZR App?
Eine Progressive Web App (PWA) für Zahnarztpraxen zur Verwaltung von:
- Professionellen Zahnreinigungsterminen  
- Patientendaten und Anamnese
- Mitarbeiterverwaltung
- Terminabsagen mit Ampel-System

### Architektur
- **Frontend:** Single-Page HTML App mit Vanilla JavaScript
- **Datei:** `pzr_app.html` (EINE einzelne HTML-Datei!)
- **Datenspeicher:** localStorage (Client-seitig)
- **Installation:** PWA (Progressive Web App) für alle Plattformen
- **Backend-Konzept:** Erster Admin-Login auf einem PC macht diesen zum Backend

### Technologie-Stack
- HTML5
- CSS3 (inline)
- Vanilla JavaScript (keine Frameworks!)
- localStorage API
- Service Worker für PWA
- Manifest.json (inline)

---

## 🚨 KRITISCHE ARBEITSREGELN

### ⚠️ REGEL #1: NIEMALS NUR DOKUMENTIEREN!
**DAS IST DIE WICHTIGSTE REGEL!**

- ❌ **FALSCH:** Features in Dokumenten/Kommentaren beschreiben
- ❌ **FALSCH:** "Ich plane Feature X zu implementieren..."
- ✅ **RICHTIG:** Mit `edit` Tool ECHTEN Code in pzr_app.html schreiben!
- ✅ **RICHTIG:** Code SOFORT implementieren, nicht später!

**Warum kritisch:** User hat wiederholt erlebt, dass Features nur dokumentiert aber NIE implementiert wurden!

**User-Zitat:** *"stelle auch sicher ads du immer direkt änderungen im code vornimmst und nicht nur deren dokumentation!"*

### ⚠️ REGEL #2: SYSTEMATISCH & GANZHEITLICH ARBEITEN!
**Keine Schnipsel-Fixes!**

- ❌ **FALSCH:** Nur eine Funktion fixen ohne Abhängigkeiten zu prüfen
- ❌ **FALSCH:** Demo-Daten ändern OHNE Login-Funktion zu prüfen
- ✅ **RICHTIG:** ALLE abhängigen Teile ZUSAMMEN fixen!
- ✅ **RICHTIG:** Gesamtbild verstehen BEVOR du änderst!

**Beispiel korrekter Workflow:**
```
Problem: Demo-Login funktioniert nicht
→ FALSCH: Nur Demo-Daten hinzufügen
→ RICHTIG: 
   1. Demo-Daten hinzufügen
   2. Login-Funktion prüfen
   3. Dashboard-Laden prüfen
   4. ALLE zusammen testen!
```

**User-Zitat:** *"bei anderen Änderungen immer mit einbezieht vorallem bei Fehlerkorrekturen, sie dürfen niemals einfach etwas neues erstellen oder verursachen ohne abzudecken ob das gabze mit dem Rest zusammen köuft"*

### ⚠️ REGEL #3: TRACKING-SYSTEM NUTZEN!
**VOR JEDER Änderung dokumentiere:**

```
TRACKING-TEMPLATE:
-----------------
WANN: [Jetzt / Nach X / Priorität]
WO: [Exakte Zeilen-Nummer in pzr_app.html]
WAS: [Konkrete Code-Änderung]
WARUM: [Welches Problem wird gelöst]
WESHALB: [Root-Cause / Tiefere Ursache]
ABHÄNGIGKEITEN: [Was könnte betroffen sein]
```

**Beispiel:**
```
WANN: Jetzt (Priorität 1)
WO: Zeile 1387 - initDemoData()
WAS: Demo-Patienten bekommen anamnese-Objekt
WARUM: Patient-Login crasht bei fehlendem anamnese
WESHALB: Demo-Daten waren unvollständig
ABHÄNGIGKEITEN: loginPatient(), loadPatientDashboard()
```

### ⚠️ REGEL #4: KEINE TEMPLATE-STRINGS MIT <script>!
**Technische Falle vermeiden:**

- ❌ **FALSCH:** `` const html = `<script>window.location='...'</script>` ``
- ✅ **RICHTIG:** `const html = '<scr' + 'ipt>window.location="..."</scr' + 'ipt>'`

**Warum:** Browser-Parser interpretiert inneres `<script>` Tag und beendet äußeres Script zu früh → Kompletter Code wird als Text angezeigt!

### ⚠️ REGEL #5: KEINE DOPPELTEN FUNKTIONEN!
**Eine Funktion = Eine Definition:**

- ❌ **FALSCH:** Zwei Versionen von `loadEmployeeDashboard()` im Code
- ✅ **RICHTIG:** Immer mit `grep` prüfen ob Funktion schon existiert
- ✅ **RICHTIG:** Bestehende Funktion erweitern statt neu zu erstellen

**Warum:** JavaScript nutzt immer die ERSTE Definition → zweite wird ignoriert!

---

## 📚 FEATURES ZU IMPLEMENTIEREN

### 1. LOGIN-SYSTEM

#### 1.1 Employee Login
**Anforderung:**
- Mitarbeiter können sich mit ID + Passwort anmelden
- Keine Praxis-Auswahl nötig (automatisch via practiceId)
- Verschiedene Rollen: Admin, Behandler

**Zu implementieren:**
```javascript
function loginEmployee() {
    // 1. Eingabe validieren
    // 2. Aus localStorage.pzrEmployees lesen
    // 3. Credentials prüfen
    // 4. practiceId automatisch übernehmen
    // 5. SessionStorage setzen
    // 6. Dashboard laden
}
```

**Test-Daten (Demo):**
- ID: MA-001
- Passwort: admin
- practiceId: demo-practice

#### 1.2 Patient Login
**Anforderung:**
- Patienten können sich mit Patienten-Nummer + PIN anmelden
- Zugriff auf eigene Termine und Anamnese

**Zu implementieren:**
```javascript
function loginPatient() {
    // 1. Eingabe validieren
    // 2. Aus localStorage.pzrPatients lesen
    // 3. Nummer + PIN prüfen
    // 4. Sicherer anamnese-Check (kann undefined sein!)
    // 5. SessionStorage setzen
    // 6. Dashboard laden
}
```

**Test-Daten (Demo):**
- Nummer: PAT-12345
- PIN: 1234
- MUSS anamnese-Objekt haben!

#### 1.3 Praxis-Registrierung
**Anforderung:**
- Neue Praxis kann sich registrieren
- Admin-Account wird automatisch erstellt
- Admin-ID wird aus Admin-Name generiert

**Zu implementieren:**
```javascript
function registerPractice() {
    // 1. Praxis-Daten erfassen
    // 2. Eindeutige practiceId generieren
    // 3. Admin-ID aus Name generieren (DR-MUELLER)
    // 4. In localStorage.pzrPractices speichern
    // 5. Admin in localStorage.pzrEmployees speichern
    // 6. Automatisch anmelden
}
```

### 2. DEMO-DATEN

**Anforderung:**
Vorgefertigte Test-Daten für sofortiges Testen OHNE Registrierung

**Zu implementieren:**
```javascript
function initDemoData() {
    // NUR wenn localStorage leer!
    if (!localStorage.getItem('pzrPractices')) {
        
        // Demo-Praxis
        const demoPractice = {
            id: 'demo-practice',
            name: 'Demo Praxis',
            address: 'Musterstraße 123, 12345 Musterstadt',
            phone: '0123456789',
            adminId: 'MA-001'
        };
        
        // Demo-Employee
        const demoEmployees = [{
            id: 'MA-001',
            password: 'admin',
            practiceId: 'demo-practice',
            name: 'Demo Behandler',
            role: 'Behandler',
            active: true  // WICHTIG!
        }];
        
        // Demo-Patients (VOLLSTÄNDIG mit anamnese!)
        const demoPatients = [{
            number: 'PAT-12345',
            pin: '1234',
            practiceId: 'demo-practice',
            name: 'Demo Patient',
            birthdate: '1990-01-01',
            phone: '0123456789',
            email: 'demo@patient.de',
            anamnese: {  // MUSS vorhanden sein!
                lastUpdated: new Date().toISOString(),
                allergies: '',
                medications: '',
                diseases: '',
                surgeries: '',
                familyHistory: ''
            }
        }, {
            number: 'PAT-67890',
            pin: '5678',
            practiceId: 'demo-practice',
            name: 'Zweiter Patient',
            birthdate: '1985-05-15',
            phone: '9876543210',
            email: 'zweiter@patient.de',
            anamnese: {
                lastUpdated: new Date().toISOString(),
                allergies: 'Penicillin',
                medications: 'Aspirin',
                diseases: '',
                surgeries: '',
                familyHistory: ''
            }
        }];
        
        // In localStorage speichern
        localStorage.setItem('pzrPractices', JSON.stringify([demoPractice]));
        localStorage.setItem('pzrEmployees', JSON.stringify(demoEmployees));
        localStorage.setItem('pzrPatients', JSON.stringify(demoPatients));
    }
}

// SOFORT beim Laden aufrufen!
initDemoData();
```

### 3. TERMIN-VERWALTUNG

#### 3.1 Termin erstellen
**Anforderung:**
- Behandler kann Termine für Patienten buchen
- Datum + Zeit + Patient + Behandler

**Zu implementieren:**
```javascript
function createAppointment() {
    // 1. Formular-Daten lesen
    // 2. Patient aus Dropdown
    // 3. Behandler aus Session
    // 4. Termin-Objekt erstellen
    // 5. In localStorage.pzrAppointments speichern
    // 6. Liste aktualisieren
}
```

#### 3.2 Patient-Dropdown
**Anforderung:**
- Bei Terminbuchung: Dropdown mit allen Patienten der Praxis

**Zu implementieren:**
```javascript
function loadPatients() {
    // 1. Aus localStorage.pzrPatients lesen
    // 2. Nach practiceId filtern
    // 3. Select-Element füllen
    const patients = JSON.parse(localStorage.getItem('pzrPatients') || '[]');
    const currentUser = JSON.parse(sessionStorage.getItem('currentUser'));
    const filtered = patients.filter(p => p.practiceId === currentUser.practiceId);
    
    const select = document.getElementById('appointmentPatient');
    select.innerHTML = '<option value="">Patient auswählen</option>';
    filtered.forEach(p => {
        const option = document.createElement('option');
        option.value = p.number;
        option.textContent = `${p.number} - ${p.name}`;
        select.appendChild(option);
    });
}
```

#### 3.3 Terminabsage mit Ampel-System
**Anforderung:**
- Patienten können Termine absagen
- Ampel-System: Grün (14+ Tage), Orange (2-13 Tage), Rot (0-1 Tag)
- Warnung vor möglicher Ausfallgebühr

**Zu implementieren:**
```javascript
function getTrafficLight(appointmentDate) {
    const now = new Date();
    const appointment = new Date(appointmentDate);
    const diffDays = Math.floor((appointment - now) / (1000 * 60 * 60 * 24));
    
    if (diffDays >= 14) {
        return { color: 'green', message: 'Rechtzeitige Absage - Keine Kosten' };
    } else if (diffDays >= 2) {
        return { color: 'orange', message: 'Knappe Absage - Evtl. Gebühr' };
    } else {
        return { color: 'red', message: 'WARNUNG: Ausfallrechnung möglich!' };
    }
}

function cancelAppointment(appointmentId) {
    // 1. Termin aus localStorage laden
    const appointments = JSON.parse(localStorage.getItem('pzrAppointments') || '[]');
    const apt = appointments.find(a => a.id === appointmentId);
    
    // 2. Ampel-Warnung zeigen
    const trafficLight = getTrafficLight(apt.date);
    const confirmMsg = `${trafficLight.message}\n\nMöchten Sie den Termin absagen?`;
    
    if (confirm(confirmMsg)) {
        // 3. Status auf 'cancelled-pending' setzen
        apt.status = 'cancelled-pending';
        apt.cancelledAt = new Date().toISOString();
        localStorage.setItem('pzrAppointments', JSON.stringify(appointments));
        
        // 4. Neutermin-Dialog
        requestNewAppointment(appointmentId);
        
        // 5. UI aktualisieren
        loadPatientAppointments();
    }
}

function requestNewAppointment(oldAppointmentId) {
    // Dialog für Neutermin-Wunsch
    const preferences = prompt('Wann hätten Sie gerne einen neuen Termin?\n(z.B. "Vormittags", "Nachmittags", "Freitag")');
    if (preferences) {
        // In Termin-Notiz speichern
        const appointments = JSON.parse(localStorage.getItem('pzrAppointments') || '[]');
        const apt = appointments.find(a => a.id === oldAppointmentId);
        apt.newAppointmentRequest = preferences;
        localStorage.setItem('pzrAppointments', JSON.stringify(appointments));
        
        alert('✅ Neutermin-Wunsch wurde an die Praxis übermittelt!');
    }
}
```

### 4. PWA INSTALLATION

#### 4.1 Download-Dialog
**Anforderung:**
- Auf Login-Seite: Download-Button sichtbar
- Modal mit Platform-Auswahl: Windows / Android / iOS / PWA

**Zu implementieren:**
```javascript
// beforeinstallprompt Event speichern
window.deferredPrompt = null;

window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault();
    window.deferredPrompt = e;
});

function downloadApp() {
    // Modal anzeigen mit 4 Optionen
    document.getElementById('downloadModal').style.display = 'block';
}

// WICHTIG: Diese Funktion MUSS VOR downloadApp() definiert sein!
function downloadForPlatform(platform) {
    if (platform === 'pwa') {
        // PWA Direkt-Installation
        if (window.deferredPrompt) {
            window.deferredPrompt.prompt();
            window.deferredPrompt.userChoice.then((choice) => {
                if (choice.outcome === 'accepted') {
                    console.log('PWA installiert');
                }
                window.deferredPrompt = null;
            });
        } else {
            alert('PWA Installation ist auf diesem Gerät nicht verfügbar.\nNutzen Sie stattdessen die Browser-Funktion "Zum Startbildschirm hinzufügen".');
        }
    } else if (platform === 'windows') {
        // HTML-Starter-Datei generieren
        const currentUrl = window.location.href;
        const htmlContent = '<!DOCTYPE html>\n' +
            '<html><head><meta charset="UTF-8"><title>PZR App</title></head>\n' +
            '<body><h1>PZR App wird geladen...</h1>\n' +
            '<scr' + 'ipt>window.location.href="' + currentUrl + '";</scr' + 'ipt>\n' +
            '</body></html>';
        
        const blob = new Blob([htmlContent], { type: 'text/html' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = 'PZR-App-Setup.html';
        link.click();
    } else if (platform === 'android') {
        // PWA Installation für Android
        downloadForPlatform('pwa');
    } else if (platform === 'ios') {
        // iOS Anleitung
        alert('📱 iOS Installation:\n\n1. Tippen Sie auf das Teilen-Symbol\n2. Wählen Sie "Zum Home-Bildschirm"\n3. Tippen Sie auf "Hinzufügen"\n\nDie App wird dann wie eine native App aussehen!');
    }
    
    document.getElementById('downloadModal').style.display = 'none';
}
```

#### 4.2 Backend-Initialisierung
**Anforderung:**
- JEDER kann die App installieren
- NUR beim ERSTEN Admin-Login auf einem installierten PC → Backend aktivieren
- Employees/Patients aktivieren KEIN Backend

**Zu implementieren:**
```javascript
function checkBackendInitialization() {
    // Nur prüfen wenn:
    // 1. PWA installiert (window.matchMedia)
    // 2. User ist Admin (role === 'admin')
    // 3. Noch kein Backend auf diesem PC (localStorage.isBackendPC)
    
    const currentUser = JSON.parse(sessionStorage.getItem('currentUser'));
    const isPWA = window.matchMedia('(display-mode: standalone)').matches;
    const isBackendPC = localStorage.getItem('isBackendPC') === 'true';
    
    if (isPWA && currentUser && currentUser.role === 'admin' && !isBackendPC) {
        if (confirm('🖥️ Möchten Sie diesen PC als Backend für Ihre Praxis einrichten?\n\nDies speichert alle Praxisdaten lokal auf diesem PC.')) {
            localStorage.setItem('isBackendPC', 'true');
            alert('✅ Backend wurde auf diesem PC initialisiert!\n\nAlle Praxisdaten werden jetzt hier gespeichert.');
        }
    }
}
```

### 5. ADMIN-FUNKTIONEN

#### 5.1 Admin-ID Auto-Generierung
**Anforderung:**
- Bei Praxis-Registrierung: Admin-Name → Admin-ID
- Beispiel: "Dr. Müller" → "DR-MUELLER"

**Zu implementieren:**
```javascript
function generateAdminLoginId(administratorName) {
    return administratorName
        .toUpperCase()
        .replace(/Ä/g, 'AE')
        .replace(/Ö/g, 'OE')
        .replace(/Ü/g, 'UE')
        .replace(/ß/g, 'SS')
        .replace(/\s+/g, '-')
        .replace(/[^A-Z0-9-]/g, '')
        .substring(0, 20);
}
```

#### 5.2 Employee-ID Bearbeitung
**Anforderung:**
- Admin kann Mitarbeiter-IDs nachträglich ändern
- Mit Validierung und Eindeutigkeits-Check

**Zu implementieren:**
```javascript
function editEmployeeId(oldId) {
    const newId = prompt('Neue Mitarbeiter-ID eingeben:', oldId);
    if (newId && newId !== oldId) {
        // Validierung
        if (newId.length < 3 || newId.length > 20) {
            alert('ID muss 3-20 Zeichen lang sein!');
            return;
        }
        
        // Eindeutigkeit prüfen
        const employees = JSON.parse(localStorage.getItem('pzrEmployees') || '[]');
        if (employees.some(e => e.id === newId && e.id !== oldId)) {
            alert('Diese ID wird bereits verwendet!');
            return;
        }
        
        // Ändern
        const employee = employees.find(e => e.id === oldId);
        employee.id = newId;
        localStorage.setItem('pzrEmployees', JSON.stringify(employees));
        
        alert('✅ Mitarbeiter-ID wurde geändert!');
        loadEmployeeList();
    }
}
```

#### 5.3 isOriginalAdmin Flag (Optional Enhancement)
**Anforderung:**
- Nur der URSPRÜNGLICHE Admin (der die Praxis registriert hat) sieht Mitarbeiter-Tab
- Andere Admins haben normale Admin-Rechte aber kein Employee-Management

**Zu implementieren:**
```javascript
// Bei Praxis-Registrierung
practice.adminId = generatedAdminId;
employee.isOriginalAdmin = true;  // Nur für ersten Admin!

// Bei Tab-Anzeige
if (currentUser.role === 'admin' && currentUser.isOriginalAdmin) {
    document.getElementById('employeeManagementTab').style.display = 'block';
} else {
    document.getElementById('employeeManagementTab').style.display = 'none';
}
```

### 6. PATIENT-VERWALTUNG

#### 6.1 Patient anlegen
**Anforderung:**
- Praxis kann neue Patienten registrieren
- Vollständige Stammdaten + leere Anamnese

**Zu implementieren:**
```javascript
function addPatient() {
    // Formular-Daten lesen
    const patient = {
        number: generatePatientNumber(),
        pin: generatePIN(),
        practiceId: currentUser.practiceId,
        name: document.getElementById('patientName').value,
        birthdate: document.getElementById('patientBirthdate').value,
        phone: document.getElementById('patientPhone').value,
        email: document.getElementById('patientEmail').value,
        anamnese: {  // IMMER mit anlegen!
            lastUpdated: new Date().toISOString(),
            allergies: '',
            medications: '',
            diseases: '',
            surgeries: '',
            familyHistory: ''
        }
    };
    
    // Speichern
    const patients = JSON.parse(localStorage.getItem('pzrPatients') || '[]');
    patients.push(patient);
    localStorage.setItem('pzrPatients', JSON.stringify(patients));
    
    alert(`✅ Patient angelegt!\n\nPatienten-Nummer: ${patient.number}\nPIN: ${patient.pin}`);
}
```

---

## 🔧 ARBEITS-WORKFLOW

### VOR JEDEM ARBEITSSCHRITT

1. **MASTER_PROMPT.md lesen** (diese Datei!)
2. **Tracking erstellen:**
   ```
   WANN: Jetzt
   WO: Zeile X
   WAS: Feature Y
   WARUM: Problem Z
   WESHALB: Root-Cause
   ABHÄNGIGKEITEN: A, B, C
   ```
3. **Gesamtbild verstehen:** Welche anderen Teile könnten betroffen sein?

### WÄHREND DER ARBEIT

1. **Code WIRKLICH ändern** mit `edit` Tool
2. **Tracking aktualisieren** während du arbeitest
3. **Mit grep prüfen** ob Änderung vorhanden ist
4. **Zusammenhänge beachten:** Wenn du Demo-Daten änderst, prüfe Login-Funktion!

### NACH JEDER ÄNDERUNG

1. **Verifizieren:**
   ```bash
   grep -n "nameOfFunction" pzr_app.html
   ```
2. **Testen:**
   - Öffne die App im Browser
   - Teste die geänderte Funktion
   - Teste AUCH abhängige Funktionen!
3. **Committen:**
   ```bash
   report_progress
   ```
4. **Test-Link präsentieren** (siehe unten)

---

## ⚠️ ANTI-PATTERNS (WAS NIEMALS TUN!)

### ❌ ANTI-PATTERN #1: Nur Dokumentieren
```
FALSCH:
"Ich habe die Anforderungen für Feature X dokumentiert..."

RICHTIG:
<edit> Tool nutzen und Feature X WIRKLICH implementieren!
```

### ❌ ANTI-PATTERN #2: Schnipsel-Fix
```
FALSCH:
Demo-Daten hinzufügen OHNE Login-Funktion zu prüfen

RICHTIG:
1. Demo-Daten hinzufügen
2. Login-Funktion prüfen  
3. Dashboard-Laden prüfen
4. ZUSAMMEN testen!
```

### ❌ ANTI-PATTERN #3: Template-String mit <script>
```javascript
// FALSCH:
const html = `<script>alert('test');</script>`;

// RICHTIG:
const html = '<scr' + 'ipt>alert("test");</scr' + 'ipt>';
```

### ❌ ANTI-PATTERN #4: Doppelte Funktionen
```javascript
// FALSCH:
function loadData() { /* Version 1 */ }
// ... 100 Zeilen später ...
function loadData() { /* Version 2 - wird ignoriert! */ }

// RICHTIG:
// Erst mit grep prüfen:
grep -n "function loadData" pzr_app.html
// Dann: Entweder erweitern ODER umbenennen!
```

### ❌ ANTI-PATTERN #5: Unsichere Objekt-Zugriffe
```javascript
// FALSCH:
if (!patient.anamnese.lastUpdated) { ... }  // Crash wenn anamnese undefined!

// RICHTIG:
if (!patient.anamnese || !patient.anamnese.lastUpdated) { ... }
```

---

## ✅ QUALITÄTSSICHERUNG

### Code-Review Checkliste

Vor jedem Commit prüfe:

- [ ] Code mit `edit` Tool WIRKLICH geändert?
- [ ] Mit `grep` verifiziert dass Änderung da ist?
- [ ] ALLE abhängigen Funktionen geprüft?
- [ ] Keine Duplikat-Funktionen erstellt?
- [ ] Keine Template-Strings mit <script>?
- [ ] Sichere Objekt-Zugriffe (anamnese prüfen)?
- [ ] Demo-Daten vollständig?
- [ ] Test durchgeführt?

### Test-Prozess

1. **Browser öffnen:**
   ```
   https://raw.githubusercontent.com/Existenzlos1997/claude-code/copilot/add-app-for-pzr-recommendations/PZR_App/pzr_app.html
   ```

2. **Demo-Employee Login testen:**
   - ID: MA-001
   - Passwort: admin
   - Erwartet: Dashboard lädt, Patienten-Liste sichtbar

3. **Demo-Patient Login testen:**
   - Nummer: PAT-12345
   - PIN: 1234
   - Erwartet: Dashboard lädt, Anamnese-Tab verfügbar

4. **Feature-spezifischer Test:**
   - Teste das Feature das du geändert hast
   - Teste AUCH abhängige Features!

---

## 📞 KOMMUNIKATION MIT USER

### Test-Link Format

Nach JEDEM Commit präsentiere:

```
✅ [FEATURE] IMPLEMENTIERT!

CODE-ÄNDERUNG:
- Datei: pzr_app.html
- Zeilen: X-Y
- Was: [Konkrete Änderung]

TEST-LINK:
https://raw.githubusercontent.com/Existenzlos1997/claude-code/copilot/add-app-for-pzr-recommendations/PZR_App/pzr_app.html

TESTEN:
1. [Spezifische Test-Anweisung]
2. [Erwartetes Ergebnis]
```

### Update-Template

```
🔧 WORKING ON: [Feature]

TRACKING:
- WANN: Jetzt
- WO: Zeile X
- WAS: [Änderung]
- WARUM: [Problem]
- WESHALB: [Root-Cause]

PROGRESS: [X%]
NEXT: [Nächster Schritt]
```

---

## 🎓 GELERNTE LEKTIONEN

### Lektion #1: Code > Dokumentation
**Immer ECHTEN Code schreiben, nie nur planen!**

### Lektion #2: Ganzheitlich denken
**Alle Abhängigkeiten ZUSAMMEN fixen!**

### Lektion #3: Tracking nutzen
**WANN-WO-WAS-WARUM-WESHALB vor JEDER Änderung!**

### Lektion #4: Technische Fallen kennen
**Template-Strings mit <script> = Tod! Duplikate = Verwirrung!**

### Lektion #5: Vollständige Daten
**Demo-Daten müssen KOMPLETT sein (inkl. anamnese)!**

### Lektion #6: Sichere Zugriffe
**IMMER prüfen ob Objekt existiert bevor du zugreifst!**

### Lektion #7: Systematisch arbeiten
**Plan → Implementieren → Testen → Verifizieren → Committen**

---

## 🚀 START-ANLEITUNG

### Schritt 1: Vorbereitung
1. Lies DIESES DOKUMENT KOMPLETT
2. Verstehe die KRITISCHEN REGELN
3. Verstehe das GESAMTBILD

### Schritt 2: Datei erstellen
```bash
create /home/runner/work/claude-code/claude-code/PZR_App/pzr_app.html
```

### Schritt 3: HTML-Grundgerüst
```html
<!DOCTYPE html>
<html lang="de">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>PZR App - Professionelle Zahnreinigung</title>
    <link rel="manifest" href="#" id="manifestPlaceholder">
    <style>
        /* Inline CSS hier */
    </style>
</head>
<body>
    <!-- HTML Structure hier -->
    
    <script>
        // ZUERST: Demo-Daten initialisieren
        function initDemoData() {
            // ... (siehe oben)
        }
        initDemoData();
        
        // DANN: Alle anderen Funktionen
        // ...
    </script>
</body>
</html>
```

### Schritt 4: Features Schritt für Schritt
Implementiere Features in dieser Reihenfolge:

1. ✅ Demo-Daten (initDemoData)
2. ✅ Login-System (loginEmployee, loginPatient)
3. ✅ Dashboard-Laden (loadEmployeeDashboard, loadPatientDashboard)
4. ✅ Patient-Dropdown (loadPatients)
5. ✅ Termin-Verwaltung
6. ✅ Ampel-System
7. ✅ PWA Installation
8. ✅ Admin-Funktionen

### Schritt 5: Nach jedem Feature
1. Mit `grep` verifizieren
2. Im Browser testen
3. `report_progress` aufrufen
4. Test-Link präsentieren

---

## 📋 FEATURE-CHECKLISTE

Nutze diese Checkliste um Fortschritt zu tracken:

- [ ] HTML-Grundgerüst erstellt
- [ ] Inline CSS (Styling)
- [ ] Manifest.json (PWA)
- [ ] Service Worker (PWA)
- [ ] Demo-Daten (initDemoData)
- [ ] Employee Login
- [ ] Patient Login
- [ ] Praxis-Registrierung
- [ ] Employee Dashboard
- [ ] Patient Dashboard
- [ ] Patient-Dropdown
- [ ] Termin erstellen
- [ ] Termin anzeigen
- [ ] Ampel-System
- [ ] Terminabsage
- [ ] Neutermin-Anfrage
- [ ] Download-Dialog
- [ ] PWA Installation
- [ ] Backend-Check
- [ ] Admin-ID Generierung
- [ ] Employee-ID Edit
- [ ] Patient anlegen
- [ ] Anamnese-Verwaltung

---

## 🎯 ERFOLGS-KRITERIEN

Die App ist fertig wenn:

✅ **Demo-Login funktioniert:**
- MA-001 / admin → Dashboard
- PAT-12345 / 1234 → Dashboard

✅ **Alle Features funktionieren:**
- Termin erstellen ✓
- Termin absagen mit Ampel ✓
- PWA installieren ✓
- Admin-Funktionen ✓

✅ **Code-Qualität:**
- Keine Duplikate
- Keine Template-String-Fehler
- Sichere Objekt-Zugriffe
- Vollständige Demo-Daten

✅ **Systematisch entwickelt:**
- Mit Tracking-System
- Ganzheitlich getestet
- Keine Schnipsel-Fixes

---

**VIEL ERFOLG! 🚀**

*Denk dran: Code > Dokumentation! Immer ECHTE Änderungen machen!*
