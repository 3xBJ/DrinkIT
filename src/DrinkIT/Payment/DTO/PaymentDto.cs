using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Payment.DTO
{
    public class PaymentDto
    {
        public string OrderId { get; set; }
        public CreditCardDataDto? CreditCard { get; set; }
    }
}
