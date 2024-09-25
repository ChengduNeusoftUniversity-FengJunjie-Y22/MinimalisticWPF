using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MinimalisticWPF
{
    public static class ExtensionForFrameworkElement
    {
        /// <summary>
        /// 按比率应用为父级元素的尺寸
        /// </summary>
        public static T ToParentSize<T>(this T element, double rate = 1) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            element.Width = Parent.Width * rate;
            element.Height = Parent.Height * rate;
            return element;
        }

        /// <summary>
        /// 按比率应用为父级元素的高度
        /// </summary>
        public static T ToParentHeight<T>(this T element, double rate = 1) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            element.Height = Parent.Height * rate;
            return element;
        }

        /// <summary>
        /// 按比率应用为父级元素的宽度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static T ToParentWidth<T>(this T element, double rate = 1) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            element.Width = Parent.Width * rate;
            return element;
        }
    }   
}
