#!/usr/bin/env python3
"""
MQG-Theorie: Extended Experiments - Pattern Recognition & Advanced Validation
==============================================================================

Additional experiments based on initial validation findings.

Focus Areas:
- Pattern recognition in various data types
- Temporal coherence analysis
- Multi-scale coherence
- Comparative analysis with traditional metrics

Author: MQG Project
Version: 1.1.0-extended
Date: 2026-02-04
"""

import sys
import os
import numpy as np
from datetime import datetime
import json

sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'src'))

from core.icq_calculator import ICQCalculator
from simulation.simulation_engine import SimulationEngine


class ExtendedExperiments:
    """Extended experimental validation."""
    
    def __init__(self):
        self.calc = ICQCalculator()
        self.sim = SimulationEngine(seed=2026)
        self.results = []
        
    def print_header(self, title):
        """Print experiment header."""
        print("\n" + "="*80)
        print(f"  {title}")
        print("="*80)
        
    def exp_various_patterns(self):
        """Test ICQ on various pattern types."""
        self.print_header("EXPERIMENT: Pattern Recognition Across Different Types")
        
        patterns = {
            'fibonacci': [1, 1, 2, 3, 5, 8, 13, 21, 34, 55] * 10,
            'powers_of_2': [1, 2, 4, 8, 16, 32, 64, 128] * 12,
            'prime_like': [2, 3, 5, 7, 11, 13, 17, 19, 23] * 11,
            'arithmetic_progression': list(range(0, 100)),
            'geometric_progression': [2**i for i in range(10)] * 10,
            'random_walk': np.cumsum(np.random.choice([-1, 1], 100)).tolist(),
            'sawtooth': ([1, 2, 3, 4, 5] * 20),
            'step_function': [1]*25 + [2]*25 + [3]*25 + [4]*25,
        }
        
        print(f"\n{'Pattern':<25} {'ICQ':<10} {'Entropy':<12} {'C-factor':<10}")
        print("-"*80)
        
        for name, pattern in patterns.items():
            icq, diag = self.calc.calculate_icq(pattern)
            print(f"{name:<25} {icq:>8.4f}   {diag['s_actual']:>8.4f}   {diag['c_factor']:>8.4f}")
            
            self.results.append({
                'experiment': 'pattern_recognition',
                'pattern_type': name,
                'icq': icq,
                'diagnostics': diag
            })
        
        print("\n✓ Pattern recognition experiments completed")
        
    def exp_data_transformation_effects(self):
        """Test how data transformations affect ICQ."""
        self.print_header("EXPERIMENT: Data Transformation Effects")
        
        base_data = np.array([1, 2, 3, 4, 5] * 20)
        
        transformations = {
            'original': base_data,
            'normalized': (base_data - np.mean(base_data)) / np.std(base_data),
            'squared': base_data ** 2,
            'log_transform': np.log(base_data + 1),
            'reversed': base_data[::-1],
            'shuffled': np.random.permutation(base_data),
            'with_noise': base_data + np.random.randn(len(base_data)) * 0.1,
            'rounded': np.round(base_data + np.random.randn(len(base_data)) * 0.3),
        }
        
        print(f"\n{'Transformation':<20} {'ICQ':<10} {'Change from Original':<20}")
        print("-"*80)
        
        original_icq, _ = self.calc.calculate_icq(transformations['original'])
        
        for name, data in transformations.items():
            icq, diag = self.calc.calculate_icq(data)
            change = icq - original_icq if name != 'original' else 0.0
            
            print(f"{name:<20} {icq:>8.4f}   {change:>+8.4f}")
            
            self.results.append({
                'experiment': 'transformation_effects',
                'transformation': name,
                'icq': icq,
                'change': change
            })
        
        print("\n✓ Transformation effects analyzed")
        
    def exp_size_sensitivity(self):
        """Test ICQ sensitivity to data size."""
        self.print_header("EXPERIMENT: Sample Size Sensitivity")
        
        # Create a simple pattern
        pattern = [1, 2, 3, 4, 5]
        
        sizes = [10, 50, 100, 500, 1000, 5000, 10000]
        
        print(f"\n{'Sample Size':<15} {'ICQ':<10} {'Std from Mean':<15}")
        print("-"*80)
        
        icq_values = []
        for size in sizes:
            repetitions = size // len(pattern)
            data = pattern * repetitions
            data = data[:size]  # Ensure exact size
            
            icq, _ = self.calc.calculate_icq(data)
            icq_values.append(icq)
            
        mean_icq = np.mean(icq_values)
        
        for size, icq in zip(sizes, icq_values):
            std_from_mean = icq - mean_icq
            print(f"{size:<15} {icq:>8.4f}   {std_from_mean:>+8.4f}")
            
            self.results.append({
                'experiment': 'size_sensitivity',
                'sample_size': size,
                'icq': icq,
                'deviation_from_mean': std_from_mean
            })
        
        print(f"\nMean ICQ: {mean_icq:.4f}")
        print(f"Std Dev: {np.std(icq_values):.6f}")
        print("\n✓ Size sensitivity analyzed")
        
    def exp_noise_resilience(self):
        """Test ICQ resilience to various noise levels."""
        self.print_header("EXPERIMENT: Noise Resilience")
        
        # Clean signal
        t = np.linspace(0, 10, 200)
        clean_signal = np.array([1, 2, 3, 4, 5] * 40)
        
        noise_levels = [0.0, 0.1, 0.5, 1.0, 2.0, 5.0, 10.0]
        
        print(f"\n{'Noise σ':<12} {'ICQ':<10} {'SNR (approx)':<15}")
        print("-"*80)
        
        for noise_std in noise_levels:
            noisy_signal = clean_signal + np.random.randn(len(clean_signal)) * noise_std
            icq, _ = self.calc.calculate_icq(noisy_signal)
            
            signal_power = np.var(clean_signal)
            noise_power = noise_std ** 2
            snr = 10 * np.log10(signal_power / noise_power) if noise_std > 0 else float('inf')
            
            print(f"{noise_std:<12.1f} {icq:>8.4f}   {snr:>8.2f} dB")
            
            self.results.append({
                'experiment': 'noise_resilience',
                'noise_std': noise_std,
                'icq': icq,
                'snr_db': snr
            })
        
        print("\n✓ Noise resilience tested")
        
    def exp_temporal_coherence(self):
        """Test temporal coherence in sequences."""
        self.print_header("EXPERIMENT: Temporal Coherence Analysis")
        
        sequences = {
            'perfectly_ordered': list(range(100)),
            'reverse_ordered': list(range(100, 0, -1)),
            'alternating': [1, 2] * 50,
            'random': np.random.randint(0, 10, 100).tolist(),
            'slow_drift': [i // 10 for i in range(100)],
            'fast_changes': [i % 10 for i in range(100)],
        }
        
        print(f"\n{'Sequence Type':<20} {'ICQ':<10} {'C-factor':<10} {'Autocorr':<10}")
        print("-"*80)
        
        for name, seq in sequences.items():
            icq, diag = self.calc.calculate_icq(seq)
            
            # Calculate autocorrelation at lag 1
            if len(seq) > 1:
                seq_array = np.array(seq)
                autocorr = np.corrcoef(seq_array[:-1], seq_array[1:])[0, 1] if np.std(seq_array) > 0 else 0
            else:
                autocorr = 0
            
            print(f"{name:<20} {icq:>8.4f}   {diag['c_factor']:>8.4f}   {autocorr:>8.4f}")
            
            self.results.append({
                'experiment': 'temporal_coherence',
                'sequence_type': name,
                'icq': icq,
                'c_factor': diag['c_factor'],
                'autocorrelation': autocorr
            })
        
        print("\n✓ Temporal coherence analyzed")
        
    def exp_comparison_with_traditional_metrics(self):
        """Compare ICQ with traditional information metrics."""
        self.print_header("EXPERIMENT: Comparison with Traditional Metrics")
        
        # Generate various test cases
        test_cases = [
            ('Low entropy (biased)', [1]*70 + [2]*20 + [3]*10),
            ('Medium entropy', [1]*40 + [2]*30 + [3]*20 + [4]*10),
            ('High entropy (uniform)', [i % 10 for i in range(100)]),
            ('Periodic pattern', [1, 2, 3] * 33 + [1]),
            ('Random', np.random.randint(0, 10, 100).tolist()),
        ]
        
        print(f"\n{'Test Case':<25} {'ICQ':<10} {'Entropy':<10} {'Normalized H':<15}")
        print("-"*80)
        
        for name, data in test_cases:
            icq, diag = self.calc.calculate_icq(data)
            
            print(f"{name:<25} {icq:>8.4f}   {diag['s_actual']:>8.4f}   {diag['normalized_entropy']:>8.4f}")
            
            self.results.append({
                'experiment': 'traditional_metrics_comparison',
                'test_case': name,
                'icq': icq,
                'entropy': diag['s_actual'],
                'max_entropy': diag['s_max'],
                'normalized_entropy': diag['normalized_entropy']
            })
        
        print("\n✓ Comparison with traditional metrics completed")
        
    def run_all(self):
        """Run all extended experiments."""
        print("\n" + "█"*80)
        print("█" + " "*78 + "█")
        print("█" + "EXTENDED EXPERIMENTAL VALIDATION".center(78) + "█")
        print("█" + "Advanced Pattern Recognition & Coherence Analysis".center(78) + "█")
        print("█" + " "*78 + "█")
        print("█"*80)
        
        self.exp_various_patterns()
        self.exp_data_transformation_effects()
        self.exp_size_sensitivity()
        self.exp_noise_resilience()
        self.exp_temporal_coherence()
        self.exp_comparison_with_traditional_metrics()
        
        # Save results
        output_file = 'extended_validation_results.json'
        with open(output_file, 'w') as f:
            json.dump({
                'metadata': {
                    'timestamp': datetime.utcnow().isoformat(),
                    'version': '1.1.0-extended',
                    'total_experiments': len(self.results)
                },
                'experiments': self.results
            }, f, indent=2, default=str)
        
        print("\n" + "="*80)
        print(f"✓ All {len(self.results)} extended experiments completed!")
        print(f"✓ Results saved to: {output_file}")
        print("="*80 + "\n")
        
        return self.results


if __name__ == "__main__":
    experiments = ExtendedExperiments()
    results = experiments.run_all()
    
    print("\n" + "█"*80)
    print("█" + " "*78 + "█")
    print("█" + "EXTENDED VALIDATION COMPLETE".center(78) + "█")
    print("█" + f"{len(results)} additional experiments successfully executed".center(78) + "█")
    print("█" + " "*78 + "█")
    print("█"*80 + "\n")
