"""
Inertial Navigation System
===========================

Dead reckoning using IMU (Inertial Measurement Unit) sensors:
- Accelerometer (acceleration in 3 axes)
- Gyroscope (angular velocity in 3 axes)
- Optional magnetometer (compass heading)

Uses ICQ to validate sensor data quality and detect anomalies.
"""

import numpy as np
from typing import Tuple, List, Dict, Optional
import sys
import os

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from core.icq_calculator import ICQCalculator


class InertialNavigator:
    """
    Dead reckoning navigation using IMU sensors.
    
    Integrates acceleration to estimate velocity and position.
    Uses ICQ to validate sensor data and detect drift/anomalies.
    """
    
    def __init__(self, initial_position: Tuple[float, float, float] = (0, 0, 0),
                 icq_calculator: Optional[ICQCalculator] = None):
        """
        Initialize inertial navigator.
        
        Args:
            initial_position: Starting (x, y, z) position in meters
            icq_calculator: ICQ calculator for sensor validation
        """
        self.position = np.array(initial_position, dtype=float)
        self.velocity = np.array([0.0, 0.0, 0.0])
        self.orientation = np.array([0.0, 0.0, 0.0])  # Roll, pitch, yaw in radians
        
        self.icq_calc = icq_calculator or ICQCalculator()
        
        # Sensor data history for ICQ calculation
        self.accel_history = {'x': [], 'y': [], 'z': []}
        self.gyro_history = {'x': [], 'y': [], 'z': []}
        
        self.dt = 0.01  # Default time step (10ms)
        
        # Calibration offsets (to be set during calibration phase)
        self.accel_bias = np.array([0.0, 0.0, 0.0])
        self.gyro_bias = np.array([0.0, 0.0, 0.0])
        
    def update(self, accel: Tuple[float, float, float],
              gyro: Tuple[float, float, float],
              dt: float = None) -> Tuple[float, float, float]:
        """
        Update position based on IMU measurements.
        
        Args:
            accel: (ax, ay, az) acceleration in m/s^2
            gyro: (gx, gy, gz) angular velocity in rad/s
            dt: Time step in seconds (uses default if None)
            
        Returns:
            Current (x, y, z) position
        """
        if dt is not None:
            self.dt = dt
        
        # Record measurements for ICQ tracking
        self._record_measurements(accel, gyro)
        
        # Remove bias
        accel_corrected = np.array(accel) - self.accel_bias
        gyro_corrected = np.array(gyro) - self.gyro_bias
        
        # Update orientation (simplified - assumes small angles)
        self.orientation += gyro_corrected * self.dt
        
        # Rotate acceleration to world frame (simplified 2D rotation)
        # For full 3D, would use rotation matrices or quaternions
        yaw = self.orientation[2]
        cos_yaw = np.cos(yaw)
        sin_yaw = np.sin(yaw)
        
        accel_world = np.array([
            accel_corrected[0] * cos_yaw - accel_corrected[1] * sin_yaw,
            accel_corrected[0] * sin_yaw + accel_corrected[1] * cos_yaw,
            accel_corrected[2]  # Vertical acceleration
        ])
        
        # Remove gravity (assuming z-axis is up)
        accel_world[2] -= 9.81
        
        # Integrate to get velocity and position
        self.velocity += accel_world * self.dt
        self.position += self.velocity * self.dt
        
        return tuple(self.position)
    
    def _record_measurements(self, accel: Tuple[float, float, float],
                           gyro: Tuple[float, float, float]):
        """Record sensor measurements for ICQ calculation."""
        axes = ['x', 'y', 'z']
        
        for i, axis in enumerate(axes):
            # Convert to integers for ICQ (multiply by 100 to preserve precision)
            accel_int = int(accel[i] * 100)
            gyro_int = int(gyro[i] * 1000)  # Gyro values are smaller
            
            self.accel_history[axis].append(accel_int)
            self.gyro_history[axis].append(gyro_int)
            
            # Keep only recent history
            if len(self.accel_history[axis]) > 200:
                self.accel_history[axis] = self.accel_history[axis][-200:]
            if len(self.gyro_history[axis]) > 200:
                self.gyro_history[axis] = self.gyro_history[axis][-200:]
    
    def calibrate(self, accel_samples: List[Tuple[float, float, float]],
                 gyro_samples: List[Tuple[float, float, float]]):
        """
        Calibrate sensor biases from stationary measurements.
        
        Args:
            accel_samples: List of accelerometer readings (device stationary)
            gyro_samples: List of gyroscope readings (device stationary)
        """
        if not accel_samples or not gyro_samples:
            return
        
        # Calculate mean bias
        accel_array = np.array(accel_samples)
        gyro_array = np.array(gyro_samples)
        
        self.accel_bias = np.mean(accel_array, axis=0)
        self.gyro_bias = np.mean(gyro_array, axis=0)
        
        # Adjust accelerometer bias for gravity (should read +9.81 on z-axis when level)
        # Assuming device is level during calibration
        self.accel_bias[2] -= 9.81
    
    def get_sensor_icq(self, sensor: str = 'accel', axis: str = 'x') -> Tuple[float, Dict]:
        """
        Calculate ICQ for a sensor axis.
        
        Args:
            sensor: 'accel' or 'gyro'
            axis: 'x', 'y', or 'z'
            
        Returns:
            (ICQ value, diagnostics dict)
        """
        if sensor == 'accel':
            history = self.accel_history[axis]
        elif sensor == 'gyro':
            history = self.gyro_history[axis]
        else:
            return 0.0, {'error': 'Unknown sensor'}
        
        if len(history) < 20:
            return 0.0, {'error': 'Insufficient data'}
        
        return self.icq_calc.calculate_icq(history)
    
    def reset_position(self, position: Tuple[float, float, float] = (0, 0, 0)):
        """Reset position to a known value."""
        self.position = np.array(position, dtype=float)
        self.velocity = np.array([0.0, 0.0, 0.0])
    
    def get_sensor_quality_report(self) -> Dict:
        """
        Generate comprehensive sensor quality report using ICQ.
        
        Returns:
            Dictionary with ICQ scores for all sensors
        """
        report = {
            'accelerometer': {},
            'gyroscope': {}
        }
        
        for axis in ['x', 'y', 'z']:
            # Accelerometer ICQ
            icq_accel, diag_accel = self.get_sensor_icq('accel', axis)
            report['accelerometer'][axis] = {
                'icq': icq_accel,
                'samples': len(self.accel_history[axis]),
                'diagnostics': diag_accel
            }
            
            # Gyroscope ICQ
            icq_gyro, diag_gyro = self.get_sensor_icq('gyro', axis)
            report['gyroscope'][axis] = {
                'icq': icq_gyro,
                'samples': len(self.gyro_history[axis]),
                'diagnostics': diag_gyro
            }
        
        return report
    
    def detect_anomaly(self, threshold: float = 0.1) -> Dict[str, bool]:
        """
        Detect sensor anomalies using ICQ.
        
        Low ICQ indicates inconsistent/noisy sensor data.
        
        Args:
            threshold: ICQ threshold below which to flag anomaly
            
        Returns:
            Dictionary indicating anomalies for each sensor axis
        """
        anomalies = {}
        
        for sensor in ['accel', 'gyro']:
            for axis in ['x', 'y', 'z']:
                icq, _ = self.get_sensor_icq(sensor, axis)
                key = f"{sensor}_{axis}"
                anomalies[key] = icq < threshold
        
        return anomalies
