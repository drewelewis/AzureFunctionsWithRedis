using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AzureFunctionsWithRedis
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache((Action<RedisCacheOptions>)(options => options.Configuration = Environment.GetEnvironmentVariable("RedisConnection")));
            builder.Services.Add(ServiceDescriptor.Singleton<IDistributedCache, RedisCache>());
        }
    }
}
