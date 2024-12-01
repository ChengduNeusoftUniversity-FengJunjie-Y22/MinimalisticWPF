using System;
using System.Collections.Concurrent;
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
        public static T ApplyGlobalTheme<T>(this T source) where T : class
        {
            DynamicTheme.Awake();
            if (!DynamicTheme.GlobalInstance.Contains(source))
            {
                DynamicTheme.GlobalInstance.Add(source);
            }
            return source;
        }
        public static T ApplyTheme<T>(this T source, Type attributeType, Action<TransitionParams>? paramAction = null) where T : class
        {
            DynamicTheme.Awake();
            var type = source.GetType();
            if (DynamicTheme.TransitionSource.TryGetValue(type, out var statedic))
            {
                if (statedic.TryGetValue(attributeType, out var state))
                {
                    if (DynamicTheme.CurrentTheme.TryGetValue(source, out var current))
                    {
                        DynamicTheme.CurrentTheme.TryUpdate(source, attributeType, current);
                    }
                    else
                    {
                        DynamicTheme.CurrentTheme.TryAdd(source, attributeType);
                    }
                    source.BeginTransition(state, paramAction ?? TransitionParams.Theme);
                }
            }
            return source;
        }
        public static T ApplyThemeHover<T>(this T source, Action<TransitionParams>? paramAction = null) where T : class
        {
            DynamicTheme.Awake();
            if (DynamicTheme.CurrentTheme.TryGetValue(source, out var current))
            {
                if (DynamicTheme.HoverSource.TryGetValue(source.GetType(), out var statedic))
                {
                    if (statedic.TryGetValue(current, out var manager))
                    {
                        manager.Enter(source, paramAction);
                    }
                }
            }
            else
            {
                DynamicTheme.CurrentTheme.TryAdd(source, DynamicTheme.InitialTheme);
                source.ApplyThemeHover(paramAction);
            }
            return source;
        }
        public static T ApplyThemeNoHover<T>(this T source, Action<TransitionParams>? paramAction = null) where T : class
        {
            DynamicTheme.Awake();
            if (DynamicTheme.CurrentTheme.TryGetValue(source, out var current))
            {
                if (DynamicTheme.HoverSource.TryGetValue(source.GetType(), out var statedic))
                {
                    if (statedic.TryGetValue(current, out var manager))
                    {
                        manager.Leave(source, paramAction);
                    }
                }
            }
            else
            {
                DynamicTheme.CurrentTheme.TryAdd(source, DynamicTheme.InitialTheme);
                source.ApplyThemeNoHover(paramAction);
            }
            return source;
        }
    }
}
