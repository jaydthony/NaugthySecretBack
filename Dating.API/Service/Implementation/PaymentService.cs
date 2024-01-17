using Data.Repository.Implementation;
using Data.Repository.Interface;
using Dating.API.Service.Interface;
using Model.DTO;
using Model.Enitities;
using Model.Enum;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace Dating.API.Service.Implementation
{
    public class PaymentService : IPaymentService
    {
        private PayPalHttpClient _client;
        private readonly IPaymentRepo _paymentdb;
        private readonly ITimerService _timerService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;
        private readonly IAccountRepo _accountRepo;

        public PaymentService(PayPalHttpClient client, IPaymentRepo paymentdb, ILogger<PaymentService> logger, IAccountRepo accountRepo, ITimerService timerService, IConfiguration configuration)
        {
            _client = client;
            _paymentdb = paymentdb;
            _logger = logger;
            _accountRepo = accountRepo;
            _timerService = timerService;
            _configuration = configuration;
        }

        public async Task<ResponseDto<Dictionary<string, string>>> MakeOrder(string paymentType, string userid)
        {
            Amount amountsenum = paymentType == "Type1" ? Amount.Type1 : paymentType == "Type2" ? Amount.Type2 : paymentType == "Type3" ? Amount.Type3 : 0.00;
            decimal amount = (decimal)amountsenum;
            var description = $"Minute order purchase for {amount}$";
            var result = new ResponseDto<Dictionary<string, string>>();
            var payment = new Payments();
            try
            {
                var orderRequest = OrderRequest(amount.ToString("0.00"), "USD", description);
                var response = await _client.Execute(orderRequest);
               
                var order = response.Result<Order>();
                if(order.Status == "CREATED")
                {
                    payment.PaymentChannel = "PAYPAL";
                    payment.OrderReferenceId = order.Id;
                    payment.UserId = userid;
                    payment.Amount = amount.ToString("0.00");
                    payment.Description = description;
                    payment.IsActive = true;
                    
                   var addpaymenttoDb = await _paymentdb.AddPayments(payment);
                    if(addpaymenttoDb == true)
                    {
                        var data = order.Links.ToDictionary(links => links.Rel, links => links.Href);
                        result.Result = data;
                        result.DisplayMessage = "Payment successfully initialize";
                        result.StatusCode = 201;
                        return result;
                    }
                    result.ErrorMessages= new List<string>() { "Failed to make order for minute" };
                    result.DisplayMessage = $"Error";
                    result.StatusCode = StatusCodes.Status400BadRequest;
                    return result;
                }

                result.ErrorMessages = new List<string>() { "Failed to make order for minute" };
                result.DisplayMessage = $"Error";
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                result.ErrorMessages = new List<string>() { "Failed to make order for minute" };
                result.DisplayMessage = $"Error";
                result.StatusCode = StatusCodes.Status500InternalServerError;
                return result;
            }
        }

        private OrdersCreateRequest OrderRequest(string amount, string currency, string description)
        {
            var orderRequest = new OrdersCreateRequest()
            {
                Body = new OrderRequest()
                {
                    CheckoutPaymentIntent = "CAPTURE",
                    PurchaseUnits = new List<PurchaseUnitRequest>()
                    {
                        new PurchaseUnitRequest()
                        {
                            AmountWithBreakdown = new AmountWithBreakdown()
                            {
                                CurrencyCode = currency,
                                Value = amount
                            },
                            Description = description
                        }
                    },
                    ApplicationContext = new ApplicationContext()
                    {
                        PaymentMethod = new PaymentMethod
                        {
                            PayeePreferred = "IMMEDIATE_PAYMENT_REQUIRED",
                            PayerSelected = "PAYPAL"
                        },
                        LandingPage = "NO_PREFERENCE",
                        ReturnUrl = _configuration["WebHook:PayPalCallBackUrl"],
                        CancelUrl = _configuration["WebHook:PayPalCallBackUrl"]
                    }
                }
            };

            return orderRequest;
        }

        public async Task<ResponseDto<string>> ConfirmPayment(string token)
        {
            var response = new ResponseDto<string>();
            try
            {
                var capture = new OrdersGetRequest(token);

                capture.Body = new Order()
                {
                    Id = token
                };

                var result = await _client.Execute(capture);
                var order = result.Result<Order>();
                bool IsComplete = order.Status == "APPROVED";
                var retrieveOrder = await _paymentdb.GetPaymentById(order.Id);
                var expectedAmount = order.PurchaseUnits[0].AmountWithBreakdown.Value;
                if (retrieveOrder.IsActive == false)
                {
                    response.ErrorMessages = new List<string> { "Invalid Transaction" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }
                if (retrieveOrder == null || !IsComplete || retrieveOrder.Amount != expectedAmount)
                {
                    retrieveOrder.CompletePaymentTime = DateTime.UtcNow;
                    retrieveOrder.IsActive = false;
                    retrieveOrder.PaymentStatus = order.Status;
                    await _paymentdb.UpdatePayments(retrieveOrder);
                    response.ErrorMessages = new List<string> { "Invalid Transaction" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }
                
                retrieveOrder.IsActive = false;
                retrieveOrder.PaymentStatus = order.Status;
                retrieveOrder.CompletePaymentTime = DateTime.UtcNow;
                var updatetransaction = await _paymentdb.UpdatePayments(retrieveOrder);
                if (!updatetransaction)
                {
                    _logger.LogError("Error update payment", "Error in updating payment transaction");
                    response.ErrorMessages = new List<string> { "Error in completing transaction" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    return response;
                }
                var time = 0;
                if(retrieveOrder.Amount == ((decimal)Amount.Type1).ToString("0.00"))
                {
                    time = 10;
                }else if (retrieveOrder.Amount == ((decimal)Amount.Type2).ToString("0.00"))
                {
                    time = 30;
                }else if (retrieveOrder.Amount == ((decimal)Amount.Type3).ToString("0.00"))
                {
                    time = 60;
                }
                var addtimeDto = new AddTimeDto()
                {
                    TimeBought = time,
                    UserId = retrieveOrder.UserId
                };
                var addUserTime = await _timerService.AddTimeAsync(addtimeDto);
                if(addUserTime.StatusCode != 200)
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in validating transaction" };
                response.DisplayMessage = $"Error";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
        }

        public async Task<ResponseDto<PaginatedPaymentInfo>> RetrieveUserAllPaymentAsync(string userid, int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedPaymentInfo>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByIdAsync(userid);
                if (checkUserExist == null)
                {
                    response.ErrorMessages = new List<string>() { "Invalid user" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var retrieveUserPayment = await _paymentdb.RetrieveUserAllPaymentAsync(userid,pageNumber,perPageSize);
                response.DisplayMessage = "Successful";
                response.Result = retrieveUserPayment;
                response.StatusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Failed to retrieve user payment" };
                response.DisplayMessage = $"Error";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }
            
        }
        public async Task<ResponseDto<PaginatedPaymentInfo>> RetrieveAllPaymentAsync(int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedPaymentInfo>();
            try
            {
                
                var retrieveUserPayment = await _paymentdb.RetrieveAllPaymentAsync(pageNumber, perPageSize);
                response.DisplayMessage = "Successful";
                response.Result = retrieveUserPayment;
                response.StatusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Failed to retrieve all payment" };
                response.DisplayMessage = $"Error";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }

        }
    }
}