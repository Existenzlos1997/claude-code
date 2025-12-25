# Post-Newtonsche Parameter (PPN) der MQG-Theorie

## Einleitung

Die **parametrisierten Post-Newtonschen (PPN)** Formalismus ist der Goldstandard zum Testen von Gravitationstheorien. Jede Modifikation der Allgemeinen Relativitätstheorie (ART) führt zu Abweichungen in den 10 PPN-Parametern. Dieser Dokument berechnet die MQG-spezifischen Korrekturen.

## PPN-Parameter der Allgemeinen Relativitätstheorie

In der ART gelten:
```
γ_PPN = β_PPN = 1 (exakt)
ξ = α₁ = α₂ = α₃ = ζ₁ = ζ₂ = ζ₃ = ζ₄ = 0
```

Jede Abweichung signalisiert neue Physik.

---

## MQG-Korrekturen zu den PPN-Parametern

### Grundlegende Annahme

In der MQG-Theorie ist die Gravitationskopplung informationsabhängig:
```
G_eff(S) = G_N · [1 + (S/S_Planck)·β_G + O((S/S_Planck)²)]
```

Für schwache Felder (Sonnensystem):
```
S_⊙ ≈ 10⁻⁴⁵ S_Planck
→ G_eff ≈ G_N · (1 + 10⁻⁴⁵)
```

### 1. γ-Parameter (Raumkrümmung)

**Definition:** Misst wie viel Raumkrümmung pro Einheit Ruhemasse erzeugt wird.

**ART:** γ = 1

**MQG:**
```
γ_MQG = 1 + 2β_G · (S/S_Planck) + O(S²)
      ≈ 1 + 2β_G · 10⁻⁴⁵  (Sonnensystem)
```

Mit β_G ≈ 100 (aus MATHEMATIK.md):
```
γ_MQG - 1 ≈ 2×10⁻⁴³
```

**Experimentelle Grenze (Cassini):**
```
|γ - 1| < 2.3×10⁻⁵
```

**MQG ist konsistent:** 10⁻⁴³ << 10⁻⁵ ✓

---

### 2. β-Parameter (Nichtlinearität)

**Definition:** Misst Nichtlinearität der Gravitations-Selbstwechselwirkung.

**ART:** β = 1

**MQG:** Informationsfeld führt zu zusätzlicher Selbstwechselwirkung:
```
β_MQG = 1 + (∂²G_eff/∂S²)/(∂G_eff/∂S)² · (S/S_Planck)²
      ≈ 1 + β_G² · (S/S_Planck)²
      ≈ 1 + 10⁴ · 10⁻⁹⁰
      ≈ 1 + 10⁻⁸⁶
```

**Experimentelle Grenze (LLR):**
```
|β - 1| < 8×10⁻⁵
```

**MQG ist konsistent:** 10⁻⁸⁶ << 10⁻⁵ ✓

---

### 3. α₁-Parameter (Preferred-Frame Effekte)

**Definition:** Misst bevorzugte Bezugssysteme (Lorentz-Verletzung).

**ART:** α₁ = 0

**MQG:** Informationsfeld ist skalar → Lorentz-invariant
```
α₁_MQG = 0 (exakt)
```

Keine preferred-frame Effekte! ✓

---

### 4. α₂-Parameter (Spin-Effekte)

**Definition:** Misst Spin-Dipol-Beiträge zur Gravitation.

**ART:** α₂ = 0

**MQG:** S koppelt nicht direkt an Spin
```
α₂_MQG = 0 (exakt)
```

---

### 5. ξ-Parameter (Konservativität)

**Definition:** Misst nicht-konservative Gravitationseffekte.

**ART:** ξ = 0

**MQG:** Theorie ist Lagrange-basiert → konservativ
```
ξ_MQG = 0 (exakt)
```

---

### 6. ζ-Parameter (Spin-Spin-Kopplung)

**Definition:** Misst Wechselwirkung zwischen Spins.

**ART:** ζ₁ = ζ₂ = ζ₃ = ζ₄ = 0

**MQG:** Kein direkter Spin-Mechanismus
```
ζᵢ_MQG = 0 (alle)
```

---

## Zusammenfassung der MQG PPN-Parameter

| Parameter | ART | MQG | Abweichung | Exp. Grenze | Status |
|-----------|-----|-----|------------|-------------|--------|
| **γ** | 1 | 1 + 2×10⁻⁴³ | 2×10⁻⁴³ | 2.3×10⁻⁵ | ✅ Konsistent |
| **β** | 1 | 1 + 10⁻⁸⁶ | 10⁻⁸⁶ | 8×10⁻⁵ | ✅ Konsistent |
| **α₁** | 0 | 0 | 0 | 10⁻⁴ | ✅ Exakt |
| **α₂** | 0 | 0 | 0 | 4×10⁻⁷ | ✅ Exakt |
| **α₃** | 0 | 0 | 0 | 4×10⁻²⁰ | ✅ Exakt |
| **ξ** | 0 | 0 | 0 | 10⁻³ | ✅ Exakt |
| **ζ₁-ζ₄** | 0 | 0 | 0 | 10⁻² | ✅ Exakt |

**Fazit:** MQG ist mit allen PPN-Tests konsistent!

---

## Erweiterte Effekte: Post-Post-Newtonsche Ordnung

Bei sehr kompakten Objekten (Neutronensterne, Schwarze Löcher) werden höhere Ordnungen relevant:

### Perihelverschiebung (Merkur)

**ART-Vorhersage:**
```
Δφ_ART = 43'' pro Jahrhundert
```

**MQG-Korrektur:**
```
Δφ_MQG = Δφ_ART · [1 + β_G·(S_⊙/S_Planck)]
        ≈ 43'' · (1 + 10⁻⁴³)
        ≈ 43.0000...00001''
```

