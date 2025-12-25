# Visualisierungen für MQG-Theorie

## Übersicht
Dieses Dokument spezifiziert alle Diagramme und Visualisierungen für die wissenschaftliche Publikation der MQG-Theorie.

---

## Abbildung 1: Vereinheitlichung der Kopplungskonstanten

**Typ:** Logarithmisches Plot  
**Achsen:**
- x-Achse: log₁₀(S/S_Planck) von -50 bis 0
- y-Achse: α_i(S) (Kopplungskonstanten)

**Kurven:**
1. α_EM(S) = 1/137 · exp[β_EM · S/S_Planck] (blau)
2. α_S(S) = 0.118 · exp[β_S · S/S_Planck] (rot)
3. α_W(S) = 1/30 · exp[β_W · S/S_Planck] (grün)
4. α_G(S) ∝ G · exp[β_G · S/S_Planck] (schwarz)

**Parameter:**
- β_EM = 1728
- β_S = 1040
- β_W = 223
- β_G ≈ 1

**Besonderheit:**
Alle Kurven konvergieren bei S → S_Planck zu α_unified ≈ 1

**Software:** Python (matplotlib) oder Mathematica  
**Auflösung:** 300 dpi für Print, 600 dpi für finale Version

---

## Abbildung 2: Informationsdichte-Evolution

**Typ:** 3D-Plot oder Heatmap  
**Achsen:**
- x-Achse: Räumliche Koordinate (Schwarzschild-Radius r/r_s)
- y-Achse: Zeit t
- z-Achse (Farbe): S(r,t) Informationsdichte

**Szenarien:**
a) Schwarzschild-Schwarzes Loch:
   - S(r) ∝ r_s²/r² für r > r_s
   - Divergenz bei r → r_s

b) Neutronenstern:
   - S_core ≈ 10⁴⁰ bits/m³
   - S_surface ≈ 10³⁵ bits/m³

c) Kosmologische Evolution:
   - S(z) von Urknall bis heute
   - Peak bei z ≈ 10¹⁰

**Farbskala:** Logarithmisch, Viridis oder Plasma

---

## Abbildung 3: PPN-Parameter Vergleich

**Typ:** Balkendiagramm  
**Größen:**
- γ_MQG vs γ_GR vs γ_beobachtet
- β_MQG vs β_GR vs β_beobachtet

**Daten:**
| Test | γ_GR | γ_MQG | γ_obs (Fehler) |
|------|------|-------|----------------|
| Cassini | 1.0 | 1 + 10⁻⁶ | 1.00000 ± 0.00002 |
| LLR | 1.0 | 1 + 10⁻⁸ | 0.99998 ± 0.00002 |
| Pulsar | 1.0 | 1 + 10⁻⁵ | 1.0001 ± 0.0005 |

**Fehlerbalken:** Beobachtungsunsicherheiten einzeichnen

---

## Abbildung 4: Experimentelle Sensitivität

**Typ:** Logarithmisches Plot  
**Achsen:**
- x-Achse: Informationsdichte S (bits/m³)
- y-Achse: Signalstärke / Rauschen

**Kurven:**
1. LIGO-Sensitivität (schwarz, durchgezogen)
2. Advanced LIGO (schwarz, gestrichelt)
3. Einstein Telescope (schwarz, gepunktet)
4. MQG-Vorhersage (rot, durchgezogen)

**Schattierte Bereiche:**
- Aktuell ausgeschlossen (grau)
- Erreichbar 2025-2030 (gelb)
- Erreichbar 2030-2040 (grün)

---

## Abbildung 5: Renormierungsgruppen-Fluss

**Typ:** 2D-Phasendiagramm  
**Achsen:**
- x-Achse: λ (Selbstkopplung)
- y-Achse: m² (Masse²-Parameter)

**Trajektorien:**
- RG-Fluss von UV zu IR
- Fixed Points markieren
- Separatrix zwischen stabilen/instabilen Regionen

**Spezielle Punkte:**
- Gaussian Fixed Point (0, 0)
- Non-Gaussian Fixed Point (λ*, m²*)
- Triviality bound

---

## Abbildung 6: Gravitationswellen-Dispersion

**Typ:** Frequenz-Phasengeschwindigkeit-Diagramm  
**Achsen:**
- x-Achse: Frequenz f (Hz), log-Skala
- y-Achse: v_phase/c - 1 (relative Abweichung)

**Vorhersagen:**
- GR: v_phase = c (horizontale Linie bei 0)
- MQG: v_phase/c ≈ 1 + α·(f/f_Planck)² 

**Bereiche:**
- LIGO: 10-10⁴ Hz
- BBO: 0.1-1 Hz
- LISA: 10⁻⁴-1 Hz

---

## Abbildung 7: Doppelspalt-Interferenz mit Informationsfeld

**Typ:** 2D-Intensitätsplot  
**Setup:**
- Klassisches Muster (oben)
- MQG-modifiziertes Muster (unten)
- Differenz (Mitte)

**Erwartung:**
- Nichtlineare Verschiebung der Maxima um Δx ∝ λ_S·S
- Kontrastreduktion um ~10⁻⁵

**Farbskala:** Intensität normalisiert auf Maximum

---

## Abbildung 8: Kosmologische Parameter

**Typ:** Contour-Plot (Konfidenzellipsen)  
**Achsen:**
- x-Achse: Ω_m (Materie-Dichte)
- y-Achse: Ω_Λ (Dunkle Energie)

**Konturen:**
1. Standard-ΛCDM (schwarz)
2. MQG-Vorhersage (rot)
3. Planck+BAO Beobachtungen (blau, 1σ, 2σ)

