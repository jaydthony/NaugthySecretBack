using Model.Enitities;

namespace Model.DTO
{
    public class UserTimeDto
    {
        public ApplicationUser User { get; set; }
        public long TotalTime { get; set; }
    }
}