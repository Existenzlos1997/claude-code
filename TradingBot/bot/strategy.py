"""
Trading Strategy – MQG-gestützte Swing/Scalping-Strategie
==========================================================

Strategie-Logik:
  1. MQG-Analyse liefert Richtung und ICQ-Wert.
  2. Technische Filter (EMA-Cross, RSI, ATR) bestätigen oder verwerfen das Signal.
  3. Nur wenn MQG-Signal UND technische Bestätigung übereinstimmen, wird ein Trade
     platziert.
  4. Der Risk-Manager berechnet Positionsgröße, SL und TP.
"""

from __future__ import annotations

import logging
from typing import Optional

import numpy as np
import pandas as pd

from bot.mqg_analyzer import MQGAnalyzer, MQGSignal
from bot.risk_manager import RiskManager, TradeParameters

logger = logging.getLogger(__name__)


class MQGStrategy:
    """
    Kombiniert MQG-Informationsfeldanalyse mit klassischen technischen Indikatoren
    für eine robuste, risikoarme Handelsstrategie.
    """

    def __init__(self, mqg_config: dict, risk_manager: RiskManager) -> None:
        self.mqg = MQGAnalyzer(
            icq_threshold=mqg_config.get("icq_threshold", 0.65),
            hotspot_sensitivity=mqg_config.get("hotspot_sensitivity", 0.1),
            window=mqg_config.get("information_field_window", 50),
        )
        self.risk = risk_manager
        self._open_trades: dict[str, TradeParameters] = {}

    # ------------------------------------------------------------------
    # Signal-Erzeugung
    # ------------------------------------------------------------------

    def generate_signal(
        self, df: pd.DataFrame, symbol: str
    ) -> Optional[TradeParameters]:
        """
        Hauptlogik: Analysiert den Markt und gibt ggf. Handelsparameter zurück.

        Returns None, wenn kein valides Signal vorliegt.
        """
        if len(df) < 60:
            return None

        # --- 1. MQG-Analyse -------------------------------------------
        mqg_signal = self.mqg.analyze(df, symbol)
        if mqg_signal is None or not self.mqg.is_signal_valid(mqg_signal):
            return None

        # --- 2. Technische Bestätigung --------------------------------
        tech_direction = self._technical_confirmation(df)
        if tech_direction == 0:
            return None

        # --- 3. Konsistenzprüfung: MQG und Technik müssen übereinstimmen
        if mqg_signal.direction != tech_direction:
            logger.debug(
                "%s: MQG (%+d) ≠ Technik (%+d) – Signal verworfen",
                symbol,
                mqg_signal.direction,
                tech_direction,
            )
            return None

        # Hotspot-Bonus: Erhöhte Konfidenz an Hotspots (konfigurierbar über hotspot_sensitivity)
        _HOTSPOT_BONUS = 1.15
        confidence = mqg_signal.confidence
        if mqg_signal.hotspot_detected:
            confidence = min(1.0, confidence * _HOTSPOT_BONUS)
            logger.info("%s: Hotspot erkannt! Konfidenz: %.2f", symbol, confidence)

        # --- 5. Risiko-Berechnung -------------------------------------
        current_price = float(df["close"].iloc[-1])
        trade = self.risk.calculate_trade(
            symbol=symbol,
            direction=mqg_signal.direction,
            entry_price=current_price,
            mqg_confidence=confidence,
        )

        if trade is not None:
            logger.info(
                "Signal: %s | %s | ICQ: %.2f | Konf: %.2f | "
                "Preis: %.4f | SL: %.4f | TP: %.4f | Größe: %.2f €",
                symbol,
                "LONG" if trade.direction == 1 else "SHORT",
                mqg_signal.icq,
                confidence,
                trade.entry_price,
                trade.stop_loss,
                trade.take_profit,
                trade.position_size_eur,
            )

        return trade

    def check_exit(
        self, symbol: str, current_price: float
    ) -> Optional[tuple[str, float]]:
        """
        Prüft ob eine offene Position geschlossen werden soll.

        Returns (reason, pnl_eur) oder None.
        """
        trade = self._open_trades.get(symbol)
        if trade is None:
            return None

        # Trailing-Stop aktualisieren
        trade = self.risk.update_trailing_stop(trade, current_price)
        self._open_trades[symbol] = trade

        pnl_eur = self._calculate_pnl(trade, current_price)
        reason = None

        if trade.direction == 1:  # Long
            if current_price <= trade.stop_loss:
                reason = "STOP_LOSS"
            elif current_price >= trade.take_profit:
                reason = "TAKE_PROFIT"
        else:                      # Short
            if current_price >= trade.stop_loss:
                reason = "STOP_LOSS"
            elif current_price <= trade.take_profit:
                reason = "TAKE_PROFIT"

        if reason:
            del self._open_trades[symbol]
            return reason, pnl_eur

        return None

    def register_open_trade(self, trade: TradeParameters) -> None:
        self._open_trades[trade.symbol] = trade

    def get_open_trades(self) -> dict:
        return dict(self._open_trades)

    # ------------------------------------------------------------------
    # Technische Indikatoren
    # ------------------------------------------------------------------

    def _technical_confirmation(self, df: pd.DataFrame) -> int:
        """
        Bestätigt die Richtung über klassische Indikatoren:
          - EMA-Crossover (EMA9 vs EMA21)
          - RSI (Überkauft/Überverkauft)
          - Volumen-Bestätigung
        Gibt +1 (long), -1 (short) oder 0 (kein Signal) zurück.
        """
        closes = df["close"].values.astype(float)
        volumes = df["volume"].values.astype(float)

        if len(closes) < 30:
            return 0

        # EMA-Crossover
        ema9 = self._ema(closes, 9)
        ema21 = self._ema(closes, 21)
        ema_signal = 1 if ema9[-1] > ema21[-1] else -1

        # RSI
        rsi = self._rsi(closes, 14)
        if rsi > 75:
            rsi_signal = -1   # Überkauft
        elif rsi < 25:
            rsi_signal = 1    # Überverkauft
        else:
            rsi_signal = ema_signal   # RSI neutral → EMA entscheidet

        # Volume-Trend (steigendes Volumen bestätigt Bewegung)
        vol_trend = np.mean(volumes[-3:]) > np.mean(volumes[-10:])

        # Kombination
        if ema_signal == rsi_signal:
            return ema_signal if vol_trend else 0
        return 0

    @staticmethod
    def _ema(data: np.ndarray, period: int) -> np.ndarray:
        """Exponentiell gleitender Durchschnitt."""
        alpha = 2.0 / (period + 1)
        ema = np.zeros_like(data)
        ema[0] = data[0]
        for i in range(1, len(data)):
            ema[i] = alpha * data[i] + (1 - alpha) * ema[i - 1]
        return ema

    @staticmethod
    def _rsi(data: np.ndarray, period: int = 14) -> float:
        """Relative Strength Index (letzter Wert)."""
        if len(data) < period + 1:
            return 50.0
        deltas = np.diff(data[-(period + 1):])
        gains = np.where(deltas > 0, deltas, 0.0)
        losses = np.where(deltas < 0, -deltas, 0.0)
        avg_gain = np.mean(gains)
        avg_loss = np.mean(losses)
        if avg_loss == 0:
            return 100.0
        rs = avg_gain / avg_loss
        return float(100.0 - (100.0 / (1.0 + rs)))

    @staticmethod
    def _calculate_pnl(trade: TradeParameters, exit_price: float) -> float:
        """Berechnet den realisierten PnL in EUR."""
        if trade.direction == 1:
            return trade.quantity * (exit_price - trade.entry_price)
        else:
            return trade.quantity * (trade.entry_price - exit_price)
