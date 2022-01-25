using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCore5_Domain.Models;
using NetCore5_MVC.Models;
using NetCore5_MVC.Wappers;
using NetCore5_Service.Interface;
using NLog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore5_MVC.Controllers
{
    public class HomeController : GenericController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ISecurityService _securityService;
        private IHttpContextAccessor accessor;
        private IMemoryCacheWappers memoryCache;

        public HomeController(
                ISecurityService _securityService, //帳號驗證服務
                IConfiguration configuration, //設定檔
                IHttpClientFactory httpClientFactory,
                ISessionWapper sessionWapper, // sessionWapper 需自行封裝
                IWebHostEnvironment environment, //主機環境
                IHttpContextAccessor accessor,
                IMemoryCacheWappers memoryCache
            )
        {
            config = configuration;
            env = environment;
            clientFactory = httpClientFactory;
            this.sessionWapper = sessionWapper;
            this._securityService = _securityService;
            this.accessor = accessor;
            this.memoryCache = memoryCache;

        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Index(string type)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignIn(string id, string password, string returnUrl)
        {

            var loginAccount = User.Identity.Name;
            var ip = accessor.HttpContext.Connection.RemoteIpAddress.ToString();

            UserInfo userInfo = await _securityService.GetUser(id, loginAccount);

            if (IsDev() || userInfo.Password == password) 
            {
                if (userInfo == null) return Forbidden();
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    GetPrincipal(userInfo),
                    new AuthenticationProperties { IsPersistent = true });

                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("Index", "Home", null);
                else
                    return Redirect(returnUrl);
            }
            return this.Forbidden();
        }
 
        private ClaimsPrincipal GetPrincipal(UserInfo e)
        {
            return new ClaimsPrincipal(GetIdentity(e));
        }

        private ClaimsIdentity GetIdentity(UserInfo e)
        {
            return new ClaimsIdentity(new List<Claim>
            {
                new Claim("ID", e.UserId),
                new Claim("Name", e.UserName),
                new Claim("DeptCode", e.DeptCode),
                new Claim("Role", string.Join(',', e.Roles)),
            }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(
                 CookieAuthenticationDefaults.AuthenticationScheme
                );
            if (IsDev()) return RedirectToAction("Index");
            return Redirect("Login");
        }
      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
