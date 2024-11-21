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
        /// 资源队列
        /// </summary>
        public static ConcurrentDictionary<Type, ConcurrentQueue<object>> FetchQueue { get; internal set; } = new();
        /// <summary>
        /// 释放与回收Action
        /// </summary>
        public static ConcurrentDictionary<Type, Tuple<MethodInfo?, MethodInfo?>> Method { get; internal set; } = new();
        /// <summary>
        /// 回收策略配置项
        /// </summary>
        public static ConcurrentDictionary<Type, Tuple<int, MethodInfo?, int>> DisposeConfig { get; internal set; } = new();
        /// <summary>
        /// 回收队列
        /// </summary>
        public static ConcurrentDictionary<Type, ConcurrentQueue<object>> DisposeQueue { get; internal set; } = new();

        private static bool CanSourceAdded(ConcurrentQueue<object> fetchqueue, ConcurrentQueue<object> disposequeue, Tuple<int, MethodInfo?, int> config)//检查当前是否有可用资源
        {
            return fetchqueue.Count + disposequeue.Count < config.Item3;
        }
        private static void AddOneSource(Type type, ConcurrentQueue<object> fetchqueue)//新增一个资源
        {
            var instance = Activator.CreateInstance(type);
            if (instance == null)
            {
                throw new ArgumentException($"MPL03 类型[ {type.Name} ]在实例化时发生错误");
            }
            else
            {
                fetchqueue.Enqueue(instance);
            }
        }
        private static int IsSourceExsit(ConcurrentQueue<object> fetchqueue, ConcurrentQueue<object> disposequeue, Tuple<int, MethodInfo?, int> config)//检查当前是否有可用资源
        {
            return (fetchqueue.Count > 0, disposequeue.Count > config.Item1) switch
            {
                (true, _) => 1, //资源池可用
                (false, true) => 2, //回收池可用
                _ => 0 //资源不足
            };
        }
        private static object FetchWhileSourceExsitSource(ConcurrentQueue<object> fetchqueue, ConcurrentQueue<object> disposequeue, Tuple<MethodInfo?, MethodInfo?> method)//当资源池可用
        {
            if (fetchqueue.TryDequeue(out var instance))
            {
                method.Item1?.Invoke(instance, null);
                disposequeue.Enqueue(instance);
                return instance;
            }
            else
            {
                throw new ArgumentException("MPL08 资源队列拒绝访问 ____> 在尝试调取资源池资源时产生异常");
            }
        }
        private static object FetchWhileDisposeExsitSource(ConcurrentQueue<object> fetchqueue, ConcurrentQueue<object> disposequeue, Tuple<MethodInfo?, MethodInfo?> method)//当回收池可用
        {
            if (disposequeue.TryDequeue(out var instance))
            {
                method.Item2?.Invoke(instance, null);
                method.Item1?.Invoke(instance, null);
                disposequeue.Enqueue(instance);
                return instance;
            }
            else
            {
                throw new ArgumentException("MPL09 回收队列拒绝访问 ____> 在尝试自动回收资源并重新作为被调出的资源时产生异常");
            }
        }
        private static bool IsConditionOk(ConcurrentQueue<object> disposequeue, Tuple<int, MethodInfo?, int> config)
        {
            if (disposequeue.TryPeek(out var instance))
            {
                if (config.Item2?.Invoke(instance, null) is bool condition)
                {
                    return condition;
                }
                else
                {
                    throw new ArgumentException("MPL12 ");
                }
            }
            else
            {
                throw new ArgumentException("MPL11 ");
            }
        }

        /// <summary>
        /// 初始化对象池资源 , 通常建议先一步执行该函数 , 当然 , 直接调用Fetch/Dispose方法也是会触发该函数的
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
                                throw new Exception($"MPL03 类型[ {target.Type.Name} ]在实例化时发生错误");
                            }
                            else
                            {
                                value.Enqueue(instance);
                            }
                        }
                        var condition = true;
                        condition &= FetchQueue.TryAdd(target.Type, value);
                        condition &= Method.TryAdd(target.Type, Tuple.Create(target.MethodA, target.MethodB));
                        condition &= DisposeConfig.TryAdd(target.Type, Tuple.Create(target.Context.Critical, target.MethodC, target.Context.Max));
                        condition &= DisposeQueue.TryAdd(target.Type, new ConcurrentQueue<object>());
                        if (!condition)
                        {
                            throw new Exception($"MPL04 类型[ {target.Type.Name} ]在试图初始化对象池时发生错误");
                        }
                    }
                }
                _isloaded = true;
            }
        }
        /// <summary>
        /// 从对象池取出对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static object Fetch(Type type)//获取一个资源
        {
            InitializeSource();
            if (FetchQueue.TryGetValue(type, out var source) && DisposeQueue.TryGetValue(type, out var dispose) && Method.TryGetValue(type, out var method) && DisposeConfig.TryGetValue(type, out var config))
            {
                Func<object> func = (IsSourceExsit(source, dispose, config), CanSourceAdded(source, dispose, config)) switch
                {
                    (1, _) => () => //资源池可用
                    {
                        return FetchWhileSourceExsitSource(source, dispose, method);
                    }
                    ,
                    (2, false) => () => //资源池耗尽但回收池可用,不允许扩容
                    {
                        return FetchWhileDisposeExsitSource(source, dispose, method);
                    }
                    ,
                    (2, true) => () => //资源池耗尽但回收池可用,允许扩容
                    {
                        AddOneSource(type, source);
                        return FetchWhileSourceExsitSource(source, dispose, method);
                    }
                    ,
                    (0, true) => () => //无资源可用但允许扩容
                    {
                        AddOneSource(type, source);
                        return FetchWhileSourceExsitSource(source, dispose, method);
                    }
                    ,
                    (0, false) => () => //无资源可用且不允许扩容
                    {
                        throw new ArgumentException($"MPL10 类型[ {type.Name} ]的对象池需要扩容,但你设置的资源最大值[ {config.Item3} ]限制了本次扩容");
                    }
                    ,
                    _ => () => { throw new ArgumentException(); } //意外情况
                };
                return func.Invoke();
            }
            else
            {
                throw new ArgumentException($"MPL01 类型[ {type.Name} ]可能不受对象池管理,请使用[ {nameof(PoolAttribute)} ]标记它");
            }
        }
        /// <summary>
        /// ( 不必要 ) 释放对象至对象池 ，这个步骤通常由对象池自动完成
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static object Dispose(Type type)//释放一个资源
        {
            InitializeSource();
            if (FetchQueue.TryGetValue(type, out var source) && DisposeQueue.TryGetValue(type, out var dispose) && Method.TryGetValue(type, out var method) && DisposeConfig.TryGetValue(type, out var config))
            {
                Func<object> func = (IsSourceExsit(source, dispose, config), IsConditionOk(dispose, config)) switch
                {
                    (2, true) => () =>
                    {
                        if (dispose.TryDequeue(out var dised))
                        {
                            method.Item2?.Invoke(dised, null);
                            source.Enqueue(dised);
                            return dised;
                        }
                        else
                        {
                            throw new ArgumentException("MPL15");
                        }
                    }
                    ,
                    (2, false) => () => { throw new ArgumentException("MPL14"); }
                    ,
                    _ => () => { throw new ArgumentException("MPL13"); }
                };
                return func.Invoke();
            }
            else
            {
                throw new ArgumentException($"MPL01 类型[ {type.Name} ]可能不受对象池管理,请使用[ {nameof(PoolAttribute)} ]标记它");
            }
        }
    }
}
