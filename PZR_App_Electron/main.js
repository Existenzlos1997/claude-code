const { app, BrowserWindow, Menu, ipcMain, dialog } = require('electron');
const path = require('path');
const fs = require('fs');
const os = require('os');
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
            contextIsolation: false,
            enableRemoteModule: true
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
ipcMain.on('activate-backend', (event) => {
    if (backendServer) {
        event.reply('backend-status', { active: true, message: 'Backend läuft bereits', url: getServerUrl() });
        return;
    }

    try {
        // Resolve server.js path: inside packaged app it is unpacked from asar
        const serverScript = app.isPackaged
            ? path.join(process.resourcesPath, 'app.asar.unpacked', 'server.js')
            : path.join(__dirname, 'server.js');

        // Use Electron's own Node.js runtime (ELECTRON_RUN_AS_NODE=1) so we
        // don't depend on a separate `node` binary being installed.
        backendServer = spawn(process.execPath, [serverScript], {
            cwd: path.dirname(serverScript),
            env: { ...process.env, PORT: '3000', ELECTRON_RUN_AS_NODE: '1' }
        });

        backendServer.stdout.on('data', (data) => {
            console.log(`Backend: ${data}`);
            event.reply('backend-log', data.toString());
        });

        backendServer.stderr.on('data', (data) => {
            console.error(`Backend Error: ${data}`);
            event.reply('backend-error', data.toString());
        });

        backendServer.on('close', (code) => {
            console.log(`Backend stopped with code ${code}`);
            backendServer = null;
            isBackendActive = false;
            event.reply('backend-stopped');
        });

        isBackendActive = true;
        const url = getServerUrl();
        event.reply('backend-status', { 
            active: true, 
            message: 'Backend erfolgreich gestartet',
            port: 3000,
            url: url
        });
    } catch (error) {
        event.reply('backend-error', error.message);
    }
});

// Stop backend server
ipcMain.on('deactivate-backend', (event) => {
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
});

// Install downloaded update and restart
ipcMain.on('install-update', () => {
    if (autoUpdater) {
        autoUpdater.quitAndInstall();
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
