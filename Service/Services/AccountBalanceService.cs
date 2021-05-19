using System.Threading.Tasks;
using Data.Repositories;
using Service.Models;

namespace Service.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IPosDbRepository _posDbRepository;

        public AccountBalanceService(IPosDbRepository posDbRepository)
        {
            _posDbRepository = posDbRepository;
        }

        public async Task<AccountBalanceResult> GetAccountBalanceById(long accountId)
        {
            var account = await _posDbRepository.SelectAccountByIdAsync(accountId).ConfigureAwait(false);
            if (account == null)
            {
                return new AccountBalanceResult(false, $"Account with id {accountId} not found");
            }

            return new AccountBalanceResult(true, $"Account {account.AccountId} found", account.Balance);
        }
    }
}
