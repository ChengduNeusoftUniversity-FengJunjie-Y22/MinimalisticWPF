using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public partial class MButton : UserControl
    {
        public MButton()
        {
            InitializeComponent();
            this.StateMachineLoading(ViewModel);
        }

        public event MouseButtonEventHandler? Click
        {
            add { BackgroundBorder.PreviewMouseLeftButtonDown += value; }
            remove { BackgroundBorder.PreviewMouseLeftButtonDown -= value; }
        }

        public void BackgroundBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.StateMachine?.Transfer("mouseInside", (x) => x.Duration = 0.2);
        }

        public void BackgroundBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.StateMachine?.Transfer("defualt", (x) => x.Duration = 0.1);
        }
    }
}
