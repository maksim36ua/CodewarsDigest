using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Entity;
using cw_itkpi.Models;
using Microsoft.AspNet.Antiforgery;

namespace cw_itkpi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();

            //AdminPassword = Configuration["AdminPassword"];
            //CodewarsAuthorization = Configuration["CodewarsAuthorization"];
        }

        public IConfigurationRoot Configuration { get; set; }
        //public string AdminPassword { get; set; }
        //public string CodewarsAuthorization { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            //services.AddAntiforgery();            

            services.ConfigureAntiforgery(options => // Enables display in Iframe
            {
                options.SuppressXFrameOptionsHeader = true;
            });

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<UserContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection"]));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseIISPlatformHandler();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
