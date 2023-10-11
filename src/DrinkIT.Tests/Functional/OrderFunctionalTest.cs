using DrinkIT.Domain.BaseClasses;
using DrinkIT.Domain.BaseInterfaces;
using DrinkIT.Domain.Events.Payments;
using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Ordering.Commands;
using DrinkIT.Ordering.CommandsHandlers;
using DrinkIT.Ordering.DomainEventHandlers;
using DrinkIT.Ordering.DTO;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel;
using System.Reflection;

namespace DrinkIT.Tests.Functional
{
    [Collection("Order aggregation functional tests")]
    public class OrderFunctionalTest
    {
        private const string ITALIAN_COFFEE = "Italian Coffee";

        public static IEnumerable<object[]> AllStatusMinusStarted => OrderStatus.GetAll<OrderStatus>()
                                                                                .Where(orderStatus => !orderStatus.Equals(OrderStatus.Started))
                                                                                .Select(orderStatus => new object[] { orderStatus });

        [Fact]
        [Description("Invalid drinks shouldn't be added to the order")]
        public async Task AddInvalidDrinkToOrder()
        {
            //Arrange
            Order? orderAdded = null;
            Mock<ILogger<CreateOrderHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = new(MockBehavior.Strict);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            repositoryMock.Setup(repo => repo.Add(It.IsAny<Order>()))
                          .Callback<Order>(order => orderAdded = order)
                          .Returns(orderAdded!)
                          .Verifiable();

            ICommandHandler<CreateOrder> handler = new CreateOrderHandler(loggerMock.Object, repositoryMock.Object);

            CreateOrder command = new();
            DrinkDTO drinkDTO = new()
            {
                Id = 2,
                Name = "",
                Quantity = -2,
                UnitPrice = 15
            };

            command.Drinks.Add(drinkDTO);

            //Act
            _ = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();

            Assert.NotNull(orderAdded);
            Assert.NotNull(orderAdded.Drinks);
            Assert.Empty(orderAdded.Drinks);
        }

        [Fact]
        [Description("Order price must be the sum of the drinks prices")]
        public async Task AddDrinksAndValidatePrice()
        {
            //Arrange
            Order? orderAdded = null;
            Mock<ILogger<CreateOrderHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = new(MockBehavior.Strict);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            repositoryMock.Setup(repo => repo.Add(It.IsAny<Order>()))
                          .Callback<Order>(order => orderAdded = order)
                          .Returns(orderAdded!)
                          .Verifiable();

            ICommandHandler<CreateOrder> handler = new CreateOrderHandler(loggerMock.Object, repositoryMock.Object);

            DrinkDTO drink1 = new()
            {
                Id = 1,
                Name = ITALIAN_COFFEE,
                Quantity = 2,
                UnitPrice = 1.10M
            };

            DrinkDTO drink2 = new()
            {
                Id = 2,
                Name = "American Coffee",
                Quantity = 1,
                UnitPrice = 2M
            };

            CreateOrder command = new();
            command.Drinks.Add(drink1);
            command.Drinks.Add(drink2);

            decimal price = command.Drinks.Sum(drink => drink.UnitPrice * drink.Quantity);
            //Act
            var response = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();

            Assert.NotNull(response);
            Assert.False(response.Rejected);
            Assert.NotNull(response.ResultingOrder);
            Assert.NotEmpty(response.ResultingOrder.Drinks);
            Assert.Equal(command.Drinks.Count, response.ResultingOrder.Drinks.Count);
            Assert.Equal(price, response.ResultingOrder.Price);
        }

        [Fact]
        [Description("If price goes over MAX_PRICE_CASH payment method should change to Credit Card")]
        public async Task AddMoreThanMaxPriceMustConvertToCreditCard()
        {
            //Arrange
            Order order = new();
            order.GetType()
                 .GetProperty(nameof(order.Id), BindingFlags.Instance | BindingFlags.NonPublic)
                 ?.SetValue(order, order.Id);

            order.SetPaymentMethod(PaymentMethod.Cash);

            Mock<ILogger<AddDrinksHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = CreateRepositoryMockGetAndUpdate(order);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            ICommandHandler<AddDrinksToOrder> handler = new AddDrinksHandler(loggerMock.Object, repositoryMock.Object);

            AddDrinksToOrder command = new(order.Id);
            command.Drinks.Add(new()
            {
                Id = 1,
                Name = "Italian Coffee",
                Quantity = Order.MAX_PRICE_CASH,
                UnitPrice = 1.10M
            });

            //Act
            _ = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();

            Assert.NotNull(order);
            Assert.NotEmpty(order.Drinks);
            Assert.Equal(PaymentMethod.CreditCard, order.PaymentMethod);
        }

