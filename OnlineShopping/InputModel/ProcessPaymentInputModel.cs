namespace OnlineShopping.Models
{
    public class ProcessPaymentInputModel
    {
        public int CartId { get; set; }
        public required CardInformation CardInfo { get; set; }
    }
}
