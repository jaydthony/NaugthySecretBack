using Model.DTO;
using Model.Enitities;

namespace Data.Repository.Interface
{
    public interface IAccountRepo
    {
        Task<ApplicationUser> SignUpAsync(ApplicationUser user, string Password);

        Task<bool> CheckAccountPassword(ApplicationUser user, string password);

        Task<bool> UpdateLoginStatus(ApplicationUser user);

        Task<bool> UpdateLogoutStatus(ApplicationUser user);

        Task<bool> UpdateUserStatusTaken(ApplicationUser user);

        Task<bool> UpdateUserStatusNotTaken(ApplicationUser user);
        Task<ApplicationUser> FIndUserByUserName(string UserName);

        Task<bool> CheckEmailConfirmed(ApplicationUser user);

        Task<bool> AddRoleAsync(ApplicationUser user, string Role);

        Task<string> ForgotPassword(ApplicationUser user);

        Task<bool> ConfirmEmail(string token, ApplicationUser user);

        Task<bool> RemoveRoleAsync(ApplicationUser user, IList<string> role);

        Task<ResetPassword> ResetPasswordAsync(ApplicationUser user, ResetPassword resetPassword);

        Task<bool> RoleExist(string Role);

        Task<ApplicationUser?> FindUserByEmailAsync(string email);

        Task<ApplicationUser> FindUserByIdAsync(string id);

        Task<bool> UpdateUserInfo(ApplicationUser applicationUser);

        Task<IList<string>> GetUserRoles(ApplicationUser user);

        Task<PaginatedUser> GetAllRegisteredUserAsync(int pageNumber, int perPageSize);

        Task<bool> DeleteUserByEmail(ApplicationUser user);
    }
}