using System;
using System.Collections.Generic;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 源特性 ] 声明字段对应属性由源生成器自动生成
    /// <para>1. 请确保字段由 "_" 开头</para>
    /// <para>2. 请确保字段由小写字母开头</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class VMPropertyAttribute : Attribute
    {
        public VMPropertyAttribute() { }
    }
}
