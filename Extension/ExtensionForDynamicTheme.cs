using System;
using System.Collections.Generic;
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
            if (!ThemeValues.TryGetValue(typeof(T), out _))
            {
                StateMachine.Create(source);
                GenerateValue<T>();
            }
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
            if (ThemeValues.TryGetValue(typeof(T), out var dictionary))
            {
                if (dictionary.TryGetValue(attributeType, out var state))
                {
                    source.BeginTransition(state, paramAction ?? TransitionParams.Theme);
                }
            }
            else
            {
                StateMachine.Create(source);
                GenerateValue<T>();
                ApplyTheme(source, attributeType, paramAction);
            }

            return source;
        }
        /// <summary>
        /// 生成主题State供过渡系统使用
        /// </summary>
        private static void GenerateValue<T>() where T : class
        {
            var dic = new Dictionary<Type, State>();
            foreach (var attributeType in Assemblies)
            {
                var state = State.FromType<T>().ToState();
                if (StateMachine.PropertyInfos.TryGetValue(typeof(T), out var infodictionary))
                {
                    foreach (var info in infodictionary.Values)
                    {
                        var inter = info.GetCustomAttribute(attributeType, true) as IThemeAttribute;
                        if (inter != null)
                        {
                            if (info.PropertyType == typeof(Brush))
                            {
                                var value = inter.Parameters?.FirstOrDefault()?.ToString()?.ToBrush() ?? Brushes.Transparent;
                                state.AddProperty(info.Name, value);
                            }
                            else if (info.PropertyType == typeof(double))
                            {
                                state.AddProperty(info.Name, inter.Parameters?.FirstOrDefault() ?? 0.0);
                            }
                            else if (info.PropertyType == typeof(Transform))
                            {
                                var value = Transform.Parse(inter.Parameters?.FirstOrDefault()?.ToString() ?? Transform.Identity.ToString());
                                state.AddProperty(info.Name, value);
                            }
                            else if (info.PropertyType == typeof(CornerRadius))
                            {
                                var value = Activator.CreateInstance(typeof(CornerRadius), inter.Parameters);
                                state.AddProperty(info.Name, value);
                            }
                            else if (info.PropertyType == typeof(Thickness))
                            {
                                var value = Activator.CreateInstance(typeof(Thickness), inter.Parameters);
                                state.AddProperty(info.Name, value);
                            }
                            else if (info.PropertyType == typeof(Point))
                            {
                                var value = Activator.CreateInstance(typeof(Point), inter.Parameters);
                                state.AddProperty(info.Name, value);
                            }
                        }
                    }
                }
                state.StateName = $"{typeof(T).FullName}_{attributeType.FullName}_DynamicTheme";
                dic.Add(attributeType, state);
            }
            ThemeValues.Add(typeof(T), dic);
        }
    }
}
