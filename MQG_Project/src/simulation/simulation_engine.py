"""
MQG-Theorie: Simulation Environment
====================================

This module provides simulation capabilities for testing and validating
the MQG theory with synthetic data of known properties.

Simulation Types:
-----------------
1. Random systems (baseline)
2. Ordered systems (high coherence)
3. Mixed systems (realistic scenarios)
4. Dynamic systems (time-varying coherence)

Author: MQG Project - Autonomous Development System
Version: 0.1.0-alpha
Date: 2026-02-01
"""

import numpy as np
from typing import List, Dict, Any, Tuple
import matplotlib.pyplot as plt


class SimulationEngine:
    """
    Engine for generating synthetic data with controlled coherence properties.
    
    This allows validation of ICQ calculations and exploration of edge cases.
    """
    
    def __init__(self, seed: int = None):
        """
        Initialize simulation engine.
        
        Parameters:
        -----------
        seed : int, optional
            Random seed for reproducibility
        """
        self.seed = seed
        if seed is not None:
            np.random.seed(seed)
        
        self.simulations = []
    
    def generate_random_system(
        self,
        n_samples: int = 1000,
        n_states: int = 10
    ) -> Tuple[np.ndarray, Dict[str, Any]]:
        """
        Generate completely random system (baseline, low coherence).
        
        Expected ICQ: Close to 0 (maximum disorder)
        
        Parameters:
        -----------
        n_samples : int
            Number of samples to generate
        n_states : int
            Number of possible states
            
        Returns:
        --------
        data : np.ndarray
            Generated sequence
        properties : dict
            Known properties of the generated system
        """
        data = np.random.randint(0, n_states, size=n_samples)
        
        properties = {
            'type': 'random',
            'n_samples': n_samples,
            'n_states': n_states,
            'expected_icq_range': [0.0, 0.2],
            'description': 'Uniform random distribution (maximum entropy)'
        }
        
        return data, properties
    
    def generate_ordered_system(
        self,
        n_samples: int = 1000,
        pattern: List[int] = None
    ) -> Tuple[np.ndarray, Dict[str, Any]]:
        """
        Generate highly ordered system (high coherence).
        
        Expected ICQ: Close to 1 (maximum order)
        
        Parameters:
        -----------
        n_samples : int
            Number of samples to generate
        pattern : list, optional
            Repeating pattern. If None, uses constant value.
            
        Returns:
        --------
        data : np.ndarray
            Generated sequence
        properties : dict
            Known properties of the generated system
        """
        if pattern is None:
            # Constant value (perfect order)
            data = np.ones(n_samples, dtype=int)
            expected_icq = [0.9, 1.0]
        else:
            # Repeating pattern
            pattern = np.array(pattern)
            n_repeats = int(np.ceil(n_samples / len(pattern)))
            data = np.tile(pattern, n_repeats)[:n_samples]
            expected_icq = [0.7, 1.0]
        
        properties = {
            'type': 'ordered',
            'n_samples': n_samples,
            'pattern': pattern.tolist() if pattern is not None else [1],
            'expected_icq_range': expected_icq,
            'description': 'Highly ordered system (low entropy)'
        }
        
        return data, properties
    
    def generate_biased_system(
        self,
        n_samples: int = 1000,
        n_states: int = 10,
        bias_factor: float = 0.7
    ) -> Tuple[np.ndarray, Dict[str, Any]]:
        """
        Generate system with biased distribution (moderate coherence).
        
        Expected ICQ: 0.3 to 0.7 (moderate order)
        
        Parameters:
        -----------
        n_samples : int
            Number of samples to generate
        n_states : int
            Number of possible states
        bias_factor : float
            Bias towards first state (0.5 = uniform, 1.0 = all first state)
            
        Returns:
        --------
        data : np.ndarray
            Generated sequence
        properties : dict
            Known properties of the generated system
        """
        # Create biased probabilities
        probs = np.ones(n_states)
        probs[0] = bias_factor * n_states
        probs = probs / np.sum(probs)
        
        data = np.random.choice(n_states, size=n_samples, p=probs)
        
        properties = {
            'type': 'biased',
            'n_samples': n_samples,
            'n_states': n_states,
            'bias_factor': bias_factor,
            'probabilities': probs.tolist(),
            'expected_icq_range': [0.3, 0.7],
            'description': f'Biased distribution (bias_factor={bias_factor})'
        }
        
        return data, properties
    
    def generate_markov_chain(
        self,
        n_samples: int = 1000,
        n_states: int = 5,
        transition_bias: float = 0.6
    ) -> Tuple[np.ndarray, Dict[str, Any]]:
        """
        Generate data from a Markov chain (temporal coherence).
        
        Expected ICQ: 0.4 to 0.8 (temporal structure)
        
        Parameters:
        -----------
        n_samples : int
            Number of samples to generate
        n_states : int
            Number of states
        transition_bias : float
            Probability of staying in same state
            
        Returns:
        --------
        data : np.ndarray
            Generated sequence
        properties : dict
            Known properties of the generated system
        """
        # Create transition matrix
        transition_matrix = np.ones((n_states, n_states))
        for i in range(n_states):
            transition_matrix[i, i] = transition_bias * (n_states - 1)
        
        # Normalize rows
        transition_matrix = transition_matrix / transition_matrix.sum(axis=1, keepdims=True)
        
        # Generate sequence
        data = np.zeros(n_samples, dtype=int)
        data[0] = np.random.randint(0, n_states)
        
        for t in range(1, n_samples):
            current_state = data[t-1]
            data[t] = np.random.choice(n_states, p=transition_matrix[current_state])
        
        properties = {
            'type': 'markov_chain',
            'n_samples': n_samples,
            'n_states': n_states,
            'transition_bias': transition_bias,
            'expected_icq_range': [0.4, 0.8],
            'description': f'Markov chain (transition_bias={transition_bias})'
        }
        
        return data, properties
    
    def generate_noisy_pattern(
        self,
        n_samples: int = 1000,
        pattern: List[int] = [1, 2, 3],
        noise_level: float = 0.1
    ) -> Tuple[np.ndarray, Dict[str, Any]]:
        """
        Generate pattern with added noise (realistic scenario).
        
        Expected ICQ: depends on noise level
        
        Parameters:
        -----------
        n_samples : int
            Number of samples to generate
        pattern : list
            Base repeating pattern
        noise_level : float
            Probability of random state instead of pattern (0.0 to 1.0)
            
        Returns:
        --------
        data : np.ndarray
            Generated sequence
        properties : dict
            Known properties of the generated system
        """
        pattern = np.array(pattern)
        n_states = len(np.unique(pattern))
        
        # Generate clean pattern
        n_repeats = int(np.ceil(n_samples / len(pattern)))
        clean_data = np.tile(pattern, n_repeats)[:n_samples]
        
        # Add noise
        noise_mask = np.random.random(n_samples) < noise_level
        noise_values = np.random.randint(0, n_states, size=n_samples)
        
        data = np.where(noise_mask, noise_values, clean_data)
        
        # Expected ICQ decreases with noise
        expected_icq_max = 1.0 - noise_level
        expected_icq_min = max(0.0, expected_icq_max - 0.3)
        
        properties = {
            'type': 'noisy_pattern',
            'n_samples': n_samples,
            'pattern': pattern.tolist(),
            'noise_level': noise_level,
            'expected_icq_range': [expected_icq_min, expected_icq_max],
            'description': f'Pattern with {noise_level*100}% noise'
        }
        
        return data, properties
    
    def run_validation_suite(self) -> Dict[str, Any]:
        """
        Run comprehensive validation suite with multiple scenarios.
        
        Returns:
        --------
        results : dict
            Validation results for all scenarios
        """
        try:
            from ..core.icq_calculator import ICQCalculator
        except ImportError:
            from core.icq_calculator import ICQCalculator
        
        calc = ICQCalculator()
        results = {
            'scenarios': [],
            'summary': {}
        }
        
        # Scenario 1: Random system
        print("Testing: Random system...")
        data, props = self.generate_random_system(n_samples=1000)
        icq, diag = calc.calculate_icq(data)
        results['scenarios'].append({
            'name': 'random_system',
            'properties': props,
            'icq': icq,
            'diagnostics': diag,
            'in_expected_range': props['expected_icq_range'][0] <= icq <= props['expected_icq_range'][1]
        })
        
        # Scenario 2: Ordered system
        print("Testing: Ordered system...")
        data, props = self.generate_ordered_system(n_samples=1000)
        icq, diag = calc.calculate_icq(data)
        results['scenarios'].append({
            'name': 'ordered_system',
            'properties': props,
            'icq': icq,
            'diagnostics': diag,
            'in_expected_range': props['expected_icq_range'][0] <= icq <= props['expected_icq_range'][1]
        })
        
        # Scenario 3: Biased system
        print("Testing: Biased system...")
        data, props = self.generate_biased_system(n_samples=1000, bias_factor=0.7)
        icq, diag = calc.calculate_icq(data)
        results['scenarios'].append({
            'name': 'biased_system',
            'properties': props,
            'icq': icq,
            'diagnostics': diag,
            'in_expected_range': props['expected_icq_range'][0] <= icq <= props['expected_icq_range'][1]
        })
        
        # Scenario 4: Markov chain
        print("Testing: Markov chain...")
        data, props = self.generate_markov_chain(n_samples=1000)
        icq, diag = calc.calculate_icq(data)
        results['scenarios'].append({
            'name': 'markov_chain',
            'properties': props,
            'icq': icq,
            'diagnostics': diag,
            'in_expected_range': props['expected_icq_range'][0] <= icq <= props['expected_icq_range'][1]
        })
        
        # Scenario 5: Noisy pattern
        print("Testing: Noisy pattern...")
        data, props = self.generate_noisy_pattern(n_samples=1000, noise_level=0.2)
        icq, diag = calc.calculate_icq(data)
        results['scenarios'].append({
            'name': 'noisy_pattern',
            'properties': props,
            'icq': icq,
            'diagnostics': diag,
            'in_expected_range': props['expected_icq_range'][0] <= icq <= props['expected_icq_range'][1]
        })
        
        # Summary
        passed = sum(1 for s in results['scenarios'] if s['in_expected_range'])
        total = len(results['scenarios'])
        
        results['summary'] = {
            'total_scenarios': total,
            'passed': passed,
            'failed': total - passed,
            'success_rate': passed / total
        }
        
        return results


