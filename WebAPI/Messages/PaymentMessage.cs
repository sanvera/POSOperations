using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using WebAPI.Validators;

namespace WebAPI.Messages
{
    public class PaymentMessage : PaymentMessageValidator, IValidatableObject
    {
        [Required]
        public string MessageType { get; set; }

        [Required]
        public Guid TransactionId { get; set; }

        [Required]
        public long AccountId { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Validate(this);
        }
    }
}
