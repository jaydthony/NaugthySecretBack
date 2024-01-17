using Dating.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Model.DTO;
using Model.Enum;

namespace Dating.API.Controllers
{
    [Route("api/camgirl")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class CamGirlController : ControllerBase
    {
        private readonly ICamGirlService _camGirlService;
        private readonly IEmailServices _emailServices;
        private IMemoryCache _cache;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public CamGirlController(ICamGirlService camGirlService, IEmailServices emailServices, IMemoryCache cache)
        {
            _camGirlService = camGirlService;
            _emailServices = emailServices;
            _cache = cache;
        }

        [AllowAnonymous]
        [HttpGet("available/{page_number}")]
        public async Task<IActionResult> GetCamGirlAvailableAsync(int page_number)
        {
           var result = await _camGirlService.GetCamGirlsAvailableAsync(page_number, 5);
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
        [AllowAnonymous]
        [HttpPost("fav/add")]
        public async Task<IActionResult> AddCamGirlFav(FavDto favDto)
        {
            var result = await _camGirlService.AddCamgirlToFav(favDto.CamGirlUserName, favDto.Userid);
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
        [AllowAnonymous]
        [HttpDelete("fav/remove")]
        public async Task<IActionResult> RemoveCamGirlFav(FavDto favDto)
        {
            var result = await _camGirlService.RemoveCamgirlFromFav(favDto.CamGirlUserName, favDto.Userid);
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
        [HttpGet("all/{per_page_size}/{page_number}")]
        public async Task<IActionResult> GetAllCamGirlAsync(int page_number, int per_page_size)
        {
            var result = await _camGirlService.GetAllCamGirlsAsync(page_number, per_page_size);
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
        [AllowAnonymous]
        [HttpPost("match")]
        public async Task<IActionResult> MatchCamgirl(MatchGirlDto matchGirl)
        {
            var result = await _camGirlService.SetCamgirlAsTaken(matchGirl.Email);
            if (result.StatusCode == 200)
            {
                var message = new Message(new string[] { matchGirl.Email }, "Join Room Link", $"<p>Click <a href={matchGirl.RoomLink}>here</a> to join the client</p>");
                _emailServices.SendEmail(message);
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
        [AllowAnonymous]
        [HttpPost("unmatch/{camGirlUserName}")]
        public async Task<IActionResult> UnMatchCamgirl(string camGirlUserName)
        {
            var result = await _camGirlService.SetCamgirlAsNotTaken(camGirlUserName);
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
        [AllowAnonymous]
        [HttpPost("status")]
        public async Task<IActionResult> UnMatchCamgirl( ChangeCallStatusDto statusDto)
        {
            var result = await _camGirlService.UpdateUserCallStatus(statusDto.Username, statusDto.Status);
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
        [HttpGet("{username}")]
        public async Task<IActionResult> GetCamgirl(string username)
        {
            var result = await _camGirlService.FindCamGirlbyUserName(username);
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