const express = require('express');
const WebSocket = require('ws');
const sqlite3 = require('sqlite3').verbose();
const path = require('path');
const fs = require('fs');
const crypto = require('crypto');

const app = express();
const PORT = process.env.PORT || 3000;

// API token — passed via BACKEND_API_TOKEN env var (set by main.js)
// Falls back to a random token if not provided (e.g., standalone dev use)
const API_TOKEN = process.env.BACKEND_API_TOKEN || crypto.randomBytes(24).toString('hex');
// Print token to stdout so main.js can forward it to the renderer
console.log(`__API_TOKEN__:${API_TOKEN}`);

// Middleware: require Bearer token on all /api/* routes except /api/health
function requireApiToken(req, res, next) {
    if (req.path === '/api/health') return next();
    const authHeader = req.headers['authorization'] || '';
    const tokenFromHeader = authHeader.startsWith('Bearer ') ? authHeader.slice(7) : null;
    // Also accept token from query string (?token=...) for URL-embedded auth
    const tokenFromQuery = req.query.token || null;
    const token = tokenFromHeader || tokenFromQuery;
    if (!token || token !== API_TOKEN) {
        return res.status(401).json({ error: 'Unauthorized – ungültiger API-Token.' });
    }
    next();
}
app.use('/api', requireApiToken);

// Simple in-process rate limiter (no extra npm package needed)
function makeRateLimiter(windowMs, maxRequests) {
    const hits = new Map();
    // Periodically clean up expired entries to prevent memory leak
    setInterval(() => {
        const now = Date.now();
        for (const [ip, entry] of hits) {
            if (now > entry.resetAt) hits.delete(ip);
        }
    }, windowMs * 2);
    return function(req, res, next) {
        const ip = req.ip || req.connection.remoteAddress || 'unknown';
        const now = Date.now();
        const entry = hits.get(ip) || { count: 0, resetAt: now + windowMs };
        if (now > entry.resetAt) { entry.count = 0; entry.resetAt = now + windowMs; }
        entry.count++;
        hits.set(ip, entry);
        if (entry.count > maxRequests) {
            return res.status(429).json({ error: 'Too many requests – bitte warte kurz.' });
        }
        next();
    };
}
// Storage endpoints: max 200 requests per 60 s per IP (generous for sync)
const storageRateLimit = makeRateLimiter(60 * 1000, 200);

// Middleware
app.use(express.json({ limit: '10mb' }));
app.use(express.urlencoded({ extended: true }));

// CORS
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
    res.header('Access-Control-Allow-Headers', 'Content-Type, Authorization');
    if (req.method === 'OPTIONS') {
        return res.sendStatus(200);
    }
    next();
});

// Static files
app.use(express.static(path.join(__dirname, 'app')));

// Database setup
const dbPath = path.join(__dirname, 'pzr_database.sqlite');
const db = new sqlite3.Database(dbPath, (err) => {
    if (err) {
        console.error('❌ Database connection error:', err);
    } else {
        console.log('✅ Database connected:', dbPath);
        initDatabase();
    }
});

