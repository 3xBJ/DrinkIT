using DrinkIT.Domain.BaseClasses;

namespace DrinkIT.Domain.Models.OrderAggregate
{
    public class OrderStatus : Enumeration
    {
        public OrderStatus(int id, string name) : base(id, name) { }

        public static OrderStatus Started { get; } = new(1, "Started");
        public static OrderStatus Cancelled { get; } = new(2, "Cancelled");
        public static OrderStatus Paid { get; } = new(3, "Paid");
        public static OrderStatus ReadyToBePikedUp { get; } = new(4, "Ready to be picked up");
        public static OrderStatus Completed { get; } = new(5, "Completed");
    }
}