using System;
using System.Collections.Generic;
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
    public partial class XTopBar : UserControl
    {
        public XTopBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
                ViewModel.Title = value;
            }
        }

        /// <summary>
        /// 标题颜色
        /// </summary>
        public SolidColorBrush TitleColor
        {
            get { return (SolidColorBrush)GetValue(TitleColorProperty); }
            set
            {
                SetValue(TitleColorProperty, value);
                ViewModel.TitleColor = value;
            }
        }

        /// <summary>
        /// 右侧控制器的文本色
        /// </summary>
        public SolidColorBrush ControlColor
        {
            get { return (SolidColorBrush)GetValue(ControlColorProperty); }
            set
            {
                SetValue(ControlColorProperty, value);
                ViewModel.ControlColor = value;
            }
        }

        /// <summary>
        /// 右侧控制器的悬停文本色
        /// </summary>
        public SolidColorBrush ControlHoverColor
        {
            get { return (SolidColorBrush)GetValue(ControlHoverColorProperty); }
            set
            {
                SetValue(ControlHoverColorProperty, value);
                ViewModel.ControlHoverColor = value;
            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(XTopBar), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TitleColorProperty =
            DependencyProperty.Register(nameof(TitleColor), typeof(SolidColorBrush), typeof(XTopBar), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty ControlColorProperty =
            DependencyProperty.Register(nameof(ControlColor), typeof(SolidColorBrush), typeof(XTopBar), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty ControlHoverColorProperty =
            DependencyProperty.Register(nameof(ControlHoverColor), typeof(SolidColorBrush), typeof(XTopBar), new PropertyMetadata(Brushes.Transparent));

        private void WindowToDragMove(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.DragMove();
        }

        private void WindowToMini(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = (Application.Current.MainWindow.WindowState == WindowState.Minimized) ? WindowState.Maximized : WindowState.Minimized;
        }

        private void WindowToScale(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = (Application.Current.MainWindow.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void WindowToClose(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
