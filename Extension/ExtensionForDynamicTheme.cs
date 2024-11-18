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
        /// 类型 => [ 主题 => 状态机动画数据 ]
        /// </summary>
        public static Dictionary<Type, Dictionary<Type, State>> ThemeValues { get; internal set; } = new Dictionary<Type, Dictionary<Type, State>>();

        /// <summary>
        /// 可全局切换主题的对象
        /// </summary>
        public static List<object> InstanceHosts { get; internal set; } = new List<object>(64);

        private static Type[] _assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IThemeAttribute).IsAssignableFrom(t) && typeof(Attribute).IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();
        /// <summary>
        /// 程序集中用于标注主题的全部自定义特性
        /// </summary>
        public static Type[] Assemblies => _assemblies;

        /// <summary>
        /// 令全局主题切换可作用至该实例
        /// </summary>
        /// <param name="source"></param>
        public static T RunWithGlobalTheme<T>(this T source) where T : class
        {
            if (!InstanceHosts.Contains(source))
            {
                InstanceHosts.Add(source);
            }
            return source;
        }
        /// <summary>
        /// 取消此实例在全局主题中的响应
        /// </summary>
        /// <param name="source"></param>
        public static T RunWithOutGlobalTheme<T>(this T source) where T : class
        {
            InstanceHosts.Remove(source);
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
            if (ThemeValues.TryGetValue(type, out var dictionary))
            {
                if (dictionary.TryGetValue(attributeType, out var state))
                {
                    source.BeginTransition(state, paramAction ?? TransitionParams.Theme);
                }
            }
            else
            {
                StateMachine.Create(source);
                GenerateValue(type);
                ApplyTheme(source, attributeType, paramAction);
            }

            return source;
        }
        /// <summary>
        /// 生成主题State供过渡系统使用
        /// </summary>
        private static void GenerateValue(Type type)
        {
            if (StateMachine.SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                var dic = new Dictionary<Type, State>();
                foreach (var attributeType in Assemblies)
                {
                    var state = new State();
                    foreach (var info in infodictionary.Item1.Values)
                    {
                        var value = info.GetCustomAttribute(attributeType, true) as IThemeAttribute;
                        if (value == null) break;
                        state.AddProperty(info.Name, value.Parameters?.FirstOrDefault() ?? 0.0);
                    }
                    foreach (var info in infodictionary.Item2.Values)
                    {
                        var inner = info.GetCustomAttribute(attributeType, true) as IThemeAttribute;
                        if (inner == null) break;
                        var value = inner.Parameters?.FirstOrDefault()?.ToString()?.ToBrush() ?? nameof(Brushes.Transparent).ToBrush();
                        state.AddProperty(info.Name, value);
                    }
                    foreach (var info in infodictionary.Item3.Values)
                    {
                        var inner = info.GetCustomAttribute(attributeType, true) as IThemeAttribute;
                        if (inner == null) break;
                        var value = Activator.CreateInstance(typeof(Transform), inner.Parameters);
                        state.AddProperty(info.Name, value);
                    }
                    foreach (var info in infodictionary.Item4.Values)
                    {
                        var inner = info.GetCustomAttribute(attributeType, true) as IThemeAttribute;
                        if (inner == null) break;
                        var value = Activator.CreateInstance(typeof(Point), inner.Parameters);
                        state.AddProperty(info.Name, value);
                    }
                    foreach (var info in infodictionary.Item5.Values)
                    {
                        var inner = info.GetCustomAttribute(attributeType, true) as IThemeAttribute;
                        if (inner == null) break;
                        var value = Activator.CreateInstance(typeof(CornerRadius), inner.Parameters);
                        state.AddProperty(info.Name, value);
                    }
                    foreach (var info in infodictionary.Item6.Values)
                    {
                        var inner = info.GetCustomAttribute(attributeType, true) as IThemeAttribute;
                        if (inner == null) break;
                        var value = Activator.CreateInstance(typeof(Thickness), inner.Parameters);
                        state.AddProperty(info.Name, value);
                    }
                    state.StateName = $"{type.FullName}_{attributeType.FullName}_DynamicTheme";
                    dic.Add(attributeType, state);
                }
                ThemeValues.Add(type, dic);
            }
        }
    }
}
