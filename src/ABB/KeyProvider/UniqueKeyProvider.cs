using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABB.Caching;
using ABB.KeyProvider.Dto;
using ABB.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ABB.KeyProvider
{
    public class UniqueKeyProvider : IUniqueKeyProvider
    {
        private readonly ICacheManager _cache;
        private readonly ABBOptions _options;
        private const string KeyGuidMatchUpDictCacheKey = "AdBlockBypass.GuidManager.KeyGuidMatchUpDictCacheKey";
        public UniqueKeyProvider(IOptions<ABBOptions> options, ICacheManager cache)
        {
            _cache = cache;
            _options = options.Value ??
                       throw new Exception("Define ABBOptions");
        }
        private char GetRandomChar()//pick char between a-z 
        {
            Random rnd = new Random();
            return Convert.ToChar(rnd.Next(97, 123));
        }

        private Dictionary<string, KeyInformation> GetDictFromCache =>
            _cache.Get<Dictionary<string, KeyInformation>>(KeyGuidMatchUpDictCacheKey) ?? new Dictionary<string, KeyInformation>();

        private Dictionary<string, KeyInformation> GenerateKey(string value)
        {
            var dict = GetDictFromCache;

            if (!dict.ContainsKey(value))
            {
                dict.Add(value, new KeyInformation()
                {
                    Value = value,
                    MatchedGuid = GetRandomChar() + Guid.NewGuid().ToString().Replace("-", ""),//names should start with char
                    ExpireTime = DateTime.Now.AddSeconds(_options.CacheExpireTimeSec)
                });
                _cache.Set(KeyGuidMatchUpDictCacheKey, dict, TimeSpan.FromSeconds(_options.CacheExpireTimeSec));
            }
            return dict;
        }

        public KeyInformation GetUniqueKey(string value)
        {
            var dict = GetDictFromCache;

            if (!dict.ContainsKey(value))
                dict = GenerateKey(value);

            if (dict[value].ExpireTime < DateTime.Now)
            {
                dict.Remove(value);
                //this will give a new key to it
                dict = GenerateKey(value);
            }
            return dict[value];
        }

        public KeyInformation GetValueFromMatchedGuid(string matchedGuid)
        {
            var dict = GetDictFromCache;

            return !dict.Values.Select(v => v.MatchedGuid).Contains(matchedGuid)
                 ? null
                 : dict.Values.FirstOrDefault(x => x.MatchedGuid == matchedGuid);
        }
    }
}
