using Model.DTO;

namespace Dating.API.Service.Interface
{
    public interface IEmailServices
    {
        void SendEmail(Message message);
    }
}