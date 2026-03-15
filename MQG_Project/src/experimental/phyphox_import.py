"""
phyphox Data Import Module for MQG Theory

This module provides tools to import and analyze data exported from the phyphox app.
phyphox (Physical Phone Experiments) is a professional sensor data collection app
available for iOS and Android.

Features:
- Import phyphox CSV exports
- Import phyphox Excel exports
- Automatic sensor type detection
- ICQ calculation from imported data
- Support for accelerometer, gyroscope, magnetometer, and more

Author: MQG Project
Version: 1.0.0
"""

import numpy as np
import pandas as pd
from typing import Dict, List, Tuple, Optional, Union
from pathlib import Path
import json

# Try to import ICQ calculator
try:
    from ..core.icq_calculator import ICQCalculator
except ImportError:
    # Fallback for standalone use
    import sys
    sys.path.append(str(Path(__file__).parent.parent))
    from core.icq_calculator import ICQCalculator


class PhyphoxDataImporter:
    """
    Import and analyze data from phyphox app exports.
    
    Supports multiple export formats:
    - CSV (comma-separated)
    - CSV (tab-separated)
    - Excel (.xlsx)
    
    Automatically detects:
    - Sensor type (accelerometer, gyroscope, magnetometer, etc.)
    - Column structure
    - Time format
    """
    
    def __init__(self):
        """Initialize the phyphox data importer."""
        self.icq_calculator = ICQCalculator()
        self.data = None
        self.metadata = {}
        
    def import_csv(self, filepath: str, delimiter: str = None) -> Dict:
        """
        Import phyphox CSV export file.
        
        Args:
            filepath: Path to the CSV file
            delimiter: Column delimiter (None = auto-detect, ',' or '\t')
            
        Returns:
            Dictionary containing:
            - 'time': Time values
            - 'data': Sensor data (dict with x, y, z components if applicable)
            - 'sensor_type': Detected sensor type
            - 'metadata': Additional information
        """
        filepath = Path(filepath)
        if not filepath.exists():
            raise FileNotFoundError(f"File not found: {filepath}")
            
        # Auto-detect delimiter if not specified
        if delimiter is None:
            with open(filepath, 'r') as f:
                first_line = f.readline()
                delimiter = '\t' if '\t' in first_line else ','
        
        # Read CSV file
        try:
            df = pd.read_csv(filepath, delimiter=delimiter)
        except Exception as e:
            raise ValueError(f"Failed to read CSV file: {e}")
        
        # Detect sensor type and extract data
        result = self._parse_dataframe(df, filepath.name)
        self.data = result
        
        return result
    
    def import_excel(self, filepath: str, sheet_name: Union[str, int] = 0) -> Dict:
        """
        Import phyphox Excel export file.
        
        Args:
            filepath: Path to the Excel file
            sheet_name: Sheet name or index (default: 0 = first sheet)
            
        Returns:
            Dictionary containing sensor data and metadata
        """
        filepath = Path(filepath)
        if not filepath.exists():
            raise FileNotFoundError(f"File not found: {filepath}")
            
        try:
            df = pd.read_excel(filepath, sheet_name=sheet_name)
        except Exception as e:
            raise ValueError(f"Failed to read Excel file: {e}")
        
        # Parse dataframe
        result = self._parse_dataframe(df, filepath.name)
        self.data = result
        
        return result
    
    def _parse_dataframe(self, df: pd.DataFrame, filename: str) -> Dict:
        """
        Parse a pandas DataFrame from phyphox export.
        
        Automatically detects:
        - Time column
        - Sensor type
        - Data columns
        """
        # Detect time column
        time_col = None
        for col in df.columns:
            col_lower = col.lower()
            if 'time' in col_lower or 't (' in col_lower:
                time_col = col
                break
        
        if time_col is None:
            # Create time from index if no time column
            time = np.arange(len(df)) * 0.01  # Assume 100 Hz
        else:
            time = df[time_col].values
        
        # Detect sensor type and extract data
        sensor_type = 'unknown'
        data_dict = {}
        
        # Check for accelerometer
        accel_cols = [col for col in df.columns if 'acceleration' in col.lower()]
        if accel_cols:
            sensor_type = 'accelerometer'
            data_dict = self._extract_xyz_data(df, accel_cols, 'acceleration')
        
        # Check for gyroscope
        gyro_cols = [col for col in df.columns if 'gyro' in col.lower()]
        if gyro_cols:
            if sensor_type == 'accelerometer':
                sensor_type = 'imu'  # Combined IMU data
            else:
                sensor_type = 'gyroscope'
            data_dict.update(self._extract_xyz_data(df, gyro_cols, 'gyroscope'))
        
        # Check for magnetometer
        mag_cols = [col for col in df.columns if 'magnet' in col.lower()]
        if mag_cols:
            data_dict.update(self._extract_xyz_data(df, mag_cols, 'magnetometer'))
            if sensor_type in ['unknown', 'accelerometer', 'gyroscope']:
                sensor_type = 'magnetometer' if sensor_type == 'unknown' else sensor_type
        
        # If still unknown, use all numeric columns
        if sensor_type == 'unknown':
            numeric_cols = df.select_dtypes(include=[np.number]).columns
            numeric_cols = [col for col in numeric_cols if col != time_col]
            for col in numeric_cols:
                data_dict[col] = df[col].values
            sensor_type = 'generic'
        
        # Build result dictionary
        result = {
            'time': time,
            'data': data_dict,
            'sensor_type': sensor_type,
            'metadata': {
                'filename': filename,
                'samples': len(time),
                'duration': float(time[-1] - time[0]) if len(time) > 1 else 0,
                'sampling_rate': len(time) / (time[-1] - time[0]) if len(time) > 1 and time[-1] != time[0] else 0
            }
        }
        
        return result
    
    def _extract_xyz_data(self, df: pd.DataFrame, columns: List[str], prefix: str) -> Dict:
        """
        Extract X, Y, Z components from columns.
        
        Args:
            df: DataFrame
            columns: List of column names
            prefix: Prefix for output keys (e.g., 'acceleration', 'gyroscope')
            
        Returns:
            Dictionary with x, y, z keys
        """
        data = {}
        
        # Find X, Y, Z components
        for axis in ['x', 'y', 'z']:
            for col in columns:
                col_lower = col.lower()
                if f' {axis}' in col_lower or f'_{axis}' in col_lower or col_lower.endswith(axis):
                    data[f'{prefix}_{axis}'] = df[col].values
                    break
        
        return data
    
    def calculate_icq(self, data_key: str = None) -> Dict[str, float]:
        """
        Calculate ICQ for imported data.
        
        Args:
            data_key: Specific data key to analyze (e.g., 'acceleration_x')
                     If None, calculates ICQ for all available data
                     
        Returns:
            Dictionary mapping data keys to ICQ values
        """
        if self.data is None:
            raise ValueError("No data imported. Call import_csv() or import_excel() first.")
        
        results = {}
        
        if data_key is not None:
            # Calculate ICQ for specific key
            if data_key not in self.data['data']:
                raise ValueError(f"Data key '{data_key}' not found. Available: {list(self.data['data'].keys())}")
            
            data = self.data['data'][data_key]
            icq, _ = self.icq_calculator.calculate_icq(data.tolist())
            results[data_key] = icq
        else:
            # Calculate ICQ for all data keys
            for key, data in self.data['data'].items():
                icq, _ = self.icq_calculator.calculate_icq(data.tolist())
                results[key] = icq
        
        return results
    
    def get_summary(self) -> str:
        """
        Get a human-readable summary of imported data.
        
        Returns:
            Formatted summary string
        """
        if self.data is None:
            return "No data imported."
        
        lines = []
        lines.append("=" * 60)
        lines.append("phyphox Data Import Summary")
        lines.append("=" * 60)
        lines.append(f"File: {self.data['metadata']['filename']}")
        lines.append(f"Sensor Type: {self.data['sensor_type']}")
        lines.append(f"Samples: {self.data['metadata']['samples']}")
        lines.append(f"Duration: {self.data['metadata']['duration']:.2f} seconds")
        lines.append(f"Sampling Rate: {self.data['metadata']['sampling_rate']:.1f} Hz")
        lines.append("")
        lines.append("Data Channels:")
        for key in self.data['data'].keys():
            lines.append(f"  - {key}")
        lines.append("")
        
        # Calculate and display ICQ values
        icq_results = self.calculate_icq()
        lines.append("ICQ Values:")
        for key, icq in icq_results.items():
            quality = self._interpret_icq(icq)
            lines.append(f"  - {key}: {icq:.3f} ({quality})")
        
        lines.append("=" * 60)
        
        return "\n".join(lines)
    
    def _interpret_icq(self, icq: float) -> str:
        """Interpret ICQ value as quality level."""
        if icq >= 0.8:
            return "Ausgezeichnet"
        elif icq >= 0.6:
            return "Gut"
        elif icq >= 0.4:
            return "Mittel"
        elif icq >= 0.2:
            return "Niedrig"
        else:
            return "Sehr niedrig"
    
    def export_results(self, output_path: str, format: str = 'json'):
        """
        Export analysis results.
        
        Args:
            output_path: Path for output file
            format: Output format ('json' or 'txt')
        """
        if self.data is None:
            raise ValueError("No data to export.")
        
        icq_results = self.calculate_icq()
        
        results = {
            'metadata': self.data['metadata'],
            'sensor_type': self.data['sensor_type'],
            'icq_values': icq_results,
            'interpretation': {key: self._interpret_icq(icq) 
                             for key, icq in icq_results.items()}
        }
        
        if format == 'json':
            with open(output_path, 'w') as f:
                json.dump(results, f, indent=2)
        elif format == 'txt':
            with open(output_path, 'w') as f:
                f.write(self.get_summary())
        else:
            raise ValueError(f"Unsupported format: {format}")


