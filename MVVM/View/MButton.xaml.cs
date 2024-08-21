using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MinimalisticWPF
{
    public partial class MButton : UserControl
    {
        private StateMachine<MButtonViewModel> Machine { get; set; }

        static MButtonViewModel MO = new MButtonViewModel()
        {
            ActualBackgroundOpacity = 0.4
        };

        public MButton()
        {
            InitializeComponent();

            State Source = new State("default", ViewModel);
            State MouseOver = new State("mouseover", MO);

            Machine = new StateMachine<MButtonViewModel>(ViewModel, Source, MouseOver);
            ViewModel.Machine = Machine;
        }

        public event MouseButtonEventHandler? Click
        {
            add { BackgroundBorder.PreviewMouseLeftButtonDown += value; }
            remove { BackgroundBorder.PreviewMouseLeftButtonDown -= value; }
        }

        private void BackgroundBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Machine.Transfer("mouseover", 0.3, isQueue: false, isLast: true);
        }

        private void BackgroundBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Machine.Transfer("default", 0.3, isQueue: false, isLast: true);
        }
    }
}
