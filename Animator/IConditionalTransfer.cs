using MinimalisticWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public interface IConditionalTransfer<T> where T : class
    {
        StateMachine? StateMachine { get; set; }
        StateVector<T>? StateVector { get; set; }
        void OnConditionsChecked();
    }
}