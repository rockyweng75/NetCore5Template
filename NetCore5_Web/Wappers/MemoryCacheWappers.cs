using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore5_Domain.Models;
using NetCore5_Service.Interface;
using System;
using System.Threading.Tasks;

namespace NetCore5_Web.Wappers
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
        }
        private IMemoryCache memoryCache;

      
    }
}
