using DrinkIT.Ordering.Commands;
using DrinkIT.Ordering.DTO;
using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Ordering.CommandsHandlers
{
    public class RemoveDrinksHandler : OrderCommandHandler<RemoveDrinksFromOrder>
    {
        public RemoveDrinksHandler(ILogger<RemoveDrinksHandler> logger, IOrderRepository orderRepository) : base(logger, orderRepository) { }

        public override async Task<CommandResult> Handle(RemoveDrinksFromOrder command, CancellationToken cancellationToken)
        {
            if (command is null) return new CommandResult(false);

            logger.LogInformation("Processing command: {@Command}", command);

            Order? order = await orderRepository.GetAsync(command.OrderId);

            if (order is null) return new CommandResult($"Order with ID {command.OrderId} not found");

            foreach (DrinkDTO drinktDTO in command.Drinks)
            {
                OrderedDrink drink = drinktDTO.ToDomain();
                order.RemoveDrink(drink);
            }

            logger.LogInformation("Adding drinks to order: {@Order}", order);

            orderRepository.Update(order);
            var saveResult = await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new CommandResult(saveResult.Errors, order);
        }
    }
}
