using AutoMapper;
using Data.Repository.Interface;
using Dating.API.Service.Interface;
using Model.DTO;
using Model.Enitities;

namespace Dating.API.Service.Implementation
{
    public class CamGirlService : ICamGirlService
    {
        private readonly ICamGirlRepo _camGirlRepo;
        private readonly ILogger<CamGirlService> _logger;
        private readonly IAccountRepo _accountRepo;
        private readonly IMapper _mapper;

        public CamGirlService(ICamGirlRepo camGirlRepo, ILogger<CamGirlService> logger, IAccountRepo accountRepo, IMapper mapper)
        {
            _camGirlRepo = camGirlRepo;
            _logger = logger;
            _accountRepo = accountRepo;
            _mapper = mapper;
        }
        public async Task<ResponseDto<string>> UpdateUserCallStatus (string username, bool state)
        {
            var response = new ResponseDto<string>();
            try
            {
                var retrieveCamgirl = await _accountRepo.FIndUserByUserName(username);
                if (retrieveCamgirl == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the username provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var updateStatus = await _camGirlRepo.UpdateAvailabilityForCall(retrieveCamgirl, state);
                if(updateStatus == null)
                {
                    response.ErrorMessages = new List<string>() { "Update Status Fail" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = $"Cam girl call status change successfully to {state}";
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in updating user call status" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> AddCamgirlToFav(string camUsername, string userid)
        {
            var response = new ResponseDto<string>();
            try
            {
                var retrievefav = await _camGirlRepo.favCamforUser(userid, camUsername);

                if (retrievefav != null)
                {
                    response.ErrorMessages = new List<string>() { "Cam Girl already add to ur fav list" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var fav = new FavouriteCamGirl()
                {
                    CamgirlUserName = camUsername,
                    UserId = userid
                };
                var add = await _camGirlRepo.AddFavoriteCamgirl(fav);
                if(add == null)
                {
                    response.ErrorMessages = new List<string>() { "Error in adding cam giel to fav list" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
               response.Result = $"{camUsername} added successfully to favorite list";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in adding cam girl to fav list" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> RemoveCamgirlFromFav(string camUsername, string userid)
        {
            var response = new ResponseDto<string>();
            try
            {
                var retrievefav = await _camGirlRepo.favCamforUser(userid, camUsername);
               
                if (retrievefav == null)
                {
                    response.ErrorMessages = new List<string>() { "Cam Girl not in fav list" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var remove = await _camGirlRepo.RemoveFavoriteCamgirl(retrievefav);
                if(remove == null)
                {
                    response.ErrorMessages = new List<string>() { "Cam girl not remove successfully" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = $"{camUsername} remove successfully from favorite list";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in adding cam girl to fav list" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<PaginatedUser>> GetCamGirlsAvailableAsync(int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedUser>();
            try
            {
                var getCAMGIRL = await _camGirlRepo.GetCamGirlsAvailableAsync(pageNumber, perPageSize);
                if (!getCAMGIRL.User.Any())
                {
                    response.ErrorMessages = new List<string>() { "Cam girl not available with the preference provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = getCAMGIRL;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() {"Error in getting camgirl available" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<PaginatedUser>> GetAllCamGirlsAsync(int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedUser>();
            try
            {
                var getCAMGIRL = await _camGirlRepo.GetAllCamGirlsAsync(pageNumber, perPageSize);
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = getCAMGIRL;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in retrieving all camgirl" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> SetCamgirlAsTaken(string camGirlEmail)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(camGirlEmail);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                if (findUser.UserIsTaken == true)
                {
                    response.ErrorMessages = new List<string>() { "Cam girl is taken, cam girl not available" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var setCamGirlHasTaken = await _accountRepo.UpdateUserStatusTaken(findUser);
                if (setCamGirlHasTaken == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in matching cam girl" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Cam girl successfully matched";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in Connecting Cam Girl" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> SetCamgirlAsNotTaken(string camGirlUserName)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _camGirlRepo.FindCamGirlbyUserName(camGirlUserName); ;
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no cam girl with the username provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }

                var setCamGirlHasTaken = await _accountRepo.UpdateUserStatusNotTaken(findUser);
                if (setCamGirlHasTaken == false)
                {
                    response.ErrorMessages = new List<string>() { "Error dismatching cam girl" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Cam girl successfully dismatched";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in disconnecting Cam Girl" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<DisplayFindUserDTO>> FindCamGirlbyUserName(string userName)
        {
            var response = new ResponseDto<DisplayFindUserDTO>();
            try
            {
                var findCamgirl = await _camGirlRepo.FindCamGirlbyUserName(userName);
                if (findCamgirl == null)
                {
                    response.ErrorMessages = new List<string>() { "Please check back, Cam girl not available" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var checkInCamgirlRole = await _camGirlRepo.CheckInCamgirlRole(userName);
                if (checkInCamgirlRole == false)
                {
                    response.ErrorMessages = new List<string>() { "username provided is not a camgirl" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapCamGirl = _mapper.Map<DisplayFindUserDTO>(findCamgirl);
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = mapCamGirl;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in getting camgirl by its username" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
    }
}