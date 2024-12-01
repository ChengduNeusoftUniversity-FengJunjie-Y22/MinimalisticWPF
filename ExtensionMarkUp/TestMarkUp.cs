using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace MinimalisticWPF
{
    public class TestMarkUp : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            // 获取使用该扩展的目标对象（通常是控件）
            var targetObject = target.TargetObject as FrameworkElement;

            // 获取目标属性
            //DependencyProperty targetProperty = target.TargetProperty as DependencyProperty;

            // 返回你想要的值
            return targetObject ?? throw new InvalidOperationException("Could not get target object.");
        }
    }
}
