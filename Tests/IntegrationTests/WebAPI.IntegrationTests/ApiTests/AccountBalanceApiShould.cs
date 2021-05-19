using System;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

namespace WebAPI.IntegrationTests.ApiTests
{
    public class AccountBalanceApiShould : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient Client;

        public AccountBalanceApiShould(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Fact]
        public async Task RunAccountBalanceAsyncSuccessfully()
        {
            // Arrange
            var accountId = 4755;
            var request = $"/AccountBalance?accountId={accountId}";

            // Act
            var response = await Client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(4755, 1001.88)]
        [InlineData(9834, 456.45)]
        [InlineData(7735, 89.36)]
        public async Task GetAccountBalanceCorrectlyWhenAccountIsInDatabase(long accountId, decimal expectedBalance)
        {
            // Arrange
            var request = $"/AccountBalance?accountId={accountId}";

            // Act
            var response = await Client.GetAsync(request).ConfigureAwait(false);
            
            // Assert
            var actualBalance = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Convert.ToDecimal(actualBalance).Should().Be(expectedBalance);
        }
    }
}