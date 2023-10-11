using DrinkIT.Domain.BaseClasses;
using DrinkIT.Domain.Events.Payments;
using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Ordering.Queries;
using DrinkIT.Ordering.Queries.models;
using DrinkIT.Payment.Commands;
using DrinkIT.Payment.DTO;
using DrinkIT.Util;
using MediatR;

namespace DrinkIT.Payment.CommandHandlers
{
    public class PayOrderHandler : IPaymentCommandHandler<PayOrder>
    {
        private readonly IExternalPaymentSystem paymentSystem;
        private readonly IOrdersQueries ordersQueries;
        private readonly ILogger<PayOrderHandler> logger;
        private readonly IMediator mediator;

        public PayOrderHandler(ILogger<PayOrderHandler> logger, IMediator mediator, IOrdersQueries ordersQueries, IExternalPaymentSystem paymentSystem)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.ordersQueries = ordersQueries ?? throw new ArgumentNullException(nameof(ordersQueries));
            this.paymentSystem = paymentSystem ?? throw new ArgumentNullException(nameof(paymentSystem));
        }

        public async Task<PaymentResponse> Handle(PayOrder command, CancellationToken cancellationToken)
        {
            if (command is null) return new PaymentResponse() { Rejected = true };

            this.logger.LogInformation("Processing command: {@Command}", command);

            OrderView? order = await this.ordersQueries.GetAsync(command.OrderId);
            if (order is null)
            {
                return new PaymentResponse()
                {
                    Rejected = true,
                    Errors = new List<Error>(1) { new Error($"Order {command.OrderId} not found.") }
                };
            }

            if (!order.Status.Equals(OrderStatus.Started))
            {
                return new PaymentResponse()
                {
                    Rejected = true,
                    Errors = new List<Error>(1) { new Error($"Orders with status {order.Status} cannot be paied.") }
                };
            }

            PaymentRequestResponseDto paymentResult = TryPayOrder(command, order);

            //TODO: melhorar a criacao do payment response
            if (!paymentResult.Aproved)
            {
                return new PaymentResponse()
                {
                    Rejected = true,
                    Errors = new List<Error>(1) { new Error(paymentResult.Reason) }
                };
            }

            this.logger.LogInformation("Payment for order: {@Order} confirmed", order);

            await this.mediator.Publish(new PaymentApproved(order.Id, paymentResult.Id));

            return new PaymentResponse()
            {
                Rejected = false
            };
        }

        private PaymentRequestResponseDto TryPayOrder(PayOrder request, OrderView order) => order.PaymentMethod switch
        {
            PaymentMethod method when method.Equals(PaymentMethod.CreditCard) => this.paymentSystem.TryPayCreditCard(request.CreditCard, order.Price),
            PaymentMethod method when method.Equals(PaymentMethod.Cash) => this.paymentSystem.TryPayCash(order.Price),
            _ => new PaymentRequestResponseDto()
            {
                Id = Guid.NewGuid().ToString(),
                Aproved = false,
                Reason = "Payment method not accepted"
            }
        };
    }
}
