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

namespace MinimalisticWPF
{
    /// <summary>
    /// 状态机
    /// </summary>
    /// <typeparam name="T">被状态机控制的对象的实际类型</typeparam>
    public class StateMachine
    {
        private int _framecount = 0;
        private int _defaultFrameRate = 244;
        private bool _isInterrupted = false;
        private Tuple<string, double, bool>? _tempTransfer = null;


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
            PointProperties = Properties.Where(x => x.PropertyType == typeof(Brush))
                .ToArray();//筛选Point属性

            foreach (var state in states)
            {
                var temp = States.FirstOrDefault(x => x.StateName == state.StateName);
                if (temp == null) States.Add(state);
                else throw new ArgumentException($"A state named [ {state.StateName} ] already exists in the collection.Modify the collection to ensure that the state name is unique");
            }//存储不重名的State

            var DCProperty = Properties.FirstOrDefault(x => x.Name == "DataContext");
            IConditions = DCProperty == null ? viewModel as IConditionalTransfer : DCProperty.GetValue(Target) as IConditionalTransfer;
            if (IConditions != null) { IConditions.Machine = this; }
            //尝试与Conditional模块建立连接
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
        /// Point属性
        /// </summary>
        public PropertyInfo[] PointProperties { get; internal set; }


        /// <summary>
        /// 受状态机控制的对象
        /// </summary>
        public object Target { get; internal set; }
        /// <summary>
        /// 与状态机相连的Conditional模块
        /// </summary>
        public IConditionalTransfer? IConditions { get; internal set; }


        /// <summary>
        /// 此状态机可导向的所有非条件驱动状态
        /// </summary>
        public StateCollection States { get; set; } = new StateCollection();


