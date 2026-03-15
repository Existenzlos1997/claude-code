#!/usr/bin/env python3
"""
Smartphone Sensor Data Upload Script
=====================================

Sammelt WiFi-RSSI und IMU-Daten vom Smartphone und sendet sie an den Server.
Optimiert für Android mit Termux.

Installation (Android Termux):
    pkg install python
    pip install requests

Verwendung:
    python upload_sensor_data.py --server http://localhost:5000
"""

import subprocess
import json
import time
import argparse
import sys
from datetime import datetime
from typing import Dict, List, Optional

try:
    import requests
except ImportError:
    print("ERROR: requests library not found!")
    print("Install with: pip install requests")
    sys.exit(1)


class SmartphoneSensorCollector:
    """Sammelt Sensordaten vom Smartphone."""
    
    def __init__(self, server_url: str = "http://localhost:5000"):
        """
        Initialisiert den Sensor-Collector.
        
        Args:
            server_url: URL des Upload-Servers
        """
        self.server_url = server_url.rstrip('/')
        self.upload_endpoint = f"{self.server_url}/api/upload_sensors"
        
    def scan_wifi(self) -> Dict[str, float]:
        """
        Scannt WiFi-Netzwerke und gibt RSSI-Werte zurück.
        
        Returns:
            Dictionary mit SSID als Key und RSSI (dBm) als Value
        """
        wifi_data = {}
        
        try:
            # Versuche iwlist (Linux/Android)
            result = subprocess.run(
                ['su', '-c', 'iwlist', 'wlan0', 'scan'],
                capture_output=True,
                text=True,
                timeout=5
            )
            
            if result.returncode == 0:
                # Parse iwlist output
                lines = result.stdout.split('\n')
                current_ssid = None
                
                for line in lines:
                    if 'ESSID:' in line:
                        current_ssid = line.split('ESSID:"')[1].split('"')[0]
                    elif 'Signal level=' in line and current_ssid:
                        # Extract RSSI value
                        rssi_str = line.split('Signal level=')[1].split()[0]
                        try:
                            rssi = float(rssi_str)
                            wifi_data[current_ssid] = rssi
                        except ValueError:
                            pass
                        current_ssid = None
                        
        except (subprocess.SubprocessError, FileNotFoundError, IndexError):
            # Fallback: Simulated data for testing
            wifi_data = {
                'TestAP_1': -45.0,
                'TestAP_2': -67.0,
                'TestAP_3': -78.0,
                'TestAP_4': -82.0
            }
            print("⚠️  WiFi-Scan fehlgeschlagen, verwende Test-Daten")
            
        return wifi_data
    
    def read_accelerometer(self) -> List[float]:
        """
        Liest Beschleunigungswerte aus.
        
        Returns:
            [x, y, z] Beschleunigung in m/s²
        """
        # Auf Android mit Termux: Zugriff über /sys/devices oder Termux-API
        try:
            # Versuche Termux-API
            result = subprocess.run(
                ['termux-sensor', '-s', 'accelerometer', '-n', '1'],
                capture_output=True,
                text=True,
                timeout=2
            )
            
            if result.returncode == 0:
                data = json.loads(result.stdout)
                if 'accelerometer' in data:
                    values = data['accelerometer']['values']
                    return [values[0], values[1], values[2]]
                    
        except (subprocess.SubprocessError, FileNotFoundError, json.JSONDecodeError):
            pass
            
        # Fallback: Simulierte Daten (Ruhezustand mit Gravitation)
        return [0.02, -0.01, 9.81]
    
    def read_gyroscope(self) -> List[float]:
        """
        Liest Gyroskopdaten aus.
        
        Returns:
            [x, y, z] Winkelgeschwindigkeit in rad/s
        """
        try:
            # Versuche Termux-API
            result = subprocess.run(
                ['termux-sensor', '-s', 'gyroscope', '-n', '1'],
                capture_output=True,
                text=True,
                timeout=2
            )
            
            if result.returncode == 0:
                data = json.loads(result.stdout)
                if 'gyroscope' in data:
                    values = data['gyroscope']['values']
                    return [values[0], values[1], values[2]]
                    
        except (subprocess.SubprocessError, FileNotFoundError, json.JSONDecodeError):
            pass
            
        # Fallback: Simulierte Daten (statisch)
        return [0.001, -0.002, 0.0]
    
    def collect_all_data(self) -> Dict:
        """
        Sammelt alle verfügbaren Sensordaten.
        
        Returns:
            Dictionary mit allen Sensordaten
        """
        print("📱 Sammle Sensordaten...")
        
        data = {
            'timestamp': datetime.now().isoformat(),
            'wifi': self.scan_wifi(),
            'accelerometer': self.read_accelerometer(),
            'gyroscope': self.read_gyroscope(),
            'device_info': {
                'platform': sys.platform,
                'python_version': sys.version
            }
        }
        
        print(f"✅ WiFi-APs: {len(data['wifi'])}")
        print(f"✅ Beschleunigung: {data['accelerometer']}")
        print(f"✅ Gyro: {data['gyroscope']}")
        
        return data
    
    def upload_data(self, data: Dict) -> bool:
        """
        Sendet Daten an den Server.
        
        Args:
            data: Sensordaten zum Upload
            
        Returns:
            True wenn erfolgreich, False sonst
        """
        try:
            print(f"📤 Sende Daten an {self.upload_endpoint}...")
            
            response = requests.post(
                self.upload_endpoint,
                json=data,
                timeout=10
            )
            
            if response.status_code == 200:
                result = response.json()
                print(f"✅ Upload erfolgreich!")
                
                if 'icq' in result:
                    print(f"📊 ICQ-Wert: {result['icq']:.4f}")
                if 'message' in result:
                    print(f"💬 Server: {result['message']}")
                    
                return True
            else:
                print(f"❌ Upload fehlgeschlagen: HTTP {response.status_code}")
                print(f"   Antwort: {response.text}")
                return False
                
        except requests.RequestException as e:
            print(f"❌ Verbindungsfehler: {e}")
            print(f"   Stelle sicher, dass der Server läuft auf {self.server_url}")
            return False
    
    def continuous_upload(self, interval: float = 1.0, count: Optional[int] = None):
        """
        Kontinuierliches Sammeln und Uploaden von Daten.
        
        Args:
            interval: Sekunden zwischen Messungen
            count: Anzahl der Messungen (None = unbegrenzt)
        """
        print(f"🔄 Starte kontinuierliche Datenerfassung (Intervall: {interval}s)")
        
        measurement_count = 0
        try:
            while count is None or measurement_count < count:
                data = self.collect_all_data()
                success = self.upload_data(data)
                
                measurement_count += 1
                print(f"\n📈 Messung #{measurement_count} abgeschlossen")
                
                if not success:
                    print("⚠️  Upload fehlgeschlagen, versuche es beim nächsten Mal erneut...")
                
                if count is None or measurement_count < count:
                    print(f"⏳ Warte {interval} Sekunden...\n")
                    time.sleep(interval)
                    
        except KeyboardInterrupt:
            print(f"\n\n⏹️  Messung gestoppt durch Benutzer nach {measurement_count} Messungen")


