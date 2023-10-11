using Dapper;
using DrinkIT.Domain.Extensions;
using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Ordering.Queries.models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DrinkIT.Ordering.Queries
{
    public class OrdersQueries : IOrdersQueries
    {
        private readonly string connectionString;
        private readonly ILogger<OrdersQueries> logger;

        public OrdersQueries(ILogger<OrdersQueries> logger, string connectionString)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.connectionString = !connectionString.IsNullOrEmptyOrWhiteSpace() ?
                connectionString :
                throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<OrderView?> GetAsync(string id)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(connectionString);
                dynamic order = await connection.QueryAsync<dynamic>(
                    @"select 
                        o.[Id] as orderId, 
                        o.PaymentMethod as paymentMethodId, 
                        o.Status as orderStatusID,
                        o.PaymentId as paymentId,

                        od.Name as drinkName,
                        od.UnitPrice as drinkUnitPrice,
                        od.Quantity as drinkQuantity

                        FROM 
                            ordering.orders o
                        LEFT JOIN 
                            ordering.orderedDrinks od  
                        ON 
                            o.Id = od.orderId
                        WHERE 
                           o.Id =@id",
                    new { id});

                return MapToOrderView(order);
            }
            catch (Exception ex)
            {
                this.logger.LogError(new EventId(), ex, "Order query resulted in an exception.");
                return null;
            }
        }

        private OrderView MapToOrderView(dynamic queryResult)
        {
            List<OrderedDrinkView> drinks = new();

            decimal price = 0;
            foreach (dynamic item in queryResult)
            {
                int drinkQty = item.drinkQuantity;
                decimal unitPrice = item.drinkUnitPrice;
                if (drinkQty == 0 || unitPrice == 0) continue;

                decimal totalPrice = drinkQty * unitPrice;

                OrderedDrinkView drink = new()
                {
                    Name = item.drinkName,
                    Quantity = drinkQty,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                };

                price += totalPrice;
                drinks.Add(drink);
            }

            var firstLine = queryResult[0];

            int paymentMethodId = firstLine.paymentMethodId;
            PaymentMethod paymentMethod = PaymentMethod.GetAll<PaymentMethod>()
                                                       .First(pm => pm.Id == paymentMethodId);

            int orderStatusId = firstLine.orderStatusID;
            OrderStatus orderStatus = OrderStatus.GetAll<OrderStatus>()
                                                 .First(os => os.Id == orderStatusId);

            return new OrderView
            {
                Id = firstLine.orderId,
                PaymentMethod = paymentMethod,
                Price = price,
                Status = orderStatus,
                Drinks = drinks,
                PaymentId = firstLine.paymentId ?? string.Empty
            };
        }
    }
}
