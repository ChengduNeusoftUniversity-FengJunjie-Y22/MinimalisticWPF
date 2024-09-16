﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public class TransferParams
    {
        internal TransferParams() { }

        internal TransferParams(Action<TransferParams>? action)
        {
            action?.Invoke(this);
        }

        /// <summary>
        /// 启动动画前需要做的事情
        /// </summary>
        public Action? Start { get; set; }
        /// <summary>
        /// 每个更新帧需要做的事情
        /// </summary>
        public Action? Update { get; set; }
        /// <summary>
        /// 持续时长(单位: s )
        /// </summary>
        public double Duration { get; set; } = 0;
        /// <summary>
        /// 过渡帧率(默认: 165 )
        /// </summary>
        public int FrameRate { get; set; } = 165;
        /// <summary>
        /// 是否排队执行(默认:不排队)
        /// </summary>
        public bool IsQueue { get; set; } = false;
        /// <summary>
        /// 是否在执行完后,清除其它排队中的过渡效果(默认: 不清除 )
        /// </summary>
        public bool IsLast { get; set; } = false;
        /// <summary>
        /// 申请进入执行队列时,若已有同名切换操作处于列队中,是否继续加入队列(默认:不加入)
        /// </summary>
        public bool IsUnique { get; set; } = true;
        /// <summary>
        /// [ 测试参数 ] 若无法响应例如MouseLeave事件,可适当增加此参数(默认:0.008)
        /// </summary>
        public double WaitTime { get; set; } = 0.008;
        /// <summary>
        /// 动画完成后需要做的事情
        /// </summary>
        public Action? Completed { get; set; }
    }
}
