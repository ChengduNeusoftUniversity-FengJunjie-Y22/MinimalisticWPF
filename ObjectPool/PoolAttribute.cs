using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 特性 ]由对象池管理
    /// <para>Pool的信号量永远大于等于0,对象不够时自动扩容</para>
    /// </summary>
    /// <remarks>
    /// <para>1.不启用自动回收,请使用1参数构造</para>
    /// <para>2.启用自动回收,请使用3参数构造</para>
    /// <para>3.无参数构造表示初始数量为零,不启用自动回收</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PoolAttribute : Attribute
    {
        public PoolAttribute() { }

        /// <param name="initialCount">初始数量</param>
        public PoolAttribute(int initialCount)
        {
            InitialCount = initialCount > -1 ? initialCount : 0;
        }

        /// <param name="initialCount">初始信号量</param>
        /// <param name="maximum">最大信号量</param>
        /// <param name="criticalDelta">若自动扩容次数达到该值,则触发自动回收</param>
        /// <param name="disposeDelta">单次自动回收量</param>
        public PoolAttribute(int initialCount, int maximum, int criticalDelta, int disposeDelta)
        {
            InitialCount = initialCount;
            Maximum = maximum;
            IsAutoDispose = true;
            CriticalDelta = criticalDelta;
            DisposeDelta = disposeDelta;
        }

        public int InitialCount { get; set; } = 0;
        public int Maximum { get; set; } = int.MaxValue;
        public bool IsAutoDispose { get; set; } = false;
        public int CriticalDelta { get; set; } = 0;
        public int DisposeDelta { get; set; } = 0;
    }
}
