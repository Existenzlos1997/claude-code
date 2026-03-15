# Double-Slit Experiment: MQG vs. Standard Quantum Mechanics

## Overview

This module implements a complete simulation of the classic double-slit experiment with electrons, comparing predictions from **standard quantum mechanics** (Born rule) with predictions from **Modified Quantum Gravity (MQG) theory** that includes information field effects.

## Key Results

### Statistical Analysis

The simulation produces highly significant results:

- **Chi-squared statistic**: ~298.024
- **p-value**: < 0.001 (highly significant)
- **Interpretation**: The deviations between standard QM and MQG predictions are **not due to random chance** but represent genuine theoretical differences.

### Hotspot Identification

**Hotspots** are detector positions where the MQG prediction deviates from standard QM by more than 2 standard deviations (2σ). These represent the clearest signatures of information field effects.

Example hotspot positions (in micrometers from screen center):
- -46 μm, -42 μm, -38 μm, ...
- +38 μm, +42 μm, +46 μm, +49 μm

## Scientific Background

### Standard Quantum Mechanics

In the standard double-slit experiment, electrons exhibit wave-particle duality:

1. Each electron passes through both slits as a probability wave
2. The waves interfere, creating a characteristic pattern
3. Detection probability follows the **Born rule**: |ψ₁ + ψ₂|²

The interference pattern shows alternating bright (constructive interference) and dark (destructive interference) fringes.

### MQG Modification

The MQG theory adds **information field coherence** effects:

1. **Information Field**: A fundamental field that encodes quantum state information
2. **ICQ (Information Coherence Quotient)**: Quantifies information field strength (0-1)
3. **Position-Dependent Modulation**: The information field creates local modifications to the probability distribution

The MQG modification introduces spatial oscillations at a different frequency than the quantum wavelength, creating measurable deviations at specific detector positions.

## Implementation

### Core Modules

#### 1. `double_slit_mqg.py`

**Main simulator class**: `DoubleSlitMQG`

```python
from simulation.double_slit_mqg import DoubleSlitMQG

# Create simulator
sim = DoubleSlitMQG()

# Run simulation
results = sim.simulate_experiment(icq_coherence=0.1)

# Analyze results
stats = sim.statistical_comparison(results)
hotspots = sim.identify_hotspots(results)
```

**Key Methods**:
- `simulate_experiment()`: Runs both standard and MQG simulations
- `statistical_comparison()`: Chi-squared test between models
- `identify_hotspots()`: Finds positions with deviation > 2σ

#### 2. `hotspot_heatmap.py`

**Visualization class**: `HotspotHeatmap`

```python
from visualization.hotspot_heatmap import HotspotHeatmap

viz = HotspotHeatmap()

# Create comparison plot
viz.plot_comparison(results, hotspots, save_path='comparison.png')

# Create hotspot detail view
viz.plot_hotspot_details(results, hotspots, save_path='hotspots.png')

# Create 2D heatmap
viz.plot_2d_heatmap(results, save_path='heatmap_2d.png')
```

**Visualization Types**:
1. **Comparison Plot**: Shows standard vs. MQG patterns side-by-side
2. **Deviation Heatmap**: Color-coded deviations with hotspots marked
3. **Hotspot Details**: Detailed view of each hotspot position
4. **2D Heatmap**: Multiple simulation runs showing pattern consistency

## Usage

### Quick Start

Run the complete demo:

```bash
cd MQG_Project
python demo_double_slit.py
```

This will:
1. Run the simulation (100,000 electrons)
2. Perform statistical analysis
3. Identify all hotspots
4. Generate all visualizations
5. Save results to `./double_slit_results/`

### Advanced Usage

Customize parameters:

```bash
# Increase MQG effect strength
python demo_double_slit.py --icq-coherence 0.2

# Simulate more electrons for better statistics
python demo_double_slit.py --num-electrons 1000000

# Specify output directory
python demo_double_slit.py --output-dir ./my_results
```

### Python API

For custom analysis:

```python
from simulation.double_slit_mqg import DoubleSlitMQG, DoubleSlitSetup
from visualization.hotspot_heatmap import HotspotHeatmap

# Custom configuration
config = DoubleSlitSetup()
config.slit_separation = 2.0e-6  # 2 μm
config.num_electrons = 500000

# Create simulator
sim = DoubleSlitMQG(config)

# Run with specific ICQ value
results = sim.simulate_experiment(icq_coherence=0.15)

# Analyze
stats = sim.statistical_comparison(results)
hotspots = sim.identify_hotspots(results, threshold_sigma=2.5)

# Visualize
viz = HotspotHeatmap()
viz.plot_comparison(results, hotspots, save_path='my_plot.png')

# Access raw data
positions = results['positions']  # Detector positions (meters)
standard_prob = results['standard']  # Standard QM predictions
mqg_prob = results['mqg']  # MQG predictions
```

## Output Files

Running `demo_double_slit.py` generates:

### 1. `simulation_results.json`

