"""
Double-Slit Quantum Experiment Simulator with MQG Modifications

This module simulates the classic double-slit experiment with electrons,
comparing the standard quantum mechanical prediction (Born rule) with
predictions from Modified Quantum Gravity (MQG) theory that includes
information field effects.

Statistical analysis identifies "hotspots" - detector positions where
the two models differ significantly.
"""

import numpy as np
from typing import Tuple, Dict, List
from dataclasses import dataclass
from scipy import stats


@dataclass
class DoubleSlitSetup:
    """Configuration for double-slit experiment"""
    slit_separation: float = 1.0e-6  # meters (1 μm)
    slit_width: float = 0.1e-6  # meters (0.1 μm)
    screen_distance: float = 1.0  # meters
    wavelength: float = 5.0e-12  # meters (electron de Broglie wavelength)
    detector_range: float = 100e-6  # meters (±50 μm from center)
    detector_resolution: int = 1000  # number of detector positions
    num_electrons: int = 100000  # electrons to simulate


class DoubleSlitMQG:
    """
    Simulator for double-slit experiment with MQG modifications.
    
    The standard quantum model uses the Born rule for interference patterns.
    The MQG model adds information field coherence effects that modify
    the probability distribution at specific detector positions.
    """
    
    def __init__(self, config: DoubleSlitSetup = None):
        """
        Initialize simulator with experiment configuration.
        
        Args:
            config: DoubleSlit experiment setup parameters
        """
        self.config = config or DoubleSlitSetup()
        self.detector_positions = self._create_detector_positions()
        
    def _create_detector_positions(self) -> np.ndarray:
        """Create array of detector positions along the screen"""
        return np.linspace(
            -self.config.detector_range / 2,
            self.config.detector_range / 2,
            self.config.detector_resolution
        )
    
    def _standard_quantum_pattern(self) -> np.ndarray:
        """
        Calculate standard quantum interference pattern using Born rule.
        
        Returns:
            Probability density at each detector position (normalized)
        """
        x = self.detector_positions
        d = self.config.slit_separation
        D = self.config.screen_distance
        wavelength = self.config.wavelength
        
        # Wave number
        k = 2 * np.pi / wavelength
        
        # Path difference from two slits
        r1 = np.sqrt(D**2 + (x + d/2)**2)
        r2 = np.sqrt(D**2 + (x - d/2)**2)
        delta = r2 - r1
        
        # Phase difference
        phi = k * delta
        
        # Standard quantum interference pattern (Born rule)
        # |ψ|² = |ψ1 + ψ2|² = |ψ1|² + |ψ2|² + 2|ψ1||ψ2|cos(φ)
        intensity = 1 + np.cos(phi)  # Simplified (assuming equal amplitudes)
        
        # Envelope function (single-slit diffraction)
        w = self.config.slit_width
        beta = k * w * x / D
        envelope = np.sinc(beta / np.pi)**2
        
        # Combined pattern
        pattern = intensity * envelope
        
        # Normalize to probability distribution
        return pattern / np.sum(pattern)
    
    def _mqg_modified_pattern(self, icq_coherence: float = 0.1) -> np.ndarray:
        """
        Calculate MQG-modified interference pattern.
        
        The MQG modification introduces information field coherence that
        creates local deviations from the standard pattern. These deviations
        are position-dependent and create "hotspots" of enhanced or reduced
        probability.
        
        Args:
            icq_coherence: Information Coherence Quotient (0-1)
                          Higher values = stronger MQG effects
        
        Returns:
            Modified probability density at each detector position
        """
        # Start with standard pattern
        standard = self._standard_quantum_pattern()
        
        # MQG modification: information field creates position-dependent
        # coherence modulation
        x = self.detector_positions
        
        # Spatial frequency of information field oscillations
        # (different from quantum wavelength)
        info_freq = 2.5e6  # rad/m (characteristic scale ~2.5 μm)
        
        # Information field phase
        info_phase = info_freq * x
        
        # MQG modification factor
        # Creates oscillating deviation from standard pattern
        mqg_factor = 1 + icq_coherence * np.sin(info_phase) * np.exp(-x**2 / (2 * (20e-6)**2))
        
        # Apply modification
        modified = standard * mqg_factor
        
        # Renormalize
        return modified / np.sum(modified)
    
    def simulate_experiment(self, icq_coherence: float = 0.1) -> Dict[str, np.ndarray]:
        """
        Simulate both standard and MQG-modified experiments.
        
        Args:
            icq_coherence: MQG information field coherence parameter
        
        Returns:
            Dictionary containing:
                - positions: detector positions (meters)
                - standard: standard QM prediction
                - mqg: MQG-modified prediction
                - standard_counts: simulated electron counts (standard)
                - mqg_counts: simulated electron counts (MQG)
        """
        # Get theoretical predictions
        standard_prob = self._standard_quantum_pattern()
        mqg_prob = self._mqg_modified_pattern(icq_coherence)
        
        # Simulate electron detection events
        N = self.config.num_electrons
        
        # Generate counts based on probabilities
        standard_counts = np.random.multinomial(N, standard_prob)
        mqg_counts = np.random.multinomial(N, mqg_prob)
        
        return {
            'positions': self.detector_positions,
            'standard': standard_prob * N,  # Expected counts
            'mqg': mqg_prob * N,  # Expected counts
            'standard_counts': standard_counts,
            'mqg_counts': mqg_counts
        }
    
    def statistical_comparison(self, results: Dict[str, np.ndarray]) -> Dict[str, float]:
        """
        Perform statistical comparison between standard and MQG predictions.
        
        Uses chi-squared test to quantify the significance of differences.
        
        Args:
            results: Output from simulate_experiment()
        
        Returns:
            Dictionary containing:
                - chi_squared: chi-squared statistic
                - p_value: statistical significance
                - dof: degrees of freedom
        """
        observed = results['mqg_counts']
        expected = results['standard']
        
        # Chi-squared test
        # χ² = Σ[(O - E)² / E]
        chi_squared = np.sum((observed - expected)**2 / expected)
        
        # Degrees of freedom
        dof = len(observed) - 1
        
        # p-value
        p_value = 1 - stats.chi2.cdf(chi_squared, dof)
        
        return {
            'chi_squared': chi_squared,
            'p_value': p_value,
            'dof': dof
        }
    
    def identify_hotspots(self, results: Dict[str, np.ndarray], 
                          threshold_sigma: float = 2.0) -> List[Dict]:
        """
        Identify detector positions with significant deviations (hotspots).
        
        A hotspot is a position where the MQG prediction deviates from
        the standard prediction by more than threshold_sigma standard deviations.
        
        Args:
            results: Output from simulate_experiment()
            threshold_sigma: Number of standard deviations for hotspot threshold
        
        Returns:
            List of dictionaries, each containing:
                - position: detector position (meters)
                - position_um: position in micrometers
                - deviation: (mqg - standard) / sigma
                - standard_value: standard QM prediction
                - mqg_value: MQG prediction
        """
        positions = results['positions']
        standard = results['standard']
        mqg = results['mqg']
        
        # Calculate deviations in units of standard deviation
        # σ ≈ √E for Poisson-like count statistics
        sigma = np.sqrt(standard)
        deviations = (mqg - standard) / sigma
        
        # Find positions exceeding threshold
        hotspot_indices = np.where(np.abs(deviations) > threshold_sigma)[0]
        
        hotspots = []
        for idx in hotspot_indices:
            hotspots.append({
                'position': positions[idx],
                'position_um': positions[idx] * 1e6,  # Convert to micrometers
                'deviation': deviations[idx],
                'standard_value': standard[idx],
                'mqg_value': mqg[idx]
            })
        
        return hotspots


