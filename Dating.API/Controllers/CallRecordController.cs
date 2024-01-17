using Dating.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Model.DTO;
using Model.Enitities;

namespace Dating.API.Controllers
{
    [Route("api/call/record")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class CallRecordController : ControllerBase
    {
        private readonly ICallRecordService _callRecordService;
        private IMemoryCache _cache;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public CallRecordController(ICallRecordService callRecordService, IMemoryCache cache)
        {
            _callRecordService = callRecordService;
            _cache = cache;
        }

        [AllowAnonymous]
        [HttpPost("add")]
        public async Task<IActionResult> AddCallRecord(AddCallRecord addCallRecord)
        {
            var result = await _callRecordService.AddCallRecordAsync(addCallRecord);
            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN")]
        [HttpGet("mostcall/{count}")]
        public async Task<IActionResult> GetTopUserCall(int count)
        {
            var cacheKey = $"GetTopUserCall_{count}";
            if (_cache.TryGetValue(cacheKey, out ResponseDto<IEnumerable<UserTimeUsedDto>> result))
            {
                return Ok(result);
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(cacheKey, out result))
                    {
                        return Ok(result);
                    }
                    else
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(10))
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1024);
                        result = await _callRecordService.GetTopUserWithUsedTime(count);
                        _cache.Set(cacheKey, result, cacheEntryOptions);
                        if (result.StatusCode == 200)
                        {
                            return Ok(result);
                        }
                        else if (result.StatusCode == 404)
                        {
                            return NotFound(result);
                        }
                        else
                        {
                            return BadRequest(result);
                        }
                    }
                }
                finally { semaphore.Release(); }
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN")]
        [HttpGet("user/{userid}")]
        public async Task<IActionResult> GetUserUsedTime(string userid)
        {
            var result = await _callRecordService.GetUserUsedTime(userid);

            if (result.StatusCode == 200)
            {
                return Ok(result);
            }
            else if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN")]

        [HttpGet("all/{pagesize}/{pagenumber}")]
        public async Task<IActionResult> GetAllCall(int pagesize, int pagenumber)
        {
            var cacheKey = $"GetAllCall_{pagenumber}_{pagesize}";
            if (_cache.TryGetValue(cacheKey, out ResponseDto<PaginatedGenericDto<IEnumerable<CallRecord>>> result))
            {
                return Ok(result);
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(cacheKey, out result))
                    {
                        return Ok(result);
                    }
                    else
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1024);
                        result = await _callRecordService.GetAllCallRecords(pagenumber, pagesize);
                        _cache.Set(cacheKey, result, cacheEntryOptions);
                        if (result.StatusCode == 200)
                        {
                            return Ok(result);
                        }
                        else if (result.StatusCode == 404)
                        {
                            return NotFound(result);
                        }
                        else
                        {
                            return BadRequest(result);
                        }
                    }
                }
                finally { semaphore.Release(); }
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN")]

        [HttpGet("camgirl/all/{pagesize}/{pagenumber}")]
        public async Task<IActionResult> GetAllCamGirlIndividualCallRecordPaginated(int pagesize, int pagenumber)
        {
            var cacheKey = $"GetAllCamGirlIndividualCallRecordPaginated_{pagesize}_{pagenumber}";
            if (_cache.TryGetValue(cacheKey, out ResponseDto<PaginatedGenericDto<IEnumerable<UserTimeUsedDto>>> result))
            {
                return Ok(result);
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(cacheKey, out result))
                    {
                        return Ok(result);
                    }
                    else
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1024);
                        result = await _callRecordService.AllCamgirlCallRecordsTimeUSed(pagenumber, pagesize);
                        _cache.Set(cacheKey, result, cacheEntryOptions);
                        if (result.StatusCode == 200)
                        {
                            return Ok(result);
                        }
                        else if (result.StatusCode == 404)
                        {
                            return NotFound(result);
                        }
                        else
                        {
                            return BadRequest(result);
                        }
                    }
                }
                finally { semaphore.Release(); }
            }
        }

        [HttpGet("user/{userid}/{pagesize}/{pagenumber}")]
        public async Task<IActionResult> GetUserCall(string userid, int pagesize, int pagenumber)
        {
            var cacheKey = $"GetUserCall_{userid}_{pagesize}_{pagenumber}";
            if (_cache.TryGetValue(cacheKey, out ResponseDto<PaginatedGenericDto<IEnumerable<CallRecord>>> result))
            {
                return Ok(result);
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(cacheKey, out result))
                    {
                        return Ok(result);
                    }
                    else
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1024);
                        result = await _callRecordService.GetUserCallRecords(userid, pagenumber, pagesize);
                        _cache.Set(cacheKey, result, cacheEntryOptions);
                        if (result.StatusCode == 200)
                        {
                            return Ok(result);
                        }
                        else if (result.StatusCode == 404)
                        {
                            return NotFound(result);
                        }
                        else
                        {
                            return BadRequest(result);
                        }
                    }
                }
                finally { semaphore.Release(); }
            }
        }
    }
}