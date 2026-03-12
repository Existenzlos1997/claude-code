const { app, BrowserWindow, Menu, ipcMain, dialog } = require('electron');
const path = require('path');
const fs = require('fs');
const os = require('os');
const crypto = require('crypto');
const { spawn } = require('child_process');

// electron-updater is only available in packaged builds
let autoUpdater = null;
try {
    autoUpdater = require('electron-updater').autoUpdater;
} catch (e) {
    // not installed or running in dev mode without it
}

let mainWindow;
let backendServer = null;
let isBackendActive = false;
let backendApiToken = null; // generated fresh each time the backend starts
let activeTunnel = null;   // localtunnel instance

/** Returns the best LAN IP of this machine so other PCs can connect */
function getServerUrl() {
    const interfaces = os.networkInterfaces();
    for (const name of Object.keys(interfaces)) {
        for (const iface of interfaces[name]) {
            if (iface.family === 'IPv4' && !iface.internal) {
                return `http://${iface.address}:3000`;
            }
        }
    }
    return 'http://localhost:3000';
}

/**
 * Normalise a name string to a safe localtunnel subdomain.
 * e.g. "Dr. Müller & Partner" → "pzr-dr-muller-partner"
 */
function makeTunnelSubdomain(name) {
    return 'pzr-' + (name || 'praxis')
        .toLowerCase()
        .replace(/[äáàâ]/g, 'a').replace(/[öóòô]/g, 'o').replace(/[üúùû]/g, 'u')
        .replace(/ß/g, 'ss')
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/^-+|-+$/g, '')
        .substring(0, 40);
}

/** Close the active tunnel and reset state (safe to call even if no tunnel exists) */
function closeTunnel() {
    if (activeTunnel) {
        try { activeTunnel.close(); } catch (e) {}
        activeTunnel = null;
    }
}

/**
 * Delay before starting the tunnel so the Express server has time to bind.
 * localtunnel tries to connect immediately; 1 second is enough for server.js to start.
 */
const TUNNEL_START_DELAY_MS = 1000;

/**
 * Opens a localtunnel to port 3000.
 * subdomain: slug derived from admin/practice name passed from renderer.
 * Sends IPC events to the renderer window as the tunnel progresses.
 */
async function startTunnel(practiceSlug, senderWindow) {
    // If tunnel already active, just re-send its URL to the renderer
    if (activeTunnel) {
        if (senderWindow && !senderWindow.isDestroyed()) {
            senderWindow.webContents.send('tunnel-status', { active: true, url: activeTunnel.url });
        }
        return;
    }
    try {
        const localtunnel = require('localtunnel');
        const tunnel = await localtunnel({ port: 3000, subdomain: practiceSlug });
        activeTunnel = tunnel;
        const tunnelUrl = tunnel.url;
        console.log(`🌍 Tunnel active: ${tunnelUrl}`);
        if (senderWindow && !senderWindow.isDestroyed()) {
            senderWindow.webContents.send('tunnel-status', { active: true, url: tunnelUrl });
        }
        tunnel.on('close', () => {
            activeTunnel = null;
            if (senderWindow && !senderWindow.isDestroyed()) {
                senderWindow.webContents.send('tunnel-status', { active: false, url: null });
            }
        });
        tunnel.on('error', (err) => {
            console.error('Tunnel error:', err.message);
            activeTunnel = null;
            if (senderWindow && !senderWindow.isDestroyed()) {
                senderWindow.webContents.send('tunnel-status', { active: false, url: null, error: err.message });
            }
        });
    } catch (err) {
        console.error('localtunnel failed:', err.message);
        if (senderWindow && !senderWindow.isDestroyed()) {
            senderWindow.webContents.send('tunnel-status', { active: false, url: null, error: err.message });
        }
    }
}

// Create the main application window
function createWindow() {
    mainWindow = new BrowserWindow({
        width: 1400,
        height: 900,
        minWidth: 1200,
        minHeight: 800,
        icon: path.join(__dirname, 'app', 'icon.png'),
        webPreferences: {
            nodeIntegration: true,
            contextIsolation: false
        },
        autoHideMenuBar: false,
        title: 'PZR App - Zahnärzte Burgau'
    });

    // Load the main HTML file
    mainWindow.loadFile(path.join(__dirname, 'app', 'pzr_app.html'));

    // Open DevTools in development mode
    if (process.env.NODE_ENV === 'development') {
        mainWindow.webContents.openDevTools();
    }

    // Create application menu
    const menuTemplate = [
        {
            label: 'Datei',
            submenu: [
                {
                    label: 'Neu laden',
                    accelerator: 'CmdOrCtrl+R',
                    click: () => mainWindow.reload()
                },
                {
                    label: 'Vollbild',
                    accelerator: 'F11',
                    click: () => {
                        mainWindow.setFullScreen(!mainWindow.isFullScreen());
                    }
                },
                { type: 'separator' },
                {
                    label: 'Beenden',
                    accelerator: 'CmdOrCtrl+Q',
                    click: () => app.quit()
                }
            ]
        },
        {
            label: 'Backend',
            submenu: [
                {
                    label: 'Backend starten',
                    enabled: !isBackendActive,
                    click: () => {
                        mainWindow.webContents.send('start-backend');
                    }
                },
                {
                    label: 'Backend stoppen',
                    enabled: isBackendActive,
                    click: () => {
                        mainWindow.webContents.send('stop-backend');
                    }
                },
                { type: 'separator' },
                {
                    label: 'Backend Status',
                    click: () => {
                        const status = isBackendActive ? 'Backend läuft aktiv' : 'Backend ist gestoppt';
                        dialog.showMessageBox(mainWindow, {
                            type: 'info',
                            title: 'Backend Status',
                            message: status,
                            buttons: ['OK']
                        });
                    }
                }
            ]
        },
        {
            label: 'Hilfe',
            submenu: [
                {
                    label: 'Über PZR App',
                    click: () => {
                        dialog.showMessageBox(mainWindow, {
                            type: 'info',
                            title: 'Über PZR App',
                            message: 'PZR App - Zahnärzte Burgau\nVersion 1.0.0\n\nProfessionelle Zahnreinigung & Patientenverwaltung',
                            buttons: ['OK']
                        });
                    }
                },
                {
                    label: 'DevTools',
                    accelerator: 'CmdOrCtrl+Shift+I',
                    click: () => mainWindow.webContents.openDevTools()
                }
            ]
        }
    ];

    const menu = Menu.buildFromTemplate(menuTemplate);
    Menu.setApplicationMenu(menu);

    mainWindow.on('closed', () => {
        mainWindow = null;
        if (backendServer) {
            backendServer.kill();
        }
    });
}

