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
    /// </summary>
    public class StateMachine
    {
        private int _maxFR = 240;

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
        public int MaxFrameRate
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
        public Queue<Tuple<string, TransitionParams>> Interpreters { get; internal set; } = new Queue<Tuple<string, TransitionParams>>();

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
        /// <exception cref="ArgumentException"></exception>
        public void Transition(string stateName, Action<TransitionParams>? actionSet)
        {
            IsReSet = false;

            TransitionParams temp = new TransitionParams();
            actionSet?.Invoke(temp);

            if (temp.IsUnSafe)
            {
                var task = Task.Run(() => InterpreterScheduler(stateName, temp));
                return;
            }

            if (Interpreter == null)
            {
                var task = Task.Run(() => InterpreterScheduler(stateName, temp));
            }
            else
            {
                var targetInterpreter = Interpreters.Where(x => x.Item1 == stateName).ToArray();
                if (targetInterpreter.Length != 0 && temp.IsUnique)
                {
                    return;
                }

                Interpreters.Enqueue(Tuple.Create(stateName, temp));
                if (!temp.IsQueue)
                {
                    Interpreter?.Interrupt();
                }
            }
        }
        internal void InterpreterScheduler(string stateName, TransitionParams actionSet)
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

            if (Application.Current == null)
            {
                return;
            }


            Application.Current.Dispatcher.Invoke(() =>
            {
                TransitionParams.Start?.Invoke();
                animationInterpreter.Frams = ComputingFrames(targetState);
            });

            if (!TransitionParams.IsUnSafe)
            {
                CurrentState = stateName;
                Interpreter = animationInterpreter;
            }
            var task = Task.Run(() => { animationInterpreter.Interpret(); });
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

        /// <summary>
        /// 动画解释器
        /// </summary>
        public class TransitionInterpreter
        {
            internal TransitionInterpreter(StateMachine machine) { Machine = machine; }

            internal StateMachine Machine { get; set; }
            internal List<List<Tuple<PropertyInfo, List<object?>>>> Frams { get; set; } = new List<List<Tuple<PropertyInfo, List<object?>>>>();
            /// <summary>
            /// 解释器描述对象指向的State的名称
            /// </summary>
            public string StateName { get; internal set; } = string.Empty;
            internal int DeltaTime { get; set; } = 0;
            internal bool IsLast { get; set; } = true;
            /// <summary>
            /// 解释器是否处于运行中
            /// </summary>
            public bool IsRunning { get; internal set; } = false;
            internal bool IsStop { get; set; } = false;
            internal Action? Update { get; set; }
            internal Action? Completed { get; set; }
            internal Action? LateUpdate { get; set; }
            internal double Acceleration { get; set; } = 0;
            internal bool IsUnSafe { get; set; } = false;
            internal int LoopTime { get; set; } = 0;
            internal bool IsAutoReverse { get; set; } = false;

            /// <summary>
            /// 执行动画
            /// </summary>
            public void Interpret()
            {
                if (IsStop || IsRunning) { WhileEnded(); return; }
                IsRunning = true;

                var Times = GetAccDeltaTime((int)Machine.FrameCount);

                for (int x = 0; LoopTime == int.MaxValue ? true : x <= LoopTime; x++)
                {
                    for (int i = 0; i < Machine.FrameCount; i++)
                    //按帧遍历
                    {
                        if (IsStop || Application.Current == null || (IsUnSafe ? false : Machine.IsReSet || Machine.Interpreter != this))
                        {
                            WhileEnded();
                            return;
                        }

                        if (Application.Current != null && Update != null)
                        {
                            Application.Current.Dispatcher.Invoke(Update);
                        }

                        for (int j = 0; j < Frams.Count; j++)
                        //按不同类属性遍历
                        {
                            for (int k = 0; k < Frams[j].Count; k++)
                            //按同类属性不同名遍历
                            {
                                if (IsFrameIndexRight(i, j, k) && Application.Current != null)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        Frams[j][k].Item1.SetValue(Machine.Target, Frams[j][k].Item2[i]);
                                    });
                                }
                                else
                                {
                                    WhileEnded();
                                }
                            }
                        }

                        if (Application.Current != null && LateUpdate != null)
                        {
                            Application.Current.Dispatcher.Invoke(LateUpdate);
                        }

                        Thread.Sleep(Acceleration == 0 ? DeltaTime : i < Times.Count & Times.Count > 0 ? Times[i] : DeltaTime);
                    }

                    if (IsAutoReverse)
                    {
                        for (int i = Machine.FrameCount > 1 ? (int)Machine.FrameCount - 1 : 0; i > -1; i--)
                        //按帧遍历
                        {
                            if (IsStop || Application.Current == null || (IsUnSafe ? false : Machine.IsReSet || Machine.Interpreter != this))
                            {
                                WhileEnded();
                                return;
                            }

                            if (Application.Current != null && Update != null)
                            {
                                Application.Current.Dispatcher.Invoke(Update);
                            }

                            for (int j = 0; j < Frams.Count; j++)
                            //按不同类属性遍历
                            {
                                for (int k = 0; k < Frams[j].Count; k++)
                                //按同类属性不同名遍历
                                {
                                    if (IsFrameIndexRight(i, j, k) && Application.Current != null)
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            Frams[j][k].Item1.SetValue(Machine.Target, Frams[j][k].Item2[i]);
                                        });
                                    }
                                    else
                                    {
                                        WhileEnded();
                                    }

                                }
                            }

                            if (Application.Current != null && LateUpdate != null)
                            {
                                Application.Current.Dispatcher.Invoke(LateUpdate);
                            }

                            Thread.Sleep(Acceleration == 0 ? DeltaTime : i < Times.Count & Times.Count > 0 ? Times[i] : DeltaTime);
                        }
                    }
                }

                WhileEnded();
            }

            /// <summary>
            /// 打断动画
            /// </summary>
            public void Interrupt()
            {
                if (IsUnSafe) return;
                IsStop = IsRunning ? true : false;
            }

            /// <summary>
            /// 当动画终止时
            /// </summary>
            internal void WhileEnded()
            {
                if (IsUnSafe || Machine.IsReSet) return;

                if (Application.Current != null && Completed != null)
                {
                    Application.Current.Dispatcher.Invoke(Completed);
                }
                IsRunning = false;
                IsStop = false;

                if (Machine.Interpreter == this)
                {
                    Machine.Interpreter = null;
                    if (IsLast)
                    {
                        Machine.Interpreters.Clear();
                    }
                    if (Machine.Interpreters.Count > 0)
                    {
                        var newAni = Machine.Interpreters.Dequeue();
                        Machine.InterpreterScheduler(newAni.Item1, newAni.Item2);
                    }
                    Machine.CurrentState = null;
                }
            }

            /// <summary>
            /// 计算应用加速度后的时间间隔
            /// </summary>
            internal List<int> GetAccDeltaTime(int Steps)
            {
                List<int> result = new List<int>();
                if (Acceleration == 0) return result;

                var acc = Math.Clamp(Acceleration, -1, 1);
                var start = DeltaTime * (1 + acc);
                var end = DeltaTime * (1 - acc);
                var delta = end - start;
                for (int i = 0; i < Steps; i++)
                {
                    var t = (double)(i + 1) / Steps;
                    result.Add((int)(start + t * delta));
                }

                return result;
            }

            /// <summary>
            /// 判断当前循环步骤是否可以正确访问帧序列
            /// </summary>
            internal bool IsFrameIndexRight(int i, int j, int k)
            {
                if (Frams.Count > 0 && j >= 0 && j < Frams.Count)
                {
                    if (Frams[j].Count > 0 && k >= 0 && k < Frams[j].Count)
                    {
                        if (Frams[j][k].Item2.Count > 0 && i >= 0 && i < Frams[j][k].Item2.Count)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
