using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace BlazePort
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //Initialize the database
            var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())// Seed COSMOS, disable after seeding
            {
                var db = scope.ServiceProvider.GetRequiredService<BlazePort.Data.BlazePortContext>();
                // prefetch: Ping Cosmos to pool connection and avoid dealy on Index
                // Uncomment to seed Cosmos
                await db.InitializeContainerAsync();
                // End Seed Code
                await db.Locations.FirstOrDefaultAsync();

            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                });
    }
}
