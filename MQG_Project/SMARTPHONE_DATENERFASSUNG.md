# Smartphone-Datenerfassung mit phyphox

## Problem & Lösung

### ❌ Problem
Die direkte Sensor-Abfrage im Browser funktioniert nicht zuverlässig:
- Browser-Sicherheitsbeschränkungen
- Eingeschränkter Sensor-Zugriff
- Plattform-abhängige Probleme
- Keine Datenpersistenz

### ✅ Lösung: phyphox Integration
Professionelle Datenerfassung mit der phyphox App:
- Zuverlässige Sensor-Aufzeichnung
- Alle Smartphone-Sensoren verfügbar
- Standard-Export-Formate (CSV, Excel)
- Funktioniert auf allen Plattformen

## Wie es funktioniert

### Schritt 1: phyphox installieren

**iOS (iPhone/iPad):**
1. App Store öffnen
2. "phyphox" suchen
3. Installieren (kostenlos)

**Android:**
1. Google Play Store öffnen
2. "phyphox" suchen
3. Installieren (kostenlos)

**Website:** https://phyphox.org

### Schritt 2: Daten aufzeichnen

1. **phyphox öffnen**
2. **Experiment wählen:**
   - "Acceleration" (Beschleunigung)
   - "Gyroscope" (Drehrate)
   - "Magnetometer" (Magnetfeld)
   - Oder andere Experimente

3. **Aufzeichnung starten:**
   - ▶️ Play-Button drücken
   - Smartphone bewegen (oder ruhig halten)
   - ⏸️ Pause zum Stoppen

4. **Aufzeichnung prüfen:**
   - Graphen anschauen
   - Sicherstellen, dass Daten aufgezeichnet wurden

### Schritt 3: Daten exportieren

1. **In phyphox:**
   - ⋮ Menü-Button (oben rechts)
   - "Export Data" wählen

2. **Format wählen:**
   - **CSV** (empfohlen)
   - Excel (.xlsx)

3. **Exportieren:**
   - Per Email an sich selbst senden
   - In Cloud speichern (Dropbox, Google Drive)
   - Auf Computer übertragen

### Schritt 4: ICQ berechnen

**Option A: Web-App (einfachste Methode)**

1. Datei `smartphone_app_phyphox.html` öffnen
2. CSV-Datei hochladen (klicken oder drag & drop)
3. ICQ wird automatisch berechnet
4. Ergebnisse werden angezeigt

**Option B: Python-Kommandozeile**

```bash
python phyphox_analyzer.py meine_daten.csv
```

**Option C: Python-Code**

```python
from src.experimental.phyphox_import import quick_analyze

results = quick_analyze('meine_daten.csv')
print(results['summary'])
```

## Verfügbare Tools

### 1. Web-App: `smartphone_app_phyphox.html`

**Features:**
- 📤 Datei-Upload (klicken oder drag & drop)
- 📊 Automatische ICQ-Berechnung
- 🎨 Visuelle Darstellung (farbcodierte Balken)
- 📋 Datenvorschau (erste 100 Zeilen)
- 💾 JSON-Export der Ergebnisse
- 📱 Touch-freundliches Design
- ⚡ Funktioniert offline

**Verwendung:**
1. Datei im Browser öffnen
2. CSV hochladen
3. Ergebnisse sofort sehen!

### 2. Python-Modul: `phyphox_import.py`

**Features:**
- CSV-Import
- Excel-Import (.xlsx)
- Automatische Sensor-Erkennung
- ICQ-Berechnung
- Zusammenfassungen

**Verwendung:**
```python
from src.experimental.phyphox_import import PhyphoxDataImporter

importer = PhyphoxDataImporter()
importer.import_csv('daten.csv')
icq_values = importer.calculate_icq()
print(importer.get_summary())
```

### 3. Kommandozeilen-Tool: `phyphox_analyzer.py`

**Features:**
- Batch-Verarbeitung
- Mehrere Dateien gleichzeitig
- JSON/Text-Export
- Detaillierte Berichte