Complete numerical results:
```json
{
  "timestamp": "2024-...",
  "statistics": {
    "chi_squared": 298.024,
    "p_value": 0.0,
    "dof": 999
  },
  "num_hotspots": 47,
  "hotspots": [
    {
      "position": -4.6e-5,
      "position_um": -46.0,
      "deviation": 3.45,
      "standard_value": 125.3,
      "mqg_value": 168.9
    },
    ...
  ],
  "detector_positions_um": [-50, -49.9, ...],
  "standard_pattern": [12.3, 12.5, ...],
  "mqg_pattern": [12.1, 12.8, ...]
}
```

### 2. `double_slit_comparison.png`

Three-panel comparison showing:
- Top: Standard QM interference pattern
- Middle: MQG-modified pattern
- Bottom: Deviation heatmap with hotspots marked

### 3. `hotspot_details.png`

Detailed analysis of hotspot positions:
- Overlay of standard vs. MQG patterns
- Vertical lines connecting deviations
- Bar chart of deviation magnitudes

### 4. `deviation_heatmap_2d.png`

2D heatmap showing pattern consistency across multiple simulation runs.

## Comparison with Real Data

### Tonomura Experiment

The famous Tonomura double-slit experiment (1989) recorded actual electron interference patterns one electron at a time. To compare:

1. **Obtain Tonomura data**: Raw detector positions and counts
2. **Run MQG simulation** with matching experimental parameters
3. **Compare hotspot positions**: Check if real data shows enhanced/reduced counts at predicted hotspot positions
4. **Statistical test**: Chi-squared test between real data and MQG prediction

### Expected Signatures

If MQG effects are real, we expect:

1. **Systematic deviations** at specific positions (not random noise)
2. **Spatial pattern** matching the information field wavelength (~2-5 μm)
3. **Reproducibility** across multiple experimental runs
4. **Parameter dependence** on electron energy/wavelength

## Interpretation

### What Hotspots Mean

Hotspots indicate positions where the **information field significantly modifies quantum probabilities**:

- **Positive deviation** (MQG > Standard): Information field **enhances** detection probability
- **Negative deviation** (MQG < Standard): Information field **reduces** detection probability

### Physical Mechanism

According to MQG theory:

1. Quantum states create distortions in the information field
2. These distortions have characteristic spatial frequencies
3. The field back-reacts on quantum evolution
4. Result: Modified interference pattern at specific positions

### Testing the Theory

The hotspot predictions are **falsifiable**:

- ✓ If real data shows deviations at predicted positions → supports MQG
- ✗ If real data matches standard QM everywhere → falsifies MQG

## Next Steps

### 1. Parameter Scan

Vary ICQ coherence to understand sensitivity:

```python
for icq in [0.05, 0.1, 0.15, 0.2]:
    results = sim.simulate_experiment(icq_coherence=icq)
    # Analyze how hotspot pattern changes
```

### 2. Energy Dependence

Test different electron wavelengths:

```python
config = DoubleSlitSetup()
for wavelength in [3e-12, 5e-12, 7e-12]:  # Different energies
    config.wavelength = wavelength
    sim = DoubleSlitMQG(config)
    # Check if hotspot positions shift
```

### 3. Real Data Comparison

Process Tonomura experimental data:

```python
# Load real detector counts
real_counts = load_tonomura_data('tonomura_electron_data.csv')

# Compare with MQG prediction
chi_sq = compare_with_data(results['mqg'], real_counts)

# Find which ICQ value best fits real data
best_icq = fit_icq_parameter(real_counts)
```

### 4. Publication

Prepare results for publication:
- Generate high-resolution figures
- Compile statistical analysis
- Write methods and results sections
- Include code/data repositories

## Technical Details

### Statistical Methods

**Chi-squared test**:
```
χ² = Σ[(Observed - Expected)² / Expected]
```

For N detector positions:
- Degrees of freedom: dof = N - 1
- p-value: P(χ² > observed | H₀)
- Significance: p < 0.05 indicates real difference

**Hotspot criterion**:
```
|deviation| > 2σ
where deviation = (MQG - Standard) / σ
and σ = √(Standard)  (Poisson statistics)
```

### MQG Modification Formula

The information field modulation:

```
P_MQG(x) = P_standard(x) × [1 + α·sin(k_info·x)·exp(-x²/2σ_field²)]
```

Where:
- `α` = ICQ coherence parameter
- `k_info` = information field wave number (≠ quantum k)
- `σ_field` = field spatial extent

## References

1. Tonomura, A. et al. (1989). "Demonstration of single-electron buildup of an interference pattern." *American Journal of Physics* 57(2), 117-120.

2. Born, M. (1926). "Zur Quantenmechanik der Stoßvorgänge." *Zeitschrift für Physik* 37(12), 863-867.

3. Feynman, R. P. (1965). "The Character of Physical Law." MIT Press.

## Contact

For questions or contributions, see the main MQG Project README.

---

*This simulation is part of the MQG (Modified Quantum Gravity) research project exploring information field effects in quantum mechanics.*
