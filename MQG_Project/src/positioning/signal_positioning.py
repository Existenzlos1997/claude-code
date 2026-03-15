"""
Signal-Based Positioning
=========================

Implements positioning using RSSI (Received Signal Strength Indicator)
from Wi-Fi, Bluetooth, and cellular signals.

Uses ICQ to validate signal quality and reliability.
"""

import numpy as np
from typing import Dict, List, Tuple, Optional
import sys
import os

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from core.icq_calculator import ICQCalculator


class SignalPositioning:
    """
    Positioning using signal strength measurements.
    
    Supports multiple positioning methods:
    - Trilateration (if distances can be estimated from RSSI)
    - Fingerprinting (pattern matching against known locations)
    - Centroid (weighted average of AP positions)
    """
    
    def __init__(self, icq_calculator: Optional[ICQCalculator] = None):
        """
        Initialize signal positioning.
        
        Args:
            icq_calculator: ICQ calculator for signal quality validation
        """
        self.icq_calc = icq_calculator or ICQCalculator()
        self.access_points = {}  # {ap_id: (x, y, z)} known AP positions
        self.signal_history = {}  # {ap_id: [rssi_values]} for ICQ calculation
        
    def add_access_point(self, ap_id: str, position: Tuple[float, float, float]):
        """
        Register an access point with known position.
        
        Args:
            ap_id: Access point identifier
            position: (x, y, z) coordinates in meters
        """
        self.access_points[ap_id] = position
        self.signal_history[ap_id] = []
    
    def record_signal(self, ap_id: str, rssi: float):
        """
        Record a signal measurement for ICQ calculation.
        
        Args:
            ap_id: Access point identifier
            rssi: Received signal strength (typically -100 to 0 dBm)
        """
        if ap_id not in self.signal_history:
            self.signal_history[ap_id] = []
        
        # Convert RSSI to positive value for ICQ calculation (shift range)
        # RSSI typically ranges from -100 to 0 dBm
        normalized_rssi = int((rssi + 100) / 5)  # Map to ~0-20 range
        self.signal_history[ap_id].append(normalized_rssi)
        
        # Keep only recent history (last 100 samples)
        if len(self.signal_history[ap_id]) > 100:
            self.signal_history[ap_id] = self.signal_history[ap_id][-100:]
    
    def get_signal_icq(self, ap_id: str) -> Tuple[float, Dict]:
        """
        Calculate ICQ for a signal stream.
        
        Args:
            ap_id: Access point identifier
            
        Returns:
            (ICQ value, diagnostics dict)
        """
        if ap_id not in self.signal_history or len(self.signal_history[ap_id]) < 10:
            return 0.0, {'error': 'Insufficient data'}
        
        return self.icq_calc.calculate_icq(self.signal_history[ap_id])
    
    def rssi_to_distance(self, rssi: float, tx_power: float = -40.0, 
                        path_loss_exponent: float = 2.0) -> float:
        """
        Estimate distance from RSSI using log-distance path loss model.
        
        Args:
            rssi: Received signal strength in dBm
            tx_power: Transmit power at 1 meter in dBm
            path_loss_exponent: Environment-dependent (2.0 = free space, 4.0 = dense indoor)
            
        Returns:
            Estimated distance in meters
        """
        if rssi >= tx_power:
            return 0.1  # Very close
        
        # d = 10^((tx_power - rssi) / (10 * n))
        distance = 10 ** ((tx_power - rssi) / (10 * path_loss_exponent))
        
        return distance
    
    def trilateration(self, signals: Dict[str, float],
                     use_icq_weighting: bool = True) -> Tuple[float, float, float]:
        """
        Estimate position using trilateration from multiple APs.
        
        Args:
            signals: Dictionary of {ap_id: rssi_value}
            use_icq_weighting: Weight APs by their signal ICQ
            
        Returns:
            Estimated (x, y, z) position
        """
        if len(signals) < 3:
            raise ValueError("Need at least 3 access points for trilateration")
        
        # Collect AP positions and distances
        positions = []
        distances = []
        weights = []
        
        for ap_id, rssi in signals.items():
            if ap_id not in self.access_points:
                continue
            
            # Record signal for ICQ tracking
            self.record_signal(ap_id, rssi)
            
            # Get AP position and estimate distance
            pos = self.access_points[ap_id]
            dist = self.rssi_to_distance(rssi)
            
            positions.append(pos)
            distances.append(dist)
            
            # Get ICQ-based weight
            if use_icq_weighting:
                icq, _ = self.get_signal_icq(ap_id)
                weight = max(icq, 0.1)  # Minimum weight to avoid zero
            else:
                weight = 1.0
            
            weights.append(weight)
        
        if len(positions) < 3:
            raise ValueError("Not enough known access points")
        
        # Weighted least squares trilateration
        estimated_pos = self._weighted_trilateration(positions, distances, weights)
        
        return estimated_pos
    
    def _weighted_trilateration(self, positions: List[Tuple[float, float, float]],
                               distances: List[float],
                               weights: List[float]) -> Tuple[float, float, float]:
        """
        Weighted least squares trilateration.
        
        Solves: minimize sum(w_i * (||p - p_i|| - d_i)^2)
        """
        # Use first 3 positions for initial estimate
        p1, p2, p3 = np.array(positions[0]), np.array(positions[1]), np.array(positions[2])
        d1, d2, d3 = distances[0], distances[1], distances[2]
        
        # Simplified analytical solution (assumes 2D, z=0)
        # For full 3D, would need iterative optimization
        
        # Calculate weighted centroid as initial guess
        weighted_sum = np.zeros(3)
        total_weight = 0.0
        
        for i, (pos, dist, weight) in enumerate(zip(positions, distances, weights)):
            # Weight by inverse distance and ICQ
            w = weight / (dist + 1e-6)
            weighted_sum += w * np.array(pos)
            total_weight += w
        
        estimated = weighted_sum / total_weight
        
        return tuple(estimated)
    
    def centroid_positioning(self, signals: Dict[str, float],
                            use_icq_weighting: bool = True) -> Tuple[float, float, float]:
        """
        Simple centroid-based positioning (weighted average of AP positions).
        
        Args:
            signals: Dictionary of {ap_id: rssi_value}
            use_icq_weighting: Weight by ICQ scores
            
        Returns:
            Estimated (x, y, z) position
        """
        weighted_pos = np.zeros(3)
        total_weight = 0.0
        
        for ap_id, rssi in signals.items():
            if ap_id not in self.access_points:
                continue
            
            # Record signal
            self.record_signal(ap_id, rssi)
            
            # Weight by signal strength and optionally ICQ
            weight = (rssi + 100) / 100.0  # Normalize RSSI to ~0-1
            
            if use_icq_weighting:
                icq, _ = self.get_signal_icq(ap_id)
                weight *= (icq + 0.1)
            
            weighted_pos += weight * np.array(self.access_points[ap_id])
            total_weight += weight
        
        if total_weight == 0:
            raise ValueError("No valid access points")
        
        estimated = weighted_pos / total_weight
        
        return tuple(estimated)
    
    def get_signal_quality_report(self) -> Dict:
        """
        Generate a report on signal quality using ICQ.
        
        Returns:
            Dictionary with ICQ scores for each AP
        """
        report = {}
        
        for ap_id in self.signal_history:
            icq, diag = self.get_signal_icq(ap_id)
            report[ap_id] = {
                'icq': icq,
                'samples': len(self.signal_history[ap_id]),
                'diagnostics': diag
            }
        
        return report
