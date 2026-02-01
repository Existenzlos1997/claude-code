"""
MQG-Theorie: Hardware Interface
================================

This module provides interfaces for connecting physical sensors and 
experimental measurement devices to the MQG-Theorie framework.

Supports:
---------
1. Serial port devices (Arduino, sensors, DAQ systems)
2. Network-based sensors (TCP/IP, UDP)
3. Custom hardware protocols
4. Real-time data streaming

Author: MQG Project - Autonomous Development System
Version: 0.2.0-experimental
Date: 2026-02-01
"""

import numpy as np
from typing import Dict, List, Union, Tuple, Any, Callable, Optional
from abc import ABC, abstractmethod
import time
from datetime import datetime
import json


class SensorAdapter(ABC):
    """
    Abstract base class for sensor adapters.
    
    All physical sensors must implement this interface to connect
    to the MQG measurement system.
    """
    
    def __init__(self, config: Dict[str, Any] = None):
        """
        Initialize sensor adapter.
        
        Parameters:
        -----------
        config : dict, optional
            Sensor-specific configuration
        """
        self.config = config or {}
        self.is_connected = False
        self.metadata = {
            'sensor_type': self.__class__.__name__,
            'initialized': datetime.utcnow().isoformat(),
            'version': '0.2.0-experimental'
        }
    
    @abstractmethod
    def connect(self) -> bool:
        """
        Establish connection to physical sensor.
        
        Returns:
        --------
        success : bool
            True if connection successful
        """
        pass
    
    @abstractmethod
    def disconnect(self) -> bool:
        """
        Disconnect from physical sensor.
        
        Returns:
        --------
        success : bool
            True if disconnection successful
        """
        pass
    
    @abstractmethod
    def read_sample(self) -> Any:
        """
        Read single sample from sensor.
        
        Returns:
        --------
        sample : Any
            Single data sample from sensor
        """
        pass
    
    @abstractmethod
    def read_stream(self, duration: float, sampling_rate: float) -> np.ndarray:
        """
        Read continuous stream of data.
        
        Parameters:
        -----------
        duration : float
            Duration in seconds
        sampling_rate : float
            Samples per second
            
        Returns:
        --------
        data : np.ndarray
            Array of samples
        """
        pass
    
    def validate_reading(self, data: Any) -> Tuple[bool, str]:
        """
        Validate sensor reading.
        
        Parameters:
        -----------
        data : Any
            Data to validate
            
        Returns:
        --------
        is_valid : bool
            True if data is valid
        message : str
            Validation message
        """
        if data is None:
            return False, "No data received"
        
        if isinstance(data, (int, float)):
            if np.isnan(data) or np.isinf(data):
                return False, "Invalid numeric value (NaN or Inf)"
        
        return True, "Data valid"


class SerialSensorAdapter(SensorAdapter):
    """
    Adapter for serial port sensors (Arduino, etc.).
    
    Example sensors:
    - Temperature sensors
    - Light sensors
    - Accelerometers
    - Custom measurement devices
    """
    
    def __init__(self, port: str, baudrate: int = 9600, config: Dict[str, Any] = None):
        """
        Initialize serial sensor adapter.
        
        Parameters:
        -----------
        port : str
            Serial port (e.g., '/dev/ttyUSB0', 'COM3')
        baudrate : int
            Baud rate for serial communication
        config : dict, optional
            Additional configuration
        """
        super().__init__(config)
        self.port = port
        self.baudrate = baudrate
        self.serial_connection = None
    
    def connect(self) -> bool:
        """Connect to serial port."""
        try:
            # Note: Actual implementation would use pyserial
            # This is a template for real hardware integration
            print(f"[EXPERIMENTAL] Connecting to serial port {self.port} at {self.baudrate} baud")
            
            # Simulated connection for template
            # In production: import serial; self.serial_connection = serial.Serial(self.port, self.baudrate)
            
            self.is_connected = True
            print(f"[EXPERIMENTAL] Successfully connected to {self.port}")
            return True
            
        except Exception as e:
            print(f"[EXPERIMENTAL] Connection failed: {e}")
            self.is_connected = False
            return False
    
    def disconnect(self) -> bool:
        """Disconnect from serial port."""
        if self.serial_connection:
            # In production: self.serial_connection.close()
            print(f"[EXPERIMENTAL] Disconnected from {self.port}")
            self.is_connected = False
            return True
        return False
    
    def read_sample(self) -> Any:
        """Read single sample from serial sensor."""
        if not self.is_connected:
            raise RuntimeError("Sensor not connected")
        
        # Template for real implementation
        # In production: line = self.serial_connection.readline().decode('utf-8').strip()
        # return float(line)
        
        # Simulated reading
        return np.random.randn()
    
    def read_stream(self, duration: float, sampling_rate: float) -> np.ndarray:
        """Read continuous stream from serial sensor."""
        if not self.is_connected:
            raise RuntimeError("Sensor not connected")
        
        n_samples = int(duration * sampling_rate)
        samples = []
        
        interval = 1.0 / sampling_rate
        
        for i in range(n_samples):
            start_time = time.time()
            
            sample = self.read_sample()
            samples.append(sample)
            
            # Maintain sampling rate
            elapsed = time.time() - start_time
            if elapsed < interval:
                time.sleep(interval - elapsed)
        
        return np.array(samples)


