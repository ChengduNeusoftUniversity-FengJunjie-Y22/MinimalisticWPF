using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public class StateVector
    {
        internal StateVector() { }

        public State State { get; set; } = new State("defualt");
        public string Name { get; internal set; } = string.Empty;
        public string StateName { get; internal set; } = string.Empty;
        public Func<dynamic, bool>? Condition { get; internal set; } = null;
        public TransferParams TransferParams { get; internal set; } = new TransferParams();

        /// <summary>
        /// 从一个State创建条件Vector
        /// </summary>
        public static TempStateVector FromState(State state)
        {
            TempStateVector result = new TempStateVector();
            result.Value.State = state;
            return result;
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

    public class TempStateVector
    {
        internal TempStateVector() { }

        internal StateVector Value { get; set; } = new StateVector();
    }
}
