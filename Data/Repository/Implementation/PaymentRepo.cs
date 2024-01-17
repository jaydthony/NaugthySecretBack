using Data.Context;
using Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.DTO;
using Model.Enitities;

namespace Data.Repository.Implementation
{
    public class PaymentRepo : IPaymentRepo
    {
        private DatingSiteContext _context;

        public PaymentRepo(DatingSiteContext context)
        {
            _context = context;
        }

        public async Task<Payments> GetPaymentById(string OrderReferenceId)
        {
            return await _context.Payments.FirstOrDefaultAsync(x => x.OrderReferenceId == OrderReferenceId);
        }
        public async Task<PaginatedPaymentInfo> RetrieveAllPaymentAsync(int pageNumber, int perPageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            perPageSize = perPageSize < 1 ? 5 : perPageSize;
            var payment = _context.Payments.Include(u => u.User).Select(p => new PaymentWithUserInfo
            {
                Id = p.Id,
                Amount = p.Amount,
                OrderReferenceId = p.OrderReferenceId,
                Description = p.Description,
                PaymentType = p.PaymentType,
                CreatedPaymentTime = p.CreatedPaymentTime,
                CompletePaymentTime = p.CompletePaymentTime,
                IsActive = p.IsActive,
                UserId = p.UserId,
                PaymentStatus = p.PaymentStatus,
                UserName = p.User.UserName,
                FirstName = p.User.FirstName,
                Email = p.User.Email
            });
            var paginatedPayment = await payment
                .Skip((pageNumber - 1) * perPageSize)
                .Take(perPageSize)
                .ToListAsync();
            var totalCount = await payment.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / perPageSize);
            var result = new PaginatedPaymentInfo
            {
                CurrentPage = pageNumber,
                PageSize = perPageSize,
                TotalPages = totalPages,
                Payments = paginatedPayment,
            };
            return result;

            
        }
        public async Task<PaginatedPaymentInfo> RetrieveUserAllPaymentAsync(string userid, int pageNumber, int perPageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            perPageSize = perPageSize < 1 ? 5 : perPageSize;
            var paymentsWithUserInfo = _context.Payments
                .Include(p => p.User)
                .Where(p => p.UserId == userid)
                .Select(p => new PaymentWithUserInfo
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    OrderReferenceId = p.OrderReferenceId,
                    Description = p.Description,
                    UserId = p.UserId,
                    PaymentType = p.PaymentType,
                    CreatedPaymentTime = p.CreatedPaymentTime,
                    CompletePaymentTime = p.CompletePaymentTime,
                    IsActive = p.IsActive,
                    PaymentStatus = p.PaymentStatus,
                    UserName = p.User.UserName,
                    FirstName = p.User.FirstName,
                    Email = p.User.Email
                });
            var paginatedPayment = await paymentsWithUserInfo
                .Skip((pageNumber - 1) * perPageSize)
                .Take(perPageSize)
                .ToListAsync();
            var totalCount = await paymentsWithUserInfo.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / perPageSize);
            var result = new PaginatedPaymentInfo
            {
                CurrentPage = pageNumber,
                PageSize = perPageSize,
                TotalPages = totalPages,
                Payments = paginatedPayment,
            };
            return result;

            
        }


        public async Task<bool> AddPayments(Payments payments)
        {
            await _context.Payments.AddAsync(payments);
            if ( await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdatePayments(Payments payments)
        {
             _context.Payments.Update(payments);
            if ( await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
    }
}