using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Payment
    {
        public Payment(Guid transactionId, long accountId, string origin, decimal amount)
        {
            TransactionId = transactionId;
            AccountId = accountId;
            Origin = origin;
            Amount = amount;
        }

        [Required]
        public Guid TransactionId { get; set; }

        [Required]
        public long AccountId { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public decimal Amount { get; set; }

    }
}
