using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class XButtonViewModel : INotifyPropertyChanged
    {
        private XButtonModel _buttonModel;

        public XButtonViewModel() { _buttonModel = new XButtonModel(); }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Text
        {
            get => _buttonModel.Text;
            set
            {
                if (value != _buttonModel.Text)
                {
                    _buttonModel.Text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public SolidColorBrush TextColor
        {
            get => _buttonModel.TextColor;
            set
            {
                if (value != _buttonModel.TextColor)
                {
                    _buttonModel.TextColor = value;
                    OnPropertyChanged(nameof(TextColor));
                }
            }
        }

        public SolidColorBrush TextHoverColor
        {
            get => _buttonModel.TextHoverColor;
            set
            {
                if (value != _buttonModel.TextHoverColor)
                {
                    _buttonModel.TextHoverColor = value;
                    OnPropertyChanged(nameof(TextColor));
                }
            }
        }

        public SolidColorBrush Background
        {
            get => _buttonModel.Background;
            set
            {
                if (value != _buttonModel.Background)
                {
                    _buttonModel.Background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }
    }
}
