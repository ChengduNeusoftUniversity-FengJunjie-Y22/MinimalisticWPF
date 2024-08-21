﻿using MinimalisticWPF;
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
    public interface IConditionalTransfer<T> where T : INotifyPropertyChanged
    {
        /// <summary>
        /// 状态机
        /// </summary>
        StateMachine<T>? Machine { get; set; }

        /// <summary>
        /// 通知状态机切换状态
        /// </summary>
        void SendMessage();
    }
}