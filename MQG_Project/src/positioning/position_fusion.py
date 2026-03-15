"""
Position Fusion System
=======================

Fuses position estimates from multiple sources:
- Signal-based positioning (Wi-Fi, Bluetooth, cellular)
- Inertial navigation (IMU dead reckoning)
- Other sources (e.g., vision, ultrasonic, etc.)

Uses ICQ to weight sources based on data quality.
Implements Kalman filtering for optimal fusion.
"""

import numpy as np
from typing import Dict, List, Tuple, Optional
import sys
import os

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from core.icq_calculator import ICQCalculator


class PositionFusion:
    """
    Multi-source position fusion using ICQ-weighted combination.
    
    Combines multiple positioning sources with weights based on:
    - ICQ scores (data quality/coherence)
    - Historical accuracy
    - Variance/uncertainty estimates
    """
    
    def __init__(self, icq_calculator: Optional[ICQCalculator] = None):
        """
        Initialize position fusion system.
        
        Args:
            icq_calculator: ICQ calculator for data quality assessment
        """
        self.icq_calc = icq_calculator or ICQCalculator()
        
        # Current best estimate
        self.position = np.array([0.0, 0.0, 0.0])
        self.variance = np.array([100.0, 100.0, 100.0])  # Initial uncertainty
        
        # Source tracking
        self.sources = {}  # {source_name: source_info}
        self.position_history = []  # History for ICQ calculation
        
        # Kalman filter state (simplified)
        self.kalman_state = np.array([0.0, 0.0, 0.0])  # Position
        self.kalman_covariance = np.eye(3) * 100.0  # Initial covariance
        
    def register_source(self, source_name: str, 
                       initial_weight: float = 1.0,
                       variance: float = 10.0):
        """
        Register a positioning source.
        
        Args:
            source_name: Identifier for the source
            initial_weight: Initial weighting factor
            variance: Expected measurement variance
        """
        self.sources[source_name] = {
            'weight': initial_weight,
            'variance': variance,
            'measurements': [],
            'icq_scores': [],
            'last_update': None
        }
    
    def update_position(self, source_name: str,
                       measured_position: Tuple[float, float, float],
                       icq_score: Optional[float] = None,
                       variance: Optional[float] = None) -> Tuple[float, float, float]:
        """
        Update fused position with new measurement from a source.
        
        Args:
            source_name: Name of the positioning source
            measured_position: (x, y, z) measurement from source
            icq_score: Optional ICQ quality score for this measurement
            variance: Optional measurement variance (overrides default)
            
        Returns:
            Fused (x, y, z) position estimate
        """
        if source_name not in self.sources:
            self.register_source(source_name)
        
        source = self.sources[source_name]
        
        # Record measurement
        source['measurements'].append(measured_position)
        if len(source['measurements']) > 100:
            source['measurements'] = source['measurements'][-100:]
        
        # Record ICQ score
        if icq_score is not None:
            source['icq_scores'].append(icq_score)
            if len(source['icq_scores']) > 50:
                source['icq_scores'] = source['icq_scores'][-50:]
        
        # Update source variance if provided
        if variance is not None:
            source['variance'] = variance
        
        # Fuse using weighted average or Kalman filter
        fused_pos = self._fuse_weighted()
        
        # Record in history for overall system ICQ
        pos_int = tuple(int(p * 10) for p in fused_pos)  # Convert to integers
        self.position_history.append(pos_int)
        if len(self.position_history) > 200:
            self.position_history = self.position_history[-200:]
        
        self.position = np.array(fused_pos)
        
        return tuple(self.position)
    
    def _fuse_weighted(self) -> Tuple[float, float, float]:
        """
        Fuse positions using ICQ-weighted average.
        
        Returns:
            Fused position
        """
        weighted_sum = np.zeros(3)
        total_weight = 0.0
        
        for source_name, source in self.sources.items():
            if not source['measurements']:
                continue
            
            # Get latest measurement
            latest_pos = np.array(source['measurements'][-1])
            
            # Calculate weight based on ICQ and variance
            weight = 1.0 / (source['variance'] + 1e-6)
            
            # Multiply by ICQ if available
            if source['icq_scores']:
                avg_icq = np.mean(source['icq_scores'][-10:])  # Recent ICQ average
                weight *= (avg_icq + 0.1)  # Avoid zero weight
            
            weighted_sum += weight * latest_pos
            total_weight += weight
        
        if total_weight == 0:
            return tuple(self.position)  # Return previous estimate
        
        fused = weighted_sum / total_weight
        
        return tuple(fused)
    
    def _fuse_kalman(self, measurement: np.ndarray, 
                    measurement_variance: float) -> np.ndarray:
        """
        Update using Kalman filter (simplified version).
        
        Args:
            measurement: New position measurement
            measurement_variance: Measurement uncertainty
            
        Returns:
            Updated position estimate
        """
        # Prediction step (assuming constant position model)
        # In full implementation, would include process noise
        predicted_state = self.kalman_state
        predicted_covariance = self.kalman_covariance + np.eye(3) * 0.1
        
        # Update step
        measurement_matrix = np.eye(3)
        measurement_noise = np.eye(3) * measurement_variance
        
        # Innovation
        innovation = measurement - predicted_state
        innovation_covariance = predicted_covariance + measurement_noise
        
        # Kalman gain
        kalman_gain = predicted_covariance @ np.linalg.inv(innovation_covariance)
        
        # Update state and covariance
        self.kalman_state = predicted_state + kalman_gain @ innovation
        self.kalman_covariance = (np.eye(3) - kalman_gain) @ predicted_covariance
        
        return self.kalman_state
    
    def get_position_icq(self) -> Tuple[float, Dict]:
        """
        Calculate ICQ for the fused position stream.
        
        Returns:
            (ICQ value, diagnostics)
        """
        if len(self.position_history) < 20:
            return 0.0, {'error': 'Insufficient data'}
        
        # Calculate ICQ on x-coordinate history (as proxy for overall quality)
        x_coords = [pos[0] for pos in self.position_history]
        
        return self.icq_calc.calculate_icq(x_coords)
    
    def get_fusion_report(self) -> Dict:
        """
        Generate comprehensive fusion quality report.
        
        Returns:
            Dictionary with fusion statistics and source quality
        """
        report = {
            'current_position': tuple(self.position),
            'position_icq': self.get_position_icq()[0],
            'sources': {}
        }
        
        for source_name, source in self.sources.items():
            source_report = {
                'weight': source['weight'],
                'variance': source['variance'],
                'measurement_count': len(source['measurements']),
                'avg_icq': np.mean(source['icq_scores']) if source['icq_scores'] else 0.0,
                'active': bool(source['measurements'])
            }
            
            report['sources'][source_name] = source_report
        
        return report
    
    def get_uncertainty(self) -> Tuple[float, float, float]:
        """
        Get current position uncertainty estimate.
        
        Returns:
            (σx, σy, σz) standard deviations
        """
        # Return diagonal of covariance matrix
        return tuple(np.sqrt(np.diag(self.kalman_covariance)))
    
    def set_reference_position(self, position: Tuple[float, float, float]):
        """
        Set a known reference position (e.g., from external source).
        
        Args:
            position: (x, y, z) reference position
        """
        self.position = np.array(position)
        self.kalman_state = np.array(position)
        
        # Reset covariance to low uncertainty
        self.kalman_covariance = np.eye(3) * 1.0
    
    def reset(self):
        """Reset fusion system to initial state."""
        self.position = np.array([0.0, 0.0, 0.0])
        self.variance = np.array([100.0, 100.0, 100.0])
        self.position_history = []
        
        for source in self.sources.values():
            source['measurements'] = []
            source['icq_scores'] = []
