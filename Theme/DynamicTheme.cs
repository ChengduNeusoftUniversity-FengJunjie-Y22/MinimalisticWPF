﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MinimalisticWPF
{
    /// <summary>
    /// <para>H ( Heading )    >> 标题文本色</para>
    /// <para>P ( Paragraph )  >> 段落文本色</para>
    /// <para>B ( Background ) >> 背景色</para>
    /// <para>E ( EdgeBrush )  >> 边界涂色</para>
    /// <para>F ( Focus )      >> 悬停色</para>
    /// </summary>
    public enum BrushTags : int
    {
        Default,
        H1, H2, H3, H4, H5,
        P1, P2, P3, P4, P5,
        B1, B2, B3, B4, B5,
        E1, E2, E3, E4, E5,
        F1, F2, F3, F4, F5,
    }

    public static class DynamicTheme
    {
        internal static ConcurrentDictionary<Type, ConcurrentDictionary<Type, State>> TransitionSource { get; set; } = new();
        internal static HashSet<object> GlobalInstance { get; set; } = new(64);

        private static bool _isloaded = false;
        public static void Awake()
        {
            if (!_isloaded)
            {
                var Assemblies = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
                var classAssemblies = Assemblies.Where(t => t.GetCustomAttribute(typeof(ThemeAttribute), true) != null);
                var attributeAssemblies = Assemblies.Where(t => typeof(IThemeAttribute).IsAssignableFrom(t) && typeof(Attribute).IsAssignableFrom(t) && !t.IsAbstract);
                var themebrushesAssemblies = Assemblies.Where(t => typeof(IThemeBrushes).IsAssignableFrom(t) && !t.IsAbstract);
                KVPGeneration(classAssemblies, attributeAssemblies, themebrushesAssemblies);
                _isloaded = true;
            }
        }
        public static void Apply(Type attributeType, Action<TransitionParams>? paramAction = null, Brush? windowBack = default)
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
        private static void KVPGeneration(IEnumerable<Type> classes, IEnumerable<Type> attributes, IEnumerable<Type> brushselectors)
        {
            foreach (var cs in classes)
            {
                StateMachine.InitializeTypes(cs);
                if (!StateMachine.SplitedPropertyInfos.TryGetValue(cs, out var group)) break;
                var unit = new ConcurrentDictionary<Type, State>();
                var hoverunit = new ConcurrentDictionary<Type, HoverActionManager>();
                foreach (var attribute in attributes)
                {
                    var properties = cs.GetProperties()
                    .Select(p => new
                    {
                        PropertyInfo = p,
                        Context = p.GetCustomAttribute(attribute, true) as IThemeAttribute,
                    });
                    var state = new State();
                    var hovermanager = new HoverActionManager();
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
                                    var value = info.Context.Value ?? info.Context.Parameters?.FirstOrDefault()?.ToString()?.ToBrush() ?? Brushes.Transparent;
                                    var focus = info.Context.FocusValue ?? value;
                                    hovermanager.ToTheme.AddProperty(info.PropertyInfo.Name, value);
                                    hovermanager.ToHover.AddProperty(info.PropertyInfo.Name, focus);
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
                    hoverunit.TryAdd(attribute, hovermanager);
                }
                TransitionSource.TryAdd(cs, unit);
            }
        }
    }
}
