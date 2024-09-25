﻿using System;
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
        internal StateVector() { }

        /// <summary>
        /// Item1 条件检查委托
        /// <para>Item2 满足条件时自动切换到的状态</para>
        /// <para>Item3 此次切换状态的过渡参数</para>
        /// </summary>
        public List<Tuple<Func<T, bool>, State, TransitionParams>> Conditions { get; internal set; } = new List<Tuple<Func<T, bool>, State, TransitionParams>>();

        /// <summary>
        /// 添加[Condition=>State+TransitionParams]的条件切换关系映射
        /// </summary>
        public StateVector<T> AddCondition(Expression<Func<T, bool>> condition, State targetState, Action<TransitionParams>? setTransitionParams)
        {
            var checker = condition.Compile();

            if (checker != null)
            {
                TransitionParams tempParams = new TransitionParams();
                setTransitionParams?.Invoke(tempParams);
                Conditions.Add(Tuple.Create(checker, targetState, tempParams));
            }

            return this;
        }
        /// <summary>
        /// 启用委托对Target检测条件
        /// </summary>
        public void Check(T target, StateMachine stateMachine)
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (Conditions[i].Item1(target))
                {
                    stateMachine.Transition(Conditions[i].Item2.StateName,
                        (x) =>
                        {
                            x.Duration = Conditions[i].Item3.Duration;
                            x.IsQueue = Conditions[i].Item3.IsQueue;
                            x.IsLast = Conditions[i].Item3.IsLast;
                            x.IsUnique = Conditions[i].Item3.IsUnique;
                            x.WaitTime = Conditions[i].Item3.WaitTime;
                            x.Start = Conditions[i].Item3.Start;
                            x.Update = Conditions[i].Item3.Update;
                            x.LateUpdate = Conditions[i].Item3.LateUpdate;
                            x.Completed = Conditions[i].Item3.Completed;
                            x.Acceleration = Conditions[i].Item3.Acceleration;
                        });
                    return;
                }
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
