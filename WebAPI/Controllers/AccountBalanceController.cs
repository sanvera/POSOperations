using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Service.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountBalanceController : ControllerBase
    {
        private readonly ILogger<AccountBalanceController> _logger;
        private readonly IAccountBalanceService _accountBalanceService;

        public AccountBalanceController(ILogger<AccountBalanceController> logger, IAccountBalanceService accountBalanceService)
        {
            _logger = logger;
            _accountBalanceService = accountBalanceService;
        }

        [HttpGet]
        public async Task<ObjectResult> GetBalance(long accountId)
        {
            _logger.LogInformation($"Getting Balance for account: {accountId}");

            var accountBalanceResult = await _accountBalanceService.GetAccountBalanceById(accountId).ConfigureAwait(false);

            return accountBalanceResult.IsSuccess ? StatusCode((int)HttpStatusCode.OK, accountBalanceResult.AccountBalance)
                                                  : StatusCode((int)HttpStatusCode.InternalServerError, accountBalanceResult.Message);
        }
    }
}
