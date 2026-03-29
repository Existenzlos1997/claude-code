"""
Market Data Fetcher – OHLCV-Daten über ccxt
============================================
"""

from __future__ import annotations

import logging
from typing import Optional

import pandas as pd

logger = logging.getLogger(__name__)


class MarketDataFetcher:
    """Lädt OHLCV-Kerzen über ccxt von einem beliebigen Broker."""

    def __init__(self, exchange) -> None:
        """
        Parameters
        ----------
        exchange : ccxt.Exchange
            Bereits initialisiertes ccxt-Exchange-Objekt.
        """
        self._ex = exchange

    def fetch_ohlcv(
        self,
        symbol: str,
        timeframe: str = "15m",
        limit: int = 200,
    ) -> Optional[pd.DataFrame]:
        """
        Lädt OHLCV-Daten und gibt einen DataFrame zurück.

        Returns None bei Fehler.
        """
        try:
            raw = self._ex.fetch_ohlcv(symbol, timeframe=timeframe, limit=limit)
            if not raw:
                return None
            df = pd.DataFrame(
                raw, columns=["timestamp", "open", "high", "low", "close", "volume"]
            )
            df["timestamp"] = pd.to_datetime(df["timestamp"], unit="ms", utc=True)
            df.set_index("timestamp", inplace=True)
            df = df.astype(float)
            return df
        except Exception as exc:
            logger.error("OHLCV-Fehler %s/%s: %s", symbol, timeframe, exc)
            return None

    def fetch_ticker(self, symbol: str) -> Optional[dict]:
        """Gibt den aktuellen Ticker (Bid/Ask/Last) zurück."""
        try:
            return self._ex.fetch_ticker(symbol)
        except Exception as exc:
            logger.error("Ticker-Fehler %s: %s", symbol, exc)
            return None

    def fetch_balance(self) -> dict:
        """Gibt das Guthaben des Kontos zurück."""
        try:
            balance = self._ex.fetch_balance()
            return balance.get("total", {})
        except Exception as exc:
            logger.error("Balance-Fehler: %s", exc)
            return {}