// Start backend server
// Accepts optional payload: { practiceName: string }
ipcMain.on('activate-backend', (event, payload) => {
    if (backendServer) {
        event.reply('backend-status', { active: true, message: 'Backend läuft bereits', url: getServerUrl() });
        return;
    }

    try {
        // Resolve server.js path: inside packaged app it is unpacked from asar
        const serverScript = app.isPackaged
            ? path.join(process.resourcesPath, 'app.asar.unpacked', 'server.js')
            : path.join(__dirname, 'server.js');

        // Generate a fresh API token for this backend session
        backendApiToken = crypto.randomBytes(24).toString('hex');

        // Use Electron's own Node.js runtime (ELECTRON_RUN_AS_NODE=1) so we
        // don't depend on a separate `node` binary being installed.
        backendServer = spawn(process.execPath, [serverScript], {
            cwd: path.dirname(serverScript),
            env: { ...process.env, PORT: '3000', ELECTRON_RUN_AS_NODE: '1', BACKEND_API_TOKEN: backendApiToken }
        });

        backendServer.stdout.on('data', (data) => {
            const text = data.toString();
            console.log(`Backend: ${text}`);
            event.reply('backend-log', text);
            // Forward API token to renderer so it can authenticate requests
            const tokenMatch = text.match(/__API_TOKEN__:([a-f0-9]+)/);
            if (tokenMatch) {
                backendApiToken = tokenMatch[1];
                if (mainWindow && !mainWindow.isDestroyed()) {
                    mainWindow.webContents.send('backend-api-token', backendApiToken);
                }
            }
        });

        backendServer.stderr.on('data', (data) => {
            console.error(`Backend Error: ${data}`);
            event.reply('backend-error', data.toString());
        });

        backendServer.on('close', (code) => {
            console.log(`Backend stopped with code ${code}`);
            backendServer = null;
            isBackendActive = false;
            closeTunnel();
            event.reply('backend-stopped');
        });

        isBackendActive = true;
        const lanUrl = getServerUrl();
        event.reply('backend-status', { 
            active: true, 
            message: 'Backend erfolgreich gestartet',
            port: 3000,
            url: lanUrl
        });

        // Auto-start internet tunnel — wait for server to bind before connecting
        const practiceName = (payload && payload.practiceName) ? payload.practiceName : 'praxis';
        const slug = makeTunnelSubdomain(practiceName);
        setTimeout(() => startTunnel(slug, mainWindow), TUNNEL_START_DELAY_MS);

    } catch (error) {
        event.reply('backend-error', error.message);
    }
});

// Stop backend server + tunnel
ipcMain.on('deactivate-backend', (event) => {
    closeTunnel();
    if (backendServer) {
        backendServer.kill();
        backendServer = null;
        isBackendActive = false;
        event.reply('backend-status', { active: false, message: 'Backend gestoppt' });
    }
});

// Get backend status
ipcMain.on('get-backend-status', (event) => {
    event.reply('backend-status', { 
        active: isBackendActive,
        port: isBackendActive ? 3000 : null,
        url: isBackendActive ? getServerUrl() : null
    });
    // Re-send token if backend is active (renderer may have reloaded)
    if (isBackendActive && backendApiToken) {
        event.reply('backend-api-token', backendApiToken);
    }
});

// Install downloaded update and restart
ipcMain.on('install-update', () => {
    if (autoUpdater) {
        autoUpdater.quitAndInstall();
    } else {
        console.log('install-update: autoUpdater not available (dev mode or not packaged)');
    }
});

// App lifecycle
app.whenReady().then(() => {
    createWindow();

    // --- Auto-updater (only in packaged app) ---
    if (app.isPackaged && autoUpdater) {
        autoUpdater.autoDownload = true;
        autoUpdater.autoInstallOnAppQuit = true;

        autoUpdater.on('update-available', (info) => {
            if (mainWindow) mainWindow.webContents.send('update-available', info);
        });

        autoUpdater.on('update-downloaded', (info) => {
            if (mainWindow) mainWindow.webContents.send('update-downloaded', info);
        });

        autoUpdater.on('error', (err) => {
            console.error('Auto-updater error:', err.message);
        });

        // Check for updates ~5 seconds after launch
        setTimeout(() => {
            autoUpdater.checkForUpdatesAndNotify().catch(err => {
                console.error('Update check failed:', err.message);
            });
        }, 5000);
    }
    // -------------------------------------------

    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) {
            createWindow();
        }
    });
});

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

app.on('before-quit', () => {
    if (backendServer) {
        backendServer.kill();
    }
});

console.log('PZR App Electron - Main Process gestartet');
