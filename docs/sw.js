// Service Worker für PZR App - pzr-app-1
// Network-first: immer die neueste Version laden, kein Neuinstallieren nötig
const CACHE_NAME = 'pzr-app-v2';
const urlsToCache = [
  './pzr_app.html',
  './manifest.json'
];

// Installation – sofort aktivieren (skipWaiting)
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => cache.addAll(urlsToCache))
      .then(() => self.skipWaiting())
  );
});

// Aktivierung – alte Caches sofort löschen, alle Clients übernehmen
self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(cacheNames =>
      Promise.all(
        cacheNames
          .filter(name => name !== CACHE_NAME)
          .map(name => caches.delete(name))
      )
    ).then(() => self.clients.claim())
  );
});

// Fetch – Network First: frisch vom Server, Cache als Fallback
self.addEventListener('fetch', event => {
  // Nur GET-Requests cachen
  if (event.request.method !== 'GET') return;

  event.respondWith(
    fetch(event.request)
      .then(response => {
        if (!response || response.status !== 200 || response.type !== 'basic') {
          return response;
        }
        // Frische Antwort in Cache schreiben
        caches.open(CACHE_NAME).then(cache => {
          cache.put(event.request, response.clone());
        });
        return response;
      })
      .catch(() => caches.match(event.request)) // Offline-Fallback
  );
});