// Initialize database tables
function initDatabase() {
    const tables = [
        `CREATE TABLE IF NOT EXISTS patients (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            patientNumber TEXT UNIQUE NOT NULL,
            firstName TEXT NOT NULL,
            lastName TEXT NOT NULL,
            dateOfBirth TEXT,
            address TEXT,
            city TEXT,
            zip TEXT,
            phone TEXT,
            email TEXT,
            emergencyContact TEXT,
            emergencyPhone TEXT,
            medicalHistory TEXT,
            dentalHistory TEXT,
            pin TEXT,
            status TEXT DEFAULT 'active',
            createdAt TEXT DEFAULT CURRENT_TIMESTAMP,
            updatedAt TEXT DEFAULT CURRENT_TIMESTAMP
        )`,
        
        `CREATE TABLE IF NOT EXISTS appointments (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            patientNumber TEXT NOT NULL,
            date TEXT NOT NULL,
            time TEXT NOT NULL,
            type TEXT,
            location TEXT,
            status TEXT DEFAULT 'active',
            notes TEXT,
            createdAt TEXT DEFAULT CURRENT_TIMESTAMP,
            updatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (patientNumber) REFERENCES patients(patientNumber)
        )`,
        
        `CREATE TABLE IF NOT EXISTS recommendations (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            patientNumber TEXT NOT NULL,
            customField1 TEXT,
            customField2 TEXT,
            customField3 TEXT,
            pzrRecommendation TEXT,
            implantType TEXT,
            notes TEXT,
            createdAt TEXT DEFAULT CURRENT_TIMESTAMP,
            updatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (patientNumber) REFERENCES patients(patientNumber)
        )`,
        
        `CREATE TABLE IF NOT EXISTS documents (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            fileName TEXT NOT NULL,
            fileData TEXT NOT NULL,
            fileSize INTEGER,
            fileType TEXT,
            category TEXT,
            patientNumber TEXT,
            notes TEXT,
            createdAt TEXT DEFAULT CURRENT_TIMESTAMP,
            updatedAt TEXT DEFAULT CURRENT_TIMESTAMP
        )`,
        
        `CREATE TABLE IF NOT EXISTS cancellation_requests (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            appointmentId INTEGER NOT NULL,
            patientNumber TEXT NOT NULL,
            reason TEXT,
            status TEXT DEFAULT 'pending',
            createdAt TEXT DEFAULT CURRENT_TIMESTAMP,
            updatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (appointmentId) REFERENCES appointments(id)
        )`,
        
        `CREATE TABLE IF NOT EXISTS registrations (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            firstName TEXT NOT NULL,
            lastName TEXT NOT NULL,
            dateOfBirth TEXT,
            address TEXT,
            city TEXT,
            zip TEXT,
            phone TEXT,
            email TEXT,
            emergencyContact TEXT,
            emergencyPhone TEXT,
            medicalHistory TEXT,
            dentalHistory TEXT,
            status TEXT DEFAULT 'pending',
            createdAt TEXT DEFAULT CURRENT_TIMESTAMP,
            updatedAt TEXT DEFAULT CURRENT_TIMESTAMP
        )`,
        
        `CREATE TABLE IF NOT EXISTS employees (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT UNIQUE NOT NULL,
            password TEXT NOT NULL,
            role TEXT NOT NULL,
            createdAt TEXT DEFAULT CURRENT_TIMESTAMP,
            updatedAt TEXT DEFAULT CURRENT_TIMESTAMP
        )`,

        // Generic key-value store: mirrors every localStorage key used by the app.
        // This allows ANY device that connects to this backend to get a full sync.
        `CREATE TABLE IF NOT EXISTS kv_store (
            key TEXT PRIMARY KEY,
            value TEXT NOT NULL,
            updatedAt TEXT DEFAULT CURRENT_TIMESTAMP
        )`
    ];

    tables.forEach(sql => {
        db.run(sql, (err) => {
            if (err) {
                console.error('❌ Table creation error:', err);
            }
        });
    });

    console.log('✅ Database tables initialized');
}

// WebSocket Server
const server = app.listen(PORT, () => {
    console.log(`🚀 Server running on http://localhost:${PORT}`);
});

const wss = new WebSocket.Server({ server });

wss.on('connection', (ws) => {
    console.log('👤 New WebSocket client connected');

    ws.on('message', (message) => {
        try {
            const data = JSON.parse(message);
            console.log('📩 Received:', data.type);

            // Broadcast to all clients except sender
            wss.clients.forEach((client) => {
                if (client !== ws && client.readyState === WebSocket.OPEN) {
                    client.send(message);
                }
            });
        } catch (err) {
            console.error('❌ WebSocket message error:', err);
        }
    });

    ws.on('close', () => {
        console.log('👋 Client disconnected');
    });
});

// API Routes

