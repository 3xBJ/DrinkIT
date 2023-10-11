using DrinkIT.BaseClasses;
using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Ordering.Commands
{
    public class ChangePaymentMethod : Command<CommandResult>
    {
        public ChangePaymentMethod(string orderId, PaymentMethod method)
        {
            OrderId = orderId;
            Method = method;
        }

        public string OrderId { get; }
        public PaymentMethod Method { get; }
    }
}
