# Datenanforderungen für GPS-freie Ortung mit MQG-ICQ

## Übersicht

Dieses Dokument erklärt **GENAU**, welche Daten Sie benötigen, um:
1. ICQ (Information Coherence Quotient) zu berechnen
2. GPS-freie Positionierung durchzuführen
3. Das System selbst zu testen

## 1. ICQ-Berechnung (Kern der MQG-Theorie)

### Minimale Anforderungen

```python
from core.icq_calculator import ICQCalculator

icq_calc = ICQCalculator()

# ALLES was Sie brauchen: Ein Array von Zahlen!
daten = [1.2, 1.3, 1.25, 1.28, 1.22, 1.24, 1.26]
icq_wert = icq_calc.calculate_icq(daten)

print(f"ICQ = {icq_wert}")  # Ergebnis: 0.0 bis 1.0+
```

### Detaillierte Spezifikation

| Parameter | Beschreibung | Beispiel |
|-----------|--------------|----------|
| **Eingabe** | Liste oder NumPy-Array von Zahlen | `[1.2, 1.3, 1.25, ...]` |
| **Typ** | `list`, `tuple`, oder `numpy.ndarray` | beliebig |
| **Mindestanzahl** | 2 Werte (empfohlen: 10+) | - |
| **Wertebereich** | Beliebige reelle Zahlen | -1000 bis +1000 |
| **Ausgabe** | ICQ-Wert (float) | 0.0 (chaotisch) bis 1.0+ (kohärent) |

### Was kann als Eingabe dienen?

**Beliebige Messreihen:**
- WiFi RSSI-Werte (Signalstärke in dBm)
- Beschleunigungswerte (m/s²)
- Temperaturmessungen (°C)
- Drucksensoren (hPa)
- Spannungswerte (V)
- Jede andere physikalische Größe!

**Beispiele:**

```python
# WiFi-Signalstärke über Zeit
wifi_rssi = [-45, -46, -44, -47, -45, -46, -45]
icq = icq_calc.calculate_icq(wifi_rssi)

# Beschleunigungsmesser X-Achse
accel_x = [0.02, 0.03, 0.01, 0.02, 0.025, 0.02]
icq = icq_calc.calculate_icq(accel_x)

# Temperatur über 10 Minuten
temperature = [22.1, 22.2, 22.15, 22.18, 22.2, 22.19]
icq = icq_calc.calculate_icq(temperature)
```

## 2. WiFi-basierte Positionierung

### Benötigte Daten

#### 2.1 RSSI-Messwerte (Received Signal Strength Indicator)

```python
# Format: Dictionary mit Access-Point-Namen und RSSI-Werten
wifi_messung = {
    'AP_Buero_1': -45.2,    # dBm
    'AP_Buero_2': -67.8,    # dBm
    'AP_Flur': -78.1,       # dBm
    'AP_Kueche': -82.5      # dBm
}
```

**Spezifikation:**

| Parameter | Beschreibung | Wert |
|-----------|--------------|------|
| **Einheit** | Dezibel-Milliwatt (dBm) | Standard |
| **Wertebereich** | -100 dBm (sehr schwach) bis -30 dBm (sehr stark) | typisch: -40 bis -90 |
| **Mindest-APs** | 3 für Trilateration, 5+ für Fingerprinting | empfohlen: 5-10 |
| **Messrate** | 1-10 Hz (Messungen pro Sekunde) | 1 Hz ausreichend |

#### 2.2 Fingerprint-Datenbank (Referenzmessungen)

Für WiFi-Fingerprinting brauchen Sie **bekannte Referenzpunkte**:

```python
from positioning.signal_fingerprint import SignalFingerprintDB

db = SignalFingerprintDB()

# Referenzpunkt 1: Position (2.0, 3.0) Meter
db.add_fingerprint(
    position=(2.0, 3.0),
    rssi_values={
        'AP_1': -45,
        'AP_2': -67,
        'AP_3': -78
    }
)

# Referenzpunkt 2: Position (5.0, 3.0) Meter
db.add_fingerprint(
    position=(5.0, 3.0),
    rssi_values={
        'AP_1': -62,
        'AP_2': -51,
        'AP_3': -73
    }
)

# Mindestens 9-16 Referenzpunkte für ein 10x10m Raum empfohlen
```

