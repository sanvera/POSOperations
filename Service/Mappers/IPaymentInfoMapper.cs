using Data.Entities;
using Service.Models;

namespace Services.Mappers
{
    public interface IPaymentInfoMapper
    {
        Payment MapPaymentInfo(PaymentInfo paymentInfo);
    }
}