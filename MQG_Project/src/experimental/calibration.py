"""
MQG-Theorie: Calibration Manager
=================================

This module provides calibration tools for experimental sensors
to ensure accurate and reliable ICQ measurements.

Features:
---------
1. Sensor calibration procedures
2. Baseline measurements
3. Noise characterization
4. Calibration curve generation

Author: MQG Project - Autonomous Development System
Version: 0.2.0-experimental
Date: 2026-02-01
"""

import numpy as np
from typing import Dict, List, Union, Tuple, Any, Optional
from datetime import datetime
import json


class CalibrationManager:
    """
    Manager for sensor calibration and validation.
    
    Ensures sensors provide accurate and consistent measurements
    for ICQ calculations.
    """
    
    def __init__(self):
        """Initialize calibration manager."""
        self.calibrations = {}
        self.metadata = {
            'created': datetime.utcnow().isoformat(),
            'version': '0.2.0-experimental'
        }
    
    def calibrate_sensor(
        self,
        sensor_name: str,
        reference_values: List[float],
        measured_values: List[float],
        method: str = 'linear'
    ) -> Dict[str, Any]:
        """
        Calibrate a sensor against reference values.
        
        Parameters:
        -----------
        sensor_name : str
            Unique sensor identifier
        reference_values : list of float
            Known reference values (ground truth)
        measured_values : list of float
            Corresponding sensor measurements
        method : str
            Calibration method ('linear', 'polynomial')
            
        Returns:
        --------
        calibration : dict
            Calibration parameters and statistics
        """
        ref = np.array(reference_values)
        meas = np.array(measured_values)
        
        if len(ref) != len(meas):
            raise ValueError("Reference and measured arrays must have same length")
        
        if method == 'linear':
            # Linear calibration: y = mx + b
            coeffs = np.polyfit(meas, ref, 1)
            
            # Calculate corrected values
            corrected = np.polyval(coeffs, meas)
            
            # Calculate error metrics
            errors = ref - corrected
            rmse = np.sqrt(np.mean(errors**2))
            mae = np.mean(np.abs(errors))
            r_squared = 1 - (np.sum(errors**2) / np.sum((ref - np.mean(ref))**2))
            
            calibration = {
                'sensor_name': sensor_name,
                'method': method,
                'coefficients': {
                    'slope': float(coeffs[0]),
                    'intercept': float(coeffs[1])
                },
                'statistics': {
                    'rmse': float(rmse),
                    'mae': float(mae),
                    'r_squared': float(r_squared),
                    'n_points': len(ref)
                },
                'timestamp': datetime.utcnow().isoformat()
            }
        
        elif method == 'polynomial':
            # Polynomial calibration (order 2)
            coeffs = np.polyfit(meas, ref, 2)
            
            corrected = np.polyval(coeffs, meas)
            errors = ref - corrected
            rmse = np.sqrt(np.mean(errors**2))
            mae = np.mean(np.abs(errors))
            r_squared = 1 - (np.sum(errors**2) / np.sum((ref - np.mean(ref))**2))
            
            calibration = {
                'sensor_name': sensor_name,
                'method': method,
                'coefficients': {
                    'a': float(coeffs[0]),
                    'b': float(coeffs[1]),
                    'c': float(coeffs[2])
                },
                'statistics': {
                    'rmse': float(rmse),
                    'mae': float(mae),
                    'r_squared': float(r_squared),
                    'n_points': len(ref)
                },
                'timestamp': datetime.utcnow().isoformat()
            }
        
        else:
            raise ValueError(f"Unknown calibration method: {method}")
        
        # Store calibration
        self.calibrations[sensor_name] = calibration
        
        return calibration
    
    def apply_calibration(
        self,
        sensor_name: str,
        raw_values: Union[float, np.ndarray]
    ) -> Union[float, np.ndarray]:
        """
        Apply calibration to raw sensor values.
        
        Parameters:
        -----------
        sensor_name : str
            Sensor identifier
        raw_values : float or np.ndarray
            Raw sensor reading(s)
            
        Returns:
        --------
        calibrated : float or np.ndarray
            Calibrated values
        """
        if sensor_name not in self.calibrations:
            raise ValueError(f"No calibration found for sensor '{sensor_name}'")
        
        calib = self.calibrations[sensor_name]
        method = calib['method']
        
        if method == 'linear':
            m = calib['coefficients']['slope']
            b = calib['coefficients']['intercept']
            return m * raw_values + b
        
        elif method == 'polynomial':
            a = calib['coefficients']['a']
            b = calib['coefficients']['b']
            c = calib['coefficients']['c']
            return a * raw_values**2 + b * raw_values + c
        
        else:
            raise ValueError(f"Unknown calibration method: {method}")
    
    def measure_baseline(
        self,
        sensor_readings: np.ndarray,
        duration: float = 60.0
    ) -> Dict[str, float]:
        """
        Measure baseline noise and drift characteristics.
        
        Parameters:
        -----------
        sensor_readings : np.ndarray
            Baseline measurements (sensor at rest)
        duration : float
            Duration of baseline measurement in seconds
            
        Returns:
        --------
        baseline : dict
            Baseline characteristics
        """
        baseline = {
            'mean': float(np.mean(sensor_readings)),
            'std': float(np.std(sensor_readings)),
            'min': float(np.min(sensor_readings)),
            'max': float(np.max(sensor_readings)),
            'range': float(np.max(sensor_readings) - np.min(sensor_readings)),
            'duration': duration,
            'n_samples': len(sensor_readings),
            'timestamp': datetime.utcnow().isoformat()
        }
        
        # Calculate drift (linear trend)
        x = np.arange(len(sensor_readings))
        p = np.polyfit(x, sensor_readings, 1)
        baseline['drift_rate'] = float(p[0])
        
        return baseline
    
    def characterize_noise(
        self,
        sensor_readings: np.ndarray
    ) -> Dict[str, Any]:
        """
        Characterize sensor noise properties.
        
        Parameters:
        -----------
        sensor_readings : np.ndarray
            Sensor measurements for noise analysis
            
        Returns:
        --------
        noise_profile : dict
            Noise characteristics
        """
        # Remove mean (detrend)
        detrended = sensor_readings - np.mean(sensor_readings)
        
        # Calculate power spectral density (simple periodogram)
        fft = np.fft.fft(detrended)
        psd = np.abs(fft)**2 / len(detrended)
        
        # Noise metrics
        noise_profile = {
            'rms_noise': float(np.sqrt(np.mean(detrended**2))),
            'peak_to_peak': float(np.max(detrended) - np.min(detrended)),
            'snr_estimate': float(np.mean(sensor_readings) / np.std(detrended)) if np.std(detrended) > 0 else np.inf,
            'psd_mean': float(np.mean(psd)),
            'psd_max': float(np.max(psd)),
            'timestamp': datetime.utcnow().isoformat()
        }
        
        return noise_profile
    
    def validate_calibration(
        self,
        sensor_name: str,
        test_references: List[float],
        test_measurements: List[float]
    ) -> Dict[str, Any]:
        """
        Validate calibration with test data.
        
        Parameters:
        -----------
        sensor_name : str
            Sensor identifier
        test_references : list of float
            Known test values
        test_measurements : list of float
            Sensor measurements of test values
            
        Returns:
        --------
        validation : dict
            Validation results
        """
        if sensor_name not in self.calibrations:
            raise ValueError(f"No calibration found for sensor '{sensor_name}'")
        
        # Apply calibration to test measurements
        calibrated = self.apply_calibration(sensor_name, np.array(test_measurements))
        
        # Calculate validation errors
        errors = np.array(test_references) - calibrated
        rmse = np.sqrt(np.mean(errors**2))
        mae = np.mean(np.abs(errors))
        max_error = np.max(np.abs(errors))
        
        validation = {
            'sensor_name': sensor_name,
            'n_test_points': len(test_references),
            'rmse': float(rmse),
            'mae': float(mae),
            'max_error': float(max_error),
            'passed': float(rmse) < 0.1,  # Example threshold
            'timestamp': datetime.utcnow().isoformat()
        }
        
        return validation
    
    def export_calibration(
        self,
        sensor_name: str,
        filepath: str
    ) -> None:
        """
        Export calibration data to file.
        
        Parameters:
        -----------
        sensor_name : str
            Sensor identifier
        filepath : str
            Output file path
        """
        if sensor_name not in self.calibrations:
            raise ValueError(f"No calibration found for sensor '{sensor_name}'")
        
        with open(filepath, 'w') as f:
            json.dump(self.calibrations[sensor_name], f, indent=2)
    
    def import_calibration(
        self,
        sensor_name: str,
        filepath: str
    ) -> None:
        """
        Import calibration data from file.
        
        Parameters:
        -----------
        sensor_name : str
            Sensor identifier
        filepath : str
            Input file path
        """
        with open(filepath, 'r') as f:
            calibration = json.load(f)
        
        self.calibrations[sensor_name] = calibration
    
    def get_calibration_report(self, sensor_name: str) -> str:
        """
        Generate calibration report.
        
        Parameters:
        -----------
        sensor_name : str
            Sensor identifier
            
        Returns:
        --------
        report : str
            Human-readable calibration report
        """
        if sensor_name not in self.calibrations:
            return f"No calibration found for sensor '{sensor_name}'"
        
        calib = self.calibrations[sensor_name]
        
        report = f"""
Calibration Report: {sensor_name}
{'=' * 60}

Method: {calib['method']}
Timestamp: {calib['timestamp']}

Coefficients:
{json.dumps(calib['coefficients'], indent=2)}

Statistics:
  RMSE: {calib['statistics']['rmse']:.6f}
  MAE: {calib['statistics']['mae']:.6f}
  R²: {calib['statistics']['r_squared']:.6f}
  Calibration points: {calib['statistics']['n_points']}

{'=' * 60}
"""
        return report