**Wie sammeln Sie Referenzdaten?**

1. **Manuell (Smartphone-App):**
   - App wie "WiFi Analyzer" (Android) oder "Network Analyzer" (iOS)
   - Gehen Sie zu bekannten Positionen
   - Notieren Sie RSSI-Werte für jeden Access Point
   
2. **Programmatisch (Python):**
   ```python
   import subprocess
   
   # Linux/Mac
   result = subprocess.run(['iwlist', 'wlan0', 'scan'], 
                          capture_output=True, text=True)
   
   # Windows
   result = subprocess.run(['netsh', 'wlan', 'show', 'networks', 'mode=bssid'],
                          capture_output=True, text=True)
   ```

3. **Mit unserem System (simuliert für Tests):**
   ```python
   # Siehe test_gps_free_positioning.py für vollständiges Beispiel
   ```

## 3. Bluetooth-basierte Positionierung

### Benötigte Daten

```python
# Format: Dictionary mit Beacon-ID und RSSI
bluetooth_messung = {
    'beacon_1': -52.3,  # dBm
    'beacon_2': -68.1,  # dBm
    'beacon_3': -71.8   # dBm
}
```

**Spezifikation:**

| Parameter | Beschreibung | Wert |
|-----------|--------------|------|
| **Einheit** | dBm | Standard |
| **Wertebereich** | -100 dBm bis -30 dBm | typisch: -50 bis -80 |
| **Mindest-Beacons** | 3 für Trilateration | empfohlen: 4-6 |
| **Beacon-Positionen** | Müssen bekannt sein | (x, y) in Metern |
| **Messrate** | 1-5 Hz | 1 Hz ausreichend |

**Beacon-Positionen:**

```python
beacon_positionen = {
    'beacon_1': (0, 0),      # Ecke 1
    'beacon_2': (10, 0),     # Ecke 2  
    'beacon_3': (0, 10),     # Ecke 3
    'beacon_4': (10, 10)     # Ecke 4
}
```

## 4. Inertial Navigation (IMU-basiert)

### Benötigte Daten

#### 4.1 Beschleunigungsmesser (Accelerometer)

```python
# Format: 3D-Vektor [x, y, z]
beschleunigung = [0.02, -0.01, 9.81]  # m/s²
```

**Spezifikation:**

| Parameter | Beschreibung | Wert |
|-----------|--------------|------|
| **Einheit** | Meter pro Sekunde² (m/s²) | Standard |
| **Achsen** | X, Y, Z | 3D-Vektor |
| **Ruheposition** | [0, 0, 9.81] (Gravitation) | Standard auf Erde |
| **Wertebereich** | -20 bis +20 m/s² | typisch für Bewegung |
| **Messrate** | 10-100 Hz | empfohlen: 50 Hz |
| **Auflösung** | 0.001 m/s² | 12-16 bit ADC |

#### 4.2 Gyroskop (Gyroscope)

```python
# Format: 3D-Vektor [x, y, z]
drehrate = [0.001, -0.002, 0.0005]  # rad/s
```

**Spezifikation:**

| Parameter | Beschreibung | Wert |
|-----------|--------------|------|
| **Einheit** | Radiant pro Sekunde (rad/s) | Standard |
| **Achsen** | X, Y, Z (Rotation um Achsen) | 3D-Vektor |
| **Ruheposition** | [0, 0, 0] | Keine Rotation |
| **Wertebereich** | -10 bis +10 rad/s | typisch für Bewegung |
| **Messrate** | 10-100 Hz | empfohlen: 50 Hz |

#### 4.3 Zeitstempel

```python
# Zeit zwischen Messungen (Delta-T)
dt = 0.02  # Sekunden (50 Hz = 0.02s pro Messung)
```

