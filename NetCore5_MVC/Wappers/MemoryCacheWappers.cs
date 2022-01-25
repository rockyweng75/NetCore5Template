using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore5_Domain.Models;
using NetCore5_Service.Interface;
using System;
using System.Threading.Tasks;

namespace NetCore5_MVC.Wappers
{
    public interface IMemoryCacheWappers
    {
    }


    public class MemoryCacheWappers : IMemoryCacheWappers
    {

        public MemoryCacheWappers(IMemoryCache memoryCache, IServiceProvider serviceProvider)
        {
            this.memoryCache = memoryCache;

            var scope = serviceProvider.CreateScope();
            securityService = scope.ServiceProvider.GetService<ISecurityService>();
            config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        }
        private IMemoryCache memoryCache;
        private ISecurityService securityService;
        private IConfiguration config;

     
    }
}
