using System.Threading.Tasks;
using Data.Repositories;
using Helpers.Enums;

using Service.Models;
using Services.Mappers;

namespace Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPosDbRepository _posDbRepository;
        private readonly IPaymentInfoMapper _paymentInfoMapper;

        public PaymentService(IPosDbRepository posDbRepository, IPaymentInfoMapper paymentInfoMapper)
        {
            _posDbRepository = posDbRepository;
            _paymentInfoMapper = paymentInfoMapper;
        }

        public async Task<PaymentResult> MakeAdjustment(PaymentInfo paymentInfo)
        {
            var payment = _paymentInfoMapper.MapPaymentInfo(paymentInfo);
            var existingPayment = await _posDbRepository.SelectPaymentAsync(payment).ConfigureAwait(false);

            var account = await _posDbRepository.SelectAccountByIdAsync(payment.AccountId).ConfigureAwait(false);
            if (account == null)
            {
                return new PaymentResult(false, $"Account with id: {payment.AccountId} not found");
            }

            if (existingPayment == null)
            {
                return new PaymentResult(false, "No payment found to adjust.");
            }

            payment.Amount = GetCommissionAddedAmount(paymentInfo);

            decimal newBalance = GetNewBalance(payment.Amount, existingPayment.Amount, account.Balance);

            int updatedPayments = await _posDbRepository.UpdatePaymentAsync(payment, newBalance).ConfigureAwait(false);

            if (updatedPayments > 0)
            {
                return new PaymentResult(true, "Payment has been updated");
            }
            return new PaymentResult(false, "Update Failed");
        }

        public async Task<PaymentResult> MakePayment(PaymentInfo paymentInfo)
        {
            var payment = _paymentInfoMapper.MapPaymentInfo(paymentInfo);

            var existingPayment = await _posDbRepository.SelectPaymentAsync(payment).ConfigureAwait(false);

            if (existingPayment != null)
            {
                return new PaymentResult(false, $"An existing transaction with Transaction id: {payment.TransactionId} has been found");
            }

            var paymentAccount = await _posDbRepository.SelectAccountByIdAsync(paymentInfo.AccountId).ConfigureAwait(false);

            if (paymentAccount == null)
            {
                return new PaymentResult(false, $"Account with id: {paymentInfo.AccountId} not found");
            }

            payment.Amount = GetCommissionAddedAmount(paymentInfo);

            int numberOfRowsInserted = await _posDbRepository.InsertPaymentAsync(payment).ConfigureAwait(false);

            if (numberOfRowsInserted > 0)
            {
                return new PaymentResult(true, $"Payment has been completed. Transaction id: {payment.TransactionId}");
            }

            return new PaymentResult(false, $"Payment Has Failed for Transaction id: {payment.TransactionId}");
        }

        public decimal GetCommissionAddedAmount(PaymentInfo paymentInfo)
        {
            return paymentInfo.Origin.Equals(Origin.VISA.ToString()) ? paymentInfo.Amount * 1.01M : paymentInfo.Amount * 1.02M;
        }

        public static decimal GetNewBalance(decimal newPaymentAmount, decimal existingPaymentAmount, decimal existingAccountBalance)
        {
            return existingAccountBalance - existingPaymentAmount + newPaymentAmount;
        }

    }
}
