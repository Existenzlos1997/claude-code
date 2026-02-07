# phyphox Integration Guide

## Übersicht

**phyphox** (Physical Phone Experiments) ist eine professionelle App zur Aufzeichnung von Smartphone-Sensordaten, entwickelt von der RWTH Aachen University.

Diese Anleitung zeigt, wie Sie phyphox-Daten in das MQG-System importieren und analysieren können.

## Warum phyphox?

### Vorteile gegenüber direkter Browser-Sensorabfrage

✅ **Zuverlässige Datenerfassung**
- Keine Browser-Sicherheitsbeschränkungen
- Zugriff auf alle Smartphone-Sensoren
- Professionelle Datenaufzeichnung

✅ **Bessere Datenqualität**
- Präzise Zeitstempel
- Kalibrierte Sensoren
- Hohe Abtastraten (bis 1000 Hz)

✅ **Flexibilität**
- Daten werden gespeichert
- Offline-Analyse möglich
- Wiederholte Auswertung

✅ **Standardformate**
- CSV-Export
- Excel-Export
- Einfach zu verarbeiten

## Installation

### phyphox App installieren

**iOS (iPhone/iPad):**
- App Store öffnen
- "phyphox" suchen
- Installieren (kostenlos)
- App öffnen

**Android:**
- Google Play Store öffnen
- "phyphox" suchen
- Installieren (kostenlos)
- App öffnen

**Offizielle Website:** https://phyphox.org

## Workflow: Von Messung bis ICQ-Berechnung

### Schritt 1: Daten mit phyphox aufzeichnen

1. **Experiment auswählen:**
   - phyphox öffnen
   - Kategorie wählen (z.B. "Acceleration")
   - Experiment auswählen (z.B. "Acceleration (without g)")

2. **Messung starten:**
   - ▶️ Play-Button drücken
   - Smartphone bewegen/testen
   - ⏸️ Pause-Button zum Stoppen

3. **Daten überprüfen:**
   - Graphen anschauen
   - Zeitbereich prüfen
   - Bei Bedarf wiederholen

### Schritt 2: Daten exportieren

1. **Menü öffnen:**
   - ⋮ Menü-Button (oben rechts)
   - "Export Data" wählen

2. **Format wählen:**
   - **CSV** (empfohlen für einfache Analyse)
   - **Excel** (für erweiterte Bearbeitung)

3. **Exportieren:**
   - "Share" oder "Save" wählen
   - Per Email senden
   - In Cloud speichern
   - Auf Computer übertragen

### Schritt 3: Daten analysieren

**Option A: Web-App (Smartphone/Computer)**

1. Datei auf `smartphone_app_phyphox.html` hochladen
2. Automatische ICQ-Berechnung
3. Ergebnisse anzeigen

**Option B: Python-Kommandozeile**

```bash
python phyphox_analyzer.py meine_daten.csv
```

**Option C: Programmatisch in Python**

```python
from src.experimental.phyphox_import import PhyphoxDataImporter

importer = PhyphoxDataImporter()
importer.import_csv('meine_daten.csv')
icq_values = importer.calculate_icq()
print(importer.get_summary())
```

## Unterstützte Sensoren

### Accelerometer (Beschleunigungssensor)

**Experimente in phyphox:**
- "Acceleration" (mit Erdbeschleunigung)
- "Acceleration (without g)" (ohne Erdbeschleunigung)
- "Linear Acceleration"

**Datenformat:**
```
Time (s), Acceleration x (m/s^2), Acceleration y (m/s^2), Acceleration z (m/s^2)
0.00, -0.123, 0.456, 9.812
0.01, -0.145, 0.434, 9.798
...
```

**ICQ-Interpretation:**
- Hoher ICQ (0.7-1.0): Gleichmäßige Bewegung
- Mittlerer ICQ (0.4-0.7): Variable Bewegung
- Niedriger ICQ (0.0-0.4): Chaotische Bewegung

### Gyroscope (Gyroskop)

**Experimente in phyphox:**
- "Gyroscope"

**Datenformat:**
```
Time (s), Gyroscope x (rad/s), Gyroscope y (rad/s), Gyroscope z (rad/s)
0.00, 0.012, -0.034, 0.056
0.01, 0.015, -0.031, 0.053
...
```

**ICQ-Interpretation:**
- Hoher ICQ: Gleichmäßige Rotation
- Niedriger ICQ: Wackelige/ungleichmäßige Rotation

### Magnetometer (Magnetfeldsensor)

**Experimente in phyphox:**
- "Magnetometer"

**Datenformat:**
```
Time (s), Magnetic field x (µT), Magnetic field y (µT), Magnetic field z (µT)
0.00, 12.3, 45.6, 78.9
0.01, 12.1, 45.8, 78.7
...
```

### GPS / Location

**Experimente in phyphox:**
- "Location"
- "GPS"

**Hinweis:** GPS-Daten können auch mit MQG analysiert werden, um Bewegungsmuster zu erkennen.

## Beispiel-Experimente

### Experiment 1: Handheld-Stabilität testen

**Ziel:** ICQ der Handstabilität beim Halten des Smartphones messen

**Durchführung:**
1. phyphox → "Acceleration (without g)"
2. Smartphone in der Hand halten (so ruhig wie möglich)
3. 10 Sekunden aufzeichnen
4. Daten exportieren und analysieren

**Erwartetes Ergebnis:**
- Ruhige Hand: ICQ > 0.6
- Normale Hand: ICQ 0.4-0.6
- Zittrige Hand: ICQ < 0.4

### Experiment 2: Gehbewegung analysieren

**Ziel:** ICQ der Gehbewegung messen

