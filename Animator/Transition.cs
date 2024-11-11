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
        /// <summary>
        /// TransitionBoard中,State的临时名称
        /// </summary>
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
        /// <summary>
        /// 创建动画板,基于 [ 实例 ],占用更多资源
        /// </summary>
        public static TransitionBoard<T> CreateBoardFromObject<T>(T target) where T : class
        {
            var result = new TransitionBoard<T>(target);
            result.IsStatic = true;
            return result;
        }
        /// <summary>
        /// 创建动画板,基于 [ 类型 ],占用少量资源
        /// </summary>
        public static TransitionBoard<T> CreateBoardFromType<T>() where T : class
        {
            return new TransitionBoard<T>() { IsStatic = true };
        }
        /// <summary>
        /// 终止所有过渡
        /// </summary>
        public static void Dispose()
        {
            foreach (var machine in StateMachine.MachinePool.Values)
            {
                machine.Interpreter?.Interrupt();
                foreach (var intor in machine.UnSafeInterpreters)
                {
                    intor.Interrupt(true);
                }
            }
        }
        /// <summary>
        /// 终止指定对象的所有过渡
        /// </summary>
        /// <param name="targets"></param>
        public static void Stop(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                machine.Interpreter?.Interrupt();
                foreach (var itor in machine.UnSafeInterpreters)
                {
                    itor.Interrupt(true);
                }
            }
        }
        /// <summary>
        /// 打断指定对象的Safe过渡
        /// </summary>
        /// <param name="targets">被打断过渡的目标对象</param>
        public static void StopSafe(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                machine.Interpreter?.Interrupt();
            }
        }
        /// <summary>
        /// 打断指定对象的UnSafe过渡
        /// </summary>
        /// <param name="targets">被打断过渡的目标对象</param>
        public static void StopUnSafe(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                foreach (var itor in machine.UnSafeInterpreters)
                {
                    itor.Interrupt(true);
                }
            }
        }
    }
}
