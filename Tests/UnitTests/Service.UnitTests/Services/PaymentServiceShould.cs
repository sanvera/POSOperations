using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Repositories;
using FluentAssertions;
using Helpers.Enums;
using Moq;
using Service.Models;
using Service.Services;
using Services.Mappers;
using Xunit;

namespace Service.UnitTests.Services
{
    public class PaymentServiceShould
    {
        private static Guid TransactionId => Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");
        public static List<Account> AccountList { get; set; }

        public static List<Payment> PaymentList { get; set; }
        
        private readonly IPaymentInfoMapper _paymentInfoMapper;

        public PaymentServiceShould()
        {
            _paymentInfoMapper = new PaymentInfoMapper();
            SetAccountList();
            SetPaymentList();
        }

        [Theory]
        [InlineData(1, Origin.MASTER, 3000, 15000)]
        [InlineData(2, Origin.VISA, 4000, 25000)]
        [InlineData(3, Origin.VISA, 2000, 45000)]
        [InlineData(4, Origin.MASTER, 5000, 55000)]
        public void AdjustPaymentsSuccessfullyWhenPaymentIsInDatabase(long accountId, Origin origin, decimal amount, decimal newBalance)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            IPosDbRepository posDbRepository = SetupMockPosDbRepository(true, newBalance);
            var paymentService = new PaymentService(posDbRepository, _paymentInfoMapper);
            PaymentInfo paymentInfo = SetPaymentInfo(accountId, origin, amount, transactionId);
            var expectedamount = paymentService.GetCommissionAddedAmount(paymentInfo);

            // Act
            PaymentResult paymentResult = paymentService.MakeAdjustment(paymentInfo).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            paymentResult.IsSuccess.Should().BeTrue();
            PaymentList.FindLast(payment => payment.AccountId.Equals(accountId)).Amount.Should().Be(expectedamount);
            AccountList.FindLast(payment => payment.AccountId.Equals(accountId)).Balance.Should().Be(newBalance);
        }

        [Theory]
        [InlineData(112578, Origin.MASTER, 3000, 15000)]
        [InlineData(4323243578, Origin.MASTER, 4000, 25000)]
        [InlineData(612345678901, Origin.VISA, 2000, 45000)]
        [InlineData(83746234, Origin.VISA, 5000, 55000)]
        [InlineData(198787878, Origin.MASTER, 3000, 35000)]
        public void FailAdjustmentWhenAccountIsNotInDatabase(long accountId, Origin origin, decimal amount, decimal newBalance)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            IPosDbRepository posDbRepository = SetupMockPosDbRepository(true, newBalance);
            var paymentService = new PaymentService(posDbRepository, _paymentInfoMapper);
            PaymentInfo paymentInfo = SetPaymentInfo(accountId, origin, amount, transactionId);

