using Model.Enum;

namespace Model.DTO
{
    public class UserTimeAvailableDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public bool IsVideoCallStatus { get; set; }
        public int TimeAvailable { get; set; }
    }
}