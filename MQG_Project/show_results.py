#!/usr/bin/env python3
"""Quick script to display the generated results"""
import json

with open('test_results/simulation_results.json', 'r') as f:
    data = json.load(f)

print("=" * 60)
print("DOUBLE-SLIT MQG SIMULATION RESULTS")
print("=" * 60)
print(f"\nTimestamp: {data['timestamp']}")
print(f"\nStatistics:")
print(f"  Chi-squared: {data['statistics']['chi_squared']:.2f}")
print(f"  p-value: {data['statistics']['p_value']:.2e}")
print(f"\nHotspots: {data['num_hotspots']} positions found")
print(f"\nTop 5 Hotspot Positions:")
for i, hs in enumerate(data['hotspots'][:5], 1):
    print(f"  {i}. {hs['position_um']:+7.2f} μm (deviation: {hs['deviation']:+.2f}σ)")
print("\n" + "=" * 60)
