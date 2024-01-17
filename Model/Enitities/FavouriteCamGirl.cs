using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Enitities
{
    public class FavouriteCamGirl
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CamgirlUserName { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
