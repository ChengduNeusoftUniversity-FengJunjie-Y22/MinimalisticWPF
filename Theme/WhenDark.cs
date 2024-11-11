using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    /// <summary>
    /// 暗色主题
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class WhenDark : Attribute, IThemeAttribute
    {
        public WhenDark(params object?[] param)
        {
            Parameters = param;
        }

        public object?[]? Parameters { get; set; }
    }
}
