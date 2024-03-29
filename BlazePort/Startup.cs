using BlazePort.Data;
using BlazePort.Pages.Home;
using BlazePort.TripCost.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace BlazePort
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTelerikBlazor();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            string modelPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "MLModels", "TripCostModel.zip");
            services.AddSingleton<ITripCostPredictionService>(new TripCostPredictionService(modelPath));
            services.AddScoped<TripConfigurationState>();
            services.AddScoped<DbContext, BlazePortContext>();
            //services.AddDbContext<BlazePortContext>(options => options.UseSqlite("Data Source=BlazePort.db"));
            services.AddEntityFrameworkCosmos();
            services.AddDbContext<BlazePortContext>(options => options.UseCosmos(
                    accountEndpoint: Configuration["CosmosSettings:AccountEndpoint"],
                    accountKey: Configuration["CosmosSettings:AccountKey"],
                    databaseName: Configuration["CosmosSettings:DatabaseName"]
            ));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
