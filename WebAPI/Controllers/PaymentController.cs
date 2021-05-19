using System.Net;
using System.Threading.Tasks;

using Helpers.Enums;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Service.Models;
using Service.Services
    ;
using WebAPI.Mappers;
using WebAPI.Messages;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class PaymentController : ControllerBase
    {

        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentMessageMapper _paymentMessageMapper;

        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService, IPaymentMessageMapper paymentMessageMapper)
        {
            _logger = logger;
            _paymentService = paymentService;
            _paymentMessageMapper = paymentMessageMapper;
        }

        [HttpPost]
        public async Task<ObjectResult> PostPayment(PaymentMessage message)
        {
            var paymentInfo = _paymentMessageMapper.MapPaymentMessage(message);

            _logger.LogInformation($"Making payment of £{message.Amount} for account: {message.AccountId}");

            PaymentResult paymentResult;

            if (message.MessageType == MessageType.PAYMENT.ToString())
            {
                paymentResult = await _paymentService.MakePayment(paymentInfo).ConfigureAwait(false);
            }
            else
            {
                paymentResult = await _paymentService.MakeAdjustment(paymentInfo).ConfigureAwait(false);
            }

            return paymentResult.IsSuccess ? StatusCode((int)HttpStatusCode.OK, paymentResult.Message)
                                           : StatusCode((int)HttpStatusCode.InternalServerError, paymentResult.Message);
        }
    }
}
