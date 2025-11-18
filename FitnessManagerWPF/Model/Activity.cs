using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    internal class Activity
    {
        private int _id;
        private string _name;
        private int maxParticipants;
        private List<int> registeredMemberIds;
    }
}
