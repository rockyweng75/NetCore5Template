
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCore5_Domain;
using NetCore5_Domain.Models;
using NetCore5_Service.Interface;
using NetCore5_Web.Wappers;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CourseSelection.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly ILogger<AuthorizeController> logger;

        public AuthorizeController(
            ILogger<AuthorizeController> logger,
            IConfiguration Configuration,
            ISecurityService service,
            IHttpContextAccessor accessor,
            IMemoryCacheWappers memoryCache,
            ISessionWapper session
            )
        {
            this.logger = logger;
            this.Configuration = Configuration;
            this.service = service;
            this.accessor = accessor;
            this.memoryCache = memoryCache;
            this.session = session;
        }
        private IConfiguration Configuration;
        private ISecurityService service;
        private IHttpContextAccessor accessor;
        private IMemoryCacheWappers memoryCache;
        private ISessionWapper session;

        private bool IsWriteLog()
        {
            if (session != null && session.User != null)
            {
                if (session.User.Roles.Any(o => o == "A"))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        // GET api/values
        [HttpPost("~/signin")]
        public async Task<IActionResult> Login()
        {
            IActionResult response = Unauthorized();

            if (User.Identity.IsAuthenticated)
            {
                var loginAccount = User.Identity.Name;
                var ip = accessor.HttpContext.Connection.RemoteIpAddress.ToString();

                logger.LogInformation("User login:{ID}, {IP}", User.Identity.Name, ip);

                var user = await service.GetUser(loginAccount, ip);
                if (user != null)
                {
                    logger.LogInformation("login success:{loginAccount}, session id:{sessionID}", loginAccount, session.SessionId);
                    session.User = user;

                    var newToken = GenerateJSONWebToken(user);

                    return Ok(new { user = user, token = newToken });
                }
                else
                {
                    logger.LogInformation("login fail : {loginAccount}", loginAccount);
                }
            }
            return response;
        }

        public class LoginUser
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [AllowAnonymous]
        [HttpPost("~/testLogin")]
        public IActionResult DebugLogin([FromBody] LoginUser user)
        {
            if (!string.IsNullOrEmpty(user.Username))
            {
                return Ok(new { token = GenerateToken(user.Username, secret, 60) });
            }
            else
            {
                return BadRequest();
            }
        }

        const string secret = "645917fa-4175-4956-af76-324ef4df86a0";

        private static string GenerateToken(string UserId, string secret, int expireMinutes = 90)
        {

            var provider = new UtcDateTimeProvider();
            var now = provider.GetNow();

            var secondsSinceEpoch = UnixEpoch.GetSecondsSince(now) + (expireMinutes * 60);

            var payload = new Dictionary<string, object>
            {
                { "sub", UserId },
                { "exp", secondsSinceEpoch }
            };


            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);

            return token;
        }

        [AllowAnonymous]
        [HttpGet("~/logout")]
        public async Task<IActionResult> Logout()
        {
            var ip = accessor.HttpContext.Connection.RemoteIpAddress.ToString();

            session.ClearSession();

            logger.LogInformation("User logout:{ID}, {IP}", User.Identity.Name, ip);

            return Ok();
        }


        private string GenerateJSONWebToken(UserInfo userInfo)
        {
            var expireMinutes = 90;
            var provider = new UtcDateTimeProvider();
            var now = provider.GetNow();

            var secondsSinceEpoch = UnixEpoch.GetSecondsSince(now) + (expireMinutes * 60);

            var payload = new List<Claim>();
            payload.Add(new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserId));
            payload.Add(new Claim(JwtRegisteredClaimNames.Exp, secondsSinceEpoch.ToString()));
            payload.Add(new Claim(ClaimTypes.NameIdentifier, userInfo.UserId));
            payload.Add(new Claim(ClaimTypes.Name, userInfo.UserId));

            foreach (var role in userInfo.Roles)
            {
                payload.Add(new Claim(ClaimTypes.Role, role));
            }

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var config = new Config();
            Configuration.GetSection(Config.Mode).Bind(config);

            var token = encoder.Encode(payload, Encoding.UTF8.GetBytes(config.SecurityKey));
            return token;
        }

    }
}
