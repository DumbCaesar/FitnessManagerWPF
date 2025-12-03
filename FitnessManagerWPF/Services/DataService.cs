using FitnessManagerWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FitnessManagerWPF.Services
{
    public class DataService
    {
        // file paths
        private readonly string _basePath;
        private readonly string _usersFile;
        private readonly string _loginFile;
        private readonly string _gymClassesFile;
        private readonly string _membershipsFile;

        //  lists
        private List<User> _users;
        private List<Login> _logins;
        private List<Membership> _memberships;
        private List<GymClass> _gymClasses;

        public User CurrentUser { get; private set; }
        public List<Membership> Memberships
        {
            get => _memberships;
            set => _memberships = value;
        }
        public List<User> Users
        {
            get => _users;
            private set => _users = value;
        }
        public List<Login> Logins
        {
            get => _logins;
            private set => _logins = value;
        }
        public List<GymClass> GymClasses
        {
            get => _gymClasses;
            private set => _gymClasses = value;
        }
        public int MaxUserId { get; set; }

        public int MaxSubscriptionId { get; set; }
        public int MaxClassId { get; set; }

        public DataService()
        {
            _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            _usersFile = Path.Combine(_basePath, "Data/users.json");
            _loginFile = Path.Combine(_basePath, "Data/logins.json");
            _gymClassesFile = Path.Combine(_basePath, "Data/gymclasses.json");
            _membershipsFile = Path.Combine(_basePath, "Data/memberships.json");

            _users = new List<User>();
            _logins = new List<Login>();
            _gymClasses = new List<GymClass>();
            _memberships = new List<Membership>();

            try
            {
                LoadData();
                MaxUserId = _users.Max(u => u.Id);
                MaxSubscriptionId = _users
                .Where(u => u.BillingHistory != null)
                .SelectMany(u => u.BillingHistory)
                .Select(sub => sub.Id)
                .DefaultIfEmpty(0)
                .Max();
                MaxClassId = _gymClasses.Max(c => c.Id);
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

                _users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_usersFile), options);
                Debug.WriteLine($"Loaded {_users?.Count ?? 0} entries from {_usersFile}");

                _logins = JsonSerializer.Deserialize<List<Login>>(File.ReadAllText(_loginFile), options);
                Debug.WriteLine($"Loaded {_logins?.Count ?? 0} entries from {_loginFile}");

                _gymClasses = JsonSerializer.Deserialize<List<GymClass>>(File.ReadAllText(_gymClassesFile), options);
                Debug.WriteLine($"Loaded {_gymClasses?.Count ?? 0} entries from {_gymClasses}");

                _memberships = JsonSerializer.Deserialize<List<Membership>>(File.ReadAllText(_membershipsFile), options);
                Debug.WriteLine($"Loaded {_memberships?.Count ?? 0} entries from {_membershipsFile}");

                LinkMemberships();
                LinkTrainers();
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

        private void LinkMemberships()
        {
            foreach(User u in _users)
            {
                if (u.ActiveMembershipId == null)
                    u.ActiveMembership = null;
                else
                    u.ActiveMembership = _memberships.Where(m => m.Id == u.ActiveMembershipId).FirstOrDefault();

                foreach (Purchase p in u.BillingHistory)
                {
                    p.Membership = _memberships.Where(m => m.Id == p.MembershipId).FirstOrDefault();
                }
            }
        }

        private void LinkTrainers()
        {
            foreach (GymClass c in _gymClasses)
            {
                c.Trainer = _users.FirstOrDefault(u => u.Id == c.TrainerId) ?? new User { Id = c.TrainerId, Name = "Deleted Trainer" };
            }
        }

        public Login? LoadUserInfo(User user)
        {
            var _user = _users.FirstOrDefault(u => u.Id == user.Id);
            var _login = _user != null ? _logins.FirstOrDefault(l => l.MembershipId == _user.Id) : null;

            if (_user == null ||  _login == null)
            {
                Debug.WriteLine("User or Login null");
                return null;
            }

            try
            {
                user.Name = _user.Name;
                user.Email = _user.Email;

                return new Login
                {
                    MembershipId = _login.MembershipId,
                    Username = _login.Username,
                    Password = _login.Password
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred: {ex.Message}");
                return null;
            }
        }

        public void CreateUser(User user, Login login)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            _users.Add(user);
            _logins.Add(login);

            string userJson = JsonSerializer.Serialize(_users, options);
            string loginJson = JsonSerializer.Serialize(_logins, options); 

            File.WriteAllText(_usersFile, userJson);
            File.WriteAllText(_loginFile, loginJson);

            Debug.WriteLine($"Added {user.Name} to {_usersFile}");
            Debug.WriteLine($"Added {login.Username} to {_loginFile}");
        }

        public void SaveGymClasses()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            string gymClassesJson = JsonSerializer.Serialize(_gymClasses, options);
            File.WriteAllText(_gymClassesFile, gymClassesJson);
            Debug.WriteLine($"Updated {_gymClassesFile}");
        }

        public void SaveUsers()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            string membersJson = JsonSerializer.Serialize(_users, options);
            File.WriteAllText(_usersFile, membersJson);
            Debug.WriteLine($"Updated {_usersFile}");
        }

        public void SaveLogins()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            string loginsJson = JsonSerializer.Serialize(_logins, options);
            File.WriteAllText(_loginFile, loginsJson);
            Debug.WriteLine($"Updated {_loginFile}");
        }
        public ObservableCollection<User> GetSelectedClass(GymClass cls)
        {
            if (cls == null) return null;
            var users = _users.FindAll(u => cls.RegisteredMemberIds.Contains(u.Id));
            ObservableCollection<User> temp = new ObservableCollection<User>(users);
            return temp;

        }
        public void DeleteMember(User user)
        {
            try
            {
                var userLogin = _logins.FirstOrDefault(u => u.MembershipId == user.Id);
                if (userLogin != null)
                {
                    // Remove login
                    _logins.Remove(userLogin);
                }

                // Remove user
                _users.Remove(user);

                // Remove user from all classes
                foreach (var cls in _gymClasses)
                {
                    while (cls.RegisteredMemberIds.Contains(user.Id))
                    {
                        cls.RegisteredMemberIds.Remove(user.Id);
                    }
                }

                SaveGymClasses();
                SaveLogins();
                SaveUsers();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
