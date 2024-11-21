using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;
using System.ComponentModel;
using System.Data;
using System.Windows.Threading;
using System.Collections;
using System.Windows.Controls;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Automation;
using System.Windows.Media;
using System.Collections.Concurrent;
using System.Security.RightsManagement;

namespace MinimalisticWPF
{
    /// <summary>
    /// 状态机
    /// <para>对于一个实例对象,状态机记录其属性信息,并且调度动画的生成、启动操作</para>
    /// </summary>
    public class StateMachine
    {
        private static int _maxFR = 240;

        /// <summary>
        /// 最大帧率限制
        /// </summary>
        public static int MaxFrameRate
        {
            get => _maxFR;
            set
            {
                _maxFR = Math.Clamp(value, 1, int.MaxValue);
            }
        }
        /// <summary>
        /// 对于任意使用Board启动动画的对象实例,全局只允许存在一台StateMachine用于为其加载过渡效果
        /// </summary>
        public static ConcurrentDictionary<Type, ConcurrentDictionary<object, StateMachine>> MachinePool { get; internal set; } = new();
        /// <summary>
        /// 类型中支持加载动画的属性
        /// </summary>
        public static ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> PropertyInfos { get; internal set; } = new();
        /// <summary>
        /// 依据属性类型不同做出的划分
        /// <para>Values</para>
        /// <para>Item1 double</para>
        /// <para>Item2 Brush</para>
        /// <para>Item3 Transform</para>
        /// <para>Item4 Point</para>
        /// <para>Item5 CornerRadius</para>
        /// <para>Item6 Thickness</para>
        /// <para>Item7 ILinearInterpolation</para>
        /// </summary>
        public static ConcurrentDictionary<Type, Tuple<ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>>> SplitedPropertyInfos = new();

