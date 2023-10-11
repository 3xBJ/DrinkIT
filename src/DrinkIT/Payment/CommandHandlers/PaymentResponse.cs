using DrinkIT.Domain.BaseClasses;

namespace DrinkIT.Payment.CommandHandlers
{
    public class PaymentResponse
    {
        public required bool Rejected { get; init; }
        public IReadOnlyList<Error>? Errors { get; init; }
    }
}
