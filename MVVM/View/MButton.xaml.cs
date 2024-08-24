using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MinimalisticWPF
{
    public partial class MButton : UserControl
    {
        private StateMachine Machine { get; set; }

        static State Start = State.FromObject(new MButtonViewModel())
            .SetName("defualt")
            .ToState();
        static State MouseIn = State.FromObject(new MButtonViewModel())
            .SetName("mouseInside")
            .SetProperty(x => x.ActualBackgroundOpacity, 0.4)
            .ToState();

        static StateVector mECondition = StateVector.FromType<MButtonViewModel>()
            .SetName("ToMInside")
            .SetTarget(MouseIn)
            .SetCondition(x => x.Text.Length > 10)
            .SetTransferParams()
            .ToStateVector();

        public MButton()
        {
            InitializeComponent();

            Machine = StateMachine.Create(ViewModel)
                .SetStates(Start, MouseIn)
                .SetConditions(mECondition);
        }

        public event MouseButtonEventHandler? Click
        {
            add { BackgroundBorder.PreviewMouseLeftButtonDown += value; }
            remove { BackgroundBorder.PreviewMouseLeftButtonDown -= value; }
        }

        public void BackgroundBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Machine.Transfer("mouseInside", (x) => x.Duration = 0.2);
        }

        public void BackgroundBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Machine.Transfer("defualt", (x) => x.Duration = 0.2);
        }
    }
}
