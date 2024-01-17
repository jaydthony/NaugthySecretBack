using Data.Context;
using Data.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model.DTO;
using Model.Enitities;

namespace Data.Repository.Implementation
{
    public class AccountRepo : IAccountRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DatingSiteContext _context;

        public AccountRepo(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, DatingSiteContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        
        public async Task<bool> AddRoleAsync(ApplicationUser user, string Role)
        {
            var AddRole = await _userManager.AddToRoleAsync(user, Role);
            if (AddRole.Succeeded)
            {
                return true;
            }
            return false;
        }
        public async Task<ApplicationUser> FIndUserByUserName(string UserName)
        {
            var User = await _context.Users.FirstOrDefaultAsync(u=>u.UserName == UserName);
          
            return User;
        }

        public async Task<bool> RemoveRoleAsync(ApplicationUser user, IList<string> role)
        {
            var removeRole = await _userManager.RemoveFromRolesAsync(user, role);
            if (removeRole.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            var getRoles = await _userManager.GetRolesAsync(user);
            if (getRoles != null)
            {
                return getRoles;
            }
            return null;
        }

        public async Task<bool> RoleExist(string Role)
        {
            var check = await _roleManager.RoleExistsAsync(Role);
            return check;
        }

        public async Task<bool> UpdateLoginStatus(ApplicationUser user)
        {
            user.IsUserOnline = true;
            var updateStatus = await _userManager.UpdateAsync(user);
            if (updateStatus.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateLogoutStatus(ApplicationUser user)
        {
            user.IsUserOnline = false;
            var updateStatus = await _userManager.UpdateAsync(user);
            if (updateStatus.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateUserStatusTaken(ApplicationUser user)
        {
            user.UserIsTaken = true;
            var updateStatus = await _userManager.UpdateAsync(user);
            if (updateStatus.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateUserStatusNotTaken(ApplicationUser user)
        {
            user.UserIsTaken = false;
            var updateStatus = await _userManager.UpdateAsync(user);
            if (updateStatus.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ConfirmEmail(string token, ApplicationUser user)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUserByEmail(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
        {
            var findUser = await _userManager.FindByEmailAsync(email);
            if (findUser == null)
            {
                return null;
            }
            return findUser;
        }

        public async Task<ApplicationUser> FindUserByIdAsync(string id)
        {
            var findUser = await _userManager.FindByIdAsync(id);
            return findUser;
        }

        public async Task<string> ForgotPassword(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<PaginatedUser> GetAllRegisteredUserAsync(int pageNumber, int perPageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            perPageSize = perPageSize < 1 ? 5 : perPageSize;

            var filteredUser = _userManager.Users
                .Join(
                    _context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { User = user, UserRole = userRole })
                .Join(
                    _roleManager.Roles,
                    userRole => userRole.UserRole.RoleId,
                    role => role.Id,
                    (userRole, role) => new { User = userRole.User, Role = role })
                .Where(u => u.Role.Name == "USER")
                .Select(u => new DisplayFindUserDTO
                {
                    UserName = u.User.UserName,
                    Email = u.User.Email,
                    FirstName = u.User.FirstName,
                    LastName = u.User.LastName,
                    PhoneNumber = u.User.PhoneNumber,
                    ProfilePicture = u.User.ProfilePicture,
                    Gender = u.User.Gender,
                    SuspendUser = u.User.SuspendUser,
                    Id = u.User.Id,
                    TimeAvailable = u.User.TimeAvailable
                });

            var totalCount = await filteredUser.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / perPageSize);

            var paginatedUser = await filteredUser
                .Skip((pageNumber - 1) * perPageSize)
                .Take(perPageSize)
                .ToListAsync();
            var result = new PaginatedUser
            {
                CurrentPage = pageNumber,
                PageSize = perPageSize,
                TotalPages = totalPages,
                User = paginatedUser,
            };
            return result;
        }

        public async Task<bool> CheckEmailConfirmed(ApplicationUser user)
        {
            var checkConfirm = await _userManager.IsEmailConfirmedAsync(user);
            return checkConfirm;
        }

        public async Task<bool> CheckAccountPassword(ApplicationUser user, string password)
        {
            var checkUserPassword = await _userManager.CheckPasswordAsync(user, password);
            return checkUserPassword;
        }

        public async Task<ResetPassword> ResetPasswordAsync(ApplicationUser user, ResetPassword resetPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (result.Succeeded)
            {
                return resetPassword;
            }
            return null;
        }

        public async Task<ApplicationUser> SignUpAsync(ApplicationUser user, string Password)
        {
            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                return user;
            }
            return null;
        }

        public async Task<bool> UpdateUserInfo(ApplicationUser applicationUser)
        {
            var updateUserInfo = await _userManager.UpdateAsync(applicationUser);
            if (updateUserInfo.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}