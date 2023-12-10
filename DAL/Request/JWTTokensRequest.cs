using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Requests
{
    public class JWTTokensRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }

    }
}
