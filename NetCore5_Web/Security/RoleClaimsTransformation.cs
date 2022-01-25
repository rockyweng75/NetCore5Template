using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using NetCore5_Service.Interface;
using NetCore5_Web.Wappers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore5_Web.Security
{
    public class RoleClaimsTransformation : IClaimsTransformation
    {
        private ISecurityService service;
        private IHttpContextAccessor accessor;
        private IMemoryCacheWappers memoryCache;
        public RoleClaimsTransformation(ISecurityService service,
            IHttpContextAccessor accessor,
            IMemoryCacheWappers memoryCache)
        {
            this.service = service;
            this.accessor = accessor;
            this.memoryCache = memoryCache;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();

            if (!principal.HasClaim(claim => claim.Type == ClaimTypes.Role)
                && !principal.HasClaim(claim => claim.Type == "NotRole"))
            {
                var userId = principal.FindFirst(o => o.Type == ClaimTypes.NameIdentifier).Value;
                var ip = accessor.HttpContext.Connection.RemoteIpAddress.ToString();

                var user = await service.GetUser(userId, ip);
                foreach (var role in user.Roles)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            principal.AddIdentity(claimsIdentity);
            return principal;
        }
    }
}
