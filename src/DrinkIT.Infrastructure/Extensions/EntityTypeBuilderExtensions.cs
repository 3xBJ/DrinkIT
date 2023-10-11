using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DrinkIT.Domain.BaseClasses;

namespace DrinkIT.Infrastructure.Extensions
{
    internal static class EntityTypeBuilderExtensions
    {
        public static void IgnoreDomainEventsAndValidation<EntityType>(this EntityTypeBuilder<EntityType> builder) where EntityType : Entity
        {
            builder.Ignore(entity => entity.DomainEvents);
            builder.Ignore(entity => entity.ValidationErrors);
            builder.Ignore(entity => entity.ErrorMessage);
            builder.Ignore(entity => entity.HasErrors);
        }

        public static PropertyBuilder<FieldType> UsePropertyAccessModeField<EntityType, FieldType>(this EntityTypeBuilder<EntityType> builder, string fieldName) where EntityType : Entity =>
            builder.Property<FieldType>(fieldName)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
