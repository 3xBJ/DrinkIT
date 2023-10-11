namespace DrinkIT.Domain.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmptyOrWhiteSpace(this string s) => string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
    }
}
