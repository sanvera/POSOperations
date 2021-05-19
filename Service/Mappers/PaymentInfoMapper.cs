using Data.Entities;
using Service.Models;

namespace Services.Mappers
{
    public class PaymentInfoMapper : IPaymentInfoMapper
    {
        public Payment MapPaymentInfo(PaymentInfo paymentInfo)
        {
            return new Payment(paymentInfo.TransactionId, paymentInfo.AccountId, paymentInfo.Origin, paymentInfo.Amount);
        }
    }
}
