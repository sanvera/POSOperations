using Service.Models;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> MakeAdjustment(PaymentInfo paymentInfo);

        Task<PaymentResult> MakePayment(PaymentInfo paymentInfo);

        decimal GetCommissionAddedAmount(PaymentInfo paymentInfo);
    }
}