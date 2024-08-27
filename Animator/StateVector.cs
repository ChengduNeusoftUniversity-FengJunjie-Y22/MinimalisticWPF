using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF
{
    public class StateVector<T> where T : class
    {
        internal StateVector(T target) { Target = target; }

        /// <summary>
        /// 与条件模块相连接的状态机
        /// </summary>
        public StateMachine? Machine { get; set; }
        /// <summary>
        /// 条件判断基于对此对象属性值的比对
        /// </summary>
        public T Target { get; set; }
        /// <summary>
        /// Item1 条件检查委托
        /// <para>Item2 满足条件时自动切换到的状态</para>
        /// <para>Item3 此次切换状态的过渡参数</para>
        /// </summary>
        public List<Tuple<Func<T, bool>, State, TransferParams>> Conditions { get; internal set; } = new List<Tuple<Func<T, bool>, State, TransferParams>>();

        /// <summary>
        /// 添加[Condition=>State+TransferParams]的条件切换关系映射
        /// </summary>
        public StateVector<T> AddCondition(Expression<Func<T, bool>> condition, State targetState, Action<TransferParams>? setTransferParams)
        {
            var checker = condition.Compile();

            if (checker != null)
            {
                TransferParams tempParams = new TransferParams();
                setTransferParams?.Invoke(tempParams);
                Conditions.Add(Tuple.Create(checker, targetState, tempParams));
            }

            return this;
        }
        /// <summary>
        /// 启用委托对Target检测条件
        /// </summary>
        public void Check()
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (Conditions[i].Item1(Target))
                {
                    Machine?.Transfer(Conditions[i].Item2.StateName,
                        (x) =>
                        {
                            x.Duration = Conditions[i].Item3.Duration;
                            x.IsQueue = Conditions[i].Item3.IsQueue;
                            x.IsLast = Conditions[i].Item3.IsLast;
                            x.IsUnique = Conditions[i].Item3.IsUnique;
                            x.WaitTime = Conditions[i].Item3.WaitTime;
                            x.ProtectNames = Conditions[i].Item3.ProtectNames;
                        });
                    return;
                }
            }
        }

        /// <summary>
        /// 开始创建条件组
        /// </summary>
        public static StateVector<T> Create(T target)
        {
            StateVector<T> result = new StateVector<T>(target);
            return result;
        }
    }
}
