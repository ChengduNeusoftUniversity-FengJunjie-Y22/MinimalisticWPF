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
    public partial class MButton : Button
    {
        public MButton()
        {
            InitializeComponent();
        }

        public object Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(object), typeof(MButton), new PropertyMetadata(string.Empty, OnTextChanged));
        public static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as MButton).VM.Text = e.NewValue.ToString() ?? string.Empty;
        }

        private void UpdateHoverInfo(object sender, MouseEventArgs e)
        {
            VM.OnHoverChanged();
        }
    }
}
