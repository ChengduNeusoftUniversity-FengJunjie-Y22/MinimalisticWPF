using MinimalisticWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// 接入条件切换
    /// </summary>
    /// <typeparam name="T">实现此接口的类型</typeparam>
    public interface IConditionalTransition<T> where T : class
    {
        StateMachine? StateMachine { get; set; }
        StateVector<T>? StateVector { get; set; }
        /// <summary>
        /// 检查当前实例是否满足StateVector描述的条件,并执行首个满足条件的状态切换
        /// </summary>
        void OnConditionsChecked();
    }
}