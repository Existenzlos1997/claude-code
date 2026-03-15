"""
MQG-Theorie: Experimental Data Adapter
=======================================

This module provides adapters for real-world experimental data streams
and real-time ICQ processing.

Features:
---------
1. Real-time data buffering
2. Streaming ICQ calculation
3. Data preprocessing and filtering
4. Experimental data import/export

Author: MQG Project - Autonomous Development System
Version: 0.2.0-experimental
Date: 2026-02-01
"""

import numpy as np
from typing import Dict, List, Union, Tuple, Any, Callable
from collections import deque
from datetime import datetime
import json


class ExperimentalDataAdapter:
    """
    Adapter for importing and preprocessing experimental data.
    
    Handles various experimental data formats and prepares them
    for ICQ analysis.
    """
    
    def __init__(self, config: Dict[str, Any] = None):
        """
        Initialize experimental data adapter.
        
        Parameters:
        -----------
        config : dict, optional
            Configuration for data processing
        """
        self.config = config or self._default_config()
        self.metadata = {
            'created': datetime.utcnow().isoformat(),
            'version': '0.2.0-experimental',
            'data_sources': []
        }
    
    def _default_config(self) -> Dict[str, Any]:
        """Return default configuration."""
        return {
            'preprocessing': {
                'remove_outliers': True,
                'outlier_threshold': 3.0,  # std deviations
                'normalize': False,
                'detrend': False
            },
            'filtering': {
                'enabled': False,
                'filter_type': 'lowpass',
                'cutoff_frequency': 10.0
            }
        }
    
    def load_from_csv(
        self,
        filepath: str,
        column: Union[int, str] = 0,
        skip_header: int = 0
    ) -> np.ndarray:
        """
        Load experimental data from CSV file.
        
        Parameters:
        -----------
        filepath : str
            Path to CSV file
        column : int or str
            Column index or name to extract
        skip_header : int
            Number of header rows to skip
            
        Returns:
        --------
        data : np.ndarray
            Loaded data array
        """
        try:
            # Load CSV data
            data = np.genfromtxt(filepath, delimiter=',', skip_header=skip_header)
            
            # Extract specific column if multi-dimensional
            if data.ndim > 1:
                if isinstance(column, int):
                    data = data[:, column]
                else:
                    raise ValueError("Named columns not yet supported")
            
            # Preprocess
            data = self._preprocess(data)
            
            self.metadata['data_sources'].append({
                'type': 'csv',
                'filepath': filepath,
                'timestamp': datetime.utcnow().isoformat()
            })
            
            return data
            
        except Exception as e:
            raise RuntimeError(f"Failed to load CSV: {e}")
    
    def load_from_json(self, filepath: str, data_key: str = 'data') -> np.ndarray:
        """
        Load experimental data from JSON file.
        
        Parameters:
        -----------
        filepath : str
            Path to JSON file
        data_key : str
            Key in JSON containing the data array
            
        Returns:
        --------
        data : np.ndarray
            Loaded data array
        """
        try:
            with open(filepath, 'r') as f:
                json_data = json.load(f)
            
            data = np.array(json_data[data_key])
            data = self._preprocess(data)
            
            self.metadata['data_sources'].append({
                'type': 'json',
                'filepath': filepath,
                'timestamp': datetime.utcnow().isoformat()
            })
            
            return data
            
        except Exception as e:
            raise RuntimeError(f"Failed to load JSON: {e}")
    
    def load_from_numpy(self, filepath: str) -> np.ndarray:
        """
        Load experimental data from NumPy file (.npy or .npz).
        
        Parameters:
        -----------
        filepath : str
            Path to NumPy file
            
        Returns:
        --------
        data : np.ndarray
            Loaded data array
        """
        try:
            data = np.load(filepath)
            
            # Handle .npz files
            if isinstance(data, np.lib.npyio.NpzFile):
                # Get first array
                key = list(data.keys())[0]
                data = data[key]
            
            data = self._preprocess(data)
            
            self.metadata['data_sources'].append({
                'type': 'numpy',
                'filepath': filepath,
                'timestamp': datetime.utcnow().isoformat()
            })
            
            return data
            
        except Exception as e:
            raise RuntimeError(f"Failed to load NumPy file: {e}")
    
    def _preprocess(self, data: np.ndarray) -> np.ndarray:
        """
        Preprocess experimental data.
        
        Parameters:
        -----------
        data : np.ndarray
            Raw data
            
        Returns:
        --------
        processed : np.ndarray
            Preprocessed data
        """
        processed = data.copy()
        
        # Remove outliers
        if self.config['preprocessing']['remove_outliers']:
            threshold = self.config['preprocessing']['outlier_threshold']
            mean = np.mean(processed)
            std = np.std(processed)
            
            mask = np.abs(processed - mean) <= threshold * std
            processed = processed[mask]
        
        # Normalize
        if self.config['preprocessing']['normalize']:
            processed = (processed - np.mean(processed)) / (np.std(processed) + 1e-10)
        
        # Detrend
        if self.config['preprocessing']['detrend']:
            # Simple linear detrend
            x = np.arange(len(processed))
            p = np.polyfit(x, processed, 1)
            trend = np.polyval(p, x)
            processed = processed - trend
        
        return processed


