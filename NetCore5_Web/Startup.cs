using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetCore5_Data;
using NetCore5_Domain;
using NetCore5_Service;
using NetCore5_Web;
using NetCore5_Web.Middlewares;
using System;

namespace eFormAPI
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
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddControllers();
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
                // ���ШD�A�w�]�OLax�A�bpost�Bput�Bdelete�|������cookie
                options.Cookie.SameSite = SameSiteMode.None;
                //options.Cookie.Path = "CourseSelectionWebApi";
                //options.Cookie.Domain = MyAllowSpecificOrigins;
                options.Cookie.IsEssential = true; // make the session cookie Essential
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = "mywebsite";
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = false;
            });

            services.AddHttpClient();

            var config = new Config();
            services.AddSingleton<Config>(config);
            Configuration.GetSection(Config.Mode).Bind(config);

            services.RegisterJWT(config.SecurityKey);

            //return Data �����j�p�g
            services.AddControllers()
                .AddNewtonsoftJson()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            //services.AddControllers().AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //    options.UseMemberCasing();
            //    options.AllowInputFormatterExceptionMessages = true;
            //});


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "eFormAPI v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCaching();
            app.UseCors("MyAllowSpecificOrigins");
            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }


}
