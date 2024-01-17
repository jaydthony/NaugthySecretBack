using Model.DTO;
using Model.Enitities;

namespace Data.Repository.Interface
{
    public interface ICamGirlRepo
    {
        Task<string> UpdateAvailabilityForCall(ApplicationUser user, bool state);
        Task<PaginatedUser> GetCamGirlsAvailableAsync(int pageNumber, int perPageSize);
        Task<ApplicationUser> FindCamGirlbyUserName(string userName);
        Task<bool> CheckInCamgirlRole(string username);
        Task<PaginatedUser> GetAllCamGirlsAsync(int pageNumber, int perPageSize);
        Task<FavouriteCamGirl> AddFavoriteCamgirl(FavouriteCamGirl user);
        Task<string> RemoveFavoriteCamgirl(FavouriteCamGirl user);
        Task<FavouriteCamGirl> favCamforUser(string Userid, string CamgirlUsername);
        Task<IEnumerable<FavList>> GetUserFavCamGirl(string Userid);
    }
}