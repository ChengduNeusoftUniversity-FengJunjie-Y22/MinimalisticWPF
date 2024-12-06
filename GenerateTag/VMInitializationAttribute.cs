﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 源特性 ] 令函数作为无参构造函数的内部逻辑之一 （ 非AOP模式下 , 源生成器不生成无参构造函数 , 可直接手动定义 ）
    /// <para>注意: 该函数不应具备任何形参</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class VMInitialization : Attribute
    {
        public VMInitialization() { }
    }
}
