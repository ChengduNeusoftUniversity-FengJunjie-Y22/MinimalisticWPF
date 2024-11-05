using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
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
    /// <summary>
    /// 导航模式
    /// <para>[ Fade ] 淡入淡出</para>
    /// <para>[ Slide ] 滑动</para>
    /// </summary>
    public enum NavigateModes
    {
        Fade,
        Slide,
        None
    }

    /// <summary>
    /// 滑动导航时,滑动的方向
    /// </summary>
    public enum SlideDirection
    {
        RightToLeft,
        LeftToRight,
        TopToBottom,
        BottomToTop,
    }
    public partial class MPageBox : UserControl
    {
        public MPageBox()
        {
            InitializeComponent();
            Navigator.Scan();
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

        /// <summary>
        /// 切页加速率
        /// </summary>
        public double Acceleration
        {
            get { return (double)GetValue(AccelerationProperty); }
            set { SetValue(AccelerationProperty, value); }
        }
        public static readonly DependencyProperty AccelerationProperty =
            DependencyProperty.Register("Acceleration", typeof(double), typeof(MPageBox), new PropertyMetadata(0.0));

        /// <summary>
        /// 切页帧率
        /// </summary>
        public int FrameRate
        {
            get { return (int)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }
        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.Register("FrameRate", typeof(int), typeof(MPageBox), new PropertyMetadata(60));

        /// <summary>
        /// 导航模式
        /// </summary>
        public NavigateModes NavigateMode
        {
            get { return (NavigateModes)GetValue(NavigateModeProperty); }
            set { SetValue(NavigateModeProperty, value); }
        }
        public static readonly DependencyProperty NavigateModeProperty =
            DependencyProperty.Register("NavigateMode", typeof(NavigateModes), typeof(MPageBox), new PropertyMetadata(NavigateModes.Slide));

        /// <summary>
        /// 滑动方向
        /// </summary>
        public SlideDirection SlideDirection
        {
            get { return (SlideDirection)GetValue(SlideDirectionProperty); }
            set { SetValue(SlideDirectionProperty, value); }
        }
        public static readonly DependencyProperty SlideDirectionProperty =
            DependencyProperty.Register("SlideDirection", typeof(SlideDirection), typeof(MPageBox), new PropertyMetadata(SlideDirection.RightToLeft));

        internal TranslateTransform Slide
        {
            get => new TranslateTransform(SlideX, SlideY);
        }
        internal double SlideX
        {
            get => (SlideDirection == SlideDirection.RightToLeft || SlideDirection == SlideDirection.LeftToRight) ? Width : 0;
        }
        internal double SlideY
        {
            get => (SlideDirection == SlideDirection.TopToBottom || SlideDirection == SlideDirection.BottomToTop) ? Height : 0;
        }

        public void Navigate(Type pageType, params object?[] value)
        {
            UpdateSource(Navigator.GetInstance(pageType, value));
        }

        private void UpdateSource(object? data)
        {
            switch (NavigateMode)
            {
                case NavigateModes.Fade:
                    FadeAction(data); break;
                case NavigateModes.Slide:
                    SlideAction(data); break;
                case NavigateModes.None:
                    CurrentPage.Child = data as UIElement;
                    break;
            }
        }
        private void FadeAction(object? data)
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
                    x.Acceleration = Acceleration;
                    x.FrameRate = FrameRate;
                    x.Completed = () =>
                    {
                        CurrentPage.Transition()
                            .SetProperty(x => x.Opacity, 1)
                            .SetParams((x) =>
                            {
                                x.Duration = FadeOutTime;
                                x.Acceleration = Acceleration;
                                x.FrameRate = FrameRate;
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
        private void SlideAction(object? data)
        {
            if (data == null)
            {
                return;
            }
            var page = data as UIElement;
            var translate = Slide;
            translate.X = (SlideDirection == SlideDirection.LeftToRight || SlideDirection == SlideDirection.TopToBottom) ? translate.X : -translate.X;
            translate.Y = (SlideDirection == SlideDirection.RightToLeft || SlideDirection == SlideDirection.BottomToTop) ? -translate.Y : translate.Y;
            CurrentPage.Transition()
                .SetProperty(x => x.Opacity, 0)
                .SetProperty(x => x.RenderTransform, translate)
                .SetParams((x) =>
                {
                    x.Duration = FadeTime;
                    x.Acceleration = Acceleration;
                    x.FrameRate = FrameRate;
                    x.Completed = () =>
                    {
                        translate.X = -translate.X;
                        translate.Y = -translate.Y;
                        CurrentPage.RenderTransform = translate;
                        CurrentPage.Transition()
                            .SetProperty(x => x.Opacity, 1)
                            .SetProperty(x => x.RenderTransform, Transform.Identity)
                            .SetParams((x) =>
                            {
                                x.Duration = FadeOutTime;
                                x.Acceleration = Acceleration;
                                x.FrameRate = FrameRate;
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
