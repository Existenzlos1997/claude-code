#!/usr/bin/env bash
# ============================================================
#  MQG Trading Bot – Automatischer Installer (Linux / macOS)
#  Installationslink:
#  curl -sSL https://raw.githubusercontent.com/Existenzlos1997/claude-code/main/TradingBot/install.sh | bash
# ============================================================
set -e

REPO="https://github.com/Existenzlos1997/claude-code"
RAW="https://raw.githubusercontent.com/Existenzlos1997/claude-code/main"
INSTALL_DIR="$HOME/mqg-trading-bot"
BRANCH="main"

GREEN='\033[0;32m'; YELLOW='\033[1;33m'; RED='\033[0;31m'; NC='\033[0m'

info()  { echo -e "${GREEN}[✔]${NC} $*"; }
warn()  { echo -e "${YELLOW}[!]${NC} $*"; }
error() { echo -e "${RED}[✗]${NC} $*"; exit 1; }

echo ""
echo "╔══════════════════════════════════════════════╗"
echo "║        MQG Trading Bot – Installer           ║"
echo "╚══════════════════════════════════════════════╝"
echo ""

# ── 1. Python prüfen ────────────────────────────────────────
if command -v python3 &>/dev/null; then
  PY=python3
elif command -v python &>/dev/null; then
  PY=python
else
  error "Python 3.10+ wird benötigt. Bitte installieren: https://www.python.org"
fi

PY_VER=$($PY -c "import sys; print(sys.version_info[:2])")
info "Python gefunden: $($PY --version)"

# ── 2. pip prüfen ───────────────────────────────────────────
if ! $PY -m pip --version &>/dev/null; then
  warn "pip nicht gefunden – versuche Installation..."
  curl -sSL https://bootstrap.pypa.io/get-pip.py | $PY
fi

# ── 3. git prüfen / klonen oder aktualisieren ───────────────
if command -v git &>/dev/null; then
  if [ -d "$INSTALL_DIR/.git" ]; then
    info "Vorhandene Installation wird aktualisiert..."
    git -C "$INSTALL_DIR" pull --ff-only
  else
    info "Klone Repository nach $INSTALL_DIR ..."
    git clone --depth=1 "$REPO" "$INSTALL_DIR"
  fi
else
  # kein git → Dateien einzeln herunterladen
  warn "git nicht gefunden – lade Dateien direkt herunter..."
  mkdir -p "$INSTALL_DIR/TradingBot"
  FILES=(
    "TradingBot/requirements.txt"
    "TradingBot/main.py"
    "TradingBot/backtest.py"
    "TradingBot/config.json"
    "TradingBot/bot/__init__.py"
    "TradingBot/bot/mqg_analyzer.py"
    "TradingBot/bot/strategy.py"
    "TradingBot/bot/risk_manager.py"
    "TradingBot/bot/broker/__init__.py"
    "TradingBot/bot/broker/base.py"
    "TradingBot/bot/broker/kraken.py"
    "TradingBot/bot/broker/binance.py"
    "TradingBot/bot/data/__init__.py"
    "TradingBot/bot/data/market_data.py"
    "TradingBot/bot/webapp/app.py"
    "TradingBot/bot/webapp/static/manifest.json"
    "TradingBot/bot/webapp/static/sw.js"
    "TradingBot/bot/webapp/templates/index.html"
  )
  for f in "${FILES[@]}"; do
    dir="$INSTALL_DIR/$(dirname $f)"
    mkdir -p "$dir"
    curl -sSL "$RAW/$f" -o "$INSTALL_DIR/$f"
  done
fi

BOT_DIR="$INSTALL_DIR/TradingBot"

# ── 4. Abhängigkeiten installieren ──────────────────────────
info "Installiere Python-Abhängigkeiten..."
$PY -m pip install -r "$BOT_DIR/requirements.txt" --quiet

# ── 5. Fertig ───────────────────────────────────────────────
echo ""
echo "╔══════════════════════════════════════════════╗"
echo "║          Installation abgeschlossen!         ║"
echo "╚══════════════════════════════════════════════╝"
echo ""
info "Verzeichnis : $BOT_DIR"
echo ""
echo "  Nächste Schritte:"
echo "  ─────────────────────────────────────────────"
echo "  1) API-Key konfigurieren:"
echo "     nano $BOT_DIR/config.json"
echo ""
echo "  2) Backtest (erst testen!):"
echo "     cd $BOT_DIR && $PY backtest.py --symbol BTC/EUR --days 90 --plot"
echo ""
echo "  3) Bot starten (Sandbox):"
echo "     cd $BOT_DIR && $PY main.py"
echo "     → Dashboard: http://localhost:5050"
echo ""
echo "  4) Smartphone-App:"
echo "     Öffne http://DEINE-IP:5050 im Handy-Browser"
echo "     → 'Zum Startbildschirm hinzufügen'"
echo ""
warn "Der Bot läuft standardmäßig im SANDBOX-Modus (kein echtes Geld)."
warn "config.json → \"sandbox\": false  nur NACH ausgiebigem Testen setzen!"
echo ""
