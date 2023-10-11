namespace DrinkIT.Payment.DTO
{
    public class PaymentRequestResponseDto
    {
        public required string Id { get; init; }
        public required bool Aproved { get; init; } 
        public required string Reason { get; init; } 
    }
}
