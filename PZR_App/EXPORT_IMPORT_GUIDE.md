# 📦 Export/Import System - Implementierungsleitfaden

## ✅ Option B: Einfache Cross-Device Synchronisation

### Konzept

Statt komplexer Cloud-Synchronisation nutzen wir **Export/Import mit QR-Codes** für Cross-Device-Datenaustausch.

## 🎯 Workflow

### PC 1 (Erstregistrierung):
```
1. Praxis registrieren
2. Patienten anlegen
3. Termine erstellen
4. "Daten exportieren" → QR-Code anzeigen
```

### PC 2 / Smartphone:
```
1. "Daten importieren" wählen
2. QR-Code scannen (mit Kamera)
3. ODER: JSON-Text kopieren/einfügen
4. Importieren → Fertig!
```

## 🔧 Implementierung

### 1. Export-Funktion

```javascript
function exportPracticeData() {
    const practiceId = currentPractice.id;
    
    // Alle Daten sammeln
    const exportData = {
        version: "1.0",
        exportDate: new Date().toISOString(),
        practice: getPractice(practiceId),
        patients: getPatients(practiceId),
        employees: getEmployees(practiceId),
        appointments: getAppointments(practiceId)
    };
    
    // Als JSON kodieren
    const jsonString = JSON.stringify(exportData);
    
    // QR-Code generieren
    generateQRCode(jsonString);
    
    // Download-Link anbieten
    offerDownload(jsonString, `praxis_${practiceId}_backup.json`);
}
```

### 2. Import-Funktion

```javascript
function importPracticeData(jsonString) {
    try {
        const data = JSON.parse(jsonString);
        
        // Validierung
        if (!data.version || !data.practice) {
            throw new Error("Ungültiges Datenformat");
        }
        
        // Daten importieren
        savePractice(data.practice);
        savePatients(data.patients);
        saveEmployees(data.employees);
        saveAppointments(data.appointments);
        
        alert("Daten erfolgreich importiert!");
        location.reload();
    } catch (e) {
        alert("Fehler beim Import: " + e.message);
    }
}
```

### 3. QR-Code Generation

```html
<!-- QRCode.js Library einbinden -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/qrcodejs/1.0.0/qrcode.min.js"></script>

<script>
function generateQRCode(data) {
    // QR-Code Container leeren
    document.getElementById("qrcode").innerHTML = "";
    
    // QR-Code erstellen
    new QRCode(document.getElementById("qrcode"), {
        text: data,
        width: 256,
        height: 256,
        colorDark : "#000000",
        colorLight : "#ffffff",
        correctLevel : QRCode.CorrectLevel.H
    });
}
</script>
```

### 4. UI-Komponenten

#### Export-Dialog
```html
<div id="exportDialog" style="display: none;">
    <h3>Daten exportieren</h3>
    <p>Scannen Sie den QR-Code mit einem anderen Gerät:</p>
    <div id="qrcode"></div>
    <p>Oder kopieren Sie den Text:</p>
    <textarea id="exportText" readonly></textarea>
    <button onclick="downloadBackup()">Als Datei herunterladen</button>
</div>
```

#### Import-Dialog
```html
<div id="importDialog">
    <h3>Daten importieren</h3>
    <p>QR-Code scannen oder JSON-Text einfügen:</p>
    
    <!-- Option 1: Kamera-Scanner -->
    <button onclick="startQRScanner()">📷 QR-Code scannen</button>
    <video id="qrScanner" style="display:none;"></video>
    
    <!-- Option 2: Text Input -->
    <textarea id="importText" placeholder="JSON-Daten hier einfügen..."></textarea>
    
    <!-- Option 3: Datei Upload -->
    <input type="file" id="importFile" accept=".json">
    
    <button onclick="importData()">Importieren</button>
</div>
```

## 📱 QR-Scanner Integration

### Mit jsQR Library:

```html
<script src="https://cdn.jsdelivr.net/npm/jsqr@1.4.0/dist/jsQR.js"></script>

<script>
async function startQRScanner() {
    const video = document.getElementById('qrScanner');
    video.style.display = 'block';
    
    try {
        const stream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: "environment" }
        });
        video.srcObject = stream;
        video.play();
        
        // Scan-Loop
        requestAnimationFrame(scanQR);
    } catch (err) {
        alert("Kamera-Zugriff fehlgeschlagen: " + err);
    }
}

function scanQR() {
    const video = document.getElementById('qrScanner');
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');
    
    if (video.readyState === video.HAVE_ENOUGH_DATA) {
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        
        const imageData = context.getImageData(0, 0, canvas.width, canvas.height);
        const code = jsQR(imageData.data, imageData.width, imageData.height);
        
        if (code) {
            // QR-Code gefunden!
            document.getElementById('importText').value = code.data;
            video.srcObject.getTracks().forEach(track => track.stop());
            video.style.display = 'none';
            return;
        }
    }
    
    requestAnimationFrame(scanQR);
}
</script>
```

