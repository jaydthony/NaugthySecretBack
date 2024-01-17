namespace Model.DTO
{
    public class PaginatedUser
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<DisplayFindUserDTO> User { get; set; }
    }
}