using Service.Models;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IAccountBalanceService
    {
        Task<AccountBalanceResult> GetAccountBalanceById(long accountId);
    }
}