        [Theory]
        [MemberData(nameof(AllStatusMinusStarted))]
        [Description("Cannot add drinks to orders with status != Started")]
        public async Task TryAddDrinkToOtherOrderStatus(OrderStatus orderStatus)
        {
            //Arrange
            Order order = new();

            order.GetType()
                 .GetField("status", BindingFlags.NonPublic | BindingFlags.Instance)
                 ?.SetValue(order, orderStatus);

            Mock<ILogger<AddDrinksHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = CreateRepositoryMockGetAndUpdate(order);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            ICommandHandler<AddDrinksToOrder> handler = new AddDrinksHandler(loggerMock.Object, repositoryMock.Object);

            AddDrinksToOrder command = new(order.Id);
            command.Drinks.Add(new()
            {
                Id = 1,
                Name = "Italian Coffee",
                Quantity = 1,
                UnitPrice = 1.10M
            });

            //Act
            _ = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();

            Assert.NotNull(order);
            Assert.Empty(order.Drinks);
            Assert.True(order.HasErrors);
        }

        [Fact]
        [Description("If removed qty is smaller than actual qty, drink must remain")]
        public async Task PartiallyRemoveDrink()
        {
            //Arrange
            Order order = new();

            Mock<ILogger<RemoveDrinksHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = CreateRepositoryMockGetAndUpdate(order);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            OrderedDrink drink = new(1, ITALIAN_COFFEE, 1.1M);
            drink.SetQuantity(10);

            DrinkDTO drinkToBeRemoved = new()
            {
                Id = 1,
                Name = ITALIAN_COFFEE,
                Quantity = 5,
                UnitPrice = 1.10M
            };

            RemoveDrinksFromOrder command = new(order.Id);
            command.Drinks.Add(drinkToBeRemoved);

            order.AddDrink(drink);

            decimal expectedPrice = (drink.Quantity - drinkToBeRemoved.Quantity) * drink.UnitPrice;
            ICommandHandler<RemoveDrinksFromOrder> handler = new RemoveDrinksHandler(loggerMock.Object, repositoryMock.Object);

            //Act
            _ = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();
            Assert.NotEmpty(order.Drinks);
            Assert.Equal(expectedPrice, order.Price);
        }

        [Fact]
        [Description("If removed qty is equals to the actual qty, drink must be removed")]
        public async Task RemoveDrink()
        {
            //Arrange
            Order order = new();

            Mock<ILogger<RemoveDrinksHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = CreateRepositoryMockGetAndUpdate(order);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            OrderedDrink drink = new(1, ITALIAN_COFFEE, 1.1M);
            drink.SetQuantity(10);

            DrinkDTO drinkToBeRemoved = new()
            {
                Id = 1,
                Name = ITALIAN_COFFEE,
                Quantity = 10,
                UnitPrice = 1.10M
            };

            RemoveDrinksFromOrder command = new(order.Id);
            command.Drinks.Add(drinkToBeRemoved);

            order.AddDrink(drink);

            ICommandHandler<RemoveDrinksFromOrder> handler = new RemoveDrinksHandler(loggerMock.Object, repositoryMock.Object);

            //Act
            _ = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();
            Assert.Empty(order.Drinks);
            Assert.Equal(0, order.Price);
        }

        [Fact]
        [Description("Cannot set drink qty to negative")]
        public async Task TryRemoveTooMuchDrinkQty()
        {
            //Arrange
            Order order = new();

            Mock<ILogger<RemoveDrinksHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = CreateRepositoryMockGetAndUpdate(order);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            OrderedDrink drink = new(1, ITALIAN_COFFEE, 1.1M);
            drink.SetQuantity(10);

            DrinkDTO drinkToBeRemoved = new()
            {
                Id = 1,
                Name = ITALIAN_COFFEE,
                Quantity = 11,
                UnitPrice = 1.10M
            };

            RemoveDrinksFromOrder command = new(order.Id);
            command.Drinks.Add(drinkToBeRemoved);

            order.AddDrink(drink);

            ICommandHandler<RemoveDrinksFromOrder> handler = new RemoveDrinksHandler(loggerMock.Object, repositoryMock.Object);

            //Act
            _ = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();
            Assert.NotEmpty(order.Drinks);
            Assert.True(order.Drinks.First().HasErrors);
        }

