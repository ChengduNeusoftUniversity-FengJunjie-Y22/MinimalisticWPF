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
    /// 可设置条件组以自动转移状态的
    /// </summary>
    public interface IConditionalTransfer
    {
        /// <summary>
        /// 状态机
        /// </summary>
        StateMachine? Machine { get; set; }

        /// <summary>
        /// 条件组
        /// </summary>
        List<StateVector> Conditions { get; set; }

        /// <summary>
        /// 通知状态机切换状态
        /// </summary>
        void OnConditionsChecked()
        {
            if (Machine == null) return;
            foreach (var kvp in Conditions)
            {
                if (kvp.Condition == null) continue;
                if (kvp.Condition(Machine.Target))
                {
                    Machine.Transfer(kvp.StateName,
                        kvp.TransferParams.TransitionTime,
                        kvp.TransferParams.IsQueue,
                        kvp.TransferParams.IsLast,
                        kvp.TransferParams.IsUnique,
                        kvp.TransferParams.FrameRate,
                        kvp.TransferParams.WaitTime,
                        kvp.TransferParams.ProtectNames);
                    return;
                }
            }
        }
    }
}
