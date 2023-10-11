using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Ordering.DTO;
using DrinkIT.Tests.Infrastructure;
using DrinkIT.Tests.Util;
using System.ComponentModel;
using System.Net.Http.Json;

namespace DrinkIT.Tests.Integration.WebAPI
{
    public class OrderingControllerTest : IntegrationTestBase
    {
        public OrderingControllerTest(WebAppWithDBFactory factory) : base(factory) { }

        [Fact]
        [Description("Create order request should add request to database")]
        public async Task PostNewOrderShouldReturn()
        {
            //Arrange
            OrderCreationDTO orderDTO = new()
            {
                Drinks = new(1)
                {
                    new DrinkDTO()
                    {
                        Id = 1,
                        Name = "Italian Coffee",
                        UnitPrice = 1.1M,
                        Quantity = 1
                    } 
                }
            };

            HttpContent content = HttpContentHelper.Create(orderDTO);

            using HttpClient client = this.factory.CreateClient();

            //Act
            using HttpResponseMessage response =  await client.PostAsync("/api/orders", content);

            //Assert
            response.EnsureSuccessStatusCode();
            OrderDto? requestResult = await response.Content.ReadFromJsonAsync<OrderDto>();

            Assert.NotNull(requestResult);
            Assert.NotNull(requestResult.Id);
            Assert.NotNull(requestResult.Drinks);
            Assert.NotEmpty(requestResult.Drinks);
            Assert.Equal(orderDTO.Drinks.Count, requestResult.Drinks.Count);
        }

        [Fact]
        [Description("Can modify drinks in a created order")]
        public async Task ModifyDrinksInACreatedOrder()
        {
            //Arrange

            var drink = new DrinkDTO()
            {
                Id = 1,
                Name = "Italian Coffee",
                UnitPrice = 1.1M,
                Quantity = 1
            };

            OrderCreationDTO orderDTO = new()
            {
                Drinks = new(1) { drink }
            };

            HttpContent content = HttpContentHelper.Create(orderDTO);

            using HttpClient client = this.factory.CreateClient();

            //Act
            using HttpResponseMessage response = await client.PostAsync("/api/orders", content);
            OrderDto? requestResult = await response.Content.ReadFromJsonAsync<OrderDto>();

            //Assert
            Assert.NotNull(requestResult);

            //Arrange 2
            string orderId = requestResult.Id;
            var drink2 = new DrinkDTO()
            {
                Id = 1,
                Name = "Italian Coffee",
                UnitPrice = 1.1M,
                Quantity = 20
            };

            HttpContent drinksContent = HttpContentHelper.Create(new List<DrinkDTO>(1) { drink2 });

            //Act 2
            using HttpResponseMessage drinkResponse = await client.PostAsync($"/api/orders/{orderId}/drinks", drinksContent);

            //Assert
            response.EnsureSuccessStatusCode();
            OrderDto? drinkResultDto = await drinkResponse.Content.ReadFromJsonAsync<OrderDto>();

            Assert.NotNull(drinkResultDto);
            Assert.NotNull(drinkResultDto.Id);
            Assert.NotNull(drinkResultDto.Drinks);
            Assert.NotEmpty(drinkResultDto.Drinks);
            Assert.Single(drinkResultDto.Drinks);
            Assert.Equal(21, drinkResultDto.Drinks[0].Quantity);
            Assert.Equal(PaymentMethod.CreditCard, drinkResultDto.PaymentMethod);
        }
    }
}
