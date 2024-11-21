using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 特性 ]判断当前对象是否需要回收
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PoolDisposeConditionAttribute : Attribute
    {
        public PoolDisposeConditionAttribute() { }
    }
}