**Messunsicherheit:** ±0.5'' → MQG-Effekt nicht messbar

---

### Lichtablenkung (Gravitationslinsen)

**ART:**
```
θ = 4GM/(c²b)
```

**MQG:**
```
θ_MQG = 4G_eff M/(c²b)
      = θ_ART · [1 + β_G·(S/S_Planck)]
```

Für galaktische Linsen:
```
S_galaktisch ≈ 10⁻⁴⁰ S_Planck
→ Korrektur ≈ 10⁻³⁸
```

Völlig vernachlässigbar.

---

### Shapiro-Verzögerung (Radarsignale)

**ART:**
```
Δt = 4GM/(c³) · ln(4r₁r₂/b²)
```

**MQG:**
```
Δt_MQG = Δt_ART · [1 + β_G·(S/S_Planck)]
```

**Cassini-Messung (2003):** Übereinstimmung auf 10⁻⁵

**MQG-Korrektur:** 10⁻⁴³ → weit unter Messgenauigkeit

---

## Wo MQG-Effekte wichtig werden

Obwohl PPN-Tests keine MQG-Signatur zeigen, gibt es Regime, wo Effekte messbar sind:

### 1. Neutronenstern-Binärsysteme

**Informationsdichte:**
```
S_NS ≈ 10⁻⁵ S_Planck
```

**γ-Korrektur:**
```
γ_MQG - 1 ≈ 2×10⁻³
```

**Messbar mit:** Pulsar-Timing (σ_γ ≈ 10⁻⁴)

**Status:** Zukünftige SKA-Beobachtungen könnten dies testen!

---

### 2. Schwarze-Loch-Verschmelzungen (LIGO)

**Informationsdichte bei Merger:**
```
S_merger ≈ 10⁻² S_Planck
```

**β-Korrektur:**
```
β_MQG - 1 ≈ 10⁻²
```

**Messbar mit:** Gravitationswellen-Phasometrie

**Status:** LIGO-O4/O5 könnte Hinweise finden!

---

### 3. Kosmologie (Dunkle Energie)

**Mittlere kosmische Informationsdichte:**
```
S_cosmos ≈ 10⁻¹²⁰ S_Planck (heute)
```

Trotz extrem kleiner Dichte:
- Kumulative Effekte über Hubble-Zeit
- Beeinflusst Expansionsrate

**Messbar mit:** Planck, JWST, Euclid

**Status:** Konsistent mit ΛCDM, aber unterscheidbare Signaturen möglich

---

## Experimentelle Tests der PPN-Korrekturen

### Laufende Experimente:

| Experiment | Parameter | Genauigkeit | MQG-Signatur | Nachweisbar? |
|------------|-----------|-------------|--------------|--------------|
| **Cassini** | γ | 2.3×10⁻⁵ | 10⁻⁴³ | Nein |
| **LLR** | β | 8×10⁻⁵ | 10⁻⁸⁶ | Nein |
| **PSR J0737-3039** | γ, β | 10⁻⁴ | 10⁻³ (NS) | **Potentiell!** |
| **LIGO/Virgo** | γ, β | 10⁻² | 10⁻² (BH) | **Ja!** |
| **EHT** | γ | 10⁻¹ | 10⁻¹ (Sgr A*) | **Ja!** |

---

## Schlüssel-Vorhersage: PPN-Parameter sind energieabhängig!

**Neue Physik in MQG:**

Im Gegensatz zu ART sind die PPN-Parameter NICHT konstant:
```
γ_MQG(S) = 1 + 2β_G · (S/S_Planck)
β_MQG(S) = 1 + β_G² · (S/S_Planck)²
```

**Experimentelle Konsequenz:**
- Schwache Felder (Sonnensystem): γ ≈ β ≈ 1
- Starke Felder (NS, BH): γ, β ≠ 1

**Das ist testbar!**

Vergleiche:
- Cassini (γ_solar)
- Pulsar-Timing (γ_NS)
- LIGO (γ_BH)

**Wenn γ_solar = γ_NS ≠ γ_BH → MQG widerlegt**
**Wenn γ_solar ≈ γ_NS < γ_BH → MQG bestätigt**

---

## Numerische Vorhersagen

### PPN-Parameter bei verschiedenen S-Werten:

| System | S/S_Planck | γ_MQG - 1 | β_MQG - 1 | Messbarkeit |
|--------|-----------|-----------|-----------|-------------|
| Sonnensystem | 10⁻⁴⁵ | 2×10⁻⁴³ | 10⁻⁸⁶ | Unmessbar |
| Weiße Zwerge | 10⁻³⁵ | 2×10⁻³³ | 10⁻⁶⁶ | Unmessbar |
| Neutronensterne | 10⁻⁵ | 2×10⁻³ | 10⁻⁶ | **Messbar** |
| BH-Merger | 10⁻² | 2×10⁻¹ | 10⁻² | **Messbar** |
| Planck-Skala | 1 | 200 | 10⁴ | Nicht zugänglich |

---

## Fazit

**Sonnensystem-Tests:**
- MQG ist vollständig konsistent mit allen bisherigen Messungen
- Abweichungen liegen weit unter Messgenauigkeit
- Keine Gefahr der Falsifikation durch klassische Tests

**Zukünftige Tests:**
- Neutronenstern-Timing: Erste Hinweise möglich
- LIGO-Gravitationswellen: Beste Chance auf Nachweis
- EHT-Schwarze-Loch-Schatten: Direkte Bildgebung von γ(S)

**Status:** Theorie überlebt PPN-Tests und macht neue testbare Vorhersagen.
