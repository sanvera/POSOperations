using System.Linq;
using FluentAssertions;
using Helpers.Enums;
using WebAPI.Validators;
using Xunit;

namespace WebAPI.UnitTests.Validators
{
    public class PaymentMessageValidatorShould
    {
        [Theory]
        [InlineData(MessageType.ADJUSTMENT)]
        [InlineData(MessageType.PAYMENT)]
        public void PassWhenMessageTypeIsValid(MessageType messageType)
        {
            // Arrange
            var paymentMessageValidator = new PaymentMessageValidator();

            // Act
            paymentMessageValidator.ValidateMessageType(messageType.ToString());

            // Assert
            paymentMessageValidator.ValidationResults.Should().BeEmpty();
        }

        [Theory]
        [InlineData("REIMBURSEMENT")]
        [InlineData("UPDATE")]
        [InlineData("")]
        public void FailWhenMessageTypeIsInvalid(string messageType)
        {
            // Arrange
            var paymentMessageValidator = new PaymentMessageValidator();

            // Act
            paymentMessageValidator.ValidateMessageType(messageType);

            // Assert
            paymentMessageValidator.ValidationResults.ToList().ForEach(vr => vr.ErrorMessage.Equals("Invalid Message Type"));
        }
        
        [Theory]
        [InlineData(Origin.MASTER)]
        [InlineData(Origin.VISA)]
        public void PassWhenOriginIsValid(Origin origin)
        {
            // Arrange
            var paymentMessageValidator = new PaymentMessageValidator();

            // Act
            paymentMessageValidator.ValidateOrigin(origin.ToString());

            // Assert
            paymentMessageValidator.ValidationResults.Should().BeEmpty();
        }

        [Theory]
        [InlineData("AMERICANEXPRESS")]
        [InlineData("DISCOVER")]
        [InlineData("")]
        public void FailWhenOriginIsInvalid(string origin)
        {
            // Arrange
            var paymentMessageValidator = new PaymentMessageValidator();

            // Act
            paymentMessageValidator.ValidateOrigin(origin);

            // Assert
            paymentMessageValidator.ValidationResults.ToList().ForEach(vr => vr.ErrorMessage.Equals("Invalid Origin"));
        }

    }
}
