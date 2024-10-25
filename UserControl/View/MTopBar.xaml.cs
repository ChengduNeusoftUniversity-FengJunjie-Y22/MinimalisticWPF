using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinimalisticWPF
{
    public partial class MTopBar : UserControl
    {
        public MTopBar()
        {
            InitializeComponent();
        }

        public double WiseHeight
        {
            get => ViewModel.Height;
            set => ViewModel.Height = value;
        }

        public double WiseWidth
        {
            get => ViewModel.Width;
            set => ViewModel.Width = value;
        }

        public string Title
        {
            get => ViewModel.Text;
            set => ViewModel.Text = value;
        }

        public ImageSource? Icon
        {
            get => ViewModel.Icon;
            set => ViewModel.Icon = value;
        }

        public Brush TextBrush
        {
            get => ViewModel.TextBrush;
            set => ViewModel.TextBrush = value;
        }

        public double SizeRatio
        {
            get => ViewModel.FontSizeConvertRate;
            set => ViewModel.FontSizeConvertRate = value;
        }

        public Brush EdgeBrush
        {
            get => ViewModel.EdgeBrush;
            set => ViewModel.EdgeBrush = value;
        }

        public Thickness EdgeThickness
        {
            get => ViewModel.EdgeThickness;
            set => ViewModel.EdgeThickness = value;
        }

        public CornerRadius CornerRadius
        {
            get => ViewModel.CornerRadius;
            set => ViewModel.CornerRadius = value;
        }

        public Brush HoverBrush { get; set; } = Brushes.Cyan;

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void MidelSize_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void MinSize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button element)
            {
                element.Transition()
                    .SetProperty(x => x.Foreground, HoverBrush)
                    .SetParams((x) => { x.Duration = 0.4; })
                    .Start();
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button element)
            {
                element.Transition()
                    .SetProperty(x => x.Foreground, TextBrush)
                    .SetParams((x) => { x.Duration = 0.4; })
                    .Start();
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.DragMove();
        }
    }
}