// Sync all data
app.get('/api/sync', (req, res) => {
    const data = {};
    
    db.all('SELECT * FROM patients', (err, patients) => {
        if (err) return res.status(500).json({ error: err.message });
        data.patients = patients;
        
        db.all('SELECT * FROM appointments', (err, appointments) => {
            if (err) return res.status(500).json({ error: err.message });
            data.appointments = appointments;
            
            db.all('SELECT * FROM recommendations', (err, recommendations) => {
                if (err) return res.status(500).json({ error: err.message });
                data.recommendations = recommendations;
                
                db.all('SELECT * FROM documents', (err, documents) => {
                    if (err) return res.status(500).json({ error: err.message });
                    data.documents = documents;
                    
                    db.all('SELECT * FROM cancellation_requests', (err, cancellations) => {
                        if (err) return res.status(500).json({ error: err.message });
                        data.cancellations = cancellations;
                        
                        db.all('SELECT * FROM registrations', (err, registrations) => {
                            if (err) return res.status(500).json({ error: err.message });
                            data.registrations = registrations;
                            
                            res.json(data);
                        });
                    });
                });
            });
        });
    });
});

// Patients
app.get('/api/patients', (req, res) => {
    db.all('SELECT * FROM patients', (err, rows) => {
        if (err) {
            return res.status(500).json({ error: err.message });
        }
        res.json(rows);
    });
});

app.post('/api/patients', (req, res) => {
    const { patientNumber, firstName, lastName, dateOfBirth, address, city, zip, phone, email, emergencyContact, emergencyPhone, medicalHistory, dentalHistory, pin } = req.body;
    
    db.run('INSERT INTO patients (patientNumber, firstName, lastName, dateOfBirth, address, city, zip, phone, email, emergencyContact, emergencyPhone, medicalHistory, dentalHistory, pin) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)',
        [patientNumber, firstName, lastName, dateOfBirth, address, city, zip, phone, email, emergencyContact, emergencyPhone, medicalHistory, dentalHistory, pin],
        function(err) {
            if (err) {
                return res.status(500).json({ error: err.message });
            }
            
            // Broadcast to WebSocket clients
            const message = JSON.stringify({ type: 'patient-created', data: { id: this.lastID, patientNumber } });
            wss.clients.forEach((client) => {
                if (client.readyState === WebSocket.OPEN) {
                    client.send(message);
                }
            });
            
            res.json({ id: this.lastID, patientNumber });
        }
    );
});

app.put('/api/patients/:id', (req, res) => {
    const { id } = req.params;
    const { firstName, lastName, dateOfBirth, address, city, zip, phone, email, emergencyContact, emergencyPhone, medicalHistory, dentalHistory } = req.body;
    
    db.run('UPDATE patients SET firstName=?, lastName=?, dateOfBirth=?, address=?, city=?, zip=?, phone=?, email=?, emergencyContact=?, emergencyPhone=?, medicalHistory=?, dentalHistory=?, updatedAt=CURRENT_TIMESTAMP WHERE id=?',
        [firstName, lastName, dateOfBirth, address, city, zip, phone, email, emergencyContact, emergencyPhone, medicalHistory, dentalHistory, id],
        (err) => {
            if (err) {
                return res.status(500).json({ error: err.message });
            }
            
            // Broadcast update
            const message = JSON.stringify({ type: 'patient-updated', data: { id } });
            wss.clients.forEach((client) => {
                if (client.readyState === WebSocket.OPEN) {
                    client.send(message);
                }
            });
            
            res.json({ success: true });
        }
    );
});

app.delete('/api/patients/:id', (req, res) => {
    const { id } = req.params;
    
    db.run('DELETE FROM patients WHERE id=?', [id], (err) => {
        if (err) {
            return res.status(500).json({ error: err.message });
        }
        
        // Broadcast deletion
        const message = JSON.stringify({ type: 'patient-deleted', data: { id } });
        wss.clients.forEach((client) => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(message);
            }
        });
        
        res.json({ success: true });
    });
});

// Similar endpoints for other resources (appointments, recommendations, etc.)
// For brevity, showing pattern - full implementation would include all CRUD for each table

