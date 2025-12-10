using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FitnessManagerWPF.Services;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class AdminDashboardViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private object _currentView;
        private AdminViewModel _parentViewModel;

        // Dashboard KPIs
        public int TotalMemberCount { get; set; }
        public int ActiveMemberCount { get; set; }
        public int InactiveMemberCount { get; set; }
        public int ClassesToday { get; set; }
        public int ClassesWeek { get; set; }
        public double CapacityPercentage { get; set; }
        public int MonthlySignups { get; set; }
        public int ExpiringMemberships { get; set; }
        public decimal MRR { get; set; } // Monthly Recurring Revenue

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public AdminDashboardViewModel(AdminViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
            // Refresh dashboard whenever data changes elsewhere in the app
            _parentViewModel.DataChanged += RefreshAll;
            UpdateMemberCounts();
            UpdateClasses();
            UpdateKPIs();
        }
        private void UpdateMemberCounts()
        {
            TotalMemberCount = _dataService.Users.Where(u => u.UserRole == Model.UserRole.Member).Count();
            ActiveMemberCount = _dataService.Users.Where(u => u.HasActiveMembership).Count();
            InactiveMemberCount = TotalMemberCount - ActiveMemberCount;
        }

        private void UpdateClasses()
        {   
            ClassesToday = _dataService.GymClasses.Where(d => d.Day == DateTime.Today.DayOfWeek).Count();
            ClassesWeek = _dataService.GymClasses.Count();
            // Total possible vs actual attendance to compute global capacity
            int MaxAttendance = _dataService.GymClasses.Sum(a => a.MaxParticipants);
            Debug.WriteLine($"Max attendance this week: {MaxAttendance}");
            int CurrentAttendance = _dataService.GymClasses.Sum(a => a.CurrentParticipants);
            Debug.WriteLine($"Current attendance this week: {CurrentAttendance}");
            CapacityPercentage = (double)CurrentAttendance/MaxAttendance;
            Debug.WriteLine($"Attendance: {CapacityPercentage}");
        }

        private void UpdateKPIs()
        {
            // Users who joined this month
            MonthlySignups = _dataService.Users.Where(u => u.DateJoined.Month == DateTime.Today.Month).Count();
            // Memberships expiring within the next 7 days
            ExpiringMemberships = _dataService.Users
                .Where(u => u.HasActiveMembership)
                .Count(u => u.MembershipExpiresAt >= DateTime.Today &&
                u.MembershipExpiresAt <= DateTime.Today.AddDays(7));
            MRR = CalculateMRR();
        }

        private decimal CalculateMRR()
        {
            decimal total = 0m;
            var now = DateTime.Now;
            // Converts each user's membership to a monthly equivalent
            foreach (var user in _dataService.Users)
            {
                if (user.MembershipExpiresAt > now && user.ActiveMembership != null)
                {
                    decimal monthlyPrice = user.ActiveMembership.Price / user.ActiveMembership.DurationInMonths;
                    total += monthlyPrice;
                }
            }
            return total;
        }

        public void RefreshAll()
        {
            UpdateClasses();
            UpdateMemberCounts();
            UpdateKPIs();
        }
    } 
}
