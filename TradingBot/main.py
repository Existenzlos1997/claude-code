"""
MQG Trading Bot – Haupt-Einstiegspunkt
=======================================

Verwendung:
  python main.py              # Live-Modus (Sandbox by default)
  python main.py --live       # Echtes Trading (ACHTUNG!)
  python main.py --no-web     # Ohne Web-Dashboard
  python main.py --symbols BTC/EUR ETH/EUR

⚠️  WICHTIGER HINWEIS:
  - Starte IMMER zuerst im Sandbox-Modus (sandbox: true in config.json)
  - Setze sandbox: false erst nach erfolgreicher Testphase
  - Kein Finanzrat – auf eigene Verantwortung handeln
  - Deutsche Steuerpflicht für Gewinne beachten (BROKER_GUIDE.md)
"""

from __future__ import annotations

import argparse
import json
import logging
import sys
import threading
import time
from datetime import datetime
from pathlib import Path

import ccxt

from bot.broker.binance import BinanceBroker
from bot.broker.kraken import KrakenBroker
from bot.data.market_data import MarketDataFetcher
from bot.risk_manager import RiskManager
from bot.strategy import MQGStrategy
from bot.webapp.app import run_dashboard, update_state

# ------------------------------------------------------------------
# Logging
# ------------------------------------------------------------------

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(name)s – %(message)s",
    datefmt="%H:%M:%S",
    handlers=[
        logging.StreamHandler(sys.stdout),
        logging.FileHandler("trading_bot.log", encoding="utf-8"),
    ],
)
logger = logging.getLogger("main")

CONFIG_PATH = Path(__file__).parent / "config.json"


def load_config() -> dict:
    import re
    with open(CONFIG_PATH, encoding="utf-8") as f:
        raw = f.read()
    # Kommentarfelder (_comment*) entfernen
    cleaned = re.sub(r'\s*"_comment[^"]*"\s*:\s*"[^"]*"\s*,?\s*\n', '\n', raw)
    # Hängende Kommas vor } oder ] entfernen
    cleaned = re.sub(r',(\s*[}\]])', r'\1', cleaned)
    return json.loads(cleaned)


def build_broker(cfg: dict):
    """Erstellt den konfigurierten Broker-Adapter."""
    name = cfg["broker"]["name"].lower()
    key = cfg["broker"]["api_key"]
    secret = cfg["broker"]["api_secret"]
    sandbox = cfg["broker"].get("sandbox", True)

    if name == "kraken":
        return KrakenBroker(key, secret, sandbox)
    elif name == "binance":
        return BinanceBroker(key, secret, sandbox)
    else:
        raise ValueError(f"Unbekannter Broker: {name}")


def trading_loop(cfg: dict, strategy: MQGStrategy, fetcher: MarketDataFetcher, broker) -> None:
    """Haupt-Handelsschleife – läuft in eigenem Thread."""
    symbols = cfg["trading"]["symbols"]
    timeframe = cfg["trading"]["timeframe"]
    interval_s = _timeframe_to_seconds(timeframe)
    recent_trades: list = []
    signal_history: list = []

    logger.info("Trading-Loop gestartet | Symbole: %s | Timeframe: %s", symbols, timeframe)
    update_state({"bot_running": True, "bot_status": "Läuft"})

    while True:
        try:
            loop_start = time.time()

            for symbol in symbols:
                # ---- Marktdaten laden --------------------------------
                df = fetcher.fetch_ohlcv(symbol, timeframe, limit=200)
                if df is None or len(df) < 60:
                    continue

                current_price = float(df["close"].iloc[-1])
                now_str = datetime.now().strftime("%H:%M:%S")

                # ---- Exit-Check offene Positionen --------------------
                exit_result = strategy.check_exit(symbol, current_price)
                if exit_result:
                    reason, pnl = exit_result
                    was_win = pnl > 0
                    strategy.risk.record_trade_result(pnl, was_win)
                    recent_trades.append({
                        "symbol": symbol,
                        "reason": reason,
                        "pnl": round(pnl, 4),
                        "time": now_str,
                    })
                    logger.info("Position geschlossen: %s | %s | PnL: %.4f €", symbol, reason, pnl)

                # ---- Neue Signal-Analyse ----------------------------
                mqg_signal = strategy.mqg.analyze(df, symbol)
                if mqg_signal and strategy.mqg.is_signal_valid(mqg_signal):
                    signal_history.append({
                        "symbol": symbol,
                        "direction": mqg_signal.direction,
                        "icq": round(mqg_signal.icq, 3),
                        "confidence": round(mqg_signal.confidence, 3),
                        "time": now_str,
                    })

                # ---- Entry-Signal -----------------------------------
                if symbol not in strategy.get_open_trades():
                    trade = strategy.generate_signal(df, symbol)
                    if trade:
                        # Order platzieren
                        side = "buy" if trade.direction == 1 else "sell"
                        broker.place_order(symbol, side, trade.quantity, trade.entry_price)
                        strategy.register_open_trade(trade)

            # ---- Dashboard aktualisieren ----------------------------
            summary = strategy.risk.get_summary()
            open_pos = [
                {
                    "symbol": t.symbol,
                    "direction": t.direction,
                    "entry_price": round(t.entry_price, 6),
                    "stop_loss": round(t.stop_loss, 6),
                    "take_profit": round(t.take_profit, 6),
                }
                for t in strategy.get_open_trades().values()
            ]
            update_state({
                **summary,
                "open_positions": open_pos,
                "recent_trades": recent_trades[-20:],
                "signals": signal_history[-20:],
                "bot_running": True,
                "bot_status": "Läuft",
            })

            # ---- Warten bis zum nächsten Intervall ------------------
            elapsed = time.time() - loop_start
            wait = max(10, interval_s - elapsed)
            logger.debug("Loop-Dauer: %.1fs | Warte %.0fs", elapsed, wait)
            time.sleep(wait)

        except KeyboardInterrupt:
            logger.info("Bot durch Benutzer gestoppt.")
            update_state({"bot_running": False, "bot_status": "Gestoppt"})
            break
        except Exception as exc:
            logger.exception("Fehler im Trading-Loop: %s", exc)
            time.sleep(30)


