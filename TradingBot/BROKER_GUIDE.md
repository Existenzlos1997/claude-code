# 🏦 Broker-Empfehlungen – Deutschland-konform, 100 € Startkapital

## Zusammenfassung

| Broker | Asset-Klasse | Mindestkapital | Gebühren | BaFin/EU-reguliert | API |
|---|---|---|---|---|---|
| **Kraken** | Crypto | 0 € | 0,16 % Maker / 0,26 % Taker | BaFin (Kraken Bank) | ✅ REST + WebSocket |
| **Binance** | Crypto | 0 € | 0,10 % (0,075 % mit BNB) | EU Registered (VASP) | ✅ REST + WebSocket |
| **Trade Republic** | Aktien/ETF/Crypto | 0 € | 1 € / Trade | BaFin | ❌ kein offizieller API |
| **Scalable Capital** | Aktien/ETF | 0 € | 0 € (PRIME+) | BaFin | ❌ kein offizieller API |
| **IBKR (Lite)** | Aktien/ETF/Optionen | 0 € | ab 0 € (IBKR Lite) | BaFin-reguliert | ✅ TWS API / REST |

---

## 🥇 Empfehlung für den Trading-Bot

### Primär: **Kraken** (Crypto)
- BaFin-lizenziert (Kraken Bank AG, Berlin)
- 0 € Mindestkapital, keine Inaktivierungsgebühren
- Vollständige REST + WebSocket API, gut dokumentiert
- Maker-Gebühr 0,16 % – bei 100 € Startkapital ≈ 0,16 € pro Trade
- Unterstützt BTC, ETH, ADA, SOL, XRP, DOGE, USDT, EUR-Paare
- **Für den Bot empfohlen**: `XBT/EUR`, `ETH/EUR`, `SOL/EUR`

### Alternative: **Binance** (Crypto)
- Niedrigste Gebühren weltweit (0,075 % mit BNB-Rabatt)
- Sehr liquide Märkte, ideal für Scalping
- Spot- und Futures-Handel (Futures erst ab fortgeschrittener KYC)
- Hinweis: Registrierung als VASP in einigen EU-Ländern, aber **kein BaFin-Konto**

### Für Aktien/ETF: **IBKR** (Interactive Brokers)
- Offiziell BaFin-reguliert, IBKR Central Europe
- 0 € Mindestkapital (IBKR Lite), ab 0,35 € pro Aktientrade
- Professionelle API (TWS API / REST API)
- Für Aktien-Swing-Trading mit dem Bot geeignet

---

## 📋 Deutsche Rechtslage – Was du beachten musst

### Steuerliche Pflichten
- **Crypto (< 1 Jahr Haltezeit):** Gewinne sind steuerpflichtig (Einkommensteuertarif)
- **Crypto (> 1 Jahr Haltezeit):** Gewinne steuerfrei (§ 23 Abs. 1 Nr. 2 EStG)
- **Freigrenze:** 1.000 € Gewinn p.a. steuerfrei (seit 2024; vorher 600 €)
- **Aktien/ETF:** Abgeltungssteuer 25 % + 5,5 % Soli + ggf. KiSt
- **Sparerpauschbetrag:** 1.000 € p.a. (Einzelperson) steuerfrei

### Bot-Betrieb
- Private Nutzung eines automatisierten Handelssystems ist legal
- Kein Erlaubnispflicht nach KWG, solange du **auf eigene Rechnung** handelst
- Gewerbliche Einstufung möglich ab regelmäßigem, umfangreichem Handel
  → Bei hohen Gewinnen Steuerberater konsultieren

### CFD / Hebelprodukte – NICHT empfohlen
- ESMA-Beschränkungen: max. 1:2 Hebel für Retail-Kunden bei Crypto
- Hohe Verlustrisiken, komplexe Steuerbehandlung
- **Für diesen Bot: kein Hebel, nur Spot-Handel**

---

## 🚀 Schnellstart: Kraken API einrichten

1. Konto erstellen: https://www.kraken.com/de-de/sign-up
2. KYC (Tier 1 = nur Email, Tier 2 = Ausweis – für Einzahlungen erforderlich)
3. **API-Schlüssel generieren:** `Account > Security > API > Create API Key`
   - Berechtigungen: `Query Funds`, `Query Orders & Trades`, `Create & Modify Orders`
   - **KEINE** Auszahlungsrechte dem Bot geben!
4. API-Key und Secret in `config.json` eintragen

---

## 💡 Realistische Rendite-Erwartung

| Zeitraum | Startkapital | Realistisches Wachstum (3 % / Monat) | Aggressiv (10 % / Monat) |
|---|---|---|---|
| Start | 100 € | 100 € | 100 € |
| 6 Monate | — | 119 € | 177 € |
| 12 Monate | — | 143 € | 314 € |
| 24 Monate | — | 203 € | 983 € |
| 36 Monate | — | 289 € | 3.091 € |

> **Hinweis:** Das Ziel von 2.500 €/Monat ist mit 100 € Startkapital mathematisch erst bei
> einem Kapitalstock von ~25.000–83.000 € erreichbar (bei 3–10 % monatlicher Rendite).
> Der Bot reinvestiert alle Gewinne automatisch – je größer das Kapital, desto schneller
> das Wachstum (Zinseszinseffekt). Realistischer Zeithorizont: **2–4 Jahre** bis zum Ziel.

---

## 🔗 Weiterführende Links

- [Kraken API-Dokumentation](https://docs.kraken.com/rest/)
- [Binance API-Dokumentation](https://binance-docs.github.io/apidocs/)
- [IBKR TWS API](https://interactivebrokers.github.io/tws-api/)
- [BZSt Steuerinfo Crypto](https://www.bzst.de)
- [BaFin Crypto-Regulierung](https://www.bafin.de/DE/Aufsicht/FinTech/Kryptowerte/kryptowerte_node.html)
