using AutoMapper;
using Data.Repository.Interface;
using Dating.API.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Model.DTO;
using Model.Enitities;
using System.IdentityModel.Tokens.Jwt;

namespace Dating.API.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;
        private readonly IGenerateJwt _generateJwt;
        private readonly ICloudinaryService _cloudinary;
        private readonly ICamGirlRepo _camGirlRepo;

        public AccountService(IAccountRepo accountRepo, ILogger<AccountService> logger, IMapper mapper, IGenerateJwt generateJwt, ICloudinaryService cloudinary, ICamGirlRepo camGirlRepo)
        {
            _accountRepo = accountRepo;
            _logger = logger;
            _mapper = mapper;
            _generateJwt = generateJwt;
            _cloudinary = cloudinary;
            _camGirlRepo = camGirlRepo;
        }

        public async Task<ResponseDto<string>> RegisterUser(SignUp signUp, string Role)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByEmailAsync(signUp.Email);
                if (checkUserExist != null)
                {
                    response.ErrorMessages = new List<string>() { "User with the email already exist" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var checkRole = await _accountRepo.RoleExist(Role);
                if (checkRole == false)
                {
                    response.ErrorMessages = new List<string>() { "Role is not available" };
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapAccount = _mapper.Map<ApplicationUser>(signUp);
                mapAccount.TimeAvailable = 0.15m;
                var createUser = await _accountRepo.SignUpAsync(mapAccount, signUp.Password);
                if (createUser == null)
                {
                    response.ErrorMessages = new List<string>() { "User not created successfully" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var addRole = await _accountRepo.AddRoleAsync(createUser, Role);
                if (addRole == false)
                {
                    response.ErrorMessages = new List<string>() { "Fail to add role to user" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Successful";
                response.Result = "User successfully created";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in resgistering the user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<PaginatedUser>> GetAllUsersAsync(int pageNumber, int perPageSize)
        {
            var response = new ResponseDto<PaginatedUser>();
            try
            {
                var getCAMGIRL = await _accountRepo.GetAllRegisteredUserAsync(pageNumber, perPageSize);
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

        public async Task<ResponseDto<LoginResultDto>> LoginUser(SignInModel signIn)
        {
            var response = new ResponseDto<LoginResultDto>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByEmailAsync(signIn.Email);
                if (checkUserExist == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                if(checkUserExist.SuspendUser == true)
                {
                    response.ErrorMessages = new List<string>() { "User is suspended, contact admin" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var checkPassword = await _accountRepo.CheckAccountPassword(checkUserExist, signIn.Password);
                if (checkPassword == false)
                {
                    response.ErrorMessages = new List<string>() { "Invalid Password" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var generateToken = await _generateJwt.GenerateToken(checkUserExist);
                if (generateToken == null)
                {
                    response.ErrorMessages = new List<string>() { "Error in generating jwt for user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var changeStatus = await _accountRepo.UpdateLoginStatus(checkUserExist);
                if (changeStatus == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in login the user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var getUserRole = await _accountRepo.GetUserRoles(checkUserExist);
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Successfully login";
                response.Result = new LoginResultDto() { Jwt=generateToken, UserRole=getUserRole};
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in login the user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> LogoutUser(string UserEmail)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByEmailAsync(UserEmail);
                if (checkUserExist == null)
                {
                    response.ErrorMessages = new List<string>() { "The user is not login" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }

                var removeStatuseActive = await _accountRepo.UpdateUserStatusNotTaken(checkUserExist);
                if (removeStatuseActive == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in logout the user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var removeloginStatus = await _accountRepo.UpdateLogoutStatus(checkUserExist);
                if (removeloginStatus == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in logout the user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully logout user";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Unable to logout user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> ForgotPassword(string UserEmail)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkUser = await _accountRepo.FindUserByEmailAsync(UserEmail);
                if (checkUser == null)
                {
                    response.ErrorMessages = new List<string>() { "Email is not available" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var result = await _accountRepo.ForgotPassword(checkUser);
                if (result == null)
                {
                    response.ErrorMessages = new List<string>() { "Error in generating reset token for user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.DisplayMessage = "Token generated Successfully";
                response.Result = result;
                response.StatusCode = 200;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in generating reset token for user" };
                response.StatusCode = 501;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> ResetUserPassword(ResetPassword resetPassword)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(resetPassword.Email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var resetPasswordAsync = await _accountRepo.ResetPasswordAsync(findUser, resetPassword);
                if (resetPasswordAsync == null)
                {
                    response.ErrorMessages = new List<string>() { "Invalid token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully reset user password";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in reset user password" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> DeleteUser(string email)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var deleteUser = await _accountRepo.DeleteUserByEmail(findUser);
                if (deleteUser == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in deleting user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully delete user";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in deleting user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> UpdateUser(string email, UpdateUserDto updateUser)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapUpdateDetails = _mapper.Map(updateUser, findUser);
                var updateUserDetails = await _accountRepo.UpdateUserInfo(mapUpdateDetails);
                if (updateUserDetails == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in updating user info" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully update user information";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in updating user info" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> UploadUserProfilePicture(string email, IFormFile file)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var uploadImage = await _cloudinary.UploadPhoto(file, email);
                if (uploadImage == null)
                {
                    response.ErrorMessages = new List<string>() { "Error in uploading profile picture for user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                findUser.ProfilePicture = uploadImage.Url.ToString();
                var updateUserDetails = await _accountRepo.UpdateUserInfo(findUser);
                if (updateUserDetails == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in updating user profile pictures" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully update user profile picture";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in updating user info" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }

        public async Task<ResponseDto<string>> UpdateUserRole(string email, string role)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var checkRole = await _accountRepo.RoleExist(role);
                if (checkRole == false)
                {
                    response.ErrorMessages = new List<string>() { "Role is not available" };
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var getExistingRoles = await _accountRepo.GetUserRoles(findUser);
                if (getExistingRoles.Count > 0)
                {
                    var removeExistingRoles = await _accountRepo.RemoveRoleAsync(findUser, getExistingRoles);
                    if (removeExistingRoles == false)
                    {
                        response.ErrorMessages = new List<string>() { "Error in removing role for user" };
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        response.DisplayMessage = "Error";
                        return response;
                    }
                }
                
                var addRole = await _accountRepo.AddRoleAsync(findUser, role);
                if (addRole == false)
                {
                    response.ErrorMessages = new List<string>() { "Fail to add role to user" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Successful";
                response.Result = "User role updated successfully";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in updating user role" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> SuspendUserAsync(string useremail)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(useremail);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                findUser.SuspendUser = true;
                var updateUser = await _accountRepo.UpdateUserInfo(findUser);
                if (updateUser == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in suspending user" };
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.DisplayMessage = "Error";
                    return response;
                }

                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully suspend user";
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in suspending user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> UnSuspendUserAsync(string useremail)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(useremail);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                findUser.SuspendUser = false;
                var updateUser = await _accountRepo.UpdateUserInfo(findUser);
                if (updateUser == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in unsuspending user" };
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.DisplayMessage = "Error";
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully unsuspend user";
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in unsuspending user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<DisplayUserWithRoleDto>> GetUserFullDetails(string userid)
        {
            var response = new ResponseDto<DisplayUserWithRoleDto>();
            try
            {
                
                var findUser = await _accountRepo.FindUserByIdAsync(userid);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapuser = _mapper.Map<DisplayUserWithRoleDto>(findUser);
                var getUserRole = await _accountRepo.GetUserRoles(findUser);
                var fav = await _camGirlRepo.GetUserFavCamGirl(userid);
                if (fav.Any())
                {
                    mapuser.FavLists = fav;
                }
                mapuser.Role = getUserRole[0];
                response.Result = mapuser;
                response.StatusCode = 200;
                response.DisplayMessage = "Successful";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in retrieving user details" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<DisplayUserWithRoleDto>> GetUserFullDetailsWithUserName(string userName)
        {
            var response = new ResponseDto<DisplayUserWithRoleDto>();
            try
            {

                var findUser = await _accountRepo.FIndUserByUserName(userName);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapuser = _mapper.Map<DisplayUserWithRoleDto>(findUser);
                var getUserRole = await _accountRepo.GetUserRoles(findUser);
                mapuser.Role = getUserRole[0];
                var fav = await _camGirlRepo.GetUserFavCamGirl(findUser.Id);
                if (fav.Any())
                {
                    mapuser.FavLists = fav;
                }
                response.Result = mapuser;
                response.StatusCode = 200;
                response.DisplayMessage = "Successful";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in retrieving user details" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
    }
}