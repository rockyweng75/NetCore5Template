using NetCore5_Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCore5_Service.Interface
{
    public interface ISecurityService
    {
        Task<UserInfo> GetUser(string UserId, string ClientIP);

        Task<IList<UserInfo>> GetListAsync(
            int PageIndex = 1,
            int Rowcount = 10);
    }
}
