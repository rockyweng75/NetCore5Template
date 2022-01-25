using AspectCore.DynamicProxy;
using NetCore5_Data.Daos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore5_Service.Aop
{
    public class TransactionalAttribute : AbstractInterceptorAttribute
    {
        IUnitOfWork unitOfWork { get; set; }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            unitOfWork = (IUnitOfWork)context.ServiceProvider.GetServices(typeof(IUnitOfWork))
                .Where(o => o.GetType() == typeof(UnitOfWork)).FirstOrDefault();

            var key = context.Implementation.GetHashCode();

            unitOfWork.BeginTransaction(key);

            try
            {
                await next(context);
                unitOfWork.Commit(key);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
