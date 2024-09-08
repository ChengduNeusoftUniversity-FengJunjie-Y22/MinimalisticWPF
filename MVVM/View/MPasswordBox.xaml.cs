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
    public partial class MPasswordBox : UserControl
    {
        public MPasswordBox()
        {
            InitializeComponent();
            this.StateMachineLoading(ViewModel);
        }

        /// <summary>
        /// 实际密码
        /// </summary>
        public string Password
        {
            get => ViewModel.TruePassword;
            set => ViewModel.TruePassword = value;
        }

        /// <summary>
        /// 密码替换符
        /// </summary>
        public string Replace
        {
            get => ViewModel.ReplacingCharacters;
            set => ViewModel.ReplacingCharacters = value;
        }

        private void TruePWD_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.TruePassword = TruePWD.Text;
        }

        private void TruePWD_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }
    }
}
