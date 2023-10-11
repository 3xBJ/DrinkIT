using DrinkIT.Domain.BaseInterfaces;

namespace DrinkIT.Domain.BaseClasses
{
    public abstract class Entity : ObjectWithInternalValidation
    {
        private readonly List<IDomainEvent> domainEvents = new();

        public IReadOnlyList<IDomainEvent> DomainEvents => this.domainEvents;

        public void ClearAllDomainEvents() => this.domainEvents.Clear();

        protected void AddDomainEvent(IDomainEvent domainEvent) => this.domainEvents.Add(domainEvent);
        protected void RemoveDomainEvent(IDomainEvent domainEvent) => this.domainEvents.Remove(domainEvent);
    }
}
