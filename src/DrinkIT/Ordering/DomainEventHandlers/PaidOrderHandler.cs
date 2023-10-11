using DrinkIT.Domain.Events.Payments;
using DrinkIT.Domain.Models.OrderAggregate;
using MediatR;

namespace DrinkIT.Ordering.DomainEventHandlers
{
    public class PaidOrderHandler : INotificationHandler<PaymentApproved>
    {
        private readonly ILogger<PaidOrderHandler> logger;
        private readonly IOrderRepository orderRepository;

        public PaidOrderHandler(ILogger<PaidOrderHandler> logger, IOrderRepository orderRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(PaymentApproved domainEvent, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing domain event: {@DomainEvent}", domainEvent);

            Order? order = await this.orderRepository.GetAsync(domainEvent.OrderID);

            if (order is null) return;

            order.SetPaidStatusAndId(domainEvent.PaymentId);

            this.orderRepository.Update(order);

            await this.orderRepository
                      .UnitOfWork
                      .SaveEntitiesAsync(cancellationToken);
        }
    }
}