def main():
    """Hauptfunktion - Kommandozeilen-Interface."""
    parser = argparse.ArgumentParser(
        description='Smartphone Sensor Data Upload für MQG-ICQ'
    )
    parser.add_argument(
        '--server',
        type=str,
        default='http://localhost:5000',
        help='Server-URL (Standard: http://localhost:5000)'
    )
    parser.add_argument(
        '--interval',
        type=float,
        default=1.0,
        help='Messintervall in Sekunden (Standard: 1.0)'
    )
    parser.add_argument(
        '--count',
        type=int,
        default=None,
        help='Anzahl der Messungen (Standard: unbegrenzt)'
    )
    parser.add_argument(
        '--once',
        action='store_true',
        help='Nur eine Messung durchführen'
    )
    
    args = parser.parse_args()
    
    print("=" * 60)
    print("📱 Smartphone Sensor Data Upload")
    print("   MQG-ICQ Messungen")
    print("=" * 60)
    print(f"Server: {args.server}")
    print(f"Intervall: {args.interval}s")
    print(f"Anzahl: {'unbegrenzt' if args.count is None and not args.once else (1 if args.once else args.count)}")
    print("=" * 60)
    print()
    
    collector = SmartphoneSensorCollector(server_url=args.server)
    
    if args.once:
        # Einmalige Messung
        data = collector.collect_all_data()
        collector.upload_data(data)
    else:
        # Kontinuierliche Messung
        collector.continuous_upload(
            interval=args.interval,
            count=args.count
        )
    
    print("\n✅ Programm beendet")


if __name__ == '__main__':
    main()
