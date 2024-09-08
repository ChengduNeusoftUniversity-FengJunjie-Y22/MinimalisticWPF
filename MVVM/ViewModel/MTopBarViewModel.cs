using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.MVVM.ViewModel
{
    public class MTopBarViewModel : StateViewModelBase<MTopBarViewModel>
    {
        public MTopBarViewModel() { }

        public MTopBarModel Model { get; set; } = new MTopBarModel();


    }
}
