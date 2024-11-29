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
    [Theme]
    public class ButtonVM : INotifyPropertyChanged
    {
        public ButtonVM() { this.RunWithGlobalTheme(); }

        private string _text = "MButton";
        private Brush _textbrush = Brushes.White;
        private Brush _borderbrush = Brushes.White;
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

        public StateMachine? StateMachine { get; set; }
        public StateVector<ButtonVM>? StateVector { get; set; } = new StateVector<ButtonVM>();
        public void OnConditionsChecked()
        {
            StateVector?.Check(this, StateMachine);
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
        [Light("#1e1e1e")]
        [Dark(nameof(Brushes.White))]
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
        [Light(LightBrushPackage.H1)]
        [Dark(DarkBrushPackage.H1)]
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
                    OnConditionsChecked();
                }
            }
        }

        public void OnHoverChanged()
        {
            IsPressed = !IsPressed;
        }
        public void WhileHover()
        {
            this.Transition()
                .SetProperty(x => x.TextBrush, TextBrush.ToRGB().Delta(100, 0, 100, 0).Brush)
                .SetParams(TransitionParams.Theme)
                .Start();
        }
        public void WhileNoHover()
        {
            this.Transition()
                .SetProperty(x => x.TextBrush, TextBrush.ToRGB().Delta(-100, 0, -100, 0).Brush)
                .SetParams(TransitionParams.Theme)
                .Start();
        }
    }
}
