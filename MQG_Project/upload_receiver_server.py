#!/usr/bin/env python3
"""
Sensor Data Upload Receiver Server
===================================

Einfacher Flask-Server zum Empfangen von Smartphone-Sensordaten.
Berechnet ICQ-Werte und speichert Daten lokal.

Installation:
    pip install flask numpy

Verwendung:
    python upload_receiver_server.py
    
Dann zugreifen auf:
    http://localhost:5000
"""

import json
import os
from datetime import datetime
from pathlib import Path
from typing import Dict, List

try:
    from flask import Flask, request, jsonify, render_template_string
    import numpy as np
except ImportError:
    print("ERROR: Missing dependencies!")
    print("Install with: pip install flask numpy")
    import sys
    sys.exit(1)

# Import ICQ calculator
import sys
sys.path.insert(0, str(Path(__file__).parent / 'src'))
from core.icq_calculator import ICQCalculator

app = Flask(__name__)
icq_calc = ICQCalculator()

# Data storage directory
DATA_DIR = Path(__file__).parent / 'sensor_uploads'
DATA_DIR.mkdir(exist_ok=True)


# HTML Template for web interface
WEB_INTERFACE = """
<!DOCTYPE html>
<html lang="de">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MQG Sensor Upload Server</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
            background: #f5f5f5;
        }
        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            border-radius: 10px;
            margin-bottom: 20px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }
        .header h1 {
            margin: 0 0 10px 0;
        }
        .card {
            background: white;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .status {
            display: inline-block;
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: bold;
        }
        .status.online {
            background: #4CAF50;
            color: white;
        }
        .endpoint {
            background: #f0f0f0;
            padding: 10px;
            border-radius: 5px;
            font-family: monospace;
            margin: 10px 0;
        }
        .example {
            background: #263238;
            color: #aed581;
            padding: 15px;
            border-radius: 5px;
            font-family: 'Courier New', monospace;
            overflow-x: auto;
            margin: 10px 0;
        }
        .data-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin: 20px 0;
        }
        .stat-box {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            text-align: center;
        }
        .stat-box .number {
            font-size: 36px;
            font-weight: bold;
            margin: 10px 0;
        }
        .stat-box .label {
            font-size: 14px;
            opacity: 0.9;
        }
        .btn {
            background: #667eea;
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 6px;
            cursor: pointer;
            font-size: 16px;
            text-decoration: none;
            display: inline-block;
            margin: 5px;
        }
        .btn:hover {
            background: #5568d3;
        }
    </style>
</head>
<body>
    <div class="header">
        <h1>📱 MQG Sensor Upload Server</h1>
        <p>Information Coherence Quotient (ICQ) Messungen</p>
        <span class="status online">● Server Online</span>
    </div>

    <div class="card">
        <h2>🔗 Upload-Endpunkt</h2>
        <div class="endpoint">
            POST {{ base_url }}/api/upload_sensors
        </div>
        <p>Senden Sie JSON-Daten mit WiFi-RSSI, Beschleunigung und Gyro-Werten.</p>
    </div>

    <div class="card">
        <h2>📊 Statistiken</h2>
        <div class="data-stats">
            <div class="stat-box">
                <div class="label">Empfangene Messungen</div>
                <div class="number">{{ stats.total_uploads }}</div>
            </div>
            <div class="stat-box">
                <div class="label">Letzte ICQ</div>
                <div class="number">{{ "%.3f"|format(stats.last_icq) if stats.last_icq else "N/A" }}</div>
            </div>
            <div class="stat-box">
                <div class="label">WiFi-APs erkannt</div>
                <div class="number">{{ stats.last_wifi_count }}</div>
            </div>
        </div>
    </div>

    <div class="card">
        <h2>💻 Verwendung</h2>
        <h3>Android (mit Termux):</h3>
        <div class="example">
# Installiere Python und Bibliotheken
pkg install python
pip install requests

# Lade Script herunter
curl -O {{ base_url }}/upload_sensor_data.py

# Starte kontinuierliche Messungen
python upload_sensor_data.py --server {{ base_url }}
        </div>

        <h3>Einmalige Messung:</h3>
        <div class="example">
python upload_sensor_data.py --server {{ base_url }} --once
        </div>

        <h3>Python (manuell):</h3>
        <div class="example">
import requests

data = {
    "wifi": {"AP1": -45.0, "AP2": -67.0},
    "accelerometer": [0.02, -0.01, 9.81],
    "gyroscope": [0.001, -0.002, 0.0]
}

response = requests.post(
    "{{ base_url }}/api/upload_sensors",
    json=data
)
print(response.json())
        </div>
    </div>

    <div class="card">
        <h2>📁 Gespeicherte Daten</h2>
        <p>Daten werden gespeichert in: <code>{{ data_dir }}</code></p>
        <a href="/api/download_data" class="btn">📥 Alle Daten herunterladen (JSON)</a>
        <a href="/api/stats" class="btn">📊 Statistiken (JSON)</a>
    </div>
</body>
</html>
"""


