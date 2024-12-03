using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PoolAttribute : Attribute
    {
        public PoolAttribute(int initial, int max)
        {
            if (!(max > initial) || initial < 0) throw new ArgumentException($"MPL06 传入的对象池参数不可用\n初始: {initial}\n最大: {max}");
            Initial = initial;
            Max = max;
        }

        public int Initial { get; set; } = 0;
        public int Max { get; set; } = 0;
    }
}
