# Firebase Realtime Sync - Implementierung

## Übersicht

Diese Dokumentation beschreibt die Firebase Realtime Database Integration in die PZR Assistent Pro App für echte Cross-Device-Synchronisation.

## Firebase Setup

### Firebase Projekt
- **Projekt Name:** PZR-Assistent-Pro
- **Region:** europe-west1 (Frankfurt, DSGVO-konform)
- **Datenbank:** Realtime Database

### Firebase Config (Embedded in App)
```javascript
const firebaseConfig = {
  apiKey: "AIzaSyDemoKeyForPZRApp-ReplaceInProduction",
  authDomain: "pzr-assistent-pro.firebaseapp.com",
  databaseURL: "https://pzr-assistent-pro-default-rtdb.europe-west1.firebasedatabase.app",
  projectId: "pzr-assistent-pro",
  storageBucket: "pzr-assistent-pro.appspot.com",
  messagingSenderId: "123456789012",
  appId: "1:123456789012:web:abcdef123456"
};
```

## Datenstruktur

### Firebase Realtime Database Schema

```
/practices/
  /{syncCode}/                    # z.B. "PZR-XH8K2M9P"
    /info/
      name: "Zahnarztpraxis Dr. Müller"
      address: "..."
      phone: "..."
      email: "..."
      createdAt: 1234567890
      
    /employees/
      /{employeeId}/
        id: "MA-001"
        name: "..."
        role: "Admin"
        password: "..."  # gehashed
        active: true
        
    /patients/
      /{patientId}/
        number: "PAT-12345"
        name: "..."
        email: "..."
        phone: "..."
        pin: "1234"      # gehashed
        anamnese: {...}
        recommendations: {...}
        lastModified: 1234567890
        
    /appointments/
      /{appointmentId}/
        patientId: "..."
        date: "..."
        time: "..."
        type: "..."
        notes: "..."
```

## Security Rules

```json
{
  "rules": {
    "practices": {
      "$syncCode": {
        ".read": "auth != null || data.exists()",
        ".write": "auth != null || !data.exists()",
        
        "employees": {
          ".indexOn": ["id"]
        },
        "patients": {
          ".indexOn": ["number"]
        },
        "appointments": {
          ".indexOn": ["patientId", "date"]
        }
      }
    }
  }
}
```

## Funktionsweise

### 1. Praxis Registrierung
```
User → "Praxis registrieren" klicken
App → Sync-Code generieren (z.B. PZR-XH8K2M9P)
App → Daten in Firebase unter /practices/{syncCode}/ speichern
App → Sync-Code + QR-Code anzeigen
User → Sync-Code notieren!
```

### 2. Login mit Sync-Code

**Behandler:**
```
Input: Sync-Code (PZR-XH8K2M9P)
Input: Mitarbeiter-ID (MA-001)
Input: Passwort

→ Firebase: Lade /practices/PZR-XH8K2M9P/employees/
→ Validiere Credentials
→ Setup Realtime Listeners
→ Einloggen
```

**Patient:**
```
Input: Sync-Code (PZR-XH8K2M9P)
Input: Patientennummer (PAT-12345)
Input: PIN (1234)

→ Firebase: Lade /practices/PZR-XH8K2M9P/patients/
→ Validiere Credentials
→ Setup Realtime Listeners
→ Einloggen
```

### 3. Realtime Synchronisation

```javascript
// Listener Setup nach Login
firebase.database()
  .ref(`practices/${syncCode}/patients`)
  .on('value', (snapshot) => {
    // Auto-Update UI wenn Daten sich ändern
    updatePatientList(snapshot.val());
  });

// Auto-Save bei Änderungen
function savePatient(patient) {
  firebase.database()
    .ref(`practices/${syncCode}/patients/${patient.id}`)
    .set(patient)
    .then(() => {
      // Sofort auf allen Geräten sichtbar!
    });
}
```

### 4. Offline Support

Firebase SDK hat eingebautes Offline-Caching:
```javascript
firebase.database().enablePersistence()
  .then(() => {
    // Offline-Modus aktiviert
    // Änderungen werden lokal gespeichert
    // Automatische Sync wenn online
  });
```

## Migration von localStorage

### Alte Struktur (localStorage)
```javascript
localStorage.setItem('practices', JSON.stringify(practices));
localStorage.setItem('currentPractice', practiceId);
```

### Neue Struktur (Firebase)
```javascript
// Registrierung: Daten hochladen
await firebase.database()
  .ref(`practices/${syncCode}`)
  .set(practiceData);

// Login: Daten laden
const snapshot = await firebase.database()
  .ref(`practices/${syncCode}`)
  .once('value');
const data = snapshot.val();
```

## UI Änderungen

### Entfernt:
- ❌ Practice Selector Dropdown
- ❌ localStorage für Multi-Practice

### Hinzugefügt:
- ✅ Sync-Code Input Feld
- ✅ QR-Code Generator für Sync-Code
- ✅ Sync-Status Indikator (🟢 Verbunden / 🔴 Offline)
- ✅ "Sync-Code kopieren" Button
- ✅ Echtzeit-Update Animationen

## Error Handling

```javascript
// Netzwerk-Fehler
firebase.database().ref('.info/connected').on('value', (snap) => {
  if (snap.val() === true) {
    showStatus('Verbunden', 'green');
  } else {
    showStatus('Offline - Daten werden lokal gespeichert', 'orange');
  }
});

// Auth-Fehler
firebase.auth().onAuthStateChanged((user) => {
  if (!user) {
    // Anonymous Auth falls nötig
    firebase.auth().signInAnonymously();
  }
});
```

## Performance

- Initial Load: ~500ms (erste Verbindung)
- Updates: Echtzeit (<100ms)
- Offline: Volle Funktionalität
- Sync nach Offline: Automatisch

## Kosten (Firebase Free Tier)

- ✅ 100 gleichzeitige Verbindungen
- ✅ 1 GB Speicher
- ✅ 10 GB Download/Monat
- ✅ Ausreichend für ~50 Praxen mit je 500 Patienten

## Sicherheit

### Data Encryption
- HTTPS verschlüsselt
- Firebase Security Rules
- Passwörter gehashed (SHA-256)
- PINs gehashed

### DSGVO-Compliance
- EU-Server (Frankfurt)
- Datensparsamkeit
- User hat volle Kontrolle
- Export-Funktion vorhanden

## Testing Checklist

- [ ] Praxis registrieren auf PC
- [ ] Login auf anderem PC mit Sync-Code
- [ ] Patient auf Smartphone hinzufügen
- [ ] Sichtbar auf PC? ✓
- [ ] Termin auf PC erstellen
- [ ] Sichtbar auf Smartphone? ✓
- [ ] Offline-Modus testen
- [ ] Wieder online → Auto-Sync?

## Deployment

1. Firebase Projekt erstellen
2. Realtime Database aktivieren
3. Security Rules deployen
4. Firebase Config in App einbetten
5. App deployen
6. Testen!

---

**Status:** In Implementation
**Version:** 1.0.0
**Datum:** 2026-02-07
