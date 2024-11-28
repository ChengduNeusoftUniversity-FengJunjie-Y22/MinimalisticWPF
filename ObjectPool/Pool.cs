using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MinimalisticWPF
{
    public static class Pool
    {
        private static bool _isloaded = false;

        /// <summary>
        /// 初始化对象池资源
        /// </summary>
        public static void InitializeSource()
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
                    MethodB = c.GetMethods().FirstOrDefault(m => m.GetCustomAttribute(typeof(PoolDisposeAttribute), true) != null),
                    MethodC = c.GetMethods().FirstOrDefault(m => m.GetCustomAttribute(typeof(PoolDisposeConditionAttribute), true) != null),
                });
                foreach (var target in targets)
                {
                    if (target.Context != null)
                    {
                        var value = new ConcurrentQueue<object>();
                        for (int i = 0; i < target.Context.Initial; i++)
                        {
                            var instance = Activator.CreateInstance(target.Type);
                            if (instance == null)
                            {
                                throw new Exception($"MPL01 类型[ {target.Type.FullName} ]在试图初始化对象池时发生错误");
                            }
                            else
                            {
                                value.Enqueue(instance);
                            }
                        }
                        var condition = true;
                        condition &= Source.TryAdd(target.Type, value);
                        condition &= Config.TryAdd(target.Type, Tuple.Create(target.Context.Max, target.MethodA, target.MethodB, target.MethodC));
                        condition &= Dispose.TryAdd(target.Type, new HashSet<object>(target.Context.Max));
                        if (!condition)
                        {
                            throw new Exception($"MPL01 类型[ {target.Type.FullName} ]在试图初始化对象池时发生错误");
                        }
                    }
                }
                _isloaded = true;
            }
        }

        /// <summary>
        /// 可用资源
        /// </summary>
        public static ConcurrentDictionary<Type, ConcurrentQueue<object>> Source { get; internal set; } = new();
        /// <summary>
        /// 配置
        /// </summary>
        public static ConcurrentDictionary<Type, Tuple<int, MethodInfo?, MethodInfo?, MethodInfo?>> Config { get; internal set; } = new();
        /// <summary>
        /// 占用资源
        /// </summary>
        public static ConcurrentDictionary<Type, HashSet<object>> Dispose { get; internal set; } = new();
        /// <summary>
        /// 当前执行中的扫描
        /// </summary>
        public static ConcurrentDictionary<Type, Task> Monitor { get; internal set; } = new();
        private static ConcurrentDictionary<Type, CancellationTokenSource> Tokens { get; } = new();

        /// <summary>
        /// 取出资源
        /// </summary>
        /// <param name="type">资源类型</param>
        public static object Fetch(Type type)
        {
            InitializeSource();
            if (Source.TryGetValue(type, out var source) && Dispose.TryGetValue(type, out var dispose) && Config.TryGetValue(type, out var config))
            {
                var sem = source.Count + dispose.Count;
                var isSourceExsit = sem > 0;
                var canSourceAdd = sem < config.Item1;
                Func<object> func = (isSourceExsit, canSourceAdd) switch
                {
                    (true, _) => () =>
                    {
                        if (source.TryDequeue(out var obj))
                        {
                            dispose.Add(obj);
                            config.Item2?.Invoke(obj, null);
                            return obj;
                        }
                        else
                        {
                            throw new ArgumentException($"MPL02 [ {type.FullName} ]资源队列为空或拒绝了本次调取");
                        }
                    }
                    ,
                    (false, true) => () =>
                    {
                        var newValue = Activator.CreateInstance(type);
                        if (newValue == null)
                        {
                            throw new ArgumentException($"MPL03 [ {type.FullName} ]扩容时未能成功创建实例");
                        }
                        else
                        {
                            config.Item2?.Invoke(newValue, null);
                            dispose.Add(newValue);
                            return newValue;
                        }
                    }
                    ,
                    (false, false) => () =>
                    {
                        var previewdisposed = dispose.FirstOrDefault(x => config.Item4?.Invoke(x, null) is bool condition && condition);
                        if (previewdisposed == null)
                        {
                            throw new ArgumentException($"MPL04 [ {type.FullName} ]资源需求溢出,请考虑增加最大资源数或加快扫描频率");
                        }
                        else
                        {
                            config.Item3?.Invoke(previewdisposed, null);
                            config.Item2?.Invoke(previewdisposed, null);
                            return previewdisposed;
                        }
                    }
                };
                return func.Invoke();
            }
            else
            {
                throw new ArgumentException($"MPL01 类型[ {type.FullName} ]在试图初始化对象池时发生错误");
            }
        }
        /// <summary>
        /// 试图从对象池取出一个资源
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="result">可能的资源</param>
        /// <returns>bool 是否成功取出资源</returns>
        public static bool TryFetch(Type type, out object result)
        {
            InitializeSource();
            result = string.Empty;
            if (Source.TryGetValue(type, out var source) && Dispose.TryGetValue(type, out var dispose) && Config.TryGetValue(type, out var config))
            {
                var sem = source.Count + dispose.Count;
                var isSourceExsit = sem > 0;
                var canSourceAdd = sem < config.Item1;
                object? temp = null;
                Func<bool> func = (isSourceExsit, canSourceAdd) switch
                {
                    (true, _) => () =>
                    {
                        if (source.TryDequeue(out var obj))
                        {
                            dispose.Add(obj);
                            config.Item2?.Invoke(obj, null);
                            temp = obj;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    ,
                    (false, true) => () =>
                    {
                        var newValue = Activator.CreateInstance(type);
                        if (newValue == null)
                        {
                            return false;
                        }
                        else
                        {
                            config.Item2?.Invoke(newValue, null);
                            dispose.Add(newValue);
                            temp = newValue;
                            return true;
                        }
                    }
                    ,
                    (false, false) => () =>
                    {
                        var previewdisposed = dispose.FirstOrDefault(x => config.Item4?.Invoke(x, null) is bool condition && condition);
                        if (previewdisposed == null)
                        {
                            return false;
                        }
                        else
                        {
                            config.Item3?.Invoke(previewdisposed, null);
                            config.Item2?.Invoke(previewdisposed, null);
                            temp = previewdisposed;
                            return true;
                        }
                    }
                };
                var invoked = func.Invoke();
                if (temp != null)
                {
                    result = temp;
                }
                return invoked;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 启用自动回收
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="scanspan">扫描间隔（毫秒）</param>
        public static void RunAutoDispose(Type type, int scanspan)
        {
            InitializeSource();
            if (!Monitor.TryGetValue(type, out _) && Source.TryGetValue(type, out var source) && Dispose.TryGetValue(type, out var dispose) && Config.TryGetValue(type, out var config))
            {
                var cts = new CancellationTokenSource();               
                var token = cts.Token;
                var task = Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        var target = new List<object>(dispose.Count);
                        foreach (object item in dispose.Where(o => config.Item4?.Invoke(o, null) is bool condition && condition))
                        {
                            target.Add(item);
                        }
                        foreach (object item in target)
                        {
                            dispose.Remove(item);
                            source.Enqueue(item);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                config.Item3?.Invoke(item, null);
                            });
                        }
                        await Task.Delay(scanspan, token);
                    }
                }, token);
                Tokens.TryAdd(type, cts);
                Monitor.TryAdd(type, task);
            }
            else
            {
                throw new InvalidOperationException($"MPL05 [ {type.FullName} ]未启用自动回收或没有挂载[ {nameof(PoolAttribute)} ]");
            }
        }
        /// <summary>
        /// 停止自动回收
        /// </summary>
        /// <param name="type">类型</param>
        public static void StopAutoDispose(Type type)
        {
            if (Monitor.TryGetValue(type, out var task) && Tokens.TryGetValue(type, out var cts))
            {
                cts.Cancel();
                Monitor.TryRemove(type, out _);
                Tokens.TryRemove(type, out _);
            }
        }
        /// <summary>
        /// 获取当前类型在对象池中的总资源数
        /// </summary>
        /// <param name="source"></param>
        /// <returns>
        /// int 小于0表示对象池未能正确加载此类型
        /// </returns>
        public static int GetPoolSemaphore(this Type source)
        {
            return (Source.TryGetValue(source, out var pool) ? pool.Count : -1) + (Dispose.TryGetValue(source, out var disc) ? disc.Count : -1);
        }
        /// <summary>
        /// 强制回收指定资源
        /// </summary>
        /// <param name="sources"></param>
        /// <returns>int 回收数</returns>
        public static int ForceDispose(params object[] sources)
        {
            var count = 0;
            foreach (var local in sources)
            {
                var type = local.GetType();
                if (Source.TryGetValue(type, out var source) && Dispose.TryGetValue(type, out var dispose) && Config.TryGetValue(type, out var config))
                {
                    if (dispose.Remove(local))
                    {
                        config.Item2?.Invoke(local, null);
                        source.Enqueue(local);
                        count++;
                    }
                }
            }
            return count;
        }
        /// <summary>
        /// 强制回收指定类型的所有资源
        /// </summary>
        /// <param name="types"></param>
        /// <returns>int 回收数</returns>s
        public static int ForceDispose(params Type[] types)
        {
            var count = 0;
            foreach (var type in types)
            {
                if (Source.TryGetValue(type, out var source) && Dispose.TryGetValue(type, out var dispose) && Config.TryGetValue(type, out var config))
                {
                    foreach (var item in dispose)
                    {
                        config.Item2?.Invoke(item, null);
                        source.Enqueue(item);
                        count++;
                    }
                    dispose.Clear();
                }
            }
            return count;
        }
    }
}