**Verwendung:**
```bash
# Einzelne Datei
python phyphox_analyzer.py daten.csv

# Mehrere Dateien
python phyphox_analyzer.py daten1.csv daten2.csv daten3.csv

# Mit Export
python phyphox_analyzer.py -o results.json daten.csv

# Verbose-Modus
python phyphox_analyzer.py -v daten.csv
```

## Unterstützte Sensoren

### Accelerometer (Beschleunigungssensor)
- Misst Beschleunigung in 3 Achsen (x, y, z)
- Einheit: m/s²
- **ICQ-Interpretation:**
  - Hoher ICQ (0.7-1.0): Gleichmäßige Bewegung
  - Mittlerer ICQ (0.4-0.7): Variable Bewegung
  - Niedriger ICQ (0.0-0.4): Chaotische Bewegung

### Gyroscope (Gyroskop)
- Misst Drehrate in 3 Achsen (x, y, z)
- Einheit: rad/s
- **ICQ-Interpretation:**
  - Hoher ICQ: Gleichmäßige Rotation
  - Niedriger ICQ: Wackelige Rotation

### Magnetometer (Magnetfeldsensor)
- Misst Magnetfeld in 3 Achsen (x, y, z)
- Einheit: µT (Mikrotesla)
- **ICQ-Interpretation:**
  - Hoher ICQ: Stabiles Magnetfeld
  - Niedriger ICQ: Störungen/Interferenzen

## Beispiel-Experimente

### Experiment 1: Hand-Stabilität

**Ziel:** Wie stabil kann ich mein Smartphone halten?

**Durchführung:**
1. phyphox → "Acceleration (without g)"
2. Smartphone in der Hand halten
3. 10 Sekunden ruhig halten
4. Daten exportieren
5. ICQ berechnen

**Erwartete Ergebnisse:**
- Sehr ruhig: ICQ > 0.7
- Normal: ICQ 0.4-0.7
- Zittrig: ICQ < 0.4

### Experiment 2: Geh-Analyse

**Ziel:** Wie gleichmäßig gehe ich?

**Durchführung:**
1. phyphox → "Acceleration"
2. Smartphone in Tasche stecken
3. 20-30 Schritte normal gehen
4. Daten exportieren
5. ICQ berechnen

**Erwartete Ergebnisse:**
- Gleichmäßig: ICQ 0.5-0.7
- Ungleichmäßig: ICQ 0.3-0.5

### Experiment 3: Fahrzeug-Erkennung

**Ziel:** Kann ICQ verschiedene Verkehrsmittel unterscheiden?

**Durchführung:**
1. Daten in verschiedenen Fahrzeugen aufzeichnen:
   - Zug
   - Auto
   - Fahrrad
   - Zu Fuß

2. ICQ für jedes Verkehrsmittel berechnen
3. Vergleichen

**Erwartete Unterschiede:**
- Zug: Hoher ICQ (glatte Fahrt)
- Auto: Mittlerer ICQ (Beschleunigung/Bremsen)
- Fahrrad: Variabler ICQ
- Zu Fuß: Periodischer ICQ

## Beispiel-Daten

### Mitgelieferte Beispiele

**`examples/phyphox_sample_data.csv`:**
- 100 Accelerometer-Datenpunkte
- 1 Sekunde Aufzeichnung
- Bereit zum sofortigen Testen

**Verwendung:**
1. `smartphone_app_phyphox.html` öffnen
2. `examples/phyphox_sample_data.csv` hochladen
3. Ergebnisse sofort sehen!

### Eigene Daten

**CSV-Format (phyphox Standard):**
```csv
Time (s),Acceleration x (m/s^2),Acceleration y (m/s^2),Acceleration z (m/s^2)
0.00,-0.123,0.456,9.812
0.01,-0.145,0.434,9.798
0.02,-0.167,0.412,9.823
...
```

