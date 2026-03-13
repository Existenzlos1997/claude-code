# MQG Simulationsergebnisse - Schnellzugriff

## Alle Durchgeführten Simulationen

Dieser Guide bietet schnellen Zugriff auf alle Simulationsergebnisse.

---

## 1. Visualisierungen (PNG)

### Doppelspalt-Vergleich
**Datei:** `simulation_results/double_slit_comparison.png` (535 KB)
**Zeigt:** 3-Panel Vergleich (Standard QM, MQG, Abweichung)

### Hotspot-Details  
**Datei:** `simulation_results/hotspot_details.png` (313 KB)
**Zeigt:** Detaillierte Ansicht aller 30 Hotspots mit Abweichungen >2σ

### 2D-Heatmap
**Datei:** `simulation_results/deviation_heatmap_2d.png` (3.2 MB)
**Zeigt:** Räumliche Verteilung der Abweichungen

---

## 2. Numerische Daten (JSON)

### Doppelspalt-Ergebnisse
**Datei:** `simulation_results/simulation_results.json` (78 KB)
```json
{
  "timestamp": "...",
  "statistics": {
    "chi_squared": 1526.25,
    "p_value": 0.0,
    "dof": 999
  },
  "num_hotspots": 30,
  "hotspots": [...]
}
```

### Validierungsergebnisse
**Datei:** `MQG_validation_results.json` (16 KB)
- 19 Experimente
- Suite-Ergebnisse
- Zeitstempel

### Erweiterte Validierung
**Datei:** `extended_validation_results.json` (9 KB)
- 41 Experimente
- 6 Typen
- Detaillierte Metriken

### Kombinierte Analyse
**Datei:** `COMPREHENSIVE_ANALYSIS_SUMMARY.json` (2 KB)
- Zusammenfassung aller Simulationen
- Hauptergebnisse
- Gesamt-Status

---

## 3. Analyse-Scripts

### Hauptanalyse
**Datei:** `analyze_all_results.py`
**Funktion:** Lädt alle JSON, analysiert, erstellt Report
**Ausführung:** `python analyze_all_results.py`

### Demo-Scripts
- `demo_double_slit.py` - Doppelspalt-Simulation
- `run_all_experiments.py` - Umfassende Validierung
- `run_extended_experiments.py` - Erweiterte Tests

---

## 4. Berichte

### Umfassende Zusammenfassung
**Datei:** `SIMULATION_COMPLETE_SUMMARY.md`
- Alle Ergebnisse
- Interpretationen
- Nächste Schritte

### Analyse-Output
**Datei:** `comprehensive_analysis_output.txt`
- Terminal-Output
- Statistische Zusammenfassung
- Experiment-Übersicht

---

## Quick Commands

### Visualisierungen anzeigen
```bash
# Alle PNG-Dateien auflisten
ls -lh simulation_results/*.png
```

### Daten analysieren
```bash
# Python-Analyse ausführen
python analyze_all_results.py

# JSON-Daten anzeigen
cat COMPREHENSIVE_ANALYSIS_SUMMARY.json | python -m json.tool
```

### Hotspot-Positionen extrahieren
```bash
# Top 10 Hotspots anzeigen
python -c "import json; d=json.load(open('simulation_results/simulation_results.json')); [print(f\"{h['position_um']:+7.2f} μm\") for h in d['hotspots'][:10]]"
```

---

## Hauptergebnisse auf einen Blick

**Chi-Quadrat:** 1526.25  
**p-Wert:** < 0.001  
**Hotspots:** 30 Positionen  
**Signifikanz:** Hochsignifikant  

**Top 5 Hotspot-Positionen:**
1. -5.76 μm
2. -5.66 μm  
3. -5.56 μm
4. -5.46 μm
5. -4.65 μm

---

*Generiert: 2026-03-13*
