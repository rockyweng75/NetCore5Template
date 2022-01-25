using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NetCore5_Web.Security;
using NetCore5_Web.Wappers;
using Newtonsoft.Json.Serialization;
using System.Security.Claims;
using System.Text;

namespace NetCore5_Web
{
    public static class DIServiceExtensions
    {
        public static void RegisterCommon(this IServiceCollection services)
        {
            services.AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.UseMemberCasing();
                    options.AllowInputFormatterExceptionMessages = true;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.AddScoped<ISessionWapper>(context =>
            {
                var contextAccessor = context.GetRequiredService<IHttpContextAccessor>();
                return new SessionWapper(contextAccessor.HttpContext.Session);
            });

            services.AddSingleton<IMemoryCache, MemoryCache>();

            services.AddSingleton<IMemoryCacheWappers, MemoryCacheWappers>();

        }

        public static void RegisterJWT(this IServiceCollection services, string SecurityKey)
        {
            //services.AddSingleton<IAuthorizationRequirement, RoleRequirement>();
            //services.AddSingleton<IAuthorizationHandler, SessionRoleAuthorizationHandler>();
            //services.AddSingleton<IAuthorizationPolicyProvider, RolePolicyProvider>();

            services.AddTransient<IClaimsTransformation, RoleClaimsTransformation>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {

                 // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
                 options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                     //NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                     NameClaimType = ClaimTypes.NameIdentifier,
                     // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                     //RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                     // 一般我們都會驗證 Issuer
                     ValidateIssuer = false,
                     //ValidIssuer = "JwtAuthDemo", // "JwtAuthDemo" 應該從 IConfiguration 取得

                     // 若是單一伺服器通常不太需要驗證 Audience
                     ValidateAudience = false,
                     //ValidAudience = "JwtAuthDemo", // 不驗證就不需要填寫

                     // 一般我們都會驗證 Token 的有效期間
                     ValidateLifetime = true,

                     // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                     ValidateIssuerSigningKey = false,

                     // "1234567890123456" 應該從 IConfiguration 取得
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey)),

                 };
             });
        }
    }
}
