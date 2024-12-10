using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 源特性 ] 令函数作为无参构造函数的内部逻辑之一
    /// <para>注意: 该函数不应具备任何形参</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class VMInitializationAttribute : Attribute
    {
        public VMInitializationAttribute() { }
    }
}