class RealTimeProcessor:
    """
    Real-time ICQ processor for streaming experimental data.
    
    Maintains a sliding window buffer and calculates ICQ in real-time.
    """
    
    def __init__(
        self,
        window_size: int = 1000,
        update_interval: int = 100,
        config: Dict[str, Any] = None
    ):
        """
        Initialize real-time processor.
        
        Parameters:
        -----------
        window_size : int
            Size of sliding window (number of samples)
        update_interval : int
            Number of new samples between ICQ recalculations
        config : dict, optional
            Processing configuration
        """
        self.window_size = window_size
        self.update_interval = update_interval
        self.config = config or {}
        
        self.buffer = deque(maxlen=window_size)
        self.sample_count = 0
        self.icq_history = []
        self.timestamps = []
        
        self.metadata = {
            'created': datetime.utcnow().isoformat(),
            'version': '0.2.0-experimental',
            'window_size': window_size,
            'update_interval': update_interval
        }
    
    def add_sample(self, sample: float) -> Tuple[bool, float]:
        """
        Add a new sample to the buffer.
        
        Parameters:
        -----------
        sample : float
            New data sample
            
        Returns:
        --------
        updated : bool
            True if ICQ was recalculated
        icq : float
            Current ICQ value (or NaN if not yet calculated)
        """
        self.buffer.append(sample)
        self.sample_count += 1
        
        # Check if we should update ICQ
        should_update = (
            len(self.buffer) >= self.window_size and
            self.sample_count % self.update_interval == 0
        )
        
        if should_update:
            icq = self._calculate_icq()
            self.icq_history.append(icq)
            self.timestamps.append(datetime.utcnow().isoformat())
            return True, icq
        
        return False, np.nan
    
    def add_batch(self, samples: np.ndarray) -> List[Tuple[bool, float]]:
        """
        Add multiple samples at once.
        
        Parameters:
        -----------
        samples : np.ndarray
            Array of samples
            
        Returns:
        --------
        results : list of tuples
            List of (updated, icq) for each sample
        """
        results = []
        for sample in samples:
            result = self.add_sample(sample)
            results.append(result)
        return results
    
    def _calculate_icq(self) -> float:
        """
        Calculate ICQ for current buffer.
        
        Returns:
        --------
        icq : float
            Information Coherence Quotient
        """
        try:
            from ..core.icq_calculator import ICQCalculator
        except ImportError:
            from core.icq_calculator import ICQCalculator
        
        data = np.array(self.buffer)
        calc = ICQCalculator()
        icq, _ = calc.calculate_icq(data)
        
        return icq
    
    def get_current_icq(self) -> float:
        """
        Get most recent ICQ value.
        
        Returns:
        --------
        icq : float
            Most recent ICQ, or NaN if not calculated yet
        """
        if self.icq_history:
            return self.icq_history[-1]
        return np.nan
    
    def get_statistics(self) -> Dict[str, Any]:
        """
        Get statistics about the real-time processing.
        
        Returns:
        --------
        stats : dict
            Processing statistics
        """
        icq_array = np.array(self.icq_history)
        
        stats = {
            'total_samples': self.sample_count,
            'buffer_size': len(self.buffer),
            'icq_calculations': len(self.icq_history),
            'current_icq': self.get_current_icq()
        }
        
        if len(self.icq_history) > 0:
            stats['icq_mean'] = float(np.mean(icq_array))
            stats['icq_std'] = float(np.std(icq_array))
            stats['icq_min'] = float(np.min(icq_array))
            stats['icq_max'] = float(np.max(icq_array))
        
        return stats
    
    def export_results(self, filepath: str, format: str = 'json') -> None:
        """
        Export processing results to file.
        
        Parameters:
        -----------
        filepath : str
            Output file path
        format : str
            Export format ('json' or 'csv')
        """
        if format == 'json':
            results = {
                'metadata': self.metadata,
                'statistics': self.get_statistics(),
                'icq_history': self.icq_history,
                'timestamps': self.timestamps
            }
            
            with open(filepath, 'w') as f:
                json.dump(results, f, indent=2)
        
        elif format == 'csv':
            import csv
            
            with open(filepath, 'w', newline='') as f:
                writer = csv.writer(f)
                writer.writerow(['timestamp', 'icq'])
                
                for timestamp, icq in zip(self.timestamps, self.icq_history):
                    writer.writerow([timestamp, icq])
        
        else:
            raise ValueError(f"Unknown format: {format}")
    
    def reset(self) -> None:
        """Reset processor state."""
        self.buffer.clear()
        self.sample_count = 0
        self.icq_history.clear()
        self.timestamps.clear()


