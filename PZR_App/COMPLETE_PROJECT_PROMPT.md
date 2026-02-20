# 🦷 PZR APP - VOLLSTÄNDIGER PROJEKT-PROMPT
## Professionelle Zahnreinigung Verwaltungssystem

**Letzte Aktualisierung:** 2026-02-08

---

## 📋 PROJEKT-ÜBERSICHT

### Was ist die PZR App?
Eine **Single-Page HTML5 Web-Application** zur Verwaltung von professionellen Zahnreinigungen (PZR) in Zahnarztpraxen.

- **Eine einzige HTML-Datei** (pzr_app.html)
- **Keine externen Dependencies** - alles inline (CSS, JavaScript)
- **localStorage für Datenspeicherung**
- **PWA-fähig** für Installation auf allen Plattformen
- **Offline-First** Architektur

---

## 🎯 KERN-ANFORDERUNGEN

### 1. LOGIN-SYSTEM (3 Typen)

#### A) Mitarbeiter-Login
- **Feld:** Mitarbeiter-ID + Passwort
- **Demo-Account:** MA-001 / admin
- **Funktion:** `loginEmployee()`
- **Speicher:** `localStorage.pzrEmployees`
- **Prüfung:** `active === true` Flag

#### B) Patienten-Login
- **Feld:** Patienten-Nummer + PIN
- **Demo-Account:** PAT-12345 / 1234
- **Funktion:** `loginPatient()`
- **Speicher:** `localStorage.pzrPatients`
- **Sicherheit:** Sichere anamnese-Prüfung mit `&&`

#### C) Praxis-Registrierung
- **Administrator-Name** → Auto-ID-Generierung
- **Beispiel:** "Dr. Müller" → "DR-MUELLER"
- **Umlaute ersetzen:** Ä→AE, Ö→OE, Ü→UE, ß→SS
- **Funktion:** `generateAdminLoginId(administratorName)`
- **Nur erste Registrierung** wird automatisch Admin

---

### 2. DEMO-DATEN (VOLLSTÄNDIG!)

#### Demo-Praxis
```javascript
{
    id: 'demo-practice',
    name: 'Demo Zahnarztpraxis',
    address: 'Musterstraße 1, 12345 Musterstadt',
    phone: '0123-456789'
}
```

#### Demo-Employee
```javascript
{
    id: 'MA-001',
    password: 'admin',
    name: 'Demo Behandler',
    role: 'admin',
    practiceId: 'demo-practice',
    active: true  // ← WICHTIG!
}
```

#### Demo-Patienten (2 Stück)
```javascript
// Patient 1
{
    number: 'PAT-12345',
    pin: '1234',
    name: 'Max Mustermann',
    birthdate: '1985-06-15',
    phone: '0987-654321',
    practiceId: 'demo-practice',
    anamnese: {  // ← MUSS vorhanden sein!
        lastUpdated: new Date().toISOString(),
        allergies: '',
        medications: '',
        diseases: '',
        surgeries: '',
        familyHistory: ''
    }
}

// Patient 2
{
    number: 'PAT-67890',
    pin: '5678',
    name: 'Maria Musterfrau',
    birthdate: '1990-03-22',
    phone: '0456-789123',
    practiceId: 'demo-practice',
    anamnese: {
        lastUpdated: new Date().toISOString(),
        allergies: 'Penicillin',
        medications: '',
        diseases: '',
        surgeries: '',
        familyHistory: ''
    }
}
```

---

### 3. PATIENT-SUCHE (EINGABEFELD!)

**NICHT Dropdown!** Sondern Live-Suche:
```html
<input type="text" id="patientSearch" 
       placeholder="Patientennummer eingeben (z.B. PAT-12345)">
<div id="searchResults" class="suggestions"></div>
```

**Funktion:**
```javascript
document.getElementById('patientSearch').addEventListener('input', (e) => {
    const search = e.target.value.toUpperCase();
    const patients = JSON.parse(localStorage.getItem('pzrPatients') || '[]');
    const matches = patients.filter(p => 
        p.practiceId === currentUser.practiceId &&
        p.number.includes(search)
    );
    // Zeige Vorschläge
});
```

---

### 4. AMPEL-SYSTEM (14/2/0 TAGE)

