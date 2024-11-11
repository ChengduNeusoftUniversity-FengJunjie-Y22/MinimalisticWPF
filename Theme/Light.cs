using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    /// <summary>
    /// 亮色主题
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Light : Attribute, IThemeAttribute
    {
        public Light(params object?[] param)
        {
            Parameters = param;
        }

        public object?[]? Parameters { get; set; }
    }
}