            // Act
            PaymentResult paymentResult = paymentService.MakeAdjustment(paymentInfo).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            paymentResult.IsSuccess.Should().BeFalse();
            paymentResult.Message.Should().Be($"Account with id: {accountId} not found");
        }

        [Theory]
        [InlineData(1, Origin.VISA, 3000, 15000)]
        [InlineData(2, Origin.MASTER, 4000, 25000)]
        [InlineData(3, Origin.MASTER, 2000, 45000)]
        [InlineData(4, Origin.VISA, 5000, 55000)]
        public void FailAdjustmentWhenPaymentIsNotInDatabase(long accountId, Origin origin, decimal amount, decimal newBalance)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            IPosDbRepository posDbRepository = SetupMockPosDbRepository(true, newBalance);
            var paymentService = new PaymentService(posDbRepository, _paymentInfoMapper);
            PaymentInfo paymentInfo = SetPaymentInfo(accountId, origin, amount, transactionId);

            // Act
            PaymentResult paymentResult = paymentService.MakeAdjustment(paymentInfo).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            paymentResult.IsSuccess.Should().BeFalse();
            paymentResult.Message.Should().Be("No payment found to adjust.");
        }

        [Theory]
        [InlineData(1, Origin.VISA, 5000, 16000)]
        [InlineData(2, Origin.MASTER, 6000, 27000)]
        [InlineData(3, Origin.MASTER, 7000, 48000)]
        [InlineData(4, Origin.VISA, 8000, 59000)]
        public void MakePaymentsSuccessfullyWhenPaymentIsNotInDatabase(long accountId, Origin origin, decimal amount, decimal newBalance)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            IPosDbRepository posDbRepository = SetupMockPosDbRepository(true, newBalance);
            var paymentService = new PaymentService(posDbRepository, _paymentInfoMapper);
            PaymentInfo paymentInfo = SetPaymentInfo(accountId, origin, amount, transactionId);
            var expectedamount = paymentService.GetCommissionAddedAmount(paymentInfo);
            var existingAcccountBalance = AccountList.FirstOrDefault(ac => ac.AccountId.Equals(accountId)).Balance;
            var expectedNewBalance = existingAcccountBalance + expectedamount;

            // Act
            PaymentResult paymentResult = paymentService.MakePayment(paymentInfo).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            paymentResult.IsSuccess.Should().BeTrue();
            PaymentList.Any(payment => payment.TransactionId.Equals(transactionId)).Should().BeTrue();
            PaymentList.First(payment => payment.TransactionId.Equals(transactionId)).AccountId.Should().Be(accountId);
            PaymentList.First(payment => payment.TransactionId.Equals(transactionId)).Origin.Should().Be(origin.ToString());
            PaymentList.First(payment => payment.TransactionId.Equals(transactionId)).Amount.Should().Be(expectedamount);
            AccountList.First(payment => payment.AccountId.Equals(accountId)).Balance.Should().Be(expectedNewBalance);
        }

        [Theory]
        [InlineData(112578, Origin.MASTER, 3000, 15000)]
        [InlineData(4323243578, Origin.MASTER, 4000, 25000)]
        [InlineData(612345678901, Origin.VISA, 2000, 45000)]
        [InlineData(83746234, Origin.VISA, 5000, 55000)]
        [InlineData(198787878, Origin.MASTER, 3000, 35000)]
        public void FailPaymentWhenAccountIsNotInDatabase(long accountId, Origin origin, decimal amount, decimal newBalance)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            IPosDbRepository posDbRepository = SetupMockPosDbRepository(true, newBalance);
            var paymentService = new PaymentService(posDbRepository, _paymentInfoMapper);
            PaymentInfo paymentInfo = SetPaymentInfo(accountId, origin, amount, transactionId);

            // Act
            PaymentResult paymentResult = paymentService.MakePayment(paymentInfo).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            paymentResult.IsSuccess.Should().BeFalse();
            paymentResult.Message.Should().Be($"Account with id: {accountId} not found");
        }

        [Fact]
        public void FailPaymentWhenPaymentIsInDatabase()
        {
            // Arrange
            Guid transactionId = TransactionId;
            long accountId = 1;
            Origin origin = Origin.MASTER;
            decimal amount = 1500M;
            decimal newBalance = 20000M;

            IPosDbRepository posDbRepository = SetupMockPosDbRepository(true, newBalance);
            var paymentService = new PaymentService(posDbRepository, _paymentInfoMapper);
            PaymentInfo paymentInfo = SetPaymentInfo(accountId, origin, amount, transactionId);

            // Act
            PaymentResult paymentResult = paymentService.MakePayment(paymentInfo).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            paymentResult.IsSuccess.Should().BeFalse();
            paymentResult.Message.Should().Be($"An existing transaction with Transaction id: {transactionId} has been found");
        }

        [Theory]
        [InlineData(1, Origin.MASTER, 3000, 3060)]
        [InlineData(2, Origin.MASTER, 4000, 4080)]
        [InlineData(3, Origin.VISA, 2000, 2020)]
        [InlineData(4, Origin.VISA, 5000, 5050)]
        public void ReturnCommissionAddedAmountCorrectly(long accountId, Origin origin, decimal amount, decimal expectedAmount)
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            IPosDbRepository posDbRepository = SetupMockPosDbRepository(true);
            var paymentService = new PaymentService(posDbRepository, _paymentInfoMapper);
            PaymentInfo paymentInfo = SetPaymentInfo(accountId, origin, amount, transactionId);

            // Act
            decimal paymentResult = paymentService.GetCommissionAddedAmount(paymentInfo);

            // Assert
            paymentResult.Should().Be(expectedAmount);
        }

        [Theory]
        [InlineData(10000, 7500, 15000, 17500)]
        [InlineData(5000, 7500, 15000, 12500)]
        [InlineData(10000, 10000, 15000, 15000)]
        public void ReturnNewBalanceCorrectly(decimal newPaymentAmount, decimal existingPaymentAmount, decimal existingBalance, decimal expectedBalance)
        {
            // Act
            decimal paymentResult = PaymentService.GetNewBalance(newPaymentAmount, existingPaymentAmount, existingBalance);

            // Assert
            paymentResult.Should().Be(expectedBalance);
        }

        private static IPosDbRepository SetupMockPosDbRepository(bool isSuccess, decimal? newAccountBalance = null)
        {
            var mockPosDbRepository = new Mock<IPosDbRepository>();
            var message = isSuccess ? "OK" : "Failed";
            var paymentResult = new PaymentResult(isSuccess, message);

            mockPosDbRepository.Setup(pr => pr.SelectPaymentAsync(It.IsAny<Payment>())).Returns((Payment payment)
                => Task.FromResult(PaymentList.FirstOrDefault(py => py.AccountId.Equals(payment.AccountId) && py.Origin.Equals(payment.Origin))));

            mockPosDbRepository.Setup(pr => pr.SelectAccountByIdAsync(It.IsAny<long>())).Returns((long accountId)
                => Task.FromResult(AccountList.FirstOrDefault(py => py.AccountId.Equals(accountId))));

            mockPosDbRepository.Setup(pr => pr.InsertPaymentAsync(It.IsAny<Payment>())).Returns((Payment payment)
                => Task.FromResult(InsertToList(payment)));

            mockPosDbRepository.Setup(pr => pr.UpdatePaymentAsync(It.IsAny<Payment>(), It.IsAny<decimal>())).Returns((Payment payment, decimal newBalance)
                => Task.FromResult(UpdateList(payment, newAccountBalance.Value)));

            return mockPosDbRepository.Object;
        }

        public static int UpdateList(Payment payment, decimal newBalance)
        {
            var existingPayment = PaymentList.FirstOrDefault(py => py.AccountId.Equals(payment.AccountId));
            existingPayment.Amount = payment.Amount;

            var existingAcccount = AccountList.FirstOrDefault(ac => ac.AccountId.Equals(payment.AccountId));
            existingAcccount.Balance = newBalance;
            return 1;
        }

        private static int InsertToList(Payment payment)
        {
            PaymentList.Add(payment);
            var existingAcccount = AccountList.FirstOrDefault(ac => ac.AccountId.Equals(payment.AccountId));
            existingAcccount.Balance += payment.Amount;
            return 1;
        }

        private static void SetPaymentList()
        {
            PaymentList = new List<Payment> {
                new Payment(TransactionId, 1, Origin.MASTER.ToString(), 1500M),
                new Payment(Guid.NewGuid(), 2, Origin.VISA.ToString(), 5000M),
                new Payment(Guid.NewGuid(), 3, Origin.VISA.ToString(), 3500M),
                new Payment(Guid.NewGuid(), 4, Origin.MASTER.ToString(), 7000M)
            };
        }
        private static void SetAccountList()
        {
            AccountList = new List<Account> {
                new Account(1, 15000M),
                new Account(2, 20000M),
                new Account(3, 40000M),
                new Account(4, 75000M)
            };
        }

        private static PaymentInfo SetPaymentInfo(long accountId, Origin origin, decimal amount, Guid transactionId) => new(
             transactionId,
             accountId,
             origin.ToString(),
             amount
         );
    }
}
