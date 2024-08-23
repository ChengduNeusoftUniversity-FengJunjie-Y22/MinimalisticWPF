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
    /// [ 可用StateMachine + StateVector 更改当前State ] 的ViewModel
    /// </summary>
    public abstract class StateViewModelBase : INotifyPropertyChanged, IConditionalTransfer
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        IConditionalTransfer? Local = default;
        public StateMachine? Machine { get; set; }
        public List<StateVector> Conditions { get; set; } = new List<StateVector>();
        public void OnConditionChecked()
        {
            if (Local == null) Local = this;
            Local.OnConditionsChecked();
        }
    }
}
