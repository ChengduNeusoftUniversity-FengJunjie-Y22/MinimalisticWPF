using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF
{
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
