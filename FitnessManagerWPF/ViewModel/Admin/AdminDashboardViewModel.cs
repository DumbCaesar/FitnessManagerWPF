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

        public int TotalMemberCount { get; set; }
        public int ActiveMemberCount { get; set; }
        public int InactiveMemberCount { get; set; }
        public int ClassesToday { get; set; }
        public int ClassesWeek { get; set; }
        public string Attendance { get; set; }
        public int MonthlySignups { get; set; }
        public int ExpiringMemberships { get; set; }
        public decimal MRR { get; set; }

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public AdminDashboardViewModel(AdminViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
            UpdateMemberCounts();
            UpdateClasses();
            UpdateKPIs();
        }
        private void UpdateMemberCounts()
        {
            TotalMemberCount = _dataService.Users.Where(u => u.UserRole == Model.UserRole.Member).Count();
            ActiveMemberCount = _dataService.Users.Where(u => u.IsActiveMember).Count();
            InactiveMemberCount = TotalMemberCount - ActiveMemberCount;
        }

        private void UpdateClasses()
        {
            ClassesToday = _dataService._activities.Where(d => d.Day == DateTime.Today.DayOfWeek).Count();
            ClassesWeek = _dataService._activities.Count();
            int MaxAttendance = _dataService._activities.Sum(a => a.MaxParticipants);
            Debug.WriteLine($"Max attendance this week: {MaxAttendance}");
            int CurrentAttendance = _dataService._activities.Sum(a => a.RegisteredMemberIds.Count());
            Debug.WriteLine($"Current attendance this week: {CurrentAttendance}");
            Attendance = $"{CurrentAttendance}/{MaxAttendance}";
            Debug.WriteLine($"Attendance: {CurrentAttendance}/{MaxAttendance}");
        }

        private void UpdateKPIs()
        {
            MonthlySignups = _dataService.Users.Where(u => u.DateJoined.Month == DateTime.Today.Month).Count();
            ExpiringMemberships = _dataService.Users
                .Where(u => u.IsActiveMember)
                .Count(u => u.CurrentMembership().EndDate >= DateTime.Today &&
                u.CurrentMembership().EndDate <= DateTime.Today.AddDays(7));
            MRR = _dataService.Users
                .Where(u => u.IsActiveMember)
                .Sum(u => u.MonthlyContribution);
        }
    }
}
