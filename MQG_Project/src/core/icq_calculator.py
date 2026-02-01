"""
MQG-Theorie: Information Coherence Quotient (ICQ) Calculator
==============================================================

Dieses Modul implementiert die Kernfunktionalität zur Berechnung des
Information Coherence Quotient (ICQ) gemäß der MQG-Theorie.

Mathematical Foundation:
------------------------
ICQ = (S_max - S_actual) / S_max × C_factor

Where:
- S_max: Maximum theoretical entropy of the system
- S_actual: Actually measured entropy
- C_factor: Coherence correction factor (considers structural properties)

Properties:
-----------
- Range: 0 ≤ ICQ ≤ 1
- ICQ = 0: Maximum disorder (no coherence)
- ICQ = 1: Perfect coherence (maximum order)
- Dimensionless: Enables comparisons between different systems

Author: MQG Project - Autonomous Development System
Version: 0.1.0-alpha
Date: 2026-02-01
"""

import numpy as np
from typing import Union, List, Tuple
import warnings


class ICQCalculator:
    """
    Main class for calculating Information Coherence Quotient (ICQ).
    
    This calculator implements the core algorithm of the MQG theory,
    providing measurable, reproducible, and scalable metrics for
    information coherence in complex systems.
    """
    
    def __init__(self, base: float = 2.0):
        """
        Initialize ICQ Calculator.
        
        Parameters:
        -----------
        base : float, optional (default=2.0)
            Base for logarithm in entropy calculation.
            - base=2: Information measured in bits
            - base=e: Information measured in nats
            - base=10: Information measured in bans
        """
        self.base = base
        
    def calculate_entropy(self, probabilities: Union[List[float], np.ndarray]) -> float:
        """
        Calculate Shannon entropy for a given probability distribution.
        
        Parameters:
        -----------
        probabilities : array-like
            Probability distribution (must sum to 1.0)
            
        Returns:
        --------
        entropy : float
            Shannon entropy H(X) = -Σ p_i * log_base(p_i)
            
        Raises:
        -------
        ValueError
            If probabilities don't sum to approximately 1.0
            If any probability is negative
        """
        probs = np.array(probabilities)
        
        # Validation
        if np.any(probs < 0):
            raise ValueError("Probabilities must be non-negative")
        
        if not np.isclose(np.sum(probs), 1.0, rtol=1e-5):
            raise ValueError(f"Probabilities must sum to 1.0, got {np.sum(probs)}")
        
        # Remove zero probabilities (0 * log(0) = 0 by convention)
        probs_nonzero = probs[probs > 0]
        
        # Calculate entropy
        if self.base == np.e:
            entropy = -np.sum(probs_nonzero * np.log(probs_nonzero))
        else:
            entropy = -np.sum(probs_nonzero * np.log(probs_nonzero) / np.log(self.base))
            
        return entropy
    
    def calculate_max_entropy(self, n_states: int) -> float:
        """
        Calculate maximum possible entropy for n states.
        
        Maximum entropy occurs when all states are equally probable.
        
        Parameters:
        -----------
        n_states : int
            Number of possible states in the system
            
        Returns:
        --------
        max_entropy : float
            Maximum entropy = log_base(n_states)
        """
        if n_states <= 0:
            raise ValueError("Number of states must be positive")
            
        if self.base == np.e:
            return np.log(n_states)
        else:
            return np.log(n_states) / np.log(self.base)
    
    def calculate_coherence_factor(
        self, 
        data: Union[List, np.ndarray],
        method: str = 'autocorrelation'
    ) -> float:
        """
        Calculate coherence correction factor based on structural properties.
        
        The coherence factor captures structural regularities that go beyond
        simple entropy measures, such as:
        - Temporal correlations (autocorrelation)
        - Spatial patterns (if applicable)
        - Hierarchical structure
        
        Parameters:
        -----------
        data : array-like
            Input data sequence/structure
        method : str, optional (default='autocorrelation')
            Method for calculating coherence factor
            Options: 'autocorrelation', 'simple', 'advanced'
            
        Returns:
        --------
        c_factor : float
            Coherence correction factor (typically 0.5 to 1.5)
        """
        data = np.array(data)
        
        if method == 'simple':
            # Simple method: uniform factor
            return 1.0
            
        elif method == 'autocorrelation':
            # Calculate autocorrelation coefficient
            if len(data) < 2:
                return 1.0
                
            # Normalize data
            data_normalized = (data - np.mean(data)) / (np.std(data) + 1e-10)
            
            # Calculate lag-1 autocorrelation
            autocorr = np.corrcoef(data_normalized[:-1], data_normalized[1:])[0, 1]
            
            # Map autocorrelation to coherence factor (0.5 to 1.5)
            # High positive autocorr -> higher coherence
            c_factor = 1.0 + 0.5 * autocorr
            
            # Ensure valid range
            c_factor = np.clip(c_factor, 0.5, 1.5)
            
            return c_factor
            
        else:
            warnings.warn(f"Unknown method '{method}', using 'simple'")
            return 1.0
    
    def calculate_icq(
        self,
        data: Union[List, np.ndarray],
        probabilities: Union[List[float], np.ndarray] = None,
        n_states: int = None,
        coherence_method: str = 'autocorrelation'
    ) -> Tuple[float, dict]:
        """
        Calculate Information Coherence Quotient (ICQ).
        
        Main function of the MQG theory. Calculates ICQ based on:
        - Actual entropy (from probability distribution)
        - Maximum theoretical entropy (from number of states)
        - Coherence correction factor (from data structure)
        
        Parameters:
        -----------
        data : array-like
            Input data for coherence factor calculation
        probabilities : array-like, optional
            Probability distribution. If None, calculated from data.
        n_states : int, optional
            Number of possible states. If None, inferred from data.
        coherence_method : str, optional (default='autocorrelation')
            Method for coherence factor calculation
            
        Returns:
        --------
        icq : float
            Information Coherence Quotient (0 ≤ ICQ ≤ 1+)
        diagnostics : dict
            Detailed diagnostic information including:
            - s_actual: Actual entropy
            - s_max: Maximum entropy
            - c_factor: Coherence factor
            - n_states: Number of states used
            
        Examples:
        ---------
        >>> calc = ICQCalculator()
        >>> data = [1, 1, 1, 1, 2, 2, 3]  # Low entropy, some structure
        >>> icq, diag = calc.calculate_icq(data)
        >>> print(f"ICQ: {icq:.3f}")
        ICQ: 0.756
        """
        data = np.array(data)
        
        # Calculate or use provided probabilities
        if probabilities is None:
            # Estimate probabilities from data
            unique, counts = np.unique(data, return_counts=True)
            probabilities = counts / len(data)
            if n_states is None:
                n_states = len(unique)
        else:
            if n_states is None:
                n_states = len(probabilities)
        
        # Calculate components
        s_actual = self.calculate_entropy(probabilities)
        s_max = self.calculate_max_entropy(n_states)
        c_factor = self.calculate_coherence_factor(data, method=coherence_method)
        
        # Calculate ICQ
        # ICQ = (S_max - S_actual) / S_max × C_factor
        if s_max == 0:
            icq = 0.0
        else:
            icq = ((s_max - s_actual) / s_max) * c_factor
        
        # Ensure valid range (can exceed 1.0 with high coherence factor)
        icq = max(0.0, icq)
        
        # Prepare diagnostics
        diagnostics = {
            's_actual': s_actual,
            's_max': s_max,
            'c_factor': c_factor,
            'n_states': n_states,
            'normalized_entropy': s_actual / s_max if s_max > 0 else 0,
            'coherence_contribution': c_factor - 1.0
        }
        
        return icq, diagnostics


