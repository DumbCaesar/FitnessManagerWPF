using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    public class User
    {
        private string _email;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email
        {
            get => _email;
            set
            {
                if (value.Contains('@'))
                {
                    _email = value;
                }
            }
        }
        public string Membership { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("role")]
        public UserRole UserRole { get; set; }

        public User() { }
    }

    public enum UserRole
    {
        Member,
        Trainer,
        Admin
    }
}
