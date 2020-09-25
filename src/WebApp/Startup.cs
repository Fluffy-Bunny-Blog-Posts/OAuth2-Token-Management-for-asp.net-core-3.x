using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FluffyBunny.OAuth2TokenManagment.Services;
using FluffyBunny.OAuth2TokenManagment.Models;
using FluffyBunny.OAuth2TokenManagment;
using FluffyBunny.OAuth2TokenManagment.Extensions;

namespace WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostingEnvironment { get; }

        public Startup(
            IConfiguration configuration,
            IHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddManagedTokenServices();

            services.AddDbContext<ApplicationDbContext>(config =>
            {
                // for in memory database  
                config.UseInMemoryDatabase("InMemoryDataBase");
            });
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllers();
            IMvcBuilder builder = services.AddRazorPages();
            if (HostingEnvironment.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
           IApplicationBuilder app,
           IWebHostEnvironment env,
           IServiceProvider serviceProvider,
           ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            var oAuth2CredentialManager = serviceProvider.GetRequiredService<IOAuth2CredentialManager>();

            // Register the credentials for my test OAuth2 service
            oAuth2CredentialManager.AddCredentialsAsync("test", new OAuth2Credentials
            {
                Authority = "https://demo.identityserver.io",
                ClientId = "m2m",
                ClientSecret = "secret"
            }).GetAwaiter().GetResult();

            // Next register the token request.  In this case its a simple client_credentials call
            var globalTokenManager = serviceProvider.GetRequiredService<ITokenManager<GlobalDistributedCacheTokenStorage>>();
            globalTokenManager.AddManagedTokenAsync("test", new ManagedToken
            {
                CredentialsKey = "test",
                RequestFunctionKey = "client_credentials",
                RequestedScope = null // everything
            }).GetAwaiter().GetResult();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