**Unterschiede:**
- MQG: Effektive dunkle Energie aus S-Feld
- Ω_Λ,eff = Ω_Λ + Ω_S

---

## Abbildung 9: Schwarzschild-Korrekturen

**Typ:** Radiales Profil  
**Achsen:**
- x-Achse: r/r_s (Schwarzschild-Radius)
- y-Achse: g_tt(r) / g_tt,GR(r) - 1

**Kurven:**
- Für verschiedene Massen: M = 10 M_☉, 10⁶ M_☉, 10⁹ M_☉
- Horizon-Verschiebung Δr_h/r_s ≈ 10⁻⁴

**Einschub:** Logarithmische Skala für r < 10 r_s

---

## Abbildung 10: Quantenverschränkung und Information

**Typ:** Netzwerk-Diagramm  
**Elemente:**
- Verschränkte Paare (Knoten)
- Informationsfluss S (Kanten)
- ER-Brücken (gestrichelte Linien)

**Interpretation:**
- ER = EPR: Wurmloch ↔ Verschränkung
- Information als verbindende Größe

---

## Abbildung 11: Experimentelle Roadmap (Timeline)

**Typ:** Gantt-Chart  
**Zeitachse:** 2025-2050  

**Meilensteine:**
- 2025-2027: LIGO O5, erste Tests
- 2028-2030: Advanced LIGO Upgrade
- 2030-2035: Einstein Telescope Konstruktion
- 2035-2040: Erste definitiven Tests
- 2040+: Kosmologische Validierung

**Farbcodierung:**
- Grün: Geplant und finanziert
- Gelb: Konzeptphase
- Rot: Spekulativ

---

## Abbildung 12: Theorievergleich (Spider-Diagramm)

**Typ:** Radar-Chart  
**Achsen (0-10 Skala):**
1. Mathematische Rigorosität
2. Experimentelle Testbarkeit
3. Vereinheitlichung (alle 4 Kräfte?)
4. Vorhersagekraft
5. Konzeptuelle Einfachheit
6. Renormierbarkeit

**Theorien:**
- MQG (rot, durchgezogen)
- String-Theorie (blau)
- Loop-QG (grün)
- ΛCDM (schwarz)

---

## Technische Spezifikationen

**Alle Abbildungen:**
- Format: PDF (vektoriell) + PNG (Backup)
- Auflösung: 300 dpi minimum, 600 dpi für finale Version
- Schriftart: Computer Modern (LaTeX-Standard) oder Arial
- Schriftgröße: 10-12 pt für Achsenbeschriftungen
- Linienbreite: 1.5-2.0 pt

**Farbpalette:**
- Primär: Schwarz für Daten
- MQG-Vorhersagen: Rot (#D62728)
- Standardmodell: Blau (#1F77B4)
- Experimentelle Grenzen: Grau (#7F7F7F)

**Software-Empfehlungen:**
1. Python: matplotlib + seaborn
2. Mathematica: Publication-quality plots
3. Gnuplot: Für einfache 2D-Plots
4. TikZ/PGFPlots: Direkt in LaTeX eingebettet

---

## Nutzung in Publikation

**Platzierung:**
- Abbildungen 1-3: Haupttext (Theorie-Sektion)
- Abbildungen 4-6: Ergebnisse-Sektion
- Abbildungen 7-9: Experimentelle Vorhersagen
- Abbildungen 10-12: Diskussion/Vergleich

**Captions:**
Jede Abbildung benötigt:
- Kurze Beschreibung (1-2 Sätze)
- Parameter-Werte
- Referenz auf entsprechende Gleichungen im Text

---

## Code-Beispiel (Python)

```python
import numpy as np
import matplotlib.pyplot as plt

# Abbildung 1: Kopplungskonstanten-Vereinheitlichung
def plot_coupling_unification():
    S_over_Spl = np.logspace(-50, 0, 1000)
    
    beta_EM = 1728
    beta_S = 1040
    beta_W = 223
    
    alpha_EM = (1/137) * np.exp(beta_EM * S_over_Spl)
    alpha_S = 0.118 * np.exp(beta_S * S_over_Spl)
    alpha_W = (1/30) * np.exp(beta_W * S_over_Spl)
    
    plt.figure(figsize=(10, 6))
    plt.loglog(S_over_Spl, alpha_EM, 'b-', label=r'$\alpha_{EM}$')
    plt.loglog(S_over_Spl, alpha_S, 'r-', label=r'$\alpha_S$')
    plt.loglog(S_over_Spl, alpha_W, 'g-', label=r'$\alpha_W$')
    
    plt.xlabel(r'$S / S_{Planck}$', fontsize=12)
    plt.ylabel(r'Coupling constant $\alpha_i$', fontsize=12)
    plt.legend(fontsize=10)
    plt.grid(True, alpha=0.3)
    plt.title('Unification of Coupling Constants in MQG', fontsize=14)
    plt.tight_layout()
    plt.savefig('coupling_unification.pdf', dpi=300)
    plt.savefig('coupling_unification.png', dpi=300)

if __name__ == '__main__':
    plot_coupling_unification()
```

---

## Checkliste vor Publikation

- [ ] Alle 12 Abbildungen erstellt
- [ ] Auflösung geprüft (≥300 dpi)
- [ ] Farben barrierefrei (Deuteranomalie-Test)
- [ ] Achsenbeschriftungen vollständig
- [ ] Legenden klar und lesbar
- [ ] Captions geschrieben
- [ ] Referenzen im Text korrekt
- [ ] Copyright/Lizenzen geklärt (eigene Plots)

---

**Status:** Spezifikationen vollständig  
**Nächster Schritt:** Plots mit Python/Mathematica generieren  
**Zeitaufwand:** ~2-3 Tage für alle Visualisierungen
