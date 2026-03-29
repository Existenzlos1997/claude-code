"""
MQG Analyzer – MQG-Informationsfeld-Signalanalyse für Marktdaten
=================================================================

Die MQG-Theorie (Makroskopische Quantengravitation / Informationsfeld) wird hier auf
Preiszeitreihen angewandt. Analog zur Doppelspaltanalyse werden Preisoszillationen als
Informationsfeldwellen behandelt. Bereiche mit hoher Informationskohärenz (ICQ ≈ 1)
zeigen statistisch signifikante Richtungsentscheidungen des Marktes.

Kernkonzepte:
  - ICQ (Information Coherence Quotient): Maß für die Phasenkohärenz der Preisbewegung
  - Hotspot-Erkennung: Positionen im Preisraum, wo MQG-Abweichungen > 2σ auftreten
  - Interferenzmuster: Konstruktive (bullish) vs. destruktive (bearish) Überlagerungen
"""

from __future__ import annotations

import numpy as np
import pandas as pd
from dataclasses import dataclass, field
from typing import Optional


@dataclass
class MQGSignal:
    """Ergebnis einer MQG-Analyse für einen einzelnen Zeitpunkt."""
    timestamp: pd.Timestamp
    symbol: str
    icq: float                    # 0.0 – 1.0
    direction: int                # +1 (long), -1 (short), 0 (neutral)
    confidence: float             # 0.0 – 1.0
    hotspot_detected: bool
    interference_type: str        # "constructive" | "destructive" | "neutral"
    phase_coherence: float
    information_entropy: float
    raw_scores: dict = field(default_factory=dict)

    @property
    def is_valid(self) -> bool:
        return self.icq >= 0.0 and self.confidence >= 0.0


