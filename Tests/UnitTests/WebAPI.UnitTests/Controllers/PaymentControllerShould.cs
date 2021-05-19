using System;
using System.Net;
using System.Threading.Tasks;

using FluentAssertions;
using Helpers.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Service.Models;
using Service.Services;
using WebAPI.Controllers;
using WebAPI.Mappers;
using WebAPI.Messages;
using Xunit;

namespace WebAPI.UnitTests.Controllers
{
    public class PaymentControllerShould
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentMessageMapper _paymentMessageMapper;

        public PaymentControllerShould()
        {
            _logger = Mock.Of<ILogger<PaymentController>>();
            _paymentMessageMapper = Mock.Of<IPaymentMessageMapper>(); ;
        }

        [Theory]
        [InlineData(MessageType.ADJUSTMENT, Origin.MASTER)]
        [InlineData(MessageType.ADJUSTMENT, Origin.VISA)]
        [InlineData(MessageType.PAYMENT, Origin.MASTER)]
        [InlineData(MessageType.PAYMENT, Origin.VISA)]
        public void ReturnOKWhenPaymentIsSucceeded(MessageType messageType, Origin origin)
        {
            // Arrange
            var paymentService = SetupMockPaymentService(true);
            var paymentController = new PaymentController(_logger, paymentService, _paymentMessageMapper);
            var paymentMessage = SetPaymentMessage(messageType, origin);
            
            // Act
            var paymentResult = paymentController.PostPayment(paymentMessage).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            paymentResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            paymentResult.Value.Should().Be("OK");
        }

        [Theory]
        [InlineData(MessageType.ADJUSTMENT, Origin.MASTER)]
        [InlineData(MessageType.ADJUSTMENT, Origin.VISA)]
        [InlineData(MessageType.PAYMENT, Origin.MASTER)]
        [InlineData(MessageType.PAYMENT, Origin.VISA)]
        public void ReturnInternalServerErrorWhenPaymentIsFailed(MessageType messageType, Origin origin)
        {
            // Arrange
            var paymentService = SetupMockPaymentService(false);
            var paymentController = new PaymentController(_logger, paymentService, _paymentMessageMapper);
            var paymentMessage = SetPaymentMessage(messageType, origin);

            // Act
            var paymentResult = paymentController.PostPayment(paymentMessage).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            paymentResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            paymentResult.Value.Should().Be("Failed");
        }

        private static IPaymentService SetupMockPaymentService(bool isSuccess)
        {
            var mockPaymentService = new Mock<IPaymentService>();
            var message = isSuccess ? "OK" : "Failed";
            var paymentResult = new PaymentResult(isSuccess, message);

            mockPaymentService.Setup(ps => ps.MakePayment(It.IsAny<PaymentInfo>())).Returns(Task.FromResult(paymentResult));
            mockPaymentService.Setup(ps => ps.MakeAdjustment(It.IsAny<PaymentInfo>())).Returns(Task.FromResult(paymentResult));
            return mockPaymentService.Object;
        }

        private static PaymentMessage SetPaymentMessage(MessageType messageType, Origin origin) 
        {
            return new PaymentMessage
            {
                AccountId = 1,
                TransactionId = Guid.NewGuid(),
                Amount = 1500M,
                MessageType = messageType.ToString(),
                Origin = origin.ToString()
            };
        }
    }
}
