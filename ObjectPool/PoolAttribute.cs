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
        public PoolAttribute(int initial, int max, int critical)
        {
            Initial = initial;
            Max = max;
            Critical = critical;
        }

        public int Initial { get; set; } = 0;
        public int Max { get; set; } = 0;
        public int Critical { get; set; } = 0;
    }
}
