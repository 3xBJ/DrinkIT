using DrinkIT.Domain.Extensions;
using DrinkIT.Payment.DTO;

namespace DrinkIT.Util
{
    public class ExternalPaymentSystemMock : IExternalPaymentSystem
    {
        private static readonly PaymentRequestResponseDto paymentApproved = new()
        {
            Id = Guid.NewGuid().ToString(),
            Aproved = true,
            Reason = string.Empty
        };

        public PaymentRequestResponseDto TryPayCash(decimal price) => paymentApproved;

        public PaymentRequestResponseDto TryPayCreditCard(CreditCardDataDto? creditCard, decimal price)
        {
            if (CreditCardDataIsValid(creditCard)) return paymentApproved;

            return new()
            {
                Id = Guid.NewGuid().ToString(),
                Aproved = false,
                Reason = "Credit card data is missing"
            };
        }

        private bool CreditCardDataIsValid(CreditCardDataDto? creditCard) =>
            creditCard is not null &&
            !creditCard.CardHolder.IsNullOrEmptyOrWhiteSpace() &&
            !creditCard.CardNumder.IsNullOrEmptyOrWhiteSpace() &&
            !creditCard.ExpirationDate.IsNullOrEmptyOrWhiteSpace() &&
            creditCard.CVV != 0;
    }
}
