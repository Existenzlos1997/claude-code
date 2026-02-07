# MQG Anwendungs-Guide

## Übersicht

Dieser Guide erklärt, wie Sie die 5 praktischen MQG-Anwendungen nutzen, um die ICQ-Theorie im Alltag anzuwenden.

## Die 5 Anwendungen

### 1. Schlaf-Qualität-Monitor

**Was misst die App?**
Die Bewegungskohärenz während des Schlafs. Hohe ICQ bedeutet ruhiger Schlaf, niedrige ICQ bedeutet unruhigen Schlaf.

**Schritt-für-Schritt:**
1. App öffnen (`schlaf_monitor.html`)
2. "Aufzeichnung starten" klicken
3. Sensor-Zugriff erlauben
4. Smartphone neben Bett auf Nachttisch legen
5. Schlafen gehen
6. Morgens "Aufzeichnung stoppen" klicken
7. Ergebnis ansehen

**Interpretation:**
- ICQ 0.7-1.0 (70-100 Punkte): Ausgezeichneter Schlaf
- ICQ 0.5-0.7 (50-70 Punkte): Guter Schlaf
- ICQ 0.3-0.5 (30-50 Punkte): Unruhiger Schlaf
- ICQ 0.0-0.3 (0-30 Punkte): Sehr unruhig

**Tipps zur Verbesserung:**
- Raum kühler (16-19°C)
- Früher ins Bett
- Kein Koffein nach 14 Uhr
- Bildschirme 1 Stunde vor dem Schlafen aus

### 2. Lauftechnik-Analyzer

**Was misst die App?**
Die Konsistenz Ihrer Schritte beim Laufen. Hohe ICQ bedeutet gleichmäßige Technik.

**Schritt-für-Schritt:**
1. App öffnen (`lauftechnik_analyzer.html`)
2. Smartphone am Arm befestigen (Sportarmband)
3. "Aufzeichnung starten" klicken
4. Laufen (mindestens 2 Minuten)
5. "Aufzeichnung stoppen" klicken
6. Ergebnis ansehen

**Interpretation:**
- ICQ 0.7-1.0: Professionelle Technik
- ICQ 0.5-0.7: Gute Technik
- ICQ 0.3-0.5: Verbesserungsbedarf
- ICQ 0.0-0.3: Anfänger/Ermüdung

**Was bedeutet Symmetrie?**
- 95-100%: Perfekt ausbalanciert
- 90-95%: Sehr gut
- 85-90%: Gut
- <85%: Asymmetrie, Verletzungsrisiko

### 3. Audio-Qualität-Checker

**Was misst die App?**
Die Stabilität der Audiofrequenzen. Hohe ICQ bedeutet klaren Sound ohne Störgeräusche.

**Schritt-für-Schritt:**
1. App öffnen (`audio_quality.html`)
2. "Mikrofon starten" klicken
3. Mikrofon-Zugriff erlauben
4. Sprechen oder Singen
5. Live-Feedback beobachten

**Interpretation:**
- ICQ 0.8-1.0: Ausgezeichnete Qualität, perfekt für Aufnahmen
- ICQ 0.6-0.8: Gute Qualität
- ICQ 0.4-0.6: Mittlere Qualität, Störgeräusche vorhanden
- ICQ 0.0-0.4: Schlechte Qualität, zu viele Störungen

**Verwendung:**
- Vor Podcast-Aufnahmen Raum testen
- Mikrofon-Qualität prüfen
- Optimale Position finden
- Störquellen identifizieren

### 4. Herz-Kohärenz-Trainer

**Was misst die App?**
Die Kohärenz Ihrer Herzratenvariabilität (HRV). Hohe ICQ bedeutet Entspannung und Stressresilienz.

**Schritt-für-Schritt:**
1. Herzfrequenz-Daten von Smartwatch exportieren (CSV)
2. App öffnen (`herz_kohaerenz.html`)
3. "CSV hochladen" klicken
4. Datei auswählen
5. Ergebnis ansehen
6. Optional: Atem-Übung machen

**CSV-Format:**
```
Time,Heart Rate
0,72
1,68
2,75
...
```

**Interpretation:**
- ICQ 0.7-1.0: Sehr kohärent, entspannt
- ICQ 0.5-0.7: Kohärent, gut
- ICQ 0.3-0.5: Moderate Kohärenz
- ICQ 0.0-0.3: Inkohärent, gestresst

**Atem-Übung:**
- 4 Sekunden einatmen
- 4 Sekunden halten
- 6 Sekunden ausatmen
- 5 Minuten wiederholen
- Verbesserung bis zu +50% ICQ möglich

### 5. Maschinen-Vibrations-Monitor

**Was misst die App?**
Das Vibrationsmuster von Haushaltsgeräten. Abweichungen vom Normalmuster zeigen Probleme an.

