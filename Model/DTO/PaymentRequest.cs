using Model.Enum;

namespace Model.DTO
{
    public class PaymentRequest
    {
        public CurrencyCode Currency { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}