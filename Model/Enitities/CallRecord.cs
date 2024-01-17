using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Enitities
{
    public class CallRecord
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public decimal TimeUsed { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public string CamgirlId { get; set; }

        [ForeignKey("CamgirlId")]
        public ApplicationUser Camgirl { get; set; }
    }
}