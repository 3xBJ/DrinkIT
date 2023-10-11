using DrinkIT.BaseClasses;
using DrinkIT.Ordering.DTO;

namespace DrinkIT.Ordering.Commands
{
    public class AddDrinksToOrder : Command<CommandResult>
    {
        public AddDrinksToOrder(string orderId) => OrderId = orderId;

        public string OrderId { get; }
        public List<DrinkDTO> Drinks { get; } = new();
    }
}