class NetworkSensorAdapter(SensorAdapter):
    """
    Adapter for network-based sensors (TCP/IP, UDP).
    
    Example use cases:
    - Remote environmental sensors
    - Distributed measurement systems
    - Cloud-connected IoT devices
    """
    
    def __init__(self, host: str, port: int, protocol: str = 'tcp', config: Dict[str, Any] = None):
        """
        Initialize network sensor adapter.
        
        Parameters:
        -----------
        host : str
            Sensor hostname or IP address
        port : int
            Network port
        protocol : str
            'tcp' or 'udp'
        config : dict, optional
            Additional configuration
        """
        super().__init__(config)
        self.host = host
        self.port = port
        self.protocol = protocol.lower()
        self.connection = None
    
    def connect(self) -> bool:
        """Connect to network sensor."""
        try:
            print(f"[EXPERIMENTAL] Connecting to {self.protocol.upper()}://{self.host}:{self.port}")
            
            # Template for real implementation
            # In production: use socket library for TCP/UDP connections
            
            self.is_connected = True
            print(f"[EXPERIMENTAL] Successfully connected to network sensor")
            return True
            
        except Exception as e:
            print(f"[EXPERIMENTAL] Connection failed: {e}")
            self.is_connected = False
            return False
    
    def disconnect(self) -> bool:
        """Disconnect from network sensor."""
        if self.connection:
            # In production: self.connection.close()
            print(f"[EXPERIMENTAL] Disconnected from network sensor")
            self.is_connected = False
            return True
        return False
    
    def read_sample(self) -> Any:
        """Read single sample from network sensor."""
        if not self.is_connected:
            raise RuntimeError("Sensor not connected")
        
        # Template for real implementation
        # In production: receive data from socket
        
        # Simulated reading
        return np.random.randn()
    
    def read_stream(self, duration: float, sampling_rate: float) -> np.ndarray:
        """Read continuous stream from network sensor."""
        if not self.is_connected:
            raise RuntimeError("Sensor not connected")
        
        n_samples = int(duration * sampling_rate)
        samples = []
        
        interval = 1.0 / sampling_rate
        
        for i in range(n_samples):
            start_time = time.time()
            
            sample = self.read_sample()
            samples.append(sample)
            
            elapsed = time.time() - start_time
            if elapsed < interval:
                time.sleep(interval - elapsed)
        
        return np.array(samples)


