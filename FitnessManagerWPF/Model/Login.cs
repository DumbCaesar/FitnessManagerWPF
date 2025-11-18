using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    internal class Login
    {
        public int MembershipId { get; set; }
        public string Username { get; set; } 
        public string Password { get; set; }
        public Login() { }
    }
}
