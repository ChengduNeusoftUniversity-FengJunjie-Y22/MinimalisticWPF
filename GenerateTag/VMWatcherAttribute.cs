using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 源特性 ] 令方法作为属性的监听器
    /// <para>1. 请确保方法具备唯一形参（ WatcherEventArgs e ）</para>
    /// <para>2. 请确保方法名包含字段/属性名 , 例如 _id => OnIdChanged( WatcherEventArgs e ) / On_idChanged( WatcherEventArgs e )</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class VMWatcherAttribute : Attribute
    {
        public VMWatcherAttribute() { }
    }
}
