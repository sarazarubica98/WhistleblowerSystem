using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using WhistleblowerSystem.Business.Services;
using WhistleblowerSystem.Database.DB;
using WhistleblowerSystem.Database.Interfaces;
using WhistleblowerSystem.Database.Repositories;
using WhistleblowerSystem.Initialization;
using WhistleblowerSystem.Shared.Exceptions;

namespace WhistleblowerSystem.Server
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public IConfiguration _configuration;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //services
            //    .AddControllers(opt => opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>())
            //    .AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;                
            //});

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddRazorPages();
            services.AddHttpContextAccessor();

            services.AddAuthorization();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie((options) =>
            {
                options.Cookie.HttpOnly = false;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsOrigins",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader());
            //});

            DependencyInjection.DependencyInjection.Init(services,
                GetConfigValue("DBNAME")!,
                GetConfigValue("MONGODBCONNECTION", true)!,
                GetConfigValue("BLOCKCHAIN_API"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
    
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseCors("CorsOrigins");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });

            InitializeDb(serviceProvider.GetRequiredService<DbContext>(),
                serviceProvider.GetRequiredService<UserService>(),
                serviceProvider.GetRequiredService<FormTemplateService>(),
                serviceProvider.GetRequiredService<IMapper>()).GetAwaiter().GetResult();
        }

        private async Task InitializeDb(DbContext dbContext, UserService userService, FormTemplateService formTemplateService, IMapper mapper)
        {
           await new Initializer(dbContext, userService, formTemplateService, mapper)
                .Init(InitializingMode.CreateIfNotExists);
        }



        private string? GetConfigValue(string name, bool isConnectionString = false)
        {
            string? value = Environment.GetEnvironmentVariable(name);
            if (value == null)
            {
                if (isConnectionString)
                {
                    value = _configuration.GetConnectionString(name);
                }
                else
                {
                    value = _configuration.GetValue<string?>(name);
                }
            }

            return value;
        }
    }
}
