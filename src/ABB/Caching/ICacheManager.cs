using System;
using System.Collections.Generic;
using System.Text;

namespace ABB.Caching
{
    public interface ICacheManager
    {
        TItem Get<TItem>(object key);
        void Set<TItem>(object key, TItem item, TimeSpan absoluteExpirationRelativeToNow);
    }
}
