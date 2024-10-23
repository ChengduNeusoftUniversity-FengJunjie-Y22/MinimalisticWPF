using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF
{
    /// <summary>
    /// 允许将 [ 用户控件 ] 作为 [ 页面 ] 显示在 [ MPageBox ] 中
    /// </summary>
    public interface IPageNavigate
    {
        /// <summary>
        /// 返回[唯一]页面名称
        /// </summary>
        string GetPageName();

        /// <summary>
        /// 返回页面尺寸
        /// </summary>
        Size GetPageSize();

        /// <summary>
        /// 返回页面实例
        /// </summary>
        object GetPage();
    }
}
