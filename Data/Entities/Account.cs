using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Account
    {
        public Account(long accountId, decimal balance)
        {
            AccountId = accountId;
            Balance = balance;
        }

        [Required]
        public long AccountId { get; set; }

        [Required]
        public decimal Balance { get; set; }

    }
}
