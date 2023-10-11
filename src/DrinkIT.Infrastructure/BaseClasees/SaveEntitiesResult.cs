using DrinkIT.Domain.BaseClasses;

namespace DrinkIT.Infrastructure.BaseClasses
{
    public class SaveEntitiesResult
    {
        public required int EntitiesModified { get; init; }
        public required bool EntitiesHaveErrors { get; init; }
        public IReadOnlyList<Error>? Errors { get; init; }
    }
}
