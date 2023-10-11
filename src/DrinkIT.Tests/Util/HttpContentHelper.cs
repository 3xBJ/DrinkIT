using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace DrinkIT.Tests.Util
{
    internal static class HttpContentHelper
    {
        internal static HttpContent Create<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj);
            return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        }
    }
}
