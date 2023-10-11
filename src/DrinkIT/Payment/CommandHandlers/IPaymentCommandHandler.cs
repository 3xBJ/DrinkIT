using DrinkIT.BaseClasses;
using MediatR;

namespace DrinkIT.Payment.CommandHandlers
{
    public interface IPaymentCommandHandler<CommandType> : IRequestHandler<CommandType, PaymentResponse> where CommandType : Command<PaymentResponse>
    {
    }
}