**Wichtig:**
- Erste Zeile = Spaltenüberschriften
- Komma oder Tab als Trennzeichen
- Dezimalpunkt (nicht Komma!)

## Fehlerbehebung

### "Datei kann nicht gelesen werden"

**Lösung:**
- Datei in Texteditor öffnen und prüfen
- Sicherstellen, dass erste Zeile Überschriften enthält
- Neu aus phyphox exportieren (CSV-Format)

### "Keine ICQ-Werte"

**Ursachen:**
- Zu wenige Datenpunkte (< 10)
- Alle Werte identisch
- Fehlende Daten

**Lösung:**
- Längere Messung durchführen
- Sensor während Messung bewegen
- Datenqualität in phyphox prüfen

### "Seltsame Werte"

**Ursachen:**
- Sensor nicht kalibriert
- Extreme Ausreißer

**Lösung:**
- Smartphone neu starten
- Sensor in phyphox kalibrieren
- Messung wiederholen

## Best Practices

### Vor der Messung
✅ Smartphone-Sensoren kalibrieren (in phyphox möglich)
✅ Ausreichend Speicherplatz sicherstellen
✅ Andere Apps schließen
✅ Ggf. Flugmodus aktivieren (für störungsfreie Messung)

### Während der Messung
✅ Konstante Bedingungen beibehalten
✅ Mindestens 5 Sekunden aufzeichnen
✅ Bei Bewegung: gleichmäßig bewegen
✅ Nicht unterbrechen

### Nach der Messung
✅ Daten in phyphox visualisieren
✅ Auf Fehler/Ausreißer prüfen
✅ Bei Bedarf wiederholen
✅ Sofort exportieren (nicht vergessen!)

### Datenexport
✅ **CSV-Format bevorzugen** (universell kompatibel)
✅ Aussagekräftige Dateinamen verwenden
✅ Metadaten notieren (Datum, Bedingungen, etc.)
✅ Backup erstellen (Cloud oder Email)

## Vorteile gegenüber direkter Browser-Abfrage

| Aspekt | Browser-Sensoren | phyphox Integration |
|--------|------------------|---------------------|
| **Zuverlässigkeit** | ❌ Inkonsistent | ✅ Sehr zuverlässig |
| **Sensor-Zugriff** | ❌ Eingeschränkt | ✅ Vollständig |
| **Plattformen** | ❌ iOS problematisch | ✅ iOS & Android |
| **Datenpersistenz** | ❌ Keine | ✅ Export möglich |
| **Datenqualität** | ❌ Variable | ✅ Professionell |
| **Zeitstempel** | ❌ Ungenau | ✅ Präzise |
| **Sicherheit** | ❌ Restriktionen | ✅ Keine Probleme |

## Zusammenfassung

**phyphox-Workflow in 4 Schritten:**

1. **📲 phyphox installieren** (5 Minuten, einmalig)
2. **📊 Daten aufzeichnen** (10-30 Sekunden)
3. **📤 Als CSV exportieren** (5 Sekunden)
4. **📈 ICQ berechnen** (sofort in Web-App)

**Vorteile:**
- ✅ Zuverlässige Datenerfassung
- ✅ Professionelle Qualität
- ✅ Einfacher Workflow
- ✅ Standardformate
- ✅ Funktioniert garantiert!

**Empfohlener Einstieg:**
1. phyphox App installieren (kostenlos)
2. Experiment "Acceleration" durchführen (10 Sekunden)
3. Daten als CSV exportieren
4. In `smartphone_app_phyphox.html` hochladen
5. ICQ-Ergebnisse anschauen!

**Dokumentation:**
- `PHYPHOX_INTEGRATION.md` - Vollständige Integration-Anleitung
- `SMARTPHONE_ANLEITUNG.md` - Original Smartphone-App Anleitung
- `DATEN_ANFORDERUNGEN.md` - Allgemeine Datenanforderungen

**Support:**
- phyphox Forum: https://phyphox.org/forum
- phyphox Wiki: https://phyphox.org/wiki
