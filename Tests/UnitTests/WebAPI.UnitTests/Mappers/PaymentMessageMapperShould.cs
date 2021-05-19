using System;
using FluentAssertions;
using Helpers.Enums;
using Service.Models;
using WebAPI.Mappers;
using WebAPI.Messages;
using Xunit;

namespace WebAPI.UnitTests.Mappers
{
    public class PaymentMessageMapperShould
    {
        [Theory]
        [InlineData(2578, MessageType.PAYMENT, Origin.MASTER, 3000)]
        [InlineData(423243578, MessageType.ADJUSTMENT, Origin.MASTER, 4000)]
        [InlineData(12345678901, MessageType.PAYMENT, Origin.VISA, 2000)]
        [InlineData(346234, MessageType.PAYMENT, Origin.VISA, 5000)]
        [InlineData(98787878, MessageType.PAYMENT, Origin.MASTER, 3000)]
        public void MapWhenPaymentMessageIsValid(long accountId, MessageType messageType, Origin origin, decimal amount)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var paymentMessageMapper = new PaymentMessageMapper();
            PaymentMessage paymentMessage = SetPaymentMessage(accountId, messageType, origin, amount, transactionId);

            // Act
            PaymentInfo paymentInfo = paymentMessageMapper.MapPaymentMessage(paymentMessage);

            // Assert
            paymentInfo.AccountId.Should().Be(paymentMessage.AccountId);
            paymentInfo.TransactionId.Should().Be(paymentMessage.TransactionId);
            paymentInfo.Origin.Should().Be(paymentMessage.Origin);
            paymentInfo.Amount.Should().Be(paymentMessage.Amount);
        }

        private static PaymentMessage SetPaymentMessage(long accountId, MessageType messageType, Origin origin, decimal amount, Guid transactionId)
        {
            return new PaymentMessage()
            {
                AccountId = accountId,
                TransactionId = transactionId,
                MessageType = messageType.ToString(),
                Origin = origin.ToString(),
                Amount = amount
            };
        }

    }
}
