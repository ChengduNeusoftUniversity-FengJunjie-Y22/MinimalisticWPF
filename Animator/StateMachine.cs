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
    public class StateMachine
    {
        private static int _maxFR = 240;

        public static int MaxFrameRate
        {
            get => _maxFR;
            set
            {
                _maxFR = Math.Clamp(value, 1, int.MaxValue);
            }
        }
        public static ConcurrentDictionary<Type, ConcurrentDictionary<object, StateMachine>> MachinePool { get; internal set; } = new();
        public static ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> PropertyInfos { get; internal set; } = new();
        public static ConcurrentDictionary<Type, Tuple<ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>>> SplitedPropertyInfos { get; internal set; } = new();

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

        public static void Dispose(params Type[] types)
        {
            foreach (var type in types)
            {
                MachinePool.TryRemove(type, out _);
            }
        }

        internal StateMachine(object viewModel, params State[] states)
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
