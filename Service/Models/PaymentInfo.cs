using System;

namespace Service.Models
{
    public class PaymentInfo
    {
        public PaymentInfo(Guid transactionId, long accountId, string origin, decimal amount)
        {
            TransactionId = transactionId;
            AccountId = accountId;
            Origin = origin;
            Amount = amount;
        }

        public Guid TransactionId { get; set; }

        public long AccountId { get; set; }

        public string Origin { get; set; }

        public decimal Amount { get; set; }

    }
}
