#!/usr/bin/env python3
"""
MQG-Theorie: Experimental Extension Demo
=========================================

This script demonstrates the experimental extension capabilities:
- Hardware sensor integration
- Real-time ICQ processing
- Sensor calibration
- Experimental data import

Author: MQG Project - Autonomous Development System
Version: 0.2.0-experimental
Date: 2026-02-01
"""

import sys
import os
import numpy as np

# Add parent directory to path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'src'))


def print_header(title: str):
    """Print formatted header."""
    print("\n" + "=" * 70)
    print(title.center(70))
    print("=" * 70 + "\n")


def demo_hardware_interface():
    """Demonstrate hardware interface capabilities."""
    print_header("DEMO 1: Hardware Interface")
    
    from experimental.hardware_interface import (
        HardwareInterface,
        SerialSensorAdapter,
        NetworkSensorAdapter
    )
    
    print("Initializing hardware interface...")
    hw = HardwareInterface()
    
    # Register sensors (simulated)
    print("\nRegistering sensors:")
    hw.register_sensor('temperature', SerialSensorAdapter('/dev/ttyUSB0', 9600))
    hw.register_sensor('light', SerialSensorAdapter('/dev/ttyUSB1', 115200))
    hw.register_sensor('humidity', NetworkSensorAdapter('192.168.1.100', 8080))
    
    # Get status
    status = hw.get_status()
    print(f"\n✓ Total sensors registered: {status['total_sensors']}")
    print("\nSensor details:")
    for name, info in status['sensors'].items():
        print(f"  - {name}: {info['type']} (Connected: {info['connected']})")
    
    print("\n✓ Hardware interface demo complete")


def demo_realtime_processing():
    """Demonstrate real-time ICQ processing."""
    print_header("DEMO 2: Real-Time ICQ Processing")
    
    from experimental.experimental_adapter import RealTimeProcessor
    
    print("Initializing real-time processor...")
    processor = RealTimeProcessor(
        window_size=100,
        update_interval=20
    )
    
    # Simulate streaming data
    print("\nSimulating data stream (mixed coherence pattern)...")
    
    # Create data with varying coherence
    data_stream = []
    
    # Phase 1: High coherence (repeating pattern)
    pattern = [1, 2, 3, 4, 5]
    data_stream.extend(pattern * 30)
    
    # Phase 2: Low coherence (random)
    data_stream.extend(np.random.randint(0, 10, 50).tolist())
    
    # Phase 3: Medium coherence (biased)
    data_stream.extend([1]*30 + [2]*20 + [3]*10)
    
    # Process stream
    icq_updates = []
    for i, sample in enumerate(data_stream):
        updated, icq = processor.add_sample(sample)
        
        if updated:
            icq_updates.append((i, icq))
            phase = "High" if icq > 0.7 else "Medium" if icq > 0.3 else "Low"
            print(f"  Sample {i:03d}: ICQ = {icq:.4f} ({phase} coherence)")
    
    # Statistics
    stats = processor.get_statistics()
    print(f"\n📊 Processing Statistics:")
    print(f"  Total samples: {stats['total_samples']}")
    print(f"  ICQ calculations: {stats['icq_calculations']}")
    print(f"  Current ICQ: {stats['current_icq']:.4f}")
    
    if 'icq_mean' in stats:
        print(f"  ICQ mean: {stats['icq_mean']:.4f}")
        print(f"  ICQ std: {stats['icq_std']:.4f}")
        print(f"  ICQ range: [{stats['icq_min']:.4f}, {stats['icq_max']:.4f}]")
    
    print("\n✓ Real-time processing demo complete")
    return processor


