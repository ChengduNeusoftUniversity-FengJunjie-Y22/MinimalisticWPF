using System.Drawing;
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

        public string Text
        {
            get => ViewModel.Text;
            set => ViewModel.Text = value;
        }

        public double FontSizeRatio
        {
            get => ViewModel.FontSizeConvertRate;
            set => ViewModel.FontSizeConvertRate = value;
        }

        public Brush TextBrush
        {
            get => ViewModel.Foreground;
            set => ViewModel.Foreground = value;
        }

        public Brush HoverBrush
        {
            get => ViewModel.ActualBackground;
            set => ViewModel.ActualBackground = value;
        }

        public Brush EdgeBrush
        {
            get => ViewModel.FixedBorderBrush;
            set => ViewModel.FixedBorderBrush = value;
        }

        public Thickness EdgeThickness
        {
            get => ViewModel.FixedBorderThickness;
            set => ViewModel.FixedBorderThickness = value;
        }

        public CornerRadius CornerRadius
        {
            get => ViewModel.CornerRadius;
            set => ViewModel.CornerRadius = value;
        }

        public void BackgroundBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.StateMachine?.Transfer("mouseInside", (x) => { x.Duration = 0.4; x.ProtectNames = new string[] { "Foreground", "ActualBackground", "FixedBorderBrush" }; });
        }

        public void BackgroundBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.StateMachine?.Transfer("defualt", (x) => { x.Duration = 0.1; x.ProtectNames = new string[] { "Foreground", "ActualBackground", "FixedBorderBrush" }; });
        }
    }
}
