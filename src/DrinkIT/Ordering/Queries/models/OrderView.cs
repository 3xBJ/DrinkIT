using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Ordering.Queries.models
{
    public class OrderView
    {
        public required string Id { get; init; }
        public required decimal Price { get; init; }
        public string? PaymentId { get; init; }
        public required OrderStatus Status { get; init; }
        public required PaymentMethod PaymentMethod { get; init; }
        public required List<OrderedDrinkView> Drinks { get; init; }
    }
}
