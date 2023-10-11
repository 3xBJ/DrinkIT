namespace DrinkIT.Products.Models
{
    public class Drink
    {
        public Drink() { }

        public Drink(int id, string name, decimal unitPrice)
        {
            Id = id;
            Name = name;
            UnitPrice = unitPrice;
        }

        public int Id { get; init; }
        public string Name { get; init; }
        public decimal UnitPrice { get; init; }
    }
}
