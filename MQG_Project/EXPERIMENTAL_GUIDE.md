# MQG-Theorie: Experimental Extension Guide

## Erweiterung für physische Messungen / Extension for Physical Measurements

**Version**: 0.2.0-experimental  
**Datum / Date**: 2026-02-01  
**Status**: ✅ Bereit für experimentelle Integration / Ready for Experimental Integration

---

## 🔬 Übersicht / Overview

Die **experimentelle Erweiterung** verbindet die software-basierte MQG-Theorie mit **realen physischen Messungen** und experimentellen Umgebungen. Diese Erweiterung ermöglicht:

The **experimental extension** connects the software-based MQG theory with **real physical measurements** and experimental environments. This extension enables:

- ✅ Integration physischer Sensoren (Serial, Netzwerk, Custom)
- ✅ Echtzeit-ICQ-Berechnung aus Live-Datenströmen
- ✅ Sensor-Kalibrierung und Validierung
- ✅ Import experimenteller Daten (CSV, JSON, NumPy)
- ✅ Kontinuierliche Datenerfassung mit Pufferung

---

## 📁 Neue Module / New Modules

### 1. Hardware Interface (`src/experimental/hardware_interface.py`)

**Zweck / Purpose**: Verbindung zu physischen Sensoren und Messgeräten

**Hauptklassen / Main Classes**:

#### `SensorAdapter` (Abstract Base Class)
Basis-Interface für alle Sensor-Adapter. Jeder physische Sensor muss diese Methoden implementieren:

```python
- connect() -> bool              # Verbindung herstellen
- disconnect() -> bool           # Verbindung trennen
- read_sample() -> Any           # Einzelne Messung
- read_stream(duration, rate)    # Kontinuierlicher Datenstrom
```

#### `SerialSensorAdapter`
Für serielle Geräte (Arduino, USB-Sensoren, DAQ-Systeme)

```python
from src.experimental.hardware_interface import SerialSensorAdapter

# Beispiel: Temperatur-Sensor
sensor = SerialSensorAdapter(
    port='/dev/ttyUSB0',
    baudrate=115200
)
sensor.connect()
data = sensor.read_stream(duration=10.0, sampling_rate=100.0)
sensor.disconnect()
```

#### `NetworkSensorAdapter`
Für Netzwerk-basierte Sensoren (TCP/IP, UDP)

```python
from src.experimental.hardware_interface import NetworkSensorAdapter

# Beispiel: Remote-Sensor
sensor = NetworkSensorAdapter(
    host='192.168.1.100',
    port=8080,
    protocol='tcp'
)
sensor.connect()
sample = sensor.read_sample()
```

#### `HardwareInterface`
Haupt-Manager für alle Sensoren

```python
from src.experimental.hardware_interface import HardwareInterface, SerialSensorAdapter

# Interface initialisieren
hw = HardwareInterface()

# Sensoren registrieren
hw.register_sensor('temp', SerialSensorAdapter('/dev/ttyUSB0'))
hw.register_sensor('light', SerialSensorAdapter('/dev/ttyUSB1'))

# ICQ direkt vom Sensor messen
hw.connect_sensor('temp')
result = hw.measure_icq_from_sensor(
    'temp',
    duration=5.0,
    sampling_rate=100.0,
    label='temperature_experiment_1'
)

print(f"ICQ: {result['icq']:.4f}")
print(f"Samples: {result['sensor']['n_samples']}")
```

---

### 2. Experimental Adapter (`src/experimental/experimental_adapter.py`)

**Zweck / Purpose**: Verarbeitung experimenteller Daten und Echtzeit-Streaming

#### `ExperimentalDataAdapter`
Import und Vorverarbeitung experimenteller Daten

```python
from src.experimental.experimental_adapter import ExperimentalDataAdapter

adapter = ExperimentalDataAdapter(config={
    'preprocessing': {
        'remove_outliers': True,
        'outlier_threshold': 3.0,
        'normalize': False,
        'detrend': True
    }
})

# Daten aus verschiedenen Formaten laden
data_csv = adapter.load_from_csv('experiment_data.csv', column=0)
data_json = adapter.load_from_json('experiment_data.json', data_key='measurements')
data_numpy = adapter.load_from_numpy('experiment_data.npy')
```

