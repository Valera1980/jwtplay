using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jwtplay.Models
{
    public class LoginRequest
    {
        public String UserName { get; set; }
        public String Password { get; set; }
    }
}
