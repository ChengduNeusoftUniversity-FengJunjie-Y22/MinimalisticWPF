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
    /// 为ViewModel接入状态机的条件切换功能
    /// <para>步骤:</para>
    /// <para>1.使控件的ViewModel实现此接口</para>
    /// <para>2.在ViewModel中定义好State与StateVector</para>
    /// <para>3.控件初始化时调用FrameworkElement.StateMachineLoading(ViewModel)</para>
    /// </summary>
    /// <typeparam name="T">具体实现此接口的类型</typeparam>
    public interface IConditionalTransfer<T> where T : class
    {
        StateMachine? StateMachine { get; set; }
        StateVector<T>? StateVector { get; set; }
        /// <summary>
        /// 检查当前实例是否满足StateVector描述的条件,并执行首个满足条件的状态切换
        /// </summary>
        void OnConditionsChecked();
    }
}