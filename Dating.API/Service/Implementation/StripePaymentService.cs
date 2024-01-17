using Data.Repository.Interface;
using Dating.API.Service.Interface;
using Model.DTO;
using Model.Enitities;
using Model.Enum;
using Stripe;
using Stripe.Checkout;

namespace Dating.API.Service.Implementation
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly IPaymentRepo _paymentdb;
        private readonly ITimerService _timerService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StripePaymentService> _logger;
        private readonly IAccountRepo _accountRepo;
        public StripePaymentService(IConfiguration configuration, ITimerService timerService = null, IPaymentRepo paymentdb = null, ILogger<StripePaymentService> logger = null, IAccountRepo accountRepo = null)
        {
            _configuration = configuration;
            _timerService = timerService;
            _paymentdb = paymentdb;
            _logger = logger;
            _accountRepo = accountRepo;
        }

        public async Task<ResponseDto<string>> IntializeStripepayment(string paymentType, string userid)
        {
            Amount amountsEnum = paymentType == "Type1" ? Amount.Type1 : paymentType == "Type2" ? Amount.Type2 : paymentType == "Type3" ? Amount.Type3 : 0.00;
            long amount = (long)amountsEnum;
            var description = $"Minute order purchase for {amount}$";
            StripeConfiguration.ApiKey = _configuration["StripeKey:SecretKey"];
            var stripePayment = new Payments();
            var response = new ResponseDto<string>();
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = $"https://naughty-secret.vercel.app/confirm_payment?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = "https://naughty-secret.vercel.app/",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                };

                var sessionListItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = amount * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = description
                        }
                    },
                    Quantity = 1
                };

                options.LineItems.Add(sessionListItem);

                var service = new SessionService();
                Session session = await service.CreateAsync(options);
                if (session.Status != "open")
                {
                    response.ErrorMessages = new List<string>() { "Failed to make order for minute" };
                    response.DisplayMessage = $"Error";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }
                stripePayment.PaymentChannel = "STRIPE";
                stripePayment.OrderReferenceId = session.Id;
                stripePayment.Amount = amount.ToString("0.00");
                stripePayment.IsActive = true;
                stripePayment.Description = description;
                stripePayment.UserId = userid;
                var addpaymenttoDb = await _paymentdb.AddPayments(stripePayment);
                if (addpaymenttoDb == false)
                {
                    response.ErrorMessages = new List<string>() { "Failed to make order for minute" };
                    response.DisplayMessage = $"Error";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;

                }
                response.Result = session.Url;
                response.StatusCode = 200;
                response.DisplayMessage = "Successfully created stripe payment";
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Failed to make order for minute" };
                response.DisplayMessage = $"Error";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }
        public async Task<ResponseDto<string>> confirmStripepayment(string session_id)
        {
            var response = new ResponseDto<string>();
            StripeConfiguration.ApiKey = _configuration["StripeKey:SecretKey"];
            try
            {
                var retrievePayment = await _paymentdb.GetPaymentById(session_id);
                if (retrievePayment == null)
                {
                    response.ErrorMessages = new List<string>() {
                        "Invalid order" };
                    response.DisplayMessage = $"Error";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }
                var service = new SessionService();
                var session = await service.GetAsync(session_id);
                if (retrievePayment.IsActive == false)
                {
                    response.ErrorMessages = new List<string> { "Invalid Transaction" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }
                if (session.PaymentStatus == "paid")
                {
                    retrievePayment.IsActive = false;
                    retrievePayment.PaymentStatus = session.PaymentStatus;
                    retrievePayment.CompletePaymentTime = DateTime.UtcNow;
                    var updatetransaction = await _paymentdb.UpdatePayments(retrievePayment);
                    if (!updatetransaction)
                    {
                        _logger.LogError("Error update payment", "Error in updating payment transaction");
                        response.ErrorMessages = new List<string> { "Error in completing transaction" };
                        response.DisplayMessage = "Error";
                        response.StatusCode = StatusCodes.Status501NotImplemented;
                        return response;
                    }
                    var time = 0;
                    if (retrievePayment.Amount == ((decimal)Amount.Type1).ToString("0.00"))
                    {
                        time = 10;
                    }
                    else if (retrievePayment.Amount == ((decimal)Amount.Type2).ToString("0.00"))
                    {
                        time = 30;
                    }
                    else if (retrievePayment.Amount == ((decimal)Amount.Type3).ToString("0.00"))
                    {
                        time = 60;
                    }
                    var addtimeDto = new AddTimeDto()
                    {
                        TimeBought = time,
                        UserId = retrievePayment.UserId
                    };
                    var addUserTime = await _timerService.AddTimeAsync(addtimeDto);
                    if (addUserTime.StatusCode != 200)
                    {
                        _logger.LogError("Error time", "Error in adding time for user");
                        response.ErrorMessages = new List<string> { "Error in updating user timer" };
                        response.DisplayMessage = addUserTime.DisplayMessage;
                        response.StatusCode = addUserTime.StatusCode;
                        return response;
                    }
                    response.StatusCode = StatusCodes.Status200OK;
                    response.DisplayMessage = "Successful";
                    response.Result = "Payment Successfully completed";
                    return response;

                }
                retrievePayment.CompletePaymentTime = DateTime.UtcNow;
                retrievePayment.IsActive = false;
                retrievePayment.PaymentStatus = session.PaymentStatus;
                await _paymentdb.UpdatePayments(retrievePayment);
                response.ErrorMessages = new List<string> { "Invalid Transaction" };
                response.DisplayMessage = "Error";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in validating transaction" };
                response.DisplayMessage = $"Error";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }




        }
    }
}
