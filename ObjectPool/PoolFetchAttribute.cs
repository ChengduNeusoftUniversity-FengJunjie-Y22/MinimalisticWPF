using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 特性 ]对象从对象池取出时触发
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PoolFetchAttribute : Attribute
    {
        public PoolFetchAttribute() { }
    }
}
