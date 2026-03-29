"""
Risk Manager – Positionsgrößen, Stop-Loss und Drawdown-Schutz
============================================================
"""

from __future__ import annotations

import logging
from dataclasses import dataclass, field
from typing import Optional

logger = logging.getLogger(__name__)


@dataclass
class TradeParameters:
    """Berechnete Handelsparameter für eine Position."""
    symbol: str
    direction: int              # +1 long, -1 short
    entry_price: float
    stop_loss: float
    take_profit: float
    position_size_eur: float
    quantity: float             # in Basiswährung
    risk_eur: float
    reward_eur: float
    rr_ratio: float             # Reward/Risk-Ratio


@dataclass
class PortfolioState:
    """Aktueller Portfoliostatus."""
    capital: float
    peak_capital: float
    open_trades: list = field(default_factory=list)
    realized_pnl: float = 0.0
    total_trades: int = 0
    winning_trades: int = 0

    @property
    def drawdown_pct(self) -> float:
        if self.peak_capital <= 0:
            return 0.0
        return (self.peak_capital - self.capital) / self.peak_capital * 100

    @property
    def win_rate(self) -> float:
        if self.total_trades == 0:
            return 0.0
        return self.winning_trades / self.total_trades * 100

    @property
    def open_count(self) -> int:
        return len(self.open_trades)