class MQGAnalyzer:
    """
    Wendet die MQG-Informationsfeldtheorie auf OHLCV-Marktdaten an.

    Methodik:
      1. Preisoszillationen werden als Wellenfunktion ψ(t) modelliert.
      2. Die Informationskohärenz wird über Kreuzkorrelation mehrerer
         Indikatoren berechnet – analog zur Phasenkohärenz in der Quantenmechanik.
      3. Hotspots sind Preisniveaus, wo die statistische Abweichung vom
         Erwartungswert > 2σ beträgt (entspricht den Interferenz-Hotspots
         aus dem Doppelspaltexperiment).
      4. Der ICQ-Wert fasst die Gesamtkohärenz als skalaren Wert zusammen.
    """

    def __init__(
        self,
        icq_threshold: float = 0.65,
        hotspot_sensitivity: float = 0.1,
        window: int = 50,
    ) -> None:
        self.icq_threshold = icq_threshold
        self.hotspot_sensitivity = hotspot_sensitivity
        self.window = window

    # ------------------------------------------------------------------
    # Öffentliche API
    # ------------------------------------------------------------------

    def analyze(self, df: pd.DataFrame, symbol: str = "") -> Optional[MQGSignal]:
        """
        Analysiert den aktuellen Marktkontext und gibt ein MQG-Signal zurück.

        Parameters
        ----------
        df : pd.DataFrame
            OHLCV-DataFrame mit Spalten open, high, low, close, volume.
            Muss mindestens `window` Zeilen enthalten.
        symbol : str
            Handelspaarbezeichnung (nur für Logging).

        Returns
        -------
        MQGSignal oder None (wenn zu wenig Daten)
        """
        if len(df) < self.window:
            return None

        closes = df["close"].values.astype(float)
        volumes = df["volume"].values.astype(float)
        highs = df["high"].values.astype(float)
        lows = df["low"].values.astype(float)

        # ---- Wellenfeld aufbauen ----------------------------------------
        psi = self._build_wave_function(closes)

        # ---- Informationskohärenz berechnen -----------------------------
        phase_coherence = self._compute_phase_coherence(psi)
        vol_coherence = self._compute_volume_coherence(volumes, closes)
        entropy = self._information_entropy(closes)
        icq = self._compute_icq(phase_coherence, vol_coherence, entropy)

        # ---- Hotspot-Erkennung ------------------------------------------
        hotspot, interference = self._detect_hotspot(closes, highs, lows)

        # ---- Richtungsbestimmung ----------------------------------------
        direction, confidence = self._determine_direction(
            psi, phase_coherence, vol_coherence, interference
        )

        return MQGSignal(
            timestamp=df.index[-1] if hasattr(df.index[-1], "to_pydatetime")
                       else pd.Timestamp.now(),
            symbol=symbol,
            icq=float(icq),
            direction=direction,
            confidence=float(confidence),
            hotspot_detected=hotspot,
            interference_type=interference,
            phase_coherence=float(phase_coherence),
            information_entropy=float(entropy),
            raw_scores={
                "phase_coherence": float(phase_coherence),
                "volume_coherence": float(vol_coherence),
                "entropy": float(entropy),
            },
        )

    def is_signal_valid(self, signal: MQGSignal) -> bool:
        """Prüft ob das Signal den konfigurierten ICQ-Schwellwert überschreitet."""
        return signal.icq >= self.icq_threshold and signal.direction != 0

    # ------------------------------------------------------------------
    # Interne Berechnungen
    # ------------------------------------------------------------------

    def _build_wave_function(self, closes: np.ndarray) -> np.ndarray:
        """
        Modelliert die Preisserie als komplexe Wellenfunktion.
        Normalisierte Preisänderungen werden als Amplitude, ihre kumulative
        Phase als Winkel interpretiert.
        """
        returns = np.diff(closes[-self.window:])
        if np.std(returns) == 0:
            return np.zeros(len(returns), dtype=complex)
        norm_returns = returns / (np.std(returns) + 1e-12)
        # Kumulative Phase (Rotation im Komplexen)
        phase = np.cumsum(norm_returns) * 2 * np.pi / self.window
        amplitude = np.abs(norm_returns)
        psi = amplitude * np.exp(1j * phase)
        return psi

    def _compute_phase_coherence(self, psi: np.ndarray) -> float:
        """
        Phasenkohärenz: |<e^{iφ}>| – Wert 1 = perfekte Kohärenz, 0 = rein stochastisch.
        Entspricht dem Vektorlängenmaß in der Kreisstatistik.
        """
        if len(psi) == 0 or np.all(psi == 0):
            return 0.0
        phases = np.angle(psi)
        coherence = np.abs(np.mean(np.exp(1j * phases)))
        return float(np.clip(coherence, 0.0, 1.0))

    def _compute_volume_coherence(
        self, volumes: np.ndarray, closes: np.ndarray
    ) -> float:
        """
        Volumenkohärenz: Korrelation zwischen Preisbewegungsrichtung und Volumen.
        Hohes Volumen in Trendrichtung → hohe Kohärenz.
        """
        w = self.window
        v = volumes[-w:]
        c = closes[-w:]
        returns = np.diff(c)
        v_short = v[1:]
        if len(returns) == 0 or np.std(returns) == 0 or np.std(v_short) == 0:
            return 0.5
        corr = np.corrcoef(returns, v_short)[0, 1]
        # Auf [0,1] normieren: hohe positive Korrelation → hohe Kohärenz
        return float(np.clip((corr + 1) / 2, 0.0, 1.0))

    def _information_entropy(self, closes: np.ndarray) -> float:
        """
        Shannonsche Entropie der normalisierten Renditeverteilung.
        Niedrige Entropie = geordnetes System = gute Handelsbedingungen.
        Gibt den auf [0,1] normierten Entropiewert zurück.
        """
        rets = np.diff(closes[-self.window:])
        if len(rets) < 2:
            return 1.0
        counts, _ = np.histogram(rets, bins=10)
        probs = counts / (counts.sum() + 1e-12)
        probs = probs[probs > 0]
        entropy = -np.sum(probs * np.log2(probs))
        max_entropy = np.log2(10)
        return float(np.clip(entropy / max_entropy, 0.0, 1.0))

    def _compute_icq(
        self, phase_coh: float, vol_coh: float, entropy: float
    ) -> float:
        """
        ICQ (Information Coherence Quotient) als gewichtetes Mittel der
        Kohärenzindikatoren. Niedrige Entropie erhöht den ICQ.
        """
        order = 1.0 - entropy   # Ordnungsgrad (inverse Entropie)
        icq = 0.45 * phase_coh + 0.35 * vol_coh + 0.20 * order
        return float(np.clip(icq, 0.0, 1.0))

    def _detect_hotspot(
        self, closes: np.ndarray, highs: np.ndarray, lows: np.ndarray
    ) -> tuple[bool, str]:
        """
        Hotspot-Erkennung analog zum Doppelspalt-Experiment:
        Berechnet die statistische Abweichung der aktuellen Preisposition
        von der erwarteten Verteilung. Abweichung > 2σ → Hotspot.
        """
        w = self.window
        c = closes[-w:]
        h = highs[-w:]
        l = lows[-w:]

        mid = (h + l) / 2
        mean_mid = np.mean(mid)
        std_mid = np.std(mid) + 1e-12

        current_z = (c[-1] - mean_mid) / std_mid
        # Threshold: default sensitivity=0.1 → 2.0 - 0.1*10 = 1.0σ (lenient).
        # Higher sensitivity → lower threshold → more hotspots detected.
        hotspot_threshold = 2.0 - self.hotspot_sensitivity * 10
        hotspot = abs(current_z) > hotspot_threshold

        # Interferenztyp aus Wellenüberlagerung bestimmen
        recent_trend = np.mean(np.diff(c[-5:])) if len(c) >= 6 else 0
        longer_trend = np.mean(np.diff(c[-20:])) if len(c) >= 21 else 0

        if recent_trend * longer_trend > 0:
            interference = "constructive"   # Trends verstärken sich
        elif recent_trend * longer_trend < 0:
            interference = "destructive"    # Trends schwächen sich
        else:
            interference = "neutral"

        return hotspot, interference

    def _determine_direction(
        self,
        psi: np.ndarray,
        phase_coh: float,
        vol_coh: float,
        interference: str,
    ) -> tuple[int, float]:
        """
        Richtungsbestimmung aus dem Wellenfeld-Impuls.
        Gibt (direction, confidence) zurück.
        """
        if len(psi) == 0:
            return 0, 0.0

        # Mittlere Phase als Richtungsindikator
        mean_phase = np.angle(np.mean(psi))
        # Wellenimpuls: positive Phase → steigend, negative → fallend
        raw_direction = np.sign(np.sin(mean_phase))

        # Interferenzmodifikation
        if interference == "constructive":
            modifier = 1.2
        elif interference == "destructive":
            modifier = 0.7
            raw_direction *= -1   # Trendumkehr wahrscheinlicher
        else:
            modifier = 1.0

        confidence = float(np.clip(phase_coh * vol_coh * modifier, 0.0, 1.0))
        direction = int(np.sign(raw_direction)) if confidence > 0.3 else 0
        return direction, confidence
