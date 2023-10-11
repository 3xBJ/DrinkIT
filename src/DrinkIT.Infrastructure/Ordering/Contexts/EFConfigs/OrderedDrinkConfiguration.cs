using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Infrastructure.Extensions;

namespace DrinkIT.Infrastructure.Ordering.Contexts.EFConfigs
{
    internal class OrderedDrinkConfiguration : IEntityTypeConfiguration<OrderedDrink>
    {
        public void Configure(EntityTypeBuilder<OrderedDrink> builder)
        {
            builder.ToTable("orderedDrinks", OrderingContext.DEFAULT_SCHEMA);

            builder.HasKey(drink => drink.Id);

            builder.IgnoreDomainEventsAndValidation();

            builder.UsePropertyAccessModeField<OrderedDrink, decimal>("unitPrice")
                   .HasColumnName("UnitPrice")
                   .IsRequired();

            builder.UsePropertyAccessModeField<OrderedDrink, string>("name")
                   .HasColumnName("Name")
                   .IsRequired();

            builder.UsePropertyAccessModeField<OrderedDrink, int>("quantity")
                   .HasColumnName("Quantity")
                   .IsRequired();

            builder.UsePropertyAccessModeField<OrderedDrink, int>("drinkId")
                   .HasColumnName("DrinkID")
                   .IsRequired();
        }
    }
}