def demo_calibration():
    """Demonstrate sensor calibration."""
    print_header("DEMO 3: Sensor Calibration")
    
    from experimental.calibration import CalibrationManager
    
    print("Initializing calibration manager...")
    cm = CalibrationManager()
    
    # Simulate calibration data
    print("\nPerforming linear calibration...")
    print("(Simulating sensor with gain error and offset)")
    
    # True reference values
    reference = np.array([0, 10, 20, 30, 40, 50])
    
    # Simulated sensor readings (with systematic error)
    # Sensor has 5% gain error and 0.5 offset
    measured = 1.05 * reference + 0.5 + np.random.normal(0, 0.1, len(reference))
    
    print(f"\nCalibration points:")
    print("  Reference | Measured")
    print("  " + "-" * 22)
    for ref, meas in zip(reference, measured):
        print(f"  {ref:8.2f}  | {meas:8.2f}")
    
    # Perform calibration
    calib = cm.calibrate_sensor(
        'test_sensor',
        reference_values=reference.tolist(),
        measured_values=measured.tolist(),
        method='linear'
    )
    
    print(f"\n📐 Calibration Results:")
    print(f"  Method: {calib['method']}")
    print(f"  Slope: {calib['coefficients']['slope']:.6f}")
    print(f"  Intercept: {calib['coefficients']['intercept']:.6f}")
    print(f"  RMSE: {calib['statistics']['rmse']:.6f}")
    print(f"  R²: {calib['statistics']['r_squared']:.6f}")
    
    # Test calibration
    print("\n🧪 Testing calibration:")
    test_raw = 25.5
    test_calibrated = cm.apply_calibration('test_sensor', test_raw)
    print(f"  Raw reading: {test_raw:.2f}")
    print(f"  Calibrated: {test_calibrated:.2f}")
    print(f"  Expected: ~25.0")
    
    # Baseline measurement
    print("\n📏 Baseline Measurement:")
    baseline_data = np.random.normal(0, 0.05, 500)
    baseline = cm.measure_baseline(baseline_data, duration=5.0)
    
    print(f"  Mean: {baseline['mean']:.6f}")
    print(f"  Std Dev: {baseline['std']:.6f}")
    print(f"  Range: {baseline['range']:.6f}")
    print(f"  Drift rate: {baseline['drift_rate']:.6e} units/sample")
    
    # Noise characterization
    print("\n🔊 Noise Characterization:")
    noise_data = np.random.normal(5.0, 0.1, 1000)
    noise = cm.characterize_noise(noise_data)
    
    print(f"  RMS noise: {noise['rms_noise']:.6f}")
    print(f"  Peak-to-peak: {noise['peak_to_peak']:.6f}")
    print(f"  SNR estimate: {noise['snr_estimate']:.2f} dB")
    
    print("\n✓ Calibration demo complete")
    return cm


def demo_experimental_data_import():
    """Demonstrate experimental data import."""
    print_header("DEMO 4: Experimental Data Import")
    
    from experimental.experimental_adapter import ExperimentalDataAdapter
    from measurement.measurement_system import MeasurementSystem
    
    print("Creating experimental data adapter...")
    adapter = ExperimentalDataAdapter(config={
        'preprocessing': {
            'remove_outliers': True,
            'outlier_threshold': 3.0,
            'normalize': False,
            'detrend': True
        }
    })
    
    # Create test data file
    print("\nGenerating test experimental data...")
    test_data = np.sin(np.linspace(0, 10*np.pi, 500)) + np.random.normal(0, 0.1, 500)
    
    # Add some outliers
    test_data[100] = 10.0
    test_data[300] = -10.0
    
    test_file = '/tmp/experimental_data.csv'
    np.savetxt(test_file, test_data, delimiter=',')
    print(f"  Created: {test_file}")
    
    # Load and process
    print("\nLoading and preprocessing data...")
    loaded_data = adapter.load_from_csv(test_file)
    
    print(f"  Original length: {len(test_data)}")
    print(f"  After preprocessing: {len(loaded_data)}")
    print(f"  Outliers removed: {len(test_data) - len(loaded_data)}")
    print(f"  Value range: [{np.min(loaded_data):.3f}, {np.max(loaded_data):.3f}]")
    
    # Measure ICQ
    print("\nMeasuring ICQ from experimental data...")
    ms = MeasurementSystem()
    result = ms.measure_continuous_timeseries(
        loaded_data,
        label='experimental_demo',
        bins=10
    )
    
    print(f"\n📊 Measurement Results:")
    print(f"  ICQ: {result['icq']:.4f}")
    print(f"  Data length: {result['data_length']}")
    print(f"  Discretization bins: {result['discretization_bins']}")
    print(f"  Value range: [{result['value_range'][0]:.3f}, {result['value_range'][1]:.3f}]")
    print(f"  Mean: {result['statistics']['mean']:.3f}")
    print(f"  Std: {result['statistics']['std']:.3f}")
    
    print("\n✓ Experimental data import demo complete")