# Global statistics
server_stats = {
    'total_uploads': 0,
    'last_icq': None,
    'last_wifi_count': 0,
    'start_time': datetime.now()
}


@app.route('/')
def index():
    """Zeigt die Web-Oberfläche."""
    return render_template_string(
        WEB_INTERFACE,
        base_url=request.host_url.rstrip('/'),
        stats=server_stats,
        data_dir=str(DATA_DIR.absolute())
    )


@app.route('/api/upload_sensors', methods=['POST'])
def upload_sensors():
    """
    Empfängt Sensordaten und berechnet ICQ.
    
    Expected JSON format:
    {
        "wifi": {"AP1": -45.0, "AP2": -67.0, ...},
        "accelerometer": [x, y, z],
        "gyroscope": [x, y, z],
        "timestamp": "2024-01-01T12:00:00"  // optional
    }
    """
    try:
        data = request.get_json()
        
        if not data:
            return jsonify({'error': 'No JSON data received'}), 400
        
        # Add server timestamp
        data['server_timestamp'] = datetime.now().isoformat()
        
        # Calculate ICQ from WiFi RSSI values
        icq_value = None
        if 'wifi' in data and data['wifi']:
            rssi_values = list(data['wifi'].values())
            if len(rssi_values) >= 2:
                icq_value = icq_calc.calculate_icq(rssi_values)
                data['calculated_icq'] = float(icq_value)
        
        # Calculate ICQ from accelerometer
        if 'accelerometer' in data:
            accel_icq = icq_calc.calculate_icq(data['accelerometer'])
            data['accelerometer_icq'] = float(accel_icq)
        
        # Save data to file
        filename = f"sensor_data_{datetime.now().strftime('%Y%m%d_%H%M%S_%f')}.json"
        filepath = DATA_DIR / filename
        
        with open(filepath, 'w') as f:
            json.dump(data, f, indent=2)
        
        # Update stats
        server_stats['total_uploads'] += 1
        server_stats['last_icq'] = icq_value
        server_stats['last_wifi_count'] = len(data.get('wifi', {}))
        
        # Prepare response
        response = {
            'status': 'success',
            'message': 'Daten erfolgreich empfangen',
            'filename': filename,
            'icq': icq_value,
            'wifi_aps': len(data.get('wifi', {}))
        }
        
        print(f"✅ Upload empfangen: {len(data.get('wifi', {}))} WiFi-APs, ICQ={icq_value:.4f if icq_value else 'N/A'}")
        
        return jsonify(response), 200
        
    except Exception as e:
        print(f"❌ Fehler beim Verarbeiten: {e}")
        return jsonify({'error': str(e)}), 500


@app.route('/api/stats', methods=['GET'])
def get_stats():
    """Gibt Server-Statistiken zurück."""
    stats = {
        **server_stats,
        'uptime_seconds': (datetime.now() - server_stats['start_time']).total_seconds(),
        'data_files': len(list(DATA_DIR.glob('*.json')))
    }
    return jsonify(stats), 200


@app.route('/api/download_data', methods=['GET'])
def download_data():
    """Gibt alle gespeicherten Daten als JSON zurück."""
    all_data = []
    
    for filepath in sorted(DATA_DIR.glob('*.json')):
        try:
            with open(filepath, 'r') as f:
                data = json.load(f)
                data['filename'] = filepath.name
                all_data.append(data)
        except Exception as e:
            print(f"Fehler beim Lesen von {filepath}: {e}")
    
    return jsonify({
        'count': len(all_data),
        'data': all_data
    }), 200


@app.route('/upload_sensor_data.py', methods=['GET'])
def download_script():
    """Bietet das Upload-Script zum Download an."""
    script_path = Path(__file__).parent / 'upload_sensor_data.py'
    
    if script_path.exists():
        with open(script_path, 'r') as f:
            script_content = f.read()
        return script_content, 200, {'Content-Type': 'text/plain; charset=utf-8'}
    else:
        return "Script not found", 404


def main():
    """Startet den Server."""
    print("=" * 60)
    print("📱 MQG Sensor Upload Receiver Server")
    print("=" * 60)
    print()
    print(f"📁 Daten werden gespeichert in: {DATA_DIR.absolute()}")
    print()
    print("🌐 Server startet auf:")
    print("   http://localhost:5000")
    print("   http://0.0.0.0:5000")
    print()
    print("📤 Upload-Endpunkt:")
    print("   POST http://localhost:5000/api/upload_sensors")
    print()
    print("💡 Öffne http://localhost:5000 im Browser für mehr Infos")
    print("=" * 60)
    print()
    
    # Start server
    app.run(host='0.0.0.0', port=5000, debug=False)


if __name__ == '__main__':
    main()
