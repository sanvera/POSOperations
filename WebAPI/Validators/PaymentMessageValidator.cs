using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Helpers.Enums;
using WebAPI.Messages;

namespace WebAPI.Validators
{
    public class PaymentMessageValidator
    {
        public IList<ValidationResult> ValidationResults { get; set; }

        public PaymentMessageValidator()
        {
            ValidationResults = new List<ValidationResult>();
        }

        public IEnumerable<ValidationResult> Validate(PaymentMessage message)
        {
            ValidateMessageType(message.MessageType);
            ValidateOrigin(message.Origin);
            return ValidationResults;
        }

        public void ValidateMessageType(string messageType)
        {
            if (!messageType.Equals(MessageType.PAYMENT.ToString(), StringComparison.OrdinalIgnoreCase) &&
                !messageType.Equals(MessageType.ADJUSTMENT.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                ValidationResults.Add(new ValidationResult("Invalid Message Type"));
            }
        }

        public void ValidateOrigin(string origin)
        {
            if (!origin.Equals(Origin.MASTER.ToString(), StringComparison.OrdinalIgnoreCase) &&
                !origin.Equals(Origin.VISA.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                ValidationResults.Add(new ValidationResult("Invalid Origin"));
            }
        }
    }
}
