using DrinkIT.BaseClasses;
using DrinkIT.Ordering.DTO;

namespace DrinkIT.Ordering.Commands
{
    public class RemoveDrinksFromOrder : Command<CommandResult>
    {
        public RemoveDrinksFromOrder(string orderId) => OrderId = orderId;

        public string OrderId { get; }
        public List<DrinkDTO> Drinks { get; } = new();
    }
}