class RiskManager:
    """
    Verwaltet das Risikomanagement des Trading-Bots:
      - Kelly-Criterion-basierte Positionsgrößen
      - Dynamische Stop-Loss / Take-Profit Berechnung
      - Drawdown-Schutz (Bot-Stopp bei überschrittenem Limit)
      - Compound-Reinvestment (Gewinne werden dem Kapital hinzugefügt)
    """

    def __init__(
        self,
        initial_capital: float = 100.0,
        risk_per_trade_pct: float = 1.5,
        stop_loss_pct: float = 2.0,
        take_profit_pct: float = 4.0,
        max_drawdown_pct: float = 15.0,
        max_open_trades: int = 2,
        trailing_stop: bool = True,
        compound_reinvest: bool = True,
    ) -> None:
        self.risk_per_trade_pct = risk_per_trade_pct
        self.stop_loss_pct = stop_loss_pct
        self.take_profit_pct = take_profit_pct
        self.max_drawdown_pct = max_drawdown_pct
        self.max_open_trades = max_open_trades
        self.trailing_stop = trailing_stop
        self.compound_reinvest = compound_reinvest

        self.portfolio = PortfolioState(
            capital=initial_capital,
            peak_capital=initial_capital,
        )

    # ------------------------------------------------------------------
    # Öffentliche API
    # ------------------------------------------------------------------

    def can_open_trade(self) -> tuple[bool, str]:
        """Prüft ob ein neuer Trade eröffnet werden darf."""
        if self.portfolio.drawdown_pct >= self.max_drawdown_pct:
            return False, f"Max-Drawdown erreicht ({self.portfolio.drawdown_pct:.1f}%)"
        if self.portfolio.open_count >= self.max_open_trades:
            return False, f"Max offene Trades erreicht ({self.max_open_trades})"
        if self.portfolio.capital < 5.0:
            return False, "Kapital zu gering (< 5 €)"
        return True, "OK"

    def calculate_trade(
        self,
        symbol: str,
        direction: int,
        entry_price: float,
        mqg_confidence: float = 0.5,
    ) -> Optional[TradeParameters]:
        """
        Berechnet alle Parameter für einen neuen Trade.

        Die Positionsgröße wird auf Basis des Kelly-Kriteriums skaliert,
        wobei der MQG-Konfidenzwert als Schätzung des Erwartungswerts dient.
        """
        allowed, reason = self.can_open_trade()
        if not allowed:
            logger.warning("Trade abgelehnt: %s", reason)
            return None

        # Risikobetrag pro Trade
        risk_eur = self.portfolio.capital * (self.risk_per_trade_pct / 100)

        # Kelly-Faktor (konservativ gekappt)
        kelly_factor = self._kelly_factor(mqg_confidence)
        adjusted_risk = risk_eur * kelly_factor

        # Stop-Loss / Take-Profit Level
        if direction == 1:  # Long
            sl = entry_price * (1 - self.stop_loss_pct / 100)
            tp = entry_price * (1 + self.take_profit_pct / 100)
        else:               # Short
            sl = entry_price * (1 + self.stop_loss_pct / 100)
            tp = entry_price * (1 - self.take_profit_pct / 100)

        # Positionsgröße aus Risiko und Stop-Loss-Abstand
        sl_distance_pct = abs(entry_price - sl) / entry_price
        if sl_distance_pct <= 0:
            return None
        position_size_eur = adjusted_risk / sl_distance_pct
        position_size_eur = min(position_size_eur, self.portfolio.capital * 0.4)

        quantity = position_size_eur / entry_price
        reward_eur = quantity * abs(tp - entry_price)
        rr = reward_eur / (adjusted_risk + 1e-12)

        return TradeParameters(
            symbol=symbol,
            direction=direction,
            entry_price=entry_price,
            stop_loss=sl,
            take_profit=tp,
            position_size_eur=position_size_eur,
            quantity=quantity,
            risk_eur=adjusted_risk,
            reward_eur=reward_eur,
            rr_ratio=rr,
        )

    def record_trade_result(self, pnl_eur: float, was_win: bool) -> None:
        """Registriert das Ergebnis eines abgeschlossenen Trades."""
        self.portfolio.total_trades += 1
        if was_win:
            self.portfolio.winning_trades += 1

        # Compound-Reinvestment: Gewinn / Verlust dem Kapital hinzufügen
        if self.compound_reinvest:
            self.portfolio.capital += pnl_eur
        else:
            # Nur Verluste abziehen, Gewinne separiert halten
            if pnl_eur < 0:
                self.portfolio.capital += pnl_eur

        self.portfolio.realized_pnl += pnl_eur

        # Peak aktualisieren
        if self.portfolio.capital > self.portfolio.peak_capital:
            self.portfolio.peak_capital = self.portfolio.capital

        logger.info(
            "Trade abgeschlossen | PnL: %.2f € | Kapital: %.2f € | "
            "Drawdown: %.1f%% | Win-Rate: %.0f%%",
            pnl_eur,
            self.portfolio.capital,
            self.portfolio.drawdown_pct,
            self.portfolio.win_rate,
        )

    def update_trailing_stop(
        self, trade: TradeParameters, current_price: float
    ) -> TradeParameters:
        """Aktualisiert den Trailing-Stop einer offenen Position."""
        if not self.trailing_stop:
            return trade

        if trade.direction == 1:  # Long
            new_sl = current_price * (1 - self.stop_loss_pct / 100)
            if new_sl > trade.stop_loss:
                trade.stop_loss = new_sl
        else:                      # Short
            new_sl = current_price * (1 + self.stop_loss_pct / 100)
            if new_sl < trade.stop_loss:
                trade.stop_loss = new_sl

        return trade

    def get_summary(self) -> dict:
        """Gibt eine Zusammenfassung des Portfoliostatus zurück."""
        return {
            "capital_eur": round(self.portfolio.capital, 2),
            "peak_capital_eur": round(self.portfolio.peak_capital, 2),
            "realized_pnl_eur": round(self.portfolio.realized_pnl, 2),
            "drawdown_pct": round(self.portfolio.drawdown_pct, 2),
            "total_trades": self.portfolio.total_trades,
            "win_rate_pct": round(self.portfolio.win_rate, 1),
            "open_trades": self.portfolio.open_count,
        }

    # ------------------------------------------------------------------
    # Interne Helpers
    # ------------------------------------------------------------------

    def _kelly_factor(self, confidence: float) -> float:
        """
        Konservatives Kelly-Kriterium.
        Verwendet confidence als Gewinnwahrscheinlichkeit p,
        mit einem fixen Gewinn/Verlust-Verhältnis von take_profit/stop_loss.
        """
        import numpy as np
        p = max(0.3, min(0.8, confidence))  # Clip auf realistischen Bereich
        odds = self.take_profit_pct / self.stop_loss_pct
        kelly = p - (1 - p) / odds
        # Fractional Kelly (1/4) für konservativen Einsatz
        return float(np.clip(kelly * 0.25, 0.05, 0.5)) if kelly > 0 else 0.1
