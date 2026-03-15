"""
MQG-Based GPS-Free Positioning System
======================================

This module implements a positioning system without GPS using:
- Signal-based positioning (Wi-Fi, Bluetooth, Cellular)
- Inertial navigation (IMU sensors)
- ICQ-based signal quality validation
- Multi-source sensor fusion

The ICQ (Information Coherence Quotient) is used to:
1. Validate signal quality and reliability
2. Weight different positioning sources
3. Detect anomalies in sensor data
4. Improve overall positioning accuracy

Author: MQG Project
Version: 0.3.0-positioning
Date: 2026-02-06
"""

from .signal_positioning import SignalPositioning
from .inertial_navigation import InertialNavigator
from .position_fusion import PositionFusion
from .signal_fingerprint import SignalFingerprintDB

__all__ = [
    'SignalPositioning',
    'InertialNavigator',
    'PositionFusion',
    'SignalFingerprintDB'
]
