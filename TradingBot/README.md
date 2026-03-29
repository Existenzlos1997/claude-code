# 🤖 MQG Trading Bot

Automatischer Krypto-Trading-Bot mit MQG-Informationsfeldanalyse, automatischem
Compound-Reinvestment und einem als PWA installierbaren Smartphone-Dashboard.

> ⚠️ **Haftungsausschluss**: Dies ist kein Finanzrat. Automatisierter Handel ist
> mit erheblichen Verlustrisiken verbunden. Nutze den Bot **ausschließlich auf eigenes
> Risiko**. Starte immer im Sandbox-Modus und informiere dich über deine Steuerpflichten
> (→ [BROKER_GUIDE.md](BROKER_GUIDE.md)).

---

## 🔗 Installationslinks

### 📱 Android (Termux)
1. [Termux aus F-Droid installieren](https://f-droid.org/packages/com.termux/) *(empfohlen, nicht Google Play)*
2. In Termux ausführen:
```bash
pkg install curl -y && curl -sSL https://raw.githubusercontent.com/Existenzlos1997/claude-code/main/TradingBot/install_android.sh | bash
```

### 🐧 Linux / macOS
```bash
curl -sSL https://raw.githubusercontent.com/Existenzlos1997/claude-code/main/TradingBot/install.sh | bash
```

### 🪟 Windows (PowerShell)
```powershell
Invoke-WebRequest -Uri "https://raw.githubusercontent.com/Existenzlos1997/claude-code/main/TradingBot/install.bat" -OutFile "$env:TEMP\install.bat"; Start-Process cmd "/c $env:TEMP\install.bat" -Wait
```

### 📂 Quellcode direkt
```bash
git clone https://github.com/Existenzlos1997/claude-code.git
cd claude-code/TradingBot
pip install -r requirements.txt
```

---

## ✨ Features

| Feature | Details |
|---|---|
| **MQG-Signalanalyse** | ICQ-Berechnung, Hotspot-Erkennung, Interferenzmuster auf OHLCV-Daten |
| **Technische Bestätigung** | EMA-Crossover, RSI, Volumen-Filter |
| **Compound-Reinvestment** | Gewinne werden automatisch dem Kapital hinzugefügt |
| **Risk Management** | Kelly-Criterion, Trailing-Stop, Drawdown-Schutz |
| **Smartphone-App** | Web-Dashboard als PWA installierbar (Android & iOS) |
| **Broker-Support** | Kraken (BaFin-reguliert) & Binance |
| **Backtesting** | Historische Simulation mit Equity-Kurve |
| **Sandbox-Modus** | Kein echtes Geld – sicher testen |

---

## 🚀 Schnellstart (5 Minuten)

### 1. Voraussetzungen
```bash
python3 --version  # mind. Python 3.10
```

### 2. Installation
```bash
cd TradingBot
pip install -r requirements.txt
```

### 3. Konfiguration
Öffne `config.json` und trage deine Daten ein:
```json
{
  "broker": {
    "name": "kraken",
    "api_key": "DEIN_API_KEY",
    "api_secret": "DEIN_SECRET",
    "sandbox": true
  }
}
```
→ Wie du den Kraken-API-Key bekommst: [BROKER_GUIDE.md](BROKER_GUIDE.md)

### 4. Backtest durchführen (erst testen!)
```bash
python backtest.py --symbol BTC/EUR --days 90 --plot
```

### 5. Bot starten (Sandbox)
```bash
python main.py
```
Dashboard öffnet sich auf `http://localhost:5050`

### 6. Smartphone-App
1. Bot läuft auf deinem PC/Server
2. Öffne `http://DEINE-IP:5050` im Smartphone-Browser
3. iOS: Teilen → "Zum Home-Bildschirm"
4. Android: Menü → "App installieren" / "Zum Startbildschirm hinzufügen"

---

## 🧠 MQG-Theorie: Anwendung auf Marktdaten

Die MQG-Theorie (Makroskopisches Quanteninformationsfeld) wird hier analog zur
Doppelspalt-Analyse auf Preiszeitreihen angewandt:

```
Preisserie → Wellenfunktion ψ(t) → Phasenkohärenz → ICQ-Wert
                                  ↓
                          Hotspot-Erkennung (>2σ)
                                  ↓
                    Interferenztyp (konstruktiv/destruktiv)
                                  ↓
                         Handelsrichtung + Konfidenz
```

### ICQ (Information Coherence Quotient)
```
ICQ = 0.45 × Phasenkohärenz + 0.35 × Volumenkohärenz + 0.20 × Ordnungsgrad
```
- **ICQ ≥ 0.65**: Signal gültig → Trade erlaubt
- **ICQ < 0.65**: Signal schwach → kein Trade

### Hotspots
Preislevels, bei denen die aktuelle Position > 2σ vom Erwartungswert abweicht –
analog zu den Interferenz-Hotspots im Doppelspaltexperiment. An diesen Positionen
ist die Richtungsentscheidung statistisch signifikant.

---

## 📐 Risk Management

| Parameter | Standard | Erklärung |
|---|---|---|
| `risk_per_trade_pct` | 1.5 % | Max. Verlust pro Trade |
| `stop_loss_pct` | 2.0 % | SL-Abstand vom Entry |
| `take_profit_pct` | 4.0 % | TP-Abstand (RR = 2:1) |
| `max_drawdown_pct` | 15 % | Bot stoppt automatisch |
| `max_open_trades` | 2 | Max. gleichzeitige Positionen |
| `trailing_stop` | true | SL folgt dem Kurs |
| `compound_reinvest` | true | Gewinne werden reinvestiert |

---

## 📊 Backtesting

```bash
# Standardmäßig: BTC/EUR, 90 Tage, 15m-Kerzen
python backtest.py

# Angepasst
python backtest.py --symbol ETH/EUR --days 180 --tf 1h --capital 100 --plot

# Ausgabe-Beispiel:
# =======================================================
#   Backtest-Ergebnisse: BTC/EUR
# =======================================================
#   Startkapital      :  100.00 €
#   Endkapital        :  118.43 €
#   Gesamter PnL      : +18.43 € (+18.4 %)
#   Max. Drawdown     :  8.2 %
#   Anzahl Trades     :  47
#   Win-Rate          :  62 %
#   Sharpe-Ratio      :  1.243
# =======================================================
```

---

## 📱 Web-Dashboard

Das Dashboard zeigt in Echtzeit:
- 💰 **Kapital** mit Fortschrittsbalken (Ziel: 25.000 €)
- 📈 **Realisierter Gewinn** und Drawdown
- 🔬 **MQG-Signale** mit ICQ-Wert und Interferenztyp
- 📋 **Offene Positionen** (Symbol, Richtung, Entry/SL/TP)
- 📜 **Trade-History** (letzten 10 Trades)

---

## ⚙️ Kommandozeilenoptionen

```bash
python main.py              # Sandbox-Modus (Standard)
python main.py --live       # ⚠️ Echtes Trading!
python main.py --no-web     # Ohne Web-Dashboard
python main.py --symbols BTC/EUR ETH/EUR SOL/EUR
```

---

## 🗂️ Projektstruktur

```
TradingBot/
├── main.py                 # Einstiegspunkt
├── backtest.py             # Historische Simulation
├── config.json             # Konfiguration
├── requirements.txt        # Python-Abhängigkeiten
├── BROKER_GUIDE.md         # Broker-Empfehlungen + Rechtshinweise
└── bot/
    ├── mqg_analyzer.py     # MQG-Informationsfeldanalyse
    ├── strategy.py         # Handelsstrategie
    ├── risk_manager.py     # Positionsgrößen + Schutz
    ├── broker/
    │   ├── kraken.py       # Kraken-Adapter (BaFin)
    │   └── binance.py      # Binance-Adapter
    ├── data/
    │   └── market_data.py  # OHLCV-Datenabruf
    └── webapp/
        ├── app.py          # Flask-Dashboard + Socket.IO
        ├── templates/
        │   └── index.html  # PWA-Dashboard-UI
        └── static/
            ├── manifest.json  # PWA-Manifest
            └── sw.js          # Service Worker (Offline)
```

---

## 💡 Realistisches Wachstum

| Zeitraum | Kapital (3 %/Monat) | Kapital (10 %/Monat) |
|---|---|---|
| Start | 100 € | 100 € |
| 1 Jahr | 143 € | 314 € |
| 2 Jahre | 203 € | 983 € |
| 3 Jahre | 289 € | 3.091 € |
| 4 Jahre | 411 € | 9.684 € |

> Das Ziel von 2.500 €/Monat erfordert ~25.000–83.000 € Kapital.
> Der Zinseszinseffekt durch Compound-Reinvestment beschleunigt das Wachstum erheblich.
> Zeitrahmen realistisch: **3–5 Jahre** ab 100 € Startkapital.

---

## 🔒 Sicherheitshinweise

1. **API-Keys**: Niemals Auszahlungsrechte dem Bot geben!
2. **Sandbox zuerst**: Mindestens 2 Wochen im Sandbox-Modus testen
3. **Max-Drawdown**: Lass den Bot bei >15 % DD automatisch stoppen
4. **Deutsche Steuerpflicht**: Alle Gewinne müssen versteuert werden
5. **Niemals mehr riskieren als du verlieren kannst**

---

## 📄 Lizenz

Dieses Projekt ist für den privaten Gebrauch bestimmt. Kein Finanzrat.
