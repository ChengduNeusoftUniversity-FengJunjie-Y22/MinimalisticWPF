using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class DynamicTheme
    {
        public static ConcurrentDictionary<Type, ConcurrentDictionary<Type, State>> TransitionSource { get; internal set; } = new();
        public static HashSet<object> GlobalInstance { get; internal set; } = new(64);

        private static bool _isloaded = false;
        public static void Awake()
        {
            if (!_isloaded)
            {
                var Assemblies = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
                var classAssemblies = Assemblies.Where(t => t.GetCustomAttribute(typeof(ThemeAttribute), true) != null);
                var attributeAssemblies = Assemblies.Where(t => typeof(IThemeAttribute).IsAssignableFrom(t) && typeof(Attribute).IsAssignableFrom(t) && !t.IsAbstract);
                KVPGeneration(classAssemblies, attributeAssemblies);
                _isloaded = true;
            }
        }

        /// <summary>
        /// [ 全局 ] 应用主题 , 需要 object.RunWithGlobalTheme() 激活对象以在全局生效
        /// </summary>
        /// <param name="attributeType">主题特性类型</param>
        /// <param name="paramAction">过渡参数构造</param>
        /// <param name="windowBack">主窗口背景色</param>
        public static void GlobalApply(Type attributeType, Action<TransitionParams>? paramAction = null, Brush? windowBack = default)
        {
            Awake();
            foreach (var item in GlobalInstance)
            {
                item.ApplyTheme(attributeType, paramAction);
            }
            Application.Current.MainWindow.Transition()
                .SetProperty(x => x.Background, windowBack ?? Application.Current.MainWindow.Background)
                .SetParams(paramAction ?? TransitionParams.Theme)
                .Start();
        }
        /// <summary>
        /// [ 局部 ] 应用主题 , 无需激活
        /// </summary>
        /// <param name="attributeType">主题特性类型</param>
        /// <param name="paramAction">过渡参数构造</param>
        /// <param name="targets">目标实例</param>
        public static void PartialApply(Type attributeType, Action<TransitionParams>? paramAction = null, params object[] targets)
        {
            Awake();
            foreach (var item in targets)
            {
                item.ApplyTheme(attributeType, paramAction);
            }
        }
        private static void KVPGeneration(IEnumerable<Type> classes, IEnumerable<Type> attributes)
        {
            foreach (var cs in classes)
            {
                StateMachine.InitializeTypes(cs);
                if (!StateMachine.SplitedPropertyInfos.TryGetValue(cs, out var group)) break;
                var unit = new ConcurrentDictionary<Type, State>();
                foreach (var attribute in attributes)
                {
                    var properties = cs.GetProperties()
                    .Select(p => new
                    {
                        PropertyInfo = p,
                        Context = p.GetCustomAttribute(attribute, true) as IThemeAttribute,
                    });
                    var state = new State();
                    foreach (var info in properties)
                    {
                        if (info.PropertyInfo.CanWrite && info.PropertyInfo.CanRead && info.Context != null)
                        {
                            Func<int> func = (group.Item1.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item2.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item3.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item4.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item5.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item6.TryGetValue(info.PropertyInfo.Name, out _))
                            switch
                            {
                                (true, false, false, false, false, false) => () =>
                                {
                                    var value = Convert.ToDouble(info.Context.Parameters?.FirstOrDefault() ?? 0);
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 1;
                                }
                                ,
                                (false, true, false, false, false, false) => () =>
                                {
                                    var value = info.Context.Parameters?.FirstOrDefault()?.ToString()?.ToBrush() ?? Brushes.Transparent;
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 2;
                                }
                                ,
                                (false, false, true, false, false, false) => () =>
                                {
                                    var value = Transform.Parse(info.Context.Parameters?.FirstOrDefault()?.ToString() ?? Transform.Identity.ToString());
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 3;
                                }
                                ,
                                (false, false, false, true, false, false) => () =>
                                {
                                    var value = Activator.CreateInstance(typeof(Point), info.Context.Parameters);
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 4;
                                }
                                ,
                                (false, false, false, false, true, false) => () =>
                                {
                                    var value = Activator.CreateInstance(typeof(CornerRadius), info.Context.Parameters);
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 5;
                                }
                                ,
                                (false, false, false, false, false, true) => () =>
                                {
                                    var value = Activator.CreateInstance(typeof(Thickness), info.Context.Parameters);
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 6;
                                }
                                ,
                                _ => () => { return -1; }
                            };
                            func.Invoke();
                        }
                    }
                    unit.TryAdd(attribute, state);
                }
                TransitionSource.TryAdd(cs, unit);
            }
        }
    }
}
