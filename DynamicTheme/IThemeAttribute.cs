using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 声明 ] 用于动态主题的特性
    /// <para>令自定义特性实现此接口 , 则可使用扩展方法 object.ApplyTheme()</para>
    /// </summary>
    public interface IThemeAttribute
    {
        /// <summary>
        /// 指定主题下属性的目标值
        /// </summary>
        object? Target { get; set; }
    }
}
