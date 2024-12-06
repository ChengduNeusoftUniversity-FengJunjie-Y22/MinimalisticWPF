using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// 传递属性变动事件中的新值与旧值
    /// </summary>
    public class WatcherEventArgs : EventArgs
    {
        public WatcherEventArgs(object? oldValue, object? newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object? OldValue { get; private set; }
        public object? NewValue { get; private set; }
    }
}