if __name__ == "__main__":
    # Self-test and demonstration
    print("=" * 70)
    print("MQG-Theorie: Calibration Manager - Self-Test")
    print("=" * 70)
    
    # Test 1: Linear calibration
    print("\n[Test 1] Linear Calibration")
    cm = CalibrationManager()
    
    # Simulate calibration data (sensor with slight offset and gain error)
    true_values = np.array([0, 10, 20, 30, 40, 50])
    measured_values = 1.05 * true_values + 0.5 + np.random.normal(0, 0.1, len(true_values))
    
    calib = cm.calibrate_sensor(
        'temp_sensor',
        reference_values=true_values.tolist(),
        measured_values=measured_values.tolist(),
        method='linear'
    )
    
    print(f"  Slope: {calib['coefficients']['slope']:.4f}")
    print(f"  Intercept: {calib['coefficients']['intercept']:.4f}")
    print(f"  RMSE: {calib['statistics']['rmse']:.4f}")
    print(f"  R²: {calib['statistics']['r_squared']:.4f}")
    
    # Test 2: Apply calibration
    print("\n[Test 2] Apply Calibration")
    raw_reading = 25.5
    calibrated_reading = cm.apply_calibration('temp_sensor', raw_reading)
    print(f"  Raw: {raw_reading:.2f}")
    print(f"  Calibrated: {calibrated_reading:.2f}")
    
    # Test 3: Baseline measurement
    print("\n[Test 3] Baseline Measurement")
    baseline_data = np.random.normal(0, 0.05, 1000)
    baseline = cm.measure_baseline(baseline_data, duration=10.0)
    print(f"  Mean: {baseline['mean']:.6f}")
    print(f"  Std: {baseline['std']:.6f}")
    print(f"  Drift rate: {baseline['drift_rate']:.6e}")
    
    # Test 4: Noise characterization
    print("\n[Test 4] Noise Characterization")
    noise_data = np.random.normal(5.0, 0.1, 1000)
    noise_profile = cm.characterize_noise(noise_data)
    print(f"  RMS noise: {noise_profile['rms_noise']:.6f}")
    print(f"  SNR estimate: {noise_profile['snr_estimate']:.2f}")
    
    # Test 5: Calibration report
    print("\n[Test 5] Calibration Report")
    report = cm.get_calibration_report('temp_sensor')
    print(report)
    
    print("=" * 70)
    print("Self-test complete. Calibration manager is operational.")
    print("=" * 70)
