# MQG-Projekt: Aufgabenliste / Task List

**Status**: 🔄 Aktiv / Active  
**Letzte Aktualisierung / Last Update**: 2026-02-01 22:30:00 UTC

---

## 🎯 Aktuelle Priorität / Current Priority

**Phase 2: Experimentelle Integration und Hardware-Anbindung**

---

## 📋 Offene Aufgaben / Open Tasks

### Hochpriorität / High Priority

#### TASK-007: Integration mit realen Sensoren ⚡ **NEU**
**Status**: Ausstehend / Pending  
**Beschreibung**: Test und Integration der experimentellen Module mit echten physischen Sensoren  
**Akzeptanzkriterien**:
- [ ] Test mit Arduino/Serial-Sensor durchgeführt
- [ ] Netzwerk-Sensor erfolgreich angebunden
- [ ] ICQ-Messung von realen Daten validiert
- [ ] Kalibrierungs-Workflow mit Hardware getestet
**Geschätzte Dauer**: 3 Arbeitseinheiten  
**Abhängigkeiten**: Experimentelle Module (abgeschlossen)  
**Nächster Schritt**: Hardware-Setup vorbereiten, erste Sensoren anschließen

---

#### TASK-008: Live-Monitoring GUI entwickeln 📊 **NEU**
**Status**: Geplant / Planned  
**Beschreibung**: Grafische Benutzeroberfläche für Echtzeit-ICQ-Monitoring  
**Akzeptanzkriterien**:
- [ ] Dashboard mit Live-ICQ-Anzeige
- [ ] Grafische Darstellung der Zeitreihen
- [ ] Sensor-Status-Übersicht
- [ ] Export-Funktionen für Messdaten
**Geschätzte Dauer**: 5 Arbeitseinheiten  
**Abhängigkeiten**: TASK-007  
**Nächster Schritt**: Framework-Auswahl (Tkinter, PyQt, Web-basiert)

---

#### TASK-001: Mathematische Formalisierung des ICQ ⏳
**Status**: Ausstehend / Pending  
**Beschreibung**: Vollständige mathematische Herleitung der ICQ-Formel mit allen Randbedingungen  
**Akzeptanzkriterien**:
- [ ] Formale Definition aller Variablen (S_max, S_actual, C_factor)
- [ ] Herleitung der Normalisierung (0 ≤ ICQ ≤ 1)
- [ ] Beweis der Additivität für unabhängige Subsysteme
- [ ] Dokumentation aller Annahmen und Limitierungen
**Geschätzte Dauer**: 2 Arbeitseinheiten  
**Abhängigkeiten**: Keine  
**Nächster Schritt**: Literaturrecherche zu Shannon-Entropie und Informationsmaßen

---

#### TASK-002: Entwicklung des Software-Messkonzepts 🔄
**Status**: In Planung / In Planning  
**Beschreibung**: Design eines konkreten, softwarebasierten Messverfahrens für ICQ  
**Akzeptanzkriterien**:
- [ ] Spezifikation der Eingabedaten (Format, Struktur)
- [ ] Definition des Messprozesses (Schritte, Algorithmen)
- [ ] Festlegung der Ausgabeformate
- [ ] Fehlerbehandlung und Randfälle definieren
**Geschätzte Dauer**: 3 Arbeitseinheiten  
**Abhängigkeiten**: TASK-001 (mathematische Formalisierung)  
**Nächster Schritt**: Nach Abschluss von TASK-001

---

### Mittlere Priorität / Medium Priority

#### TASK-003: Implementierung der ICQ-Kernalgorithmen 📝
**Status**: Ausstehend / Pending  
**Beschreibung**: Python-Implementierung der ICQ-Berechnungsalgorithmen  
**Akzeptanzkriterien**:
- [ ] `icq_calculator.py` mit Hauptfunktion zur ICQ-Berechnung
- [ ] Unit-Tests für alle Funktionen (mindestens 90% Abdeckung)
- [ ] Dokumentation (Docstrings, README)
- [ ] Beispiele für typische Anwendungsfälle
**Geschätzte Dauer**: 4 Arbeitseinheiten  
**Abhängigkeiten**: TASK-001, TASK-002  
**Nächster Schritt**: Nach Abschluss von TASK-002

---

#### TASK-004: Erstellung einer Simulationsumgebung ⚙️
**Status**: Ausstehend / Pending  
**Beschreibung**: Entwicklung einer Umgebung zur Simulation verschiedener Informationssysteme  
**Akzeptanzkriterien**:
- [ ] Generierung synthetischer Testdaten mit bekannten ICQ-Werten
- [ ] Verschiedene Szenarien (niedrige/hohe Kohärenz)
- [ ] Parametrierbare Simulationen
- [ ] Validierung der Ergebnisse
**Geschätzte Dauer**: 5 Arbeitseinheiten  
**Abhängigkeiten**: TASK-003  
**Nächster Schritt**: Nach Abschluss von TASK-003

