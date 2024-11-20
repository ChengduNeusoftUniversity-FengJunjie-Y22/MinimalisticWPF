using System;
using System.Collections;
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
        /// 自动回收次数
        /// </summary>
        public static ConcurrentDictionary<Type, int> Counter { get; internal set; } = new();

        /// <summary>
        /// 可用资源池
        /// </summary>
        public static ConcurrentDictionary<Type, ConcurrentQueue<object?>> Source { get; internal set; } = new();

        /// <summary>
        /// 由手动释放的资源的哈希
        /// </summary>
        public static ConcurrentDictionary<Type, HashSet<object?>> DisposeHash { get; internal set; } = new();

        /// <summary>
        /// 资源存取Action
        /// </summary>
        public static ConcurrentDictionary<Type, Tuple<MethodInfo?, MethodInfo?>> Method { get; internal set; } = new();

        /// <summary>
        /// 自动回收的配置信息
        /// </summary>
        public static ConcurrentDictionary<Type, Tuple<int, int, int>> AutoDisposeConfig { get; internal set; } = new();

        /// <summary>
        /// 自动回收栈
        /// </summary>
        public static ConcurrentDictionary<Type, ConcurrentQueue<object?>> AutoDisposeQueue { get; internal set; } = new();

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
                });
                foreach (var target in targets)
                {
                    if (target.Context != null)
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
                            Counter.TryAdd(target.Type, 0);
                            AutoDisposeConfig.TryAdd(target.Type, Tuple.Create(target.Context.Maximum, target.Context.CriticalDelta, target.Context.DisposeDelta));
                            AutoDisposeQueue.TryAdd(target.Type, new ConcurrentQueue<object?>());
                            DisposeHash.TryAdd(target.Type, new HashSet<object?>());
                        }
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
            var isSource = Source.TryGetValue(type, out var queue);
            var isMethod = Method.TryGetValue(type, out var method);
            var isDisposeHash = DisposeHash.TryGetValue(type, out var hash);
            var isAutoDispose = AutoDisposeConfig.TryGetValue(type, out var config);
            var isAutoDisposeQueue = AutoDisposeQueue.TryGetValue(type, out var disposequeue);
            var isCounter = Counter.TryGetValue(type, out var counter);
            if (!isSource || queue == null || disposequeue == null || config == null || hash == null) throw new ArgumentException($"MPL01 This type has an unexpected situation when initializing the object pool.\n=>{type.FullName}");
            Func<object?> func = (isAutoDispose) switch
            {
                (true) => () =>
                {
                    var result = GetInstance(type, queue, counter, disposequeue, config, method)
                    .InstanceFetchInvoke(method, methodparams)
                    .RunAutoDispose(type, config, hash, disposequeue, queue, method);
                    return result;
                }
                ,
                (false) => () =>
                {
                    var result = GetInstance(type, queue, -1, disposequeue, config, method)
                    .InstanceFetchInvoke(method, methodparams);
                    return result;
                }
            };
            return func.Invoke();
        }
        /// <summary>
        /// （ 手动 ）释放资源至对象池
        /// </summary>
        /// <param name="value">需要释放的对象</param>
        /// <param name="methodparams">触发对象回收函数时传入的参数</param>
        public static void Dispose(object? value, params object[] methodparams)
        {
            if (value == null) throw new ArgumentNullException("MPL02 An null object cannot be deallocated into an object pool");
            Type type = value.GetType();
            var isSource = Source.TryGetValue(type, out var queue);
            Method.TryGetValue(type, out var method);
            DisposeHash.TryGetValue(type, out var hash);
            if (!isSource) throw new ArgumentException($"MPL01 This type has an unexpected situation when initializing the object pool.\n=>{type.FullName}");
            if (AutoDisposeConfig.ContainsKey(type))
            {
                hash?.Add(value);
            }
            method?.Item2?.Invoke(value, methodparams);
            queue?.Enqueue(value);
        }

        private static object? GetInstance(Type type, ConcurrentQueue<object?> queue, int counter, ConcurrentQueue<object?> disposequeue, Tuple<int, int, int> config, Tuple<MethodInfo?, MethodInfo?>? method, params object?[] actionparams)
        {
            if (queue.TryDequeue(out var value))
            {
                disposequeue.Enqueue(value);
                return value;
            }
            else
            {
                if (type.GetPoolSemaphore() >= config.Item1 && disposequeue.TryDequeue(out var dis))
                {
                    method?.Item2?.Invoke(dis, actionparams);
                    disposequeue.Enqueue(dis);
                    return dis;
                }
                else
                {
                    return CreateNew(type, counter, disposequeue);
                }
            }
        }
        private static object? CreateNew(Type type, int counter, ConcurrentQueue<object?> disposequeue)
        {
            var result = Activator.CreateInstance(type).AddCounter(type, counter);
            disposequeue.Enqueue(result);
            return result;
        }
        private static object? InstanceFetchInvoke(this object? target, Tuple<MethodInfo?, MethodInfo?>? method, params object?[] values)
        {
            method?.Item1?.Invoke(target, values);
            return target;
        }
        private static object? InstanceDisposeInvoke(this object? target, Tuple<MethodInfo?, MethodInfo?>? method, params object?[] values)
        {
            method?.Item2?.Invoke(target, values);
            return target;
        }
        private static object? AddCounter(this object? target, Type type, int counter)
        {
            Counter.TryUpdate(type, counter + 1, counter);
            return target;
        }
        private static object? RunAutoDispose(this object? target, Type type, Tuple<int, int, int> config, HashSet<object?> hash, ConcurrentQueue<object?> disposequeue, ConcurrentQueue<object?> queue, Tuple<MethodInfo?, MethodInfo?>? method)
        {
            Counter.TryGetValue(type, out var counter);
            if (counter == config.Item2)
            {
                for (int i = 0; i < config.Item3; i++)
                {
                    var temp = DisposeStackAndHash(hash, disposequeue, queue, method);
                }
                var inc = queue.Count + config.Item3 < config.Item1 - 1 ? config.Item3 - 1 : config.Item1 - queue.Count - 1;
                for (var i = 0; i < inc; i++)
                {
                    queue.Enqueue(Activator.CreateInstance(type));
                }
                Counter.TryUpdate(type, 0, counter);
            }
            return target;
        }
        private static object? DisposeStackAndHash(HashSet<object?> hash, ConcurrentQueue<object?> disposequeue, ConcurrentQueue<object?> queue, Tuple<MethodInfo?, MethodInfo?>? method)
        {
            if (disposequeue.TryDequeue(out var value))
            {
                if (hash.Remove(value))
                {
                    return DisposeStackAndHash(hash, disposequeue, queue, method);
                }
                else
                {
                    InstanceDisposeInvoke(value, method);
                    queue.Enqueue(value);
                    return value;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
