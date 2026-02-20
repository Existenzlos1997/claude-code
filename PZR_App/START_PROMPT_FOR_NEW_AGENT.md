# 🚀 START-PROMPT FÜR NEUEN AGENT - PZR APP NEUAUFBAU

## AUFTRAG
Erstelle eine **komplett neue, fehlerfreie** Version der PZR App von Grund auf.

---

## 📚 QUELLEN

### 1. Vollständige Anforderungen:
**Datei:** `COMPLETE_PROJECT_PROMPT.md` (709 Zeilen)
- Alle Features dokumentiert
- Code-Beispiele für alles
- Kritische Arbeitsregeln
- Anti-Patterns

### 2. Design-Vorlage:
**Datei:** `pzr_app.html` (erste Version)
- HTML-Struktur übernehmen
- CSS-Styles kopieren
- UI-Design beibehalten
- Demo-Legende im Footer

---

## 🎯 ZIEL

**Neue Datei:** `pzr_app_v2_final.html`

**Eigenschaften:**
- ✅ Alle Features aus COMPLETE_PROJECT_PROMPT.md
- ✅ Design von pzr_app.html
- ✅ KEINE Fehler
- ✅ Komplett funktionsfähig
- ✅ Von Grund auf neu geschrieben

---

## ⚠️ KRITISCHE REGELN

### 1. NIEMALS Template-Strings mit <script>
```javascript
// ❌ FALSCH:
const html = `<script>...</script>`;

// ✅ RICHTIG:
const html = '<scr' + 'ipt>...</scr' + 'ipt>';
```

### 2. IMMER sichere Objekt-Zugriffe
```javascript
// ❌ FALSCH:
if (!patient.anamnese.lastUpdated) { }

// ✅ RICHTIG:
if (!patient.anamnese || !patient.anamnese.lastUpdated) { }
```

### 3. NIEMALS doppelte Funktionen
- Jede Funktion nur EINMAL definieren
- Vor neuer Funktion: grep prüfen!

### 4. IMMER echten Code schreiben
- ❌ NICHT nur dokumentieren
- ✅ Mit edit Tool ECHTEN Code implementieren

---

## 📋 ARBEITSSCHRITTE

### PHASE 1: Vorbereitung
1. Lies COMPLETE_PROJECT_PROMPT.md komplett
2. Öffne pzr_app.html für Design-Referenz
3. Erstelle neues Arbeits-Tracking

### PHASE 2: Grundgerüst (300-500 Zeilen)
```html
<!DOCTYPE html>
<html lang="de">
<head>
    <meta charset="UTF-8">
    <title>PZR App - Professionelle Zahnreinigung</title>
    <style>
        /* CSS aus pzr_app.html kopieren */
    </style>
</head>
<body>
    <!-- HTML-Struktur aus pzr_app.html übernehmen -->
    
    <script>
        // JavaScript komplett neu schreiben
    </script>
</body>
</html>
```

### PHASE 3: JavaScript Basis (500-800 Zeilen)
1. **localStorage Helpers**
   ```javascript
   function getFromStorage(key) { }
   function saveToStorage(key, data) { }
   ```

2. **Demo-Daten Init**
   ```javascript
   function initDemoData() {
       // Demo-Praxis
       // Demo-Employee (mit active:true!)
       // Demo-Patienten (mit vollständiger anamnese!)
   }
   ```

3. **Navigation**
   ```javascript
   function showSection(sectionId) { }
   function switchTab(tabName) { }
   ```

### PHASE 4: Login-System (800-1200 Zeilen)
```javascript
function loginEmployee() {
    const employeeId = document.getElementById('employeeId').value.trim();
    const password = document.getElementById('employeePassword').value.trim();
    
    const employees = getFromStorage('pzrEmployees') || [];
    const employee = employees.find(e => 
        e.id === employeeId && 
        e.password === password &&
        e.active === true
    );
    
    if (employee) {
        sessionStorage.setItem('currentUser', JSON.stringify(employee));
        loadEmployeeDashboard();
    } else {
        alert('❌ Ungültige Anmeldedaten');
    }
}

function loginPatient() {
    const patientNumber = document.getElementById('patientNumber').value.trim();
    const pin = document.getElementById('patientPin').value.trim();
    
    const patients = getFromStorage('pzrPatients') || [];
    const patient = patients.find(p => 
        p.number === patientNumber && 
        p.pin === pin
    );
    
    if (patient) {
        // Sichere anamnese-Prüfung!
        if (!patient.anamnese || !patient.anamnese.lastUpdated) {
            alert('⚠️ Bitte Anamnese aktualisieren');
        }
        sessionStorage.setItem('currentUser', JSON.stringify(patient));
        loadPatientDashboard();
    } else {
        alert('❌ Ungültige Anmeldedaten');
    }
}

function registerPractice() {
    const adminName = document.getElementById('administratorName').value.trim();
    const adminId = generateAdminLoginId(adminName);
    // ... Rest der Registrierung
}

function generateAdminLoginId(name) {
    return name.toUpperCase()
        .replace(/Ä/g, 'AE')
        .replace(/Ö/g, 'OE')
        .replace(/Ü/g, 'UE')
        .replace(/ß/g, 'SS')
        .replace(/\s+/g, '-')
        .replace(/[^A-Z0-9-]/g, '')
        .substring(0, 20);
}
```

### PHASE 5: Dashboards (1200-1800 Zeilen)
- loadEmployeeDashboard()
- loadPatientDashboard()
- Tab-Content laden
- Listen anzeigen

### PHASE 6: Verwaltungsfunktionen (1800-2400 Zeilen)
- addPatient()
- addEmployee()
- loadPatientList()
- loadEmployeeList()

