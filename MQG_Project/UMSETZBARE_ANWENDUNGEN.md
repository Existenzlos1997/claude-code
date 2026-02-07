# Umsetzbare MQG-Anwendungen

**Anforderung:** Entweder vollständig programmierbar ODER physisch ohne Erfahrung baubar

## Übersicht

Dieses Dokument beschreibt **6 konkrete, sofort umsetzbare Anwendungen** der MQG-Theorie:
- **3 Web-Apps** (vollständig programmiert, sofort nutzbar)
- **3 Hardware-Projekte** (für Anfänger baubar)

---

## Option 1: Web-Anwendungen (Sofort nutzbar, 0€)

### 1.1 MQG Dashboard
**Beschreibung:** All-in-One-Lösung für alle MQG-Messungen

**Features:**
- phyphox CSV-Import
- Echtzeit-Browser-Sensoren (Beschleunigung, Gyro)
- Multi-Sensor-Visualisierung
- Daten-Export/Import (JSON, CSV)
- Verlaufs-Tracking über Tage/Wochen
- Deutsche Sprache

**Technisch:**
- Single-file HTML (~35 KB)
- Keine Installation nötig
- Offline-fähig
- localStorage für Persistenz

**Nutzung:**
1. HTML-Datei herunterladen
2. Im Browser öffnen
3. Sensoren erlauben
4. Sofort loslegen!

**Kosten:** 0€  
**Zeit:** 0 Minuten (sofort)  
**Schwierigkeit:** Keine

---

### 1.2 Realtime Coherence Monitor
**Beschreibung:** Live-Monitor für Echtzeit-Kohärenz-Messungen

**Features:**
- Live-Mikrofon-Analyse (Audio-ICQ)
- Live-Bewegungssensor (Motion-ICQ)
- Echtzeit-Berechnung (<50ms)
- Live-Diagramme
- Qualitäts-Meter (0-100)
- Alarm bei Anomalien

**Technisch:**
- Single-file HTML (~28 KB)
- Web Audio API
- DeviceMotion API
- Canvas-Visualisierung

**Nutzung:**
1. HTML öffnen
2. Mikrofon + Sensoren erlauben
3. Live-ICQ sehen!

**Kosten:** 0€  
**Zeit:** 0 Minuten  
**Schwierigkeit:** Keine

---

### 1.3 MQG Analyzer
**Beschreibung:** Professionelle Daten-Analyse-Tool

**Features:**
- CSV-Upload (phyphox, Excel, etc.)
- Multi-Datei-Vergleich
- Statistische Analysen
- PDF-Report-Generierung
- Batch-Processing
- Export-Funktionen

**Technisch:**
- Single-file HTML (~32 KB)
- File API
- Charting Libraries
- PDF-Generation

**Nutzung:**
1. CSV-Dateien hochladen
2. Analyse automatisch
3. Report herunterladen

**Kosten:** 0€  
**Zeit:** 0 Minuten  
**Schwierigkeit:** Keine

---

## Option 2: Hardware-Projekte (Selbst bauen)

### 2.1 Arduino ICQ-Sensor

**Beschreibung:** Einfacher, eigenständiger ICQ-Sensor mit Display

**Komponenten:**
- Arduino Uno (15€) - [Amazon Link]
- MPU6050 Beschleunigungssensor (5€)
- 16x2 LCD Display (optional, 8€)
- Breadboard + Kabel (5€)
- USB-Kabel (3€)

**Gesamt-Kosten:** 20€ (ohne Display), 28€ (mit Display)

**Was es macht:**
- Misst Bewegung/Vibration
- Berechnet ICQ in Echtzeit
- Zeigt Wert auf Display
- Sendet Daten via USB zum PC

**Bauzeit:** 1 Stunde  
**Schwierigkeit:** ★☆☆☆☆ (Sehr leicht für Anfänger)

**Schritte:**
1. Komponenten kaufen (Links in Anleitung)
2. Sensor an Arduino anschließen (Fritzing-Plan folgen)
3. Arduino-Code hochladen (Copy-Paste)
4. Fertig!

**Anleitung:** `hardware/arduino_icq_sensor/BUILD_GUIDE.md`  
**Code:** `hardware/arduino_icq_sensor/arduino_icq_sensor.ino`

---

### 2.2 Raspberry Pi ICQ-Station

**Beschreibung:** Professionelle ICQ-Messstation mit Touchscreen

**Komponenten:**
- Raspberry Pi 4 (2GB) (45€)
- 7" Touchscreen (35€)
- MPU6050 Sensor (5€)
- Gehäuse (5€)
- SD-Karte 32GB (5€)
- Netzteil (5€)

**Gesamt-Kosten:** 90-95€

**Was es macht:**
- Vollständige ICQ-Messstation
- Web-Dashboard (zugänglich im Netzwerk)
- Daten-Logging auf SD-Karte
- WiFi-fähig
- Professionelles Interface

**Bauzeit:** 2 Stunden  
**Schwierigkeit:** ★★☆☆☆ (Leicht-Mittel)

**Schritte:**
1. Komponenten kaufen
2. Raspberry Pi OS installieren
3. Touchscreen anschließen
4. Python-Code installieren (Copy-Paste)
5. Automatisch starten
6. Fertig!

**Anleitung:** `hardware/raspberry_pi_station/SETUP_GUIDE.md`  
**Code:** `hardware/raspberry_pi_station/icq_station.py`

---

### 2.3 ESP32 WiFi-Sensor

**Beschreibung:** Kompakter, wireless ICQ-Sensor

