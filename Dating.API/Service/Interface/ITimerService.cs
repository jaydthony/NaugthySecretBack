using Model.DTO;
using Model.Enitities;

namespace Dating.API.Service.Interface
{
    public interface ITimerService
    {
        Task<ResponseDto<string>> AddTimeAsync(AddTimeDto time);

        Task<ResponseDto<string>> DeleteTimeAsync(string timeid);

        Task<ResponseDto<PaginatedTimeGenericDto<IEnumerable<Timers>>>> GetUserTimerBuy(string userid, int pageNumber, int perPageSize);

        Task<ResponseDto<PaginatedTimeGenericDto<IEnumerable<UserTimeBuyDto>>>> GetAllTimeBuyAsync(int pageNumber, int perPageSize);

        Task<ResponseDto<UserTimeAvailableDto>> VerifyUserTimerAvalable(string userid);

        Task<ResponseDto<IEnumerable<UserTimeBuyDto>>> GetUserWithMostBuyTime(int topcount);
    }
}