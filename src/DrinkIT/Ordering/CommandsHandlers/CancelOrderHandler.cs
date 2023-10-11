using DrinkIT.Ordering.Commands;
using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Ordering.CommandsHandlers
{
    public class CancelOrderHandler : OrderCommandHandler<CancelOrder>
    {
        public CancelOrderHandler(ILogger<CancelOrderHandler> logger, IOrderRepository orderRepository) : base(logger, orderRepository) { }

        public override async Task<CommandResult> Handle(CancelOrder command, CancellationToken cancellationToken)
        {
            if (command is null) return new CommandResult(false);

            logger.LogInformation("Processing command: {@Command}", command);

            Order? order = await orderRepository.GetAsync(command.OrderId);

            if (order is null) return new CommandResult($"Order with ID {command.OrderId} not found");

            order.SetCancelledStatus();
            logger.LogInformation("Cancelling order: {@Order}", order);

            orderRepository.Update(order);
            var saveResult = await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new CommandResult(saveResult.Errors, order);
        }
    }
}