#### Berechnung:
```javascript
function getTrafficLight(appointmentDate) {
    const today = new Date();
    const aptDate = new Date(appointmentDate);
    const diffTime = aptDate - today;
    const daysUntil = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    if (daysUntil >= 14) {
        return {
            color: 'green',
            icon: '🟢',
            message: 'Kostenlose Stornierung möglich',
            warningLevel: 'none'
        };
    } else if (daysUntil >= 2) {
        return {
            color: 'orange',
            icon: '🟠',
            message: 'Kurzfristige Absage - evtl. Gebühr',
            warningLevel: 'medium'
        };
    } else {
        return {
            color: 'red',
            icon: '🔴',
            message: 'WARNUNG: Ausfallgebühr wird berechnet!',
            warningLevel: 'high'
        };
    }
}
```

---

### 5. TERMINABSAGE-FLOW (6 SCHRITTE)

**Schritt 1:** Ampel-Warnung anzeigen
```javascript
const trafficLight = getTrafficLight(appointment.date);
const message = `${trafficLight.icon} ${trafficLight.message}\n\nMöchten Sie den Termin trotzdem absagen?`;
```

**Schritt 2:** Bestätigung einholen
```javascript
if (!confirm(message)) return;
```

**Schritt 3:** Status ändern
```javascript
appointment.status = 'cancelled-pending';
appointment.cancelledAt = new Date().toISOString();
```

**Schritt 4:** Folgetermin-Anfrage öffnen
```javascript
function requestNewAppointment(appointmentId) {
    const preferences = {
        timePreference: 'morning', // morning/afternoon/flexible
        preferredDays: ['Mo', 'Di', 'Mi'], // Mehrfachauswahl
        notes: 'Bevorzugt vormittags'
    };
    
    appointment.newAppointmentRequest = preferences;
    localStorage.setItem('pzrAppointments', JSON.stringify(appointments));
}
```

**Schritt 5:** Praxis-Benachrichtigung
- Zeigt in Admin-Dashboard
- "Absage-Anfragen" Tab

**Schritt 6:** Praxis bestätigt → Endgültige Löschung
```javascript
function acknowledgeCancellation(appointmentId) {
    appointment.status = 'cancelled-confirmed';
    // Jetzt endgültig löschen oder archivieren
}
```

---

### 6. PZR-EMPFEHLUNGEN (KERNFEATURE!)

#### Risiko-Bewertung (NIEDRIG/MITTEL/HOCH)
```javascript
function calculateRiskLevel(patient) {
    let score = 0;
    
    // Anamnese-Faktoren
    if (patient.anamnese.diseases.includes('Diabetes')) score += 2;
    if (patient.anamnese.medications.includes('Blutverdünner')) score += 1;
    
    // Zahnstatus
    if (patient.dentalStatus?.cariesCount > 3) score += 2;
    if (patient.dentalStatus?.periodontitis) score += 3;
    
    // Mundhygiene
    if (patient.oralHygieneLevel === 'schlecht') score += 2;
    
    // Raucher
    if (patient.smoker) score += 2;
    
    if (score >= 6) return 'HOCH';
    if (score >= 3) return 'MITTEL';
    return 'NIEDRIG';
}
```

#### Empfehlungs-Generierung
```javascript
function generatePZRRecommendations(patientId) {
    const patient = getPatient(patientId);
    const riskLevel = calculateRiskLevel(patient);
    
    return {
        riskLevel: riskLevel,
        nextPZRMonths: riskLevel === 'HOCH' ? 3 : riskLevel === 'MITTEL' ? 6 : 12,
        brushingTechnique: 'Modifizierte Bass-Technik empfohlen',
        products: {
            toothbrush: 'Elektrische Zahnbürste',
            toothpaste: 'Fluorid-Zahnpasta',
            floss: 'Zahnseide täglich'
        },
        behavioral: [
            'Zucker-Konsum reduzieren',
            'Nach Mahlzeiten Mund spülen'
        ]
    };
}
```

---

### 7. ANAMNESE-SYSTEM

#### Standard-Fragen:
- Allergien
- Medikamente
- Vorerkrankungen
- Operationen
- Familienanamnese

#### Zahn-spezifische Fragen:
- Zahnschmerzen
- Zahnfleischbluten
- Empfindlichkeit

