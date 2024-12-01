using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static MinimalisticWPF.ProxyInstance;

namespace MinimalisticWPF
{
    public static class ExtensionForProxy
    {
        public static T CreateProxy<T>(this T target) where T : IProxy
        {
            var type = typeof(T);
            dynamic proxy = DispatchProxy.Create<T, ProxyInstance>() ?? throw new InvalidOperationException();
            proxy._target = target;
            proxy._targetType = type;
            ProxyIDs.Add(proxy, proxy._localid);
            return proxy;
        }
        public static T SetPropertyGetter<T>(this T source, string propertyName, ProxyHandler? start, ProxyHandler? coverage, ProxyHandler? end) where T : IProxy
        {
            if (!ProxyIDs.TryGetValue(source, out var id))
            {
                return source;
            }
            if (ProxyInstances.TryGetValue(id, out var instance))
            {
                var Name = $"get_{propertyName}";
                if (instance.GetterActions.ContainsKey(Name))
                {
                    instance.GetterActions[Name] = Tuple.Create(start, coverage, end);
                }
                else
                {
                    instance.GetterActions.Add(Name, Tuple.Create(start, coverage, end));
                }
            }
            return source;
        }
        public static T SetPropertySetter<T>(this T source, string propertyName, ProxyHandler? start, ProxyHandler? coverage, ProxyHandler? end) where T : IProxy
        {
            if (!ProxyIDs.TryGetValue(source, out var id))
            {
                return source;
            }
            if (ProxyInstances.TryGetValue(id, out var instance))
            {
                var Name = $"set_{propertyName}";
                if (instance.SetterActions.ContainsKey(Name))
                {
                    instance.SetterActions[Name] = Tuple.Create(start, coverage, end);
                }
                else
                {
                    instance.SetterActions.Add(Name, Tuple.Create(start, coverage, end));
                }
            }
            return source;
        }
        public static T SetMethod<T>(this T source, string methodName, ProxyHandler? start, ProxyHandler? coverage, ProxyHandler? end) where T : IProxy
        {
            if (!ProxyIDs.TryGetValue(source, out var id))
            {
                return source;
            }
            if (ProxyInstances.TryGetValue(id, out var instance))
            {
                if (instance.MethodActions.ContainsKey(methodName))
                {
                    instance.MethodActions[methodName] = Tuple.Create(start, coverage, end);
                }
                else
                {
                    instance.MethodActions.Add(methodName, Tuple.Create(start, coverage, end));
                }
            }
            return source;
        }
    }
}