        /// <summary>
        /// 是否处于过渡过程中
        /// </summary>
        public bool IsTransferRunning { get; internal set; } = false;
        /// <summary>
        /// 排队处理连续申请的Transfer操作
        /// </summary>
        internal Queue<Tuple<string, double>> Transfers { get; set; } = new Queue<Tuple<string, double>>();
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
                if (_framecount == 0 && Transfers.Count > 0 && !_isInterrupted)
                {
                    var temp = Transfers.Dequeue();
                    Transfer(temp.Item1, temp.Item2);
                }
            }
        }


        /// <summary>
        /// 过渡帧率
        /// </summary>
        internal int FrameRate { get; set; } = 244;
        /// <summary>
        /// 一帧持续的时长
        /// </summary>
        public double FrameDuration { get => 1000.0 / FrameRate; }
        /// <summary>
        /// 全局受保护的属性
        /// </summary>
        public List<string> GlobalProtectedProperty { get; internal set; } = new List<string>();


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
        /// 设置非条件驱动状态集合
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
        /// <summary>
        /// 设置条件驱动状态集合
        /// </summary>
        public StateMachine SetConditions(params StateVector[] conditionGroups)
        {
            if (IConditions != null)
            {
                if (conditionGroups.Length == 0) IConditions.Conditions.Clear();

                foreach (var condition in conditionGroups)
                {
                    if (IConditions.Conditions.FirstOrDefault(x => x.Name == condition.Name) == null)
                    {
                        IConditions.Conditions.Add(condition);
                    }
                }
            }
            return this;
        }
        /// <summary>
        /// 保护属性不受状态机影响
        /// </summary>
        /// <param name="propertyNames">若干属性名</param>
        public void SetProtects(params string[] propertyNames)
        {
            if (propertyNames.Length == 0) GlobalProtectedProperty.Clear();

            foreach (var propertyName in propertyNames)
            {
                var result = GlobalProtectedProperty.FirstOrDefault(x => x == propertyName);
                if (result == null) { GlobalProtectedProperty.Add(propertyName); }
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
        public void Transfer(string stateName, double transitionTime, bool isQueue = false, bool isLast = true, bool isUnique = false, int? frameRate = default, double waitTime = 0.008, ICollection<string>? protectNames = default)
        {
            if (isLast) { Transfers.Clear(); }

            if (!isQueue && IsTransferRunning)
            {
                _isInterrupted = true;
                _tempTransfer = Tuple.Create(stateName, transitionTime, true);
                FrameCount = 0;
                LoadAnimation(stateName, transitionTime, waitTime, frameRate, protectNames);
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
            LoadAnimation(stateName, transitionTime, waitTime, frameRate, protectNames);
        }
        private async void LoadAnimation(string stateName, double transitionTime, double waitTime, int? frameRate, ICollection<string>? protectNames)
        {
            IsTransferRunning = true;
            FrameRate = frameRate == null ? _defaultFrameRate : (int)frameRate;

            await Task.Delay((int)(waitTime * 1000));
            //意义:例如在MouseLeave事件中,鼠标若移动过快,可能无法正确执行过渡效果,因此需要设置一个前置延迟

            var viewModelState = States.FirstOrDefault(x => x.StateName == stateName);
            if (viewModelState == null)
            {
                throw new ArgumentException($"Failed to find state named [ {stateName} ] from controller");
            }

            List<Tuple<PropertyInfo, double>> viewModels = new List<Tuple<PropertyInfo, double>>();
            //预加载:[ 需要平滑过渡的属性 ]+[ 新值相对于旧值的变化量 ]
            for (int i = 0; i < DoubleProperties.Length; i++)
            {
                if (protectNames?.FirstOrDefault(x => x == DoubleProperties[i].Name) == null &&
                    GlobalProtectedProperty.FirstOrDefault(x => x == DoubleProperties[i].Name) == null)
                {
                    double now = (double)DoubleProperties[i].GetValue(Target);
                    double viewModel = viewModelState[DoubleProperties[i].Name];
                    if (now != viewModel)
                    {
                        viewModels.Add(Tuple.Create(DoubleProperties[i], viewModel - now));
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

                double currentValue = (double)viewModels[i].Item1.GetValue(Target);
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
                    result.Item1.SetValue(Target, result.Item2);
                }
                await Task.Delay(deltaTime);
                if (_isInterrupted) { break; }
            }

            _isInterrupted = false;
            IsTransferRunning = false;
            if (FrameCount != 0) { FrameCount = 0; }
        }
        internal List<List<Tuple<PropertyInfo, object?>>> ComputingFrames()
        {
            List<List<Tuple<PropertyInfo, object?>>> result = new List<List<Tuple<PropertyInfo, object?>>>();

            result.Add(DoubleComputing());
            result.Add(BrushComputing());
            result.Add(PointComputing());

            return result;
        }
        internal List<Tuple<PropertyInfo, object?>> DoubleComputing()
        {
            List<Tuple<PropertyInfo, object?>> result = new List<Tuple<PropertyInfo, object?>>();



            return result;
        }
        internal List<Tuple<PropertyInfo, object?>> BrushComputing()
        {
            List<Tuple<PropertyInfo, object?>> result = new List<Tuple<PropertyInfo, object?>>();



            return result;
        }
        internal List<Tuple<PropertyInfo, object?>> PointComputing()
        {
            List<Tuple<PropertyInfo, object?>> result = new List<Tuple<PropertyInfo, object?>>();



            return result;
        }


        /// <summary>
        /// 动画解释器
        /// </summary>
        internal class AnimationInterpreter
        {
            internal AnimationInterpreter() { }

            internal object? Target { get; set; }
            internal int DeltaTime { get; set; }
            internal List<List<Tuple<PropertyInfo, object?>>> Frams { get; set; } = new List<List<Tuple<PropertyInfo, object?>>>();
            internal bool IsRunning { get; set; } = false;
            internal bool IsStop { get; set; } = false;

            internal async void Interpreter()
            {
                if (IsStop || IsRunning) { return; }
                IsRunning = true;

                for (int i = 0; i < Frams[0].Count; i++)
                {
                    if (IsStop) { return; }
                    for (int j = 0; j < Frams.Count; j++)
                    {

                        var result = Frams[j][i];
                        result.Item1.SetValue(Target, result.Item2);
                    }
                    await Task.Delay(DeltaTime);
                }
            }

            internal void Interrupt()
            {
                IsStop = true;
            }
        }
    }
}
