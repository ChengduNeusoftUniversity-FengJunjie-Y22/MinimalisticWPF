using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 特性 ] 判断当前对象是否受到保护 , 受到保护的对象不被自动回收
    /// </summary>
    /// <remarks>
    /// 注意: 方法必须是一个返回bool值的实例方法,并且不包含任何形参
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PoolDisposeConditionAttribute : Attribute
    {
        public PoolDisposeConditionAttribute() { }
    }
}