**Komplettes IMU-Update:**

```python
from positioning.inertial_navigation import InertialNavigator

nav = InertialNavigator()
nav.reset(position=[0, 0, 0])  # Startposition

# Jede Messung
accel = [0.02, -0.01, 9.81]  # m/s²
gyro = [0.001, -0.002, 0.0005]  # rad/s
dt = 0.02  # Sekunden seit letzter Messung

nav.update(accel, gyro, dt)

position = nav.get_position()  # [x, y, z] in Metern
```

## 5. Sensor Fusion

### Benötigte Daten

Kombiniert ALLE Quellen:

```python
from positioning.position_fusion import PositionFusion

fusion = PositionFusion()

# WiFi-Schätzung
wifi_position = [5.2, 4.8, 0]
wifi_icq = 0.85  # Qualität
fusion.add_estimate(wifi_position, 'WiFi', wifi_icq)

# Bluetooth-Schätzung
bt_position = [5.1, 4.9, 0]
bt_icq = 0.72
fusion.add_estimate(bt_position, 'Bluetooth', bt_icq)

# IMU-Schätzung
imu_position = [5.3, 4.7, 0]
imu_icq = 0.91
fusion.add_estimate(imu_position, 'IMU', imu_icq)

# Fusioniertes Ergebnis (ICQ-gewichtet)
final_position = fusion.get_fused_position()
```

## 6. Wo bekomme ich diese Daten?

### Option 1: Hardware (Real)

**WiFi:**
- Jedes Smartphone, Laptop, Raspberry Pi
- Python-Libraries: `scapy`, `wifi`, `pywifi`
- Linux: `iwlist`, `iw`
- Windows: `netsh wlan`

**Bluetooth:**
- Bluetooth-Beacons (z.B. Estimote, Kontakt.io)
- Raspberry Pi mit Bluetooth
- Python-Library: `pybluez`, `bleak`

**IMU:**
- MPU-6050 / MPU-9250 Sensor (5-10 EUR)
- Smartphone (über App + Netzwerk)
- Arduino / ESP32 mit IMU
- Python-Library: `smbus`, `adafruit-circuitpython-mpu6050`

### Option 2: Simulation (Test)

**Verwenden Sie unser Test-Script:**

```bash
python test_gps_free_positioning.py
```

Dieses Script:
- Generiert ALLE benötigten Daten automatisch
- Simuliert realistische Szenarien
- Zeigt GENAU, welche Daten wohin fließen
- Braucht KEINE Hardware!

### Option 3: Smartphone-App

**Erstellen Sie eine einfache App, die:**
1. WiFi-RSSI ausliest
2. Beschleunigung/Gyro ausliest
3. Daten an unseren Server sendet

**Beispiel (Android mit Termux):**
```bash
# Installiere Termux (Android)
pkg install python
pip install requests

# Python-Script für Daten-Upload
python upload_sensor_data.py
```

## 7. Datenformat-Beispiele

### JSON-Format (für Speicherung/Übertragung)

```json
{
  "timestamp": "2026-02-07T01:30:00Z",
  "wifi": {
    "AP_Office_1": -45.2,
    "AP_Office_2": -67.8,
    "AP_Hallway": -78.1
  },
  "bluetooth": {
    "beacon_1": -52.3,
    "beacon_2": -68.1
  },
  "imu": {
    "accel": [0.02, -0.01, 9.81],
    "gyro": [0.001, -0.002, 0.0005]
  },
  "position_estimate": {
    "x": 5.2,
    "y": 4.8,
    "z": 0.0,
    "confidence": 0.85
  }
}
```

### CSV-Format (für Zeitreihen)

```csv
timestamp,wifi_AP1,wifi_AP2,wifi_AP3,accel_x,accel_y,accel_z,gyro_x,gyro_y,gyro_z,pos_x,pos_y,icq
2026-02-07T01:30:00,-45.2,-67.8,-78.1,0.02,-0.01,9.81,0.001,-0.002,0.0005,5.2,4.8,0.85
2026-02-07T01:30:01,-45.5,-67.5,-78.3,0.03,-0.02,9.80,0.002,-0.001,0.0003,5.3,4.8,0.84
```

