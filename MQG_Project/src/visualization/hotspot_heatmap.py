"""
Hotspot Heatmap Visualization for Double-Slit MQG Experiments

Creates visual representations of detector positions showing where
the MQG model significantly deviates from standard quantum mechanics.
"""

import numpy as np
import matplotlib.pyplot as plt
import matplotlib.colors as mcolors
from typing import Dict, List, Optional, Tuple


class HotspotHeatmap:
    """
    Visualizer for double-slit experiment hotspots.
    
    Creates heatmaps showing detector positions color-coded by:
    - Intensity (electron detection probability)
    - Deviation from standard QM (in units of σ)
    - Statistical significance
    """
    
    def __init__(self, figsize: Tuple[int, int] = (14, 10)):
        """
        Initialize visualizer.
        
        Args:
            figsize: Figure size in inches (width, height)
        """
        self.figsize = figsize
        
    def plot_comparison(self, results: Dict[str, np.ndarray],
                       hotspots: List[Dict],
                       save_path: Optional[str] = None):
        """
        Create comprehensive comparison plot.
        
        Shows:
        1. Standard QM pattern
        2. MQG-modified pattern
        3. Deviation heatmap with hotspots marked
        
        Args:
            results: Simulation results from DoubleSlitMQG
            hotspots: List of hotspot dictionaries
            save_path: Optional path to save figure
        """
        fig, axes = plt.subplots(3, 1, figsize=self.figsize)
        
        positions_um = results['positions'] * 1e6  # Convert to μm
        
        # 1. Standard QM Pattern
        ax1 = axes[0]
        ax1.plot(positions_um, results['standard'], 'b-', linewidth=2, label='Standard QM')
        ax1.fill_between(positions_um, 0, results['standard'], alpha=0.3)
        ax1.set_ylabel('Expected Counts', fontsize=12)
        ax1.set_title('Standard Quantum Mechanics (Born Rule)', fontsize=14, fontweight='bold')
        ax1.grid(True, alpha=0.3)
        ax1.legend()
        
        # 2. MQG-Modified Pattern
        ax2 = axes[1]
        ax2.plot(positions_um, results['mqg'], 'r-', linewidth=2, label='MQG Model')
        ax2.plot(positions_um, results['standard'], 'b--', linewidth=1, alpha=0.5, label='Standard (reference)')
        ax2.fill_between(positions_um, 0, results['mqg'], alpha=0.3, color='red')
        ax2.set_ylabel('Expected Counts', fontsize=12)
        ax2.set_title('MQG-Modified Pattern (Information Field Effects)', fontsize=14, fontweight='bold')
        ax2.grid(True, alpha=0.3)
        ax2.legend()
        
        # 3. Deviation Heatmap
        ax3 = axes[2]
        
        # Calculate deviation in σ units
        standard = results['standard']
        mqg = results['mqg']
        sigma = np.sqrt(standard)
        deviations = (mqg - standard) / sigma
        
        # Create heatmap using imshow (requires 2D data)
        # We'll create a narrow strip to show as heatmap
        heatmap_data = np.repeat(deviations.reshape(1, -1), 20, axis=0)
        
        # Custom colormap: blue (negative) -> white (zero) -> red (positive)
        cmap = mcolors.LinearSegmentedColormap.from_list(
            'deviation',
            ['#0000FF', '#4444FF', '#FFFFFF', '#FF4444', '#FF0000']
        )
        
        im = ax3.imshow(heatmap_data, aspect='auto', cmap=cmap,
                       extent=[positions_um[0], positions_um[-1], 0, 1],
                       vmin=-3, vmax=3, interpolation='bilinear')
        
        # Mark hotspots
        for hs in hotspots:
            pos_um = hs['position_um']
            dev = hs['deviation']
            marker_color = 'yellow' if abs(dev) > 2.5 else 'lime'
            ax3.axvline(pos_um, color=marker_color, linewidth=2, alpha=0.7)
            ax3.plot(pos_um, 0.5, 'o', color=marker_color, markersize=8, 
                    markeredgecolor='black', markeredgewidth=1.5)
        
        ax3.set_ylabel('Deviation\nScale', fontsize=12)
        ax3.set_xlabel('Detector Position (μm)', fontsize=12)
        ax3.set_title(f'Deviation Heatmap ({len(hotspots)} Hotspots Marked)', 
                     fontsize=14, fontweight='bold')
        ax3.set_yticks([])
        
        # Add colorbar
        cbar = plt.colorbar(im, ax=ax3, orientation='horizontal', 
                           pad=0.15, aspect=40)
        cbar.set_label('Deviation from Standard QM (σ)', fontsize=11)
        cbar.ax.axvline(2.0, color='lime', linewidth=2, linestyle='--', label='2σ threshold')
        cbar.ax.axvline(-2.0, color='lime', linewidth=2, linestyle='--')
        
        plt.tight_layout()
        
        if save_path:
            plt.savefig(save_path, dpi=300, bbox_inches='tight')
            print(f"✓ Saved comparison plot to: {save_path}")
        
        return fig
    
    def plot_hotspot_details(self, results: Dict[str, np.ndarray],
                            hotspots: List[Dict],
                            save_path: Optional[str] = None):
        """
        Create detailed view of hotspot positions.
        
        Args:
            results: Simulation results
            hotspots: List of hotspot dictionaries
            save_path: Optional path to save figure
        """
        if not hotspots:
            print("No hotspots to plot")
            return None
        
        fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(12, 8))
        
        positions_um = results['positions'] * 1e6
        
        # Extract hotspot positions and deviations
        hotspot_positions = [hs['position_um'] for hs in hotspots]
        hotspot_deviations = [hs['deviation'] for hs in hotspots]
        
        # 1. Hotspot positions along detector
        ax1.plot(positions_um, results['mqg'], 'r-', linewidth=1.5, alpha=0.5, label='MQG Pattern')
        ax1.plot(positions_um, results['standard'], 'b-', linewidth=1.5, alpha=0.5, label='Standard QM')
        
        # Mark hotspots
        for hs in hotspots:
            pos_um = hs['position_um']
            mqg_val = hs['mqg_value']
            std_val = hs['standard_value']
            dev = hs['deviation']
            
            color = 'red' if dev > 0 else 'blue'
            ax1.plot([pos_um, pos_um], [std_val, mqg_val], color=color, 
                    linewidth=2, alpha=0.7)
            ax1.plot(pos_um, mqg_val, 'o', color=color, markersize=6)
        
        ax1.set_xlabel('Detector Position (μm)', fontsize=11)
        ax1.set_ylabel('Expected Counts', fontsize=11)
        ax1.set_title(f'Hotspot Positions (|deviation| > 2σ)', fontsize=13, fontweight='bold')
        ax1.legend()
        ax1.grid(True, alpha=0.3)
        
        # 2. Hotspot deviation distribution
        colors = ['red' if dev > 0 else 'blue' for dev in hotspot_deviations]
        bars = ax2.bar(range(len(hotspots)), hotspot_deviations, color=colors, alpha=0.7, edgecolor='black')
        
        # Threshold lines
        ax2.axhline(2.0, color='green', linestyle='--', linewidth=2, label='2σ threshold')
        ax2.axhline(-2.0, color='green', linestyle='--', linewidth=2)
        ax2.axhline(0, color='black', linestyle='-', linewidth=1)
        
        ax2.set_xlabel('Hotspot Index', fontsize=11)
        ax2.set_ylabel('Deviation (σ)', fontsize=11)
        ax2.set_title('Deviation Magnitude at Each Hotspot', fontsize=13, fontweight='bold')
        ax2.legend()
        ax2.grid(True, alpha=0.3, axis='y')
        
        plt.tight_layout()
        
        if save_path:
            plt.savefig(save_path, dpi=300, bbox_inches='tight')
            print(f"✓ Saved hotspot details to: {save_path}")
        
        return fig
    
    def plot_2d_heatmap(self, results: Dict[str, np.ndarray],
                       save_path: Optional[str] = None):
        """
        Create 2D heatmap representation (position vs. time/ensemble dimension).
        
        Simulates multiple runs to show consistency of deviation pattern.
        
        Args:
            results: Simulation results
            save_path: Optional path to save figure
        """
        positions_um = results['positions'] * 1e6
        standard = results['standard']
        mqg = results['mqg']
        
        # Create 2D deviation map by simulating multiple runs
        n_runs = 50
        deviations_2d = np.zeros((n_runs, len(positions_um)))
        
        for i in range(n_runs):
            # Add statistical noise to simulate different runs
            noise_std = np.sqrt(mqg)
            noise = np.random.normal(0, noise_std)
            mqg_noisy = mqg + noise
            
            sigma = np.sqrt(standard)
            deviations_2d[i] = (mqg_noisy - standard) / sigma
        
        # Create heatmap
        fig, ax = plt.subplots(figsize=(14, 6))
        
        cmap = mcolors.LinearSegmentedColormap.from_list(
            'deviation',
            ['#0000FF', '#8888FF', '#FFFFFF', '#FF8888', '#FF0000']
        )
        
        im = ax.imshow(deviations_2d, aspect='auto', cmap=cmap,
                      extent=[positions_um[0], positions_um[-1], 0, n_runs],
                      vmin=-4, vmax=4, interpolation='bilinear')
        
        ax.set_xlabel('Detector Position (μm)', fontsize=12)
        ax.set_ylabel('Simulation Run', fontsize=12)
        ax.set_title('2D Deviation Heatmap (Multiple Simulation Runs)', 
                    fontsize=14, fontweight='bold')
        
        # Colorbar
        cbar = plt.colorbar(im, ax=ax)
        cbar.set_label('Deviation from Standard QM (σ)', fontsize=11)
        
        plt.tight_layout()
        
        if save_path:
            plt.savefig(save_path, dpi=300, bbox_inches='tight')
            print(f"✓ Saved 2D heatmap to: {save_path}")
        
        return fig


def create_all_visualizations(results: Dict[str, np.ndarray],
                              hotspots: List[Dict],
                              output_dir: str = "."):
    """
    Generate all visualization types and save to directory.
    
    Args:
        results: Simulation results
        hotspots: Hotspot list
        output_dir: Directory to save plots
    """
    import os
    os.makedirs(output_dir, exist_ok=True)
    
    visualizer = HotspotHeatmap()
    
    print("\nGenerating visualizations...")
    
    # 1. Comparison plot
    visualizer.plot_comparison(
        results, hotspots,
        save_path=f"{output_dir}/double_slit_comparison.png"
    )
    
    # 2. Hotspot details
    if hotspots:
        visualizer.plot_hotspot_details(
            results, hotspots,
            save_path=f"{output_dir}/hotspot_details.png"
        )
    
    # 3. 2D heatmap
    visualizer.plot_2d_heatmap(
        results,
        save_path=f"{output_dir}/deviation_heatmap_2d.png"
    )
    
    print(f"\n✓ All visualizations saved to: {output_dir}/")


if __name__ == "__main__":
    # Example usage requires running the simulation first
    print("This module requires simulation results.")
    print("Run: python demo_double_slit.py")
