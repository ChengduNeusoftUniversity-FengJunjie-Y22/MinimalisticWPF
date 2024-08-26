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
    /// [可用StateMachine + StateVector更改当前State]的 ViewModel
    /// </summary>
    public abstract class StateViewModelBase<T> : INotifyPropertyChanged, IConditionalTransfer<T> where T : class
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));         
        }

        public StateMachine? StateMachine { get; set; }
        public StateVector<T>? StateVector { get; set; }
        public void OnConditionsChecked()
        {
            StateVector?.Check();
        }
    }
}
