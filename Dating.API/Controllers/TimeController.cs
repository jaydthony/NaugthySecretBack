using Dating.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Model.DTO;
using Model.Enitities;
using System.IdentityModel.Tokens.Jwt;

namespace Dating.API.Controllers
{
    [Route("api/time")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        private readonly ITimerService _timerService;
        private IMemoryCache _cache;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public TimeController(ITimerService timerService, IMemoryCache cache)
        {
            _timerService = timerService;
            _cache = cache;
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN")]
        [HttpPost("add")]
        public async Task<IActionResult> AddTime(AddTimeDto time)
        {
            var result = await _timerService.AddTimeAsync(time);
            try
            {
                await semaphore.WaitAsync();
                var cacheKey = $"VerifyUserTimeAvailableAsync_{time.UserId}";
                _cache.Remove(cacheKey);
                for (int pageNumber = 1; pageNumber <= 500; pageNumber++)
                {
                    for (int pageSize = 1; pageSize <= 500; pageSize++)
                    {
                        cacheKey = $"GetTopUserWithMinute_{pageNumber}";
                        _cache.Remove(cacheKey);
                        cacheKey = $"GetallUserTimeRecord_{pageNumber}_{pageSize}";
                        _cache.Remove(cacheKey);
                        cacheKey = $"GetUserTime_{time.UserId}_{pageSize}_{pageNumber}";
                        _cache.Remove(cacheKey);
                    }
                }
            }
            finally { semaphore.Release(); }
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

        [HttpGet("most/{count}")]
        public async Task<IActionResult> GetTopUserWithMinute(int count)
        {
            
            var cacheKey = $"GetTopUserWithMinute_{count}";
            if (_cache.TryGetValue(cacheKey, out ResponseDto<IEnumerable<UserTimeBuyDto>> result))
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
                        .SetSlidingExpiration(TimeSpan.FromSeconds(120))
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1024);
                        result = await _timerService.GetUserWithMostBuyTime(count);
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

        [HttpGet("all/{pagesize}/{pagenumber}")]
        public async Task<IActionResult> GetallUserTimeRecord(int pagesize, int pagenumber)
        {
            var cacheKey = $"GetallUserTimeRecord_{pagesize}_{pagenumber}";
            if (_cache.TryGetValue(cacheKey, out ResponseDto<PaginatedTimeGenericDto<IEnumerable<UserTimeBuyDto>>> result))
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
                        .SetSlidingExpiration(TimeSpan.FromSeconds(120))
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1024);
                        result = await _timerService.GetAllTimeBuyAsync(pagenumber, pagesize);
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
        public async Task<IActionResult> GetUserTime(string userid, int pagesize, int pagenumber)
        {
            var cacheKey = $"GetUserTime_{userid}_{pagesize}_{pagenumber}";
            if (_cache.TryGetValue(cacheKey, out ResponseDto<PaginatedTimeGenericDto<IEnumerable<Timers>>> result))
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
                        .SetSlidingExpiration(TimeSpan.FromSeconds(120))
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1024);
                        result = await _timerService.GetUserTimerBuy(userid, pagenumber, pagesize);
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

        
        [HttpGet("verify/status/{user_id}")]
        public async Task<IActionResult> VerifyUserTimeAvailableAsync(string? user_id)

        {
            if (user_id == null)
            {
                user_id = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            }
            var cacheKey = $"VerifyUserTimeAvailableAsync_{user_id}";
            if (_cache.TryGetValue(cacheKey, out ResponseDto<UserTimeAvailableDto> result))
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
                        result = await _timerService.VerifyUserTimerAvalable(user_id);
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
        [HttpDelete("delete/{user_id}/{time_id}")]
        public async Task<IActionResult> DeleteTime(string time_id, string user_id)
        {
            var result = await _timerService.DeleteTimeAsync(time_id);
            try
            {
                await semaphore.WaitAsync();
                var cacheKey = $"VerifyUserTimeAvailableAsync_{user_id}";
                _cache.Remove(cacheKey);
                for (int pageNumber = 1; pageNumber <= 500; pageNumber++)
                {
                    for (int pageSize = 1; pageSize <= 500; pageSize++)
                    {
                        cacheKey = $"GetTopUserWithMinute_{pageNumber}";
                        _cache.Remove(cacheKey);
                        cacheKey = $"GetallUserTimeRecord_{pageNumber}_{pageSize}";
                        _cache.Remove(cacheKey);
                        cacheKey = $"GetUserTime_{user_id}_{pageSize}_{pageNumber}";
                        _cache.Remove(cacheKey);
                    }
                }
            }
            finally { semaphore.Release(); }

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
}