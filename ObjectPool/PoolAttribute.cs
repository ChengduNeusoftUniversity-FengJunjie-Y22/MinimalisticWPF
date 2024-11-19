using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 特性 ]由对象池管理
    /// <para>Pool的信号量永远大于等于0,通常建议设置</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PoolAttribute : Attribute
    {
        /// <summary>
        /// 1.若不启用自动回收,请使用1参数构造
        /// <para>2.若要启用自动回收,请使用3参数构造</para>
        /// <para>3.采用0参数构造代表非自动回收且初始信号量为0</para>
        /// </summary>
        public PoolAttribute() { }

        /// <summary>
        /// 1.若不启用自动回收,请使用1参数构造
        /// <para>2.若要启用自动回收,请使用3参数构造</para>
        /// <para>3.采用0参数构造代表非自动回收且初始信号量为0</para>
        /// </summary>
        /// <param name="initialCount">初始数量</param>
        public PoolAttribute(int initialCount)
        {
            InitialCount = initialCount > -1 ? initialCount : 0;
        }

        /// <summary>
        /// 1.若不启用自动回收,请使用1参数构造
        /// <para>2.若要启用自动回收,请使用3参数构造</para>
        /// <para>3.采用0参数构造代表非自动回收且初始信号量为0</para>
        /// </summary>
        /// <param name="initialCount">初始数量</param>
        /// <param name="criticalValue">信号量达到该阈值时触发自动回收</param>
        /// <param name="disposeDelta">单次自动回收量</param>
        public PoolAttribute(int initialCount, int criticalValue, int disposeDelta)
        {
            var condition1 = criticalValue > 0 && criticalValue < initialCount;
            var condition2 = disposeDelta <= initialCount - criticalValue && disposeDelta >= 0;
            var condition3 = criticalValue > 0;
            if (!(condition1 && condition2 && condition3)) throw new ArgumentException("PA01 : Usually the threshold value must be between 0 and the initial value. The number of objects in a reclaim must be between 0 and the difference between the initial value and the threshold value.");
            InitialCount = initialCount > -1 ? initialCount : 0;
            IsAutoDispose = true;
            CriticalValue = criticalValue;
            DisposeDelta = disposeDelta;
        }

        public int InitialCount { get; set; } = 0;
        public bool IsAutoDispose { get; set; } = false;
        public int CriticalValue { get; set; } = 0;
        public int DisposeDelta { get; set; } = 0;
    }
}
