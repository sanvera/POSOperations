namespace Service.Models
{
    public class AccountBalanceResult
    {
        public AccountBalanceResult(bool isSuccess, string message, decimal? accountBalance = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            AccountBalance = accountBalance;
        }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public decimal? AccountBalance { get; set; }

    }
}
