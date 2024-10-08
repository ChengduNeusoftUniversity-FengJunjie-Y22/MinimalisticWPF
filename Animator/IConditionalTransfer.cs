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
    /// 接入状态机的条件切换功能
    /// <para>使用场景:</para>
    /// <para>1.ViewModel实现此接口,然后在控件初始化时调用FrameworkElement.StateMachineLoading( )</para>
    /// <para>2.任何类型可实现此接口,初始化时手动赋予StateMachine和StateVector ( StateMachine需要手动使用StateMachine.Create()创建 )</para>
    /// </summary>
    /// <typeparam name="T">具体实现此接口的类型</typeparam>
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