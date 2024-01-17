using Model.DTO;

namespace Dating.API.Service.Interface
{
    public interface ICamGirlService
    {
        Task<ResponseDto<string>> UpdateUserCallStatus(string username, bool state);
        Task<ResponseDto<PaginatedUser>> GetCamGirlsAvailableAsync(int pageNumber, int perPageSize);

        Task<ResponseDto<string>> SetCamgirlAsTaken(string camGirlEmail);

        Task<ResponseDto<string>> SetCamgirlAsNotTaken(string camGirlUserName);

        Task<ResponseDto<PaginatedUser>> GetAllCamGirlsAsync(int pageNumber, int perPageSize);

        Task<ResponseDto<DisplayFindUserDTO>> FindCamGirlbyUserName(string userName);
        Task<ResponseDto<string>> RemoveCamgirlFromFav(string camUsername, string userid);
        Task<ResponseDto<string>> AddCamgirlToFav(string camUsername, string userid);
    }
}