# Mathematische Grundlagen der MQG-Theorie

## 1. Basis-Formalismus

### 1.1 Metriktensor mit Quantenkorrekturen

Der Metriktensor in MQG wird als Summe eines klassischen und eines Quantenanteils geschrieben:

```
gμν = g⁰μν + ℏ·g¹μν + ℏ²·g²μν + ...
```

Wobei:
- `g⁰μν` der klassische Metriktensor ist
- `g^n_μν` die n-te Quantenkorrektur darstellt
- `ℏ` die reduzierte Planck-Konstante ist (Entwicklungsparameter)

### 1.2 Modifizierte Wirkung

Die Wirkung der MQG-Theorie hat die Form:

```
S = S_EH + S_Q + S_M
```

Wobei:
- `S_EH = (c⁴/16πG) ∫ R√(-g) d⁴x` die Einstein-Hilbert-Wirkung
- `S_Q` die Quantenkorrekturen enthält
- `S_M` die Materiewirkung ist

## 2. Quantenkorrekturen im Detail

### 2.1 Führende Quantenkorrekturen

Die führenden Korrekturen haben die Form:

```
S_Q = ∫ [α·R² + β·RμνR^μν + γ·RμνρσR^μνρσ] √(-g) d⁴x
```

Mit dimensionslosen Kopplungskonstanten α, β, γ.

### 2.2 Effektive Feldgleichungen

Variation der Wirkung führt zu:

```
Gμν + Λgμν = (8πG/c⁴)[Tμν + T^Q_μν]
```

Wobei `T^Q_μν` der Energie-Impuls-Tensor der Quantenkorrekturen ist:

```
T^Q_μν = (2/√(-g)) δS_Q/δg^μν
```

## 3. Störungstheorie

### 3.1 Entwicklung um flache Raumzeit

Für schwache Felder: `gμν = ημν + hμν` mit `|hμν| << 1`

Die Feldgleichungen werden zu:

```
□hμν - ∂μ∂ρh^ρ_ν - ∂ν∂ρh^ρ_μ + ∂μ∂νh + ℏ·Qμν[h] = -(16πG/c⁴)Tμν
```

### 3.2 Propagator

Der modifizierte Graviton-Propagator in Impulsraum:

```
D̃μνρσ(k) = D⁰μνρσ(k)/[1 + F(k²/M²_Pl)]
```

Wobei:
- `D⁰μνρσ` der klassische Propagator
- `F(k²/M²_Pl)` die Quantenkorrektur-Funktion
- `M_Pl = √(ℏc/G)` die Planck-Masse

## 4. Renormierung

### 4.1 Renormierungsgruppen-Gleichungen

Die laufenden Kopplungen genügen:

```
μ dG(μ)/dμ = β_G(G, α, β, γ)
μ dα(μ)/dμ = β_α(G, α, β, γ)
...
```

### 4.2 Fixpunkte

Nicht-triviale Fixpunkte bei:
- `β_G(G*, α*, β*, γ*) = 0`
- Stabilität untersuchen via Eigenwerte der Stabilitätsmatrix

## 5. Schwarzschild-Lösung mit Quantenkorrekturen

### 5.1 Modifizierte Metrik

```
ds² = -(1 - 2GM/c²r + Q(r))c²dt² + (1 - 2GM/c²r + Q(r))⁻¹dr² + r²dΩ²
```

Wobei `Q(r)` die Quantenkorrektur ist:

```
Q(r) = (ℏG/c³) · [a/r² + b/r³ + ...]
```

### 5.2 Horizont-Struktur

Der Ereignishorizont liegt bei:

```
r_h = 2GM/c² · [1 + ε·(ℏc/GM²) + O(ℏ²)]
```

Mit Korrekturparameter ε.

## 6. Kosmologische Anwendungen

### 6.1 Friedmann-Gleichungen mit Quantenkorrekturen

```
H² = (8πG/3c²)ρ - k/a² + Λ/3 + Q_H(a)
Ḣ = -(4πG/c²)(ρ + 3p) + Q_Ḣ(a)
```

### 6.2 Inflation aus Quanteneffekten

Quantenkorrekturen können inflationäre Phase erzeugen:

```
Q_H ~ (ℏ/M²_Pl·a⁴)
```

## 7. Gravitationswellen

### 7.1 Dispersionsrelation

Modifiziert für hohe Frequenzen:

```
ω² = c²k²[1 - δ·(k/k_Pl)² + ...]
```

### 7.2 Amplitudenmodifikation

```
h(t) = h₀(t)·[1 - ξ·(ω/ω_Pl)² + ...]
```

## 8. Numerische Methoden

### 8.1 Diskretisierung

Raumzeit-Gitter mit Gitterkonstante `a ~ l_Pl`

### 8.2 Simulationsschema

1. Initialisiere Metrik und Materiefelder
2. Berechne Quantenkorrekturen
3. Entwickle Feldgleichungen zeitlich
4. Iteriere bis Konvergenz

## Anhang: Notation

- `c` = Lichtgeschwindigkeit
- `G` = Gravitationskonstante
- `ℏ` = reduzierte Planck-Konstante
- `l_Pl = √(ℏG/c³)` = Planck-Länge
- `M_Pl = √(ℏc/G)` = Planck-Masse
- `t_Pl = l_Pl/c` = Planck-Zeit
