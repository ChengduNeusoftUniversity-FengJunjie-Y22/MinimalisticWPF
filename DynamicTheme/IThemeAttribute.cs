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
        /// 目标值
        /// </summary>
        object? Target { get; set; }
    }
}
