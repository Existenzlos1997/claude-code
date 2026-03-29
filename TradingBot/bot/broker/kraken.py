"""
Kraken Broker-Adapter
=====================
BaFin-reguliert | Deutschland-konform | 0 € Mindestkapital
"""

from __future__ import annotations

import logging
from typing import Optional

import ccxt

from bot.broker.base import BaseBroker

logger = logging.getLogger(__name__)


class KrakenBroker(BaseBroker):
    """ccxt-basierter Adapter für die Kraken-Exchange."""

    def __init__(self, api_key: str, api_secret: str, sandbox: bool = True) -> None:
        params: dict = {
            "apiKey": api_key,
            "secret": api_secret,
            "enableRateLimit": True,
        }
        self._exchange = ccxt.kraken(params)

        if sandbox:
            # Kraken hat kein offizielles Sandbox; im Sandbox-Modus werden
            # keine echten Orders platziert – nur simuliert.
            logger.warning(
                "Kraken: Sandbox-Modus aktiv – KEINE echten Orders!"
            )
            self._sandbox = True
        else:
            self._sandbox = False

    def get_exchange(self) -> ccxt.kraken:
        return self._exchange

    def place_order(
        self,
        symbol: str,
        side: str,
        quantity: float,
        price: Optional[float] = None,
    ) -> dict:
        if self._sandbox:
            logger.info("[SANDBOX] %s %s %.6f @ %s", side.upper(), symbol, quantity, price)
            return {"id": "SANDBOX", "status": "simulated", "symbol": symbol}
        try:
            if price:
                return self._exchange.create_limit_order(symbol, side, quantity, price)
            return self._exchange.create_market_order(symbol, side, quantity)
        except ccxt.BaseError as exc:
            logger.error("Kraken Order-Fehler: %s", exc)
            raise

    def cancel_order(self, order_id: str, symbol: str) -> dict:
        if self._sandbox:
            return {"id": order_id, "status": "cancelled"}
        try:
            return self._exchange.cancel_order(order_id, symbol)
        except ccxt.BaseError as exc:
            logger.error("Kraken Cancel-Fehler: %s", exc)
            raise

    def fetch_open_orders(self, symbol: Optional[str] = None) -> list:
        if self._sandbox:
            return []
        try:
            return self._exchange.fetch_open_orders(symbol)
        except ccxt.BaseError as exc:
            logger.error("Kraken OpenOrders-Fehler: %s", exc)
            return []
