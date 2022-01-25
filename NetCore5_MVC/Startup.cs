using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCore5_Data;
using NetCore5_Domain;
using NetCore5_MVC.Middlewares;
using NetCore5_Service;
using System;

namespace NetCore5_MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddControllersWithViews();

            services.RegisterConnection(Configuration, "SqlServer");
            services.RegisterService();
            services.RegisterDao();

            services.RegisterCommon();

            // AOP
            services.RegisterAOP();

            services.AddCors(options =>
            {
                options.AddPolicy("MyAllowSpecificOrigins",
                    builder =>
                    {
                        builder
                        // for test
                        .SetIsOriginAllowed(hostname => true)
                        // for publish
                        //.WithOrigins(MyAllowSpecificOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // 跨域請求，預設是Lax，在post、put、delete會拿不到cookie
                options.Cookie.SameSite = SameSiteMode.None;
                //options.Cookie.Path = "CourseSelectionWebApi";
                //options.Cookie.Domain = MyAllowSpecificOrigins;
                options.Cookie.IsEssential = true; // make the session cookie Essential
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = "yunwebsite";
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = false;
            });

            services.AddHttpClient();

            var config = new Config();
            services.AddSingleton<Config>(config);
            Configuration.GetSection(Config.Mode).Bind(config);

            // Cookie Auth
            var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            services.AddAuthentication(scheme)
            .AddCookie(option =>
            {
                option.Cookie.HttpOnly = false;
                option.Cookie.SameSite = SameSiteMode.None;
                //option.Cookie.SameSite = SameSiteMode.Unspecified;

                option.LoginPath = new PathString(Configuration.GetValue<string>("LoginPage"));
                option.LogoutPath = new PathString(Configuration.GetValue<string>("LogoutPage"));
            });

            //return Data 維持大小寫
            services.AddControllers()
                .AddNewtonsoftJson()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCaching();
            app.UseCors("MyAllowSpecificOrigins");
            //app.UseResponseCaching();
            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
