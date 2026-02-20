# Cloud Sync Plan für PZR App

## Problem
- localStorage funktioniert nur auf einem Gerät
- Keine Synchronisation zwischen PC, anderem PC und Smartphone möglich
- Practice-Selector funktioniert nicht richtig

## Lösung: Einfaches Cloud-Storage System

### Gewählter Ansatz: Firebase Realtime Database (kostenlos)
**Warum Firebase?**
- Kostenlos bis 1GB Speicher / 10GB/Monat Traffic
- Echtzeit-Synchronisation
- Funktioniert auf allen Plattformen (Android, iOS, Desktop)
- Kein eigener Server nötig
- HTTPS verschlüsselt
- Einfache JavaScript API

### Alternativer Ansatz (falls Firebase nicht gewünscht): LocalStorage + Sync-Codes
- Jede Praxis bekommt einen eindeutigen Sync-Code
- Export/Import Funktion für manuellen Datenaustausch
- QR-Code zum schnellen Teilen

## Implementation Details

### Neue Datenstruktur:
```
{
  "practices": {
    "{syncCode}": {
      "info": {
        "name": "...",
        "address": "...",
        ...
      },
      "employees": [...],
      "patients": [...],
      "appointments": [...],
      "reminders": [...]
    }
  }
}
```

### Login-Flow (vereinfacht):
1. **Registrierung:**
   - Praxis registrieren → Sync-Code wird generiert
   - Erster Benutzer wird Admin
   - Sync-Code muss notiert werden!

2. **Login (Behandler):**
   - Sync-Code eingeben
   - Mitarbeiter-ID + Passwort
   - Daten werden von Cloud geladen

3. **Login (Patient):**
   - Sync-Code der Praxis (QR-Code scannen oder eingeben)
   - Patientennummer + PIN
   - Daten werden von Cloud geladen

### Features:
- ✅ Auto-Save bei jeder Änderung
- ✅ Auto-Load beim Login
- ✅ Sync-Status-Indikator
- ✅ Offline-Modus mit lokalem Cache
- ✅ Export/Import als Backup
- ✅ Kein Practice-Selector mehr nötig

## Nächste Schritte:
1. Firebase Projekt einrichten (oder alternative Lösung)
2. API Integration implementieren
3. Login-System vereinfachen
4. Sync-Mechanismus implementieren
5. Testing auf mehreren Geräten