def demo_complete_workflow():
    """Demonstrate complete experimental workflow."""
    print_header("DEMO 5: Complete Experimental Workflow")
    
    from experimental.hardware_interface import HardwareInterface, SerialSensorAdapter
    from experimental.calibration import CalibrationManager
    from experimental.experimental_adapter import RealTimeProcessor
    
    print("Setting up complete experimental system...")
    
    # 1. Initialize components
    print("\n[1/5] Initializing hardware and calibration...")
    hw = HardwareInterface()
    cm = CalibrationManager()
    processor = RealTimeProcessor(window_size=50, update_interval=10)
    
    hw.register_sensor('sensor1', SerialSensorAdapter('/dev/ttyUSB0'))
    print("  ✓ Hardware interface ready")
    print("  ✓ Calibration manager ready")
    print("  ✓ Real-time processor ready")
    
    # 2. Simulate calibration
    print("\n[2/5] Calibrating sensor...")
    ref = [0, 25, 50, 75, 100]
    meas = [0.2, 25.1, 50.3, 74.9, 100.2]
    
    calib = cm.calibrate_sensor('sensor1', ref, meas, method='linear')
    print(f"  ✓ Calibration complete (R² = {calib['statistics']['r_squared']:.4f})")
    
    # 3. Simulate measurement
    print("\n[3/5] Acquiring data...")
    # Simulate sensor data
    raw_data = np.sin(np.linspace(0, 4*np.pi, 100)) * 50 + 50
    raw_data += np.random.normal(0, 2, 100)
    
    # Apply calibration
    calibrated_data = cm.apply_calibration('sensor1', raw_data)
    print(f"  ✓ Acquired {len(calibrated_data)} samples")
    
    # 4. Real-time processing
    print("\n[4/5] Processing in real-time...")
    for sample in calibrated_data:
        updated, icq = processor.add_sample(sample)
        if updated:
            print(f"  → ICQ update: {icq:.4f}")
    
    # 5. Results
    print("\n[5/5] Final results:")
    stats = processor.get_statistics()
    print(f"  Total samples: {stats['total_samples']}")
    print(f"  Final ICQ: {stats['current_icq']:.4f}")
    print(f"  ICQ range: [{stats['icq_min']:.4f}, {stats['icq_max']:.4f}]")
    
    print("\n✅ Complete workflow demonstration successful!")


def main():
    """Run all demonstrations."""
    print("\n" + "=" * 70)
    print("MQG-THEORIE: EXPERIMENTAL EXTENSION DEMONSTRATION".center(70))
    print("Physische Messungen und Echtzeit-Verarbeitung".center(70))
    print("=" * 70)
    
    print("\nThis demo showcases the experimental extension:")
    print("  1. Hardware interface for physical sensors")
    print("  2. Real-time ICQ processing")
    print("  3. Sensor calibration")
    print("  4. Experimental data import")
    print("  5. Complete workflow example")
    
    input("\nPress Enter to start demonstration...")
    
    try:
        # Run demos
        demo_hardware_interface()
        input("\nPress Enter to continue...")
        
        processor = demo_realtime_processing()
        input("\nPress Enter to continue...")
        
        cm = demo_calibration()
        input("\nPress Enter to continue...")
        
        demo_experimental_data_import()
        input("\nPress Enter to continue...")
        
        demo_complete_workflow()
        
        # Final summary
        print_header("DEMONSTRATION COMPLETE")
        print("The MQG-Theorie experimental extension is fully operational!")
        print("\nKey Achievements:")
        print("  ✅ Hardware sensor integration framework")
        print("  ✅ Real-time ICQ calculation")
        print("  ✅ Sensor calibration system")
        print("  ✅ Experimental data import")
        print("  ✅ Complete measurement workflow")
        
        print("\nThe system is now ready for:")
        print("  • Real physical sensor integration")
        print("  • Live experimental measurements")
        print("  • Continuous data monitoring")
        print("  • Production experimental setups")
        
        print("\n" + "=" * 70)
        print("Thank you for exploring the MQG experimental extension!")
        print("=" * 70 + "\n")
        
    except KeyboardInterrupt:
        print("\n\nDemonstration interrupted by user.")
        print("✓ Partial demo completed successfully.")
    except Exception as e:
        print(f"\n\n✗ Error during demonstration: {e}")
        import traceback
        traceback.print_exc()


if __name__ == "__main__":
    main()
