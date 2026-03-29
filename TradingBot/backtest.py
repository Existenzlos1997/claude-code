"""
Backtesting – Historische Strategie-Simulation
===============================================

Verwendung:
  python backtest.py                         # BTC/EUR, letzte 90 Tage
  python backtest.py --symbol ETH/EUR        # anderes Symbol
  python backtest.py --days 180 --tf 1h      # 180 Tage, 1h-Kerzen
  python backtest.py --plot                  # Equity-Kurve anzeigen

Gibt am Ende Statistiken aus: Trades, Win-Rate, PnL, Sharpe-Ratio, max. Drawdown
"""

from __future__ import annotations

import argparse
import json
import logging
from pathlib import Path
from datetime import datetime, timezone
from typing import Optional

import numpy as np
import pandas as pd

from bot.mqg_analyzer import MQGAnalyzer
from bot.risk_manager import RiskManager
from bot.strategy import MQGStrategy

logger = logging.getLogger(__name__)
logging.basicConfig(level=logging.WARNING, format="%(message)s")

CONFIG_PATH = Path(__file__).parent / "config.json"


def load_config() -> dict:
    import re
    raw = Path(CONFIG_PATH).read_text(encoding="utf-8")
    cleaned = re.sub(r'\s*"_comment[^"]*"\s*:\s*"[^"]*"\s*,?\s*\n', '\n', raw)
    cleaned = re.sub(r',(\s*[}\]])', r'\1', cleaned)
    return json.loads(cleaned)


def fetch_historical(symbol: str, timeframe: str, days: int) -> Optional[pd.DataFrame]:
    """Lädt historische OHLCV-Daten über ccxt (Kraken, öffentlich)."""
    try:
        import ccxt
        ex = ccxt.kraken({"enableRateLimit": True})
        since = int((datetime.now(timezone.utc).timestamp() - days * 86400) * 1000)
        all_rows = []
        limit = 500
        while True:
            rows = ex.fetch_ohlcv(symbol, timeframe, since=since, limit=limit)
            if not rows:
                break
            all_rows.extend(rows)
            since = rows[-1][0] + 1
            if len(rows) < limit:
                break
        if not all_rows:
            return None
        df = pd.DataFrame(all_rows, columns=["timestamp", "open", "high", "low", "close", "volume"])
        df["timestamp"] = pd.to_datetime(df["timestamp"], unit="ms", utc=True)
        df.set_index("timestamp", inplace=True)
        return df.astype(float)
    except Exception as exc:
        logger.error("Fehler beim Laden der Daten: %s", exc)
        return None


def run_backtest(
    df: pd.DataFrame,
    symbol: str,
    cfg: dict,
    initial_capital: float = 100.0,
) -> dict:
    """Simuliert den Bot auf historischen Daten."""
    risk = RiskManager(
        initial_capital=initial_capital,
        risk_per_trade_pct=cfg["risk"]["risk_per_trade_pct"],
        stop_loss_pct=cfg["risk"]["stop_loss_pct"],
        take_profit_pct=cfg["risk"]["take_profit_pct"],
        max_drawdown_pct=cfg["risk"]["max_drawdown_pct"],
        max_open_trades=cfg["trading"]["max_open_trades"],
        trailing_stop=cfg["risk"]["trailing_stop"],
        compound_reinvest=cfg["trading"]["compound_reinvest"],
    )
    strategy = MQGStrategy(cfg["mqg"], risk)

    equity_curve: list[float] = [initial_capital]
    trade_log: list[dict] = []
    window = cfg["mqg"].get("information_field_window", 50)

    open_trade = None
    open_idx = None

    for i in range(window + 20, len(df)):
        slice_df = df.iloc[:i]
        current_price = float(df["close"].iloc[i])
        ts = str(df.index[i])

        # Exit prüfen
        if open_trade is not None:
            open_trade = risk.update_trailing_stop(open_trade, current_price)
            pnl = MQGStrategy._calculate_pnl(open_trade, current_price)
            reason = None

            if open_trade.direction == 1:
                if current_price <= open_trade.stop_loss:
                    reason = "SL"
                elif current_price >= open_trade.take_profit:
                    reason = "TP"
            else:
                if current_price >= open_trade.stop_loss:
                    reason = "SL"
                elif current_price <= open_trade.take_profit:
                    reason = "TP"

            if reason:
                risk.record_trade_result(pnl, pnl > 0)
                trade_log.append({
                    "ts": ts, "symbol": symbol, "dir": open_trade.direction,
                    "entry": open_trade.entry_price, "exit": current_price,
                    "pnl": round(pnl, 4), "reason": reason,
                })
                open_trade = None
                open_idx = None

        equity_curve.append(risk.portfolio.capital)

        # Entry suchen (nur wenn keine offene Position)
        if open_trade is None:
            trade = strategy.generate_signal(slice_df, symbol)
            if trade:
                open_trade = trade
                open_idx = i

    # Falls noch offen am Ende: schließen
    if open_trade is not None:
        exit_price = float(df["close"].iloc[-1])
        pnl = MQGStrategy._calculate_pnl(open_trade, exit_price)
        risk.record_trade_result(pnl, pnl > 0)
        trade_log.append({
            "ts": str(df.index[-1]), "symbol": symbol, "dir": open_trade.direction,
            "entry": open_trade.entry_price, "exit": exit_price,
            "pnl": round(pnl, 4), "reason": "EOD",
        })

    equity = np.array(equity_curve)
    daily_returns = np.diff(equity) / (equity[:-1] + 1e-12)
    # Crypto handelt 365 Tage/Jahr (nicht 252 wie Aktien)
    sharpe = (np.mean(daily_returns) / (np.std(daily_returns) + 1e-12)) * np.sqrt(365) \
        if len(daily_returns) > 1 else 0.0

    return {
        "symbol": symbol,
        "initial_capital": initial_capital,
        "final_capital": round(risk.portfolio.capital, 2),
        "total_pnl": round(risk.portfolio.realized_pnl, 2),
        "total_pnl_pct": round(risk.portfolio.realized_pnl / initial_capital * 100, 2),
        "max_drawdown_pct": round(risk.portfolio.drawdown_pct, 2),
        "total_trades": risk.portfolio.total_trades,
        "win_rate_pct": round(risk.portfolio.win_rate, 1),
        "sharpe_ratio": round(float(sharpe), 3),
        "equity_curve": equity_curve,
        "trade_log": trade_log,
    }


