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
    public class MPasswordBoxViewModel : ViewModelBase<MPasswordBoxViewModel, MPasswordBoxModel>
    {
        public MPasswordBoxViewModel() { }

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

        public static string[] protects = new string[] { "TextSize", "TextBrush", "BackBrush","Width","Height", "FontSizeConvertRate" };

        public static StateVector<MPasswordBoxViewModel> ConditionA = StateVector<MPasswordBoxViewModel>.Create(new MPasswordBoxViewModel())
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 0, Default, (x) => { x.Duration = 0.1; x.ProtectNames = protects; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 1, Level1, (x) => { x.Duration = 0.1; x.ProtectNames = protects; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 2, Level2, (x) => { x.Duration = 0.1; x.ProtectNames = protects; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 3, Level3, (x) => { x.Duration = 0.1; x.ProtectNames = protects; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 4, Level4, (x) => { x.Duration = 0.1; x.ProtectNames = protects; });

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
    }
}
