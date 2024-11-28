using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 声明 ] 用于动态主题的特性
    /// </summary>
    public interface IThemeAttribute
    {
        /// <summary>
        /// 构造新值所需的参数
        /// </summary>
        object?[]? Parameters { get; set; }
    }
}
