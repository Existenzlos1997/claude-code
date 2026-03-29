"""
Broker-Adapter Basisklasse
==========================
"""

from __future__ import annotations

from abc import ABC, abstractmethod
from typing import Optional


class BaseBroker(ABC):
    """Abstrakte Basisklasse für alle Broker-Adapter."""

    @abstractmethod
    def get_exchange(self):
        """Gibt das ccxt-Exchange-Objekt zurück."""

    @abstractmethod
    def place_order(
        self,
        symbol: str,
        side: str,
        quantity: float,
        price: Optional[float] = None,
    ) -> dict:
        """Platziert eine Order."""

    @abstractmethod
    def cancel_order(self, order_id: str, symbol: str) -> dict:
        """Storniert eine Order."""

    @abstractmethod
    def fetch_open_orders(self, symbol: Optional[str] = None) -> list:
        """Gibt offene Orders zurück."""
