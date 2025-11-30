/**
 * Urban Scum - Service Worker
 * Enables offline play and caching
 */

const CACHE_NAME = 'urban-scum-v1';
const urlsToCache = [
    '/',
    '/index.html',
    '/manifest.json',
    '/src/css/style.css',
    '/src/js/utils.js',
    '/src/js/storage.js',
    '/src/js/audio.js',
    '/src/js/sprites.js',
    '/src/js/entities.js',
    '/src/js/levels.js',
    '/src/js/game.js',
    '/src/js/ui.js',
    '/src/js/ads.js',
    '/src/js/main.js'
];

// Install event
self.addEventListener('install', (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then((cache) => {
                console.log('Opened cache');
                return cache.addAll(urlsToCache);
            })
    );
});

// Fetch event
self.addEventListener('fetch', (event) => {
    event.respondWith(
        caches.match(event.request)
            .then((response) => {
                // Return cached version or fetch from network
                if (response) {
                    return response;
                }
                return fetch(event.request);
            })
    );
});

// Activate event
self.addEventListener('activate', (event) => {
    event.waitUntil(
        caches.keys().then((cacheNames) => {
            return Promise.all(
                cacheNames.map((cacheName) => {
                    if (cacheName !== CACHE_NAME) {
                        console.log('Deleting old cache:', cacheName);
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});
