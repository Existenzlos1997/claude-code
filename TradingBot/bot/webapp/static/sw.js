// MQG Trading Bot – Service Worker (PWA Offline-Support)
const CACHE = 'mqg-bot-v1';
const ASSETS = ['/', '/static/manifest.json'];

self.addEventListener('install', e => {
  e.waitUntil(
    caches.open(CACHE).then(c => c.addAll(ASSETS)).then(() => self.skipWaiting())
  );
});

self.addEventListener('activate', e => {
  e.waitUntil(
    caches.keys().then(keys =>
      Promise.all(keys.filter(k => k !== CACHE).map(k => caches.delete(k)))
    ).then(() => self.clients.claim())
  );
});

self.addEventListener('fetch', e => {
  // API- und Socket.IO-Requests immer live durchlassen
  const url = new URL(e.request.url);
  const isApi = url.pathname.startsWith('/api/') || url.pathname.startsWith('/socket.io/');
  if (isApi) {
    e.respondWith(fetch(e.request));
    return;
  }
  e.respondWith(
    fetch(e.request).catch(() => caches.match(e.request))
  );
});
