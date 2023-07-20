using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShopping.Services;

namespace OnlineShopping
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            // Register the IShoppingService and ShoppingService
            services.AddScoped<IShoppingService, ShoppingService>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure other middleware

            app.UseRouting();

            // Configure other routing and endpoints

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
