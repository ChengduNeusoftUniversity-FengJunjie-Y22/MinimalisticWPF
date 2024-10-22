using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF
{
    /// <summary>
    /// 允许用户控件作为 [ 页面 ] 并在特定容器内实现丝滑切换
    /// </summary>
    public interface IPageChanging
    {
        /// <summary>
        /// 页面实例的名称（ 唯一 ）
        /// </summary>
        string PageName { get; }

        /// <summary>
        /// 描述如何返回页面实例,这将在每次切换至该页面时调用
        /// </summary>
        object GetPage();

        /// <summary>
        /// 描述页面的实际尺寸
        /// </summary>
        Size GetPageSize();
    }
}
