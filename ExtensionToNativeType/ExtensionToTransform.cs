using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.ExtensionToNativeType
{
    public static class ExtensionToTransform
    {
        /// <summary>
        /// 应用控件的中心点
        /// </summary>
        public static RotateTransform CenterTo(this RotateTransform source, FrameworkElement target)
        {
            var x = target.ActualWidth / 2;
            var y = target.ActualHeight / 2;
            source.CenterX = x;
            source.CenterY = y;

            return source;
        }

        /// <summary>
        /// 应用控件的中心点
        /// </summary>
        public static ScaleTransform CenterTo(this ScaleTransform source, FrameworkElement target)
        {
            var x = target.ActualWidth / 2;
            var y = target.ActualHeight / 2;
            source.CenterX = x;
            source.CenterY = y;

            return source;
        }
    }
}
