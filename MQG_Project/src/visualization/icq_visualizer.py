"""
MQG-Theorie: Visualization Tools
=================================

This module provides visualization capabilities for ICQ data and analysis.

Visualization Types:
--------------------
1. ICQ time series plots
2. Comparative bar charts
3. Distribution histograms
4. Diagnostic dashboards

Author: MQG Project - Autonomous Development System
Version: 0.1.0-alpha
Date: 2026-02-01
"""

import numpy as np
import matplotlib.pyplot as plt
from typing import List, Dict, Any, Tuple
import json


class ICQVisualizer:
    """
    Visualization tools for ICQ data and analysis.
    
    Provides publication-quality plots and interactive visualizations.
    """
    
    def __init__(self, style: str = 'default'):
        """
        Initialize visualizer.
        
        Parameters:
        -----------
        style : str, optional
            Matplotlib style ('default', 'seaborn', 'ggplot')
        """
        self.style = style
        if style != 'default':
            try:
                plt.style.use(style)
            except:
                print(f"Warning: Style '{style}' not found, using default")
    
    def plot_icq_timeseries(
        self,
        icq_values: List[float],
        timestamps: List[str] = None,
        title: str = "ICQ Time Series",
        save_path: str = None
    ) -> None:
        """
        Plot ICQ values over time.
        
        Parameters:
        -----------
        icq_values : list of float
            ICQ values to plot
        timestamps : list of str, optional
            Timestamps for x-axis
        title : str, optional
            Plot title
        save_path : str, optional
            Path to save figure
        """
        fig, ax = plt.subplots(figsize=(12, 6))
        
        x = timestamps if timestamps else range(len(icq_values))
        
        ax.plot(x, icq_values, linewidth=2, marker='o', markersize=4)
        ax.set_xlabel('Time' if timestamps else 'Sample Index', fontsize=12)
        ax.set_ylabel('ICQ (Information Coherence Quotient)', fontsize=12)
        ax.set_title(title, fontsize=14, fontweight='bold')
        ax.grid(True, alpha=0.3)
        ax.set_ylim([0, max(1.2, max(icq_values) * 1.1)])
        
        # Add reference line at ICQ=1.0
        ax.axhline(y=1.0, color='r', linestyle='--', alpha=0.5, label='Perfect Coherence')
        ax.legend()
        
        plt.tight_layout()
        
        if save_path:
            plt.savefig(save_path, dpi=300, bbox_inches='tight')
            print(f"Figure saved to: {save_path}")
        else:
            plt.show()
        
        plt.close()
    
    def plot_icq_comparison(
        self,
        data: Dict[str, float],
        title: str = "ICQ Comparison",
        save_path: str = None
    ) -> None:
        """
        Create bar chart comparing ICQ values across different scenarios.
        
        Parameters:
        -----------
        data : dict
            Dictionary mapping labels to ICQ values
        title : str, optional
            Plot title
        save_path : str, optional
            Path to save figure
        """
        fig, ax = plt.subplots(figsize=(10, 6))
        
        labels = list(data.keys())
        values = list(data.values())
        
        colors = plt.cm.viridis(np.linspace(0, 1, len(labels)))
        
        bars = ax.bar(labels, values, color=colors, alpha=0.8, edgecolor='black')
        
        # Add value labels on bars
        for bar in bars:
            height = bar.get_height()
            ax.text(bar.get_x() + bar.get_width()/2., height,
                   f'{height:.3f}',
                   ha='center', va='bottom', fontsize=10)
        
        ax.set_ylabel('ICQ (Information Coherence Quotient)', fontsize=12)
        ax.set_title(title, fontsize=14, fontweight='bold')
        ax.set_ylim([0, max(1.2, max(values) * 1.1)])
        ax.grid(axis='y', alpha=0.3)
        
        # Add reference line
        ax.axhline(y=1.0, color='r', linestyle='--', alpha=0.5, label='Perfect Coherence')
        ax.legend()
        
        plt.xticks(rotation=45, ha='right')
        plt.tight_layout()
        
        if save_path:
            plt.savefig(save_path, dpi=300, bbox_inches='tight')
            print(f"Figure saved to: {save_path}")
        else:
            plt.show()
        
        plt.close()
    
    def plot_diagnostic_dashboard(
        self,
        diagnostics: Dict[str, Any],
        title: str = "ICQ Diagnostic Dashboard",
        save_path: str = None
    ) -> None:
        """
        Create comprehensive diagnostic dashboard.
        
        Parameters:
        -----------
        diagnostics : dict
            Diagnostic information from ICQ calculation
        title : str, optional
            Dashboard title
        save_path : str, optional
            Path to save figure
        """
        fig = plt.figure(figsize=(14, 8))
        gs = fig.add_gridspec(2, 3, hspace=0.3, wspace=0.3)
        
        # 1. ICQ Gauge
        ax1 = fig.add_subplot(gs[0, 0])
        icq = diagnostics.get('icq', 0)
        self._plot_gauge(ax1, icq, "ICQ")
        
        # 2. Entropy comparison
        ax2 = fig.add_subplot(gs[0, 1])
        s_actual = diagnostics.get('s_actual', 0)
        s_max = diagnostics.get('s_max', 1)
        ax2.bar(['Actual', 'Maximum'], [s_actual, s_max], color=['blue', 'red'], alpha=0.7)
        ax2.set_ylabel('Entropy')
        ax2.set_title('Entropy Analysis')
        ax2.grid(axis='y', alpha=0.3)
        
        # 3. Coherence factor
        ax3 = fig.add_subplot(gs[0, 2])
        c_factor = diagnostics.get('c_factor', 1.0)
        self._plot_gauge(ax3, c_factor, "C-Factor", vmin=0.5, vmax=1.5)
        
        # 4. Summary statistics table
        ax4 = fig.add_subplot(gs[1, :])
        ax4.axis('tight')
        ax4.axis('off')
        
        table_data = [
            ['Metric', 'Value', 'Description'],
            ['ICQ', f"{icq:.4f}", 'Information Coherence Quotient'],
            ['S_actual', f"{s_actual:.4f}", 'Measured Entropy'],
            ['S_max', f"{s_max:.4f}", 'Maximum Entropy'],
            ['C_factor', f"{c_factor:.4f}", 'Coherence Correction Factor'],
            ['N_states', f"{diagnostics.get('n_states', 'N/A')}", 'Number of States'],
            ['Normalized Entropy', f"{diagnostics.get('normalized_entropy', 0):.4f}", 'S_actual / S_max']
        ]
        
        table = ax4.table(cellText=table_data, cellLoc='left', loc='center',
                         colWidths=[0.25, 0.15, 0.6])
        table.auto_set_font_size(False)
        table.set_fontsize(9)
        table.scale(1, 2)
        
        # Style header row
        for i in range(3):
            table[(0, i)].set_facecolor('#40466e')
            table[(0, i)].set_text_props(weight='bold', color='white')
        
        fig.suptitle(title, fontsize=16, fontweight='bold')
        
        if save_path:
            plt.savefig(save_path, dpi=300, bbox_inches='tight')
            print(f"Dashboard saved to: {save_path}")
        else:
            plt.show()
        
        plt.close()
    
    def _plot_gauge(
        self,
        ax,
        value: float,
        label: str,
        vmin: float = 0.0,
        vmax: float = 1.0
    ) -> None:
        """
        Plot a gauge/dial visualization.
        
        Parameters:
        -----------
        ax : matplotlib axis
            Axis to plot on
        value : float
            Value to display
        label : str
            Label for the gauge
        vmin : float
            Minimum value
        vmax : float
            Maximum value
        """
        # Normalize value
        norm_value = (value - vmin) / (vmax - vmin)
        norm_value = np.clip(norm_value, 0, 1)
        
        # Create gauge
        theta = np.linspace(0, np.pi, 100)
        x = np.cos(theta)
        y = np.sin(theta)
        
        # Background arc
        ax.plot(x, y, 'k-', linewidth=8, alpha=0.2)
        
        # Value arc
        theta_val = np.linspace(0, np.pi * norm_value, 100)
        x_val = np.cos(theta_val)
        y_val = np.sin(theta_val)
        
        # Color based on value
        if norm_value < 0.33:
            color = 'red'
        elif norm_value < 0.67:
            color = 'orange'
        else:
            color = 'green'
        
        ax.plot(x_val, y_val, linewidth=8, color=color, alpha=0.8)
        
        # Needle
        angle = np.pi * norm_value
        ax.plot([0, np.cos(angle)], [0, np.sin(angle)], 'k-', linewidth=2)
        
        # Value text
        ax.text(0, -0.3, f'{value:.3f}', ha='center', va='top',
               fontsize=14, fontweight='bold')
        ax.text(0, -0.5, label, ha='center', va='top', fontsize=10)
        
        # Range labels
        ax.text(-1, -0.1, f'{vmin:.1f}', ha='left', va='center', fontsize=8)
        ax.text(1, -0.1, f'{vmax:.1f}', ha='right', va='center', fontsize=8)
        
        ax.set_xlim(-1.2, 1.2)
        ax.set_ylim(-0.6, 1.2)
        ax.set_aspect('equal')
        ax.axis('off')
    
    def plot_icq_histogram(
        self,
        icq_values: List[float],
        bins: int = 20,
        title: str = "ICQ Distribution",
        save_path: str = None
    ) -> None:
        """
        Plot histogram of ICQ values.
        
        Parameters:
        -----------
        icq_values : list of float
            ICQ values
        bins : int, optional
            Number of histogram bins
        title : str, optional
            Plot title
        save_path : str, optional
            Path to save figure
        """
        fig, ax = plt.subplots(figsize=(10, 6))
        
        ax.hist(icq_values, bins=bins, edgecolor='black', alpha=0.7, color='skyblue')
        ax.set_xlabel('ICQ (Information Coherence Quotient)', fontsize=12)
        ax.set_ylabel('Frequency', fontsize=12)
        ax.set_title(title, fontsize=14, fontweight='bold')
        ax.grid(axis='y', alpha=0.3)
        
        # Add statistics
        mean_icq = np.mean(icq_values)
        std_icq = np.std(icq_values)
        ax.axvline(mean_icq, color='red', linestyle='--', linewidth=2, label=f'Mean: {mean_icq:.3f}')
        ax.axvline(mean_icq - std_icq, color='orange', linestyle=':', alpha=0.7)
        ax.axvline(mean_icq + std_icq, color='orange', linestyle=':', alpha=0.7, label=f'±1 Std: {std_icq:.3f}')
        ax.legend()
        
        plt.tight_layout()
        
        if save_path:
            plt.savefig(save_path, dpi=300, bbox_inches='tight')
            print(f"Figure saved to: {save_path}")
        else:
            plt.show()
        
        plt.close()


