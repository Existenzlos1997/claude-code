#!/usr/bin/env python3
"""
phyphox Data Analyzer - Command-Line Tool

Analyze phyphox sensor data exports and calculate ICQ values.

Usage:
    python phyphox_analyzer.py <file1.csv> [file2.csv ...]
    python phyphox_analyzer.py -o results.json data.csv
    python phyphox_analyzer.py --help

Author: MQG Project
Version: 1.0.0
"""

import argparse
import sys
import json
from pathlib import Path
from typing import List

# Import the phyphox importer
from src.experimental.phyphox_import import PhyphoxDataImporter, quick_analyze


def analyze_file(filepath: str, verbose: bool = False) -> dict:
    """
    Analyze a single phyphox export file.
    
    Args:
        filepath: Path to the file
        verbose: Print detailed information
        
    Returns:
        Analysis results dictionary
    """
    try:
        print(f"\n{'='*60}")
        print(f"Analyzing: {Path(filepath).name}")
        print(f"{'='*60}")
        
        results = quick_analyze(filepath)
        
        if verbose:
            print(results['summary'])
        else:
            print(f"Sensor Type: {results['sensor_type']}")
            print(f"Samples: {results['metadata']['samples']}")
            print(f"Duration: {results['metadata']['duration']:.2f} s")
            print(f"\nICQ Values:")
            for key, icq in results['icq_values'].items():
                print(f"  {key}: {icq:.4f}")
        
        return results
        
    except Exception as e:
        print(f"Error analyzing {filepath}: {e}", file=sys.stderr)
        return None


def analyze_multiple(filepaths: List[str], output_file: str = None, 
                     format: str = 'text', verbose: bool = False):
    """
    Analyze multiple phyphox export files.
    
    Args:
        filepaths: List of file paths
        output_file: Optional output file path
        format: Output format ('text' or 'json')
        verbose: Print detailed information
    """
    all_results = []
    
    for filepath in filepaths:
        result = analyze_file(filepath, verbose=verbose)
        if result:
            all_results.append(result)
    
    # Summary
    print(f"\n{'='*60}")
    print(f"Summary: {len(all_results)} / {len(filepaths)} files analyzed successfully")
    print(f"{'='*60}")
    
    if len(all_results) > 1:
        print("\nICQ Comparison:")
        for result in all_results:
            print(f"\n{result['metadata']['filename']}:")
            for key, icq in result['icq_values'].items():
                print(f"  {key}: {icq:.4f}")
    
    # Export results if requested
    if output_file:
        export_results(all_results, output_file, format)
        print(f"\nResults exported to: {output_file}")


def export_results(results: List[dict], output_file: str, format: str):
    """
    Export analysis results to file.
    
    Args:
        results: List of result dictionaries
        output_file: Output file path
        format: Output format ('text' or 'json')
    """
    output_path = Path(output_file)
    
    if format == 'json':
        with open(output_path, 'w') as f:
            json.dump(results, f, indent=2)
    else:  # text format
        with open(output_path, 'w') as f:
            f.write("phyphox Data Analysis Results\n")
            f.write("="*60 + "\n\n")
            
            for i, result in enumerate(results, 1):
                f.write(f"File {i}: {result['metadata']['filename']}\n")
                f.write(f"Sensor Type: {result['sensor_type']}\n")
                f.write(f"Samples: {result['metadata']['samples']}\n")
                f.write(f"Duration: {result['metadata']['duration']:.2f} seconds\n")
                f.write(f"Sampling Rate: {result['metadata']['sampling_rate']:.1f} Hz\n\n")
                f.write("ICQ Values:\n")
                for key, icq in result['icq_values'].items():
                    f.write(f"  {key}: {icq:.4f}\n")
                f.write("\n" + "-"*60 + "\n\n")


def main():
    """Main command-line interface."""
    parser = argparse.ArgumentParser(
        description='Analyze phyphox sensor data and calculate ICQ values.',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  # Analyze single file
  python phyphox_analyzer.py acceleration_data.csv
  
  # Analyze multiple files
  python phyphox_analyzer.py data1.csv data2.csv data3.csv
  
  # Export results as JSON
  python phyphox_analyzer.py -o results.json data.csv
  
  # Verbose output
  python phyphox_analyzer.py -v data.csv

For more information about phyphox:
  https://phyphox.org
        """
    )
    
    parser.add_argument(
        'files',
        nargs='+',
        help='phyphox export file(s) to analyze (CSV or Excel)'
    )
    
    parser.add_argument(
        '-o', '--output',
        help='Output file path for results'
    )
    
    parser.add_argument(
        '-f', '--format',
        choices=['text', 'json'],
        default='text',
        help='Output format (default: text)'
    )
    
    parser.add_argument(
        '-v', '--verbose',
        action='store_true',
        help='Verbose output with detailed information'
    )
    
    args = parser.parse_args()
    
    # Validate files exist
    for filepath in args.files:
        if not Path(filepath).exists():
            print(f"Error: File not found: {filepath}", file=sys.stderr)
            sys.exit(1)
    
    # Analyze files
    try:
        analyze_multiple(
            args.files,
            output_file=args.output,
            format=args.format,
            verbose=args.verbose
        )
    except KeyboardInterrupt:
        print("\nAnalysis interrupted by user.")
        sys.exit(1)
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)


if __name__ == '__main__':
    main()
