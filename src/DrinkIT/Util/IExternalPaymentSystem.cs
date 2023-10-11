using DrinkIT.Payment.DTO;

namespace DrinkIT.Util
{
    public interface IExternalPaymentSystem
    {
        public PaymentRequestResponseDto TryPayCreditCard(CreditCardDataDto? creditCard, decimal price);
        public PaymentRequestResponseDto TryPayCash(decimal price);
    }
}