if __name__ == "__main__":
    # Self-test and demonstration
    print("=" * 70)
    print("MQG-Theorie: Experimental Data Adapter - Self-Test")
    print("=" * 70)
    
    # Test 1: Data adapter
    print("\n[Test 1] Experimental Data Adapter")
    adapter = ExperimentalDataAdapter()
    
    # Create test data
    test_data = np.sin(np.linspace(0, 10, 100)) + np.random.normal(0, 0.1, 100)
    np.savetxt('/tmp/test_data.csv', test_data, delimiter=',')
    
    loaded_data = adapter.load_from_csv('/tmp/test_data.csv')
    print(f"  Loaded {len(loaded_data)} samples from CSV")
    print(f"  Data range: [{np.min(loaded_data):.3f}, {np.max(loaded_data):.3f}]")
    
    # Test 2: Real-time processor
    print("\n[Test 2] Real-Time Processor")
    processor = RealTimeProcessor(window_size=50, update_interval=10)
    
    # Simulate streaming data
    for i in range(100):
        sample = np.sin(i * 0.1) + np.random.normal(0, 0.1)
        updated, icq = processor.add_sample(sample)
        
        if updated:
            print(f"  Sample {i}: ICQ updated to {icq:.4f}")
    
    # Statistics
    stats = processor.get_statistics()
    print(f"\n  Total samples processed: {stats['total_samples']}")
    print(f"  ICQ calculations: {stats['icq_calculations']}")
    print(f"  Current ICQ: {stats['current_icq']:.4f}")
    
    if 'icq_mean' in stats:
        print(f"  ICQ mean: {stats['icq_mean']:.4f}")
        print(f"  ICQ std: {stats['icq_std']:.4f}")
    
    # Test 3: Export results
    print("\n[Test 3] Export Results")
    processor.export_results('/tmp/realtime_results.json', format='json')
    print("  ✓ Exported to JSON")
    
    processor.export_results('/tmp/realtime_results.csv', format='csv')
    print("  ✓ Exported to CSV")
    
    print("\n" + "=" * 70)
    print("Self-test complete. Experimental adapter is operational.")
    print("=" * 70)
