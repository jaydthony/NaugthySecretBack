using Model.Enum;

namespace Model.DTO
{
    public class DisplayUserWithRoleDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public bool SuspendUser { get; set; }
        public int TimeAvailable { get; set; }
        public string Role { get; set; }
        public IEnumerable<FavList> FavLists { get; set; }
    }
}