// Sync endpoint for bulk updates
// Accepts structured collections and upserts them into the kv_store
// so that /api/storage consumers see consistent data.
app.post('/api/sync', (req, res) => {
    const { patients, appointments, recommendations, documents, cancellations, registrations } = req.body;

    const keyMap = {
        pzrPatients: patients,
        pzrAppointments: appointments,
        pzrRecommendations: recommendations,
        pzrDocuments: documents,
        pzrCancellationRequests: cancellations,
        pzrRegistrations: registrations
    };

    try {
        const upsertPromises = Object.entries(keyMap)
            .filter(([, val]) => Array.isArray(val))
            .map(([key, val]) => new Promise((resolve, reject) => {
                const value = JSON.stringify(val);
                db.run(
                    'INSERT OR REPLACE INTO kv_store (key, value, updatedAt) VALUES (?, ?, CURRENT_TIMESTAMP)',
                    [key, value],
                    err => { if (err) reject(err); else resolve(); }
                );
            }));

        Promise.all(upsertPromises)
            .then(() => {
                const message = JSON.stringify({ type: 'data-sync', data: { timestamp: new Date().toISOString() } });
                wss.clients.forEach((client) => {
                    if (client.readyState === WebSocket.OPEN) {
                        client.send(message);
                    }
                });
                res.json({ success: true, message: 'Data synchronized' });
            })
            .catch(err => res.status(500).json({ error: err.message }));
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// ─── Generic key-value storage (mirrors localStorage) ─────────────────────
// Rate-limit all /api/storage requests at the path level
app.use('/api/storage', storageRateLimit);

// GET /api/storage          → returns all keys as { key: value, ... }
// GET /api/storage/:key     → returns single value
// POST /api/storage/:key    → upserts a value; body: { value: <any> }
// DELETE /api/storage/:key  → removes a key

app.get('/api/storage', storageRateLimit, (req, res) => {
    db.all('SELECT key, value FROM kv_store', (err, rows) => {
        if (err) return res.status(500).json({ error: err.message });
        const result = {};
        rows.forEach(r => {
            try { result[r.key] = JSON.parse(r.value); } catch (e) {
                console.error('kv_store parse error for key', r.key, e.message);
                result[r.key] = r.value;
            }
        });
        res.json(result);
    });
});

app.get('/api/storage/:key', storageRateLimit, (req, res) => {
    const key = decodeURIComponent(req.params.key);
    db.get('SELECT value FROM kv_store WHERE key=?', [key], (err, row) => {
        if (err) return res.status(500).json({ error: err.message });
        if (!row) return res.json({ value: null });
        try { res.json({ value: JSON.parse(row.value) }); } catch (e) {
            console.error('kv_store parse error for key', key, e.message);
            res.json({ value: row.value });
        }
    });
});

app.post('/api/storage/:key', storageRateLimit, (req, res) => {
    const key = decodeURIComponent(req.params.key);
    const value = JSON.stringify(req.body.value !== undefined ? req.body.value : req.body);
    db.run(
        'INSERT OR REPLACE INTO kv_store (key, value, updatedAt) VALUES (?, ?, CURRENT_TIMESTAMP)',
        [key, value],
        function(err) {
            if (err) return res.status(500).json({ error: err.message });
            // Notify all connected clients that this key changed
            const msg = JSON.stringify({ type: 'storage-updated', key });
            wss.clients.forEach(c => { if (c.readyState === WebSocket.OPEN) c.send(msg); });
            res.json({ success: true });
        }
    );
});

app.delete('/api/storage/:key', storageRateLimit, (req, res) => {
    const key = decodeURIComponent(req.params.key);
    db.run('DELETE FROM kv_store WHERE key=?', [key], err => {
        if (err) return res.status(500).json({ error: err.message });
        res.json({ success: true });
    });
});
// ───────────────────────────────────────────────────────────────────────────

// Health check
app.get('/api/health', (req, res) => {
    res.json({ status: 'ok', timestamp: new Date().toISOString() });
});

// Graceful shutdown
process.on('SIGTERM', () => {
    console.log('🛑 SIGTERM received, closing server...');
    server.close(() => {
        db.close((err) => {
            if (err) {
                console.error('❌ Error closing database:', err);
            }
            console.log('✅ Server and database closed');
            process.exit(0);
        });
    });
});

module.exports = { app, db, wss };