## 8. Praktische Checkliste

**Für WiFi-Positionierung:**
- [ ] Mindestens 3 Access Points sichtbar
- [ ] RSSI-Werte in dBm auslesen können
- [ ] 9-16 Referenzpunkte mit bekannten Positionen
- [ ] RSSI an jedem Referenzpunkt gemessen

**Für Bluetooth-Positionierung:**
- [ ] Mindestens 3 Beacons installiert
- [ ] Beacon-Positionen bekannt (x, y in Metern)
- [ ] RSSI-Werte auslesbar
- [ ] Beacons senden kontinuierlich (1 Hz+)

**Für IMU-Navigation:**
- [ ] 3-Achsen Beschleunigungsmesser
- [ ] 3-Achsen Gyroskop
- [ ] Mindestens 10 Hz Messrate
- [ ] Kalibrierung durchgeführt (Ruheposition = [0,0,9.81])

**Für ICQ-Berechnung:**
- [ ] Mindestens 10 Messwerte pro Signal
- [ ] Werte als Liste/Array verfügbar
- [ ] Beliebige physikalische Einheit möglich

## 9. Schnellstart-Code

```python
#!/usr/bin/env python3
"""Minimales Beispiel für GPS-freie Ortung."""

from core.icq_calculator import ICQCalculator
from positioning.signal_positioning import SignalPositioning
from positioning.signal_fingerprint import SignalFingerprintDB

# 1. ICQ-Calculator erstellen
icq = ICQCalculator()

# 2. Fingerprint-DB erstellen
db = SignalFingerprintDB()
db.add_fingerprint((0, 0), {'AP1': -40, 'AP2': -70})
db.add_fingerprint((5, 0), {'AP1': -60, 'AP2': -50})
db.add_fingerprint((0, 5), {'AP1': -50, 'AP2': -80})

# 3. Aktuelle RSSI-Messung
current_rssi = {'AP1': -45, 'AP2': -65}

# 4. Position schätzen
positioning = SignalPositioning(db)
position = positioning.estimate_position_knn(current_rssi, k=3)

# 5. ICQ für Qualitätsbewertung
rssi_values = list(current_rssi.values())
quality = icq.calculate_icq(rssi_values)

print(f"Position: {position}")
print(f"Qualität (ICQ): {quality}")
```

## 10. Häufige Fragen

**Q: Kann ich andere Sensoren verwenden?**
A: Ja! ICQ funktioniert mit ALLEN Messreihen. Probieren Sie: Ultraschall-Sensoren, LIDAR, Kamera-Tracking, etc.

**Q: Wie genau muss die Referenz-Datenbank sein?**
A: Für 1-2m Genauigkeit: Referenzpunkte alle 2-3 Meter. Für höhere Genauigkeit: dichter.

**Q: Funktioniert es ohne Fingerprint-DB?**
A: Ja, mit Bluetooth-Trilateration oder reiner IMU-Navigation. Fingerprinting ist aber genauer.

**Q: Wie oft muss ich ICQ berechnen?**
A: Bei jedem neuen Signal oder wenn Sie die Qualität prüfen wollen. Empfohlen: 1 Hz.

**Q: Kann ich GPS und dieses System kombinieren?**
A: Absolut! GPS ist einfach eine weitere Quelle für die Sensor Fusion.

## Nächste Schritte

1. **Testen:** `python test_gps_free_positioning.py`
2. **Verstehen:** Lesen Sie `PRAKTISCHER_EINSTIEG.md`
3. **Experimentieren:** `python demo_positioning.py`
4. **Integrieren:** Siehe `EXPERIMENTAL_GUIDE.md` für Hardware

---

**Autor:** MQG Project  
**Datum:** 2026-02-07  
**Version:** 1.0.0
