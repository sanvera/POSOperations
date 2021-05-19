using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;
using Helper;
using Helpers.Enums;
using WebAPI.Messages;
using Xunit;

namespace WebAPI.IntegrationTests.ApiTests
{
    public class PaymentApiShould : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient Client;

        public PaymentApiShould(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Theory]
        [InlineData(MessageType.PAYMENT, 4755, 15000, Origin.VISA)]
        [InlineData(MessageType.PAYMENT, 9834, 3000, Origin.MASTER)]
        public async Task ReturnOKWhenPostPaymentRequestIsValid(MessageType messageType, long accountId, decimal amount, Origin origin)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            PaymentMessage paymentMessage = SetPaymentMessage(transactionId, accountId, amount, origin, messageType);

            var paymentRequest = ContentHelper.GetStringContent(paymentMessage);

            // Act
            var response = await Client.PostAsync("/Payment", paymentRequest);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(MessageType.PAYMENT, 9999999, 15000, Origin.VISA)]
        [InlineData(MessageType.PAYMENT, 5555, 3000, Origin.MASTER)]
        public async Task ReturnErrorWhenPostPaymentRequestIsInvalid(MessageType messageType, long accountId, decimal amount, Origin origin)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            PaymentMessage paymentMessage = SetPaymentMessage(transactionId, accountId, amount, origin, messageType);

            var paymentRequest = ContentHelper.GetStringContent(paymentMessage);

            // Act
            var response = await Client.PostAsync("/Payment", paymentRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Theory]
        [InlineData(MessageType.PAYMENT, 4755, 15000, Origin.VISA)]
        [InlineData(MessageType.PAYMENT, 9834, 3000, Origin.MASTER)]
        [InlineData(MessageType.PAYMENT, 7735, 7000, Origin.VISA)]
        [InlineData(MessageType.PAYMENT, 9834, 13000, Origin.MASTER)]
        public async Task UpdateBalanceWhenPostPaymentsIsSuccessful(MessageType messageType, long accountId, decimal amount, Origin origin)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            PaymentMessage paymentMessage = SetPaymentMessage(transactionId, accountId, amount, origin, messageType);

            var initialBalance = await GetAccountBalance(accountId).ConfigureAwait(false);

            // Assert

            var paymentRequest = ContentHelper.GetStringContent(paymentMessage);

            // Act
            await Client.PostAsync("/Payment", paymentRequest).ConfigureAwait(false);

            // Assert
            var finalBalance = await GetAccountBalance(accountId).ConfigureAwait(false);
            var commisionaddedAmount = GetCommissionAddedAmount(amount, origin);
            var expectedBalance = initialBalance + commisionaddedAmount;

            // Assert
            finalBalance.Should().Be(expectedBalance);
        }

        [Theory]
        [InlineData(4755, 1000, 500, Origin.VISA)]
        [InlineData(4755, 500, 1000, Origin.VISA)] 
        [InlineData(4755, 1000, 500, Origin.MASTER)]
        [InlineData(4755, 500, 1000, Origin.MASTER)]         
        [InlineData(9834, 1000, 500, Origin.VISA)]
        [InlineData(9834, 500, 1000, Origin.VISA)] 
        [InlineData(9834, 1000, 500, Origin.MASTER)]
        [InlineData(9834, 500, 1000, Origin.MASTER)] 
        [InlineData(7735, 1000, 500, Origin.VISA)]
        [InlineData(7735, 500, 1000, Origin.VISA)] 
        [InlineData(7735, 1000, 500, Origin.MASTER)]
        [InlineData(7735, 500, 1000, Origin.MASTER)] 
        public async Task UpdateBalanceSuccessfullyWhenPostPaymentAdjustmentIsSuccessful(long accountId, decimal paymentAmount, decimal adjustmentAmount, Origin origin)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var messageType = MessageType.PAYMENT;
            PaymentMessage paymentMessage = SetPaymentMessage(transactionId, accountId, paymentAmount, origin, messageType);

            var initialBalance = await GetAccountBalance(accountId).ConfigureAwait(false);

            // Assert

            var paymentRequest = ContentHelper.GetStringContent(paymentMessage);

            // Act
            await Client.PostAsync("/Payment", paymentRequest).ConfigureAwait(false);

            messageType = MessageType.ADJUSTMENT;
            var adjustmentMessage = SetPaymentMessage(transactionId, accountId, adjustmentAmount, origin, messageType);
            var adjustmentRequest = ContentHelper.GetStringContent(adjustmentMessage);

            await Client.PostAsync("/Payment", adjustmentRequest).ConfigureAwait(false);

            // Assert
            var finalBalance = await GetAccountBalance(accountId).ConfigureAwait(false);
            decimal commisionaddedAdjustmentAmount = GetCommissionAddedAmount(adjustmentAmount, origin);
            var expectedBalance = initialBalance + commisionaddedAdjustmentAmount;

            // Assert
            finalBalance.Should().Be(expectedBalance);
        }

        private static decimal GetCommissionAddedAmount(decimal amount, Origin origin)
        {
            return amount * (origin == Origin.MASTER ? 1.02M : 1.01M);
        }

        private static PaymentMessage SetPaymentMessage(Guid transactionId, long accountId, decimal amount, Origin origin, MessageType messageType)
        {
            return new PaymentMessage()
            {
                MessageType = messageType.ToString(),
                AccountId = accountId,
                TransactionId = transactionId,
                Amount = amount,
                Origin = origin.ToString()
            };
        }

        private async Task<decimal> GetAccountBalance(long accountId)
        {
            var accountBalanceRequest = $"/AccountBalance?accountId={accountId}";
            var accountBalancResponse = await Client.GetAsync(accountBalanceRequest).ConfigureAwait(false);
            var balanceContent = await accountBalancResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return Convert.ToDecimal(balanceContent);
        }
    }
}