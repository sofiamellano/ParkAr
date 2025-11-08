using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models.Login
{
    public class LoginResponse
    {
        public string? UserUid { get; set; }
        public string Error { get; set; }
    }
}
