"""
MQG-Theorie: Measurement System
================================

This module provides tools for measuring Information Coherence in various
data sources and systems according to MQG theory principles.

Measurement Types:
------------------
1. Discrete measurements (symbolic sequences)
2. Continuous measurements (time series)
3. Multi-dimensional measurements (structured data)

Author: MQG Project - Autonomous Development System
Version: 0.1.0-alpha
Date: 2026-02-01
"""

import numpy as np
from typing import Dict, List, Union, Tuple, Any
import json
from datetime import datetime


class MeasurementSystem:
    """
    Comprehensive measurement system for ICQ data acquisition.
    
    This class handles:
    - Data collection from various sources
    - Data preprocessing and normalization
    - Measurement validation
    - Result storage and export
    """
    
    def __init__(self, config: Dict[str, Any] = None):
        """
        Initialize measurement system.
        
        Parameters:
        -----------
        config : dict, optional
            Configuration dictionary with measurement parameters
        """
        self.config = config or self._default_config()
        self.measurements = []
        self.metadata = {
            'created': datetime.utcnow().isoformat(),
            'version': '0.1.0-alpha',
            'theory': 'MQG (Measurable Information Coherence Law)'
        }
    
    def _default_config(self) -> Dict[str, Any]:
        """Return default configuration."""
        return {
            'sampling_rate': 1.0,
            'window_size': 100,
            'overlap': 0.5,
            'discretization_bins': 10,
            'validation_enabled': True
        }
    
    def measure_discrete_sequence(
        self,
        sequence: Union[List, np.ndarray],
        label: str = None
    ) -> Dict[str, Any]:
        """
        Measure ICQ for a discrete symbolic sequence.
        
        Parameters:
        -----------
        sequence : array-like
            Discrete sequence of symbols/states
        label : str, optional
            Descriptive label for this measurement
            
        Returns:
        --------
        measurement : dict
            Measurement result with ICQ value and metadata
        """
        try:
            from ..core.icq_calculator import ICQCalculator
        except ImportError:
            from core.icq_calculator import ICQCalculator
        
        calc = ICQCalculator()
        icq, diagnostics = calc.calculate_icq(sequence)
        
        measurement = {
            'timestamp': datetime.utcnow().isoformat(),
            'type': 'discrete_sequence',
            'label': label or f'measurement_{len(self.measurements)}',
            'icq': icq,
            'diagnostics': diagnostics,
            'data_length': len(sequence),
            'unique_states': len(np.unique(sequence))
        }
        
        self.measurements.append(measurement)
        return measurement
    
    def measure_continuous_timeseries(
        self,
        timeseries: Union[List[float], np.ndarray],
        label: str = None,
        bins: int = None
    ) -> Dict[str, Any]:
        """
        Measure ICQ for a continuous time series.
        
        The time series is discretized into bins for ICQ calculation.
        
        Parameters:
        -----------
        timeseries : array-like
            Continuous time series data
        label : str, optional
            Descriptive label for this measurement
        bins : int, optional
            Number of bins for discretization
            
        Returns:
        --------
        measurement : dict
            Measurement result with ICQ value and metadata
        """
        try:
            from ..core.icq_calculator import ICQCalculator
        except ImportError:
            from core.icq_calculator import ICQCalculator
        
        timeseries = np.array(timeseries)
        bins = bins or self.config['discretization_bins']
        
        # Discretize continuous data
        discretized, bin_edges = np.histogram(timeseries, bins=bins)
        
        # Create sequence from bin indices
        sequence = np.digitize(timeseries, bin_edges[:-1])
        
        calc = ICQCalculator()
        icq, diagnostics = calc.calculate_icq(sequence)
        
        measurement = {
            'timestamp': datetime.utcnow().isoformat(),
            'type': 'continuous_timeseries',
            'label': label or f'measurement_{len(self.measurements)}',
            'icq': icq,
            'diagnostics': diagnostics,
            'data_length': len(timeseries),
            'discretization_bins': bins,
            'value_range': [float(np.min(timeseries)), float(np.max(timeseries))],
            'statistics': {
                'mean': float(np.mean(timeseries)),
                'std': float(np.std(timeseries)),
                'median': float(np.median(timeseries))
            }
        }
        
        self.measurements.append(measurement)
        return measurement
    
    def measure_batch(
        self,
        data_items: List[Tuple[Any, str, str]],
        parallel: bool = False
    ) -> List[Dict[str, Any]]:
        """
        Measure ICQ for multiple data items in batch.
        
        Parameters:
        -----------
        data_items : list of tuples
            Each tuple: (data, label, type)
            where type is 'discrete' or 'continuous'
        parallel : bool, optional (default=False)
            Enable parallel processing (not yet implemented)
            
        Returns:
        --------
        measurements : list of dict
            List of measurement results
        """
        results = []
        
        for data, label, data_type in data_items:
            if data_type == 'discrete':
                result = self.measure_discrete_sequence(data, label)
            elif data_type == 'continuous':
                result = self.measure_continuous_timeseries(data, label)
            else:
                raise ValueError(f"Unknown data type: {data_type}")
            
            results.append(result)
        
        return results
    
    def validate_measurement(self, measurement: Dict[str, Any]) -> Tuple[bool, str]:
        """
        Validate a measurement result.
        
        Parameters:
        -----------
        measurement : dict
            Measurement to validate
            
        Returns:
        --------
        is_valid : bool
            True if measurement is valid
        message : str
            Validation message
        """
        # Check required fields
        required_fields = ['icq', 'diagnostics', 'timestamp', 'type']
        for field in required_fields:
            if field not in measurement:
                return False, f"Missing required field: {field}"
        
        # Check ICQ range
        icq = measurement['icq']
        if icq < 0:
            return False, f"ICQ value is negative: {icq}"
        
        # Check diagnostics
        diag = measurement['diagnostics']
        if diag['s_actual'] < 0 or diag['s_max'] < 0:
            return False, "Negative entropy values detected"
        
        if diag['s_actual'] > diag['s_max'] * 1.01:  # Small tolerance
            return False, f"Actual entropy ({diag['s_actual']}) exceeds maximum ({diag['s_max']})"
        
        return True, "Measurement is valid"
    
    def export_measurements(self, filepath: str, format: str = 'json') -> None:
        """
        Export all measurements to file.
        
        Parameters:
        -----------
        filepath : str
            Path to output file
        format : str, optional (default='json')
            Export format ('json', 'csv')
        """
        if format == 'json':
            export_data = {
                'metadata': self.metadata,
                'config': self.config,
                'measurements': self.measurements,
                'summary': self.get_summary()
            }
            
            with open(filepath, 'w') as f:
                json.dump(export_data, f, indent=2)
        
        elif format == 'csv':
            import csv
            
            with open(filepath, 'w', newline='') as f:
                if not self.measurements:
                    return
                
                # Get all possible keys
                fieldnames = set()
                for m in self.measurements:
                    fieldnames.update(m.keys())
                    if 'diagnostics' in m:
                        for k in m['diagnostics'].keys():
                            fieldnames.add(f'diag_{k}')
                
                fieldnames = sorted(fieldnames)
                writer = csv.DictWriter(f, fieldnames=fieldnames)
                writer.writeheader()
                
                for m in self.measurements:
                    row = {k: v for k, v in m.items() if k != 'diagnostics'}
                    if 'diagnostics' in m:
                        for k, v in m['diagnostics'].items():
                            row[f'diag_{k}'] = v
                    writer.writerow(row)
        
        else:
            raise ValueError(f"Unknown export format: {format}")
    
    def get_summary(self) -> Dict[str, Any]:
        """
        Get summary statistics of all measurements.
        
        Returns:
        --------
        summary : dict
            Summary statistics including:
            - Number of measurements
            - Average ICQ
            - ICQ range
            - Measurement types distribution
        """
        if not self.measurements:
            return {
                'total_measurements': 0,
                'message': 'No measurements recorded'
            }
        
        icq_values = [m['icq'] for m in self.measurements]
        
        types_count = {}
        for m in self.measurements:
            t = m.get('type', 'unknown')
            types_count[t] = types_count.get(t, 0) + 1
        
        return {
            'total_measurements': len(self.measurements),
            'icq_statistics': {
                'mean': float(np.mean(icq_values)),
                'std': float(np.std(icq_values)),
                'min': float(np.min(icq_values)),
                'max': float(np.max(icq_values)),
                'median': float(np.median(icq_values))
            },
            'measurement_types': types_count,
            'first_measurement': self.measurements[0]['timestamp'],
            'last_measurement': self.measurements[-1]['timestamp']
        }
    
    def clear_measurements(self) -> None:
        """Clear all stored measurements."""
        self.measurements = []


