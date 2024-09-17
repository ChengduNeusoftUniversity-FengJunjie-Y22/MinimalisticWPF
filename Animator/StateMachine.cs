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
using static System.TimeZoneInfo;
using static MinimalisticWPF.StateMachine;
using System.Security.Cryptography.Xml;
using System.Reflection.PortableExecutable;

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
                .ToArray();//所有可用属性

            DoubleProperties = Properties.Where(x => x.PropertyType == typeof(double))
                .ToArray();//筛选Double属性
            BrushProperties = Properties.Where(x => x.PropertyType == typeof(Brush))
                .ToArray();//筛选Brush属性

            foreach (var state in states)
            {
                var temp = States.FirstOrDefault(x => x.StateName == state.StateName);
                if (temp == null) States.Add(state);
                else throw new ArgumentException($"A state named [ {state.StateName} ] already exists in the collection.Modify the collection to ensure that the state name is unique");
            }//存储不重名的State
        }
        /// <summary>
        /// Public属性
        /// </summary>
        public PropertyInfo[] Properties { get; internal set; }
        /// <summary>
        /// Double属性
        /// </summary>
        public PropertyInfo[] DoubleProperties { get; internal set; }
        /// <summary>
        /// Brush属性
        /// </summary>
        public PropertyInfo[] BrushProperties { get; internal set; }
        /// <summary>
        /// 受状态机控制的对象
        /// </summary>
        public object Target { get; internal set; }
        /// <summary>
        /// 此状态机可导向的所有非条件驱动状态
        /// </summary>
        public StateCollection States { get; set; } = new StateCollection();

        internal TransferParams TransferParams { get; set; } = new TransferParams();
        /// <summary>
        /// 一帧持续的时间(单位: ms )
        /// </summary>
        public double DeltaTime { get => 1000.0 / TransferParams.FrameRate; }
        /// <summary>
        /// 总计需要的帧数
        /// </summary>
        public double FrameCount { get => (TransferParams.Duration == 0 ? 0.01 : TransferParams.Duration) * 1000 / DeltaTime; }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <param name="targetObj">需要应用状态机的实例</param>
        public static StateMachine Create(object targetObj)
        {
            StateMachine result = new StateMachine(targetObj);
            return result;
        }
        /// <summary>
        /// 设置可导向的State
        /// </summary>
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
        /// <summary>
        /// 正在执行中的解释器
        /// </summary>
        internal AnimationInterpreter? Interpreter { get; set; }
        /// <summary>
        /// 排队等待执行的解释器
        /// </summary>
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

                if (!temp.IsQueue)
                {
                    Interpreter?.Interrupt();
                    var task = Task.Run(() => InterpreterScheduler(stateName, actionSet));
                }
                else
                {
                    Interpreters.Enqueue(Tuple.Create(stateName, actionSet));
                }
            }
        }
        /// <summary>
        /// 调度解释器开始执行动画
        /// </summary>
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

            if (Application.Current == null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                animationInterpreter.Frams = ComputingFrames(targetState, TransferParams);
            });
            CurrentState = stateName;
            animationInterpreter.Interpret();
        }
        /// <summary>
        /// 计算属性的每个帧状态
        /// </summary>
        internal List<List<Tuple<PropertyInfo, List<object?>>>> ComputingFrames(State state, TransferParams transferParams)
        {
            List<List<Tuple<PropertyInfo, List<object?>>>> result = new List<List<Tuple<PropertyInfo, List<object?>>>>();

            result.Add(DoubleComputing(state, transferParams));
            result.Add(BrushComputing(state, transferParams));

            return result;
        }

        internal List<Tuple<PropertyInfo, List<object?>>> DoubleComputing(State state, TransferParams transferParams)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>();
            //预加载:[ 所有Double属性变化帧序列 ]
            List<Tuple<PropertyInfo, double?>> viewModels = new List<Tuple<PropertyInfo, double?>>();
            //预加载:[ 需要平滑过渡的属性 ]+[ 新值相对于旧值的变化量 ]
            for (int i = 0; i < DoubleProperties.Length; i++)
            {
                if (state.Values.ContainsKey(DoubleProperties[i].Name))
                {
                    double? now = (double?)DoubleProperties[i].GetValue(Target);
                    double? viewModel = (double?)state[DoubleProperties[i].Name];
                    if (now != viewModel)
                    {
                        now = now == null ? 0 : double.IsNaN((double)now) ? 0 : now;
                        viewModel = viewModel == null ? 0 : double.IsNaN((double)viewModel) ? 0 : viewModel;
                        viewModels.Add(Tuple.Create(DoubleProperties[i], viewModel - now));
                    }
                }
            }

            for (int i = 0; i < viewModels.Count; i++)
            {
                var currentValue = (double?)viewModels[i].Item1.GetValue(Target);
                List<object?> deltas = new List<object?>();
                for (int j = 0; j < FrameCount; j++)
                {
                    object? newValue = currentValue + viewModels[i].Item2 * (j + 1) / FrameCount;
                    deltas.Add(newValue);
                }
                allFrames.Add(Tuple.Create(viewModels[i].Item1, deltas));
            }

            return allFrames;
        }
        internal List<Tuple<PropertyInfo, List<object?>>> BrushComputing(State state, TransferParams transferParams)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new List<Tuple<PropertyInfo, List<object?>>>();
            List<Tuple<PropertyInfo, Brush?>> viewModels = new List<Tuple<PropertyInfo, Brush?>>();

            // 预加载需要平滑过渡的属性及其变化量
            for (int i = 0; i < BrushProperties.Length; i++)
            {
                if (state.Values.ContainsKey(BrushProperties[i].Name))
                {
                    Brush? now = (Brush?)BrushProperties[i].GetValue(Target);
                    Brush? viewModel = (Brush?)state[BrushProperties[i].Name];
                    if (!object.Equals(now, viewModel))
                    {
                        viewModels.Add(Tuple.Create(BrushProperties[i], viewModel));
                    }
                }
            }

            for (int i = 0; i < viewModels.Count; i++)
            {
                Brush? startValue = (Brush?)viewModels[i].Item1.GetValue(Target);
                Brush? endValue = viewModels[i].Item2;

                List<object?> deltas = CalculateGradientSteps(startValue ?? Brushes.Transparent, endValue ?? Brushes.Transparent, (int)FrameCount);
                allFrames.Add(Tuple.Create(viewModels[i].Item1, deltas));
            }

            return allFrames;
        }
        private static List<object?> CalculateGradientSteps(Brush brushA, Brush brushB, int steps)
        {
            // 验证输入是否为 SolidColorBrush
            if (!(brushA is SolidColorBrush solidBrushA) || !(brushB is SolidColorBrush solidBrushB))
            {
                throw new ArgumentException("Both brushes must be of type SolidColorBrush.");
            }

            Color colorA = solidBrushA.Color;
            Color colorB = solidBrushB.Color;

            var gradientSteps = new List<object?>();

            for (int i = 0; i <= steps; i++)
            {
                double ratio = (double)i / steps;
                Color interpolatedColor = InterpolateColor(colorA, colorB, ratio);
                Brush gradientBrush = new SolidColorBrush(interpolatedColor);
                gradientSteps.Add(gradientBrush);
            }

            return gradientSteps;
        }
        private static Color InterpolateColor(Color colorA, Color colorB, double ratio)
        {
            byte r = (byte)(colorA.R + (colorB.R - colorA.R) * ratio);
            byte g = (byte)(colorA.G + (colorB.G - colorA.G) * ratio);
            byte b = (byte)(colorA.B + (colorB.B - colorA.B) * ratio);
            byte a = (byte)(colorA.A + (colorB.A - colorA.A) * ratio);
            return Color.FromArgb(a, r, g, b);
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

            /// <summary>
            /// 执行动画
            /// </summary>
            internal async void Interpret()
            {
                if (IsStop || IsRunning) { WhileEnded(); return; }
                IsRunning = true;
                Machine.Interpreter = this;

                try
                {
                    Start?.Invoke();
                }
                catch
                {
                    if (Application.Current != null && Start != null)
                    {
                        Application.Current.Dispatcher.Invoke(Start);
                    }
                }

                for (int i = 0; i < Machine.FrameCount; i++)
                //按帧遍历
                {
                    if (IsStop || Application.Current == null)
                    {
                        WhileEnded();
                        return;
                    }

                    try
                    {
                        Update?.Invoke();
                    }
                    catch
                    {
                        if (Application.Current != null && Update != null)
                        {
                            Application.Current.Dispatcher.Invoke(Update);
                        }
                    }

                    for (int j = 0; j < Frams.Count; j++)
                    //按不同类属性遍历
                    {
                        for (int k = 0; k < Frams[j].Count; k++)
                        //按同类属性不同名遍历
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (Frams.Count > 0 && j >= 0 && j < Frams.Count)
                                {
                                    if (Frams[j].Count > 0 && k >= 0 && k < Frams[j].Count)
                                    {
                                        if (Frams[j][k].Item2.Count > 0 && i >= 0 && i < Frams[j][k].Item2.Count)
                                        {
                                            Frams[j][k].Item1.SetValue(Machine.Target, Frams[j][k].Item2[i]);
                                        }
                                        else
                                        {
                                            WhileEnded();
                                        }
                                    }
                                    else
                                    {
                                        WhileEnded();
                                    }
                                }
                                else
                                {
                                    WhileEnded();
                                }
                            });
                        }
                    }

                    try
                    {
                        LateUpdate?.Invoke();
                    }
                    catch
                    {
                        if (Application.Current != null && LateUpdate != null)
                        {
                            Application.Current.Dispatcher.Invoke(LateUpdate);
                        }
                    }

                    await Task.Delay(DeltaTime);
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
                try
                {
                    Completed?.Invoke();
                }
                catch
                {
                    if (Application.Current != null && Completed != null)
                    {
                        Application.Current.Dispatcher.Invoke(Completed);
                    }
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
        }
    }
}
