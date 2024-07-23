using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class XButtonViewModel : XViewModelBase
    {
        private XButtonModel _buttonModel;

        private SolidColorBrush TempColor { get; set; } = Brushes.Transparent;

        public XButtonViewModel()
        {
            _buttonModel = new XButtonModel();
        }

        #region 属性

        public SolidColorBrush FixedTransparent
        {
            get => _buttonModel.FixedTransparent;
            set
            {
                OnPropertyChanged(nameof(FixedTransparent));
            }
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

        public SolidColorBrush BorderBrush
        {
            get => _buttonModel.BorderBrush;
            set
            {
                if (_buttonModel.BorderBrush != value)
                {
                    _buttonModel.BorderBrush = value;
                    OnPropertyChanged(nameof(BorderBrush));
                }
            }
        }

        public Thickness BorderThickness
        {
            get => _buttonModel.BorderThickness;
            set
            {
                if (value != _buttonModel.BorderThickness)
                {
                    _buttonModel.BorderThickness = value;
                    OnPropertyChanged(nameof(BorderThickness));
                }
            }
        }

        public SolidColorBrush AnimationFill
        {
            get => _buttonModel.AnimationFill;
            set
            {
                if (value != _buttonModel.AnimationFill)
                {
                    _buttonModel.AnimationFill = value;
                    OnPropertyChanged(nameof(AnimationFill));
                }
            }
        }

        public SolidColorBrush BorderHoverBrush
        {
            get => _buttonModel.BorderHoverBrush;
            set
            {
                if (value != _buttonModel.BorderHoverBrush)
                {
                    _buttonModel.BorderHoverBrush = value;
                    OnPropertyChanged(nameof(BorderHoverBrush));
                }
            }
        }

        public CornerRadius CornerRadius
        {
            get => _buttonModel.CornerRadius;
            set
            {
                if (value != _buttonModel.CornerRadius)
                {
                    _buttonModel.CornerRadius = value;
                    OnPropertyChanged(nameof(CornerRadius));
                }
            }
        }

        public SolidColorBrush HoverFill
        {
            get => _buttonModel.HoverFill;
            set
            {
                if (value != _buttonModel.HoverFill)
                {
                    _buttonModel.HoverFill = value;
                    OnPropertyChanged(nameof(HoverFill));
                }
            }
        }

        #endregion
    }
}
