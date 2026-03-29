"""
Binance Broker-Adapter
======================
Niedrigste Gebühren | Höchste Liquidität | EU-VASP registriert
"""

from __future__ import annotations

import logging
from typing import Optional

import ccxt

from bot.broker.base import BaseBroker

logger = logging.getLogger(__name__)


class BinanceBroker(BaseBroker):
    """ccxt-basierter Adapter für Binance Spot."""

    def __init__(self, api_key: str, api_secret: str, sandbox: bool = True) -> None:
        params: dict = {
            "apiKey": api_key,
            "secret": api_secret,
            "enableRateLimit": True,
            "options": {"defaultType": "spot"},
        }
        self._exchange = ccxt.binance(params)

        if sandbox:
            self._exchange.set_sandbox_mode(True)
            logger.warning("Binance: Sandbox-Modus aktiv – KEINE echten Orders!")
        self._sandbox = sandbox

    def get_exchange(self) -> ccxt.binance:
        return self._exchange

    def place_order(
        self,
        symbol: str,
        side: str,
        quantity: float,
        price: Optional[float] = None,
    ) -> dict:
        try:
            if price:
                return self._exchange.create_limit_order(symbol, side, quantity, price)
            return self._exchange.create_market_order(symbol, side, quantity)
        except ccxt.BaseError as exc:
            logger.error("Binance Order-Fehler: %s", exc)
            raise

    def cancel_order(self, order_id: str, symbol: str) -> dict:
        try:
            return self._exchange.cancel_order(order_id, symbol)
        except ccxt.BaseError as exc:
            logger.error("Binance Cancel-Fehler: %s", exc)
            raise

    def fetch_open_orders(self, symbol: Optional[str] = None) -> list:
        try:
            return self._exchange.fetch_open_orders(symbol)
        except ccxt.BaseError as exc:
            logger.error("Binance OpenOrders-Fehler: %s", exc)
            return []