if __name__ == "__main__":
    # Self-test and demonstration
    print("=" * 70)
    print("MQG-Theorie: Measurement System - Self-Test")
    print("=" * 70)
    
    # Initialize measurement system
    ms = MeasurementSystem()
    
    # Test 1: Discrete sequence measurement
    print("\n[Test 1] Discrete sequence measurement")
    seq1 = [1, 1, 1, 2, 2, 3, 3, 3, 3]
    result1 = ms.measure_discrete_sequence(seq1, label="test_discrete_1")
    print(f"Sequence: {seq1}")
    print(f"ICQ: {result1['icq']:.4f}")
    print(f"Valid: {ms.validate_measurement(result1)}")
    
    # Test 2: Continuous timeseries measurement
    print("\n[Test 2] Continuous timeseries measurement")
    ts1 = np.sin(np.linspace(0, 4*np.pi, 100)) + np.random.normal(0, 0.1, 100)
    result2 = ms.measure_continuous_timeseries(ts1, label="test_timeseries_1")
    print(f"Timeseries length: {len(ts1)}")
    print(f"ICQ: {result2['icq']:.4f}")
    print(f"Statistics: {result2['statistics']}")
    
    # Test 3: Batch measurement
    print("\n[Test 3] Batch measurement")
    batch_data = [
        ([1, 1, 2, 2, 3, 3], "batch_discrete_1", "discrete"),
        ([1, 2, 3, 4, 5, 6], "batch_discrete_2", "discrete"),
        (np.random.randn(50), "batch_continuous_1", "continuous")
    ]
    batch_results = ms.measure_batch(batch_data)
    print(f"Batch measurements: {len(batch_results)}")
    for r in batch_results:
        print(f"  - {r['label']}: ICQ = {r['icq']:.4f}")
    
    # Summary
    print("\n[Summary]")
    summary = ms.get_summary()
    print(json.dumps(summary, indent=2))
    
    print("\n" + "=" * 70)
    print("Self-test complete. Measurement system is operational.")
    print("=" * 70)
