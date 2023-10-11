using DrinkIT.Domain.BaseInterfaces;

namespace DrinkIT.Domain.Models.OrderAggregate
{
    public interface IOrderRepository : IRepository<Order>
    {
        Order Add(Order order);
        void Update(Order order);
        Task<Order?> GetAsync(string id);
    }
}
