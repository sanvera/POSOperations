using Services.Mappers;
using System;

using FluentAssertions;
using Helpers.Enums;

using Xunit;
using Data.Entities;
using Service.Models;

namespace Services.UnitTests.Mappers
{
    public class PaymentInfoMapperShould
    {
        [Theory]
        [InlineData(2578, Origin.MASTER, 3000)]
        [InlineData(423243578, Origin.MASTER, 4000)]
        [InlineData(12345678901, Origin.VISA, 2000)]
        [InlineData(346234, Origin.VISA, 5000)]
        [InlineData(98787878, Origin.MASTER, 3000)]
        public void MapWhenPaymentInfoIsValid(long accountId, Origin origin, decimal amount)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var paymentInfoMapper = new PaymentInfoMapper();
            PaymentInfo paymentInfo = SetPaymentInfo(accountId, origin, amount, transactionId);

            // Act
            Payment payment = paymentInfoMapper.MapPaymentInfo(paymentInfo);

            // Assert
            payment.AccountId.Should().Be(paymentInfo.AccountId);
            payment.TransactionId.Should().Be(paymentInfo.TransactionId);
            payment.Origin.Should().Be(paymentInfo.Origin);
            payment.Amount.Should().Be(paymentInfo.Amount);
        }

        private static PaymentInfo SetPaymentInfo(long accountId,  Origin origin, decimal amount, Guid transactionId) => new(
            transactionId,
            accountId,
            origin.ToString(),
            amount
        );
    }
}
