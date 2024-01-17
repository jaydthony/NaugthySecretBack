using Model.DTO;
using Model.Enitities;

namespace Dating.API.Service.Interface
{
    public interface ICallRecordService
    {
        Task<ResponseDto<string>> AddCallRecordAsync(AddCallRecord callRecord);
        Task<ResponseDto<UserTimeUsedDto>> GetUserUsedTime(string userid);

        Task<ResponseDto<IEnumerable<UserTimeUsedDto>>> GetTopUserWithUsedTime(int count);

        Task<ResponseDto<PaginatedGenericDto<IEnumerable<UserTimeUsedDto>>>> AllCamgirlCallRecordsTimeUSed(int pageNumber, int perPageSize);

        Task<ResponseDto<PaginatedGenericDto<IEnumerable<CallRecord>>>> GetAllCallRecords(int pageNumber, int perPageSize);

        Task<ResponseDto<PaginatedGenericDto<IEnumerable<CallRecord>>>> GetUserCallRecords(string userid, int pageNumber, int perPageSize);
    }
}