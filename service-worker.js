// Define o nome do cache
const CACHE_NAME = 'my-site-cache-v1';

// Arquivos a serem armazenados em cache
const urlsToCache = [
  '/',
  '/assets/styles/styles.css',
  '/assets/img/logo.png'
];

// Instala o service worker e armazena os arquivos em cache
self.addEventListener('install', function(event) {
  // Perform install steps
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(function(cache) {
        console.log('Cache aberto');
        return cache.addAll(urlsToCache);
      })
  );
});

// Intercepta as requisições e verifica se a resposta já está em cache
self.addEventListener('fetch', function(event) {
  event.respondWith(
    caches.match(event.request)
      .then(function(response) {
        // Resposta encontrada no cache, retorna a resposta em cache
        if (response) {
          return response;
        }
        // A resposta não está no cache, faz a requisição para a rede
        return fetch(event.request)
          .then(function(response) {
            // Faz clone da resposta
            const responseClone = response.clone();
            // Armazena a resposta em cache
            caches.open(CACHE_NAME).then(function(cache) {
              cache.put(event.request, responseClone);
            });
            return response;
          })
          .catch(function() {
            // Se a requisição falhar, retorna uma resposta offline personalizada
            return caches.match('/offline.html');
          });
      }
    )
  );
});

// Atualiza o cache quando um novo Service Worker é ativado
self.addEventListener('activate', function(event) {
  var cacheWhitelist = ['my-site-cache-v2'];
  
  event.waitUntil(
    caches.keys().then(function(cacheNames) {
      return Promise.all(
        cacheNames.map(function(cacheName) {
          if (cacheWhitelist.indexOf(cacheName) === -1) {
            return caches.delete(cacheName);
          }
        })
      );
    })
  );
});
