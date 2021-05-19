using Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IPosDbRepository
    {
        void SetupPaymentsTable();

        void SetupAccountsTable();

        Task<IEnumerable<Payment>> SelectAllPaymentsAsync();

        Task<Payment> SelectPaymentAsync(Payment payment);

        Task<Payment> SelectPaymentByTransactionIdAsync(Guid transactionId);

        Task<int> InsertPaymentAsync(Payment payment);

        Task<int> UpdatePaymentAsync(Payment payment, decimal newBalance);

        Task<Account> SelectAccountByIdAsync(long accountId);

        Task<decimal> GetAccountBalanceAsync(long accountid);

        Task<int> InsertAccountAsync(Account account, IDbTransaction transaction);

        Task<int> UpdateAccountBalanceAsync(Payment payment, IDbTransaction transaction);

        Task<int> AddAccountBalanceAsync(Payment payment, IDbTransaction transaction);
    }
}