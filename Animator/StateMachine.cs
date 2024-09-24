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
        public StateCollection States { get; set; } = new StateCollection();

        internal TransferParams TransferParams { get; set; } = new TransferParams();
        /// <summary>
        /// 一帧持续的时间(单位: ms )
        /// </summary>
        public double DeltaTime { get => 1000.0 / Math.Clamp(TransferParams.FrameRate, 1, 240); }
        /// <summary>
        /// 总计需要的帧数
        /// </summary>
        public double FrameCount { get => Math.Clamp(TransferParams.Duration * Math.Clamp(TransferParams.FrameRate, 1, 240), 1, int.MaxValue); }

        public static StateMachine Create(object targetObj)
        {
            StateMachine result = new StateMachine(targetObj);
            return result;
        }
        public StateMachine SetStates(params State[] states)
        {
            if (states.Length == 0) States.Clear();

            foreach (var state in states)
            {
                var temp = States.FirstOrDefault(x => x.StateName == state.StateName);
                if (temp == null) States.Add(state);
                else throw new ArgumentException($"A state named [ {state.StateName} ] already exists in the collection.Modify the collection to ensure that the state name is unique");
            }

            return this;
        }

        public string? CurrentState { get; internal set; }
        internal AnimationInterpreter? Interpreter { get; set; }
        internal Queue<Tuple<string, Action<TransferParams>?>> Interpreters { get; set; } = new Queue<Tuple<string, Action<TransferParams>?>>();

        /// <summary>
        /// 转移至目标状态
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <param name="actionSet">设置过渡参数</param>
        /// <exception cref="ArgumentException"></exception>
        public void Transfer(string stateName, Action<TransferParams>? actionSet)
        {
            TransferParams temp = new TransferParams();
            actionSet?.Invoke(temp);
            if (Interpreter == null)
            {
                var task = Task.Run(() => InterpreterScheduler(stateName, actionSet));
            }
            else
            {
                var targetInterpreter = Interpreters.Where(x => x.Item1 == stateName).ToArray();
                if (targetInterpreter.Length != 0 && temp.IsUnique)
                {
                    return;
                }

                Interpreters.Enqueue(Tuple.Create(stateName, actionSet));
                if (!temp.IsQueue)
                {
                    Interpreter?.Interrupt();
                }
            }
        }
        private async void InterpreterScheduler(string stateName, Action<TransferParams>? actionSet)
        {
            var targetState = States.FirstOrDefault(x => x.StateName == stateName);
            if (targetState == null) throw new ArgumentException($"The State Named [ {stateName} ] Cannot Be Found");

            TransferParams transferParams = new TransferParams();
            actionSet?.Invoke(transferParams);
            TransferParams = transferParams;

            await Task.Delay((int)(TransferParams.WaitTime * 1000));

            AnimationInterpreter animationInterpreter = new AnimationInterpreter(this);
            animationInterpreter.IsLast = TransferParams.IsLast;
            animationInterpreter.DeltaTime = (int)DeltaTime;
            animationInterpreter.Start = TransferParams.Start;
            animationInterpreter.Update = TransferParams.Update;
            animationInterpreter.Completed = TransferParams.Completed;
            animationInterpreter.LateUpdate = TransferParams.LateUpdate;
            animationInterpreter.Acceleration = TransferParams.Acceleration;

            if (Application.Current == null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                animationInterpreter.Frams = ComputingFrames(targetState);
            });
            CurrentState = stateName;
            Interpreter = animationInterpreter;
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
        internal class AnimationInterpreter
        {
            internal AnimationInterpreter(StateMachine machine) { Machine = machine; }

            internal StateMachine Machine { get; set; }
            internal List<List<Tuple<PropertyInfo, List<object?>>>> Frams { get; set; } = new List<List<Tuple<PropertyInfo, List<object?>>>>();
            internal string StateName { get; set; } = string.Empty;
            internal int DeltaTime { get; set; } = 0;
            internal bool IsLast { get; set; } = true;
            internal bool IsRunning { get; set; } = false;
            internal bool IsStop { get; set; } = false;
            internal Action? Start { get; set; }
            internal Action? Update { get; set; }
            internal Action? Completed { get; set; }
            internal Action? LateUpdate { get; set; }
            internal double Acceleration { get; set; } = 0;

            /// <summary>
            /// 执行动画
            /// </summary>
            internal void Interpret()
            {
                if (IsStop || IsRunning) { WhileEnded(); return; }
                IsRunning = true;
                Machine.Interpreter = this;

                var Times = GetAccDeltaTime((int)Machine.FrameCount);

                if (Application.Current != null && Start != null)
                {
                    Application.Current.Dispatcher.Invoke(Start);
                }

                for (int i = 0; i < Machine.FrameCount; i++)
                //按帧遍历
                {
                    if (IsStop || Application.Current == null || Machine.Interpreter != this)
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

                WhileEnded();
            }

            /// <summary>
            /// 打断动画
            /// </summary>
            internal void Interrupt()
            {
                IsStop = IsRunning ? true : false;
            }

            /// <summary>
            /// 当动画终止时
            /// </summary>
            internal void WhileEnded()
            {
                if (Application.Current != null && Completed != null)
                {
                    Application.Current.Dispatcher.Invoke(Completed);
                }
                IsRunning = false;
                IsStop = false;
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
