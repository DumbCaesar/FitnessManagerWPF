using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    /// <summary>
    /// Memberships belonging to individual members
    /// </summary>
    public class MembershipSubscription
    {
        private int _id;
        private Membership _membership;
        private int _userId;
        private DateTime _startDate;
        private DateTime _endDate;
        private bool isActive;
    }
}
