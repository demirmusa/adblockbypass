using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace ABB.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _memoryCache;

        public CacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public TItem Get<TItem>(object key)
        {
            return _memoryCache.Get<TItem>(key);
        }

        public void Set<TItem>(object key, TItem item, TimeSpan absoluteExpirationRelativeToNow)
        {
            _memoryCache.Set(key, item, absoluteExpirationRelativeToNow);
        }

    }

}
