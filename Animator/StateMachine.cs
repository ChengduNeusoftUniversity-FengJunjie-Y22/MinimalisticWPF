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

namespace MinimalisticWPF
{
    /// <summary>
    /// 状态机,基于对ViewModel属性的平滑变动,加载过渡效果
    /// </summary>
    /// <typeparam name="T">ViewModel的具体类型</typeparam>
    public class StateMachine<T> where T : StateViewModelBase
    {
        public StateMachine(T viewModel, params State[] states)
        {
            ViewModel = viewModel;
            Type type = ViewModel.GetType();
            Properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var state in states)
            {
                var temp = States.FirstOrDefault(x => x.StateName == state.StateName);
                if (temp == null) { States.Add(state); }
            }
        }

        /// <summary>
        /// 控件ViewModel层的所有 [ public double ] 属性
        /// </summary>
        public PropertyInfo[] Properties { get; internal set; }

        /// <summary>
        /// 控件的ViewModel层
        /// </summary>
        public T ViewModel { get; internal set; }

        /// <summary>
        /// 初始化时添加的状态信息集合
        /// </summary>
        public StateCollection States { get; set; } = new StateCollection();

        /// <summary>
        /// 排队处理连续申请的Transfer操作
        /// </summary>
        public Queue<Tuple<string, double>> Transfers { get; internal set; } = new Queue<Tuple<string, double>>();

        /// <summary>
        /// 是否处于过渡过程中
        /// </summary>
        public bool IsTransferRunning { get; internal set; } = false;

        private int _framecount = 0;
        /// <summary>
        /// 当前正在执行的任务数量
        /// </summary>
        public int FrameCount
        {
            get => _framecount;
            internal set
            {
                _framecount = value >= 0 ? value : 0;
                IsTransferRunning = value > 0 ? true : false;
                ProtectedState = value > 0 ? ProtectedState : string.Empty;
                if (_framecount == 0 && Transfers.Count > 0)
                {
                    var temp = Transfers.Dequeue();
                    Transfer(temp.Item1, temp.Item2);
                }
            }
        }

        /// <summary>
        /// 过渡帧率
        /// </summary>
        public int FrameRate { get; set; } = 400;

        /// <summary>
        /// 一帧持续的时长
        /// </summary>
        public double FrameDuration { get => 1000.0 / FrameRate; }

        public string ProtectedState { get; set; } = string.Empty;

        /// <summary>
        /// 转移至下一个状态
        /// </summary>
        /// <param name="stateName">状态的名称</param>
        /// <param name="transitionTime">过渡持续时间</param>
        /// <exception cref="ArgumentException"></exception>
        public async void Transfer(string stateName, double transitionTime)
        {
            if (IsTransferRunning)
            {
                if (stateName != ProtectedState) { Transfers.Enqueue(Tuple.Create(stateName, transitionTime)); }
                return;
            }
            //连续调用Transfer时,需要排队执行

            var targetState = States.FirstOrDefault(x => x.StateName == stateName);
            //寻找到指定名称的State

            if (targetState == null)
            {
                throw new ArgumentException($"Failed to find state named [ {stateName} ] from controller");
                //异常:不存在指定的State
            }

            ProtectedState = targetState.StateName;

            List<Tuple<PropertyInfo, double>> targets = new List<Tuple<PropertyInfo, double>>();
            //预加载:[ 需要平滑过渡的属性 ]+[ 新值相对于旧值的变化量 ]
            for (int i = 0; i < Properties.Length; i++)
            {
                if (Properties[i].PropertyType == typeof(double))
                {
                    double now = (double)Properties[i].GetValue(ViewModel);
                    double target = targetState[Properties[i].Name];
                    if (now != target)
                    {
                        targets.Add(Tuple.Create(Properties[i], target - now));
                    }
                }
            }

            int frameCount = (int)(transitionTime / (FrameDuration / 1000));
            //单个属性的渐变流程所需要的总帧数

            List<List<Tuple<PropertyInfo, double>>> allFrames = new List<List<Tuple<PropertyInfo, double>>>();
            //所有帧(未排序)

            for (int i = 0; i < targets.Count; i++)
            {
                allFrames.Add(new List<Tuple<PropertyInfo, double>>());

                double delta = targets[i].Item2 / frameCount;
                //每帧变化的量

                double currentValue = (double)targets[i].Item1.GetValue(ViewModel);
                //当前值

                for (int j = 0; j < frameCount; j++)
                {
                    double newValue = currentValue + j * delta;
                    allFrames[i].Add(Tuple.Create(targets[i].Item1, newValue));
                    FrameCount++;
                }
            }

            if (allFrames.Count == 0)
            {
                return;
            }

            var deltaTime = (int)FrameDuration;

            for (int i = 0; i < allFrames[0].Count; i++)
            {
                for (int j = 0; j < allFrames.Count; j++)
                {
                    var result = allFrames[j][i];
                    ApplyPropertyChanging(result.Item1, result.Item2);
                }
                await Task.Delay(deltaTime);
            }
        }

        private void ApplyPropertyChanging(PropertyInfo target, double newValue)
        {
            target.SetValue(ViewModel, newValue);
            //设置属性的新值

            FrameCount--;
        }
    }
}
