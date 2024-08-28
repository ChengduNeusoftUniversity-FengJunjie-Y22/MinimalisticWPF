using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Collections;
using System.Windows.Input;

namespace MinimalisticWPF
{
    public class MPasswordBoxViewModel : StateViewModelBase<MPasswordBoxViewModel>
    {
        public MPasswordBoxViewModel() { }

        public MPasswordBoxModel Model { get; set; } = new MPasswordBoxModel();

        public static State Default = State.FromObject(new MPasswordBoxViewModel())
            .SetName("default")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.White)
            .ToState();
        public static State Level1 = State.FromObject(new MPasswordBoxViewModel())
            .SetName("L1")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Tomato)
            .ToState();
        public static State Level2 = State.FromObject(new MPasswordBoxViewModel())
            .SetName("L2")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Yellow)
            .ToState();
        public static State Level3 = State.FromObject(new MPasswordBoxViewModel())
            .SetName("L3")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Cyan)
            .ToState();
        public static State Level4 = State.FromObject(new MPasswordBoxViewModel())
            .SetName("L4")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Lime)
            .ToState();

        public static StateVector<MPasswordBoxViewModel> ConditionA = StateVector<MPasswordBoxViewModel>.Create(new MPasswordBoxViewModel())
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 0, Default, (x) => { x.Duration = 0.1; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 1, Level1, (x) => { x.Duration = 0.1; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 2, Level2, (x) => { x.Duration = 0.1; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 3, Level3, (x) => { x.Duration = 0.1; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 4, Level4, (x) => { x.Duration = 0.1; });

        public string TruePassword
        {
            get => Model.TruePassword;
            set
            {
                Model.TruePassword = value;
                string result = string.Empty;
                for (int i = 0; i < value.Length; i++)
                {
                    result += ReplacingCharacters;
                }
                UIPassword = result;
                OnPropertyChanged(nameof(TruePassword));
                OnConditionsChecked();
            }
        }

        public string UIPassword
        {
            get => Model.UIPassword;
            set
            {
                Model.UIPassword = value;
                OnPropertyChanged(nameof(UIPassword));
            }
        }

        public string ReplacingCharacters
        {
            get => Model.ReplacingCharacters;
            set
            {
                Model.ReplacingCharacters = value;
                string result = string.Empty;
                for (int i = 0; i < TruePassword.Length; i++)
                {
                    result += ReplacingCharacters;
                }
                UIPassword = result;
                OnPropertyChanged(nameof(ReplacingCharacters));
            }
        }

        public Brush PasswordStrengthColor
        {
            get => Model.PasswordStrengthColor;
            set
            {
                Model.PasswordStrengthColor = value;
                OnPropertyChanged(nameof(PasswordStrengthColor));
            }
        }

        public Brush FixedTransparent
        {
            get => Model.FixedTransparent;
            set
            {
                OnPropertyChanged(nameof(FixedTransparent));
            }
        }

        public double Height
        {
            get => Model.Height;
            set
            {
                Model.Height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public double Width
        {
            get => Model.Width;
            set
            {
                Model.Width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public double FontSizeConvertRate
        {
            get => Model.FontSizeConvertRate;
            set
            {
                Model.FontSizeConvertRate = value;
                OnPropertyChanged(nameof(FontSizeConvertRate));
            }
        }

        public double FontSize
        {
            get => Model.Height * Model.FontSizeConvertRate;
            set
            {
                Model.FontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        public Brush Foreground
        {
            get => Model.Foreground;
            set
            {
                Model.Foreground = value;
                OnPropertyChanged(nameof(Foreground));
            }
        }

        public CornerRadius CornerRadius
        {
            get => Model.CornerRadius;
            set
            {
                Model.CornerRadius = value;
                OnPropertyChanged(nameof(CornerRadius));
            }
        }

        public Thickness FixedBorderThickness
        {
            get => Model.FixedBorderThickness;
            set
            {
                Model.FixedBorderThickness = value;
                OnPropertyChanged(nameof(FixedBorderThickness));
            }
        }

        public Brush FixedBorderBrush
        {
            get => Model.FixedBorderBrush;
            set
            {
                Model.FixedBorderBrush = value;
                OnPropertyChanged(nameof(FixedBorderBrush));
            }
        }

        public Brush ActualBackground
        {
            get => Model.ActualBackground;
            set
            {
                Model.ActualBackground = value;
                OnPropertyChanged(nameof(ActualBackground));
            }
        }

        public double ActualBackgroundOpacity
        {
            get => Model.ActualBackgroundOpacity;
            set
            {
                Model.ActualBackgroundOpacity = value;
                OnPropertyChanged(nameof(ActualBackgroundOpacity));
            }
        }
    }
}