#### `RealTimeProcessor`
Echtzeit-ICQ-Berechnung mit Sliding Window

```python
from src.experimental.experimental_adapter import RealTimeProcessor

# Echtzeit-Prozessor initialisieren
processor = RealTimeProcessor(
    window_size=1000,      # 1000 Samples im Puffer
    update_interval=100     # ICQ alle 100 Samples neu berechnen
)

# Daten kontinuierlich hinzufügen
for sample in data_stream:
    updated, icq = processor.add_sample(sample)
    
    if updated:
        print(f"Neues ICQ: {icq:.4f}")

# Statistiken abrufen
stats = processor.get_statistics()
print(f"Durchschnittliches ICQ: {stats['icq_mean']:.4f}")

# Ergebnisse exportieren
processor.export_results('realtime_results.json', format='json')
```

---

### 3. Calibration Manager (`src/experimental/calibration.py`)

**Zweck / Purpose**: Sensor-Kalibrierung und Validierung

#### Kalibrierung durchführen / Perform Calibration

```python
from src.experimental.calibration import CalibrationManager

cm = CalibrationManager()

# Lineare Kalibrierung
reference_values = [0, 10, 20, 30, 40, 50]  # Bekannte Referenzwerte
measured_values = [0.5, 10.6, 20.4, 30.8, 40.2, 50.5]  # Sensor-Messungen

calib = cm.calibrate_sensor(
    'my_sensor',
    reference_values=reference_values,
    measured_values=measured_values,
    method='linear'  # oder 'polynomial'
)

print(f"Steigung: {calib['coefficients']['slope']:.4f}")
print(f"Offset: {calib['coefficients']['intercept']:.4f}")
print(f"RMSE: {calib['statistics']['rmse']:.4f}")
print(f"R²: {calib['statistics']['r_squared']:.4f}")
```

#### Kalibrierung anwenden / Apply Calibration

```python
# Rohdaten kalibrieren
raw_reading = 25.7
calibrated = cm.apply_calibration('my_sensor', raw_reading)
print(f"Roh: {raw_reading:.2f} → Kalibriert: {calibrated:.2f}")
```

#### Baseline und Rauschen charakterisieren / Characterize Baseline and Noise

```python
# Baseline-Messung (Sensor in Ruhe)
baseline_data = sensor.read_stream(duration=60.0, sampling_rate=100.0)
baseline = cm.measure_baseline(baseline_data, duration=60.0)

print(f"Baseline Mittelwert: {baseline['mean']:.6f}")
print(f"Standardabweichung: {baseline['std']:.6f}")
print(f"Drift-Rate: {baseline['drift_rate']:.6e}")

# Rausch-Charakterisierung
noise_profile = cm.characterize_noise(baseline_data)
print(f"RMS-Rauschen: {noise_profile['rms_noise']:.6f}")
print(f"SNR-Schätzung: {noise_profile['snr_estimate']:.2f}")
```

---

## 🚀 Anwendungsbeispiele / Use Cases

### Beispiel 1: Einfache Sensor-Messung

```python
from src.experimental.hardware_interface import HardwareInterface, SerialSensorAdapter

# Setup
hw = HardwareInterface()
hw.register_sensor('temperature', SerialSensorAdapter('/dev/ttyUSB0', 9600))

# Messung
hw.connect_sensor('temperature')
result = hw.measure_icq_from_sensor('temperature', duration=10.0, sampling_rate=100.0)

print(f"ICQ der Temperatur-Daten: {result['icq']:.4f}")
print(f"Entropie: {result['diagnostics']['s_actual']:.4f}")

hw.disconnect_sensor('temperature')
```

### Beispiel 2: Echtzeit-Monitoring