def print_results(r: dict) -> None:
    print("\n" + "=" * 55)
    print(f"  Backtest-Ergebnisse: {r['symbol']}")
    print("=" * 55)
    print(f"  Startkapital      : {r['initial_capital']:.2f} €")
    print(f"  Endkapital        : {r['final_capital']:.2f} €")
    pnl = r['total_pnl']
    pnl_pct = r['total_pnl_pct']
    sign = "+" if pnl >= 0 else ""
    print(f"  Gesamter PnL      : {sign}{pnl:.2f} € ({sign}{pnl_pct:.1f} %)")
    print(f"  Max. Drawdown     : {r['max_drawdown_pct']:.1f} %")
    print(f"  Anzahl Trades     : {r['total_trades']}")
    print(f"  Win-Rate          : {r['win_rate_pct']:.0f} %")
    print(f"  Sharpe-Ratio      : {r['sharpe_ratio']:.3f}")
    print("=" * 55)
    if r['trade_log']:
        print(f"\n  Letzte {min(5,len(r['trade_log']))} Trades:")
        for t in r['trade_log'][-5:]:
            sign2 = "+" if t['pnl'] >= 0 else ""
            print(f"    {t['ts'][:16]}  {t['symbol']:10}  "
                  f"{'LONG' if t['dir']==1 else 'SHORT':5}  "
                  f"{sign2}{t['pnl']:.4f} €  [{t['reason']}]")
    print()


def main() -> None:
    parser = argparse.ArgumentParser(description="MQG Trading Bot Backtester")
    parser.add_argument("--symbol", default="BTC/EUR")
    parser.add_argument("--days", type=int, default=90)
    parser.add_argument("--tf", default="15m")
    parser.add_argument("--capital", type=float, default=100.0)
    parser.add_argument("--plot", action="store_true", help="Equity-Kurve plotten")
    args = parser.parse_args()

    print(f"\nLade historische Daten: {args.symbol} | {args.tf} | {args.days} Tage ...")
    df = fetch_historical(args.symbol, args.tf, args.days)
    if df is None or len(df) < 100:
        print("Fehler: Zu wenig Daten. Internetverbindung prüfen.")
        return

    print(f"Daten geladen: {len(df)} Kerzen | {df.index[0]} → {df.index[-1]}")

    cfg = load_config()
    result = run_backtest(df, args.symbol, cfg, args.capital)
    print_results(result)

    if args.plot:
        try:
            import matplotlib.pyplot as plt
            eq = result["equity_curve"]
            plt.figure(figsize=(12, 5))
            plt.plot(eq, color="#3fb950", linewidth=1.5, label="Equity")
            plt.axhline(args.capital, color="#8b949e", linestyle="--", linewidth=1, label="Startkapital")
            plt.fill_between(range(len(eq)), args.capital, eq,
                             where=[e >= args.capital for e in eq], alpha=0.2, color="#3fb950")
            plt.fill_between(range(len(eq)), args.capital, eq,
                             where=[e < args.capital for e in eq], alpha=0.2, color="#f85149")
            plt.title(f"MQG Bot Equity – {args.symbol} ({args.tf}, {args.days}d)")
            plt.xlabel("Kerzen")
            plt.ylabel("Kapital (€)")
            plt.legend()
            plt.tight_layout()
            plt.savefig("backtest_equity.png", dpi=120)
            print("Equity-Kurve gespeichert: backtest_equity.png")
            plt.show()
        except ImportError:
            print("matplotlib nicht installiert – kein Plot möglich.")


if __name__ == "__main__":
    main()
