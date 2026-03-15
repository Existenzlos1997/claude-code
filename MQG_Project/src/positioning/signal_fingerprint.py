"""
Signal Fingerprint Database
============================

Stores and manages signal fingerprints for location-based positioning.
Uses RSSI (Received Signal Strength Indicator) patterns from Wi-Fi,
Bluetooth, and cellular signals to identify locations.

The ICQ is used to validate the consistency and quality of signal patterns.
"""

import numpy as np
from typing import Dict, List, Tuple, Optional
import json


class SignalFingerprintDB:
    """
    Database for storing and matching signal fingerprints.
    
    Each fingerprint contains:
    - Location coordinates (x, y, z or lat, lon, alt)
    - Signal measurements from multiple access points/beacons
    - ICQ score for signal quality validation
    """
    
    def __init__(self):
        """Initialize the fingerprint database."""
        self.fingerprints = []  # List of fingerprint entries
        self.access_points = set()  # Set of all known APs
        
    def add_fingerprint(self, location: Tuple[float, float, float], 
                       signals: Dict[str, float],
                       icq_score: float = None) -> int:
        """
        Add a signal fingerprint to the database.
        
        Args:
            location: (x, y, z) coordinates in meters or (lat, lon, alt)
            signals: Dictionary of {ap_id: rssi_value}
            icq_score: Optional ICQ quality score for this fingerprint
            
        Returns:
            Index of the added fingerprint
        """
        fingerprint = {
            'location': location,
            'signals': signals.copy(),
            'icq_score': icq_score,
            'timestamp': None  # Could add timestamp if needed
        }
        
        self.fingerprints.append(fingerprint)
        self.access_points.update(signals.keys())
        
        return len(self.fingerprints) - 1
    
    def find_matches(self, observed_signals: Dict[str, float],
                    k: int = 5,
                    min_icq: float = 0.0) -> List[Tuple[int, float, Tuple[float, float, float]]]:
        """
        Find k nearest fingerprints based on signal similarity.
        
        Args:
            observed_signals: Current signal measurements {ap_id: rssi}
            k: Number of nearest neighbors to return
            min_icq: Minimum ICQ score to consider (filter low-quality fingerprints)
            
        Returns:
            List of (index, similarity_score, location) tuples
        """
        if not self.fingerprints:
            return []
        
        matches = []
        
        for idx, fingerprint in enumerate(self.fingerprints):
            # Skip if ICQ score too low
            if fingerprint['icq_score'] is not None and fingerprint['icq_score'] < min_icq:
                continue
            
            # Calculate similarity using Euclidean distance in RSSI space
            similarity = self._calculate_similarity(observed_signals, fingerprint['signals'])
            
            matches.append((idx, similarity, fingerprint['location']))
        
        # Sort by similarity (lower distance = higher similarity)
        matches.sort(key=lambda x: x[1])
        
        return matches[:k]
    
    def _calculate_similarity(self, signals1: Dict[str, float], 
                             signals2: Dict[str, float]) -> float:
        """
        Calculate similarity between two signal sets using Euclidean distance.
        
        Args:
            signals1: First signal set
            signals2: Second signal set
            
        Returns:
            Distance (lower = more similar)
        """
        # Find common access points
        common_aps = set(signals1.keys()) & set(signals2.keys())
        
        if not common_aps:
            return float('inf')  # No common signals
        
        # Calculate Euclidean distance for common APs
        distance_sq = 0.0
        for ap_id in common_aps:
            diff = signals1[ap_id] - signals2[ap_id]
            distance_sq += diff * diff
        
        # Penalize for missing signals
        missing_penalty = 1000.0  # Large penalty for each missing AP
        all_aps = set(signals1.keys()) | set(signals2.keys())
        missing_count = len(all_aps) - len(common_aps)
        
        distance = np.sqrt(distance_sq) + missing_penalty * missing_count
        
        return distance
    
    def estimate_position(self, observed_signals: Dict[str, float],
                         k: int = 3,
                         use_icq_weighting: bool = True) -> Tuple[float, float, float]:
        """
        Estimate position using k-nearest neighbors with optional ICQ weighting.
        
        Args:
            observed_signals: Current signal measurements
            k: Number of neighbors to use
            use_icq_weighting: If True, weight by ICQ scores
            
        Returns:
            Estimated (x, y, z) position
        """
        matches = self.find_matches(observed_signals, k=k)
        
        if not matches:
            raise ValueError("No matching fingerprints found")
        
        # Weighted average of k nearest neighbors
        total_weight = 0.0
        weighted_pos = np.array([0.0, 0.0, 0.0])
        
        for idx, similarity, location in matches:
            # Weight by inverse distance (closer = higher weight)
            weight = 1.0 / (similarity + 1e-6)
            
            # Additional weighting by ICQ if enabled
            if use_icq_weighting:
                icq = self.fingerprints[idx]['icq_score']
                if icq is not None:
                    weight *= (icq + 0.1)  # Avoid zero weight
            
            weighted_pos += weight * np.array(location)
            total_weight += weight
        
        estimated_pos = weighted_pos / total_weight
        
        return tuple(estimated_pos)
    
    def save_to_file(self, filename: str):
        """Save database to JSON file."""
        data = {
            'fingerprints': self.fingerprints,
            'access_points': list(self.access_points)
        }
        with open(filename, 'w') as f:
            json.dump(data, f, indent=2)
    
    def load_from_file(self, filename: str):
        """Load database from JSON file."""
        with open(filename, 'r') as f:
            data = json.load(f)
        
        self.fingerprints = data['fingerprints']
        self.access_points = set(data['access_points'])
    
    def get_statistics(self) -> Dict:
        """Get database statistics."""
        return {
            'total_fingerprints': len(self.fingerprints),
            'total_access_points': len(self.access_points),
            'avg_icq_score': np.mean([fp['icq_score'] for fp in self.fingerprints 
                                     if fp['icq_score'] is not None]) if self.fingerprints else 0.0
        }
