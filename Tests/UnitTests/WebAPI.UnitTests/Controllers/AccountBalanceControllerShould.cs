using System.Net;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

using Service.Models;
using Service.Services;
using WebAPI.Controllers;
using Xunit;

namespace WebAPI.UnitTests.Controllers
{
    public class AccountBalanceControllerShould
    {
        private readonly ILogger<AccountBalanceController> _logger;

        public AccountBalanceControllerShould()
        {
            _logger = Mock.Of<ILogger<AccountBalanceController>>();
        }

        [Theory]
        [InlineData(112345, 1700)]
        [InlineData(6723645, 1900)]
        [InlineData(54234556434, 7000)]
        public void ReturnOKWhenGetAccountBalanceIsSucceeded(long accountId, decimal balance)
        {
            // Arrange
            var accountBalanceService = SetupMockAccountBalanceService(true, balance);
            var accountBalanceController = new AccountBalanceController(_logger, accountBalanceService);

            // Act
            var accountBalanceResult = accountBalanceController.GetBalance(accountId).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            accountBalanceResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            accountBalanceResult.Value.Should().Be(balance);
        }

        [Theory]
        [InlineData(454346, 3000)]
        [InlineData(23235467, 72100)]
        [InlineData(89778979789, 100)]
        public void ReturnInternalServerErrorWhenGetAccountBalanceIsFailed(long accountId, decimal balance)
        {
            // Arrange
            var accountBalanceService = SetupMockAccountBalanceService(false, balance);
            var accountBalanceController = new AccountBalanceController(_logger, accountBalanceService);

            // Act
            var accountBalanceResult = accountBalanceController.GetBalance(accountId).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            accountBalanceResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            accountBalanceResult.Value.Should().Be("Failed");
        }

        private static IAccountBalanceService SetupMockAccountBalanceService(bool isSuccess, decimal balance)
        {
            var mockAccountBalanceService = new Mock<IAccountBalanceService>();
            var message = isSuccess ? "OK" : "Failed";
            var accountBalanceResult = new AccountBalanceResult(isSuccess, message, isSuccess ? balance : null);

            mockAccountBalanceService.Setup(ps => ps.GetAccountBalanceById(It.IsAny<long>())).Returns(Task.FromResult(accountBalanceResult));
            return mockAccountBalanceService.Object;
        }
    }
}
