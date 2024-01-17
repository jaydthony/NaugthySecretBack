using Data.Repository.Interface;
using Dating.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Enum;

namespace Dating.API.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _pay;
        private readonly IPaymentRepo _paydb;
        private readonly IStripePaymentService _stripePaymentService;

        public PaymentsController(IPaymentService pay, IPaymentRepo paydb, IStripePaymentService stripePaymentService)
        {
            _pay = pay;
            _paydb = paydb;
            _stripePaymentService = stripePaymentService;
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("create/buy_minute")]
        public async Task<IActionResult> GetOrder(string paymentType, string user_id)
        {

            var result = await _pay.MakeOrder(paymentType, user_id);

            if (result.StatusCode == 200 || result.StatusCode == 201)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("create/stripe")]
        public async Task<IActionResult> createStripePayment(string userid, string paymentType)
        {

            var result = await  _stripePaymentService.IntializeStripepayment(paymentType,userid);

            if (result.StatusCode == 200 || result.StatusCode == 201)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }


        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("webhook/confirm-payment/stripe")]
        public async Task<IActionResult> ConfirmStripePayment(string session_id)
        {
            var result = await _stripePaymentService.confirmStripepayment(session_id);

            if (result.StatusCode == 200 || result.StatusCode == 201)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("webhook/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(string token)
        {
            var result = await _pay.ConfirmPayment(token);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles ="ADMIN")]
        [HttpGet("get-payment-by-orderId")]
        public async Task<IActionResult> GetPaymentByOrderId(string OrderId)
        {
            var data = await _paydb.GetPaymentById(OrderId);
            return Ok(data);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("user/all/{user_id}/{perPageSize}/{pageNumber}")]
        public async Task<IActionResult> UserPaymentHistory(string user_id, int pageNumber, int perPageSize)
        {
            var result = await _pay.RetrieveUserAllPaymentAsync(user_id, pageNumber, perPageSize);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles ="ADMIN")]
        [HttpGet("user/all/{perPageSize}/{pageNumber}")]
        public async Task<IActionResult> AllPaymentHistory(int pageNumber, int perPageSize)
        {
            var result = await _pay.RetrieveAllPaymentAsync(pageNumber, perPageSize);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}