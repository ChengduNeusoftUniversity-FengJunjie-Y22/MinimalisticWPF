using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MinimalisticWPF
{
    /// <summary>
    /// 【动画工具】提供系列静态方法，为动画的创建提供便利
    /// </summary>
    public static class AnimationTool
    {
        /// <summary>
        /// 指定元素的透明度由当前变为目标值
        /// </summary>
        public static void CurrentOpacityToTarget<T>(T target, DoubleAnimation info) where T : FrameworkElement
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                From = target.Opacity,
                To = info.To,
                AccelerationRatio = info.AccelerationRatio,
                AutoReverse = info.AutoReverse,
                Duration = info.Duration
            };
            target.BeginAnimation(FrameworkElement.OpacityProperty, doubleAnimation);
        }

        /// <summary>
        /// 指定元素的宽度由当前值过渡为0
        /// </summary>
        public static void CurrentWidthToZero<T>(T target, DoubleAnimation info) where T : FrameworkElement
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                From = target.ActualWidth,
                To = 0,
                AccelerationRatio = info.AccelerationRatio,
                AutoReverse = info.AutoReverse,
                Duration = info.Duration
            };
            target.BeginAnimation(FrameworkElement.WidthProperty, doubleAnimation);
        }

        /// <summary>
        /// 指定元素的高度由当前值过渡为0
        /// </summary>
        public static void CurrentHeightToZero<T>(T target, DoubleAnimation info) where T : FrameworkElement
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                From = target.ActualWidth,
                To = 0,
                AccelerationRatio = info.AccelerationRatio,
                AutoReverse = info.AutoReverse,
                Duration = info.Duration
            };
            target.BeginAnimation(FrameworkElement.HeightProperty, doubleAnimation);
        }

        /// <summary>
        /// 指定元素的宽度由父级元素宽度过渡为0
        /// </summary>
        public static void FatherWidthToZero<T>(T target, DoubleAnimation info) where T : FrameworkElement
        {
            FrameworkElement? parent = target.Parent as FrameworkElement;

            if (parent != null)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    From = parent.ActualWidth,
                    To = 0,
                    AccelerationRatio = info.AccelerationRatio,
                    AutoReverse = info.AutoReverse,
                    Duration = info.Duration
                };
                target.BeginAnimation(FrameworkElement.WidthProperty, doubleAnimation);
            }
        }

        /// <summary>
        /// 指定元素的高度由父级元素高度过渡为0
        /// </summary>
        public static void FatherHeightToZero<T>(T target, DoubleAnimation info) where T : FrameworkElement
        {
            FrameworkElement? parent = target.Parent as FrameworkElement;

            if (parent != null)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    From = parent.ActualWidth,
                    To = 0,
                    AccelerationRatio = info.AccelerationRatio,
                    AutoReverse = info.AutoReverse,
                    Duration = info.Duration
                };
                target.BeginAnimation(FrameworkElement.HeightProperty, doubleAnimation);
            }
        }

        /// <summary>
        /// 指定元素的宽度由0过渡为父级元素宽度
        /// </summary>
        public static void ZeroWidthToFather<T>(T target, DoubleAnimation info) where T : FrameworkElement
        {
            FrameworkElement? parent = target.Parent as FrameworkElement;

            if (parent != null)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    From = 0,
                    To = parent.ActualWidth,
                    AccelerationRatio = info.AccelerationRatio,
                    AutoReverse = info.AutoReverse,
                    Duration = info.Duration
                };
                target.BeginAnimation(FrameworkElement.WidthProperty, doubleAnimation);
            }
        }

        /// <summary>
        /// 指定元素的高度由0过渡到父级控件高度
        /// </summary>
        public static void ZeroHeightToFather<T>(T target, DoubleAnimation info) where T : FrameworkElement
        {
            FrameworkElement? parent = target.Parent as FrameworkElement;

            if (parent != null)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    From = 0,
                    To = parent.ActualWidth,
                    AccelerationRatio = info.AccelerationRatio,
                    AutoReverse = info.AutoReverse,
                    Duration = info.Duration
                };
                target.BeginAnimation(FrameworkElement.HeightProperty, doubleAnimation);
            }
        }

        /// <summary>
        /// 解决Template中的控件无法直接获取的问题
        /// </summary>
        /// <typeparam name="T">待寻找元素的类型</typeparam>
        /// <param name="father">Template顶层元素</param>
        /// <param name="name">Template内部元素的命名</param>
        public static T FindTemplateElementByName<T>(Control father, string name) where T : FrameworkElement, new()
        {
            try
            {
                T? target = father.Template.FindName(name, father) as T;
                return target == null ? new T() : target;
            }
            catch { }
            return new T();
        }
    }
}
