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
    public partial class MPageBox : UserControl
    {
        public MPageBox()
        {
            InitializeComponent();
            if (PageManager.Pages.Length == 0)
            {
                PageManager.Scan();
            }
        }

        /// <summary>
        /// 淡入时间 ( 秒 )
        /// </summary>
        public double FadeTime
        {
            get { return (double)GetValue(FadeTimeProperty); }
            set { SetValue(FadeTimeProperty, value); }
        }
        public static readonly DependencyProperty FadeTimeProperty =
            DependencyProperty.Register("FadeTime", typeof(double), typeof(MPageBox), new PropertyMetadata(0.2));

        /// <summary>
        /// 淡出时间 ( 秒 )
        /// </summary>
        public double FadeOutTime
        {
            get { return (double)GetValue(FadeOutTimeProperty); }
            set { SetValue(FadeOutTimeProperty, value); }
        }
        public static readonly DependencyProperty FadeOutTimeProperty =
            DependencyProperty.Register("FadeOutTime", typeof(double), typeof(MPageBox), new PropertyMetadata(0.8));

        public void Navigate(Type pageType)
        {
            UpdateSource(PageManager.Find(pageType));
        }
        public void Navigate(string pageName)
        {
            UpdateSource(PageManager.Find(pageName));
        }
        public void Navigate(int pageIndex)
        {
            UpdateSource(PageManager.Find(pageIndex));
        }
        private void UpdateSource(object? data)
        {
            if (data == null)
            {
                return;
            }
            var page = data as UIElement;
            CurrentPage.Transition()
                .SetProperty(x => x.Opacity, 0)
                .SetParams((x) =>
                {
                    x.Duration = FadeTime;
                    x.Completed = () =>
                    {
                        CurrentPage.Transition()
                            .SetProperty(x => x.Opacity, 1)
                            .SetParams((x) =>
                            {
                                x.Duration = FadeOutTime;
                                x.Start = () =>
                                {
                                    CurrentPage.Child = page;
                                };
                            })
                            .Start();
                    };
                })
                .Start();
        }
    }
}
