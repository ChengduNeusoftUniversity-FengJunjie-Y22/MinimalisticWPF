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
        /// 构造新值所需的参数
        /// </summary>
        object?[]? Parameters { get; set; }
    }
}
