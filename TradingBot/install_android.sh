#!/usr/bin/env bash
# ============================================================
#  MQG Trading Bot – Android / Termux Installer
#
#  📱 Installationslink (Termux auf Android):
#  pkg install curl -y && curl -sSL https://raw.githubusercontent.com/Existenzlos1997/claude-code/main/TradingBot/install_android.sh | bash
# ============================================================
set -e

REPO="https://github.com/Existenzlos1997/claude-code"
RAW="https://raw.githubusercontent.com/Existenzlos1997/claude-code/main"
INSTALL_DIR="$HOME/mqg-trading-bot"

GREEN='\033[0;32m'; YELLOW='\033[1;33m'; RED='\033[0;31m'; NC='\033[0m'

info()  { echo -e "${GREEN}[✔]${NC} $*"; }
warn()  { echo -e "${YELLOW}[!]${NC} $*"; }
error() { echo -e "${RED}[✗]${NC} $*"; exit 1; }

echo ""
echo "╔══════════════════════════════════════════════╗"
echo "║   MQG Trading Bot – Android / Termux         ║"
echo "╚══════════════════════════════════════════════╝"
echo ""

# ── Termux erkennen ─────────────────────────────────────────
if [ -d "/data/data/com.termux" ] || [ -n "$TERMUX_VERSION" ]; then
  IS_TERMUX=true
  info "Termux erkannt ✓"
else
  IS_TERMUX=false
  warn "Kein Termux – versuche normales bash-Umfeld..."
fi

# ── 1. Termux-Pakete aktualisieren und installieren ─────────
if [ "$IS_TERMUX" = true ]; then
  info "Aktualisiere Termux-Pakete..."
  pkg update -y -q 2>/dev/null || true

  info "Installiere python..."
  pkg install python -y -q 2>/dev/null || error "Python-Installation fehlgeschlagen"

  info "Installiere git..."
  pkg install git -y -q 2>/dev/null || warn "git nicht installierbar – nutze direkten Download"
else
  # Standard Linux-Umgebung (z.B. iSH auf iOS)
  if ! command -v python3 &>/dev/null && ! command -v python &>/dev/null; then
    error "Python 3 wird benötigt. Bitte installieren."
  fi
fi

# ── Python-Befehl ermitteln ─────────────────────────────────
if command -v python3 &>/dev/null; then
  PY=python3
elif command -v python &>/dev/null; then
  PY=python
else
  error "Python nicht gefunden."
fi
info "Python: $($PY --version)"

# ── 2. pip sicherstellen ────────────────────────────────────
if ! $PY -m pip --version &>/dev/null; then
  warn "pip fehlt – installiere..."
  if [ "$IS_TERMUX" = true ]; then
    pkg install python-pip -y -q 2>/dev/null || curl -sSL https://bootstrap.pypa.io/get-pip.py | $PY
  else
    curl -sSL https://bootstrap.pypa.io/get-pip.py | $PY
  fi
fi

# ── 3. Repository klonen oder aktualisieren ─────────────────
if command -v git &>/dev/null; then
  if [ -d "$INSTALL_DIR/.git" ]; then
    info "Vorhandene Installation wird aktualisiert..."
    git -C "$INSTALL_DIR" pull --ff-only
  else
    info "Klone Repository nach $INSTALL_DIR ..."
    git clone --depth=1 "$REPO" "$INSTALL_DIR"
  fi
else
  warn "git nicht verfügbar – lade Dateien direkt herunter..."
  mkdir -p "$INSTALL_DIR/TradingBot/bot/broker" \
           "$INSTALL_DIR/TradingBot/bot/data" \
           "$INSTALL_DIR/TradingBot/bot/webapp/static" \
           "$INSTALL_DIR/TradingBot/bot/webapp/templates"

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
    curl -sSL "$RAW/$f" -o "$INSTALL_DIR/$f"
    info "Heruntergeladen: $f"
  done
fi

BOT_DIR="$INSTALL_DIR/TradingBot"

# ── 4. Python-Abhängigkeiten installieren ───────────────────
info "Installiere Python-Abhängigkeiten..."
$PY -m pip install -r "$BOT_DIR/requirements.txt" --quiet

# ── 5. Start-Skript erstellen ───────────────────────────────
START_SCRIPT="$HOME/start-trading-bot.sh"
cat > "$START_SCRIPT" << EOF
#!/usr/bin/env bash
cd "$BOT_DIR"
$PY main.py
EOF
chmod +x "$START_SCRIPT"
info "Start-Skript: $START_SCRIPT"

# ── 6. Fertig ───────────────────────────────────────────────
echo ""
echo "╔══════════════════════════════════════════════╗"
echo "║         Installation abgeschlossen! 🎉       ║"
echo "╚══════════════════════════════════════════════╝"
echo ""
info "Installiert in: $BOT_DIR"
echo ""
echo "  Nächste Schritte:"
echo "  ─────────────────────────────────────────────"
echo "  1) API-Key konfigurieren:"
echo "     nano $BOT_DIR/config.json"
echo ""
echo "  2) Backtest (empfohlen – erst testen!):"
echo "     cd $BOT_DIR && $PY backtest.py --symbol BTC/EUR --days 90"
echo ""
echo "  3) Bot starten:"
echo "     bash ~/start-trading-bot.sh"
echo "     → Dashboard: http://localhost:5050"
echo ""
echo "  4) Dashboard als App installieren:"
echo "     Öffne  http://localhost:5050  in Chrome/Firefox"
echo "     → Menü → 'Zum Startbildschirm hinzufügen'"
echo ""
warn "Standardmäßig SANDBOX-Modus (kein echtes Geld)."
warn "Ändere in config.json  \"sandbox\": false  erst nach ausgiebigem Testen!"
echo ""
