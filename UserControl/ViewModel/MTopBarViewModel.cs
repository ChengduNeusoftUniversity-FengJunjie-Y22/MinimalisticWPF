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
        public MTopBarViewModel() { this.AsGlobalTheme(); }

        private bool _isUseBorder = false;
        public bool IsUseBorder
        {
            get => _isUseBorder;
            set
            {
                _isUseBorder = value;
                if(value)
                {
                    OnPropertyChanged(nameof(EdgeBrush));
                }
            }
        }

        public override Brush EdgeBrush { get => base.EdgeBrush; set { SetEdgeBrush(value); } }

        public ImageSource? Icon
        {
            get => Model.Icon;
            set
            {
                Model.Icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public void SetEdgeBrush(Brush brush)
        {
            if (!_isUseBorder) return;

            base.EdgeBrush = brush;
        }
    }
}
