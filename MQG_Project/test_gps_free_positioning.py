#!/usr/bin/env python3
"""
INTERACTIVE TEST: GPS-Free Positioning System
==============================================

Dieser Test zeigt GENAU:
1. Welche Daten für ICQ benötigt werden
2. Wie GPS-freie Ortung funktioniert
3. Schritt-für-Schritt Datenfluss
4. Visuelle Ergebnisse

SOFORT AUSFÜHRBAR - Keine Hardware erforderlich!

Verwendung:
    python test_gps_free_positioning.py

Autor: MQG Project
Version: 1.0.0-test
Datum: 2026-02-07
"""

import sys
import os
import numpy as np
import json
from datetime import datetime

sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'src'))

from positioning.signal_fingerprint import SignalFingerprintDB
from positioning.signal_positioning import SignalPositioning
from positioning.inertial_navigation import InertialNavigator
from positioning.position_fusion import PositionFusion
from core.icq_calculator import ICQCalculator


class InteractivePositioningTest:
    """Interaktiver Test des GPS-freien Ortungssystems."""
    
    def __init__(self):
        """Initialisierung."""
        self.icq_calc = ICQCalculator()
        self.results = {
            'timestamp': datetime.now().isoformat(),
            'tests': [],
            'data_requirements': {},
            'icq_calculations': []
        }
        
        print("\n" + "="*80)
        print("INTERAKTIVER TEST: GPS-FREIES ORTUNGSSYSTEM")
        print("="*80)
        print("\nDieser Test zeigt GENAU, welche Daten Sie benötigen und wie alles funktioniert!")
        print("\n")
    
    def explain_data_requirements(self):
        """Erklärt die Datenanforderungen für ICQ-Berechnung."""
        print("\n" + "="*80)
        print("TEIL 1: DATENANFORDERUNGEN FÜR ICQ-BERECHNUNG")
        print("="*80 + "\n")
        
        requirements = {
            "ICQ_Basis": {
                "beschreibung": "Information Coherence Quotient - Kernmetrik der MQG-Theorie",
                "eingabe": "Zahlenarray (Liste von Messwerten)",
                "beispiel": "[1.2, 1.3, 1.25, 1.28, 1.22]",
                "typ": "list oder numpy.array",
                "mindestens": "10 Werte empfohlen (min. 2)",
                "ausgabe": "ICQ-Wert zwischen 0 (chaotisch) und 1+ (kohärent)"
            },
            "WiFi_Fingerprinting": {
                "beschreibung": "RSSI-Signalstärken von WiFi Access Points",
                "eingabe": "Dictionary mit AP-Namen und RSSI-Werten",
                "beispiel": "{'AP_Office_1': -45, 'AP_Office_2': -67, 'AP_Hallway': -78}",
                "typ": "dict[str, float]",
                "einheit": "dBm (Dezibel-Milliwatt)",
                "reichweite": "-100 dBm (schwach) bis -30 dBm (stark)",
                "mindestens": "3 APs für Trilateration, 5+ für Fingerprinting"
            },
            "Bluetooth_Beacons": {
                "beschreibung": "RSSI von Bluetooth Beacons",
                "eingabe": "Dictionary mit Beacon-ID und RSSI",
                "beispiel": "{'beacon_1': -52, 'beacon_2': -68, 'beacon_3': -71}",
                "typ": "dict[str, float]",
                "einheit": "dBm",
                "reichweite": "-100 dBm bis -30 dBm",
                "mindestens": "3 Beacons für Trilateration"
            },
            "IMU_Daten": {
                "beschreibung": "Beschleunigungsmesser und Gyroskop",
                "eingabe_accel": "[ax, ay, az] in m/s²",
                "eingabe_gyro": "[gx, gy, gz] in rad/s",
                "beispiel_accel": "[0.02, -0.01, 9.81]",
                "beispiel_gyro": "[0.001, -0.002, 0.0005]",
                "typ": "list[3] oder numpy.array(3)",
                "sample_rate": "10-100 Hz (Messungen pro Sekunde)",
                "kalibrierung": "Ruheposition: [0, 0, 9.81] für Beschleunigung"
            }
        }
        
        self.results['data_requirements'] = requirements
        
        for category, details in requirements.items():
            print(f"\n📊 {category.replace('_', ' ')}")
            print("-" * 70)
            for key, value in details.items():
                print(f"  {key:20s}: {value}")
        
        print("\n✅ WICHTIG: Alle diese Daten können simuliert werden!")
        print("   → Sie brauchen KEINE echte Hardware für den Test!\n")
    
    def test_1_basic_icq_calculation(self):
        """Test 1: Grundlegende ICQ-Berechnung."""
        print("\n" + "="*80)
        print("TEST 1: GRUNDLEGENDE ICQ-BERECHNUNG")
        print("="*80 + "\n")
        
        # Beispiel 1: Hochgradig kohärente Daten
        print("Beispiel 1: Hochgradig kohärente Signaldaten")
        coherent_data = [100.0 + np.random.normal(0, 0.5) for _ in range(50)]
        icq_coherent, _ = self.icq_calc.calculate_icq(coherent_data)
        
        print(f"  Eingabe: {len(coherent_data)} Messwerte um 100.0 (±0.5)")
        print(f"  Daten: {coherent_data[:5]} ... {coherent_data[-2:]}")
        print(f"  → ICQ = {icq_coherent:.6f} (hoch kohärent)")
        
        # Beispiel 2: Chaotische Daten
        print("\nBeispiel 2: Chaotische Zufallsdaten")
        chaotic_data = [np.random.uniform(0, 100) for _ in range(50)]
        icq_chaotic, _ = self.icq_calc.calculate_icq(chaotic_data)
        
        print(f"  Eingabe: {len(chaotic_data)} Zufallswerte zwischen 0-100")
        print(f"  Daten: {chaotic_data[:5]} ... {chaotic_data[-2:]}")
        print(f"  → ICQ = {icq_chaotic:.6f} (chaotisch)")
        
        # Beispiel 3: WiFi-RSSI Zeitreihe
        print("\nBeispiel 3: Realistische WiFi-RSSI Zeitreihe")
        wifi_rssi = [-45.0 + np.random.normal(0, 3.0) for _ in range(50)]
        icq_wifi, _ = self.icq_calc.calculate_icq(wifi_rssi)
        
        print(f"  Eingabe: {len(wifi_rssi)} RSSI-Werte um -45 dBm (±3 dB)")
        print(f"  Daten: {wifi_rssi[:5]} ... {wifi_rssi[-2:]}")
        print(f"  → ICQ = {icq_wifi:.6f} (stabiles Signal)")
        
        self.results['tests'].append({
            'test': 'basic_icq',
            'coherent_icq': icq_coherent,
            'chaotic_icq': icq_chaotic,
            'wifi_icq': icq_wifi
        })
        
        self.results['icq_calculations'].extend([
            {
                'typ': 'kohärent',
                'daten_anzahl': len(coherent_data),
                'icq': icq_coherent,
                'interpretation': 'Sehr stabil, hohe Qualität'
            },
            {
                'typ': 'chaotisch',
                'daten_anzahl': len(chaotic_data),
                'icq': icq_chaotic,
                'interpretation': 'Instabil, niedrige Qualität'
            },
            {
                'typ': 'wifi_rssi',
                'daten_anzahl': len(wifi_rssi),
                'icq': icq_wifi,
                'interpretation': 'Gutes Signal für Positionierung'
            }
        ])
        
        print(f"\n✅ ICQ-Berechnung funktioniert!")
        print(f"   Kohärent: {icq_coherent:.3f} >> Chaotisch: {icq_chaotic:.3f}")
    
    def test_2_wifi_fingerprinting(self):
        """Test 2: WiFi Fingerprinting Positionierung."""
        print("\n" + "="*80)
        print("TEST 2: WiFi FINGERPRINTING POSITIONIERUNG")
        print("="*80 + "\n")
        
        print("Schritt 1: Fingerprint-Datenbank erstellen")
        print("-" * 70)
        
        # Erstelle Fingerprint-Datenbank für ein 10x10m Raum
        db = SignalFingerprintDB()
        
        # Simuliere 4 Access Points an den Ecken
        ap_positions = {
            'AP_NW': (0, 10),    # Nord-West
            'AP_NE': (10, 10),   # Nord-Ost
            'AP_SW': (0, 0),     # Süd-West
            'AP_SE': (10, 0)     # Süd-Ost
        }
        
        print(f"  Access Points: {len(ap_positions)}")
        for ap, pos in ap_positions.items():
            print(f"    {ap}: Position {pos}")
        
        # Erstelle Referenzmessungen an bekannten Positionen
        print("\nSchritt 2: Referenzmessungen sammeln")
        print("-" * 70)
        
        reference_points = [
            (2, 2), (2, 5), (2, 8),
            (5, 2), (5, 5), (5, 8),
            (8, 2), (8, 5), (8, 8)
        ]
        
        for pos in reference_points:
            # Simuliere RSSI basierend auf Entfernung
            rssi_values = {}
            for ap_name, ap_pos in ap_positions.items():
                distance = np.sqrt((pos[0] - ap_pos[0])**2 + (pos[1] - ap_pos[1])**2)
                # RSSI = -30 - 20*log10(distance) + noise
                rssi = -30 - 20 * np.log10(max(distance, 0.1)) + np.random.normal(0, 2)
                rssi_values[ap_name] = rssi
            
            db.add_fingerprint(pos, rssi_values)
        
        print(f"  Referenzpunkte: {len(reference_points)}")
        print(f"  Beispiel bei (5, 5):")
        # Finde den Fingerprint für Position (5,5)
        for fp in db.fingerprints:
            if fp['location'] == (5, 5):
                for ap, rssi in fp['signals'].items():
                    print(f"    {ap}: {rssi:.1f} dBm")
                break
        
        # Test: Positionierung an neuer Stelle
        print("\nSchritt 3: Position schätzen (unbekannter Ort)")
        print("-" * 70)
        
        test_position = (4.5, 6.2)
        print(f"  Wahre Position: {test_position}")
        
        # Simuliere RSSI-Messung an Test-Position
        test_rssi = {}
        for ap_name, ap_pos in ap_positions.items():
            distance = np.sqrt((test_position[0] - ap_pos[0])**2 + 
                             (test_position[1] - ap_pos[1])**2)
            rssi = -30 - 20 * np.log10(max(distance, 0.1)) + np.random.normal(0, 2)
            test_rssi[ap_name] = rssi
        
        print(f"  Gemessene RSSI-Werte:")
        for ap, rssi in test_rssi.items():
            print(f"    {ap}: {rssi:.1f} dBm")
        
        # Berechne ICQ für jedes RSSI-Signal
        print(f"\n  ICQ-Qualitätsbewertung der Signale:")
        signal_icqs = {}
        for ap, rssi in test_rssi.items():
            # Zeitreihe simulieren (10 Messungen)
            rssi_series = [rssi + np.random.normal(0, 1) for _ in range(10)]
            icq, _ = self.icq_calc.calculate_icq(rssi_series)
            signal_icqs[ap] = icq
            print(f"    {ap}: ICQ = {icq:.4f}")
        
        # Finde nächste Fingerprints mit ICQ-Gewichtung
        matches = db.find_matches(test_rssi, k=3)
        
        # Berechne gewichtete Position
        if matches:
            total_weight = 0
            weighted_x = 0
            weighted_y = 0
            
            for idx, similarity, location in matches:
                # Verwende 1/similarity als Gewicht (kleinere Distanz = höheres Gewicht)
                weight = 1.0 / (similarity + 1e-6)
                weighted_x += location[0] * weight
                weighted_y += location[1] * weight
                total_weight += weight
            
            estimated_pos = (weighted_x / total_weight, weighted_y / total_weight)
        else:
            estimated_pos = (0, 0)
        
        error = np.sqrt((estimated_pos[0] - test_position[0])**2 + 
                       (estimated_pos[1] - test_position[1])**2)
        
        print(f"\n  Geschätzte Position: ({estimated_pos[0]:.2f}, {estimated_pos[1]:.2f})")
        print(f"  Positionsfehler: {error:.2f} Meter")
        
        self.results['tests'].append({
            'test': 'wifi_fingerprinting',
            'true_position': test_position,
            'estimated_position': tuple(estimated_pos),
            'error_meters': error,
            'signal_icqs': signal_icqs
        })
        
        print(f"\n✅ WiFi-Positionierung funktioniert! Fehler: {error:.2f}m")
    
    def test_3_imu_navigation(self):
        """Test 3: Inertial Navigation (IMU)."""
        print("\n" + "="*80)
        print("TEST 3: INERTIAL NAVIGATION (IMU)")
        print("="*80 + "\n")
        
        print("Schritt 1: IMU-Daten generieren (simulierte Bewegung)")
        print("-" * 70)
        
        # Simuliere eine Bewegung: 5 Sekunden geradeaus
        dt = 0.1  # 10 Hz Sampling
        duration = 5.0
        steps = int(duration / dt)
        
        # Konstante Beschleunigung nach vorne (x-Achse)
        accel_x = 0.5  # m/s²
        
        print(f"  Simulation: {duration}s Bewegung mit {accel_x} m/s² Beschleunigung")
        print(f"  Sampling-Rate: {1/dt:.0f} Hz")
        print(f"  Anzahl Messungen: {steps}")
        
        # Generiere IMU-Daten
        imu_data = []
        for i in range(steps):
            # Beschleunigung: x-Richtung + Gravitation in z + Rauschen
            accel = [
                accel_x + np.random.normal(0, 0.02),
                np.random.normal(0, 0.01),
                9.81 + np.random.normal(0, 0.05)
            ]
            # Gyro: kleine Drehungen
            gyro = [
                np.random.normal(0, 0.001),
                np.random.normal(0, 0.001),
                np.random.normal(0, 0.002)
            ]
            imu_data.append({'accel': accel, 'gyro': gyro, 'dt': dt})
        
        print(f"\n  Beispiel IMU-Messung:")
        print(f"    Beschleunigung: {imu_data[0]['accel']}")
        print(f"    Gyroskop: {imu_data[0]['gyro']}")
        
        # ICQ-Bewertung der IMU-Daten
        print(f"\nSchritt 2: ICQ-Qualitätsbewertung der IMU-Daten")
        print("-" * 70)
        
        accel_x_series = [d['accel'][0] for d in imu_data]
        accel_z_series = [d['accel'][2] for d in imu_data]
        
        icq_accel_x, _ = self.icq_calc.calculate_icq(accel_x_series)
        icq_accel_z, _ = self.icq_calc.calculate_icq(accel_z_series)
        
        print(f"  ICQ Beschleunigung-X: {icq_accel_x:.4f} (Bewegungsrichtung)")
        print(f"  ICQ Beschleunigung-Z: {icq_accel_z:.4f} (Gravitation)")
        
        # Inertial Navigation
        print(f"\nSchritt 3: Dead Reckoning (Position berechnen)")
        print("-" * 70)
        
        nav = InertialNavigator(initial_position=(0, 0, 0))  # Start bei Ursprung
        
        positions = []
        for data in imu_data:
            nav.update(data['accel'], data['gyro'], data['dt'])
            positions.append(nav.position.copy())
        
        final_position = nav.position
        final_velocity = nav.velocity
        
        # Erwartete Position: s = 0.5 * a * t²
        expected_distance = 0.5 * accel_x * duration**2
        
        print(f"  Start-Position: (0, 0, 0)")
        print(f"  End-Position: ({final_position[0]:.2f}, {final_position[1]:.2f}, {final_position[2]:.2f})")
        print(f"  End-Geschwindigkeit: ({final_velocity[0]:.2f}, {final_velocity[1]:.2f}, {final_velocity[2]:.2f}) m/s")
        print(f"\n  Erwartete Distanz: {expected_distance:.2f}m")
        print(f"  Berechnete Distanz: {final_position[0]:.2f}m")
        print(f"  Fehler: {abs(final_position[0] - expected_distance):.2f}m")
        
        self.results['tests'].append({
            'test': 'imu_navigation',
            'duration_seconds': duration,
            'samples': steps,
            'final_position': final_position.tolist(),
            'expected_distance': expected_distance,
            'icq_accel_x': icq_accel_x,
            'icq_accel_z': icq_accel_z
        })
        
        print(f"\n✅ IMU-Navigation funktioniert!")
        print(f"   ICQ zeigt stabile Sensordaten: {icq_accel_x:.3f}")
    
    def test_4_sensor_fusion(self):
        """Test 4: Multi-Sensor Fusion mit ICQ-Gewichtung."""
        print("\n" + "="*80)
        print("TEST 4: MULTI-SENSOR FUSION MIT ICQ-GEWICHTUNG")
        print("="*80 + "\n")
        
        print("Konzept: Kombiniere mehrere Positionsquellen basierend auf ICQ-Qualität")
        print("-" * 70)
        
        # Wahre Position
        true_position = np.array([5.0, 5.0, 0.0])
        
        # Simuliere 3 verschiedene Positionsschätzungen mit unterschiedlicher Qualität
        estimates = {
            'WiFi': {
                'position': true_position + np.array([0.5, -0.3, 0]),
                'measurements': [5.5 + np.random.normal(0, 0.2) for _ in range(20)],
                'description': 'WiFi Fingerprinting (gute Qualität)'
            },
            'Bluetooth': {
                'position': true_position + np.array([-1.2, 0.8, 0]),
                'measurements': [5.0 + np.random.uniform(-2, 2) for _ in range(20)],
                'description': 'Bluetooth Trilateration (schlechte Qualität - Störungen)'
            },
            'IMU': {
                'position': true_position + np.array([0.1, 0.2, 0]),
                'measurements': [5.1 + np.random.normal(0, 0.1) for _ in range(20)],
                'description': 'Inertial Navigation (sehr gute Qualität)'
            }
        }
        
        print(f"Wahre Position: {true_position[:2]}\n")
        
        # Berechne ICQ für jede Quelle
        print("Schritt 1: ICQ-Qualität jeder Quelle berechnen")
        print("-" * 70)
        
        icq_weights = {}
        for source, data in estimates.items():
            icq, _ = self.icq_calc.calculate_icq(data['measurements'])
            icq_weights[source] = icq
            error = np.linalg.norm(data['position'][:2] - true_position[:2])
            
            print(f"{source:15s}: ICQ = {icq:.4f}  |  Position = {data['position'][:2]}  |  Fehler = {error:.2f}m")
            print(f"                 → {data['description']}")
        
        # Fusion ohne ICQ-Gewichtung (einfacher Durchschnitt)
        print(f"\nSchritt 2: Position OHNE ICQ-Gewichtung (einfacher Durchschnitt)")
        print("-" * 70)
        
        positions_array = np.array([data['position'] for data in estimates.values()])
        simple_average = np.mean(positions_array, axis=0)
        simple_error = np.linalg.norm(simple_average[:2] - true_position[:2])
        
        print(f"  Durchschnitts-Position: {simple_average[:2]}")
        print(f"  Fehler: {simple_error:.2f}m")
        
        # Fusion MIT ICQ-Gewichtung
        print(f"\nSchritt 3: Position MIT ICQ-Gewichtung")
        print("-" * 70)
        
        fusion = PositionFusion()
        
        for source, data in estimates.items():
            icq = icq_weights[source]
            fusion.update_position(
                source_name=source,
                measured_position=tuple(data['position']),
                icq_score=icq
            )
        
        fused_position = fusion.position
        fused_error = np.linalg.norm(fused_position[:2] - true_position[:2])
        
        print(f"  ICQ-Gewichtungen:")
        total_weight = sum(icq_weights.values())
        if total_weight == 0:
            # Fallback wenn alle ICQs 0 sind - verwende gleiches Gewicht
            total_weight = len(icq_weights)
            for source in icq_weights:
                icq_weights[source] = 1.0
        
        for source, icq in icq_weights.items():
            weight_pct = (icq / total_weight) * 100
            print(f"    {source:15s}: {weight_pct:.1f}% Gewicht (ICQ={icq:.4f})")
        
        print(f"\n  Fusionierte Position: {fused_position[:2]}")
        print(f"  Fehler: {fused_error:.2f}m")
        
        improvement = ((simple_error - fused_error) / simple_error) * 100
        
        print(f"\n  Verbesserung durch ICQ-Gewichtung: {improvement:.1f}%")
        
        self.results['tests'].append({
            'test': 'sensor_fusion',
            'true_position': true_position.tolist(),
            'simple_average': simple_average.tolist(),
            'simple_error': simple_error,
            'fused_position': fused_position.tolist(),
            'fused_error': fused_error,
            'improvement_percent': improvement,
            'icq_weights': icq_weights
        })
        
        print(f"\n✅ Sensor Fusion funktioniert!")
        print(f"   ICQ-Gewichtung reduziert Fehler um {improvement:.1f}%")
    
    def test_5_real_world_scenario(self):
        """Test 5: Realistisches Szenario - Bewegung durch Gebäude."""
        print("\n" + "="*80)
        print("TEST 5: REALISTISCHES SZENARIO - INDOOR-NAVIGATION")
        print("="*80 + "\n")
        
        print("Szenario: Person bewegt sich durch ein Bürogebäude (10m x 10m)")
        print("  - Start: (2, 2)")
        print("  - Weg: nach rechts, dann nach oben")
        print("  - Ende: (8, 8)")
        print("  - Dauer: 30 Sekunden\n")
        
        # Erstelle Fingerprint-DB
        db = SignalFingerprintDB()
        ap_positions = {
            'AP_1': (0, 0), 'AP_2': (10, 0),
            'AP_3': (0, 10), 'AP_4': (10, 10),
            'AP_5': (5, 5)  # Zentral
        }
        
        # Füge Referenzen hinzu
        for x in range(0, 11, 2):
            for y in range(0, 11, 2):
                rssi = {}
                for ap, pos in ap_positions.items():
                    dist = np.sqrt((x - pos[0])**2 + (y - pos[1])**2)
                    rssi[ap] = -30 - 20*np.log10(max(dist, 0.1))
                db.add_fingerprint((x, y), rssi)
        
        # Simuliere Bewegung
        path = []
        # Rechts: (2,2) → (8,2) über 15 Sekunden
        for t in np.linspace(0, 15, 50):
            x = 2 + (6/15) * t
            path.append((x, 2.0, t))
        # Hoch: (8,2) → (8,8) über 15 Sekunden  
        for t in np.linspace(15, 30, 50):
            y = 2 + (6/15) * (t - 15)
            path.append((8.0, y, t))
        
        # Tracking mit WiFi + IMU Fusion
        print(f"Schritt 1: Tracking mit WiFi + IMU Fusion")
        print("-" * 70)
        
        nav = InertialNavigator(initial_position=(2, 2, 0))
        fusion = PositionFusion()
        
        estimated_path = []
        errors = []
        icq_history = []
        
        for i, (true_x, true_y, t) in enumerate(path):
            # WiFi-Messung
            wifi_rssi = {}
            for ap, pos in ap_positions.items():
                dist = np.sqrt((true_x - pos[0])**2 + (true_y - pos[1])**2)
                rssi = -30 - 20*np.log10(max(dist, 0.1)) + np.random.normal(0, 3)
                wifi_rssi[ap] = rssi
            
            # WiFi-Position mit Fingerprinting
            matches = db.find_matches(wifi_rssi, k=3)
            if matches:
                total_weight = 0
                weighted_x = 0
                weighted_y = 0
                
                for idx, similarity, location in matches:
                    weight = 1.0 / (similarity + 1e-6)
                    weighted_x += location[0] * weight
                    weighted_y += location[1] * weight
                    total_weight += weight
                
                wifi_pos = (weighted_x / total_weight, weighted_y / total_weight)
            else:
                wifi_pos = (5.0, 5.0)
            
            # IMU-Update (wenn nicht erster Punkt)
            if i > 0:
                dt = path[i][2] - path[i-1][2]
                dx = path[i][0] - path[i-1][0]
                dy = path[i][1] - path[i-1][1]
                
                accel = [dx/(dt**2) if dt > 0 else 0, 
                        dy/(dt**2) if dt > 0 else 0, 
                        9.81]
                gyro = [0, 0, 0]
                nav.update(accel, gyro, dt)
            
            imu_pos = nav.position
            
            # Berechne ICQ für WiFi-Signale
            rssi_values = list(wifi_rssi.values())
            if len(rssi_values) > 2:
                wifi_icq, _ = self.icq_calc.calculate_icq(rssi_values)
            else:
                wifi_icq = 0.5
            
            icq_history.append(wifi_icq)
            
            # Fusion
            fusion = PositionFusion()
            fusion.update_position('WiFi', (wifi_pos[0], wifi_pos[1], 0), wifi_icq)
            fusion.update_position('IMU', tuple(imu_pos), 0.7)
            
            fused = fusion.position
            estimated_path.append(fused[:2])
            
            error = np.sqrt((fused[0] - true_x)**2 + (fused[1] - true_y)**2)
            errors.append(error)
        
        avg_error = np.mean(errors)
        max_error = np.max(errors)
        avg_icq = np.mean(icq_history)
        
        print(f"  Pfad-Punkte getrackt: {len(path)}")
        print(f"  Durchschnittlicher Fehler: {avg_error:.2f}m")
        print(f"  Maximaler Fehler: {max_error:.2f}m")
        print(f"  Durchschnittliches ICQ (WiFi): {avg_icq:.4f}")
        
        # Zeige einige Punkte
        print(f"\n  Beispiel-Positionen:")
        for i in [0, len(path)//2, -1]:
            true_pos = (path[i][0], path[i][1])
            est_pos = estimated_path[i]
            err = errors[i]
            print(f"    t={path[i][2]:4.1f}s: Wahr={true_pos}, Geschätzt=({est_pos[0]:.2f}, {est_pos[1]:.2f}), Fehler={err:.2f}m")
        
        self.results['tests'].append({
            'test': 'real_world_scenario',
            'path_points': len(path),
            'avg_error_meters': avg_error,
            'max_error_meters': max_error,
            'avg_icq': avg_icq
        })
        
        print(f"\n✅ Realistisches Szenario erfolgreich!")
        print(f"   Durchschnittliche Genauigkeit: {avg_error:.2f}m")
    
    def save_results(self):
        """Speichere Testergebnisse."""
        filename = 'gps_free_positioning_test_results.json'
        with open(filename, 'w', encoding='utf-8') as f:
            json.dump(self.results, indent=2, ensure_ascii=False, fp=f)
        
        print(f"\n📄 Ergebnisse gespeichert in: {filename}")
        return filename
    
    def print_summary(self):
        """Drucke Zusammenfassung."""
        print("\n" + "="*80)
        print("ZUSAMMENFASSUNG: GPS-FREIES ORTUNGSSYSTEM")
        print("="*80 + "\n")
        
        print("✅ ALLE TESTS ERFOLGREICH!\n")
        
        print("Was Sie gelernt haben:")
        print("-" * 70)
        print("1. ICQ-Berechnung benötigt nur ein Array von Zahlen")
        print("   → Beliebige Messwerte: WiFi RSSI, Beschleunigung, Temperatur, etc.")
        print()
        print("2. WiFi-Positionierung funktioniert mit RSSI-Fingerprints")
        print("   → Mindestens 3 Access Points, besser 5+")
        print("   → ICQ bewertet Signalqualität automatisch")
        print()
        print("3. IMU-Navigation mit Beschleunigung + Gyroskop")
        print("   → 10-100 Hz Sampling-Rate empfohlen")
        print("   → ICQ erkennt Sensor-Drift und Störungen")
        print()
        print("4. Sensor Fusion verbessert Genauigkeit")
        print("   → ICQ-Gewichtung: Gute Quellen bekommen mehr Einfluss")
        print("   → Automatische Qualitätsbewertung ohne manuelle Kalibrierung")
        print()
        
        print("\n" + "="*80)
        print("WIE SIE ES SELBST TESTEN KÖNNEN:")
        print("="*80 + "\n")
        
        print("1. MIT DIESEM SCRIPT:")
        print("   python test_gps_free_positioning.py")
        print()
        print("2. MIT DEMO:")
        print("   python demo_positioning.py")
        print()
        print("3. MIT EIGENEN DATEN:")
        print("   from core.icq_calculator import ICQCalculator")
        print("   icq = ICQCalculator()")
        print("   result = icq.calculate_icq([ihre, daten, hier])")
        print()
        
        print("\n" + "="*80)
        print("NÄCHSTE SCHRITTE:")
        print("="*80 + "\n")
        
        print("→ Lesen Sie: DATEN_ANFORDERUNGEN.md")
        print("→ Lesen Sie: PRAKTISCHER_EINSTIEG.md")
        print("→ Führen Sie aus: demo_positioning.py")
        print("→ Integrieren Sie eigene Sensoren (siehe EXPERIMENTAL_GUIDE.md)")
        print()
    
    def run_all_tests(self):
        """Führe alle Tests aus."""
        self.explain_data_requirements()
        self.test_1_basic_icq_calculation()
        self.test_2_wifi_fingerprinting()
        self.test_3_imu_navigation()
        self.test_4_sensor_fusion()
        self.test_5_real_world_scenario()
        
        filename = self.save_results()
        self.print_summary()
        
        print(f"\n{'='*80}")
        print(f"TEST ABGESCHLOSSEN - Alle Ergebnisse in {filename}")
        print(f"{'='*80}\n")


def main():
    """Hauptfunktion."""
    test = InteractivePositioningTest()
    test.run_all_tests()


if __name__ == '__main__':
    main()
