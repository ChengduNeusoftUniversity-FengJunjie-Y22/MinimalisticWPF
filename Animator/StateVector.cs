using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF
{
    /// <summary>
    /// 条件向量
    /// <para>记当前状态为StateA,目标状态为StateB,指定条件为Condition,此类型记录了对象在满足Condition时由StateA加载指向StateB动画的条件关系</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateVector<T> where T : class
    {
        internal StateVector() { }

        /// <summary>
        /// Item1 条件检查委托
        /// <para>Item2 满足条件时自动切换到的状态</para>
        /// <para>Item3 此次切换状态的过渡参数</para>
        /// </summary>
        public List<Tuple<Func<T, bool>, Func<State>, Action<TransitionParams>?>> Conditions { get; internal set; } = new List<Tuple<Func<T, bool>, Func<State>, Action<TransitionParams>?>>();

        /// <summary>
        /// 添加[Condition=>State+TransitionParams]的条件切换关系映射
        /// </summary>
        public StateVector<T> AddCondition(Expression<Func<T, bool>> condition, Func<State> targetState, Action<TransitionParams>? setTransitionParams)
        {
            var checker = condition.Compile();

            if (checker != null)
            {
                Conditions.Add(Tuple.Create(checker, targetState, setTransitionParams));
            }

            return this;
        }
        /// <summary>
        /// 启用委托对Target检测条件
        /// </summary>
        public void Check(object target, StateMachine? stateMachine)
        {
            stateMachine ??= StateMachine.Create(target);

            if (target is T value)
            {
                for (int i = 0; i < Conditions.Count; i++)
                {
                    if (Conditions[i].Item1(value))
                    {
                        var st = Conditions[i].Item2.Invoke();
                        stateMachine.States.Add(st);
                        stateMachine.Transition(st.StateName, Conditions[i].Item3);
                        return;
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"SV01 [ {target.GetType().FullName} ]与[ {nameof(T)} ]类型不一致,不允许执行条件检查");
            }
        }

        /// <summary>
        /// 开始创建条件组
        /// </summary>
        public static StateVector<T> Create()
        {
            StateVector<T> result = new StateVector<T>();
            return result;
        }
    }
}
