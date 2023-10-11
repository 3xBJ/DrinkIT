using DrinkIT.Domain.BaseInterfaces;

namespace DrinkIT.Domain.Events.Payments
{
    public class PaymentApproved : IDomainEvent
    {
        public PaymentApproved(string orderID, string paymentId) => (OrderID, PaymentId) = (orderID, paymentId);

        public string OrderID { get; }
        public string PaymentId { get; }
    }
}
