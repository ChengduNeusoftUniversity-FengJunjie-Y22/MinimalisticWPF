using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MinimalisticWPF
{
    public partial class MButton : UserControl
    {
        private StateMachine Machine { get; set; }

        static State MouseOver = State.FromObject(new MButtonViewModel())
            .SetName("mouseover")
            .SetProperty(x => x.ActualBackgroundOpacity, 0.4)
            .ToState();

        public MButton()
        {
            InitializeComponent();

            State Source = State.FromObject(ViewModel)
                .SetName("defualt")
                .ToState();


            Machine = new StateMachine(ViewModel, Source, MouseOver);
            ViewModel.Machine = Machine;
        }

        public event MouseButtonEventHandler? Click
        {
            add { BackgroundBorder.PreviewMouseLeftButtonDown += value; }
            remove { BackgroundBorder.PreviewMouseLeftButtonDown -= value; }
        }

        public void BackgroundBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Machine.Transfer("mouseover", 0.35);
        }

        public void BackgroundBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Machine.Transfer("defualt", 0.15);
        }
    }
}
