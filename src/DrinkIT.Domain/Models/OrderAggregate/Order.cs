using DrinkIT.Domain.BaseClasses;
using DrinkIT.Domain.BaseInterfaces;

namespace DrinkIT.Domain.Models.OrderAggregate
{
    public class Order : Entity, IAggregateRoot
    {
        public const int MAX_PRICE_CASH = 10;

        private readonly HashSet<OrderedDrink> drinks = new();

        //Used by EFCore access
        private OrderStatus status;
        private PaymentMethod paymentMethod;
        private string? paymentId;

        public Order()
        {
            this.status = OrderStatus.Started;
            this.paymentMethod = PaymentMethod.CreditCard;
        }

        public string Id { get; } = Guid.NewGuid().ToString();
        public IReadOnlySet<OrderedDrink> Drinks => this.drinks;
        public OrderStatus Status => this.status;
        public PaymentMethod PaymentMethod => this.paymentMethod;
        public string? PaymentId => this.paymentId;

        public decimal Price => this.drinks.Sum(drink => drink.UnitPrice * drink.Quantity);

        public void AddDrink(OrderedDrink toAddDrink)
        {
            if (toAddDrink is null || toAddDrink.HasErrors) return;

            if (!Status.Equals(OrderStatus.Started))
            {
                AddError($"It's not possible to add drinks to orders with status {Status}");
                return;
            }

            if (this.drinks.TryGetValue(toAddDrink, out OrderedDrink? drink))
            {
                drink.SetQuantity(drink.Quantity + toAddDrink.Quantity);
            }
            else
            {
                this.drinks.Add(toAddDrink);
            }

            if (!VerifyPaymentMethodIsValid(PaymentMethod))
            {
                SetPaymentMethod(PaymentMethod.CreditCard);
            }
        }

        public void RemoveDrink(OrderedDrink toRemoveDrink)
        {
            if (toRemoveDrink is null || toRemoveDrink.HasErrors) return;

            if (Status != OrderStatus.Started)
            {
                AddError($"It's not possible to add drinks to orders with status {Status}");
                return;
            }

            if (this.drinks.TryGetValue(toRemoveDrink, out OrderedDrink? drink))
            {
                drink.SetQuantity(drink.Quantity - toRemoveDrink.Quantity);
                if (drink.Quantity > 0 || drink.HasErrors) return;
            }

            this.drinks.Remove(toRemoveDrink);
        }

        public void SetPaymentMethod(PaymentMethod paymentMethod)
        {
            if (!VerifyPaymentMethodIsValid(paymentMethod))
            {
                AddError($"An order with price higher than {MAX_PRICE_CASH} must be paid with {PaymentMethod.CreditCard.Name}");

                return;
            }

            bool isAcceptedMethod = !paymentMethod.Equals(PaymentMethod.Cash) ||
                                    !paymentMethod.Equals(PaymentMethod.CreditCard);
            if (!isAcceptedMethod)
            {
                AddError($"{paymentMethod.Name} is not accepted as a payment method");

                return;
            }

            this.paymentMethod = paymentMethod;
        }

        public void SetCancelledStatus()
        {
            if (!Status.Equals(OrderStatus.Started))
            {
                AddError($"An order with status {Status.Name} cannot be canceled");
                return;
            }

            this.status = OrderStatus.Cancelled;
        }

        public bool CanBePaid() => Status.Equals(OrderStatus.Started);
        public void SetPaidStatusAndId(string paymentId)
        {
            if (!CanBePaid())
            {
                AddError($"An order with status {Status.Name} cannot be paid");
                return;
            }

            this.paymentId = paymentId;
            this.status = OrderStatus.Paid;
        }

        private bool VerifyPaymentMethodIsValid(PaymentMethod? paymentMethod)
        {
            if (paymentMethod is null || paymentMethod.Equals(PaymentMethod.CreditCard)) return true;
            return Price < MAX_PRICE_CASH;
        }
    }
}
