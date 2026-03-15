"""
MQG-Theorie Package
===================

Messbares Informations-Kohärenz-Gesetz (Measurable Information Coherence Law)

Main modules:
- core: ICQ calculation algorithms
- measurement: Data measurement and collection
- simulation: Validation and simulation
- visualization: Data visualization tools

Version: 0.1.0-alpha
"""

__version__ = '0.1.0-alpha'
__author__ = 'MQG Project - Autonomous Development System'

# Import main classes for easy access
try:
    from .core.icq_calculator import ICQCalculator, quick_icq
    from .measurement.measurement_system import MeasurementSystem
    from .simulation.simulation_engine import SimulationEngine
    from .visualization.icq_visualizer import ICQVisualizer
    
    __all__ = [
        'ICQCalculator',
        'quick_icq',
        'MeasurementSystem',
        'SimulationEngine',
        'ICQVisualizer'
    ]
except ImportError:
    # Modules not yet installed or dependencies missing
    pass
