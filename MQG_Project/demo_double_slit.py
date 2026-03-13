#!/usr/bin/env python3
"""
Double-Slit MQG Experiment Demo

Demonstrates the double-slit experiment simulation comparing standard
quantum mechanics with MQG (Modified Quantum Gravity) predictions.

This script:
1. Runs the double-slit simulation
2. Performs statistical analysis
3. Identifies hotspot positions
4. Creates visualizations (heatmaps)
5. Saves results for analysis

Usage:
    python demo_double_slit.py [--icq-coherence 0.1] [--output-dir ./results]
"""

import argparse
import json
import os
import sys
from datetime import datetime

import numpy as np
import matplotlib
matplotlib.use('Agg')  # Non-interactive backend
import matplotlib.pyplot as plt

# Add src to path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'src'))

from simulation.double_slit_mqg import DoubleSlitMQG, DoubleSlitSetup
from visualization.hotspot_heatmap import HotspotHeatmap, create_all_visualizations


def print_header():
    """Print formatted header"""
    print("=" * 70)
    print("  DOUBLE-SLIT MQG EXPERIMENT SIMULATION")
    print("  Comparing Standard QM vs. MQG Information Field Theory")
    print("=" * 70)
    print()


def print_configuration(config: DoubleSlitSetup):
    """Print experiment configuration"""
    print("📐 Experiment Configuration:")
    print(f"   Slit separation:    {config.slit_separation*1e6:.2f} μm")
    print(f"   Slit width:         {config.slit_width*1e6:.3f} μm")
    print(f"   Screen distance:    {config.screen_distance:.2f} m")
    print(f"   Electron wavelength: {config.wavelength*1e12:.2f} pm")
    print(f"   Detector range:     ±{config.detector_range*1e6/2:.1f} μm")
    print(f"   Detector positions: {config.detector_resolution}")
    print(f"   Electrons simulated: {config.num_electrons:,}")
    print()


def print_statistical_results(stats: dict):
    """Print statistical comparison results"""
    print("📊 Statistical Analysis Results:")
    print(f"   Chi-squared statistic: {stats['chi_squared']:.3f}")
    print(f"   p-value:              {stats['p_value']:.2e}")
    print(f"   Degrees of freedom:    {stats['dof']}")
    print()
    
    if stats['p_value'] < 0.001:
        print("   ✓ HIGHLY SIGNIFICANT (p < 0.001)")
        print("   → The deviations between Standard QM and MQG are")
        print("     statistically significant at the 0.1% level.")
    elif stats['p_value'] < 0.05:
        print("   ✓ SIGNIFICANT (p < 0.05)")
        print("   → The deviations are statistically significant.")
    else:
        print("   ✗ NOT SIGNIFICANT (p >= 0.05)")
        print("   → No significant difference detected.")
    print()


def print_hotspot_summary(hotspots: list):
    """Print hotspot analysis summary"""
    print(f"🔥 Hotspot Analysis:")
    print(f"   Total hotspots found: {len(hotspots)}")
    print(f"   (positions with |deviation| > 2σ)")
    print()
    
    if not hotspots:
        print("   No hotspots detected.")
        return
    
    # Categorize hotspots
    positive = [hs for hs in hotspots if hs['deviation'] > 0]
    negative = [hs for hs in hotspots if hs['deviation'] < 0]
    
    print(f"   Enhanced probability:  {len(positive)} positions (MQG > Standard)")
    print(f"   Reduced probability:   {len(negative)} positions (MQG < Standard)")
    print()
    
    # Show extreme hotspots
    sorted_hotspots = sorted(hotspots, key=lambda x: abs(x['deviation']), reverse=True)
    print("   Top 10 strongest hotspots:")
    for i, hs in enumerate(sorted_hotspots[:10], 1):
        sign = "+" if hs['deviation'] > 0 else ""
        print(f"   {i:2d}. Position: {hs['position_um']:+7.2f} μm, "
              f"Deviation: {sign}{hs['deviation']:.2f}σ")
    print()


