using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 特性 ] 在对象从对象池中取出时触发
    /// </summary>
    /// <remarks>
    /// 注意: 方法必须是一个实例方法,并且不包含任何形参
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PoolFetchAttribute : Attribute
    {
        public PoolFetchAttribute() { }
    }
}
