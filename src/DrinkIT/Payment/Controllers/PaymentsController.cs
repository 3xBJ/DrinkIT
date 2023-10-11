using DrinkIT.Payment.CommandHandlers;
using DrinkIT.Payment.Commands;
using DrinkIT.Payment.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrinkIT.Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator mediator;

        public PaymentsController(IMediator mediator) => this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PayOrder([FromBody] PaymentDto paymentDto)
        {
            if (paymentDto is null) return BadRequest();

            PayOrder command = new()
            {
                OrderId = paymentDto.OrderId,
                CreditCard = paymentDto.CreditCard
            };

            PaymentResponse paymentResponse = await this.mediator.Send(command);

            return paymentResponse.Rejected ? BadRequest(paymentResponse) : Ok($"{paymentDto.OrderId} successful paid");
        }
    }
}
