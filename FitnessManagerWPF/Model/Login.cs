using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    public class Login
    {
        public int MembershipId { get; set; } // Foreign id property for the user. MembershipId = User.Id
        public string Username { get; set; }
        public string Password { get; set; }
        public Login() { } // Empty constructor used for JSON serialization

        public Login(int id, string username, string password)
        {
            MembershipId = id;
            Username = username;
            Password = password;
        }
    }
}
