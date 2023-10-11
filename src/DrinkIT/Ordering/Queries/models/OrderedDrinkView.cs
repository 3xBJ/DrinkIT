namespace DrinkIT.Ordering.Queries.models
{
    public class OrderedDrinkView
    {
        public required string Name { get; init; }
        public required int Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required decimal TotalPrice { get; init; }
    }
}
