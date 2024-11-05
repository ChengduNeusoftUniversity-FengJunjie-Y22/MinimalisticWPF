using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Singleton ] 单例模式
    /// <para>[ Refresh ] 调取时新建</para>
    /// </summary>
    public enum ConstructionModes
    {
        Singleton,
        Refresh
    }

    /// <summary>
    /// 标记为 [ 可导航 ]
    /// <para>可使用容器[ MPageBox ]实现页面切换效果</para>
    /// <para>可使用类型[ PageManager ]执行导航操作</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class Navigable : Attribute
    {
        public Navigable() { }
        public Navigable(ConstructionModes mode)
        {
            Mode = mode;
        }

        public ConstructionModes Mode { get; set; } = ConstructionModes.Singleton;
    }
}
