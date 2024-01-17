namespace Model.Enitities
{
    public class Timers
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int TimeBought { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}