
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore5_Data.Daos;
using System;
using System.Linq;

namespace NetCore5_Data
{
    public static class DIServiceExtensions
    {
        /// <summary>
        /// 註冊資料庫
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="configName"></param>
        public static void RegisterConnection(this IServiceCollection services, IConfiguration configuration, string configName)
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void RegisterDao(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>(provider => UseDB<UnitOfWork>(provider));
        }
        /// <summary>
        /// 選擇使用的資料庫
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TService UseDB<TService>(IServiceProvider provider) where TService : class
        {
            var factorys = provider.GetServices<IDbConnectionFactory>();
            var configuration = provider.GetRequiredService<IConfiguration>();

            // TODO filter connection factory
            var factory = factorys
                .FirstOrDefault();

            var paramLength = typeof(TService).GetConstructors().FirstOrDefault().GetParameters().Length;
            var param = new Object[] { factory };
            if (paramLength == 2)
            {
                param = param.Append(configuration).ToArray();
            }
            object obj = Activator.CreateInstance(typeof(TService), param);

            return (TService)obj;
        }

       
    }
}
