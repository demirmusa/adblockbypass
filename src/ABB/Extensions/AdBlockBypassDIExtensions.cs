using System;
using ABB.Caching;
using ABB.KeyProvider;
using ABB.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace ABB.Extensions
{
    public static class AdBlockBypassDIExtensions
    {
        public static IServiceCollection AddAdBlockBypass(this IServiceCollection services,
            Action<ABBOptions> opt)
        {
            services.Configure<ABBOptions>(opt);

            services.AddSingleton<StringReplacer>();
            services.AddSingleton<HtmlReplacer>();
            services.AddSingleton<FileProvider>();
            services.AddSingleton<IUniqueKeyProvider, UniqueKeyProvider>();
            services.AddSingleton<ICacheManager, CacheManager>();

            return services;
        }
    }
}
