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
    // =====================================
    //             DataService
    //      Author: Nicolaj og Oliver
    // =====================================
    public class DataService
    {
        // File Paths
        private readonly string _basePath;
        private readonly string _usersFile;
        private readonly string _loginFile;
        private readonly string _gymClassesFile;
        private readonly string _membershipsFile;

        // Data Collections
        private List<User> _users;
        private List<Login> _logins;
        private List<Membership> _memberships;
        private List<GymClass> _gymClasses;

        // Public Properties
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
        // Properties to track the maximum IDs for new record generation
        public int MaxUserId { get; set; }
        public int MaxSubscriptionId { get; set; }
        public int MaxClassId { get; set; }

        public DataService()
        {
            // Calculate base path relative to the application's root directory
            _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            // Define full file paths for JSON data
            _usersFile = Path.Combine(_basePath, "Data/users.json");
            _loginFile = Path.Combine(_basePath, "Data/logins.json");
            _gymClassesFile = Path.Combine(_basePath, "Data/gymclasses.json");
            _membershipsFile = Path.Combine(_basePath, "Data/memberships.json");

            // Initialize lists
            _users = new List<User>();
            _logins = new List<Login>();
            _gymClasses = new List<GymClass>();
            _memberships = new List<Membership>();

            try
            {
                // Load all data from JSON files
                LoadData();
            }
            catch (Exception ex)
            {
                // Log and re-throw exceptions during initialization
                Debug.WriteLine($"Failed to initialize DataService: {ex.Message}");
                throw;
            }

            try
            {
                MaxUserId = _users.Max(u => u.Id);
            }
            catch(InvalidOperationException ex)
            {
                MaxUserId = 0;
                Debug.WriteLine($"Failed to read max user ID. Defaulting to 0. Error: {ex.Message}");
            }

            try
            {
                MaxSubscriptionId = _users
                                    .Where(u => u.BillingHistory != null)
                                    .SelectMany(u => u.BillingHistory)
                                    .Select(sub => sub.Id)
                                    .DefaultIfEmpty(0)
                                    .Max();
            }
            catch (InvalidOperationException ex)
            {
                MaxSubscriptionId = 0;
                Debug.WriteLine($"Failed to read max subscription ID. Defaulting to 0. Error: {ex.Message}");
            }

            try
            {
                MaxClassId = _gymClasses.Max(c => c.Id);
            }
            catch (InvalidOperationException ex)
            {
                MaxClassId = 0;
                Debug.WriteLine($"Failed to read max class ID. Defaulting to 0. Error: {ex.Message}");
            }
        }

        // =====================================
        //             LoadData()
        //      Author: Nicolaj og Oliver
        // =====================================

        // --- Data Loading and Initialization ---
        public void LoadData()
        {
            try
            {
                // Set up JSON deserialization options
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                options.Converters.Add(new JsonStringEnumConverter());

                // Deserialize data from JSON files into in-memory lists
                _users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_usersFile), options);
                Debug.WriteLine($"Loaded {_users?.Count ?? 0} entries from {_usersFile}");

                _logins = JsonSerializer.Deserialize<List<Login>>(File.ReadAllText(_loginFile), options);
                Debug.WriteLine($"Loaded {_logins?.Count ?? 0} entries from {_loginFile}");

                _gymClasses = JsonSerializer.Deserialize<List<GymClass>>(File.ReadAllText(_gymClassesFile), options);
                Debug.WriteLine($"Loaded {_gymClasses?.Count ?? 0} entries from {_gymClasses}");

                _memberships = JsonSerializer.Deserialize<List<Membership>>(File.ReadAllText(_membershipsFile), options);
                Debug.WriteLine($"Loaded {_memberships?.Count ?? 0} entries from {_membershipsFile}");

                // Establish object relationships (e.g., link User to their Membership object)
                LinkMemberships();
                LinkTrainers();
            }
            // Catch specific file/format exceptions
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

        // =====================================
        //           ValidateUser()
        //      Author: Nicolaj og Oliver
        // =====================================
        // --- Core Application Logic ---
        // Validates a user's login credentials against the loaded logins.
        public bool ValidateUser(string username, string password)
        {
            var login = _logins.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (login != null)
            {
                // Set the CurrentUser if login is successful
                CurrentUser = _users.FirstOrDefault(u => u.Id == login.MembershipId);
                return true;
            }
            CurrentUser = null;
            return false;
        }

        // =====================================
        //            LinkMemberships()
        //            Author: Nicolaj
        // =====================================
        // Links User objects to their Membership objects using IDs.
        private void LinkMemberships()
        {
            foreach (User u in _users)
            {
                if (u.ActiveMembershipId == null)
                    u.ActiveMembership = null;
                else
                    // Find and assign the active membership object
                    u.ActiveMembership = _memberships.Where(m => m.Id == u.ActiveMembershipId).FirstOrDefault();

                foreach (Purchase p in u.BillingHistory)
                {
                    // Find and assign the membership object for each purchase in history
                    p.Membership = _memberships.Where(m => m.Id == p.MembershipId).FirstOrDefault();
                }
            }
        }

        // =====================================
        //             LinkTrainers()
        //            Author: Nicolaj
        // =====================================

        // Links GymClass objects to their Trainer (User) objects using the TrainerId.
        private void LinkTrainers()
        {
            foreach (GymClass c in _gymClasses)
            {
                // Find and assign the trainer user object, or a placeholder if the trainer is missing
                c.Trainer = _users.FirstOrDefault(u => u.Id == c.TrainerId) ?? new User { Id = c.TrainerId, Name = "Deleted Trainer" };
            }
        }

        // =====================================
        //            LoadUserInfo()
        //            Author: Oliver
        // =====================================
        // Retrieves the login details (username/password) for a given User.
        public Login? LoadUserInfo(User user)
        {
            if (user == null)
            {
                Debug.WriteLine("User or Login null");
                return null;
            }

            try
            {
                // Find the corresponding Login object by MembershipId
                return _logins.Where(l => l.MembershipId == user.Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred: {ex.Message}");
                return null;
            }
        }

        // =====================================
        //             CreateUser()
        //      Author: Nicolaj og Oliver
        // =====================================
        // Adds a new User and their Login details to the in-memory lists and persists the changes to JSON files.
        public void CreateUser(User user, Login login)  
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            _users.Add(user);
            _logins.Add(login);

            // Serialize updated lists to JSON strings
            string userJson = JsonSerializer.Serialize(_users, options);
            string loginJson = JsonSerializer.Serialize(_logins, options);

            // Write JSON strings back to files
            File.WriteAllText(_usersFile, userJson);
            File.WriteAllText(_loginFile, loginJson);

            Debug.WriteLine($"Added {user.Name} to {_usersFile}");
            Debug.WriteLine($"Added {login.Username} to {_loginFile}");
        }

        // =====================================
        //           SaveGymClasses()
        //           Author: Nicolaj
        // =====================================
        // Saves the current state of GymClasses to its JSON file.
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

        // =====================================
        //           SaveGymClasses()
        //           Author: Nicolaj
        // =====================================
        // Saves the current state of Users to its JSON file.
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

        // =====================================
        //           SaveGymClasses()
        //           Author: Nicolaj
        // =====================================
        // Saves the current state of Logins to its JSON file.
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

        // =====================================
        //         GetSelectedClass()
        //           Author: Oliver
        // =====================================
        // Retrieves an ObservableCollection of Users registered for a specific GymClass.
        public ObservableCollection<User> GetSelectedClass(GymClass cls)
        {
            if (cls == null) return null;
            // Find all users whose ID is in the class's RegisteredMemberIds list
            var users = _users.FindAll(u => cls.RegisteredMemberIds.Contains(u.Id));
            ObservableCollection<User> temp = new ObservableCollection<User>(users);
            return temp;

        }

        // =====================================
        //           DeleteMember()
        //           Author: Oliver
        // =====================================
        // Deletes a User and their associated Login from the system and updates files.
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

                // Remove user ID from all registered classes
                foreach (var cls in _gymClasses)
                {
                    while (cls.RegisteredMemberIds.Contains(user.Id))
                    {
                        cls.RegisteredMemberIds.Remove(user.Id);
                    }
                }

                // Persist all changes
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