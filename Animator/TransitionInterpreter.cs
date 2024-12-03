using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MinimalisticWPF
{
    public class TransitionInterpreter
    {
        internal TransitionInterpreter(StateMachine machine) { Machine = machine; }

        internal StateMachine Machine { get; set; }
        internal List<List<Tuple<PropertyInfo, List<object?>>>> Frams { get; set; } = new List<List<Tuple<PropertyInfo, List<object?>>>>();
        public string StateName { get; internal set; } = string.Empty;
        internal int DeltaTime { get; set; } = 0;
        internal bool IsLast { get; set; } = true;
        public bool IsRunning { get; internal set; } = false;
        internal bool IsStop { get; set; } = false;
        public Action? Update { get; set; }
        public Action? LateUpdate { get; set; }
        public Action? Completed { get; set; }
        public Func<Task>? UpdateAsync { get; set; }
        public Func<Task>? LateUpdateAsync { get; set; }
        public Func<Task>? CompletedAsync { get; set; }
        internal double Acceleration { get; set; } = 0;
        internal bool IsUnSafe { get; set; } = false;
        internal int LoopTime { get; set; } = 0;
        internal bool IsAutoReverse { get; set; } = false;
        internal DispatcherPriority UIPriority { get; set; } = DispatcherPriority.Render;
        internal bool IsBeginInvoke { get; set; } = false;

        public async void Interpret()
        {
            if (IsStop || IsRunning) { WhileEnded(); return; }
            IsRunning = true;

            var Times = GetAccDeltaTime((int)Machine.FrameCount);

            for (int x = 0; LoopTime == int.MaxValue ? true : x <= LoopTime; x++)
            {
                for (int i = 0; i < Machine.FrameCount; i++)
                {
                    if (IsStop || Application.Current == null || (IsUnSafe ? false : Machine.IsReSet || Machine.Interpreter != this))
                    {
                        WhileEnded();
                        return;
                    }

                    if (Application.Current != null)
                    {
                        if (Update != null)
                        {
                            Application.Current.Dispatcher.Invoke(Update);
                        }
                        if (UpdateAsync != null)
                        {
                            await UpdateAsync.Invoke();
                        }
                    }

                    for (int j = 0; j < Frams.Count; j++)
                    {
                        for (int k = 0; k < Frams[j].Count; k++)
                        {
                            if (IsFrameIndexRight(i, j, k) && Application.Current != null)
                            {
                                if (IsBeginInvoke)
                                {
                                    try
                                    {
                                        await Application.Current.Dispatcher.BeginInvoke(UIPriority, () =>
                                        {
                                            Frams[j][k].Item1.SetValue(Machine.Target, Frams[j][k].Item2[i]);
                                        });
                                    }
                                    catch { }
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(UIPriority, () =>
                                    {
                                        Frams[j][k].Item1.SetValue(Machine.Target, Frams[j][k].Item2[i]);
                                    });
                                }
                            }
                        }
                    }

                    if (Application.Current != null)
                    {
                        if (LateUpdate != null)
                        {
                            Application.Current.Dispatcher.Invoke(LateUpdate);
                        }
                        if (LateUpdateAsync != null)
                        {
                            await LateUpdateAsync.Invoke();
                        }
                    }

                    Thread.Sleep(Acceleration == 0 ? DeltaTime : i < Times.Count & Times.Count > 0 ? Times[i] : DeltaTime);
                }

                if (IsAutoReverse)
                {
                    for (int i = Machine.FrameCount > 1 ? (int)Machine.FrameCount - 1 : 0; i > -1; i--)
                    {
                        if (IsStop || Application.Current == null || (IsUnSafe ? false : Machine.IsReSet || Machine.Interpreter != this))
                        {
                            WhileEnded();
                            return;
                        }

                        if (Application.Current != null)
                        {
                            if (Update != null)
                            {
                                Application.Current.Dispatcher.Invoke(Update);
                            }
                            if (UpdateAsync != null)
                            {
                                await UpdateAsync.Invoke();
                            }
                        }

                        for (int j = 0; j < Frams.Count; j++)
                        {
                            for (int k = 0; k < Frams[j].Count; k++)
                            {
                                if (IsFrameIndexRight(i, j, k) && Application.Current != null)
                                {
                                    if (IsBeginInvoke)
                                    {
                                        try
                                        {
                                            await Application.Current.Dispatcher.BeginInvoke(UIPriority, () =>
                                            {
                                                Frams[j][k].Item1.SetValue(Machine.Target, Frams[j][k].Item2[i]);
                                            });
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke(UIPriority, () =>
                                        {
                                            Frams[j][k].Item1.SetValue(Machine.Target, Frams[j][k].Item2[i]);
                                        });
                                    }
                                }
                            }
                        }

                        if (Application.Current != null)
                        {
                            if (LateUpdate != null)
                            {
                                Application.Current.Dispatcher.Invoke(LateUpdate);
                            }
                            if (LateUpdateAsync != null)
                            {
                                await LateUpdateAsync.Invoke();
                            }
                        }

                        Thread.Sleep(Acceleration == 0 ? DeltaTime : i < Times.Count & Times.Count > 0 ? Times[i] : DeltaTime);
                    }
                }
            }

            WhileEnded();
        }
        public void Interrupt(bool IsUnsafeOver = false)
        {
            if (IsUnSafe && !IsUnsafeOver) return;
            IsStop = IsRunning;
        }
        internal async void WhileEnded()
        {
            if (IsUnSafe)
            {
                Machine.UnSafeInterpreters.Remove(this);
                return;
            }

            if (Machine.IsReSet)
            {
                return;
            }

            if (Application.Current != null)
            {
                if (Completed != null)
                {
                    Application.Current.Dispatcher.Invoke(Completed);
                }
                if (CompletedAsync != null)
                {
                    await CompletedAsync.Invoke();
                }
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
                if (Machine.Interpreters.TryDequeue(out var source))
                {
                    Machine.InterpreterScheduler(source.Item1, source.Item2, source.Item3);
                }
                Machine.CurrentState = null;
            }
        }
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