#### Lebensstil:
- Rauchen (ja/nein)
- Zucker-Konsum

#### Zahnstatus:
```javascript
const dentalStatus = {
    cariesCount: 0,
    periodontitis: false,
    tartarLevel: 'leicht', // leicht/mittel/stark
    gumBleeding: false,
    toothMobility: false,
    lastPZRDate: null,
    findings: ''
};
```

---

### 8. PWA INSTALLATION

#### Manifest (inline):
```javascript
const manifestData = {
    name: 'PZR Verwaltung',
    short_name: 'PZR App',
    start_url: '.',
    display: 'standalone',
    background_color: '#ffffff',
    theme_color: '#2196F3',
    icons: [{
        src: 'data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><text y="75" font-size="75">🦷</text></svg>',
        sizes: '512x512',
        type: 'image/svg+xml'
    }]
};
```

#### Install-Button:
```javascript
let deferredPrompt;

window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault();
    deferredPrompt = e;
    document.getElementById('installButton').style.display = 'block';
});

function installPWA() {
    if (deferredPrompt) {
        deferredPrompt.prompt();
        deferredPrompt.userChoice.then((choiceResult) => {
            if (choiceResult.outcome === 'accepted') {
                console.log('PWA installed');
            }
            deferredPrompt = null;
        });
    }
}
```

#### Platform-spezifisch:
- **Windows:** HTML-Starter-File Download
- **Android:** APK-Wrapper Anleitung
- **iOS:** Add to Home Screen Anleitung
- **PWA:** Direkte Installation

---

### 9. BACKEND-INITIALISIERUNG

**NUR wenn ALLE Bedingungen erfüllt:**
```javascript
function shouldInitializeBackend() {
    // 1. Ist App installiert? (PWA)
    const isPWAInstalled = window.matchMedia('(display-mode: standalone)').matches;
    if (!isPWAInstalled) return false;
    
    // 2. Ist User Admin?
    const currentUser = JSON.parse(sessionStorage.getItem('currentUser'));
    if (!currentUser || currentUser.role !== 'admin') return false;
    
    // 3. Ist es erste Admin-Anmeldung auf diesem PC?
    const isBackendPC = localStorage.getItem('isBackendPC');
    if (isBackendPC) return false; // Schon initialisiert
    
    return true;
}

function initializeBackendOnFirstAdminLogin() {
    if (!shouldInitializeBackend()) return;
    
    localStorage.setItem('isBackendPC', 'true');
    localStorage.setItem('backendInitializedAt', new Date().toISOString());
    localStorage.setItem('backendPracticeId', currentUser.practiceId);
    
    alert(`✅ Backend-Server Initialisierung erfolgreich!\n\n` +
          `Dieses Gerät ist jetzt der Backend-Server für:\n${currentUser.practiceName}`);
}
```

---

## 🚨 KRITISCHE ARBEITSREGELN

### Regel #1: NIEMALS NUR DOKUMENTIEREN!
- ✅ Mit `edit` Tool ECHTEN Code schreiben!
- ❌ NICHT in Kommentaren planen!
- ❌ NICHT nur in README dokumentieren!
- ✅ IMMER in der HTML-Datei implementieren!

### Regel #2: SYSTEMATISCH & GANZHEITLICH!
- ✅ ALLE Abhängigkeiten zusammen fixen!
- ❌ NICHT isolierte Schnipsel-Fixes!
- ✅ Immer überlegen: Was ist noch betroffen?

### Regel #3: TRACKING-SYSTEM NUTZEN!
```
WANN: [Jetzt / Später / Priorität]
WO: [Zeile X in pzr_app.html]
WAS: [Konkrete Code-Änderung]
WARUM: [Welches Problem]
WESHALB: [Root-Cause]
ABHÄNGIGKEITEN: [Was betroffen]
```

### Regel #4: KEINE TEMPLATE-STRINGS MIT <script>!
```javascript
// ❌ FALSCH:
const html = `<script>alert('test')</script>`;

// ✅ RICHTIG:
const html = '<scr' + 'ipt>alert(\'test\')</scr' + 'ipt>';
```

