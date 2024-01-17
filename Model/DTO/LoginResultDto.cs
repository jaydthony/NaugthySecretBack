using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class LoginResultDto
    {
        public string Jwt { get; set; }
        public IList<string> UserRole { get; set; }
    }
}
