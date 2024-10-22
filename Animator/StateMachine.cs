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

        /// <param name="viewModel">受状态机控制的实例对象</param>
        /// <param name="states">所有该对象可能具备的状态</param>
        /// <exception cref="ArgumentException"></exception>
        internal StateMachine(object viewModel, params State[] states)
        {
            Target = viewModel;
            Type type = viewModel.GetType();

            Properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.CanRead)
                .ToArray();

            DoubleProperties = Properties.Where(x => x.PropertyType == typeof(double))
                .ToArray();
            BrushProperties = Properties.Where(x => x.PropertyType == typeof(Brush))
                .ToArray();
            TransformProperties = Properties.Where(x => x.PropertyType == typeof(Transform))
                .ToArray();
            PointProperties = Properties.Where(x => x.PropertyType == typeof(Point))
                .ToArray();
            CornerRadiusProperties = Properties.Where(x => x.PropertyType == typeof(CornerRadius))
                .ToArray();
            ThicknessProperties = Properties.Where(x => x.PropertyType == typeof(Thickness))
                .ToArray();
            ILinearInterpolationProperties = Properties.Where(x => typeof(ILinearInterpolation).IsAssignableFrom(x.PropertyType))
                .ToArray();

            foreach (var state in states)
            {
                var temp = States.FirstOrDefault(x => x.StateName == state.StateName);
                if (temp == null) States.Add(state);
                else throw new ArgumentException($"A state named [ {state.StateName} ] already exists in the collection.Modify the collection to ensure that the state name is unique");
            }
        }
        public PropertyInfo[] Properties { get; internal set; }
        public PropertyInfo[] DoubleProperties { get; internal set; }
        public PropertyInfo[] BrushProperties { get; internal set; }
        public PropertyInfo[] TransformProperties { get; internal set; }
        public PropertyInfo[] PointProperties { get; internal set; }
        public PropertyInfo[] CornerRadiusProperties { get; internal set; }
        public PropertyInfo[] ThicknessProperties { get; internal set; }
        public PropertyInfo[] ILinearInterpolationProperties { get; internal set; }
        public object Target { get; internal set; }
        public StateCollection States { get; internal set; } = new StateCollection();
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

        internal TransitionParams TransitionParams { get; set; } = new TransitionParams();
        internal bool IsReSet { get; set; } = false;

        /// <summary>
        /// 一帧持续的时间(单位: ms )
        /// </summary>
        public double DeltaTime { get => 1000.0 / Math.Clamp(TransitionParams.FrameRate, 1, MaxFrameRate); }
        /// <summary>
        /// 总计需要的帧数
        /// </summary>
        public double FrameCount { get => Math.Clamp(TransitionParams.Duration * Math.Clamp(TransitionParams.FrameRate, 1, MaxFrameRate), 1, int.MaxValue); }

        /// <summary>
        /// 创建一个用于管理指定对象过渡行为的状态机实例
        /// </summary>
        public static StateMachine Create(object targetObj)
        {
            StateMachine result = new StateMachine(targetObj);
            return result;
        }
        /// <summary>
        /// 手动添加State用于过渡
        /// </summary>
        public StateMachine SetStates(params State[] states)
        {
            if (states.Length == 0) States.Clear();

            foreach (var state in states)
            {
                States.Add(state);
            }

            return this;
        }

        public string? CurrentState { get; internal set; }
        public TransitionInterpreter? Interpreter { get; internal set; }
        public Queue<Tuple<string, TransitionParams, List<List<Tuple<PropertyInfo, List<object?>>>>?>> Interpreters { get; internal set; } = new Queue<Tuple<string, TransitionParams, List<List<Tuple<PropertyInfo, List<object?>>>>?>>();

        /// <summary>
        /// 重置状态机
        /// </summary>
        public void ReSet()
        {
            IsReSet = true;
            CurrentState = null;
            Interpreter?.Interrupt();
            Interpreter = null;
            Interpreters.Clear();
        }
        /// <summary>
        /// 过渡至目标状态
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <param name="actionSet">细节参数</param>
        /// <param name="preload">预载数据</param>
        /// <exception cref="ArgumentException"></exception>
        public void Transition(string stateName, Action<TransitionParams>? actionSet, List<List<Tuple<PropertyInfo, List<object?>>>>? preload = null)
        {
            IsReSet = false;

            TransitionParams temp = new TransitionParams();
            actionSet?.Invoke(temp);

            if (temp.IsUnSafe)
            {
                var task = Task.Run(() => InterpreterScheduler(stateName, temp, preload));
                return;
            }

            if (Interpreter == null)
            {
                var task = Task.Run(() => InterpreterScheduler(stateName, temp, preload));
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

            TransitionInterpreter animationInterpreter = new TransitionInterpreter(this);
            animationInterpreter.IsLast = TransitionParams.IsLast;
            animationInterpreter.DeltaTime = (int)DeltaTime;
            animationInterpreter.Update = TransitionParams.Update;
            animationInterpreter.Completed = TransitionParams.Completed;
            animationInterpreter.LateUpdate = TransitionParams.LateUpdate;
            animationInterpreter.Acceleration = TransitionParams.Acceleration;
            animationInterpreter.IsUnSafe = TransitionParams.IsUnSafe;
            animationInterpreter.LoopTime = TransitionParams.LoopTime;
            animationInterpreter.IsAutoReverse = TransitionParams.IsAutoReverse;
            animationInterpreter.UpdateAsync = TransitionParams.UpdateAsync;
            animationInterpreter.LateUpdateAsync = TransitionParams.LateUpdateAsync;
            animationInterpreter.CompletedAsync = TransitionParams.CompletedAsync;

            if (Application.Current == null)
            {
                return;
            }
            else
            {
                if (TransitionParams.Start != null)
                {
                    Application.Current.Dispatcher.Invoke(TransitionParams.Start);
                }
                if (TransitionParams.StartAsync != null)
                {
                    await TransitionParams.StartAsync.Invoke();
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                animationInterpreter.Frams = preload ?? ComputingFrames(targetState);
            });

            if (!TransitionParams.IsUnSafe)
            {
                CurrentState = stateName;
                Interpreter = animationInterpreter;
            }
            var task = Task.Run(() => { animationInterpreter.Interpret(); });
        }
        /// <summary>
        /// 预载实例对象过渡至指定State过程中的所有帧数据
        /// </summary>
        /// <param name="Target">目标对象</param>
        /// <param name="state">目标State</param>
        /// <returns>帧数据</returns>
        public static List<List<Tuple<PropertyInfo, List<object?>>>>? PreloadFrames(object? Target, State state, TransitionParams par)
        {
            if (Target == null)
            {
                return null;
            }
            var machine = new StateMachine(Target, state);
            machine.TransitionParams = par;
            var result = machine.ComputingFrames(state);
            return result;
        }
        internal List<List<Tuple<PropertyInfo, List<object?>>>> ComputingFrames(State state)
        {
            List<List<Tuple<PropertyInfo, List<object?>>>> result = new List<List<Tuple<PropertyInfo, List<object?>>>>();

            result.Add(DoubleComputing(state));
            result.Add(BrushComputing(state));
            result.Add(TransformComputing(state));
            result.Add(PointComputing(state));
            result.Add(CornerRadiusComputing(state));
            result.Add(ThicknessComputing(state));
            result.Add(ILinearInterpolationComputing(state));

            return result;
        }
        internal List<Tuple<PropertyInfo, List<object?>>> DoubleComputing(State state)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>();
            for (int i = 0; i < DoubleProperties.Length; i++)
            {
                if (state.Values.ContainsKey(DoubleProperties[i].Name))
                {
                    var currentValue = DoubleProperties[i].GetValue(Target);
                    var newValue = state[DoubleProperties[i].Name];
                    if (currentValue != newValue)
                    {
                        allFrames.Add(Tuple.Create(DoubleProperties[i], ILinearInterpolation.DoubleComputing(currentValue, newValue, (int)FrameCount)));
                    }
                }
            }

            return allFrames;
        }
        internal List<Tuple<PropertyInfo, List<object?>>> BrushComputing(State state)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>();
            for (int i = 0; i < BrushProperties.Length; i++)
            {
                if (state.Values.ContainsKey(BrushProperties[i].Name))
                {
                    var currentValue = BrushProperties[i].GetValue(Target);
                    var newValue = state[BrushProperties[i].Name];
                    if (currentValue != newValue)
                    {
                        allFrames.Add(Tuple.Create(BrushProperties[i], ILinearInterpolation.BrushComputing(currentValue, newValue, (int)FrameCount)));
                    }
                }
            }

            return allFrames;
        }
        internal List<Tuple<PropertyInfo, List<object?>>> TransformComputing(State state)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>();
            for (int i = 0; i < TransformProperties.Length; i++)
            {
                if (state.Values.ContainsKey(TransformProperties[i].Name))
                {
                    var currentValue = TransformProperties[i].GetValue(Target);
                    var newValue = state[TransformProperties[i].Name];
                    if (currentValue != newValue)
                    {
                        allFrames.Add(Tuple.Create(TransformProperties[i], ILinearInterpolation.TransformComputing(currentValue, newValue, (int)FrameCount)));
                    }
                }
            }

            return allFrames;
        }
        internal List<Tuple<PropertyInfo, List<object?>>> PointComputing(State state)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>();
            for (int i = 0; i < PointProperties.Length; i++)
            {
                if (state.Values.ContainsKey(PointProperties[i].Name))
                {
                    var currentValue = PointProperties[i].GetValue(Target);
                    var newValue = state[PointProperties[i].Name];
                    if (currentValue != newValue)
                    {
                        allFrames.Add(Tuple.Create(PointProperties[i], ILinearInterpolation.PointComputing(currentValue, newValue, (int)FrameCount)));
                    }
                }
            }

            return allFrames;
        }
        internal List<Tuple<PropertyInfo, List<object?>>> CornerRadiusComputing(State state)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>();
            for (int i = 0; i < CornerRadiusProperties.Length; i++)
            {
                if (state.Values.ContainsKey(CornerRadiusProperties[i].Name))
                {
                    var currentValue = CornerRadiusProperties[i].GetValue(Target);
                    var newValue = state[CornerRadiusProperties[i].Name];
                    if (currentValue != newValue)
                    {
                        allFrames.Add(Tuple.Create(CornerRadiusProperties[i], ILinearInterpolation.CornerRadiusComputing(currentValue, newValue, (int)FrameCount)));
                    }
                }
            }

            return allFrames;
        }
        internal List<Tuple<PropertyInfo, List<object?>>> ThicknessComputing(State state)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>();
            for (int i = 0; i < ThicknessProperties.Length; i++)
            {
                if (state.Values.ContainsKey(ThicknessProperties[i].Name))
                {
                    var currentValue = ThicknessProperties[i].GetValue(Target);
                    var newValue = state[ThicknessProperties[i].Name];
                    if (currentValue != newValue)
                    {
                        allFrames.Add(Tuple.Create(ThicknessProperties[i], ILinearInterpolation.ThicknessComputing(currentValue, newValue, (int)FrameCount)));
                    }
                }
            }

            return allFrames;
        }
        internal List<Tuple<PropertyInfo, List<object?>>> ILinearInterpolationComputing(State state)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>();
            for (int i = 0; i < ILinearInterpolationProperties.Length; i++)
            {
                if (state.Values.ContainsKey(ILinearInterpolationProperties[i].Name))
                {
                    var currentValue = (ILinearInterpolation?)ILinearInterpolationProperties[i].GetValue(Target);
                    var newValue = (ILinearInterpolation?)state[ILinearInterpolationProperties[i].Name];
                    if (currentValue != newValue)
                    {
                        allFrames.Add(Tuple.Create(ILinearInterpolationProperties[i], newValue.Interpolate(currentValue?.Current, newValue.Current, (int)FrameCount)));
                    }
                }
            }

            return allFrames;
        }
    }
}
