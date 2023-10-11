using DrinkIT.Domain.BaseClasses;
using DrinkIT.Domain.BaseInterfaces;
using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Infrastructure.Ordering.Contexts;
using DrinkIT.Ordering.Queries;
using DrinkIT.Ordering.Queries.models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel;
using Testcontainers.MsSql;

namespace DrinkIT.Tests.Integration.Context
{
    public class OrderingContextTest : IAsyncLifetime
    {
        protected MsSqlContainer databaseContainer = new MsSqlBuilder()
            .WithCleanUp(true)
            .WithName($"OrderingContextTest-{Guid.NewGuid()}")
            .Build();

        public async Task InitializeAsync()
        {
            await databaseContainer.StartAsync();
            await ApplyMigrations();
        }

        public async Task DisposeAsync() => await databaseContainer.DisposeAsync();

        [Fact]
        [Description("Entities should be saved if they have no error")]
        public async Task ShouldSaveIfNoErrors()
        {
            //Arrange
            Mock<IMediator> mediatorMock = CreateMediatorStrict();
            using OrderingContext dbContext = CreateDbContext(mediatorMock);
            
            Order order = CreateOrderWithDomainEvents();
            Mock<ILogger<OrdersQueries>> loggerMock = new(MockBehavior.Loose);

            IOrdersQueries orderQueries = new OrdersQueries(loggerMock.Object, databaseContainer.GetConnectionString());

            //Act
            dbContext.Add(order);
            UnitOfWorkResult result = await dbContext.SaveEntitiesAsync(CancellationToken.None);
            OrderView? orderView = await orderQueries.GetAsync(order.Id);

            //Assert
            mediatorMock.Verify(mediator => mediator.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);

            Assert.NotNull(result);
            Assert.False(result.EntitiesHaveErrors);
            Assert.Null(result.Errors);
            Assert.Equal(2, result.EntitiesModified);

            Assert.NotNull(orderView);
            Assert.Equal(order.Id, orderView.Id);
        }

        [Fact]
        [Description("Should not save when entities have errors")]
        public async Task ShouldNotSaveIfErrors()
        {
            //Arrange
            Mock<IMediator> mediatorMock = CreateMediatorStrict();
            using OrderingContext dbContext = CreateDbContext(mediatorMock);

            Order order = CreateOrderWithDomainEvents();
            order.SetCancelledStatus();
            order.SetPaidStatusAndId("id22");

            //Act
            dbContext.Add(order);
            UnitOfWorkResult result = await dbContext.SaveEntitiesAsync(CancellationToken.None);

            //Assert
            mediatorMock.Verify(mediator => mediator.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);

            Assert.NotNull(result);
            Assert.True(result.EntitiesHaveErrors);
            Assert.NotNull(result.Errors);
            Assert.NotEmpty(result.Errors);
            Assert.Equal(0, result.EntitiesModified);
        }

        private static Order CreateOrderWithDomainEvents()
        {
            Order order = new();
            order.AddDrink(new OrderedDrink(1, "Italian Coffee", 1.1M));
            order.SetPaidStatusAndId("paymentId");
            return order;
        }

        private static Mock<IMediator> CreateMediatorStrict()
        {
            Mock<IMediator> mediatorMock = new(MockBehavior.Strict);
            return mediatorMock;
        }

        private OrderingContext CreateDbContext(Mock<IMediator> mediatorMock)
        {
            DbContextOptions<OrderingContext> options = new DbContextOptionsBuilder<OrderingContext>()
                .UseSqlServer(databaseContainer.GetConnectionString())
                .EnableSensitiveDataLogging()
                .Options;

            OrderingContext dbContext = new(mediatorMock.Object, options);
            return dbContext;
        }
        private async Task ApplyMigrations()
        {
            Mock<IMediator> mediatorMock = CreateMediatorStrict();
            using OrderingContext dbContext = CreateDbContext(mediatorMock);
            await dbContext.Database.MigrateAsync();
        }

        //should delete notifications after publish
    }
}