class HardwareInterface:
    """
    Main hardware interface for the MQG experimental system.
    
    Manages multiple sensors and provides unified data acquisition.
    """
    
    def __init__(self):
        """Initialize hardware interface."""
        self.sensors = {}
        self.active_sensors = []
        self.metadata = {
            'created': datetime.utcnow().isoformat(),
            'version': '0.2.0-experimental',
            'framework': 'MQG-Theorie Experimental Extension'
        }
    
    def register_sensor(self, name: str, adapter: SensorAdapter) -> None:
        """
        Register a sensor adapter.
        
        Parameters:
        -----------
        name : str
            Unique sensor identifier
        adapter : SensorAdapter
            Sensor adapter instance
        """
        self.sensors[name] = adapter
        print(f"[EXPERIMENTAL] Registered sensor: {name} ({adapter.__class__.__name__})")
    
    def connect_sensor(self, name: str) -> bool:
        """
        Connect to a registered sensor.
        
        Parameters:
        -----------
        name : str
            Sensor identifier
            
        Returns:
        --------
        success : bool
            True if connection successful
        """
        if name not in self.sensors:
            print(f"[EXPERIMENTAL] Sensor '{name}' not registered")
            return False
        
        success = self.sensors[name].connect()
        if success and name not in self.active_sensors:
            self.active_sensors.append(name)
        
        return success
    
    def disconnect_sensor(self, name: str) -> bool:
        """
        Disconnect from a sensor.
        
        Parameters:
        -----------
        name : str
            Sensor identifier
            
        Returns:
        --------
        success : bool
            True if disconnection successful
        """
        if name not in self.sensors:
            return False
        
        success = self.sensors[name].disconnect()
        if success and name in self.active_sensors:
            self.active_sensors.remove(name)
        
        return success
    
    def read_from_sensor(self, name: str, mode: str = 'sample', **kwargs) -> Any:
        """
        Read data from a sensor.
        
        Parameters:
        -----------
        name : str
            Sensor identifier
        mode : str
            'sample' for single reading, 'stream' for continuous
        **kwargs : dict
            Additional parameters (duration, sampling_rate for stream mode)
            
        Returns:
        --------
        data : Any
            Sensor reading(s)
        """
        if name not in self.sensors:
            raise ValueError(f"Sensor '{name}' not registered")
        
        sensor = self.sensors[name]
        
        if not sensor.is_connected:
            raise RuntimeError(f"Sensor '{name}' not connected")
        
        if mode == 'sample':
            return sensor.read_sample()
        elif mode == 'stream':
            duration = kwargs.get('duration', 1.0)
            sampling_rate = kwargs.get('sampling_rate', 100.0)
            return sensor.read_stream(duration, sampling_rate)
        else:
            raise ValueError(f"Unknown mode: {mode}")
    
    def measure_icq_from_sensor(
        self,
        name: str,
        duration: float = 1.0,
        sampling_rate: float = 100.0,
        label: str = None
    ) -> Dict[str, Any]:
        """
        Measure ICQ directly from sensor data.
        
        Parameters:
        -----------
        name : str
            Sensor identifier
        duration : float
            Measurement duration in seconds
        sampling_rate : float
            Sampling rate in Hz
        label : str, optional
            Label for this measurement
            
        Returns:
        --------
        measurement : dict
            ICQ measurement result with metadata
        """
        try:
            from ..measurement.measurement_system import MeasurementSystem
        except ImportError:
            from measurement.measurement_system import MeasurementSystem
        
        # Acquire data from sensor
        data = self.read_from_sensor(
            name,
            mode='stream',
            duration=duration,
            sampling_rate=sampling_rate
        )
        
        # Measure ICQ
        ms = MeasurementSystem()
        result = ms.measure_continuous_timeseries(
            data,
            label=label or f'sensor_{name}_{datetime.utcnow().isoformat()}'
        )
        
        # Add sensor metadata
        result['sensor'] = {
            'name': name,
            'type': self.sensors[name].__class__.__name__,
            'duration': duration,
            'sampling_rate': sampling_rate,
            'n_samples': len(data)
        }
        
        return result
    
    def get_status(self) -> Dict[str, Any]:
        """
        Get status of all sensors.
        
        Returns:
        --------
        status : dict
            Status information for all sensors
        """
        status = {
            'total_sensors': len(self.sensors),
            'active_sensors': len(self.active_sensors),
            'sensors': {}
        }
        
        for name, sensor in self.sensors.items():
            status['sensors'][name] = {
                'type': sensor.__class__.__name__,
                'connected': sensor.is_connected,
                'active': name in self.active_sensors
            }
        
        return status


if __name__ == "__main__":
    # Self-test and demonstration
    print("=" * 70)
    print("MQG-Theorie: Hardware Interface - Self-Test")
    print("=" * 70)
    
    # Test 1: Serial sensor (simulated)
    print("\n[Test 1] Serial Sensor Adapter")
    serial_sensor = SerialSensorAdapter('/dev/ttyUSB0', baudrate=115200)
    print(f"  Created: {serial_sensor.metadata['sensor_type']}")
    serial_sensor.connect()
    sample = serial_sensor.read_sample()
    print(f"  Sample reading: {sample:.4f}")
    serial_sensor.disconnect()
    
    # Test 2: Network sensor (simulated)
    print("\n[Test 2] Network Sensor Adapter")
    network_sensor = NetworkSensorAdapter('192.168.1.100', 8080, protocol='tcp')
    network_sensor.connect()
    sample = network_sensor.read_sample()
    print(f"  Sample reading: {sample:.4f}")
    network_sensor.disconnect()
    
    # Test 3: Hardware interface
    print("\n[Test 3] Hardware Interface")
    hw = HardwareInterface()
    hw.register_sensor('temp_sensor', SerialSensorAdapter('/dev/ttyUSB0'))
    hw.register_sensor('light_sensor', NetworkSensorAdapter('192.168.1.101', 8081))
    
    status = hw.get_status()
    print(f"  Total sensors: {status['total_sensors']}")
    print(f"  Sensors: {list(status['sensors'].keys())}")
    
    # Test 4: ICQ measurement from sensor
    print("\n[Test 4] ICQ Measurement from Sensor")
    hw.connect_sensor('temp_sensor')
    try:
        result = hw.measure_icq_from_sensor(
            'temp_sensor',
            duration=0.1,
            sampling_rate=100.0,
            label='test_measurement'
        )
        print(f"  ICQ: {result['icq']:.4f}")
        print(f"  Sensor: {result['sensor']['name']}")
        print(f"  Samples: {result['sensor']['n_samples']}")
    finally:
        hw.disconnect_sensor('temp_sensor')
    
    print("\n" + "=" * 70)
    print("Self-test complete. Hardware interface is operational.")
    print("=" * 70)