def quick_analyze(filepath: str) -> Dict:
    """
    Quick analysis of a phyphox export file.
    
    Args:
        filepath: Path to phyphox CSV or Excel file
        
    Returns:
        Dictionary with ICQ results and metadata
    """
    importer = PhyphoxDataImporter()
    
    # Detect file type and import
    filepath = Path(filepath)
    if filepath.suffix.lower() in ['.xlsx', '.xls']:
        importer.import_excel(str(filepath))
    else:
        importer.import_csv(str(filepath))
    
    # Calculate ICQ
    icq_results = importer.calculate_icq()
    
    return {
        'metadata': importer.data['metadata'],
        'sensor_type': importer.data['sensor_type'],
        'icq_values': icq_results,
        'summary': importer.get_summary()
    }


if __name__ == '__main__':
    # Example usage
    print("phyphox Data Import Module for MQG Theory")
    print("=" * 60)
    print()
    print("This module allows you to import data from the phyphox app")
    print("and calculate ICQ (Information Coherence Quotient) values.")
    print()
    print("Example usage:")
    print()
    print("  from phyphox_import import PhyphoxDataImporter")
    print()
    print("  # Import CSV file")
    print("  importer = PhyphoxDataImporter()")
    print("  importer.import_csv('acceleration_data.csv')")
    print()
    print("  # Calculate ICQ")
    print("  icq_values = importer.calculate_icq()")
    print("  print(icq_values)")
    print()
    print("  # Get summary")
    print("  print(importer.get_summary())")
    print()
