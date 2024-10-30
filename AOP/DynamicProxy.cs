﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static MinimalisticWPF.ProxyInstance;

namespace MinimalisticWPF
{
    public static class DynamicProxy
    {
        public static T CreateProxy<T>(this T target) where T : IProxy
        {
            var type = typeof(T);
            dynamic proxy = DispatchProxy.Create<T, ProxyInstance>() ?? throw new InvalidOperationException();
            proxy._target = target;
            proxy._targetType = type;
            ProxyIDs.Add(proxy,proxy._localid);
            return proxy;
        }

        /// <summary>
        /// 设置属性getter器切面逻辑
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="start"></param>
        /// <param name="coverage"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static object SetPropertyGetter(this object source, string propertyName, Action? start, ProxyReturnHandler? coverage, Action? end)
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

        /// <summary>
        /// 设置属性setter器切面逻辑
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="start"></param>
        /// <param name="coverage"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static object SetPropertySetter(this object source, string propertyName, Action? start, Action? coverage, Action? end)
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

        /// <summary>
        /// 设置方法切面逻辑
        /// </summary>
        /// <param name="source"></param>
        /// <param name="methodName"></param>
        /// <param name="start"></param>
        /// <param name="coverage"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static object SetMethod(this object source, string methodName, Action? start, ProxyReturnHandler? coverage, Action? end)
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