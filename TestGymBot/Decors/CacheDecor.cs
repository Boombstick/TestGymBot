using Microsoft.Extensions.Caching.Memory;

namespace TestGymBot.Decors
{
    public class CacheDecor
    {
        private readonly IMemoryCache _memoryCache;
        public CacheDecor(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGetValue(object key, out object? result) => _memoryCache.TryGetValue(key, out result);
        public bool TryGetValue<T,T2>(T key, out T2? result) => _memoryCache.TryGetValue(key, out result);
        public object Get<T>(T key) => _memoryCache.Get(key);
        public void Remove(object key) => _memoryCache.Remove(key);
        public T2 Set<T,T2>(T key, T2 item) => _memoryCache.Set(key, item);
        public T2 Set<T,T2>(T key, T2 value, MemoryCacheEntryOptions? options) => _memoryCache.Set(key, value, options);
        public void UpdateEntry<T,T2>(T key, T2 newItem, MemoryCacheEntryOptions? options=null)=>_memoryCache.Set(key, newItem,options);

    }
}