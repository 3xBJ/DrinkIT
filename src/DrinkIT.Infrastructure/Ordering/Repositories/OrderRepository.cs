using Microsoft.EntityFrameworkCore;
using DrinkIT.Domain.BaseInterfaces;
using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Infrastructure.Ordering.Contexts;

namespace DrinkIT.Infrastructure.Ordering.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderingContext orderingContext;

        public OrderRepository(OrderingContext orderingContext) =>
            this.orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));

        public IUnitOfWork UnitOfWork => orderingContext;

        public Order Add(Order order) => orderingContext.Orders.Add(order).Entity;
        public void Update(Order order) => orderingContext.Orders.Update(order);

        public async Task<Order?> GetAsync(string id)
        {
            Order? order = await GetFromDbAsync(id) ??
                           GetFromLocal(id);

            if (order is null) return order;

            await orderingContext
                      .Entry(order)
                      .Collection(entity => entity.Drinks)
                      .LoadAsync();

            return order;
        }

        private async Task<Order?> GetFromDbAsync(string id) => await orderingContext
            .Orders
            .FirstOrDefaultAsync(entity => entity.Id == id);

        private Order? GetFromLocal(string id) => orderingContext
            .Orders
            .Local
            .FirstOrDefault(entity => entity.Id == id);
    }
}
