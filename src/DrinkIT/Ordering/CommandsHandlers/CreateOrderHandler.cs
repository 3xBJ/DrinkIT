using DrinkIT.Ordering.Commands;
using DrinkIT.Ordering.DTO;
using DrinkIT.Domain.BaseClasses;
using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Ordering.CommandsHandlers
{
    public class CreateOrderHandler : OrderCommandHandler<CreateOrder>
    {
        public CreateOrderHandler(ILogger<CreateOrderHandler> logger, IOrderRepository orderRepository) : base(logger, orderRepository) { }

        public override async Task<CommandResult> Handle(CreateOrder command, CancellationToken cancellationToken)
        {
            if (command is null) return new CommandResult(false);

            logger.LogInformation("Processing command: {@Command}", command);

            Order order = new();
            AddDrinksToOrder(command.Drinks, order);

            logger.LogInformation("Creating order: {@Order}", order);

            orderRepository.Add(order);
            UnitOfWorkResult saveResult = await orderRepository
                                                    .UnitOfWork
                                                    .SaveEntitiesAsync(cancellationToken);

            return new CommandResult(saveResult.Errors, order);
        }

        private static void AddDrinksToOrder(IList<DrinkDTO> drinksDTO, Order order)
        {
            foreach (DrinkDTO drinkDTO in drinksDTO)
            {
                OrderedDrink drink = drinkDTO.ToDomain();
                order.AddDrink(drink);
            }
        }
    }
}
