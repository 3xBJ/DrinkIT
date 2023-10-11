namespace DrinkIT.Payment.DTO
{
    public class CreditCardDataDto
    {
        public string CardHolder { get; set; }
        public string CardNumder { get; set; }
        public short CVV { get; set; }
        public string ExpirationDate { get; set; }
    }
}
