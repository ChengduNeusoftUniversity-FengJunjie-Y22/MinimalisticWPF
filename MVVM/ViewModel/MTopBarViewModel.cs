using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF
{
    public class MTopBarViewModel : ViewModelBase<MTopBarViewModel, MTopBarModel>
    {
        public MTopBarViewModel() { }



        public ImageSource? Icon
        {
            get => Model.Icon;
            set
            {
                Model.Icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
    }
}
