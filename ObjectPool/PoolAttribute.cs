using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 特性 ] 应用对象池的
    /// <para>1.自动扩容 : 基于初始值和最大值控制对象实例在全局中的总数</para>
    /// <para>2.自动回收 : 基于定时扫描+判定函数的后台回收机制</para>
    /// <para>3.异常处理 : 调用(Try)Fetch函数(尝试)调出一个可用资源</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PoolAttribute : Attribute
    {
        /// <param name="initial">初始化数量</param>
        /// <param name="max">最大数量</param>
        public PoolAttribute(int initial, int max)
        {
            if (!(max > initial) || initial < 0) throw new ArgumentException($"MPL01 传入的对象池参数不可用\n初始: {initial}\n最大: {max}");
            Initial = initial;
            Max = max;
        }

        public int Initial { get; set; } = 0;
        public int Max { get; set; } = 0;
    }
}
