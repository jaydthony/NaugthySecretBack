using Model.DTO;
using Model.Enitities;

namespace Data.Repository.Interface
{
    public interface ITimerRepo
    {
        Task<Timers> AddTimeAsync(Timers time);

        Task<bool> DeleteTimeAsync(Timers time);

        Task<Timers> getTimeAsync(string timeid);

        Task<PaginatedTimeGenericDto<IEnumerable<Timers>>> GetUserTimerBuy(string userid, int pageNumber, int perPageSize);

        Task<PaginatedTimeGenericDto<IEnumerable<UserTimeBuyDto>>> GetAllTimeBuyAsync(int pageNumber, int perPageSize);

        Task<IEnumerable<UserTimeBuyDto>> GetUserWithMostBuyTime(int topcount);
    }
}