## 🔐 Sicherheit (Optional)

### Verschlüsselung mit AES:

```javascript
// Einfache Passwort-basierte Verschlüsselung
async function encryptData(data, password) {
    // Verwende Web Crypto API
    const encoder = new TextEncoder();
    const dataBuffer = encoder.encode(JSON.stringify(data));
    
    // Key aus Passwort ableiten
    const keyMaterial = await window.crypto.subtle.importKey(
        "raw",
        encoder.encode(password),
        "PBKDF2",
        false,
        ["deriveBits", "deriveKey"]
    );
    
    const key = await window.crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: encoder.encode("pzr-salt"),
            iterations: 100000,
            hash: "SHA-256"
        },
        keyMaterial,
        { name: "AES-GCM", length: 256 },
        false,
        ["encrypt"]
    );
    
    const iv = window.crypto.getRandomValues(new Uint8Array(12));
    const encrypted = await window.crypto.subtle.encrypt(
        { name: "AES-GCM", iv },
        key,
        dataBuffer
    );
    
    return {
        encrypted: btoa(String.fromCharCode(...new Uint8Array(encrypted))),
        iv: btoa(String.fromCharCode(...iv))
    };
}
```

## 📋 Merge-Strategie

Wenn Daten auf beiden Geräten vorhanden sind:

```javascript
function mergeData(localData, importedData) {
    // Strategie: Neueste Änderung gewinnt
    const merged = {};
    
    // Praxis: Importierte überschreibt
    merged.practice = importedData.practice;
    
    // Patienten: Merge by ID, neuere Daten gewinnen
    merged.patients = mergeByTimestamp(
        localData.patients,
        importedData.patients
    );
    
    // Termine: Merge by ID
    merged.appointments = mergeByTimestamp(
        localData.appointments,
        importedData.appointments
    );
    
    // Mitarbeiter: Merge by ID
    merged.employees = mergeByTimestamp(
        localData.employees,
        importedData.employees
    );
    
    return merged;
}

function mergeByTimestamp(localArray, importedArray) {
    const merged = {};
    
    // Lokale Daten hinzufügen
    localArray.forEach(item => {
        merged[item.id] = item;
    });
    
    // Importierte Daten (überschreiben wenn neuer)
    importedArray.forEach(item => {
        if (!merged[item.id] || 
            new Date(item.lastModified) > new Date(merged[item.id].lastModified)) {
            merged[item.id] = item;
        }
    });
    
    return Object.values(merged);
}
```

## ✅ Vorteile dieser Lösung

1. **Sofort einsatzbereit** - Keine externe Abhängigkeit
2. **DSGVO-konform** - Daten bleiben lokal
3. **Einfach** - QR-Code scannen = fertig
4. **Offline-fähig** - Funktioniert ohne Internet
5. **Sicher** - Optional verschlüsselbar
6. **Backup** - Export = Datensicherung

## 🚀 Nächste Schritte

1. QRCode.js und jsQR einbinden
2. Export/Import Funktionen implementieren
3. UI für Export/Import erstellen
4. QR-Scanner integrieren
5. Merge-Strategie implementieren
6. Testen auf mehreren Geräten

## 📱 Demo-Flow

```
Szenario: Praxis auf 2 PCs und 1 Smartphone nutzen

Tag 1 - PC 1:
  → Praxis registrieren
  → 10 Patienten anlegen
  → 5 Termine erstellen
  → "Exportieren" → QR-Code anzeigen

Tag 1 - PC 2:
  → App öffnen
  → "Daten importieren"
  → QR-Code scannen
  → Fertig! Alle 10 Patienten und 5 Termine da!

Tag 2 - PC 2:
  → 3 neue Patienten anlegen
  → "Exportieren" → QR-Code

Tag 2 - Smartphone:
  → "Importieren" → QR scannen
  → Jetzt 13 Patienten total!

Tag 3 - PC 1:
  → "Importieren" → Daten von Smartphone
  → Merge: 13 Patienten, 5 Termine
```

## 🎯 Fazit

Diese Lösung bietet **90% der Funktionalität** von Firebase/Cloud-Sync mit **0% der Komplexität**.

Perfekt für:
- Kleine Praxen (1-5 Mitarbeiter)
- Gelegentlicher Gerätewechsel
- DSGVO-kritische Umgebungen
- Offline-Nutzung

Für große Praxen mit vielen gleichzeitigen Benutzern wäre später ein Upgrade auf Firebase sinnvoll.
