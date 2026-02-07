# PZR App - Cloud Sync Implementation

## Implementierung: JSONBin.io für automatische Geräte-Synchronisation

### Wie es funktioniert:

1. **Praxis registrieren:**
   - Benutzer füllt Praxis-Formular aus
   - System generiert eindeutigen Sync-Code (z.B. "PZR-ABC123XYZ")
   - Daten werden automatisch in Cloud gespeichert
   - Sync-Code wird angezeigt → **MUSS NOTIERT WERDEN!**

2. **Auf anderem Gerät anmelden:**
   - Sync-Code eingeben
   - Mitarbeiter-ID + Passwort (oder Patienten-Nr + PIN)
   - Daten werden automatisch von Cloud geladen

3. **Automatische Synchronisation:**
   - Jede Änderung wird sofort in Cloud gespeichert
   - Beim nächsten Login auf jedem Gerät: Aktuelle Daten werden geladen
   - Sync-Status-Anzeige zeigt an wann zuletzt synchronisiert wurde

### Technische Details:

**API:** JSONBin.io (kostenlos)
- 100.000 Requests pro Monat
- HTTPS verschlüsselt
- Keine Registrierung nötig
- Embedded API-Key in App

**Datenstruktur:**
```json
{
  "syncCode": "PZR-ABC123XYZ",
  "practice": {
    "name": "...",
    "address": "...",
    ...
  },
  "employees": [...],
  "patients": [...],
  "appointments": [...],
  "reminders": [...],
  "lastSync": "2026-02-07T22:00:00Z"
}
```

### Änderungen in der App:

#### 1. Login-Screen
- **ALT:** Role auswählen → Practice auswählen → Login
- **NEU:** Role auswählen → Sync-Code eingeben → Login

#### 2. Registrierung
- Praxis-Formular
- Erster Admin-Benutzer
- **Sync-Code wird generiert und angezeigt**
- QR-Code zum einfachen Teilen

#### 3. Sync-Funktionen
```javascript
// Automatisches Speichern nach jeder Änderung
async function saveToCloud() {
  const data = {
    practice: currentPractice,
    employees: employees,
    patients: patients,
    appointments: appointments,
    reminders: reminders,
    lastSync: new Date().toISOString()
  };
  
  await fetch('https://api.jsonbin.io/v3/b/' + binId, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'X-Master-Key': API_KEY
    },
    body: JSON.stringify(data)
  });
  
  updateSyncStatus('Synchronisiert ✓');
}

// Automatisches Laden beim Login
async function loadFromCloud(syncCode) {
  const response = await fetch('https://api.jsonbin.io/v3/b/' + binId);
  const data = await response.json();
  
  // Daten in App laden
  currentPractice = data.practice;
  employees = data.employees;
  patients = data.patients;
  ...
  
  // Auch lokal cachen (für Offline-Nutzung)
  localStorage.setItem('pzr_cache', JSON.stringify(data));
}
```

#### 4. Offline-Modus
- Daten werden auch lokal gecacht
- Bei Offline-Nutzung: Warnung anzeigen
- Beim nächsten Online-Zugriff: Automatisch synchronisieren

#### 5. Sync-Status-Anzeige
- Grüner Punkt: "Synchronisiert" (< 1 Min alt)
- Gelber Punkt: "Synchronisation läuft..."
- Roter Punkt: "Offline - Nicht synchronisiert"
- Zeigt letzte Sync-Zeit an

### Sicherheit:

1. **Sync-Code als "Passwort":**
   - Nur wer den Sync-Code kennt, kann auf Daten zugreifen
   - Sync-Code ist lang und zufällig (z.B. 16 Zeichen)

2. **HTTPS Verschlüsselung:**
   - Alle Daten werden verschlüsselt übertragen

3. **Optional: Zusätzliche Verschlüsselung:**
   - Daten können vor dem Upload verschlüsselt werden
   - AES-256 Verschlüsselung mit Sync-Code als Key

### Benutzer-Workflow:

#### Erstmalige Einrichtung:
1. App öffnen auf PC
2. "Behandler" → "Praxis registrieren"
3. Formular ausfüllen
4. **Sync-Code notieren:** `PZR-ABC123XYZ`
5. Admin-Account erstellen
6. Fertig!

#### Auf zweitem PC:
1. App öffnen
2. "Behandler" → Sync-Code eingeben: `PZR-ABC123XYZ`
3. Mitarbeiter-ID + Passwort
4. **Daten werden automatisch geladen!**

#### Auf Smartphone:
1. App öffnen/installieren
2. "Patient" → Sync-Code eingeben (oder QR-Code scannen)
3. Patientennummer + PIN
4. **Daten werden automatisch geladen!**

### Vorteile dieser Lösung:

✅ Keine Registrierung nötig
✅ Funktioniert sofort
✅ Kostenlos (bis 100k Requests/Monat)
✅ Automatische Synchronisation
✅ Funktioniert auf allen Geräten
✅ Offline-fähig (mit Cache)
✅ DSGVO-konform (da in EU gehostet werden kann)

### Nachteile:

⚠️ Abhängigkeit von JSONBin.io Service
⚠️ Bei hoher Nutzung (>100k Requests) ggf. Upgrade nötig
⚠️ Kein Echtzeit-Sync (nur bei Login/Save)

### Alternative für später:

Wenn mehr Kontrolle gewünscht: Migration zu **Firebase** oder **eigenem Server** möglich, da Datenstruktur gleich bleibt.

---

## Bereit zur Implementierung!

Soll ich fortfahren und diese Lösung implementieren?
