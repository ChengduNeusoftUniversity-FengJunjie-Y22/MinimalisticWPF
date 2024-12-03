using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    [Theme]
    public class ButtonVM : INotifyPropertyChanged
    {
        public ButtonVM() { this.ApplyGlobalTheme(); }

        private string _text = "MButton";
        private Brush _textbrush = DarkBrushes.Selector.Select(BrushTags.P1);
        private Brush _borderbrush = DarkBrushes.Selector.Select(BrushTags.E1);
        private Brush _background = DarkBrushes.Selector.Select(BrushTags.B1);
        private Thickness _borderthickness = new Thickness(1);
        private CornerRadius _cornerradius = new CornerRadius(5);
        private bool _isPressed = false;
        private static Action<TransitionParams> _animationparams = (p) =>
        {
            p.Duration = 0.2;
            p.FrameRate = 60;
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Text
        {
            get => _text;
            set
            {
                if (value != _text)
                {
                    _text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        [Light(BrushTags.P1, BrushTags.F1)]
        [Dark(BrushTags.P1, BrushTags.F1)]
        public Brush TextBrush
        {
            get => _textbrush;
            set
            {
                if (value != _textbrush)
                {
                    _textbrush = value;
                    OnPropertyChanged(nameof(TextBrush));
                }
            }
        }

        [Light(BrushTags.E1, BrushTags.F1)]
        [Dark(BrushTags.E1, BrushTags.F1)]
        public Brush BorderBrush
        {
            get => _borderbrush;
            set
            {
                if (value != _borderbrush)
                {
                    _borderbrush = value;
                    OnPropertyChanged(nameof(BorderBrush));
                }
            }
        }

        [Light(BrushTags.B1, BrushTags.F5)]
        [Dark(BrushTags.B1, BrushTags.F5)]
        public Brush Background
        {
            get => _background;
            set
            {
                if (_background != value)
                {
                    _background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }

        public Thickness BorderThickness
        {
            get => _borderthickness;
            set
            {
                if (value != _borderthickness)
                {
                    _borderthickness = value;
                    OnPropertyChanged(nameof(BorderThickness));
                }
            }
        }
        public CornerRadius CornerRadius
        {
            get => _cornerradius;
            set
            {
                if (value != _cornerradius)
                {
                    _cornerradius = value;
                    OnPropertyChanged(nameof(CornerRadius));
                }
            }
        }

        public bool IsPressed
        {
            get => _isPressed;
            set
            {
                if (value != _isPressed)
                {
                    _isPressed = value;
                    if (value)
                    {
                        WhileHover();
                    }
                    else
                    {
                        WhileNoHover();
                    }
                }
            }
        }
        public void OnHoverChanged()
        {
            IsPressed = !IsPressed;
        }
        public void WhileHover()
        {
            this.ApplyThemeHover(TransitionParams.Hover);
        }
        public void WhileNoHover()
        {
            this.ApplyThemeNoHover(TransitionParams.Hover);
        }
    }
}
