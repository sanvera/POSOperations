using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Data.Entities;
using Data.Queries;

namespace Data.Repositories
{
    public class PosDbRepository : IPosDbRepository
    {
        private readonly IDbConnection _connection;

        public PosDbRepository(IDbConnection connection)
        {
            _connection = connection;
            InitialiseInMemoryDb();
        }

        protected void InitialiseInMemoryDb()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
                SetupPaymentsTable();
                SetupAccountsTable();
            }
        }

        public void SetupPaymentsTable()
        {
            _connection.ExecuteAsync(PaymentQueries.CreatePaymentsTableQuery).ConfigureAwait(false);
        }

        public void SetupAccountsTable()
        {
            using var transaction = _connection.BeginTransaction();
            try
            {
                _connection.ExecuteAsync(AccountQueries.CreateAccountTableQuery).ConfigureAwait(false);
                var accounts = GetInitialAccounts();

                foreach (var account in accounts)
                {
                    InsertAccountAsync(account, transaction).ConfigureAwait(false);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        public async Task<IEnumerable<Payment>> SelectAllPaymentsAsync()
        {
            return await _connection.QueryAsync<Payment>(PaymentQueries.SelectTableQuery).ConfigureAwait(false);
        }

        public async Task<int> InsertPaymentAsync(Payment payment)
        {
            using var transaction = _connection.BeginTransaction();
            var insertedPayments = 0;
            try
            {
                insertedPayments = await _connection.ExecuteAsync(PaymentQueries.InsertIntoPaymentsTableQuery, payment, transaction)
                    .ConfigureAwait(false);

                if (insertedPayments > 0)
                {
                    var updatedAccounts = await AddAccountBalanceAsync(payment, transaction).ConfigureAwait(false);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return 0;
            }
            return insertedPayments;
        }

        public async Task<Payment> SelectPaymentAsync(Payment payment)
        {
            var result = await _connection.QueryAsync<Payment>(PaymentQueries.SelectIndividualPaymentQuery, payment).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        public async Task<Payment> SelectPaymentByTransactionIdAsync(Guid transactionId)
        {
            var result = await _connection.QueryAsync<Payment>(PaymentQueries.SelectPaymentByTransactionIdQuery,
                new { TransactionId = transactionId }).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        public async Task<int> UpdatePaymentAsync(Payment payment, decimal newBalance)
        {
            using var transaction = _connection.BeginTransaction();
            var updatedPayments = 0;
            try
            {
                updatedPayments = await _connection.ExecuteAsync(PaymentQueries.UpdatePaymentQuery, payment, transaction)
                    .ConfigureAwait(false);

                if (updatedPayments > 0)
                {
                    payment.Amount = newBalance;
                    var updatedAccounts = await UpdateAccountBalanceAsync(payment, transaction).ConfigureAwait(false);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return 0;
            }
            return updatedPayments;
        }

        public async Task<decimal> GetAccountBalanceAsync(long accountId)
        {
            var result = await _connection.QueryAsync<decimal>(AccountQueries.SelectAccountBalanceQuery, new { AccountId = accountId })
                .ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        public async Task<Account> SelectAccountByIdAsync(long accountId)
        {
            var result = await _connection.QueryAsync<Account>(AccountQueries.SelectAccountByAccountId,
                new { AccountId = accountId }).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        public async Task<int> InsertAccountAsync(Account account, IDbTransaction transaction)
        {
            return await _connection.ExecuteAsync(AccountQueries.InsertIntoAccountsTableQuery, account, transaction).ConfigureAwait(false);
        }

        public async Task<int> UpdateAccountBalanceAsync(Payment payment, IDbTransaction transaction)
        {
            return await _connection.ExecuteAsync(AccountQueries.UpdateBalanceQuery, payment, transaction).ConfigureAwait(false);
        }

        public async Task<int> AddAccountBalanceAsync(Payment payment, IDbTransaction transaction)
        {
            return await _connection.ExecuteAsync(AccountQueries.AddBalanceQuery, payment, transaction).ConfigureAwait(false);
        }

        private static List<Account> GetInitialAccounts()
        {
            return new List<Account> {
            new Account(4755, 1001.88M),
            new Account(9834, 456.45M),
            new Account(7735, 89.36M)};
        }
    }
}
