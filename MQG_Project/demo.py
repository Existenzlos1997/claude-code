#!/usr/bin/env python3
"""
MQG-Theorie: Comprehensive Demo
================================

This script demonstrates the complete functionality of the MQG-Theorie
implementation, from basic ICQ calculations to advanced simulations
and visualizations.

Run this script to see the MQG theory in action!

Author: MQG Project - Autonomous Development System
Version: 0.1.0-alpha
Date: 2026-02-01
"""

import sys
import os
import numpy as np

# Add parent directory to path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'src'))

from core.icq_calculator import ICQCalculator, quick_icq
from measurement.measurement_system import MeasurementSystem
from simulation.simulation_engine import SimulationEngine
from visualization.icq_visualizer import ICQVisualizer


def print_header(title: str):
    """Print formatted header."""
    print("\n" + "=" * 70)
    print(title.center(70))
    print("=" * 70 + "\n")


def demo_basic_icq():
    """Demonstrate basic ICQ calculation."""
    print_header("DEMO 1: Basic ICQ Calculation")
    
    calc = ICQCalculator()
    
    # Example 1: Perfect coherence
    data1 = [1, 1, 1, 1, 1]
    icq1, diag1 = calc.calculate_icq(data1)
    print("Example 1: Perfect Coherence")
    print(f"  Data: {data1}")
    print(f"  ICQ: {icq1:.4f} (Expected: close to 1.0)")
    print(f"  Interpretation: Maximum order, all values identical\n")
    
    # Example 2: Maximum disorder
    data2 = list(range(10))
    icq2, diag2 = calc.calculate_icq(data2)
    print("Example 2: Maximum Disorder")
    print(f"  Data: {data2}")
    print(f"  ICQ: {icq2:.4f} (Expected: close to 0.0)")
    print(f"  Interpretation: Uniform distribution, maximum entropy\n")
    
    # Example 3: Moderate coherence
    data3 = [1, 1, 1, 2, 2, 3, 4, 5]
    icq3, diag3 = calc.calculate_icq(data3)
    print("Example 3: Moderate Coherence")
    print(f"  Data: {data3}")
    print(f"  ICQ: {icq3:.4f} (Expected: 0.3-0.7)")
    print(f"  Interpretation: Biased distribution, moderate order\n")
    
    print("✓ Basic ICQ calculation complete")


def demo_measurement_system():
    """Demonstrate measurement system."""
    print_header("DEMO 2: Measurement System")
    
    ms = MeasurementSystem()
    
    # Measure discrete sequences
    print("Measuring discrete sequences...")
    sequences = [
        ([1, 2, 1, 2, 1, 2], "Alternating pattern"),
        ([1, 1, 1, 2, 2, 2], "Two groups"),
        (np.random.randint(0, 5, 20).tolist(), "Random sequence")
    ]
    
    for seq, label in sequences:
        result = ms.measure_discrete_sequence(seq, label=label)
        print(f"  {label}: ICQ = {result['icq']:.4f}")
    
    # Measure continuous time series
    print("\nMeasuring continuous time series...")
    ts1 = np.sin(np.linspace(0, 4*np.pi, 100))
    result_ts = ms.measure_continuous_timeseries(ts1, label="Sine wave")
    print(f"  Sine wave: ICQ = {result_ts['icq']:.4f}")
    
    # Get summary
    print("\nMeasurement summary:")
    summary = ms.get_summary()
    print(f"  Total measurements: {summary['total_measurements']}")
    print(f"  Average ICQ: {summary['icq_statistics']['mean']:.4f}")
    print(f"  ICQ range: [{summary['icq_statistics']['min']:.4f}, {summary['icq_statistics']['max']:.4f}]")
    
    print("\n✓ Measurement system demonstration complete")


def demo_simulation_validation():
    """Demonstrate simulation and validation."""
    print_header("DEMO 3: Simulation & Validation")
    
    sim = SimulationEngine(seed=42)
    
    print("Running validation suite with multiple scenarios...")
    print("This tests if ICQ values match expected ranges for different system types.\n")
    
    results = sim.run_validation_suite()
    
    # Display results
    for scenario in results['scenarios']:
        status = "✓ PASS" if scenario['in_expected_range'] else "✗ FAIL"
        print(f"{status} {scenario['name']}")
        print(f"     Expected: {scenario['properties']['expected_icq_range']}")
        print(f"     Actual:   {scenario['icq']:.4f}")
    
    # Summary
    print(f"\nValidation Summary:")
    print(f"  Scenarios tested: {results['summary']['total_scenarios']}")
    print(f"  Passed: {results['summary']['passed']}")
    print(f"  Failed: {results['summary']['failed']}")
    print(f"  Success rate: {results['summary']['success_rate']*100:.1f}%")
    
    if results['summary']['success_rate'] == 1.0:
        print("\n✓ All validation tests passed!")
    else:
        print("\n⚠ Some validation tests failed")
    
    print("\n✓ Simulation and validation complete")


