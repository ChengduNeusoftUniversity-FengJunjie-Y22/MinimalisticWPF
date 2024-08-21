using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public class ConditionGroup
    {
        public ConditionGroup() { }

        public List<string> StateNames { get; internal set; } = new List<string>();

        public List<List<string>> StateConditions { get; internal set; } = new List<List<string>>();

        bool Add(string stateName, double transitionTime, ICollection<string>? conditions = null, bool isQueue = true, bool isLast = false, bool isUnique = false, double waitTime = 0.008, ICollection<string>? protectNames = default)
        {
            bool result = false;



            return result;
        }

        void Remove(string stateName)
        {

        }
    }
}
