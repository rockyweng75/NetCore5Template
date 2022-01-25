using AspectCore.Configuration;
using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NetCore5_Service.Aop;
using NetCore5_Service.Interface;

namespace NetCore5_Service
{
    public static class DIServiceExtensions
    {
        public static void RegisterAOP(this IServiceCollection services)
        {
            services.AddSingleton<IInterceptorCollector, InterceptorCollector>();
            services.AddSingleton<IPropertyInjectorFactory, PropertyInjectorFactory>();
            services.AddSingleton<LoggerInterceptor>();
            services.ConfigureDynamicProxy(config =>
            {
                config.Interceptors.AddServiced<LoggerInterceptor>(Predicates.ForService("*Service"));
            });
            services.BuildDynamicProxyProvider();
        }


        public static void RegisterService(this IServiceCollection services)
        {
            services.AddScoped<ISecurityService>();
        }
    
    }
}
