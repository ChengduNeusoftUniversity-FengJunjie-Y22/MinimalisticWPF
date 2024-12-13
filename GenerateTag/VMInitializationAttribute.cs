using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 源特性 ] 令函数作为构造函数的内部逻辑之一
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class VMInitializationAttribute : Attribute
    {
        public VMInitializationAttribute() { }
    }
}
