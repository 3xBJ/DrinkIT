using DrinkIT.Domain.BaseClasses;
using DrinkIT.Domain.BaseInterfaces;
using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Infrastructure.Extensions;
using DrinkIT.Infrastructure.Ordering.Contexts.EFConfigs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DrinkIT.Infrastructure.Ordering.Contexts
{
    public class OrderingContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "ordering";

        private readonly IMediator mediator;

        public OrderingContext(IMediator mediator, DbContextOptions<OrderingContext> options) : base(options)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderedDrink> Drinks { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderedDrinkConfiguration());
        }

        public async Task<UnitOfWorkResult> SaveEntitiesAsync(CancellationToken cancellationToken)
        {
            await mediator.PublishDomainEventsAsync(this);

            if (AnyEntityHasErrors())
            {
                return new()
                {
                    EntitiesHaveErrors = true,
                    EntitiesModified = 0,
                    Errors = GetErrors().ToList()
                };
            }

            int entitiesModified = await base.SaveChangesAsync(cancellationToken);

            return new()
            {
                EntitiesHaveErrors = false,
                EntitiesModified = entitiesModified
            };
        }

        public IEnumerable<EntityEntry<Entity>> GetEntitiesWithDomainEvents() => ChangeTracker
                                                                                     .Entries<Entity>()
                                                                                     .Where(entry => entry.Entity.DomainEvents.Count > 0);

        public IEnumerable<IDomainEvent> GetDomainEvents() => GetEntitiesWithDomainEvents()
                                                                  .SelectMany(entry => entry.Entity.DomainEvents);

        private bool AnyEntityHasErrors() => ChangeTracker
                                                 .Entries<Entity>()
                                                 .Any(entry => entry.Entity.HasErrors);

        private IEnumerable<Error> GetErrors() => ChangeTracker
                                                      .Entries<Entity>()
                                                      .Where(entry => entry.Entity.HasErrors)
                                                      .SelectMany(entry => entry.Entity.ValidationErrors);

    }
}
