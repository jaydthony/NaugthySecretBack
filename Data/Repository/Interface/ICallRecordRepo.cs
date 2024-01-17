using Model.DTO;
using Model.Enitities;

namespace Data.Repository.Interface
{
    public interface ICallRecordRepo
    {
        Task<bool> AddCallRecord(CallRecord callRecord);

        Task<PaginatedGenericDto<IEnumerable<CallRecord>>> GetAllCallRecords(int pageNumber, int perPageSize);

        Task<PaginatedGenericDto<IEnumerable<CallRecord>>> GetUserCallRecords(string userid, int pageNumber, int perPageSize);
        Task<UserTimeUsedDto> GetUserWithTotalTimeUsedById(string userid);

        Task<IEnumerable<UserTimeUsedDto>> GetUserWithMostUsedTime(int topcount);

        Task<PaginatedGenericDto<IEnumerable<UserTimeUsedDto>>> AllCamgirlCallRecordsTimeUSed(int pageNumber, int perPageSize);
    }
}