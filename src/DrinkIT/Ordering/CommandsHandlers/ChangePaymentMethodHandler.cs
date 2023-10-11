using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Ordering.Commands;

namespace DrinkIT.Ordering.CommandsHandlers
{
    public class ChangePaymentMethodHandler : OrderCommandHandler<ChangePaymentMethod>
    {
        public ChangePaymentMethodHandler(ILogger<ChangePaymentMethodHandler> logger, IOrderRepository orderRepository) : base(logger, orderRepository) { }

        public override async Task<CommandResult> Handle(ChangePaymentMethod command, CancellationToken cancellationToken)
        {
            if (command is null) return new CommandResult(false);

            logger.LogInformation("Processing command: {@Command}", command);

            Order? order = await this.orderRepository.GetAsync(command.OrderId);

            if (order is null) return new CommandResult($"Order with ID {command.OrderId} not found");

            order.SetPaymentMethod(command.Method);
            logger.LogInformation("Updating payment type: {@Order}", order);

            this.orderRepository.Update(order);
            var saveResult = await this.orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new CommandResult(saveResult.Errors, order);
        }
    }
}
