using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Repositories;
using FluentAssertions;
using Moq;
using Service.Models;
using Service.Services;
using Xunit;

namespace Service.UnitTests.Services
{
    public class AccountBalanceServiceShould
    {
        public static List<Account> AccountList { get; set; }
        
        public AccountBalanceServiceShould()
        {
            SetAccountList();
        }

        [Theory]
        [InlineData(1, 15000)]
        [InlineData(2, 20000)]
        [InlineData(3, 40000)]
        [InlineData(4, 75000)]
        public void ReturnBalanceSuccessfullyWhenAccountIsInDatabase(long accountId, decimal expectedBalance)
        {
            // Arrange
            IPosDbRepository posDbRepository = SetupMockPosDbRepository();
            var accountBalanceService = new AccountBalanceService(posDbRepository);

            // Act
            AccountBalanceResult accountBalanceResult = accountBalanceService.GetAccountBalanceById(accountId).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            accountBalanceResult.IsSuccess.Should().BeTrue();
            accountBalanceResult.Message.Should().Be($"Account {accountId} found");
            accountBalanceResult.AccountBalance.Should().Be(expectedBalance);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void ReturnFailureWhenAccountIsNotInDatabase(long accountId)
        {
            // Arrange
            IPosDbRepository posDbRepository = SetupMockPosDbRepository();
            var accountBalanceService = new AccountBalanceService(posDbRepository);

            // Act
            AccountBalanceResult accountBalanceResult = accountBalanceService.GetAccountBalanceById(accountId).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            accountBalanceResult.IsSuccess.Should().BeFalse();
            accountBalanceResult.Message.Should().Be($"Account with id {accountId} not found");
            accountBalanceResult.AccountBalance.Should().BeNull();
        }

        private static IPosDbRepository SetupMockPosDbRepository()
        {
            var mockPosDbRepository = new Mock<IPosDbRepository>();

            mockPosDbRepository.Setup(pr => pr.SelectAccountByIdAsync(It.IsAny<long>())).Returns((long accountId)
                => Task.FromResult(AccountList.FirstOrDefault(py => py.AccountId.Equals(accountId))));

            return mockPosDbRepository.Object;
        }

        private static void SetAccountList()
        {
            AccountList = new List<Account> {
                new Account(1, 15000M),
                new Account(2, 20000M),
                new Account(3, 40000M),
                new Account(4, 75000M)
            };
        }
    }
}
