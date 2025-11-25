using FitnessManagerWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly string _classesFile;
        private List<User> _users;
        private List<Login> _logins;
        private readonly string _membershipsFile;
        private List<Membership> _memberships;
        public List<Classes> _activities;
        public User CurrentUser { get; private set; }
        public int MaxUserId { get; set; }

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

        public DataService()
        {
            _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            _membersFile = Path.Combine(_basePath, "Data/members.json");
            _loginFile = Path.Combine(_basePath, "Data/logins.json");
            _classesFile = Path.Combine(_basePath, "Data/classes.json");
            _membershipsFile = Path.Combine(_basePath, "Data/memberships.json");

            _users = new List<User>();
            _logins = new List<Login>();
            _activities = new List<Classes>();
            _memberships = new List<Membership>();

            try
            {
                LoadData();
                MaxUserId = _users.Max(u => u.Id);
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

                _activities = JsonSerializer.Deserialize<List<Classes>>(File.ReadAllText(_classesFile), options);
                Debug.WriteLine($"Loaded {_activities?.Count ?? 0} entries from {_classesFile}");

                _memberships = JsonSerializer.Deserialize<List<Membership>>(File.ReadAllText(_membershipsFile), options);
                Debug.WriteLine($"Loaded {_memberships?.Count ?? 0} entries from {_membershipsFile}");

                SetUserCurrentMembership();
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

        private void SetUserCurrentMembership()
        {
            foreach (User u in _users.Where(u => u.UserRole == UserRole.Member))
            {
                // Link Membership object to each subscription in BillingHistory
                if (u.BillingHistory != null)
                {
                    foreach (MembershipSubscription sub in u.BillingHistory)
                    {
                        sub.Membership = _memberships.FirstOrDefault(m => m.Id == sub.MembershipId);
                    }
                }

                var activeSub = u.CurrentMembership();
                if (activeSub?.Membership != null)
                {
                    u.MembershipType = activeSub.Membership.Name;
                }
                else
                {
                    u.MembershipType = "No Active Membership";
                }
            }

            // Trainers & Admins
            foreach (User u in _users.Where(u => u.UserRole != UserRole.Member))
            {
                u.MembershipType = u.UserRole.ToString();
            }
        }

        public void GetMemberDetails(int memberId,
        ObservableCollection<Classes> MemberClasses,
        ObservableCollection<MembershipSubscription> MemberSubscriptions)
        {
            MemberClasses.Clear();
            MemberSubscriptions.Clear();

            // Find all classes where the member is registered
            var memberClasses = _activities
                .Where(c => c.RegisteredMemberIds != null && c.RegisteredMemberIds.Contains(memberId))
                .ToList();

            // ADD the classes to the collection
            foreach (var cls in memberClasses)
            {
                MemberClasses.Add(cls);
            }

            // Get the member's billing history
            var member = _users.FirstOrDefault(u => u.Id == memberId);
            if (member?.BillingHistory != null)
            {
                // ADD the billing records to the collection
                foreach (var billing in member.BillingHistory)
                {
                    MemberSubscriptions.Add(billing);
                }
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

            File.WriteAllText(_membersFile, userJson);
            File.WriteAllText(_loginFile, loginJson);

            Debug.WriteLine($"Added {user.Name} to {_membersFile}");
            Debug.WriteLine($"Added {login.Username} to {_loginFile}");
        }

        public void SaveUser(User user, Login login)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            var _user = _users.FirstOrDefault(u => u.Id == user.Id);
            var _userLogin = _logins.FirstOrDefault(l => l.MembershipId == _user.Id);

            if (_user == null || _userLogin == null)
            {
                Debug.WriteLine("User or login not found, nothing to update.");
                return;
            }

            try
            {

                _user.Name = user.Name;
                _user.Email = user.Email;
                _userLogin.Username = login.Username;
                _userLogin.Password = login.Password;

                int userIndex = _users.IndexOf(_user);
                int loginIndex = _logins.IndexOf(_userLogin);

                _users[userIndex] = _user;
                _logins[loginIndex] = _userLogin;

                string userJson = JsonSerializer.Serialize(_users, options);
                string loginJson = JsonSerializer.Serialize(_logins, options);
                File.WriteAllText(_membersFile, userJson);
                File.WriteAllText(_loginFile, loginJson);

                Debug.WriteLine($"Saved {user.Name} to {_membersFile}");
                Debug.WriteLine($"Saved {login.Username} to {_loginFile}");
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
                Debug.WriteLine($"Exception occured: {ex.Message}");
            }
        }
    }
}
