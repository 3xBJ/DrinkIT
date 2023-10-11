using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Ordering.DTO
{
    public class OrderDto
    {
        public required string Id { get; init; }
        public required List<DrinkDTO> Drinks { get; init; }
        public required OrderStatus Status { get; init; }
        public PaymentMethod? PaymentMethod { get; init; }


        public static OrderDto? FromDomain(Order? order)
        {
            if (order is null) return null;

            return new()
            {
                Id = order.Id,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                Drinks = order.Drinks.Select(drink => DrinkDTO.FromDomain(drink)).ToList()
            };
        }

    }
}
