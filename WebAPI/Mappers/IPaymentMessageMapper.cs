using Service.Models;
using WebAPI.Messages;

namespace WebAPI.Mappers
{
    public interface IPaymentMessageMapper
    {
        PaymentInfo MapPaymentMessage(PaymentMessage message);
    }
}