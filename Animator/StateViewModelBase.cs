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
    /// 可用StateMachine更改当前State的ViewModel,且支持状态机的条件切换功能
    /// </summary>
    public abstract class StateViewModelBase<T> : INotifyPropertyChanged, IConditionalTransfer<T> where T : class
    {
        public StateMachine<T>? Machine { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SendMessage()
        {

        }
    }
}
