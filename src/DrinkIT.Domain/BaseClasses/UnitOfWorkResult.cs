namespace DrinkIT.Domain.BaseClasses
{
    public class UnitOfWorkResult
    {
        public required int EntitiesModified { get; init; }
        public required bool EntitiesHaveErrors { get; init; }
        public IReadOnlyList<Error>? Errors { get; init; }
    }
}
