using DrinkIT.Domain.BaseClasses;

namespace DrinkIT.Domain.Models.OrderAggregate
{
    internal class OrderableDrinkType : Enumeration
    {
        public OrderableDrinkType(int id, string name) : base(id, name) { }

        public static OrderableDrinkType ItalianCofee { get; } = new(1, "Italian Coffee");
        public static OrderableDrinkType AmericanCofee { get; } = new(2, "American Coffee");
        public static OrderableDrinkType Tea { get; } = new(3, "Tea");
        public static OrderableDrinkType Chocolate { get; } = new(4, "Chocolate");
    }
}
