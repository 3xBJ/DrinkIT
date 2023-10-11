using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Ordering.DTO
{
    public class DrinkDTO
    {
        public required int Id { get; init; }
        public required int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public required string Name { get; init; }

        public OrderedDrink ToDomain()
        {
            OrderedDrink drink = new(Id, Name, UnitPrice);
            drink.SetQuantity(Quantity);

            return drink;
        }

        public static DrinkDTO FromDomain(OrderedDrink drink) => new()
        {
            Id = drink.DrinkId,
            Quantity = drink.Quantity,
            UnitPrice = drink.UnitPrice,
            Name = drink.Name
        };
    }
}
