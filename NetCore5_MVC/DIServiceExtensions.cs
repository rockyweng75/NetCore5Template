using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NetCore5_MVC.Wappers;
using Newtonsoft.Json.Serialization;

namespace NetCore5_MVC
{
    public static class DIServiceExtensions
    {
    
        public static void RegisterCommon(this IServiceCollection services)
        {
            services.AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.UseMemberCasing();
                    options.AllowInputFormatterExceptionMessages = true;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.AddScoped<ISessionWapper>(context =>
            {
                var contextAccessor = context.GetRequiredService<IHttpContextAccessor>();
                return new SessionWapper(contextAccessor.HttpContext.Session);
            });

            services.AddSingleton<IMemoryCache, MemoryCache>();

            services.AddSingleton<IMemoryCacheWappers, MemoryCacheWappers>();

        }
    }
}
