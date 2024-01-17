using Model.Enum;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string UserName { get; set; }
    }
}