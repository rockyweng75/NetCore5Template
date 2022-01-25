using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NetCore5_Service.Aop
{
    public class LoggerInterceptor : AbstractInterceptor
    {

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            Console.WriteLine("開始記錄日誌");
            var logger = (ILogger<LoggerInterceptor>)context.ServiceProvider.GetService(typeof(ILogger<LoggerInterceptor>));
            try
            {
                logger.LogInformation("{method}, {params}", context.ImplementationMethod, context.Parameters);
                await next.Invoke(context);
            }
            catch (Exception e)
            {

                logger.LogError("{method}, {params}, {exception}", context.ImplementationMethod, context.Parameters, e);
                throw e;
            }
            Console.WriteLine("結束記錄日誌");
        }
    }
}