```python
from src.experimental.hardware_interface import SerialSensorAdapter
from src.experimental.experimental_adapter import RealTimeProcessor

# Sensor und Prozessor vorbereiten
sensor = SerialSensorAdapter('/dev/ttyUSB0', 115200)
processor = RealTimeProcessor(window_size=500, update_interval=50)

sensor.connect()

try:
    # Kontinuierliche Erfassung
    for i in range(10000):
        sample = sensor.read_sample()
        updated, icq = processor.add_sample(sample)
        
        if updated:
            print(f"[{i:05d}] ICQ: {icq:.4f}")
            
            # Alarm bei niedriger Kohärenz
            if icq < 0.3:
                print("  ⚠️  Warnung: Niedrige Kohärenz!")
                
finally:
    sensor.disconnect()
    processor.export_results('monitoring_results.json')
```

### Beispiel 3: Experimentelle Daten analysieren

```python
from src.experimental.experimental_adapter import ExperimentalDataAdapter
from src.measurement.measurement_system import MeasurementSystem

# Adapter und Messsystem
adapter = ExperimentalDataAdapter()
ms = MeasurementSystem()

# Daten laden und vorverarbeiten
data = adapter.load_from_csv('experiment_results.csv', column=2)

# ICQ messen
result = ms.measure_continuous_timeseries(data, label='experiment_2024-02-01')

print(f"ICQ: {result['icq']:.4f}")
print(f"Datenpunkte: {result['data_length']}")
print(f"Wertebereich: {result['value_range']}")

# Export
ms.export_measurements('analysis_results.json', format='json')
```

### Beispiel 4: Kalibrierter Sensor-Workflow

```python
from src.experimental.hardware_interface import SerialSensorAdapter
from src.experimental.calibration import CalibrationManager

# Kalibrierungs-Manager
cm = CalibrationManager()

# Sensor
sensor = SerialSensorAdapter('/dev/ttyUSB0', 9600)
sensor.connect()

# 1. Kalibrierung mit Referenzwerten
print("Schritt 1: Kalibrierung...")
ref_values = [0, 25, 50, 75, 100]
measured = []

for ref in ref_values:
    input(f"Stelle Referenz auf {ref} ein und drücke Enter...")
    sample = sensor.read_sample()
    measured.append(sample)

calib = cm.calibrate_sensor('my_sensor', ref_values, measured, method='linear')
print(cm.get_calibration_report('my_sensor'))

# 2. Baseline-Messung
print("\nSchritt 2: Baseline-Messung...")
baseline_data = sensor.read_stream(duration=30.0, sampling_rate=10.0)
baseline = cm.measure_baseline(baseline_data, duration=30.0)

# 3. Eigentliche Messung mit Kalibrierung
print("\nSchritt 3: Messung...")
raw_data = sensor.read_stream(duration=60.0, sampling_rate=100.0)
calibrated_data = cm.apply_calibration('my_sensor', raw_data)

# 4. ICQ-Analyse
from src.measurement.measurement_system import MeasurementSystem
ms = MeasurementSystem()
result = ms.measure_continuous_timeseries(calibrated_data, label='calibrated_measurement')

print(f"\nErgebnis:")
print(f"  ICQ: {result['icq']:.4f}")
print(f"  Baseline: {baseline['mean']:.4f} ± {baseline['std']:.4f}")

sensor.disconnect()
```

---

## 🔌 Hardware-Integration / Hardware Integration

### Unterstützte Schnittstellen / Supported Interfaces

1. **Serial Port (RS232, USB)**
   - Arduino
   - Microcontroller
   - USB-Sensoren
   - DAQ-Systeme

2. **Netzwerk (TCP/IP, UDP)**
   - Remote-Sensoren
   - IoT-Geräte
   - Verteilte Messsysteme

3. **Custom Protocols**
   - Eigene Sensor-Adapter durch Subclassing von `SensorAdapter`

### Eigenen Sensor-Adapter erstellen / Create Custom Sensor Adapter