        [Fact]
        [Description("Can cancel a started order")]
        public async Task CancelStartedOrder()
        {
            //Arrange
            Order order = new();

            Mock<ILogger<CancelOrderHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = CreateRepositoryMockGetAndUpdate(order);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            CancelOrder command = new(order.Id);
            ICommandHandler<CancelOrder> handler = new CancelOrderHandler(loggerMock.Object, repositoryMock.Object);

            //Act
            _ = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();
            Assert.NotNull(order);
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }

        [Theory]
        [MemberData(nameof(AllStatusMinusStarted))]
        [Description("Cannot cancel an order with status different than started")]
        public async Task TryCancelOrderShouldFail(OrderStatus orderStatus)
        {
            //Arrange
            Order order = new();
            order.GetType()
                 .GetField("status", BindingFlags.NonPublic | BindingFlags.Instance)
                 ?.SetValue(order, orderStatus);

            Mock<ILogger<CancelOrderHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = CreateRepositoryMockGetAndUpdate(order);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            CancelOrder command = new(order.Id);
            ICommandHandler<CancelOrder> handler = new CancelOrderHandler(loggerMock.Object, repositoryMock.Object);

            //Act
            _ = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();
            Assert.NotNull(order);
            Assert.Equal(orderStatus, order.Status);
            Assert.True(order.HasErrors);
        }

        [Fact]
        [Description("When a payment is approved, the corresponding order should be marked as paid")]
        public async Task SendPaymentApprovedEventAndVerifyOrderStatus()
        {
            //Arrange
            Order order = new();
            OrderedDrink drink = new(1, "Italian Coffee", 2);
            drink.SetQuantity(1);

            order.AddDrink(drink);

            Mock<ILogger<PaidOrderHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = CreateRepositoryMockGetAndUpdate(order);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            string paymentId = Guid.NewGuid().ToString();
            PaymentApproved command = new(order.Id, paymentId);
            INotificationHandler<PaymentApproved> handler = new PaidOrderHandler(loggerMock.Object, repositoryMock.Object);

            //Act
            await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();
            Assert.Equal(OrderStatus.Paid, order.Status);
            Assert.Equal(paymentId, order.PaymentId);
        }

        [Fact]
        [Description("Cannot set payment method to cash when Price is over MAX_PRICE_CASH should fail")]
        public async Task TryChangePaymentToCashWithPriceTooHigh()
        {
            //Arrange
            Order order = new();
            OrderedDrink drink = new(1, "Italian Coffee", 200);
            drink.SetQuantity(10);

            order.AddDrink(drink);

            Mock<ILogger<ChangePaymentMethodHandler>> loggerMock = new(MockBehavior.Loose);
            Mock<IOrderRepository> repositoryMock = CreateRepositoryMockGetAndUpdate(order);
            Mock<IUnitOfWork> unitOfWorkMock = CreateAndAssociateUnitOfWorkMock(repositoryMock);

            ChangePaymentMethod command = new(order.Id, PaymentMethod.Cash);
            ICommandHandler<ChangePaymentMethod> handler = new ChangePaymentMethodHandler(loggerMock.Object, repositoryMock.Object);

            //Act
            _ = await handler.Handle(command, CancellationToken.None);

            //Assert
            repositoryMock.Verify();
            unitOfWorkMock.Verify();

            Assert.True(order.HasErrors);
            Assert.NotNull(order.ValidationErrors);
            Assert.NotEmpty(order.ValidationErrors);
            Assert.NotEqual(PaymentMethod.Cash, order.PaymentMethod);
        }

        //TODO no existing drink

        private static Mock<IUnitOfWork> CreateAndAssociateUnitOfWorkMock(Mock<IOrderRepository> repositoryMock)
        {
            Mock<IUnitOfWork> unitOfWorkMock = new(MockBehavior.Strict);
            repositoryMock.SetupGet(repo => repo.UnitOfWork)
                  .Returns(unitOfWorkMock.Object)
                  .Verifiable();

            unitOfWorkMock.Setup(unitOfWork => unitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                          .Returns(Task.FromResult(new UnitOfWorkResult() { EntitiesHaveErrors  = false, EntitiesModified = 0}));

            return unitOfWorkMock;
        }

        private static Mock<IOrderRepository> CreateRepositoryMockGetAndUpdate(Order order)
        {
            Mock<IOrderRepository> repositoryMock = new(MockBehavior.Strict);
            repositoryMock.Setup(repo => repo.GetAsync(It.Is<string>(s => s.Equals(order.Id))))
                          .Returns(Task.FromResult<Order?>(order))
                          .Verifiable();

            repositoryMock.Setup(repo => repo.Update(It.Is<Order>(orderUpdated => orderUpdated.Id.Equals(order.Id))))
                          .Verifiable();

            return repositoryMock;
        }
    }
}
