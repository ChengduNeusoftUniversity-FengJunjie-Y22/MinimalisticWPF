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
using System.Windows.Shapes;

namespace MinimalisticWPF
{
    /// <summary>
    /// 处理范畴:
    /// <para>1.文本提示</para>
    /// <para>2.选择执行</para>
    /// </summary>
    public partial class Notification : Window
    {
        public Notification()
        {
            InitializeComponent();
            this.StateMachineLoading(ViewModel);            
        }

        internal bool Result = false;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        /// <summary>
        /// 打印通知消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="title">标题</param>
        /// <param name="YesText">确定键的文本</param>
        public static void Message(string message, string title = "Message", string YesText = "Yes")
        {
            var result = new Notification();
            result.ViewModel.Text = message;
            result.ViewModel.Title = title;
            result.B1.WiseWidth = 350;
            result.B1.Text = YesText;
            result.B2.WiseWidth = 0;
            result.ShowDialog();
        }

        /// <summary>
        /// 等待用户选择下一步是否要继续执行
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="title">标题</param>
        /// <param name="YesText">确定键文本</param>
        /// <param name="NoText">否定键文本</param>
        /// <returns>bool 用户选择的结果</returns>
        public static bool Select(string message, string title = "Select", string YesText = "Yes", string NoText = "No")
        {
            var result = new Notification();
            result.ViewModel.Text = message;
            result.ViewModel.Title = title;
            result.B1.Text = YesText;
            result.B2.Text = NoText;
            result.ShowDialog();
            return result.Result;
        }

        private void MButton_Click1(object sender, MouseButtonEventArgs e)
        {
            Close();
            Result = true;
        }
        private void MButton_Click2(object sender, MouseButtonEventArgs e)
        {
            Close();
            Result = false;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            e.Handled = true;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimationC.Transition()
                .SetProperty(x => x.Width, 350)
                .SetProperty(x => x.Opacity, 0.9)
                .SetParams((x) =>
                {
                    x.Duration = 0.4;
                })
                .Start();
            AnimationD.Transition()
                .SetProperty(x => x.Width, 350)
                .SetProperty(x => x.Opacity, 0.9)
                .SetParams((x) =>
                {
                    x.Duration = 0.4;
                })
                .Start();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimationC.Transition()
                .SetProperty(x => x.Width, 1)
                .SetProperty(x => x.Opacity, 0)
                .SetParams((x) =>
                {
                    x.Duration = 0.4;
                })
                .Start();
            AnimationD.Transition()
                .SetProperty(x => x.Width, 1)
                .SetProperty(x => x.Opacity, 0)
                .SetParams((x) =>
                {
                    x.Duration = 0.4;
                })
                .Start();
        }
    }
}
