using DrinkIT.Domain.BaseClasses;

namespace DrinkIT.Domain.Models.OrderAggregate
{
    public class PaymentMethod : Enumeration
    {
        public PaymentMethod(int id, string name) : base(id, name) { }

        public static PaymentMethod Cash { get; } = new(1, "Cash");
        public static PaymentMethod CreditCard { get; } = new(2, "Credit Card");
    }
}