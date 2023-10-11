using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Infrastructure.Extensions;
using DrinkIT.Domain.BaseClasses;
using System;

namespace DrinkIT.Infrastructure.Ordering.Contexts.EFConfigs
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders", OrderingContext.DEFAULT_SCHEMA);

            builder.HasKey(order => order.Id);

            builder.IgnoreDomainEventsAndValidation();
            builder.Ignore(order => order.Price);

            builder.UsePropertyAccessModeField<Order, OrderStatus>("status")
                    .HasConversion(status => status.Id, id => Enumeration.GetAll<OrderStatus>().First(os => os.Id == id))
                   .IsRequired();

            builder.UsePropertyAccessModeField<Order, PaymentMethod>("paymentMethod")
                   .HasConversion(pm => pm.Id, id => Enumeration.GetAll<PaymentMethod>().First(os => os.Id == id))
                   .IsRequired();

            builder.UsePropertyAccessModeField<Order, string?>("paymentId")
                   .IsRequired(false)
                   .HasMaxLength(36);

            builder.Property(order => order.Id)
                   .HasMaxLength(36);

            builder.Metadata
                   .FindNavigation(nameof(Order.Drinks))
                   ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
