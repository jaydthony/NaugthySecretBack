using Model.Enitities;

namespace Dating.API.Service.Interface
{
    public interface IGenerateJwt
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}