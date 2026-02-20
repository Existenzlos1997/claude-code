# 📱 Installation & Backend-Setup

## Option 1: Als Web-App nutzen (EINFACH)

**Keine Installation nötig!**
1. `pzr_app.html` im Browser öffnen
2. Als Lesezeichen speichern
3. Fertig!

**Vorteile:**
- ✅ Sofort nutzbar
- ✅ Keine Installation
- ✅ Funktioniert überall

**Nachteile:**
- ❌ Keine Multi-Device-Sync
- ❌ Daten nur im Browser

---

## Option 2: Als PWA installieren (EMPFOHLEN)

### Auf Desktop (Chrome, Edge, Firefox)
1. `pzr_app.html` öffnen
2. Adressleiste → "App installieren" Icon
3. Auf "Installieren" klicken
4. App läuft jetzt wie normale Software!

### Auf Android
1. Chrome öffnen
2. Zur App navigieren
3. Menü → "Zum Startbildschirm hinzufügen"
4. App-Icon erscheint

### Auf iOS (iPhone/iPad)
1. Safari öffnen
2. Zur App navigieren
3. Teilen-Button → "Zum Home-Bildschirm"
4. App-Icon erscheint

**Vorteile:**
- ✅ Installiert wie native App
- ✅ Offline nutzbar
- ✅ Schneller Start
- ✅ Eigenes App-Icon

**Nachteile:**
- ❌ Noch keine Multi-Device-Sync

---

## Option 3: Mit lokalem Backend-Server (ADVANCED)

### Voraussetzungen
- Node.js installiert ODER
- Python installiert ODER
- Beliebiger Webserver

### A) Mit Node.js Backend

**1. Node.js installieren:**
- Windows: https://nodejs.org/de/download/
- Android: Termux → `pkg install nodejs`
- iOS: iSH App → `apk add nodejs npm`

**2. Backend-Server Script erstellen:**

```bash
# Erstelle backend-server.js
```

```javascript
const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = 3000;
let sharedData = {};

const server = http.createServer((req, res) => {
  // CORS Headers für Cross-Origin
  res.setHeader('Access-Control-Allow-Origin', '*');
  res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS');
  res.setHeader('Access-Control-Allow-Headers', 'Content-Type');
  
  if (req.method === 'OPTIONS') {
    res.writeHead(200);
    res.end();
    return;
  }
  
  // Sync-Endpunkt
  if (req.url === '/sync' && req.method === 'GET') {
    res.writeHead(200, {'Content-Type': 'application/json'});
    res.end(JSON.stringify(sharedData));
    return;
  }
  
  if (req.url === '/sync' && req.method === 'POST') {
    let body = '';
    req.on('data', chunk => {
      body += chunk.toString();
    });
    req.on('end', () => {
      try {
        const newData = JSON.parse(body);
        // Merge data (neueste gewinnt)
        Object.keys(newData).forEach(key => {
          if (!sharedData[key] || 
              newData[key].timestamp > sharedData[key].timestamp) {
            sharedData[key] = newData[key];
          }
        });
        res.writeHead(200, {'Content-Type': 'application/json'});
        res.end(JSON.stringify({success: true}));
      } catch (e) {
        res.writeHead(400);
        res.end(JSON.stringify({error: e.message}));
      }
    });
    return;
  }
  
  // HTML-Datei ausliefern
  if (req.url === '/' || req.url === '/pzr_app.html') {
    const filePath = path.join(__dirname, 'pzr_app.html');
    fs.readFile(filePath, (err, data) => {
      if (err) {
        res.writeHead(404);
        res.end('Not found');
      } else {
        res.writeHead(200, {'Content-Type': 'text/html'});
        res.end(data);
      }
    });
    return;
  }
  
  res.writeHead(404);
  res.end('Not found');
});

server.listen(PORT, () => {
  console.log(`\n✅ PZR Backend-Server läuft!`);
  console.log(`📱 Zugriff von diesem Gerät: http://localhost:${PORT}`);
  console.log(`📱 Zugriff von anderen Geräten: http://<YOUR-IP>:${PORT}`);
  console.log(`\nIhre IP finden Sie mit:`);
  console.log(`- Windows: ipconfig`);
  console.log(`- Mac/Linux: ifconfig`);
  console.log(`- Android: Settings → About → Status → IP`);
  console.log(`\nDrücken Sie Strg+C zum Beenden\n`);
});
```

**3. Server starten:**
```bash
node backend-server.js
```

**4. Von anderen Geräten verbinden:**
- Backend-Gerät IP herausfinden (z.B. 192.168.1.100)
- Auf anderen Geräten: http://192.168.1.100:3000

### B) Mit Python Backend (Alternative)

```python
# backend-server.py
from http.server import HTTPServer, BaseHTTPRequestHandler
import json

PORT = 3000
shared_data = {}

class Handler(BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == '/sync':
            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.send_header('Access-Control-Allow-Origin', '*')
            self.end_headers()
            self.wfile.write(json.dumps(shared_data).encode())
        else:
            with open('pzr_app.html', 'rb') as f:
                self.send_response(200)
                self.send_header('Content-type', 'text/html')
                self.end_headers()
                self.wfile.write(f.read())
    
    def do_POST(self):
        if self.path == '/sync':
            content_length = int(self.headers['Content-Length'])
            post_data = self.rfile.read(content_length)
            new_data = json.loads(post_data)
            shared_data.update(new_data)
            
            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.send_header('Access-Control-Allow-Origin', '*')
            self.end_headers()
            self.wfile.write(json.dumps({'success': True}).encode())

print(f'✅ Server läuft auf Port {PORT}')
HTTPServer(('0.0.0.0', PORT), Handler).serve_forever()
```

**Starten:**
```bash
python backend-server.py
```

---

## 🔄 Synchronisation einrichten

**Im Backend-Gerät (Admin):**
1. Backend-Server starten
2. In App: Einstellungen → Backend aktivieren
3. Server-URL eingeben: `http://localhost:3000/sync`
4. "Backend aktivieren" klicken

**In Client-Geräten:**
1. App öffnen
2. Einstellungen → Mit Backend verbinden
3. Server-URL eingeben: `http://192.168.1.100:3000/sync`
4. "Verbinden" klicken
5. Auto-Sync läuft alle 30 Sekunden

---

## ⚠️ Wichtige Hinweise

**Netzwerk:**
- Alle Geräte müssen im selben WLAN sein ODER
- Port-Forwarding einrichten für Internet-Zugriff ODER
- VPN nutzen für sichere Verbindung

**Sicherheit:**
- Backend läuft OHNE Verschlüsselung
- Nur in vertrauenswürdigen Netzwerken nutzen
- Für Produktion: HTTPS + Authentifizierung hinzufügen

**Firewall:**
- Port 3000 muss freigegeben sein
- Windows: Firewall-Regel erstellen
- Android/iOS: Normalerweise kein Problem

---

## 🚀 Zusammenfassung

| Methode | Installation | Sync | Komplexität |
|---------|-------------|------|-------------|
| Web-App | ❌ Keine | ❌ Nein | ⭐ Einfach |
| PWA | ✅ Ja | ❌ Nein | ⭐⭐ Mittel |
| Backend | ✅ Ja | ✅ Ja | ⭐⭐⭐ Advanced |

**Empfehlung:**
1. **Starten:** Als Web-App nutzen
2. **Später:** Als PWA installieren
3. **Bei Bedarf:** Backend-Server einrichten

Die App funktioniert JETZT SOFORT ohne Installation!
Für Multi-Device-Sync brauchen Sie Backend-Server.
