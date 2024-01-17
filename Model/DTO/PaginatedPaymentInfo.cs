using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class PaginatedPaymentInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<PaymentWithUserInfo> Payments { get; set; }
    }
}
