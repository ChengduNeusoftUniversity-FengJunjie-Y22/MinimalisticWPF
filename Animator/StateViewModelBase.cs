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
    /// MVVM下,使ViewModel支持状态机的示例写法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StateViewModelBase<T> : INotifyPropertyChanged, IConditionalTransfer<T> where T : class
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public T? Target { get; set; }
        public StateMachine? StateMachine { get; set; }
        public StateVector<T>? StateVector { get; set; }
        public void OnConditionsChecked()
        {
            if (Target == null)
            {
                Target = this as T;
            }
            if (StateVector != null && StateMachine != null && Target != null)
            {
                StateVector?.Check(Target, StateMachine);
            }
        }
    }
}