---

### Niedrige Priorität / Low Priority

#### TASK-005: Visualisierung und Dashboard 📊
**Status**: Geplant / Planned  
**Beschreibung**: Entwicklung von Visualisierungstools für ICQ-Daten  
**Akzeptanzkriterien**:
- [ ] Zeitreihen-Visualisierung von ICQ-Werten
- [ ] Vergleichsgrafiken für verschiedene Systeme
- [ ] Interaktives Dashboard (optional)
- [ ] Export von Grafiken (PNG, SVG)
**Geschätzte Dauer**: 3 Arbeitseinheiten  
**Abhängigkeiten**: TASK-003, TASK-004  
**Nächster Schritt**: Nach erfolgreicher Validierung der Simulationen

---

#### TASK-006: Automatisches Task-Generierungssystem 🤖
**Status**: Geplant / Planned  
**Beschreibung**: System zur automatischen Generierung neuer Tasks basierend auf Projektfortschritt  
**Akzeptanzkriterien**:
- [ ] Analyse des aktuellen Projektstatus
- [ ] Identifikation von Lücken und nächsten Schritten
- [ ] Automatische Erstellung von Task-Beschreibungen
- [ ] Priorisierung nach Abhängigkeiten
**Geschätzte Dauer**: 6 Arbeitseinheiten  
**Abhängigkeiten**: TASK-001 bis TASK-005 (Basis-Funktionalität muss vorhanden sein)  
**Nächster Schritt**: Nach Phase 1 und 2

---

## ✅ Abgeschlossene Aufgaben / Completed Tasks

### TASK-EXP-001: Experimentelle Hardware-Interface-Module ✓ **NEU**
**Status**: Abgeschlossen / Completed  
**Abschlussdatum**: 2026-02-01 22:30:00 UTC  
**Ergebnis**: 
- ✅ `hardware_interface.py` - Sensor-Adapter und Hardware-Manager
- ✅ `experimental_adapter.py` - Echtzeit-Prozessor und Daten-Import
- ✅ `calibration.py` - Kalibrierungs- und Validierungs-System
- ✅ EXPERIMENTAL_GUIDE.md - Vollständige Dokumentation
- ✅ Unterstützung für Serial, Network und Custom Sensoren
- ✅ Real-Time ICQ-Berechnung mit Sliding Window
- ✅ Sensor-Kalibrierung (linear und polynomial)

---

### TASK-000: Projektinitialisierung ✓
**Status**: Abgeschlossen / Completed  
**Abschlussdatum**: 2026-02-01 21:40:50 UTC  
**Ergebnis**: 
- ✅ MQG_Project Verzeichnisstruktur erstellt
- ✅ README.md mit theoretischer Grundlage
- ✅ log.md für kontinuierliche Dokumentation
- ✅ tasks.md für Aufgabenverwaltung
- ✅ src/ Verzeichnis für Implementierungen

---

## 📊 Fortschrittsübersicht / Progress Overview

- **Abgeschlossen / Completed**: 2 Tasks (22%)
- **In Bearbeitung / In Progress**: 2 Tasks (experimentelle Integration)
- **Ausstehend / Pending**: 6 Tasks (78%)
- **Gesamt / Total**: 9 Tasks
- **In Bearbeitung / In Progress**: 0 Tasks (0%)
- **Ausstehend / Pending**: 6 Tasks (86%)
- **Gesamt / Total**: 7 Tasks

---

## 🔄 Nächste Schritte / Next Steps

1. **Sofort**: TASK-001 starten - Mathematische Formalisierung
2. **Danach**: TASK-002 - Messkonzept entwickeln
3. **Parallel möglich**: Dokumentation erweitern, Literaturrecherche

---

## 🤖 Automatische Aktualisierung / Automatic Updates

Diese Datei wird automatisch aktualisiert, wenn:
- Ein Task abgeschlossen wird
- Ein neuer Task identifiziert wird
- Prioritäten sich ändern
- Abhängigkeiten sich ändern

**Update-Frequenz**: Bei jedem bedeutenden Projektfortschritt  
**Verantwortlich**: Automatisches Task-Management-System (in Entwicklung)

---

## 📝 Hinweise / Notes

- Tasks sind atomar und testbar definiert
- Jeder Task hat klare Akzeptanzkriterien
- Abhängigkeiten sind explizit dokumentiert
- Zeitschätzungen sind Richtwerte und können angepasst werden
- Prioritäten können sich basierend auf neuen Erkenntnissen ändern