**Durchführung:**
1. phyphox → "Acceleration"
2. Smartphone in Tasche stecken
3. Normal gehen (20-30 Schritte)
4. Daten exportieren

**Erwartetes Ergebnis:**
- Gleichmäßiges Gehen: ICQ 0.5-0.7
- Ungleichmäßiges Gehen: ICQ 0.3-0.5

### Experiment 3: Fahrzeugerkennung

**Ziel:** Verschiedene Verkehrsmittel anhand von ICQ unterscheiden

**Durchführung:**
1. Daten in verschiedenen Fahrzeugen aufzeichnen:
   - Auto
   - Zug
   - Fahrrad
   - Zu Fuß

2. ICQ für jedes Verkehrsmittel berechnen

**Erwartete Unterschiede:**
- Zug: Hoher ICQ (glatte Fahrt)
- Auto: Mittlerer ICQ (Beschleunigung/Bremsen)
- Fahrrad: Variabler ICQ (Pedalbewegung)
- Zu Fuß: Periodischer mittlerer ICQ

## Datenformat-Details

### CSV-Format

**Standard phyphox CSV:**
- Erste Zeile: Spaltenüberschriften
- Komma oder Tab als Trennzeichen
- Dezimalpunkt (nicht Komma)
- Zeit in Sekunden

**Beispiel:**
```csv
Time (s),Acceleration x (m/s^2),Acceleration y (m/s^2),Acceleration z (m/s^2)
0.00,-0.123,0.456,9.812
0.01,-0.145,0.434,9.798
```

**Automatische Erkennung:**
- Zeitspalte wird automatisch erkannt
- X-, Y-, Z-Komponenten werden erkannt
- Sensortyp wird aus Spaltennamen abgeleitet

### Excel-Format

**Unterstützt:**
- .xlsx Dateien
- Erste Zeile = Überschriften
- Gleiche Struktur wie CSV

## Fehlerbehebung

### "Datei kann nicht gelesen werden"

**Mögliche Ursachen:**
- Falsches Dateiformat
- Korrupte Datei
- Fehlende Spaltenüberschriften

**Lösung:**
- CSV-Datei in Texteditor öffnen und prüfen
- Sicherstellen, dass erste Zeile Überschriften enthält
- Neu aus phyphox exportieren

### "Keine ICQ-Werte berechnet"

**Mögliche Ursachen:**
- Zu wenige Datenpunkte (< 10)
- Alle Werte identisch
- Fehlende Daten (NaN)

**Lösung:**
- Längere Messung durchführen
- Sensor während Messung bewegen
- Datenqualität in phyphox prüfen

### "Seltsame ICQ-Werte"

**Mögliche Ursachen:**
- Sensor nicht kalibriert
- Extreme Ausreißer in Daten
- Skalierungsfehler

**Lösung:**
- Smartphone neu starten
- Sensor in phyphox kalibrieren
- Ausreißer manuell entfernen

## Best Practices

### Datenaufzeichnung

✅ **Vor der Messung:**
- Smartphone-Sensoren kalibrieren (in phyphox)
- Ausreichend Speicherplatz sicherstellen
- Andere Apps schließen

✅ **Während der Messung:**
- Konstante Bedingungen beibehalten
- Messdauer: mindestens 5 Sekunden
- Bei Bewegung: gleichmäßig bewegen

✅ **Nach der Messung:**
- Daten in phyphox visualisieren
- Auf Fehler/Ausreißer prüfen
- Bei Bedarf wiederholen

### Datenexport

✅ **Format-Wahl:**
- CSV: Für einfache Analyse (empfohlen)
- Excel: Für manuelle Bearbeitung
- JSON: Für Programmierer

✅ **Speicherort:**
- Cloud (Dropbox, Google Drive) für Zugriff von mehreren Geräten
- Email für schnellen Transfer
- Lokaler Speicher für Offline-Analyse

### ICQ-Analyse

✅ **Interpretation:**
- ICQ immer im Kontext betrachten
- Mehrere Messungen vergleichen
- Referenzwerte aufzeichnen

✅ **Validierung:**
- Messung wiederholen (Reproduzierbarkeit)
- Verschiedene Bedingungen testen
- Mit Erwartungen vergleichen

## Weitere Ressourcen

**phyphox:**
- Website: https://phyphox.org
- Forum: https://phyphox.org/forum
- Wiki: https://phyphox.org/wiki
- GitHub: https://github.com/phyphox

**MQG-Theorie:**
- README.md → Theoretische Grundlagen
- VALIDATION_BEDEUTUNG.md → Experimentelle Bestätigung
- EXPERIMENT_SUMMARY.md → Validierungsexperimente

**Support:**
- SMARTPHONE_ANLEITUNG.md → Smartphone-App Anleitung
- DATEN_ANFORDERUNGEN.md → Allgemeine Datenanforderungen
- PRAKTISCHER_EINSTIEG.md → Praktische Beispiele

## Zusammenfassung

**phyphox Integration bietet:**
- ✅ Zuverlässige Sensordatenerfassung
- ✅ Einfacher Export in Standardformaten
- ✅ Automatische ICQ-Berechnung
- ✅ Validierung der MQG-Theorie mit echten Daten

**Workflow in 3 Schritten:**
1. Daten in phyphox aufzeichnen
2. Als CSV/Excel exportieren
3. In MQG-System importieren → ICQ berechnen

**Empfohlener Start:**
1. phyphox App installieren
2. Experiment "Acceleration" durchführen
3. Daten exportieren
4. In `smartphone_app_phyphox.html` hochladen
5. ICQ-Ergebnisse anschauen!
