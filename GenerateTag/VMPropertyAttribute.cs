using System;
using System.Collections.Generic;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 源特性 ] 声明字段对应属性由源生成器自动生成
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class VMPropertyAttribute : Attribute
    {
        public VMPropertyAttribute() { }
    }
}
