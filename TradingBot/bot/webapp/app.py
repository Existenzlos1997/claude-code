"""
Web-Dashboard – Flask + Socket.IO + PWA
========================================
Smartphone-installierbar: Im Browser öffnen → "Zum Startbildschirm hinzufügen"
"""

from __future__ import annotations

import json
import logging
import os
from pathlib import Path

from flask import Flask, jsonify, render_template, send_from_directory
from flask_socketio import SocketIO

logger = logging.getLogger(__name__)

BASE_DIR = Path(__file__).parent

app = Flask(
    __name__,
    template_folder=str(BASE_DIR / "templates"),
    static_folder=str(BASE_DIR / "static"),
)
app.config["SECRET_KEY"] = os.environ.get("WEBAPP_SECRET", "trading-bot-secret")
socketio = SocketIO(app, cors_allowed_origins="*", async_mode="eventlet")

# Globaler State-Speicher (wird vom Bot-Loop befüllt)
_state: dict = {
    "capital_eur": 100.0,
    "peak_capital_eur": 100.0,
    "realized_pnl_eur": 0.0,
    "drawdown_pct": 0.0,
    "total_trades": 0,
    "win_rate_pct": 0.0,
    "open_trades": 0,
    "open_positions": [],
    "recent_trades": [],
    "signals": [],
    "bot_running": False,
    "bot_status": "Gestoppt",
}


def update_state(new_data: dict) -> None:
    """Aktualisiert den globalen State und broadcastet an alle Clients."""
    _state.update(new_data)
    socketio.emit("state_update", _state)


# ------------------------------------------------------------------
# Routen
# ------------------------------------------------------------------

@app.route("/")
def index():
    return render_template("index.html")


@app.route("/api/state")
def api_state():
    return jsonify(_state)


@app.route("/manifest.json")
def manifest():
    return send_from_directory(str(BASE_DIR / "static"), "manifest.json")


@app.route("/sw.js")
def service_worker():
    response = send_from_directory(str(BASE_DIR / "static"), "sw.js")
    response.headers["Service-Worker-Allowed"] = "/"
    return response


@app.route("/api/health")
def health():
    return jsonify({"status": "ok"})


# ------------------------------------------------------------------
# Socket.IO Events
# ------------------------------------------------------------------

@socketio.on("connect")
def on_connect():
    logger.info("Dashboard-Client verbunden")
    socketio.emit("state_update", _state)


@socketio.on("request_state")
def on_request_state():
    socketio.emit("state_update", _state)


def run_dashboard(host: str = "0.0.0.0", port: int = 5050) -> None:
    """Startet den Dashboard-Server (blockierend)."""
    logger.info("Dashboard gestartet: http://%s:%d", host, port)
    socketio.run(app, host=host, port=port, debug=False, use_reloader=False)
