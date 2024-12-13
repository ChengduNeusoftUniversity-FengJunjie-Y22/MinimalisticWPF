using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 源特性 ] 令方法作为属性的监听器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class VMWatcherAttribute : Attribute
    {
        public VMWatcherAttribute() { }
    }
}
