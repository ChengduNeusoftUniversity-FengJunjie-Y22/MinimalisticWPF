using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class WhenDark : Attribute, IThemeAttribute
    {
        public WhenDark(object? target)
        {
            Target = target;
        }

        public WhenDark(Type type, params object?[] param)
        {
            if (type == typeof(Brush))
            {
                var value = param.FirstOrDefault()?.ToString()?.ToBrush();
                Target = value ?? Brushes.Transparent;
            }
            else
            {
                Target = Activator.CreateInstance(type, param);
            }
        }

        public object? Target { get; set; }
    }
}
