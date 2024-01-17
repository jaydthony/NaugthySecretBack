using Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.Enitities;

namespace Dating.API.Extension
{
    public static class DbRegisteredExtension
    {
        public static void ConfigureDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<DatingSiteContext>()
                    .AddDefaultTokenProviders();

            services.AddDbContext<DatingSiteContext>(dbContextOptions =>
            {
                var connectionString = configuration["ConnectionStrings:ProdDb"];
                var maxRetryCount = 3;
                var maxRetryDelay = TimeSpan.FromSeconds(10);

                dbContextOptions.UseNpgsql(connectionString, options =>
                {
                    options.EnableRetryOnFailure(maxRetryCount, maxRetryDelay, null);

                });
            });
        }
    }

}