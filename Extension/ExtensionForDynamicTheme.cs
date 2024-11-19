using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class ExtensionForDynamicTheme
    {
        /// <summary>
        /// 令全局主题切换可作用至该实例
        /// </summary>
        /// <param name="source"></param>
        public static T RunWithGlobalTheme<T>(this T source) where T : class
        {
            if (!DynamicTheme.GlobalInstance.Contains(source))
            {
                DynamicTheme.GlobalInstance.Add(source);
            }
            return source;
        }
        /// <summary>
        /// 取消此实例在全局主题中的响应
        /// </summary>
        /// <param name="source"></param>
        public static T RunWithOutGlobalTheme<T>(this T source) where T : class
        {
            DynamicTheme.GlobalInstance.Remove(source);
            return source;
        }
        /// <summary>
        /// 应用指定类型的主题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="attributeType">主题特性类型</param>
        /// <param name="paramAction">过渡参数构造</param>
        /// <returns></returns>
        public static T ApplyTheme<T>(this T source, Type attributeType, Action<TransitionParams>? paramAction = null) where T : class
        {
            var type = source.GetType();
            if (DynamicTheme.Source.TryGetValue(type, out var statedic))
            {
                if (statedic.TryGetValue(attributeType, out var state))
                {
                    source.BeginTransition(state, paramAction ?? TransitionParams.Theme);
                }
                else
                {
                    MessageBox.Show("未能找寻State");
                }
            }
            else
            {
                MessageBox.Show("未能找寻StateDictionary");
            }

            return source;
        }
    }
}