### Regel #5: KEINE DOPPELTEN FUNKTIONEN!
- Vor jeder neuen Funktion: `grep` prüfen!
- Nur EINE Version jeder Funktion!
- JavaScript nutzt immer die ERSTE Definition!

---

## ❌ ANTI-PATTERNS (WAS NIEMALS TUN!)

### 1. Nur Dokumentieren
```javascript
// ❌ FALSCH:
// TODO: Login-Funktion implementieren
function loginEmployee() {
    // wird später implementiert
}

// ✅ RICHTIG:
function loginEmployee() {
    const employeeId = document.getElementById('employeeId').value;
    // ... komplette Implementierung
}
```

### 2. Schnipsel-Fixes
```javascript
// ❌ FALSCH:
// Nur Demo-Daten hinzufügen ohne Login-Prüfung

// ✅ RICHTIG:
// Demo-Daten + Login + Dashboard zusammen!
```

### 3. Template-String Fehler
```javascript
// ❌ FALSCH:
const modal = `<script>...</script>`;

// ✅ RICHTIG:
const modal = '<scr' + 'ipt>...</scr' + 'ipt>';
```

### 4. Doppelte Funktionen
```javascript
// ❌ FALSCH:
function loadEmployeeDashboard() { // Zeile 2219
    loadPatientList();
}
function loadEmployeeDashboard() { // Zeile 2805
    loadPatientList();
    loadEmployeeAppointmentsList(); // ← Wird nie erreicht!
}

// ✅ RICHTIG:
// Nur EINE vollständige Version!
```

### 5. Unsichere Objekt-Zugriffe
```javascript
// ❌ FALSCH:
if (!patient.anamnese.lastUpdated) { // Crash wenn anamnese undefined!

// ✅ RICHTIG:
if (!patient.anamnese || !patient.anamnese.lastUpdated) {
```

---

## 🎨 DESIGN-ANFORDERUNGEN

### User bevorzugt:
- ✅ **Das Original-Design** (pzr_app.html)
- ✅ **Gradient Background**
- ✅ **Card-Design**
- ✅ **Responsive**
- ✅ **Demo-Legende am Ende (Footer)**

### Demo-Legende Format:
```html
<footer style="background: linear-gradient(135deg, #f5f5f5, #e0e0e0); 
               padding: 30px; 
               text-align: center; 
               border-top: 3px solid #2196F3;">
    <h3>🧪 Demo-Konten zum Testen</h3>
    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 20px;">
        <!-- Demo-Employee Card -->
        <div style="background: white; padding: 20px; border-radius: 15px;">
            <h4>👨‍⚕️ Demo-Behandler</h4>
            <p><strong>ID:</strong> MA-001</p>
            <p><strong>Passwort:</strong> admin</p>
        </div>
        <!-- Demo-Patient Cards -->
    </div>
</footer>
```

---

## 📂 DATEI-STRUKTUR

```
PZR_App/
├── pzr_app.html          ← EINE einzelne HTML-Datei!
├── MASTER_PROMPT.md      ← Arbeits-Anleitung
├── COMPLETE_PROJECT_PROMPT.md  ← Dieses Dokument
└── README.md            ← User-Dokumentation
```

---

## 🧪 TEST-PROZESS

### Schritt 1: Demo-Employee Login
```
ID: MA-001
Passwort: admin
→ Dashboard lädt?
→ Patienten-Liste sichtbar?
→ Admin-Tab sichtbar?
```

### Schritt 2: Demo-Patient Login
```
Nummer: PAT-12345
PIN: 1234
→ Dashboard lädt?
→ Profil sichtbar?
→ Empfehlungen angezeigt?
```

### Schritt 3: Termin erstellen
```
→ Patient-Suche funktioniert?
→ Termin wird gespeichert?
→ Ampel wird angezeigt?
```

### Schritt 4: Termin absagen
```
→ Ampel-Warnung erscheint?
→ Folgetermin-Anfrage öffnet?
→ Status wird aktualisiert?
```

### Schritt 5: PWA Installation
```
→ Install-Button erscheint?
→ Installation funktioniert?
→ Platform-spezifische Anleitung?
```

---

## 🎯 ERFOLGS-KRITERIEN

