using DrinkIT.Ordering.Commands;
using DrinkIT.Ordering.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DrinkIT.Domain.Extensions;
using DrinkIT.Ordering.Queries;
using DrinkIT.Ordering.Queries.models;
using DrinkIT.Payment.Commands;
using DrinkIT.Domain.Models.OrderAggregate;

namespace DrinkIT.Ordering.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IOrdersQueries orderQueries;

        public OrdersController(IMediator mediator, IOrdersQueries orderQueries)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
        }

        [Route("{orderId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string orderId)
        {
            if (orderId.IsNullOrEmptyOrWhiteSpace()) return BadRequest("orderID must be a non null, empty or white space value");

            OrderView? order = await this.orderQueries.GetAsync(orderId);

            return order is null ? BadRequest("Order doesn't exist or something bad happened :(") : Ok(order);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreationDTO? orderDTO)
        {
            if (orderDTO is null) return BadRequest();

            CreateOrder command = new();
            command.Drinks.AddRange(orderDTO.Drinks);
            CommandResult result = await mediator.Send(command);

            return result.Rejected ? BadRequest(BadRequestResponse.FromDomain(result)) : Ok(OrderDto.FromDomain(result.ResultingOrder!));
        }

        [Route("{orderId}/cancel")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            if (orderId.IsNullOrEmptyOrWhiteSpace()) return BadRequest();

            CancelOrder command = new(orderId);
            CommandResult result = await mediator.Send(command);

            return result.Rejected ? BadRequest(BadRequestResponse.FromDomain(result)) : Ok($"Order {orderId} cancelled");
        }

        [Route("{orderId}/drinks")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddDrink(string orderId, [FromBody] List<DrinkDTO> drinksDTO)
        {
            if (drinksDTO is null || drinksDTO.Count == 0 || orderId.IsNullOrEmptyOrWhiteSpace()) return BadRequest();

            AddDrinksToOrder command = new(orderId);
            command.Drinks.AddRange(drinksDTO);

            CommandResult result = await mediator.Send(command);

            return result.Rejected ? BadRequest(BadRequestResponse.FromDomain(result)) : Ok(OrderDto.FromDomain(result.ResultingOrder!));
        }

        [Route("{orderId}/drinks")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveDrinks(string orderId, [FromBody] List<DrinkDTO> drinksDTO)
        {
            if (drinksDTO is null || drinksDTO.Count == 0 || orderId.IsNullOrEmptyOrWhiteSpace()) return BadRequest();

            RemoveDrinksFromOrder command = new(orderId);
            command.Drinks.AddRange(drinksDTO);

            CommandResult result = await mediator.Send(command);

            return result.Rejected ? BadRequest(BadRequestResponse.FromDomain(result)) : Ok(OrderDto.FromDomain(result.ResultingOrder!));
        }

        [Route("{orderId}/PaymentMethod")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePaymentMethod(string orderId, [FromBody] PaymentMethod paymentMethod)
        {
            if (paymentMethod is null || orderId.IsNullOrEmptyOrWhiteSpace()) return BadRequest();

            ChangePaymentMethod command = new(orderId, paymentMethod);

            CommandResult result = await mediator.Send(command);

            return result.Rejected ? BadRequest(BadRequestResponse.FromDomain(result)) : Ok(OrderDto.FromDomain(result.ResultingOrder!));
        }
    }
}
