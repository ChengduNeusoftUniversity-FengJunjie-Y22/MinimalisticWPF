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
    /// <summary>
    /// 作为密码框的DataContext
    /// </summary>
    public class MPasswordBoxViewModel : ViewModelBase<MPasswordBoxViewModel, MPasswordBoxModel>
    {
        public MPasswordBoxViewModel() { }

        //默认颜色
        public static State Default = State.FromType<MPasswordBoxViewModel>()
            .SetName("default")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.White)
            .ToState();

        //密码强度有四个分级，对应四个不同的颜色
        public static State Level1 = State.FromType<MPasswordBoxViewModel>()
            .SetName("L1")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Tomato)
            .ToState();
        public static State Level2 = State.FromType<MPasswordBoxViewModel>()
            .SetName("L2")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Yellow)
            .ToState();
        public static State Level3 = State.FromType<MPasswordBoxViewModel>()
            .SetName("L3")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Cyan)
            .ToState();
        public static State Level4 = State.FromType<MPasswordBoxViewModel>()
            .SetName("L4")
            .SetProperty(x => x.PasswordStrengthColor, Brushes.Lime)
            .ToState();

        //达到指定密码强度时，切换至指定的State
        public StateVector<MPasswordBoxViewModel> Condition { get; set; } = StateVector<MPasswordBoxViewModel>.Create()
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 0, Default, (x) => { x.Duration = 0.3; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 1, Level1, (x) => { x.Duration = 0.3; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 2, Level2, (x) => { x.Duration = 0.3; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 3, Level3, (x) => { x.Duration = 0.3; })
            .AddCondition(x => x.TruePassword.CheckPasswordStrength(8) == 4, Level4, (x) => { x.Duration = 0.3; });

        //真实密码
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
                //由IConditionalTransition接口规定的方法
                //StateViewModelBase是实现MVVM并且接入状态机系统【条件切换】功能的最小实现单元,它帮你实现了INotifyPropertyChanged与IConditionalTransition接口
                //调用这句话即可在密码改变时,检测密码强度并执行动画
            }
        }

        /// <summary>
        /// 用户视觉可见的密码
        /// </summary>
        public string UIPassword
        {
            get => Model.UIPassword;
            set
            {
                Model.UIPassword = value;
                OnPropertyChanged(nameof(UIPassword));
            }
        }

        /// <summary>
        /// 用于替换真实密码的字符
        /// </summary>
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

        /// <summary>
        /// 密码强度对应的边框颜色
        /// </summary>
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
