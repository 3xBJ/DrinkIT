using DrinkIT.BaseClasses;
using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Ordering.Commands;

namespace DrinkIT.Ordering.CommandsHandlers
{
    public abstract class OrderCommandHandler<CommandType> : ICommandHandler<CommandType> where CommandType : Command<CommandResult>
    {
        protected readonly ILogger logger;
        protected readonly IOrderRepository orderRepository;

        protected OrderCommandHandler(ILogger logger, IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public abstract Task<CommandResult> Handle(CommandType command, CancellationToken cancellationToken);
    }
}
