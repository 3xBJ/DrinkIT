using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Ordering.Queries.models;

namespace DrinkIT.Ordering.Queries
{
    public interface IOrdersQueries
    {
        Task<OrderView?> GetAsync(string id);
    }
}