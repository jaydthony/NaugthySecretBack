namespace Model.DTO
{
    public class PaginatedTimeGenericDto<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public T Result { get; set; }
        public long TotalMinuteBuy { get; set; }
    }
}