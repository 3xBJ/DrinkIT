using DrinkIT.Infrastructure.Ordering.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DrinkIT.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddOrderingDbContext(this IServiceCollection services, ConfigurationManager configuration) =>
            services.AddDbContext<OrderingContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
            });

    }
}
