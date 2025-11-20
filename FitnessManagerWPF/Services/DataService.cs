using FitnessManagerWPF.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FitnessManagerWPF.Services
{
    public class DataService
    {
        private readonly string _basePath;
        private readonly string _membersFile;
        private readonly string _loginFile;
        private readonly string _membershipsFile;
        private List<User> _users;
        private List<Login> _logins;
        private List<Membership> _memberships;

        public User CurrentUser { get; private set; }

        public List<User> Users
        {
            get => _users;
            private set => _users = value;
        }

        public DataService()
        {
            _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            _membersFile = Path.Combine(_basePath, "Data/members.json");
            _loginFile = Path.Combine(_basePath, "Data/logins.json");
            _membershipsFile = Path.Combine(_basePath, "Data/memberships.json");

            _users = new List<User>();
            _logins = new List<Login>();
            _memberships = new List<Membership>();

            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to initialize DataService: {ex.Message}");
                throw;
            }
        }

        public void LoadData()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                options.Converters.Add(new JsonStringEnumConverter());

                _users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_membersFile), options);
                Debug.WriteLine($"Loaded {_users?.Count ?? 0} entries from {_membersFile}");

                _logins = JsonSerializer.Deserialize<List<Login>>(File.ReadAllText(_loginFile), options);
                Debug.WriteLine($"Loaded {_logins?.Count ?? 0} entries from {_loginFile}");

                _memberships = JsonSerializer.Deserialize<List<Membership>>(File.ReadAllText(_membershipsFile), options);
                Debug.WriteLine($"Loaded {_memberships?.Count ?? 0} entries from {_membershipsFile}");
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine($"Data file not found: {ex.Message}");        
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        public bool ValidateUser(string username, string password)
        {
            var login = _logins.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (login != null)
            {
                CurrentUser = _users.FirstOrDefault(u => u.Id == login.MembershipId);
                return true;
            }
            CurrentUser = null;
            return false;
        }

        public void SetUserCurrentMembership()
        {
            foreach (User u in _users)
            {
                if (u.UserRole == UserRole.Trainer)
                {
                    u.MembershipType = "Trainer";
                    continue;
                }
                if (u.UserRole == UserRole.Admin)
                {
                    u.MembershipType = "Admin";
                    continue;
                }

                var activeSub = u.CurrentMembership();
                if (activeSub == null)
                {
                    u.MembershipType = "No Active Membership";
                    continue;
                }

                var membershipType = _memberships.FirstOrDefault(m => m.Id == activeSub.MembershipId);
                u.MembershipType = membershipType?.Name ?? "Unknown";
            }
        }
    }
}
