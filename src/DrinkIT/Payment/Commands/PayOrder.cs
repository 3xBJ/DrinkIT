using DrinkIT.BaseClasses;
using DrinkIT.Payment.CommandHandlers;
using DrinkIT.Payment.DTO;

namespace DrinkIT.Payment.Commands
{
    public class PayOrder : Command<PaymentResponse>
    {
        public required string OrderId { get; init; }
        public CreditCardDataDto? CreditCard { get; set; }
    }
}
