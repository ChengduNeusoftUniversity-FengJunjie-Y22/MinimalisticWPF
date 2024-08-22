using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public class StateVector
    {
        internal StateVector() { }

        public string Name { get; internal set; } = string.Empty;
        public string StateName { get; internal set; } = string.Empty;
        public List<string> Conditions { get; internal set; } = new List<string>();
        public TransferParams TransferParams { get; internal set; } = new TransferParams();

        public static StateVector Creat(string vectorName)
        {
            StateVector result = new StateVector();
            result.Name = vectorName;
            return result;
        }

        public StateVector SetState(string stateName)
        {
            StateName = stateName;
            return this;
        }

        public StateVector SetConditions(params string[] conditions)
        {
            Conditions.Clear();
            List<string> noSame = conditions.Distinct().ToList();
            foreach (var item in noSame)
            {
                Conditions.Add(item.Replace(" ", string.Empty));
            }
            return this;
        }

        public StateVector SetTransferParams(double transitionTime = 0, bool isQueue = false, bool isLast = true, bool isUnique = false, int? frameRate = default, double waitTime = 0.008, ICollection<string>? protectNames = default)
        {
            TransferParams = new TransferParams(transitionTime, isQueue, isLast, isUnique, frameRate, waitTime, protectNames);
            return this;
        }
    }

    public class TransferParams
    {
        internal TransferParams(double transitionTime = 0, bool isQueue = false, bool isLast = true, bool isUnique = false, int? frameRate = default, double waitTime = 0.008, ICollection<string>? protectNames = default)
        {
            TransitionTime = transitionTime;
            FrameRate = frameRate == null ? FrameRate : (int)frameRate;
            IsQueue = isQueue;
            IsLast = isLast;
            IsUnique = isUnique;
            WaitTime = waitTime;
            ProtectNames = protectNames == null ? ProtectNames : ProtectNames;
        }

        public double TransitionTime { get; internal set; } = 0;
        public int FrameRate { get; internal set; } = 500;
        public bool IsQueue { get; internal set; } = false;
        public bool IsLast { get; internal set; } = true;
        public bool IsUnique { get; internal set; } = true;
        public double WaitTime { get; internal set; } = 0.008;
        public ICollection<string>? ProtectNames { get; internal set; } = default;
    }
}
