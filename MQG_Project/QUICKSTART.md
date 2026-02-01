# MQG-Theorie: Quick Start Guide

## Installation

1. **Clone or navigate to the project:**
   ```bash
   cd MQG_Project
   ```

2. **Install dependencies:**
   ```bash
   pip install -r requirements.txt
   ```

## Running the Demo

The fastest way to see MQG-Theorie in action:

```bash
python demo.py
```

This will run through all major components:
- Basic ICQ calculations
- Measurement system
- Simulation and validation
- Visualizations
- Complete workflow example

## Quick Examples

### Example 1: Calculate ICQ for simple data

```python
from src.core.icq_calculator import quick_icq

# Your data (can be any sequence)
data = [1, 1, 1, 2, 2, 3, 3, 3]

# Calculate ICQ
icq = quick_icq(data)
print(f"ICQ: {icq:.4f}")
```

### Example 2: Detailed ICQ analysis

```python
from src.core.icq_calculator import ICQCalculator

calc = ICQCalculator()
data = [1, 2, 1, 2, 1, 2]

icq, diagnostics = calc.calculate_icq(data)

print(f"ICQ: {icq:.4f}")
print(f"Entropy: {diagnostics['s_actual']:.4f}")
print(f"Max Entropy: {diagnostics['s_max']:.4f}")
print(f"Coherence Factor: {diagnostics['c_factor']:.4f}")
```

### Example 3: Run simulations

```python
from src.simulation.simulation_engine import SimulationEngine

sim = SimulationEngine(seed=42)

# Run complete validation suite
results = sim.run_validation_suite()

print(f"Success rate: {results['summary']['success_rate']*100:.1f}%")
```

### Example 4: Measure and visualize

```python
from src.measurement.measurement_system import MeasurementSystem
from src.visualization.icq_visualizer import ICQVisualizer

# Measure
ms = MeasurementSystem()
result = ms.measure_discrete_sequence([1,1,2,2,3,3], label="test")

# Visualize
viz = ICQVisualizer()
viz.plot_diagnostic_dashboard(result['diagnostics'])
```

## Running Tests

Each module has built-in self-tests:

```bash
# Test ICQ calculator
python src/core/icq_calculator.py

# Test measurement system
python src/measurement/measurement_system.py

# Test simulation engine
python src/simulation/simulation_engine.py

# Test visualization
python src/visualization/icq_visualizer.py
```

## Automation System

Run the automation engine to:
- Analyze project state
- Generate new tasks
- Update logs
- Get optimization suggestions

```bash
# Single iteration
python automation.py

# Multiple iterations
python automation.py --iterations 3

# With optimization suggestions
python automation.py --optimize
```

## Project Structure

```
MQG_Project/
├── README.md              # Main documentation
├── QUICKSTART.md          # This file
├── log.md                 # Change log
├── tasks.md               # Task list
├── requirements.txt       # Dependencies
├── demo.py               # Comprehensive demo
├── automation.py         # Automation system
└── src/                  # Source code
    ├── core/             # ICQ calculator
    ├── measurement/      # Measurement system
    ├── simulation/       # Simulation engine
    └── visualization/    # Visualization tools
```

## Next Steps

1. **Explore the theory**: Read `README.md` for theoretical background
2. **Run demos**: Execute `demo.py` to see all features
3. **Customize**: Adapt the code for your specific use case
4. **Contribute**: Check `tasks.md` for open tasks
5. **Iterate**: Use `automation.py` for continuous improvement

## Getting Help

- Read the comprehensive docstrings in each module
- Run module self-tests for usage examples
- Check `log.md` for development history
- Review `tasks.md` for current priorities

## Key Concepts

- **ICQ**: Information Coherence Quotient (0 to 1+)
  - 0 = Maximum disorder
  - 1 = Perfect coherence
  
- **Components**:
  - S_actual: Measured entropy
  - S_max: Maximum possible entropy
  - C_factor: Coherence correction factor

- **Applications**:
  - AI/ML systems analysis
  - Data quality assessment
  - Signal processing
  - Complex systems analysis

---

**Version**: 0.1.0-alpha  
**Last Updated**: 2026-02-01  
**License**: Research/Educational Use
