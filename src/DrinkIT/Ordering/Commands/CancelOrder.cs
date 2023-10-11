using DrinkIT.BaseClasses;

namespace DrinkIT.Ordering.Commands
{
    public class CancelOrder : Command<CommandResult>
    {
        public CancelOrder(string orderId) => OrderId = orderId;
        public string OrderId { get; }
    }
}
