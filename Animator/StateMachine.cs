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

namespace MinimalisticWPF
{
    /// <summary>
    /// 状态机
    /// </summary>
    /// <typeparam name="T">被状态机控制的对象的实际类型</typeparam>
    public class StateMachine<T> where T : INotifyPropertyChanged
    {
        /// <param name="viewModel">被状态机控制的实例对象</param>
        /// <param name="states">所有该对象可能具备的状态</param>
        /// <exception cref="ArgumentException"></exception>
        public StateMachine(T viewModel, params State[] states)
        {
            ViewModel = viewModel;
            Type type = typeof(T);
            Properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var state in states)
            {
                var temp = States.FirstOrDefault(x => x.StateName == state.StateName);
                if (temp == null) { States.Add(state); }
            }
        }

        /// <summary>
        /// 反射 T 获取的全部Public属性
        /// </summary>
        public PropertyInfo[] Properties { get; internal set; }

        /// <summary>
        /// T 接管的实例对象
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
                if (_framecount == 0 && Transfers.Count > 0 && !IsInterrupted)
                {
                    var temp = Transfers.Dequeue();
                    Transfer(temp.Item1, temp.Item2);
                }
            }
        }

        /// <summary>
        /// 过渡帧率
        /// </summary>
        internal int FrameRate { get; set; } = 500;

        public int DefaultFrameRate { get; set; } = 500;

        /// <summary>
        /// 一帧持续的时长
        /// </summary>
        public double FrameDuration { get => 1000.0 / FrameRate; }

        /// <summary>
        /// 全局受保护的属性
        /// </summary>
        public List<string> GlobalProtectedProperty { get; internal set; } = new List<string>();

        private bool IsInterrupted { get; set; } = false;

        private Tuple<string, double, bool>? TempTransfer { get; set; }

        /// <summary>
        /// 保护属性不受状态机影响
        /// </summary>
        /// <param name="propertyNames">若干属性名</param>
        public void Protect(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var result = GlobalProtectedProperty.FirstOrDefault(x => x == propertyName);
                if (result == null) { GlobalProtectedProperty.Add(propertyName); }
            }
        }

        /// <summary>
        /// 解除属性的保护态
        /// </summary>
        /// <param name="propertyNames">若干属性名</param>
        public void Unprotect(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                GlobalProtectedProperty.Remove(propertyName);
            }
        }

        /// <summary>
        /// 转移至目标状态
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <param name="transitionTime">过渡时间</param>
        /// <param name="isQueue">是否排队</param>
        /// <param name="isLast">是否视为最后一个转移操作</param>
        /// <param name="frameRate">过渡帧率</param>
        /// <param name="waitTime">延时启动</param>
        /// <param name="isUnique">是否在队列中保持唯一</param>
        /// <param name="protectNames">局部保护属性不受状态机影响</param>
        /// <exception cref="ArgumentException"></exception>
        public void Transfer(string stateName, double transitionTime, bool isQueue = true, bool isLast = false, bool isUnique = false, int? frameRate = null, double waitTime = 0.008, ICollection<string>? protectNames = default)
        {
            if (isLast) { Transfers.Clear(); }

            if (!isQueue && IsTransferRunning)
            {
                IsInterrupted = true;
                TempTransfer = Tuple.Create(stateName, transitionTime, true);
                FrameCount = 0;
                DoubleAnimation(stateName, transitionTime, waitTime, frameRate, protectNames);
                return;
            }

            if (IsTransferRunning)
            {
                if (!isUnique)
                {
                    Transfers.Enqueue(Tuple.Create(stateName, transitionTime));
                }
                else
                {
                    var viewModel = Transfers.FirstOrDefault(x => x.Item1 == stateName);
                    if (viewModel == null) { Transfers.Enqueue(Tuple.Create(stateName, transitionTime)); }
                }
                return;
            }
            DoubleAnimation(stateName, transitionTime, waitTime, frameRate, protectNames);
        }

        private async void DoubleAnimation(string stateName, double transitionTime, double waitTime, int? frameRate, ICollection<string>? protectNames)
        {
            IsTransferRunning = true;
            FrameRate = frameRate == null ? DefaultFrameRate : (int)frameRate;

            await Task.Delay((int)(waitTime * 1000));
            //意义:例如在MouseLeave事件中,鼠标若移动过快,可能无法正确执行过渡效果,因此需要设置一个前置延迟

            var viewModelState = States.FirstOrDefault(x => x.StateName == stateName);
            if (viewModelState == null)
            {
                throw new ArgumentException($"Failed to find state named [ {stateName} ] from controller");
                //异常:无法根据名称找到指定的State
            }

            List<Tuple<PropertyInfo, double>> viewModels = new List<Tuple<PropertyInfo, double>>();
            //预加载:[ 需要平滑过渡的属性 ]+[ 新值相对于旧值的变化量 ]
            for (int i = 0; i < Properties.Length; i++)
            {
                if (Properties[i].PropertyType == typeof(double)
                    && protectNames?.FirstOrDefault(x => x == Properties[i].Name) == null
                    && GlobalProtectedProperty.FirstOrDefault(x => x == Properties[i].Name) == null)
                {
                    double now = (double)Properties[i].GetValue(ViewModel);
                    double viewModel = viewModelState[Properties[i].Name];
                    if (now != viewModel)
                    {
                        viewModels.Add(Tuple.Create(Properties[i], viewModel - now));
                    }
                }
            }

            var deltaTime = (int)FrameDuration;
            //每一帧持续的时间

            int frameCount = (int)(transitionTime / (FrameDuration / 1000));
            //单个属性的渐变流程所需要的总帧数

            List<List<Tuple<PropertyInfo, double>>> allFrames = new List<List<Tuple<PropertyInfo, double>>>();
            //预加载:[ 所有待执行的属性变化帧 ]
            for (int i = 0; i < viewModels.Count; i++)
            {
                allFrames.Add(new List<Tuple<PropertyInfo, double>>());

                double delta = viewModels[i].Item2 / frameCount;
                //每帧变化的量

                double currentValue = (double)viewModels[i].Item1.GetValue(ViewModel);
                //当前值

                for (int j = 0; j < frameCount; j++)
                {
                    double newValue = currentValue + j * delta;
                    allFrames[i].Add(Tuple.Create(viewModels[i].Item1, newValue));
                    FrameCount++;
                }
            }
            if (allFrames.Count == 0) { return; }

            //应用:[ 计算出的每一帧 ]
            for (int i = 0; i < allFrames[0].Count; i++)
            {
                for (int j = 0; j < allFrames.Count; j++)
                {
                    var result = allFrames[j][i];
                    result.Item1.SetValue(ViewModel, result.Item2);
                }
                await Task.Delay(deltaTime);
                if (IsInterrupted) { break; }
            }

            IsInterrupted = false;
            IsTransferRunning = false;
            if (FrameCount != 0) { FrameCount = 0; }
        }
    }
}
