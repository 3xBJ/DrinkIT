using MediatR;
using DrinkIT.Domain.BaseInterfaces;
using DrinkIT.Infrastructure.Ordering.Contexts;

namespace DrinkIT.Infrastructure.Extensions
{
    internal static class MediatorExtensions
    {
        internal static async Task PublishDomainEventsAsync(this IMediator mediator, OrderingContext dbContext)
        {
            List<IDomainEvent> domainEvents = dbContext.GetDomainEvents().ToList();

            var entitiesEntry = dbContext.GetEntitiesWithDomainEvents().ToList();
            entitiesEntry.ForEach(entry => entry.Entity.ClearAllDomainEvents());

            foreach(IDomainEvent domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