```python
from src.experimental.hardware_interface import SensorAdapter
import numpy as np

class MyCustomSensor(SensorAdapter):
    """Custom sensor for specific hardware."""
    
    def __init__(self, config=None):
        super().__init__(config)
        self.device = None
    
    def connect(self) -> bool:
        # Ihre Hardware-spezifische Verbindungslogik
        self.device = my_hardware_library.connect()
        self.is_connected = True
        return True
    
    def disconnect(self) -> bool:
        if self.device:
            self.device.close()
            self.is_connected = False
        return True
    
    def read_sample(self):
        if not self.is_connected:
            raise RuntimeError("Not connected")
        return self.device.read_value()
    
    def read_stream(self, duration, sampling_rate):
        n_samples = int(duration * sampling_rate)
        samples = []
        for _ in range(n_samples):
            samples.append(self.read_sample())
            time.sleep(1.0 / sampling_rate)
        return np.array(samples)
```

---

## 📊 Datenformate / Data Formats

### Unterstützte Import-Formate / Supported Import Formats

1. **CSV**: `adapter.load_from_csv('data.csv')`
2. **JSON**: `adapter.load_from_json('data.json')`
3. **NumPy**: `adapter.load_from_numpy('data.npy')`

### Export-Formate / Export Formats

1. **JSON**: Vollständige Metadaten + Ergebnisse
2. **CSV**: Einfache Tabelle für weitere Analyse

---

## 🎯 Best Practices

### 1. Kalibrierung
- ✅ Regelmäßige Kalibrierung (vor jedem Experiment)
- ✅ Mehrere Referenzpunkte verwenden (mindestens 5)
- ✅ Baseline-Messung vor Experimenten
- ✅ Rausch-Charakterisierung dokumentieren

### 2. Datenerfassung
- ✅ Ausreichende Sampling-Rate (mind. 2x Nyquist-Frequenz)
- ✅ Genügend Datenpunkte für ICQ-Berechnung (mind. 100)
- ✅ Metadaten speichern (Sensor, Zeit, Bedingungen)
- ✅ Rohdaten archivieren (vor Vorverarbeitung)

### 3. Echtzeit-Verarbeitung
- ✅ Passende Window-Größe wählen (abhängig vom Signal)
- ✅ Update-Intervall an Anforderungen anpassen
- ✅ Puffer-Überlauf verhindern
- ✅ Statistiken regelmäßig exportieren

---

## 🔧 Installation zusätzlicher Abhängigkeiten / Installing Additional Dependencies

Für serielle Kommunikation:
```bash
pip install pyserial
```

Für erweiterte Signal-Verarbeitung:
```bash
pip install scipy
```

---

## 📝 Nächste Schritte / Next Steps

### Sofort verfügbar / Available Now
- ✅ Hardware-Interface Framework
- ✅ Echtzeit-Prozessor
- ✅ Kalibrierungs-System
- ✅ Daten-Import/Export

### Geplante Erweiterungen / Planned Extensions
- [ ] GUI für Live-Monitoring
- [ ] Multi-Sensor-Synchronisation
- [ ] Erweiterte Signal-Filter
- [ ] Automatische Anomalie-Erkennung
- [ ] Cloud-Integration für Remote-Experimente

---

## 🆘 Troubleshooting

**Problem**: Sensor verbindet nicht
- Prüfen Sie Port/Adresse
- Überprüfen Sie Berechtigungen (Linux: `chmod 666 /dev/ttyUSB0`)
- Testen Sie mit Hardware-Tools (z.B. `screen`, `minicom`)

**Problem**: Inkonsistente ICQ-Werte
- Kalibrierung durchführen
- Baseline-Rauschen prüfen
- Sampling-Rate erhöhen
- Outlier-Filterung aktivieren

**Problem**: Speicher-Überlauf bei Echtzeit-Verarbeitung
- Window-Größe reduzieren
- Update-Intervall erhöhen
- Alte Daten exportieren und löschen

---

**Version**: 0.2.0-experimental  
**Letzte Aktualisierung / Last Update**: 2026-02-01  
**Status**: ✅ Production-Ready für experimentelle Anwendungen
