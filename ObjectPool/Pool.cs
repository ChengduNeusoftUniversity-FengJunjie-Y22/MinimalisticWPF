using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF
{
    public static class Pool
    {
        private static bool _isloaded = false;
        /// <summary>
        /// 类型在对象池中存储的实例
        /// </summary>
        public static ConcurrentDictionary<Type, ConcurrentQueue<object?>> Source { get; internal set; } = new();
        /// <summary>
        /// 类型实例从对象池中存/取时需要执行的方法
        /// </summary>
        public static ConcurrentDictionary<Type, Tuple<MethodInfo?, MethodInfo?>> Method { get; internal set; } = new();
        /// <summary>
        /// 自动回收的配置信息
        /// </summary>
        public static ConcurrentDictionary<Type, Tuple<int, int>> AutoDispose { get; internal set; } = new();
        /// <summary>
        /// 实例离开Pool的记录
        /// </summary>
        public static ConcurrentStack<object?> SourceToBeAutoDisposed { get; internal set; } = new();
        /// <summary>
        /// 激活对象池
        /// </summary>
        public static void Awake()
        {
            if (!_isloaded)
            {
                var targets = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Select(c => new
                {
                    Type = c,
                    Context = c.GetCustomAttribute(typeof(PoolAttribute), true) as PoolAttribute,
                    MethodA = c.GetMethods().FirstOrDefault(m => m.GetCustomAttribute(typeof(PoolFetchAttribute), true) != null),
                    MethodB = c.GetMethods().FirstOrDefault(m => m.GetCustomAttribute(typeof(PoolDisposeAttribute), true) != null)
                }).Where(r => r.Context != null);
                foreach (var target in targets)
                {
                    var value = new ConcurrentQueue<object?>();
                    for (int i = 0; i < target.Context.InitialCount; i++)
                    {
                        value.Enqueue(Activator.CreateInstance(target.Type));
                    }
                    Source.TryAdd(target.Type, value);
                    Method.TryAdd(target.Type, Tuple.Create(target.MethodA, target.MethodB));
                    if (target.Context.IsAutoDispose)
                    {
                        AutoDispose.TryAdd(target.Type, Tuple.Create(target.Context.CriticalValue, target.Context.DisposeDelta));
                    }
                }
                _isloaded = true;
            }
        }
        /// <summary>
        /// 从对象池取出
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodparams"></param>
        public static object? Fetch(Type type, params object[] methodparams)
        {
            if (!Source.TryGetValue(type, out var que) || que == null) return null;
            var result = (que.Count > 0) switch
            {
                true => OnlyMethod(type, que, methodparams),
                false => Generate(type, methodparams),
            };
            if (AutoDispose.TryGetValue(type, out var config))
            {
                SourceToBeAutoDisposed.Push(result);
                if (config.Item1 == que.Count)
                {
                    for (int i = 0; i < config.Item2; i++)
                    {
                        SourceToBeAutoDisposed.TryPop(out var dis);
                        que.Enqueue(dis);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 归还至对象池
        /// </summary>
        /// <param name="value"></param>
        /// <param name="methodparams"></param>
        public static void Dispose(object? value, params object[] methodparams)
        {
            if (value == null) return;
            Type type = value.GetType();
            if (!Source.TryGetValue(type, out var que) || que == null) return;
            if (Method.TryGetValue(type, out var method))
            {
                method.Item2?.Invoke(value, methodparams);
            }
            que.Enqueue(value);
        }

        private static object? Generate(Type type, params object[] methodparams)
        {
            var instance = Activator.CreateInstance(type);
            if (Method.TryGetValue(type, out var method))
            {
                method.Item1?.Invoke(instance, methodparams);
            }
            return instance;
        }
        private static object? OnlyMethod(Type type, ConcurrentQueue<object?> que, params object[] methodparams)
        {
            que.TryDequeue(out var result);
            if (Method.TryGetValue(type, out var method))
            {
                method.Item1?.Invoke(result, methodparams);
            }
            return result;
        }
    }
}
