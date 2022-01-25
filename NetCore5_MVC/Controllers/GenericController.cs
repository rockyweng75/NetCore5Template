using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetCore5_MVC.Wappers;
using System.Net.Http;

namespace NetCore5_MVC.Controllers
{
    [Authorize]
    public class GenericController : Controller
    {
        protected IConfiguration config;
        protected IHttpClientFactory clientFactory;
        protected IWebHostEnvironment env;
        protected ISessionWapper sessionWapper;

        protected bool IsDev() => env.IsDevelopment();

        protected ActionResult Forbidden() =>
            StatusCode(401);
        //RedirectToAction("Info", "Home", new { info = "沒有權限檢視此頁" });

    }
}
