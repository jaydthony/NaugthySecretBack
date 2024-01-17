using AutoMapper;
using Data.Repository.Interface;
using Dating.API.Service.Interface;
using Model.DTO;
using Model.Enitities;

namespace Dating.API.Service.Implementation
{
    public class TimerService : ITimerService
    {
        private readonly ITimerRepo _timerRepo;
        private readonly IAccountRepo _accountRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<TimerService> _logger;

        public TimerService(ITimerRepo timerRepo, IAccountRepo accountRepo, IMapper mapper, ILogger<TimerService> logger)
        {
            _timerRepo = timerRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto<string>> AddTimeAsync(AddTimeDto time)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByIdAsync(time.UserId);
                if (checkUserExist == null)
                {
                    response.ErrorMessages = new List<string>() { "User not found" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapTime = _mapper.Map<Timers>(time);
                var addTime = await _timerRepo.AddTimeAsync(mapTime);
                if (addTime == null)
                {
                    response.ErrorMessages = new List<string>() { "Error in adding time" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                checkUserExist.TimeAvailable += addTime.TimeBought;
                var updateUserTimer = await _accountRepo.UpdateUserInfo(checkUserExist);
                if (updateUserTimer == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in updating user time" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Successful";
                response.Result = "Time added successfully for user";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in adding Time for user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> DeleteTimeAsync(string timeid)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkTimeExist = await _timerRepo.getTimeAsync(timeid);
                if (checkTimeExist == null)
                {
                    response.ErrorMessages = new List<string>() { "Invalid time id" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }

                var deletTime = await _timerRepo.DeleteTimeAsync(checkTimeExist);
                if (deletTime == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in deleting call record" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var checkUserExist = await _accountRepo.FindUserByIdAsync(checkTimeExist.UserId);
                if (checkUserExist == null)
                {
                    response.ErrorMessages = new List<string>() { "User not found" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                checkUserExist.TimeAvailable -= checkTimeExist.TimeBought;
                var updateUserTimer = await _accountRepo.UpdateUserInfo(checkUserExist);
                if (updateUserTimer == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in updating user time" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Successful";
                response.Result = "Time deleted successfully for user";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in adding Time for user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<PaginatedTimeGenericDto<IEnumerable<UserTimeBuyDto>>>> GetAllTimeBuyAsync(int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedTimeGenericDto<IEnumerable<UserTimeBuyDto>>>();
            try
            {
                var retrievePaginatedTime = await _timerRepo.GetAllTimeBuyAsync(pageNumber, perPageSize);
                response.Result = retrievePaginatedTime;
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in retrieving all time record" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<PaginatedTimeGenericDto<IEnumerable<Timers>>>> GetUserTimerBuy(string userid, int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedTimeGenericDto<IEnumerable<Timers>>>();
            try
            {
                var retrievePaginatedTime = await _timerRepo.GetUserTimerBuy(userid, pageNumber, perPageSize);
                response.Result = retrievePaginatedTime;
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in retrieving user time record" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<UserTimeAvailableDto>> VerifyUserTimerAvalable(string userid)
        {
            var response = new ResponseDto<UserTimeAvailableDto>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByIdAsync(userid);
                if (checkUserExist == null)
                {
                    response.ErrorMessages = new List<string>() { "User not found" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapuser = _mapper.Map<UserTimeAvailableDto>(checkUserExist);
                if (checkUserExist.TimeAvailable > 0)
                {
                    mapuser.IsVideoCallStatus = true;
                    response.Result = mapuser;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.DisplayMessage = "Success";
                    return response;
                }
                mapuser.IsVideoCallStatus = false;
                response.Result = mapuser;
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in getting user video status availability" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<IEnumerable<UserTimeBuyDto>>> GetUserWithMostBuyTime(int topcount)
        {
            var response = new ResponseDto<IEnumerable<UserTimeBuyDto>>();
            try
            {
                var retrievePaginatedTime = await _timerRepo.GetUserWithMostBuyTime(topcount);
                response.Result = retrievePaginatedTime;
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in retrieving all time record" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
    }
}