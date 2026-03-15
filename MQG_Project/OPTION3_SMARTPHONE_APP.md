# Option 3: Smartphone-App für MQG-ICQ Messungen

## 🎯 Überblick

**Option 3** ermöglicht es Ihnen, Ihr Smartphone als Sensor für MQG-ICQ Messungen zu verwenden:

1. **WiFi-RSSI** (Signalstärke) auslesen
2. **Beschleunigung/Gyro** (Bewegungssensoren) auslesen  
3. **Daten an Server senden** für ICQ-Berechnung
4. **Ergebnisse in Echtzeit** erhalten

## 🚀 Schnellstart

### Schritt 1: Server starten

```bash
cd MQG_Project
pip install flask numpy
python upload_receiver_server.py
```

Der Server läuft jetzt auf: **http://localhost:5000**

### Schritt 2: Smartphone vorbereiten (Android)

#### Option A: Mit Termux (empfohlen)

1. **Termux installieren** (aus F-Droid oder Play Store)
2. **Python installieren**:
   ```bash
   pkg install python
   pip install requests
   ```

3. **Script herunterladen**:
   ```bash
   curl -O http://YOUR_SERVER_IP:5000/upload_sensor_data.py
   ```
   Ersetze `YOUR_SERVER_IP` mit der IP deines Computers (z.B. `192.168.1.100`)

4. **Daten senden**:
   ```bash
   python upload_sensor_data.py --server http://YOUR_SERVER_IP:5000
   ```

#### Option B: Web-Browser (begrenzt)

Öffne einfach: `http://YOUR_SERVER_IP:5000` im Smartphone-Browser

### Schritt 3: Daten sammeln

Das Script sammelt automatisch:
- ✅ WiFi-Signalstärken aller verfügbaren Access Points
- ✅ Beschleunigungssensor-Daten (X, Y, Z)
- ✅ Gyroskop-Daten (X, Y, Z)

Und sendet sie an den Server zur ICQ-Berechnung!

## 📱 Verwendungsbeispiele

### Einmalige Messung

```bash
python upload_sensor_data.py --server http://192.168.1.100:5000 --once
```

**Ausgabe:**
```
📱 Sammle Sensordaten...
✅ WiFi-APs: 4
✅ Beschleunigung: [0.02, -0.01, 9.81]
✅ Gyro: [0.001, -0.002, 0.0]
📤 Sende Daten an http://192.168.1.100:5000/api/upload_sensors...
✅ Upload erfolgreich!
📊 ICQ-Wert: 0.7234
```

### Kontinuierliche Messungen (1 pro Sekunde)

```bash
python upload_sensor_data.py --server http://192.168.1.100:5000 --interval 1.0
```

### 10 Messungen mit 2 Sekunden Pause

```bash
python upload_sensor_data.py --server http://192.168.1.100:5000 --interval 2.0 --count 10
```

## 🔧 Erweiterte Konfiguration

### Termux-API Sensoren (bessere Datenqualität)

1. **Termux:API installieren** (separates App)
2. **API-Package installieren**:
   ```bash
   pkg install termux-api
   ```

3. **Sensoren testen**:
   ```bash
   termux-sensor -l  # Liste aller Sensoren
   termux-sensor -s accelerometer -n 1  # Beschleunigung
   termux-sensor -s gyroscope -n 1      # Gyroskop
   ```

Das Script erkennt automatisch, ob Termux-API verfügbar ist!

### WiFi-Scanning (Root erforderlich)

Für vollständiges WiFi-Scanning benötigt Android Root-Zugriff:

```bash
su -c "iwlist wlan0 scan"
```

**Ohne Root**: Das Script verwendet Fallback-Testdaten für Entwicklung.

## 📊 Server-Interface

Öffne im Browser: **http://localhost:5000**

### Funktionen:

- 📈 **Live-Statistiken**: Anzahl Uploads, letzte ICQ-Werte
- 📥 **Daten-Download**: Alle gesammelten Messungen als JSON
- 📖 **API-Dokumentation**: Endpunkte und Beispiele
- 🔗 **Script-Download**: Direkter Download des Upload-Scripts

### API-Endpunkte:

| Endpunkt | Methode | Beschreibung |
|----------|---------|--------------|
| `/` | GET | Web-Interface |
| `/api/upload_sensors` | POST | Sensordaten hochladen |
| `/api/stats` | GET | Server-Statistiken |
| `/api/download_data` | GET | Alle Daten herunterladen |
| `/upload_sensor_data.py` | GET | Upload-Script herunterladen |

## 📝 Datenformat

### Upload (JSON POST):

```json
{
  "timestamp": "2024-01-15T14:30:00",
  "wifi": {
    "HomeNetwork": -45.0,
    "Neighbor_WiFi": -67.0,
    "Coffee_Shop": -78.0
  },
  "accelerometer": [0.02, -0.01, 9.81],
  "gyroscope": [0.001, -0.002, 0.0],
  "device_info": {
    "platform": "android",
    "python_version": "3.11.2"
  }
}
```

### Response (JSON):

```json
{
  "status": "success",
  "message": "Daten erfolgreich empfangen",
  "filename": "sensor_data_20240115_143000_123456.json",
  "icq": 0.7234,
  "wifi_aps": 3
}
```

## 🎨 Use Cases

