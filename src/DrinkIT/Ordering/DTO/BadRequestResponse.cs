using DrinkIT.Ordering.Commands;
using DrinkIT.Domain.BaseClasses;

namespace DrinkIT.Ordering.DTO
{
    public class BadRequestResponse
    {
        public required bool Rejected { get; init; }
        public OrderDto? Order { get; init; }
        public IReadOnlyList<Error>? ValidationErrors { get; init; }

        public static BadRequestResponse FromDomain(CommandResult result) => new()
        {
            Rejected = result.Rejected,
            Order = OrderDto.FromDomain(result.ResultingOrder),
            ValidationErrors = result.ValidationErrors
        };
    }
}
