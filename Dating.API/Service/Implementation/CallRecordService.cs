using AutoMapper;
using Data.Repository.Interface;
using Dating.API.Service.Interface;
using Model.DTO;
using Model.Enitities;

namespace Dating.API.Service.Implementation
{
    public class CallRecordService : ICallRecordService
    {
        private readonly ICallRecordRepo _callRecord;
        private readonly IAccountRepo _accountRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CallRecordService> _logger;

        public CallRecordService(ICallRecordRepo callRecord, IMapper mapper, IAccountRepo accountRepo, ILogger<CallRecordService> logger)
        {
            _callRecord = callRecord;
            _mapper = mapper;
            _accountRepo = accountRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<string>> AddCallRecordAsync(AddCallRecord callRecord)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByIdAsync(callRecord.UserId);
                if (checkUserExist == null)
                {
                    response.ErrorMessages = new List<string>() { "User not found" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mappcallrecord = _mapper.Map<CallRecord>(callRecord);
                var addCall = await _callRecord.AddCallRecord(mappcallrecord);
                if (addCall == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in adding call record" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                checkUserExist.TimeAvailable -= callRecord.TimeUsed;
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
                response.Result = "Record added successfully";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in adding call record" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<PaginatedGenericDto<IEnumerable<UserTimeUsedDto>>>> AllCamgirlCallRecordsTimeUSed(int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedGenericDto<IEnumerable<UserTimeUsedDto>>>();
            try
            {
                var retrievePaginatedCall = await _callRecord.AllCamgirlCallRecordsTimeUSed(pageNumber, perPageSize);
                response.Result = retrievePaginatedCall;
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in getting cam girl call record" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<PaginatedGenericDto<IEnumerable<CallRecord>>>> GetAllCallRecords(int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedGenericDto<IEnumerable<CallRecord>>>();
            try
            {
                var retrievePaginatedCall = await _callRecord.GetAllCallRecords(pageNumber, perPageSize);
                response.Result = retrievePaginatedCall;
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in getting call record" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<IEnumerable<UserTimeUsedDto>>> GetTopUserWithUsedTime(int count)
        {
            var response = new ResponseDto<IEnumerable<UserTimeUsedDto>>();
            try
            {
                var retrieveUser = await _callRecord.GetUserWithMostUsedTime(count);
                response.DisplayMessage = "Success";
                response.Result = retrieveUser;
                response.StatusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in getting user with the most call" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<UserTimeUsedDto>> GetUserUsedTime(string userid)
        {
            var response = new ResponseDto<UserTimeUsedDto>();
            try
            {
                var retrieveUser = await _callRecord.GetUserWithTotalTimeUsedById(userid);
                if (retrieveUser == null)
                {
                    response.ErrorMessages = new List<string>() { "invalid User Id" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.DisplayMessage = "Success";
                response.Result = retrieveUser;
                response.StatusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in getting user time call" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<PaginatedGenericDto<IEnumerable<CallRecord>>>> GetUserCallRecords(string userid, int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedGenericDto<IEnumerable<CallRecord>>>();
            try
            {
                var retrievePaginatedCall = await _callRecord.GetUserCallRecords(userid, pageNumber, perPageSize);
                response.Result = retrievePaginatedCall;
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in getting call record" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
    }
}