var current_cache_name = "v37";
self.addEventListener('install', event => {
    console.log(current_cache_name + ' installing…');
    event.waitUntil(
        caches.open(current_cache_name).then(function(cache) {
          return cache.addAll([
            'svg/plus.svg'
          ]);
        })
      );
  });
  
  self.addEventListener('activate', event => {
    console.log(current_cache_name + ' now ready to handle fetches!');
    event.waitUntil(
      caches.keys().then(function(cacheNames) {
        return Promise.all(
          cacheNames.map(function(cacheName) {
            if (cacheName !== current_cache_name) {
              return caches.delete(cacheName);
            }
          })
        );
      })
    );
  });


  self.addEventListener('fetch', function(event) {
    event.respondWith(caches.match(event.request).then(function(response) {
      // caches.match() always resolves
      // but in case of success response will have value
      if (response !== undefined) {
        return response;
      } else {
        return fetch(event.request).then(function (response) {
          // do not cache because it takes to long to update web-site
          return response;
        }).catch(function () {
          return caches.match('/svg/plus.svg');
        });
      }
    }));
  });