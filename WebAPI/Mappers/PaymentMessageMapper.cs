using Service.Models;
using WebAPI.Messages;

namespace WebAPI.Mappers
{
    public class PaymentMessageMapper : IPaymentMessageMapper
    {
        public PaymentInfo MapPaymentMessage(PaymentMessage message)
        {
            return new PaymentInfo(message.TransactionId, message.AccountId, message.Origin, message.Amount);
        }
    }
}
