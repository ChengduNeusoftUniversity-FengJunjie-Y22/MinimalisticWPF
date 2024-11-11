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

        public static void BeginTransition<T>(this T source, TransitionBoard<T> transfer) where T : class
        {
            transfer.Start(source);
        }
        public static void BeginTransition<T>(this T source, TransitionBoard<T> transfer, Action<TransitionParams> set) where T : class
        {
            transfer.TransitionParams = set;
            transfer.Start(source);
        }
        public static void BeginTransition<T>(this T source, State state, Action<TransitionParams> set) where T : class
        {
            if (state == null) return;
            if (state.ActualType != typeof(T)) throw new ArgumentException("State does not match the type of the object");
            var machine = source.FindStateMachine();
            if (machine == null)
            {
                var newMachine = StateMachine.Create(source);
                newMachine.States.Add(state);
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
        /// 尝试找出系统中管理该对象过渡效果的状态机实例,注意该方法不会在不存在状态机实例时自动创建状态机
        /// <para>第一优先级 : TransitionBoard 对象池（字典）内存储的状态机</para>
        /// <para>第二优先级 : 实例对象自身包含的属性</para>
        /// <para>第三优先级 : 若为一个FrameworkElement，尝试从其DataContext中获取状态机</para>
        /// </summary>
        public static StateMachine? FindStateMachine<T>(this T source) where T : class
        {
            StateMachine.MachinePool.TryGetValue(source, out var machineA);
            if (machineA != null) return machineA;


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