        /// <summary>
        /// 创建/获取 一个用于管理指定对象过渡行为的状态机实例
        /// </summary>
        public static StateMachine Create(object targetObj, params State[] states)
        {
            var type = targetObj.GetType();
            if (MachinePool.TryGetValue(type, out var machinedictionary))
            {
                if (machinedictionary.TryGetValue(targetObj, out var machine))
                {
                    foreach (var state in states)
                    {
                        machine.States.Add(state);
                    }
                    return machine;
                }
                else
                {
                    var newMachine = new StateMachine(targetObj, states);
                    machinedictionary.TryAdd(targetObj, newMachine);
                    return newMachine;
                }
            }
            else
            {
                var newMachine = new StateMachine(targetObj, states);
                var newChildDic = new ConcurrentDictionary<object, StateMachine>();
                newChildDic.TryAdd(targetObj, newMachine);
                MachinePool.TryAdd(type, newChildDic);
                return newMachine;
            }
        }
        /// <summary>
        /// 预载实例对象过渡至指定State过程中的所有帧数据
        /// </summary>
        /// <param name="Target">目标对象</param>
        /// <param name="state">目标State</param>
        /// <param name="par"></param>
        /// <returns>帧数据</returns>
        public static List<List<Tuple<PropertyInfo, List<object?>>>>? PreloadFrames(object? Target, State state, TransitionParams par)
        {
            if (Target == null)
            {
                return null;
            }
            var machine = new StateMachine(Target, state);
            machine.TransitionParams = par;
            var result = ComputingFrames(state, machine);
            return result;
        }
        /// <summary>
        /// 计算从当前状态指向指定状态的过渡帧序列
        /// </summary>
        public static List<List<Tuple<PropertyInfo, List<object?>>>> ComputingFrames(State state, StateMachine machine)
        {
            List<List<Tuple<PropertyInfo, List<object?>>>> result = new List<List<Tuple<PropertyInfo, List<object?>>>>(7);

            var count = (int)machine.FrameCount;
            var fc = count == 0 ? 1 : count;
            result.Add(DoubleComputing(machine.Type, state, machine.Target, fc));
            result.Add(BrushComputing(machine.Type, state, machine.Target, fc));
            result.Add(TransformComputing(machine.Type, state, machine.Target, fc));
            result.Add(PointComputing(machine.Type, state, machine.Target, fc));
            result.Add(CornerRadiusComputing(machine.Type, state, machine.Target, fc));
            result.Add(ThicknessComputing(machine.Type, state, machine.Target, fc));
            result.Add(ILinearInterpolationComputing(machine.Type, state, machine.Target, fc));

            return result;
        }
        /// <summary>
        /// 插值计算
        /// </summary>
        public static List<Tuple<PropertyInfo, List<object?>>> DoubleComputing(Type type, State state, object Target, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item1.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(Target);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, ILinearInterpolation.DoubleComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        /// <summary>
        /// 插值计算
        /// </summary>
        public static List<Tuple<PropertyInfo, List<object?>>> BrushComputing(Type type, State state, object Target, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item2.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(Target);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, ILinearInterpolation.BrushComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        /// <summary>
        /// 插值计算
        /// </summary>
        public static List<Tuple<PropertyInfo, List<object?>>> TransformComputing(Type type, State state, object Target, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item3.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(Target);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, ILinearInterpolation.TransformComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        /// <summary>
        /// 插值计算
        /// </summary>
        public static List<Tuple<PropertyInfo, List<object?>>> PointComputing(Type type, State state, object Target, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item4.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(Target);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, ILinearInterpolation.PointComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        /// <summary>
        /// 插值计算
        /// </summary>
        public static List<Tuple<PropertyInfo, List<object?>>> CornerRadiusComputing(Type type, State state, object Target, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item5.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(Target);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, ILinearInterpolation.CornerRadiusComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        /// <summary>
        /// 插值计算
        /// </summary>
        public static List<Tuple<PropertyInfo, List<object?>>> ThicknessComputing(Type type, State state, object Target, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item6.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(Target);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, ILinearInterpolation.ThicknessComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        /// <summary>
        /// 插值计算
        /// </summary>
        public static List<Tuple<PropertyInfo, List<object?>>> ILinearInterpolationComputing(Type type, State state, object Target, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item7.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = (ILinearInterpolation?)propertyinfo.GetValue(Target);
                        var newValue = (ILinearInterpolation?)state.Values[propertyname];
                        if (currentValue != newValue && newValue != null)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, newValue.Interpolate(currentValue?.Current, newValue.Current, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        /// <summary>
        /// 初始化指定类型中受过渡系统支持的属性信息
        /// </summary>
        /// <param name="types">指定的若干类型</param>
        public static void InitializeTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                if (!PropertyInfos.ContainsKey(type))
                {
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanWrite && x.CanRead &&
                    (x.PropertyType == typeof(double)
                    || x.PropertyType == typeof(Brush)
                    || x.PropertyType == typeof(Transform)
                    || x.PropertyType == typeof(Point)
                    || x.PropertyType == typeof(CornerRadius)
                    || x.PropertyType == typeof(Thickness)
                    || typeof(ILinearInterpolation).IsAssignableFrom(x.PropertyType)
                    ));
                    var propdictionary = new ConcurrentDictionary<string, PropertyInfo>();
                    foreach (var property in properties)
                    {
                        propdictionary.TryAdd(property.Name, property);
                    }
                    PropertyInfos.TryAdd(type, propdictionary);
                    SplitedPropertyInfos.TryAdd(type, Tuple.Create(new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(double)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(Brush)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(Transform)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(Point)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(CornerRadius)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(Thickness)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => typeof(ILinearInterpolation).IsAssignableFrom(x.PropertyType)).ToDictionary(x => x.Name, x => x))));
                }
            }
        }
        /// <summary>
        /// 尝试获取指定类型中指定名称的属性信息
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <param name="propertyname">指定属性名称</param>
        /// <param name="result">输出属性信息</param>
        public static bool TryGetPropertyInfo(Type type, string propertyname, out PropertyInfo? result)
        {
            if (PropertyInfos.TryGetValue(type, out var propdic))
            {
                if (propdic.TryGetValue(propertyname, out var propertyInfo))
                {
                    result = propertyInfo;
                    return true;
                }
            }
            result = null;
            return false;
        }
        /// <summary>
        /// 尝试获取指定实例的已有状态机
        /// </summary>
        public static bool TryGetMachine(object target, out StateMachine? result)
        {
            if (MachinePool.TryGetValue(target.GetType(), out var machinedic))
            {
                if (machinedic.TryGetValue(target, out var machine))
                {
                    result = machine;
                    return true;
                }
            }
            result = null;
            return false;
        }
        /// <summary>
        /// 释放指定类型的所有已有状态机
        /// </summary>
        public static void Dispose(params Type[] types)
        {
            foreach (var type in types)
            {
                MachinePool.TryRemove(type, out _);
            }
        }

        private StateMachine(object viewModel, params State[] states)
        {
            Target = viewModel;
            Type = viewModel.GetType();
            InitializeTypes(Type);
            foreach (var state in states)
            {
                States.Add(state);
            }
        }

        public object Target { get; internal set; }
        public Type Type { get; internal set; }
        public StateCollection States { get; internal set; } = new();
        internal TransitionParams TransitionParams { get; set; } = new();
        internal bool IsReSet { get; set; } = false;
        public double DeltaTime { get => 1000.0 / Math.Clamp(TransitionParams.FrameRate, 1, MaxFrameRate); }
        public double FrameCount { get => Math.Clamp(TransitionParams.Duration * Math.Clamp(TransitionParams.FrameRate, 1, MaxFrameRate), 1, int.MaxValue); }
        public string? CurrentState { get; internal set; }
        public TransitionInterpreter? Interpreter { get; internal set; }
        public ConcurrentQueue<Tuple<string, TransitionParams, List<List<Tuple<PropertyInfo, List<object?>>>>?>> Interpreters { get; internal set; } = new();
        public List<TransitionInterpreter> UnSafeInterpreters { get; internal set; } = new();

        /// <summary>
        /// 重置状态机数据
        /// </summary>
        /// <param name="IsStopUnsafe">设为true可以重置Unsafe过渡组</param>
        public void ReSet(bool IsStopUnsafe = false)
        {
            IsReSet = true;
            CurrentState = null;
            Interpreter?.Interrupt();
            Interpreter = null;
            Interpreters.Clear();
            if (IsStopUnsafe)
            {
                foreach (var item in UnSafeInterpreters)
                {
                    item.Interrupt(true);
                }
            }
        }
        /// <summary>
        /// 手动触发状态机的调度
        /// </summary>
        /// <param name="stateName">目标状态的名称，必须先一步存在于Machine.States中</param>
        /// <param name="actionSet">设置过渡效果参数</param>
        /// <param name="preload">（可选）传入预先计算的帧序列</param>
        public void Transition(string stateName, Action<TransitionParams>? actionSet, List<List<Tuple<PropertyInfo, List<object?>>>>? preload = null)
        {
            IsReSet = false;

            TransitionParams temp = new TransitionParams();
            actionSet?.Invoke(temp);

            if (temp.IsUnSafe)
            {
                InterpreterScheduler(stateName, temp, preload);
                return;
            }

            if (Interpreter == null)
            {
                InterpreterScheduler(stateName, temp, preload);
            }
            else
            {
                var targetInterpreter = Interpreters.Where(x => x.Item1 == stateName).ToArray();
                if (targetInterpreter.Length != 0 && temp.IsUnique)
                {
                    return;
                }

                Interpreters.Enqueue(Tuple.Create(stateName, temp, preload));
                if (!temp.IsQueue)
                {
                    Interpreter?.Interrupt();
                }
            }
        }
        internal async void InterpreterScheduler(string stateName, TransitionParams actionSet, List<List<Tuple<PropertyInfo, List<object?>>>>? preload)
        {
            var targetState = States.FirstOrDefault(x => x.StateName == stateName);
            if (targetState == null) throw new ArgumentException($"The State Named [ {stateName} ] Cannot Be Found");

            TransitionParams = actionSet;

            TransitionInterpreter animationInterpreter = new(this)
            {
                IsLast = TransitionParams.IsLast,
                DeltaTime = (int)DeltaTime,
                Update = TransitionParams.Update,
                Completed = TransitionParams.Completed,
                LateUpdate = TransitionParams.LateUpdate,
                Acceleration = TransitionParams.Acceleration,
                IsUnSafe = TransitionParams.IsUnSafe,
                LoopTime = TransitionParams.LoopTime,
                IsAutoReverse = TransitionParams.IsAutoReverse,
                UpdateAsync = TransitionParams.UpdateAsync,
                LateUpdateAsync = TransitionParams.LateUpdateAsync,
                CompletedAsync = TransitionParams.CompletedAsync,
                UIPriority = TransitionParams.UIPriority,
                IsBeginInvoke = TransitionParams.IsBeginInvoke
            };

            if (Application.Current == null)
            {
                return;
            }
            else
            {
                if (TransitionParams.Start != null)
                {
                    TransitionParams.Start.Invoke();
                }
                if (TransitionParams.StartAsync != null)
                {
                    await TransitionParams.StartAsync.Invoke();
                }
            }

            animationInterpreter.Frams = preload ?? ComputingFrames(targetState, this);

            if (TransitionParams.IsUnSafe)
            {
                UnSafeInterpreters.Add(animationInterpreter);
            }
            else
            {
                CurrentState = stateName;
                Interpreter = animationInterpreter;
            }
            var task = Task.Run(() => { animationInterpreter.Interpret(); });
        }
    }
}
