using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    internal class User
    {
        private int _id;
        private string _name;
        private string _email;
        private string _membership;
        private Role role;
    }

    enum Role
    {
        Member,
        Trainer,
        Admin
    }
}
