using Model.DTO;

namespace Dating.API.Service.Interface
{
    public interface IStripePaymentService
    {
        Task<ResponseDto<string>> IntializeStripepayment(string paymentType, string userid);
        Task<ResponseDto<string>> confirmStripepayment(string session_id);
    }
}
