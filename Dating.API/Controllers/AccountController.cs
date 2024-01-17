using Dating.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using System.IdentityModel.Tokens.Jwt;

namespace Dating.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IEmailServices _emailServices;

        public AccountController(IAccountService accountService, IEmailServices emailServices)
        {
            _accountService = accountService;
            _emailServices = emailServices;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(SignUp signUp)
        {
            var registerUser = await _accountService.RegisterUser(signUp, "USER");
            if (registerUser.StatusCode == 200)
            {
                return Ok(registerUser);
            }
            else if (registerUser.StatusCode == 404)
            {
                return NotFound(registerUser);
            }
            else
            {
                return BadRequest(registerUser);
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN")]
        [HttpPost("register/camgirl")]
        public async Task<IActionResult> RegisterCamgirl(SignUp signUp)
        {
            var registerUser = await _accountService.RegisterUser(signUp, "CAMGIRL");
            if (registerUser.StatusCode == 200)
            {
                return Ok(registerUser);
            }
            else if (registerUser.StatusCode == 404)
            {
                return NotFound(registerUser);
            }
            else
            {
                return BadRequest(registerUser);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(SignInModel signIn)
        {
            var loginUser = await _accountService.LoginUser(signIn);
            if (loginUser.StatusCode == 200)
            {
                return Ok(loginUser);
            }
            else if (loginUser.StatusCode == 404)
            {
                return NotFound(loginUser);
            }
            else
            {
                return BadRequest(loginUser);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string loginEmail)
        {
            var result = await _accountService.LogoutUser(loginEmail);
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

        [HttpPost("forgot_password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _accountService.ForgotPassword(email);
            if (result.StatusCode == 200)
            {
                var message = new Message(new string[] { email }, "Reset Password Code", $"<p>Your reset password code is below<p><br/><h6>{result.Result}</h6><br/> <p>Please use it in your reset password page</p>");
                _emailServices.SendEmail(message);
                result.Result = $"Forgot passsword token was successfully sent to {email}";
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

        [HttpPost("reset_password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var result = await _accountService.ResetUserPassword(resetPassword);
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

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("delete_user/{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var result = await _accountService.DeleteUser(email);
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("me/info")]
        public async Task<IActionResult> GetUserFullDetails()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            if (userIdClaim == null)
            {
                return BadRequest("Invalid user");
            }
            var result = await _accountService.GetUserFullDetails(userIdClaim);
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
        
        [HttpGet("me/username/info/{username}")]
        public async Task<IActionResult> GetUserFullDetailByUserName(string username)
        {

            var result = await _accountService.GetUserFullDetailsWithUserName(username);
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPatch("update_details/{email}")]
        public async Task<IActionResult> UpdateUserInfo(string email, UpdateUserDto updateUser)
        {
            var result = await _accountService.UpdateUser(email, updateUser);
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
        [HttpPost("suspend_user/{email}")]
        public async Task<IActionResult> SuspendUser(string email)
        {
            var result = await _accountService.SuspendUserAsync(email);
            if (result.StatusCode == 200)
            {
                var message = new Message(new string[] { email }, "Suspend", $"<p>You have been suspended on lucky crush, please contact admin<p>");
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
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN")]
        [HttpPost("unsuspend_user/{email}")]
        public async Task<IActionResult> UnSuspendUser(string email)
        {
            var result = await _accountService.UnSuspendUserAsync(email);
            if (result.StatusCode == 200)
            {
                var message = new Message(new string[] { email }, "Unsuspend", $"<p>Congrat, you have been unsuspended on lucky crush, you can continue to use our service<p>");
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

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPatch("update_picture/{email}")]
        public async Task<IActionResult> UploadUserPicture(string email, IFormFile file)
        {
            var result = await _accountService.UploadUserProfilePicture(email, file);
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

        [HttpPatch("update_role/{email}")]
        public async Task<IActionResult> UpdateUserRoleAsync(string email, string role)
        {
            var result = await _accountService.UpdateUserRole(email, role);
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
        public async Task<IActionResult> GetAllUserAsync(int page_number, int per_page_size)
        {
            var result = await _accountService.GetAllUsersAsync(page_number, per_page_size);
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