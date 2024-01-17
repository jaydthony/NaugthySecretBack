using Model.DTO;
using Model.Enitities;
using PayPalCheckoutSdk.Orders;

namespace Data.Repository.Interface
{
    public interface IPaymentRepo
    {
        Task<Payments> GetPaymentById(string OrderReferenceId);
        Task<bool> AddPayments(Payments payments);
        Task<bool> UpdatePayments(Payments payments);
        Task<PaginatedPaymentInfo> RetrieveAllPaymentAsync(int pageNumber, int perPageSize);
        Task<PaginatedPaymentInfo> RetrieveUserAllPaymentAsync(string userid, int pageNumber, int perPageSize);
    }
}