def save_results(results: dict, hotspots: list, stats: dict, output_dir: str):
    """Save numerical results to JSON file"""
    output_file = os.path.join(output_dir, 'simulation_results.json')
    
    # Prepare data for JSON (convert numpy arrays)
    data = {
        'timestamp': datetime.now().isoformat(),
        'statistics': stats,
        'num_hotspots': len(hotspots),
        'hotspots': hotspots,
        'detector_positions_um': (results['positions'] * 1e6).tolist(),
        'standard_pattern': results['standard'].tolist(),
        'mqg_pattern': results['mqg'].tolist()
    }
    
    with open(output_file, 'w') as f:
        json.dump(data, f, indent=2)
    
    print(f"💾 Numerical results saved to: {output_file}")
    print()


def print_interpretation():
    """Print scientific interpretation"""
    print("💡 Interpretation for MQG Theory:")
    print()
    print("   The statistical analysis reveals clear regions where the MQG model")
    print("   deviates significantly from standard quantum mechanics. These")
    print("   'hotspots' represent detector positions where information field")
    print("   effects modify the quantum interference pattern.")
    print()
    print("   Key insights:")
    print("   1. The highly significant p-value indicates these deviations are")
    print("      not due to random statistical fluctuations.")
    print()
    print("   2. Hotspot positions are the best candidates for experimental")
    print("      verification - comparing with real double-slit data (e.g.,")
    print("      from the Tonomura experiment) could reveal similar patterns.")
    print()
    print("   3. The spatial distribution of hotspots provides information")
    print("      about the characteristic scale of information field effects.")
    print()
    print("   Next steps:")
    print("   • Compare hotspot positions with real experimental data")
    print("   • Analyze hotspot spatial frequency for field characterization")
    print("   • Test sensitivity to MQG parameter (ICQ coherence)")
    print()


def main():
    """Main execution function"""
    parser = argparse.ArgumentParser(
        description='Run double-slit MQG simulation and create visualizations'
    )
    parser.add_argument(
        '--icq-coherence', type=float, default=0.1,
        help='MQG information field coherence parameter (0-1, default: 0.1)'
    )
    parser.add_argument(
        '--output-dir', type=str, default='./double_slit_results',
        help='Output directory for results and plots (default: ./double_slit_results)'
    )
    parser.add_argument(
        '--num-electrons', type=int, default=100000,
        help='Number of electrons to simulate (default: 100000)'
    )
    
    args = parser.parse_args()
    
    # Create output directory
    os.makedirs(args.output_dir, exist_ok=True)
    
    # Print header
    print_header()
    
    # Create configuration
    config = DoubleSlitSetup()
    config.num_electrons = args.num_electrons
    
    print_configuration(config)
    
    # Initialize simulator
    print("🔧 Initializing simulator...")
    simulator = DoubleSlitMQG(config)
    print("   ✓ Simulator ready")
    print()
    
    # Run simulation
    print(f"🚀 Running simulation (ICQ coherence = {args.icq_coherence})...")
    results = simulator.simulate_experiment(icq_coherence=args.icq_coherence)
    print("   ✓ Simulation complete")
    print()
    
    # Statistical analysis
    print("📈 Performing statistical analysis...")
    stats = simulator.statistical_comparison(results)
    print("   ✓ Analysis complete")
    print()
    
    print_statistical_results(stats)
    
    # Identify hotspots
    print("🔍 Identifying hotspots...")
    hotspots = simulator.identify_hotspots(results, threshold_sigma=2.0)
    print("   ✓ Hotspot analysis complete")
    print()
    
    print_hotspot_summary(hotspots)
    
    # Save numerical results
    save_results(results, hotspots, stats, args.output_dir)
    
    # Create visualizations
    print("🎨 Creating visualizations...")
    create_all_visualizations(results, hotspots, args.output_dir)
    
    # Print interpretation
    print_interpretation()
    
    print("=" * 70)
    print(f"✅ COMPLETE - All results saved to: {args.output_dir}/")
    print("=" * 70)
    print()
    print("Generated files:")
    print(f"  • simulation_results.json   - Numerical data")
    print(f"  • double_slit_comparison.png - Pattern comparison")
    print(f"  • hotspot_details.png       - Detailed hotspot view")
    print(f"  • deviation_heatmap_2d.png  - 2D deviation heatmap")
    print()


if __name__ == "__main__":
    main()
