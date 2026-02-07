import os
import argparse
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

def compute_entropy(x, bins=64):
    """Histogram-basierte Shannon-Entropie (log2)."""
    x = np.asarray(x)
    if x.size == 0:
        return 0.0
    # Robustheit: Wenn Varianz ~0, Entropie ~0
    if np.allclose(np.var(x), 0.0):
        return 0.0
    hist, bin_edges = np.histogram(x, bins=bins)
    p = hist.astype(float) / np.sum(hist)
    p = p[p > 0]
    H = -np.sum(p * np.log2(p))
    return float(H)

def compute_icq(x, bins=64):
    """ICQ = 1 / (1 + H(X)). Werte in (0,1], höher = kohärenter."""
    H = compute_entropy(x, bins=bins)
    return 1.0 / (1.0 + H)

def sliding_windows_by_time(df, time_col, win_seconds=3.0, step_seconds=0.5):
    """
    Erzeugt zeitbasierte Fenster über die Time(s)-Spalte.
    Gibt Liste von (t_center, df_window) zurück.
    """
    t = df[time_col].values
    t_min, t_max = float(np.min(t)), float(np.max(t))
    centers = np.arange(t_min + win_seconds/2.0, t_max - win_seconds/2.0 + 1e-9, step_seconds)
    windows = []
    half = win_seconds / 2.0
    for c in centers:
        mask = (t >= c - half) & (t <= c + half)
        dfw = df.loc[mask]
        windows.append((c, dfw))
    return windows

def robust_column_names(cols):
    """Entfernt Anführungszeichen und trimmt Spaltennamen."""
    return [c.strip().strip('"').strip("'") for c in cols]

def main():
    parser = argparse.ArgumentParser(description="Sliding-Window-ICQ Analyse für Linear Acceleration CSV")
    parser.add_argument("--csv", type=str, required=True, help="Pfad zur CSV-Datei (Linear Acceleration.csv)")
    parser.add_argument("--win_seconds", type=float, default=3.0, help="Fensterlänge in Sekunden (default: 3.0)")
    parser.add_argument("--step_seconds", type=float, default=0.5, help="Schrittweite in Sekunden (default: 0.5)")
    parser.add_argument("--bins", type=int, default=64, help="Histogrammbins für Entropie (default: 64)")
    parser.add_argument("--out_dir", type=str, default="MQG_Project/results", help="Ausgabeverzeichnis (default: MQG_Project/results)")
    args = parser.parse_args()

    os.makedirs(args.out_dir, exist_ok=True)

    # CSV laden
    df = pd.read_csv(args.csv)
    df.columns = robust_column_names(df.columns)
    # Erwartete Spalten
    time_col = "Time (s)"
    axes = [
        "Linear Acceleration x (m/s^2)",
        "Linear Acceleration y (m/s^2)",
        "Linear Acceleration z (m/s^2)",
    ]
    for col in [time_col] + axes:
        if col not in df.columns:
            raise ValueError(f"Spalte '{col}' nicht in CSV gefunden. Gefundene Spalten: {list(df.columns)}")

    # Fenster berechnen
    windows = sliding_windows_by_time(df, time_col=time_col, win_seconds=args.win_seconds, step_seconds=args.step_seconds)

    # ICQ pro Fenster und Achse
    t_centers = []
    icq_per_axis = {ax: [] for ax in axes}
    for c, dfw in windows:
        t_centers.append(c)
        for ax in axes:
            icq = compute_icq(dfw[ax].values, bins=args.bins)
            icq_per_axis[ax].append(icq)

    # Zusammenfassungsstatistik
    summary = {}
    for ax in axes:
        vals = np.array(icq_per_axis[ax], dtype=float)
        summary[ax] = {
            "mean": float(np.mean(vals)) if vals.size else None,
            "median": float(np.median(vals)) if vals.size else None,
            "max": float(np.max(vals)) if vals.size else None,
            "min": float(np.min(vals)) if vals.size else None,
        }

    # Plotten
    plt.figure(figsize=(10, 6))
    for ax in axes:
        plt.plot(t_centers, icq_per_axis[ax], label=ax)
    plt.xlabel("Zeit (s) (Fenstermitte)")
    plt.ylabel("ICQ (0..1)")
    plt.title(f"Sliding-Window-ICQ (win={args.win_seconds}s, step={args.step_seconds}s, bins={args.bins})")
    plt.grid(True, alpha=0.3)
    plt.legend()
    out_png = os.path.join(args.out_dir, "icq_window_plot.png")
    plt.tight_layout()
    plt.savefig(out_png, dpi=150)
    print(f"Plot gespeichert: {out_png}")

    # Summary JSON
    import json
    out_json = os.path.join(args.out_dir, "icq_window_summary.json")
    with open(out_json, "w", encoding="utf-8") as f:
        json.dump({
            "params": {
                "win_seconds": args.win_seconds,
                "step_seconds": args.step_seconds,
                "bins": args.bins,
                "csv": args.csv
            },
            "axes_summary": summary
        }, f, ensure_ascii=False, indent=2)
    print(f"Zusammenfassung gespeichert: {out_json}")

    # ICQ der Gesamtdatei (optional, zur Einordnung)
    global_icq = {}
    for ax in axes:
        global_icq[ax] = compute_icq(df[ax].values, bins=args.bins)
    out_global = os.path.join(args.out_dir, "icq_global.json")
    with open(out_global, "w", encoding="utf-8") as f:
        json.dump(global_icq, f, ensure_ascii=False, indent=2)
    print(f"Global-ICQ gespeichert: {out_global}")

if __name__ == "__main__":
    main()