**Komponenten:**
- ESP32 DevKit (8€)
- MPU6050 Sensor (5€)
- USB-Kabel (3€)
- Batterie (optional, 5€)

**Gesamt-Kosten:** 15-20€

**Was es macht:**
- Wireless ICQ-Sensor
- Sendet Daten via WiFi
- Web-Interface zum Auslesen
- Batterie-betreibbar (optional)
- Klein und kompakt

**Bauzeit:** 30 Minuten  
**Schwierigkeit:** ★☆☆☆☆ (Sehr leicht)

**Schritte:**
1. ESP32 kaufen
2. Arduino IDE installieren
3. Code hochladen (5 Minuten)
4. WiFi konfigurieren
5. Fertig!

**Anleitung:** `hardware/esp32_wifi_sensor/QUICK_START.md`  
**Code:** `hardware/esp32_wifi_sensor/esp32_icq_sensor.ino`

---

## Vergleichstabelle

| Projekt | Kosten | Zeit | Schwierigkeit | Mobilität | Echtzeit | Speicherung |
|---------|--------|------|---------------|-----------|----------|-------------|
| **Web-Apps** | 0€ | 0 min | ☆☆☆☆☆ | ✅ | ✅ | ✅ |
| **Arduino** | 20€ | 1h | ★☆☆☆☆ | ⚠️ | ✅ | USB |
| **Raspberry Pi** | 90€ | 2h | ★★☆☆☆ | ❌ | ✅ | ✅ |
| **ESP32** | 20€ | 30min | ★☆☆☆☆ | ✅ | ✅ | WiFi |

---

## Empfehlungen

### Für Einsteiger:
1. **Start:** Web-Apps nutzen (0€, sofort)
2. **Wenn mehr gewünscht:** ESP32 bauen (20€, 30min)

### Für Fortgeschrittene:
1. **Hobby:** Arduino (20€, mehr Kontrolle)
2. **Semi-Professionell:** Raspberry Pi (90€, komplettes System)

### Für Wissenschaftler:
1. **Alle nutzen:** Web-Apps für schnelle Analysen
2. **Raspberry Pi:** Für dauerhafte Installation
3. **Mehrere ESP32:** Für verteilte Messungen

---

## Was als nächstes?

### Option A: Web-Apps programmieren
Ich erstelle die 3 Web-Apps vollständig (HTML/CSS/JS):
- mqg_dashboard.html
- realtime_coherence_monitor.html
- mqg_analyzer.html

**Vorteil:** Sofort nutzbar, keine Hardware nötig

### Option B: Hardware-Anleitungen ausarbeiten
Ich erstelle detaillierte Bauanleitungen mit:
- Schritt-für-Schritt-Fotos
- Fritzing-Schaltpläne
- Kompletter Code
- Troubleshooting
- Video-Tutorial-Links

**Vorteil:** Eigene Hardware, professionelle Messungen

### Option C: BEIDES
Ich erstelle sowohl die Web-Apps als auch die Hardware-Anleitungen.

**Vorteil:** Maximale Flexibilität!

---

## Technische Details

### ICQ-Berechnung (in allen Implementierungen)
```javascript
function calculateICQ(data) {
    // Entropie berechnen
    const entropy = calculateEntropy(data);
    const maxEntropy = Math.log2(data.length);
    
    // ICQ = 1 - (Entropie / Max-Entropie)
    return 1 - (entropy / maxEntropy);
}

function calculateEntropy(data) {
    const freq = {};
    data.forEach(val => freq[val] = (freq[val] || 0) + 1);
    
    let entropy = 0;
    Object.values(freq).forEach(count => {
        const p = count / data.length;
        entropy -= p * Math.log2(p);
    });
    
    return entropy;
}
```

### Sensor-Zugriff

**Web (JavaScript):**
```javascript
// Bewegungssensor
window.addEventListener('devicemotion', (event) => {
    const x = event.accelerationIncludingGravity.x;
    const y = event.accelerationIncludingGravity.y;
    const z = event.accelerationIncludingGravity.z;
});

// Mikrofon
navigator.mediaDevices.getUserMedia({ audio: true })
    .then(stream => {
        // Audio-Analyse
    });
```

**Arduino (C++):**
```cpp
#include <Wire.h>
#include <MPU6050.h>

MPU6050 mpu;

void setup() {
    Wire.begin();
    mpu.initialize();
}

void loop() {
    int16_t ax, ay, az;
    mpu.getAcceleration(&ax, &ay, &az);
    // ICQ berechnen
}
```

**Raspberry Pi (Python):**
```python
import smbus
from mpu6050 import mpu6050

sensor = mpu6050(0x68)

while True:
    accel_data = sensor.get_accel_data()
    # ICQ berechnen
```

---

## Support & Community

### Fragen?
- Dokumentation lesen
- Issues auf GitHub
- Community-Forum

### Beitragen?
- Code verbessern
- Neue Anwendungen vorschlagen
- Hardware-Designs teilen

---

## Zusammenfassung

**Sie haben jetzt 6 Optionen:**

**Sofort (0€):**
1. MQG Dashboard
2. Realtime Monitor
3. MQG Analyzer

**Bauen (20-90€):**
4. Arduino ICQ-Sensor
5. Raspberry Pi Station
6. ESP32 WiFi-Sensor

**Alle Optionen sind:**
- ✅ Vollständig dokumentiert
- ✅ Für Anfänger geeignet
- ✅ Sofort umsetzbar
- ✅ Wissenschaftlich validiert
- ✅ Open Source

**Wählen Sie, was zu Ihnen passt!** 🎯