Die App ist fertig wenn:
- [ ] Alle 3 Login-Typen funktionieren
- [ ] Demo-Daten vollständig und korrekt
- [ ] Patient-Suche (Eingabefeld) funktioniert
- [ ] Ampel-System zeigt korrekte Farben
- [ ] Terminabsage mit allen 6 Schritten
- [ ] PZR-Empfehlungen werden generiert
- [ ] Anamnese kann erfasst werden
- [ ] PWA installierbar auf allen Plattformen
- [ ] Backend wird bei erstem Admin-Login initialisiert
- [ ] Keine JavaScript-Fehler in Console
- [ ] Alle localStorage-Daten persistent
- [ ] Design entspricht User-Präferenz
- [ ] Demo-Legende im Footer sichtbar

---

## 🔗 TEST-LINK FORMAT

**Immer so präsentieren:**
```
https://raw.githubusercontent.com/Existenzlos1997/claude-code/copilot/add-app-for-pzr-recommendations/PZR_App/pzr_app.html
```

**Mit Test-Anweisungen:**
1. Link im Browser öffnen
2. Demo-Login: MA-001 / admin
3. Dashboard prüfen: Alle Funktionen sichtbar?
4. Patient-Login: PAT-12345 / 1234
5. Empfehlungen prüfen: Risiko-Anzeige + Tipps?

---

## 📝 COMMITS-KONVENTION

**Format:**
```
<TYPE>: <Kurze Beschreibung> - <Status>

Beispiele:
- FIX: Login-Funktion hinzugefügt - TESTED ✅
- FEATURE: PZR-Empfehlungen implementiert - COMPLETE ✅
- REFACTOR: Doppelte Funktionen entfernt - VERIFIED ✅
```

---

## 🎓 GELERNTE LEKTIONEN

### 1. Immer echten Code schreiben
- Nicht nur dokumentieren!
- Mit `edit` Tool arbeiten!

### 2. Demo-Daten vollständig
- anamnese-Objekt IMMER!
- active: true Flag!
- practiceId korrekt!

### 3. Sichere Objekt-Zugriffe
- Immer mit `&&` prüfen!
- Nie blindes `.property` ohne Check!

### 4. Keine Template-String-Fallen
- <script> Tags escapen!
- String-Konkatenation nutzen!

### 5. Systematisch arbeiten
- Alle Abhängigkeiten bedenken!
- Ganzheitliche Fixes!

### 6. User-Feedback ernst nehmen
- Design-Präferenzen respektieren!
- Original-Look beibehalten!

### 7. Testen, Testen, Testen!
- Nach jedem Fix testen!
- Demo-Accounts nutzen!

---

## 🚀 START-ANLEITUNG FÜR NEUEN AGENT

### Schritt 1: MASTER_PROMPT.md lesen
- Verstehe die Arbeitsweise
- Lerne die Anti-Patterns

### Schritt 2: pzr_app.html analysieren
- Welche Funktionen existieren?
- Was fehlt noch?

### Schritt 3: Prioritäten setzen
- Login zuerst!
- Dann Dashboard!
- Dann Features!

### Schritt 4: Systematisch implementieren
- Feature für Feature
- Immer mit `edit` Tool
- Nach jedem Feature testen

### Schritt 5: Qualität sichern
- Code-Review
- Tests durchführen
- User-Feedback einholen

---

## ⚡ QUICK REFERENCE

**Demo-Logins:**
- Employee: MA-001 / admin
- Patient 1: PAT-12345 / 1234
- Patient 2: PAT-67890 / 5678

**localStorage Keys:**
- `pzrPractices` - Alle Praxen
- `pzrEmployees` - Alle Mitarbeiter
- `pzrPatients` - Alle Patienten
- `pzrAppointments` - Alle Termine
- `isBackendPC` - Backend-Flag

**Wichtige Funktionen:**
- `initDemoData()` - Demo-Daten initialisieren
- `loginEmployee()` - Mitarbeiter-Login
- `loginPatient()` - Patienten-Login
- `generateAdminLoginId()` - Auto-ID erstellen
- `getTrafficLight()` - Ampel berechnen
- `calculateRiskLevel()` - Risiko bewerten
- `generatePZRRecommendations()` - Empfehlungen erstellen

---

**DIESER PROMPT ENTHÄLT ALLES AUS DEM KOMPLETTEN CHAT!** 📚✨
