using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class ExtensionForAnyClass
    {
        /// <summary>
        /// 开始创建过渡效果,可直接启动 ( IsStatic == false )
        /// </summary>
        public static TransitionBoard<T> Transition<T>(this T element) where T : class
        {
            TransitionBoard<T> tempStoryBoard = new TransitionBoard<T>(element);
            return tempStoryBoard;
        }

        public static bool IsSatisfy<T>(this T target, Expression<Func<T, bool>> condition) where T : class
        {
            var checker = condition.Compile();
            if (checker == null) return false;
            return checker(target);
        }
        public static bool IsSatisfy<T>(this T target, Expression<Func<T, bool>> condition, TransitionBoard<T> transfer, Action<TransitionParams> set) where T : class
        {
            var checker = condition.Compile();
            if (checker == null) return false;
            var result = checker(target);
            if (result) target.BeginTransition(transfer, set);
            return result;
        }
        public static bool IsSatisfy<T>(this T target, Expression<Func<T, bool>> condition, TransitionBoard<T> transfer) where T : class
        {
            var checker = condition.Compile();
            if (checker == null) return false;
            var result = checker(target);
            if (result) target.BeginTransition(transfer);
            return result;
        }
        public static bool IsSatisfy<T>(this T target, Expression<Func<T, bool>> condition, State state, Action<TransitionParams> set) where T : class
        {
            var checker = condition.Compile();
            if (checker == null) return false;
            var result = checker(target);
            if (result) target.BeginTransition(state, set);
            return result;
        }

        /// <summary>
        /// 启动过渡
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="transfer">动画效果</param>
        public static void BeginTransition<T>(this T source, TransitionBoard<T> transfer) where T : class
        {
            transfer.Start(source);
        }
        /// <summary>
        /// 启动过渡
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="transfer">动画效果</param>
        /// <param name="set">效果参数</param>
        public static void BeginTransition<T>(this T source, TransitionBoard<T> transfer, Action<TransitionParams> set) where T : class
        {
            transfer.TransitionParams = set;
            transfer.Start(source);
        }
        /// <summary>
        /// 启动过渡
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="state">状态信息</param>
        /// <param name="set">效果参数</param>
        /// <exception cref="ArgumentException"></exception>
        public static void BeginTransition<T>(this T source, State state, Action<TransitionParams> set) where T : class
        {
            if (state == null) return;
            var machine = source.FindStateMachine();
            if (machine == null)
            {
                var newMachine = StateMachine.Create(source, state);
                newMachine.Transition(state.StateName, set);
            }
            else
            {
                machine.ReSet();
                machine.States.Add(state);
                machine.Transition(state.StateName, set);
            }
        }

        /// <summary>
        /// 打断过渡
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsStopSafe">是否停止Safe过渡</param>
        /// <param name="IsStopUnSafe">是否停止UnSafe过渡</param>
        public static T StopTransition<T>(this T source, bool IsStopSafe = true, bool IsStopUnSafe = false) where T : class
        {
            var machine = StateMachine.Create(source);
            if (IsStopSafe)
            {
                machine.Interpreter?.Interrupt();
            }
            if (IsStopUnSafe)
            {
                foreach (var itor in machine.UnSafeInterpreters)
                {
                    itor.Interrupt(true);
                }
            }
            return source;
        }

        /// <summary>
        /// 尝试找出系统中管理该对象过渡效果的状态机实例,注意该方法不会在不存在状态机实例时自动创建状态机
        /// <para>第一优先级 : StateMachine 对象池（字典）内存储的状态机</para>
        /// <para>第二优先级 : 实例对象自身包含的属性</para>
        /// <para>第三优先级 : 若为一个FrameworkElement，尝试从其DataContext中获取状态机</para>
        /// </summary>
        public static StateMachine? FindStateMachine<T>(this T source) where T : class
        {
            if (StateMachine.TryGetMachine(source, out var machineA))
            {
                return machineA;
            }

            var Incodebehind = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.CanRead && x.CanWrite && x.PropertyType == typeof(StateMachine));
            if (Incodebehind != null)
            {
                return Incodebehind.GetValue(source) as StateMachine;
            }

            if (source is FrameworkElement element)
            {
                if (element.DataContext == null) return null;
                var Inviewmodel = element.DataContext.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(x => x.CanRead && x.CanWrite && x.PropertyType == typeof(StateMachine));
                if (Inviewmodel != null)
                {
                    return Inviewmodel.GetValue(element.DataContext) as StateMachine;
                }
            }

            return null;
        }
    }
}
