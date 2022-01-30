using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class UserDetailsResponse
    {
#nullable enable

        public string? email { get; set; }
        public string? weatherStateName { get; set; }
        public string? updateDate { get; set; }

#nullable disable
    }
}
