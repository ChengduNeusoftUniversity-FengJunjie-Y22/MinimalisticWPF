using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF
{
    public static class ExtensionToFrameworkElement
    {
        public static T ToParentSize<T>(this T element) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            element.Width = Parent.Width;
            element.Height = Parent.Height;
            return element;
        }

        public static T ToParentHeight<T>(this T element) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            element.Height = Parent.Height;
            return element;
        }

        public static T ToParentWidth<T>(this T element) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            element.Width = Parent.Width;
            return element;
        }
    }
}
