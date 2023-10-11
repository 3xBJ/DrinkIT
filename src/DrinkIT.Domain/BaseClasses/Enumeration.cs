using System.Reflection;

namespace DrinkIT.Domain.BaseClasses
{
    public abstract class Enumeration
    {
        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public string Name { get; private set; }
        public int Id { get; private set; }

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetProperties(BindingFlags.Public |
                                    BindingFlags.Static |
                                    BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();

        public override bool Equals(object? obj)
        {
            if (obj is not Enumeration otherValue) return false;

            bool typeMatches = GetType().Equals(obj.GetType());
            bool valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Id;

        // Other utility methods ...
    }
}