### 1. Indoor-Positionierung

Sammle WiFi-Daten an verschiedenen Orten:

```bash
# Position 1: Wohnzimmer
python upload_sensor_data.py --server http://192.168.1.100:5000 --once

# Position 2: Küche  
python upload_sensor_data.py --server http://192.168.1.100:5000 --once

# Position 3: Schlafzimmer
python upload_sensor_data.py --server http://192.168.1.100:5000 --once
```

→ Analysiere ICQ-Werte für verschiedene Räume!

### 2. Bewegungsanalyse

Kontinuierliche Messungen während Bewegung:

```bash
# Starte, dann bewege dich
python upload_sensor_data.py --server http://192.168.1.100:5000 --interval 0.5 --count 60
```

→ 60 Messungen über 30 Sekunden = Bewegungsprofil!

### 3. WiFi-Kartierung

Gehe durch dein Haus und sammle Daten:

```bash
# Automatische Kartierung
python upload_sensor_data.py --server http://192.168.1.100:5000 --interval 2.0 --count 50
```

→ 50 Messpunkte für Heatmap!

## 🔍 Troubleshooting

### Problem: "Connection refused"

**Lösung:** Stelle sicher, dass:
1. Server läuft (`python upload_receiver_server.py`)
2. Firewall erlaubt Port 5000
3. Smartphone ist im gleichen Netzwerk
4. Richtige IP-Adresse verwendet wird

**IP finden (Server-Computer):**
```bash
# Linux/Mac
ifconfig | grep "inet "

# Windows
ipconfig
```

### Problem: "WiFi-Scan fehlgeschlagen"

**Grund:** Android 10+ hat WiFi-Scanning eingeschränkt.

**Lösungen:**
- Nutze Termux:API mit Berechtigungen
- Root-Zugriff für `iwlist`
- Verwende Test-Daten (automatischer Fallback)

### Problem: "Termux-API nicht gefunden"

**Installation:**
1. Installiere "Termux:API" App aus F-Droid
2. `pkg install termux-api` in Termux
3. Gebe Sensor-Berechtigungen

### Problem: "Permission denied" für Sensoren

**Android-Berechtigungen:**
1. Settings → Apps → Termux
2. Permissions → Enable "Physical Activity", "Location"
3. Neustart der App

## 📦 Dateien in diesem System

```
MQG_Project/
├── upload_sensor_data.py        # Smartphone-Client
├── upload_receiver_server.py    # Server (empfängt Daten)
├── OPTION3_SMARTPHONE_APP.md    # Diese Dokumentation
└── sensor_uploads/              # Gespeicherte Messungen
    ├── sensor_data_20240115_143000.json
    ├── sensor_data_20240115_143001.json
    └── ...
```

## 🚀 Nächste Schritte

Nachdem Sie Daten gesammelt haben:

1. **Analysiere ICQ-Werte**:
   ```python
   import json
   from pathlib import Path
   
   # Lade alle Messungen
   data_dir = Path('sensor_uploads')
   for file in data_dir.glob('*.json'):
       with open(file) as f:
           data = json.load(f)
           print(f"ICQ: {data.get('calculated_icq', 'N/A')}")
   ```

2. **Visualisiere Daten**:
   ```bash
   python analyze_all_results.py
   ```

3. **GPS-freie Positionierung**:
   ```bash
   python test_gps_free_positioning.py
   ```

## 💡 Tipps & Tricks

### Energie sparen

```bash
# Nur alle 5 Sekunden messen
python upload_sensor_data.py --server http://IP:5000 --interval 5.0
```

### Hintergrund-Ausführung (Termux)

```bash
# Mit 'nohup' im Hintergrund laufen lassen
nohup python upload_sensor_data.py --server http://IP:5000 > output.log 2>&1 &

# Prozess finden
ps aux | grep upload_sensor

# Beenden
kill <PID>
```

### Daten-Export

```bash
# Alle Daten als JSON herunterladen
curl http://localhost:5000/api/download_data > all_measurements.json
```

### Server auf Remote-Computer

```bash
# Server mit externer IP zugänglich machen
python upload_receiver_server.py --host 0.0.0.0 --port 5000
```

Dann von jedem Gerät im Netzwerk erreichbar!

## 📚 Weitere Ressourcen

- [DATEN_ANFORDERUNGEN.md](DATEN_ANFORDERUNGEN.md) - Komplette Datenspezifikation
- [SMARTPHONE_ANLEITUNG.md](SMARTPHONE_ANLEITUNG.md) - Detaillierte Smartphone-Anleitung
- [README.md](README.md) - MQG-Theorie Grundlagen

## ✅ Zusammenfassung

**Option 3 gibt Ihnen:**

✅ Einfache Smartphone-Sensordaten-Sammlung  
✅ Automatische ICQ-Berechnung  
✅ Web-basiertes Dashboard  
✅ Keine spezielle Hardware nötig  
✅ Sofort einsatzbereit  

**Perfekt für:**
- 🏠 Indoor-Positionierung
- 📊 ICQ-Messungen
- 🔬 MQG-Experimente
- 📱 Schnelle Tests

---

**Start jetzt:**
```bash
python upload_receiver_server.py
```

Dann von Smartphone:
```bash
python upload_sensor_data.py --server http://YOUR_IP:5000
```

**Fertig! 🎉**
