using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 特性 ]由对象池管理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PoolAttribute : Attribute
    {
        public PoolAttribute() { }
        /// <param name="initialcount">对象池初始加载该类型实例的数量</param>
        public PoolAttribute(int initialcount) { InitialCount = initialcount > -1 ? initialcount : 0; }
        public int InitialCount { get; set; } = 0;
    }
}
