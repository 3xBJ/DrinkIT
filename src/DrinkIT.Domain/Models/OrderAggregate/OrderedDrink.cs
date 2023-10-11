using DrinkIT.Domain.BaseClasses;

namespace DrinkIT.Domain.Models.OrderAggregate
{
    public class OrderedDrink : Entity
    {
        private int quantity;
        private readonly int drinkId;
        private readonly decimal unitPrice;
        private readonly string name;

        private static readonly List<OrderableDrinkType> DrinkTypes = OrderableDrinkType.GetAll<OrderableDrinkType>().ToList();

        public OrderedDrink(int drinkId, string name, decimal unitPrice)
        {
            if (DrinkTypes.Find(drinkType => drinkType.Id == drinkId && drinkType.Name.Equals(name)) is null)
            {
                AddError($"{drinkId} - {name} is not a valid drink type");
            }
           
            //TODO: better way?
            this.drinkId = drinkId;
            //unit price should be negative
            this.unitPrice = unitPrice;
            this.name = name;
        }

        public string Id { get; } = Guid.NewGuid().ToString();
        public int DrinkId => drinkId;
        public string Name => name;
        public decimal UnitPrice => unitPrice;
        public int Quantity => quantity;

        public void SetQuantity(int qty)
        {
            if (qty < 0)
            {
                AddError($"{Name} cannot have negative quantity");
            }

            this.quantity = qty;
        }

        public override bool Equals(object? obj)
        {
            if (obj is OrderedDrink otherDrink) return drinkId == otherDrink.DrinkId;
            return false;
        }

        public override int GetHashCode() => drinkId;
    }
}
