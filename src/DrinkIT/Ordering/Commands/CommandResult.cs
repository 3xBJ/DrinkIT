using DrinkIT.Domain.BaseClasses;
using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Ordering.Commands
{
    public class CommandResult
    {
        public CommandResult(bool rejected, Order? order = null)
        {
            if (!rejected && order is null) throw new ArgumentNullException(nameof(order));

            Rejected = rejected;
            ResultingOrder = order;
        }

        public CommandResult(IReadOnlyList<Error>? validationErrors, Order order)
        {
            ValidationErrors = validationErrors;
            ResultingOrder = order ?? throw new ArgumentNullException(nameof(order));
            Rejected = ValidationErrors?.Count > 0;
        }

        public CommandResult(string errorMessage)
        {
            Error error = new(errorMessage);
            ValidationErrors = new List<Error>(1) { error };
            Rejected = true;
        }


        public bool Rejected { get; }
        public Order? ResultingOrder { get; }
        public IReadOnlyList<Error>? ValidationErrors { get; }
    }
}