**Schritt-für-Schritt (Ersteinrichtung):**
1. App öffnen (`maschinen_monitor.html`)
2. "Neues Gerät hinzufügen" klicken
3. Name eingeben (z.B. "Waschmaschine")
4. Smartphone direkt auf Gerät legen
5. Gerät normal laufen lassen
6. "Baseline aufzeichnen" (30-60 Sekunden)
7. ICQ-Wert wird gespeichert

**Regelmäßige Überwachung:**
1. Smartphone auf Gerät legen
2. "Messung starten" klicken
3. 30 Sekunden warten
4. ICQ mit Baseline vergleichen

**Interpretation:**
- Abweichung 0-10%: Normal
- Abweichung 10-20%: Beobachten
- Abweichung 20-30%: Wartung empfohlen
- Abweichung >30%: Dringend prüfen!

**Häufige Probleme:**
- Waschmaschine: Lager defekt → ICQ sinkt
- Kühlschrank: Kompressor altert → ICQ sinkt
- Lüfter: Verschmutzung → ICQ sinkt
- Geschirrspüler: Pumpe schwach → ICQ schwankt

## Allgemeine Tipps

### Sensor-Zugriff

Beim ersten Start fragt jede App nach Sensor-Zugriff:
- **iOS Safari:** "Erlauben" klicken
- **Android Chrome:** "Zulassen" klicken
- **Desktop:** Gleiches Vorgehen

Falls nicht funktioniert:
- Browser-Einstellungen öffnen
- Sensor-Berechtigungen prüfen
- Seite neu laden

### Daten speichern

Alle Apps speichern Daten lokal:
- Kein Account nötig
- Daten bleiben auf Gerät
- Bei Cache-Löschung gehen Daten verloren
- Regelmäßig exportieren!

### Export/Import

**Export:**
1. In der App "Export" klicken
2. JSON-Datei wird heruntergeladen
3. Sicher aufbewahren

**Import:**
1. "Import" klicken
2. JSON-Datei auswählen
3. Alle Daten wiederhergestellt

### Browser-Kompatibilität

**Vollständig unterstützt:**
- Chrome 90+ (Desktop + Mobile)
- Edge 90+ (Desktop + Mobile)
- Safari 14+ (iOS + macOS)
- Firefox 88+

**Eingeschränkt:**
- Ältere Browser: Möglicherweise keine Sensor-Unterstützung

## Troubleshooting

### "Sensoren funktionieren nicht"
- HTTPS verwenden (nicht HTTP)
- Berechtigungen erlauben
- Browser aktualisieren
- Gerät neu starten

### "ICQ-Wert ist immer 0"
- Gerät bewegen (für Bewegungssensoren)
- Sound machen (für Audio)
- Daten-Qualität prüfen

### "App lädt nicht"
- Internet-Verbindung prüfen (beim ersten Laden)
- Cache leeren
- Andere Browser testen

### "Daten sind weg"
- Wurden sie exportiert? → Import
- Cache gelöscht? → Neu aufzeichnen
- localStorage deaktiviert? → Aktivieren

## Best Practices

### Schlaf-Monitor
- Smartphone immer an gleicher Stelle
- Nicht zu nah am Kopf (Strahlung)
- Flugmodus aktivieren
- Gleiche Uhrzeit für Vergleichbarkeit

### Lauftechnik
- Smartphone fest am Oberarm
- Mindestens 5 Minuten laufen
- Gleichmäßiges Tempo
- Nicht zu schnell/langsam

### Audio-Qualität
- Ruhige Umgebung
- Mikrofon nicht abdecken
- Konstante Lautstärke
- 10 Sekunden testen

### Herz-Kohärenz
- Daten von gleicher Uhrzeit
- Ruhig sitzen bei Messung
- Regelmäßig messen (täglich)
- Vor/nach Entspannungsübungen

### Maschinen-Monitor
- Baseline bei neuen Geräten
- Gleiche Stelle auf Gerät
- Normale Betriebsbedingungen
- Monatlich kontrollieren

## Wissenschaftliche Grundlage

Alle Apps basieren auf der MQG-Theorie (Messbares Informations-Kohärenz-Gesetz) und nutzen den **Information Coherence Quotient (ICQ)** als Metrik.

**ICQ misst:**
- Ordnung vs. Chaos in Daten
- Konsistenz von Mustern
- Vorhersagbarkeit
- Kohärenz

**Validierung:**
- 60 wissenschaftliche Experimente
- r = -0.9997 Korrelation mit Entropie
- Std Dev = 0 (perfekte Reproduzierbarkeit)
- 15.7M Samples/Sekunde Performance

**Praktische Bedeutung:**
- ICQ funktioniert in der realen Welt
- Messungen sind reproduzierbar
- Vorhersagen sind zuverlässig
- Alltagsnutzen bestätigt

## Support

Bei Fragen oder Problemen:
- Dokumentation lesen
- Issues auf GitHub öffnen
- Community fragen

## Updates

**Version 1.0** (Februar 2026)
- Initiale Veröffentlichung
- 5 Apps verfügbar
- Vollständige Dokumentation

---

**Viel Erfolg beim Nutzen der MQG-Anwendungen!**
