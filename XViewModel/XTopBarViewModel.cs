using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class XTopBarViewModel : INotifyPropertyChanged
    {
        private XTopBarModel _topBarModel;

        public XTopBarViewModel()
        {
            _topBarModel = new XTopBarModel();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title
        {
            get => _topBarModel.Title;
            set
            {
                if (_topBarModel.Title != value)
                {
                    _topBarModel.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public SolidColorBrush TitleColor
        {
            get => _topBarModel.TitleColor;
            set
            {
                if (_topBarModel.TitleColor != value)
                {
                    _topBarModel.TitleColor = value;
                    OnPropertyChanged(nameof(TitleColor));
                }
            }
        }

        public SolidColorBrush ControlColor
        {
            get => _topBarModel.ControlColor;
            set
            {
                if (_topBarModel.ControlColor != value)
                {
                    _topBarModel.ControlColor = value;
                    OnPropertyChanged(nameof(ControlColor));
                }
            }
        }

        public SolidColorBrush ControlHoverColor
        {
            get => _topBarModel.ControlHoverColor;
            set
            {
                if (_topBarModel.ControlHoverColor != value)
                {
                    _topBarModel.ControlHoverColor = value;
                    OnPropertyChanged(nameof(ControlHoverColor));
                }
            }
        }
    }
}
