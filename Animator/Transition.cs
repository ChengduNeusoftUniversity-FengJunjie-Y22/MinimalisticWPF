using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public static class Transition
    {
        private static string _tempName = "temp";
        public static string TempName
        {
            get => _tempName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _tempName = value;
                }
            }
        }
        public static TransitionBoard<T> CreateBoardFromObject<T>(T target) where T : class
        {
            var result = new TransitionBoard<T>(target)
            {
                IsStatic = true
            };
            return result;
        }
        public static TransitionBoard<T> CreateBoardFromType<T>() where T : class
        {
            return new TransitionBoard<T>() { IsStatic = true };
        }
        public static void Dispose()
        {
            foreach (var machinedic in StateMachine.MachinePool.Values)
            {
                foreach (var machine in machinedic.Values)
                {
                    machine.Interpreter?.Stop();
                    foreach (var intor in machine.UnSafeInterpreters)
                    {
                        intor.Stop(true);
                    }
                }
            }
        }
        public static void Stop(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                machine.Interpreter?.Stop();
                foreach (var itor in machine.UnSafeInterpreters)
                {
                    itor.Stop(true);
                }
            }
        }
        public static void StopSafe(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                machine.Interpreter?.Stop();
            }
        }
        public static void StopUnSafe(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                foreach (var itor in machine.UnSafeInterpreters)
                {
                    itor.Stop(true);
                }
            }
        }
    }
}