if __name__ == "__main__":
    # Self-test and demonstration
    print("=" * 70)
    print("MQG-Theorie: Visualization Tools - Self-Test")
    print("=" * 70)
    
    # Initialize visualizer
    viz = ICQVisualizer()
    
    # Test 1: Time series plot
    print("\n[Test 1] Creating ICQ time series plot...")
    icq_series = [0.3, 0.35, 0.4, 0.5, 0.65, 0.7, 0.68, 0.72, 0.75, 0.8]
    viz.plot_icq_timeseries(icq_series, title="Sample ICQ Time Series")
    print("✓ Time series plot created")
    
    # Test 2: Comparison plot
    print("\n[Test 2] Creating ICQ comparison plot...")
    comparison_data = {
        'Random': 0.05,
        'Ordered': 0.95,
        'Biased': 0.45,
        'Markov': 0.65,
        'Noisy': 0.55
    }
    viz.plot_icq_comparison(comparison_data, title="ICQ Across Different Systems")
    print("✓ Comparison plot created")
    
    # Test 3: Diagnostic dashboard
    print("\n[Test 3] Creating diagnostic dashboard...")
    diagnostics = {
        'icq': 0.732,
        's_actual': 1.234,
        's_max': 2.321,
        'c_factor': 1.15,
        'n_states': 8,
        'normalized_entropy': 0.532
    }
    viz.plot_diagnostic_dashboard(diagnostics)
    print("✓ Diagnostic dashboard created")
    
    # Test 4: Histogram
    print("\n[Test 4] Creating ICQ histogram...")
    icq_samples = np.random.beta(5, 2, 100)  # Generate sample distribution
    viz.plot_icq_histogram(icq_samples.tolist(), title="Sample ICQ Distribution")
    print("✓ Histogram created")
    
    print("\n" + "=" * 70)
    print("Self-test complete. Visualization tools are operational.")
    print("Note: Plots displayed interactively (close windows to continue).")
    print("=" * 70)