def _timeframe_to_seconds(tf: str) -> int:
    units = {"m": 60, "h": 3600, "d": 86400}
    if tf[-1] in units:
        return int(tf[:-1]) * units[tf[-1]]
    return 900  # default: 15 Minuten


def main() -> None:
    parser = argparse.ArgumentParser(description="MQG Trading Bot")
    parser.add_argument("--live", action="store_true", help="Echtes Trading aktivieren (ACHTUNG!)")
    parser.add_argument("--no-web", action="store_true", help="Web-Dashboard deaktivieren")
    parser.add_argument("--symbols", nargs="+", help="Handelssymbole überschreiben")
    args = parser.parse_args()

    logger.info("=" * 60)
    logger.info("  MQG Trading Bot startet")
    logger.info("=" * 60)

    # Konfiguration laden
    cfg = load_config()

    if args.live:
        cfg["broker"]["sandbox"] = False
        logger.warning("⚠️  LIVE-TRADING AKTIVIERT – Echtes Kapital!")
    else:
        cfg["broker"]["sandbox"] = True
        logger.info("Sandbox-Modus aktiv – kein echtes Kapital")

    if args.symbols:
        cfg["trading"]["symbols"] = args.symbols

    # Broker und Exchange aufbauen
    broker = build_broker(cfg)
    exchange = broker.get_exchange()
    fetcher = MarketDataFetcher(exchange)

    # Risk Manager und Strategie initialisieren
    risk = RiskManager(
        initial_capital=cfg["risk"]["initial_capital_eur"],
        risk_per_trade_pct=cfg["risk"]["risk_per_trade_pct"],
        stop_loss_pct=cfg["risk"]["stop_loss_pct"],
        take_profit_pct=cfg["risk"]["take_profit_pct"],
        max_drawdown_pct=cfg["risk"]["max_drawdown_pct"],
        max_open_trades=cfg["trading"]["max_open_trades"],
        trailing_stop=cfg["risk"]["trailing_stop"],
        compound_reinvest=cfg["trading"]["compound_reinvest"],
    )
    strategy = MQGStrategy(cfg["mqg"], risk)

    # Web-Dashboard in separatem Thread starten
    if not args.no_web:
        web_cfg = cfg.get("webapp", {})
        web_thread = threading.Thread(
            target=run_dashboard,
            kwargs={"host": web_cfg.get("host", "0.0.0.0"), "port": web_cfg.get("port", 5050)},
            daemon=True,
        )
        web_thread.start()
        logger.info(
            "Dashboard: http://localhost:%d  (vom Smartphone: http://DEINE-IP:%d)",
            web_cfg.get("port", 5050),
            web_cfg.get("port", 5050),
        )

    # Trading-Loop starten (blockierend)
    trading_loop(cfg, strategy, fetcher, broker)


if __name__ == "__main__":
    main()
