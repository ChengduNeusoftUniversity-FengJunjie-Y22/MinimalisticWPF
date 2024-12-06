using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ 源特性 ] 启用AOP模式 ( 类型实例拥有Proxy属性,从该属性操作对象将允许做出拦截/覆盖行为 )
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AspectOrientedAttribute : Attribute
    {
        public AspectOrientedAttribute() { }
    }
}