if __name__ == "__main__":
    # Self-test and demonstration
    print("=" * 70)
    print("MQG-Theorie: Simulation Engine - Self-Test")
    print("=" * 70)
    
    # Initialize simulation engine
    sim = SimulationEngine(seed=42)
    
    # Run validation suite
    print("\nRunning validation suite...\n")
    results = sim.run_validation_suite()
    
    # Print results
    print("\n" + "=" * 70)
    print("VALIDATION RESULTS")
    print("=" * 70)
    
    for scenario in results['scenarios']:
        status = "✓ PASS" if scenario['in_expected_range'] else "✗ FAIL"
        print(f"\n{status} | {scenario['name']}")
        print(f"  Description: {scenario['properties']['description']}")
        print(f"  Expected ICQ range: {scenario['properties']['expected_icq_range']}")
        print(f"  Actual ICQ: {scenario['icq']:.4f}")
        print(f"  Entropy: {scenario['diagnostics']['s_actual']:.4f} / {scenario['diagnostics']['s_max']:.4f}")
    
    # Summary
    print("\n" + "=" * 70)
    print("SUMMARY")
    print("=" * 70)
    print(f"Total scenarios: {results['summary']['total_scenarios']}")
    print(f"Passed: {results['summary']['passed']}")
    print(f"Failed: {results['summary']['failed']}")
    print(f"Success rate: {results['summary']['success_rate']*100:.1f}%")
    
    if results['summary']['success_rate'] == 1.0:
        print("\n✓ All validation tests passed!")
        print("MQG theory implementation is consistent with expected behavior.")
    else:
        print("\n⚠ Some validation tests failed.")
        print("Review scenarios for potential issues.")
    
    print("\n" + "=" * 70)
    print("Self-test complete. Simulation engine is operational.")
    print("=" * 70)