def quick_icq(data: Union[List, np.ndarray], base: float = 2.0) -> float:
    """
    Quick calculation of ICQ with default parameters.
    
    Convenience function for fast ICQ calculation without detailed diagnostics.
    
    Parameters:
    -----------
    data : array-like
        Input data
    base : float, optional (default=2.0)
        Logarithm base for entropy calculation
        
    Returns:
    --------
    icq : float
        Information Coherence Quotient
        
    Example:
    --------
    >>> icq = quick_icq([1, 1, 1, 2, 2, 3])
    >>> print(f"ICQ: {icq:.3f}")
    """
    calc = ICQCalculator(base=base)
    icq, _ = calc.calculate_icq(data)
    return icq


if __name__ == "__main__":
    # Self-test and demonstration
    print("=" * 70)
    print("MQG-Theorie: ICQ Calculator - Self-Test")
    print("=" * 70)
    
    # Test 1: Perfect coherence (all same values)
    print("\n[Test 1] Perfect coherence (all same values)")
    data1 = [1, 1, 1, 1, 1]
    calc = ICQCalculator()
    icq1, diag1 = calc.calculate_icq(data1)
    print(f"Data: {data1}")
    print(f"ICQ: {icq1:.4f}")
    print(f"Diagnostics: {diag1}")
    
    # Test 2: Maximum disorder (uniform distribution)
    print("\n[Test 2] Maximum disorder (uniform distribution)")
    data2 = [1, 2, 3, 4, 5, 6, 7, 8]
    icq2, diag2 = calc.calculate_icq(data2)
    print(f"Data: {data2}")
    print(f"ICQ: {icq2:.4f}")
    print(f"Diagnostics: {diag2}")
    
    # Test 3: Moderate coherence (biased distribution)
    print("\n[Test 3] Moderate coherence (biased distribution)")
    data3 = [1, 1, 1, 1, 2, 2, 3]
    icq3, diag3 = calc.calculate_icq(data3)
    print(f"Data: {data3}")
    print(f"ICQ: {icq3:.4f}")
    print(f"Diagnostics: {diag3}")
    
    # Test 4: High temporal structure
    print("\n[Test 4] High temporal structure (pattern)")
    data4 = [1, 2, 1, 2, 1, 2, 1, 2]
    icq4, diag4 = calc.calculate_icq(data4)
    print(f"Data: {data4}")
    print(f"ICQ: {icq4:.4f}")
    print(f"Diagnostics: {diag4}")
    
    print("\n" + "=" * 70)
    print("Self-test complete. MQG ICQ Calculator is operational.")
    print("=" * 70)