def run_example():
    """Run example simulation and print results"""
    print("=== Double-Slit MQG Simulation ===\n")
    
    # Create simulator
    sim = DoubleSlitMQG()
    
    print(f"Configuration:")
    print(f"  Slit separation: {sim.config.slit_separation*1e6:.1f} μm")
    print(f"  Screen distance: {sim.config.screen_distance:.2f} m")
    print(f"  Electrons: {sim.config.num_electrons:,}")
    print(f"  Detector positions: {sim.config.detector_resolution}")
    print()
    
    # Run simulation
    print("Running simulation...")
    results = sim.simulate_experiment(icq_coherence=0.1)
    print("✓ Simulation complete\n")
    
    # Statistical comparison
    stats_results = sim.statistical_comparison(results)
    print("Statistical Comparison:")
    print(f"  Chi-squared: {stats_results['chi_squared']:.3f}")
    print(f"  p-value: {stats_results['p_value']:.2e}")
    print(f"  → Highly significant (p < 0.001): ", stats_results['p_value'] < 0.001)
    print()
    
    # Identify hotspots
    hotspots = sim.identify_hotspots(results, threshold_sigma=2.0)
    print(f"Hotspots Found: {len(hotspots)}")
    print(f"  (positions with deviation > 2σ)\n")
    
    if hotspots:
        print("Sample hotspots:")
        for i, hs in enumerate(hotspots[:5]):
            print(f"  {i+1}. Position: {hs['position_um']:+6.1f} μm, "
                  f"Deviation: {hs['deviation']:+.2f}σ")
    
    return results, hotspots


if __name__ == "__main__":
    results, hotspots = run_example()
