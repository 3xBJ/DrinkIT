using DrinkIT.Infrastructure.Ordering.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Testcontainers.MsSql;

namespace DrinkIT.Tests.Infrastructure
{
    public class WebAppWithDBFactory : WebApplicationFactory<Program>
    {
        protected MsSqlContainer databaseContainer = new MsSqlBuilder()
                .WithCleanUp(true)
                .WithName($"TestDBServer-{Guid.NewGuid()}")
                .Build();

        public async Task InicializeDBAsync() => await this.databaseContainer.StartAsync();
        public async Task DisposeDBAsync() => await this.databaseContainer.DisposeAsync();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(async services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<OrderingContext>));
                if (descriptor != null) services.Remove(descriptor);

                var oldDbContext = services.SingleOrDefault(d => d.ServiceType == typeof(OrderingContext));
                if (oldDbContext != null) services.Remove(oldDbContext);

                services.AddDbContext<OrderingContext>(options =>
                { 
                    options.UseSqlServer(this.databaseContainer.GetConnectionString());
                    options.EnableSensitiveDataLogging();
                    options.LogTo(message => Debug.WriteLine(message));
                });

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                using var context = scopedServices.GetRequiredService<OrderingContext>();
                await context.Database.MigrateAsync();
            });
        }
    }
}