### PHASE 7: Terminverwaltung (2400-3000 Zeilen)
- createAppointment()
- loadAppointments()
- Patient-Suche (Eingabefeld!)

### PHASE 8: Ampel-System (3000-3300 Zeilen)
```javascript
function getTrafficLight(appointmentDate) {
    const daysUntil = calculateDaysUntil(appointmentDate);
    
    if (daysUntil >= 14) {
        return { color: 'green', icon: '🟢', message: 'Kostenlose Stornierung' };
    } else if (daysUntil >= 2) {
        return { color: 'orange', icon: '🟠', message: 'Kurzfristige Absage' };
    } else {
        return { color: 'red', icon: '🔴', message: 'Ausfallgebühr!' };
    }
}

function cancelAppointment(appointmentId) {
    // 6-Schritt Prozess
    // 1. Ampel anzeigen
    // 2. Bestätigung
    // 3. Status ändern
    // 4. Folgetermin-Anfrage
    // 5. Praxis-Benachrichtigung
    // 6. Praxis bestätigt
}
```

### PHASE 9: PZR-Empfehlungen (3300-3800 Zeilen)
```javascript
function generatePZRRecommendations(patientId) {
    const patient = getPatient(patientId);
    const riskLevel = calculateRiskLevel(patient);
    
    return {
        riskLevel: riskLevel,
        nextPZRMonths: riskLevel === 'HOCH' ? 3 : riskLevel === 'MITTEL' ? 6 : 12,
        brushingTechnique: getBrushingRecommendation(patient),
        products: getProductRecommendations(patient),
        behavioral: getBehavioralAdvice(patient)
    };
}

function calculateRiskLevel(patient) {
    let score = 0;
    
    if (patient.anamnese && patient.anamnese.diseases) {
        if (patient.anamnese.diseases.includes('Diabetes')) score += 2;
    }
    if (patient.dentalStatus) {
        if (patient.dentalStatus.cariesCount > 3) score += 2;
        if (patient.dentalStatus.periodontitis) score += 3;
    }
    if (patient.smoker) score += 2;
    
    if (score >= 6) return 'HOCH';
    if (score >= 3) return 'MITTEL';
    return 'NIEDRIG';
}
```

### PHASE 10: PWA & Installation (3800-4000 Zeilen)
```javascript
// PWA Manifest inline
const manifest = {
    name: 'PZR App',
    short_name: 'PZR',
    start_url: '.',
    display: 'standalone',
    background_color: '#fff',
    theme_color: '#2196F3',
    icons: [{ src: 'data:image/svg+xml,...', sizes: '512x512', type: 'image/svg+xml' }]
};

// Backend-Initialisierung
function initializeBackendOnFirstAdminLogin() {
    const isPWA = window.matchMedia('(display-mode: standalone)').matches;
    const user = JSON.parse(sessionStorage.getItem('currentUser'));
    const isBackendPC = localStorage.getItem('isBackendPC');
    
    if (isPWA && user.role === 'admin' && !isBackendPC) {
        localStorage.setItem('isBackendPC', 'true');
        alert('✅ Dieses Gerät ist jetzt Backend-Server!');
    }
}
```

---

## ✅ QUALITÄTS-CHECKLISTE

### Vor dem Commit prüfen:
- [ ] Alle Funktionen implementiert (nicht nur dokumentiert)?
- [ ] Keine Template-Strings mit `<script>`?
- [ ] Keine doppelten Funktionen?
- [ ] Sichere Objekt-Zugriffe überall?
- [ ] Demo-Daten vollständig (mit anamnese)?
- [ ] Employee hat `active: true`?
- [ ] Design von pzr_app.html übernommen?
- [ ] Demo-Legende im Footer?

### Test-Durchlauf:
1. ✅ Employee-Login: MA-001 / admin
2. ✅ Patient-Login: PAT-12345 / 1234
3. ✅ Praxis-Registrierung funktioniert
4. ✅ Dashboards laden
5. ✅ Tabs funktionieren
6. ✅ Keine JavaScript-Fehler in Console

---

## 🔗 ERGEBNIS

**Neue Datei:** `pzr_app_v2_final.html`

**Test-Link:**
```
https://raw.githubusercontent.com/Existenzlos1997/claude-code/copilot/add-app-for-pzr-recommendations/PZR_App/pzr_app_v2_final.html
```

**Eigenschaften:**
- ~4000 Zeilen fehlerfreier Code
- Alle Features funktionsfähig
- Schönes Design
- Komplett getestet

---

## 💡 TIPPS FÜR ERFOLG

1. **Lies BEIDE Quellen zuerst komplett**
   - COMPLETE_PROJECT_PROMPT.md für Features
   - pzr_app.html für Design

2. **Arbeite systematisch Phase für Phase**
   - Nicht zu viel auf einmal
   - Jede Phase testen

3. **Nutze die Code-Beispiele**
   - Aus COMPLETE_PROJECT_PROMPT.md
   - An App anpassen

4. **Vermeide bekannte Fehler**
   - Template-Strings mit <script>
   - Unsichere Zugriffe
   - Doppelte Funktionen

5. **Teste kontinuierlich**
   - Nach jeder Phase
   - Console prüfen
   - Demo-Logins testen

---

## 🎯 ERFOLGS-KRITERIEN

Die neue App ist fertig wenn:
- ✅ Alle 15+ Features funktionieren
- ✅ Design identisch mit Original
- ✅ Keine JavaScript-Fehler
- ✅ Demo-Logins funktionieren
- ✅ Alle Dashboards laden
- ✅ PWA installierbar
- ✅ Backend-Init funktioniert

**VIEL ERFOLG BEIM NEUAUFBAU!** 🚀✨