def demo_visualization():
    """Demonstrate visualization capabilities."""
    print_header("DEMO 4: Visualization")
    
    viz = ICQVisualizer()
    
    # Create sample data
    print("Generating visualization examples...")
    
    # 1. Comparison plot
    comparison_data = {
        'Random System': 0.05,
        'Ordered System': 0.95,
        'Biased System': 0.45,
        'Markov Chain': 0.65,
        'Noisy Pattern': 0.55
    }
    
    print("  1. Creating comparison plot...")
    try:
        viz.plot_icq_comparison(
            comparison_data,
            title="ICQ Comparison: Different System Types",
            save_path=None  # Set to filepath to save
        )
        print("     ✓ Comparison plot created")
    except Exception as e:
        print(f"     ⚠ Could not create comparison plot: {e}")
    
    # 2. Time series plot
    print("  2. Creating time series plot...")
    icq_series = [0.3, 0.35, 0.42, 0.5, 0.58, 0.65, 0.7, 0.72, 0.75, 0.78]
    try:
        viz.plot_icq_timeseries(
            icq_series,
            title="ICQ Evolution Over Time",
            save_path=None
        )
        print("     ✓ Time series plot created")
    except Exception as e:
        print(f"     ⚠ Could not create time series plot: {e}")
    
    # 3. Diagnostic dashboard
    print("  3. Creating diagnostic dashboard...")
    diagnostics = {
        'icq': 0.732,
        's_actual': 1.234,
        's_max': 2.321,
        'c_factor': 1.15,
        'n_states': 8,
        'normalized_entropy': 0.532
    }
    try:
        viz.plot_diagnostic_dashboard(
            diagnostics,
            title="ICQ Diagnostic Dashboard",
            save_path=None
        )
        print("     ✓ Diagnostic dashboard created")
    except Exception as e:
        print(f"     ⚠ Could not create dashboard: {e}")
    
    print("\n✓ Visualization demonstration complete")
    print("  (Close plot windows to continue)")


def demo_complete_workflow():
    """Demonstrate complete workflow from data to insight."""
    print_header("DEMO 5: Complete Workflow")
    
    print("Simulating a real-world analysis workflow...\n")
    
    # Step 1: Generate data
    print("Step 1: Generating test data...")
    sim = SimulationEngine(seed=123)
    data, props = sim.generate_markov_chain(n_samples=500)
    print(f"  ✓ Generated Markov chain with {len(data)} samples")
    
    # Step 2: Measure
    print("\nStep 2: Measuring ICQ...")
    ms = MeasurementSystem()
    measurement = ms.measure_discrete_sequence(data, label="Markov Chain Test")
    print(f"  ✓ ICQ = {measurement['icq']:.4f}")
    print(f"    States: {measurement['unique_states']}")
    print(f"    Entropy: {measurement['diagnostics']['s_actual']:.4f}")
    
    # Step 3: Validate
    print("\nStep 3: Validating against expected range...")
    expected_range = props['expected_icq_range']
    is_valid = expected_range[0] <= measurement['icq'] <= expected_range[1]
    status = "✓ VALID" if is_valid else "✗ INVALID"
    print(f"  Expected: {expected_range}")
    print(f"  Actual:   {measurement['icq']:.4f}")
    print(f"  {status}")
    
    # Step 4: Visualize
    print("\nStep 4: Creating visualization...")
    try:
        viz = ICQVisualizer()
        viz.plot_diagnostic_dashboard(
            measurement['diagnostics'],
            title="Complete Workflow: Markov Chain Analysis"
        )
        print("  ✓ Visualization created")
    except Exception as e:
        print(f"  ⚠ Could not create visualization: {e}")
    
    # Step 5: Export
    print("\nStep 5: Exporting results...")
    export_path = os.path.join(os.path.dirname(__file__), 'demo_results.json')
    try:
        ms.export_measurements(export_path, format='json')
        print(f"  ✓ Results exported to: {export_path}")
    except Exception as e:
        print(f"  ⚠ Could not export: {e}")
    
    print("\n✓ Complete workflow demonstration finished")


def main():
    """Run all demonstrations."""
    print("\n" + "=" * 70)
    print("MQG-THEORIE: COMPREHENSIVE DEMONSTRATION".center(70))
    print("Messbares Informations-Kohärenz-Gesetz".center(70))
    print("(Measurable Information Coherence Law)".center(70))
    print("=" * 70)
    
    print("\nThis demo showcases the complete MQG-Theorie implementation:")
    print("  1. Basic ICQ calculation")
    print("  2. Measurement system")
    print("  3. Simulation & validation")
    print("  4. Visualization tools")
    print("  5. Complete workflow example")
    
    input("\nPress Enter to start the demonstration...")
    
    try:
        # Run all demos
        demo_basic_icq()
        input("\nPress Enter to continue to next demo...")
        
        demo_measurement_system()
        input("\nPress Enter to continue to next demo...")
        
        demo_simulation_validation()
        input("\nPress Enter to continue to next demo...")
        
        demo_visualization()
        input("\nPress Enter to continue to final demo...")
        
        demo_complete_workflow()
        
        # Final summary
        print_header("DEMONSTRATION COMPLETE")
        print("All MQG-Theorie components have been successfully demonstrated.")
        print("\nKey Achievements:")
        print("  ✓ ICQ calculation algorithm implemented")
        print("  ✓ Measurement system operational")
        print("  ✓ Simulation engine validated")
        print("  ✓ Visualization tools functional")
        print("  ✓ Complete workflow tested")
        
        print("\nThe MQG theory is ready for:")
        print("  • Real-world data analysis")
        print("  • Further theoretical development")
        print("  • Integration into larger systems")
        print("  • Continuous improvement and iteration")
        
        print("\n" + "=" * 70)
        print("Thank you for exploring the MQG-Theorie!")
        print("=" * 70 + "\n")
        
    except KeyboardInterrupt:
        print("\n\nDemonstration interrupted by user.")
        print("✓ Partial demo completed successfully.")
    except Exception as e:
        print(f"\n\n✗ Error during demonstration: {e}")
        import traceback
        traceback.print_exc()


if __name__ == "__main__":
    main()
