﻿using Model.DTO;

namespace Dating.API.Service.Interface
{
    public interface IPaymentService
    {
        Task<ResponseDto<Dictionary<string, string>>> MakeOrder(string paymentType, string userid);
        Task<ResponseDto<string>> ConfirmPayment(string token);
        Task<ResponseDto<PaginatedPaymentInfo>> RetrieveUserAllPaymentAsync(string userid, int pageNumber, int perPageSize);
        Task<ResponseDto<PaginatedPaymentInfo>> RetrieveAllPaymentAsync(int pageNumber, int perPageSize);
    }
}