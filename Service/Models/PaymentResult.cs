namespace Service.Models
{
    public class PaymentResult
    {
        public PaymentResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}
