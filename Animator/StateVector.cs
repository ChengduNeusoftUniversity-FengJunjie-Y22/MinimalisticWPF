using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public class StateVector
    {
        internal StateVector() { }

        public State State { get; set; } = new State("defualt");
        public string Name { get; internal set; } = string.Empty;
        public string StateName { get; internal set; } = string.Empty;
        public Func<dynamic, bool>? Condition { get; internal set; } = null;
        public TransferParams TransferParams { get; internal set; } = new TransferParams();

        /// <summary>
        /// 开始记录指向指定状态的触发条件
        /// </summary>
        public static TempStateVector<T> FromType<T>() where T : class
        {
            TempStateVector<T> result = new TempStateVector<T>();
            return result;
        }
    }

    public class TransferParams
    {
        internal TransferParams() { }

        internal TransferParams(double transitionTime = 0, bool isQueue = false, bool isLast = true, bool isUnique = false, int? frameRate = default, double waitTime = 0.008, ICollection<string>? protectNames = default)
        {
            Duration = transitionTime;
            FrameRate = frameRate == null ? FrameRate : (int)frameRate;
            IsQueue = isQueue;
            IsLast = isLast;
            IsUnique = isUnique;
            WaitTime = waitTime;
            ProtectNames = protectNames == null ? ProtectNames : ProtectNames;
        }

        /// <summary>
        /// 持续时长(单位:S)
        /// </summary>
        public double Duration { get; set; } = 0;

        /// <summary>
        /// 过渡帧率(默认:500)
        /// </summary>
        public int FrameRate { get; set; } = 244;

        /// <summary>
        /// 是否排队执行(默认:不排队)
        /// </summary>
        public bool IsQueue { get; set; } = false;

        /// <summary>
        /// 是否在执行完后,清除其它排队中的过渡效果(默认:清除)
        /// </summary>
        public bool IsLast { get; set; } = true;

        /// <summary>
        /// 申请进入执行队列时,若已有同名切换操作处于列队中,是否继续加入队列(默认:不加入)
        /// </summary>
        public bool IsUnique { get; set; } = true;

        /// <summary>
        /// [ 测试参数 ] 若无法响应例如MouseLeave事件,可适当增加此参数(默认:0.008)
        /// </summary>
        public double WaitTime { get; set; } = 0.008;

        /// <summary>
        /// 本次过渡过程中,不受状态机影响的属性的名称
        /// </summary>
        public ICollection<string>? ProtectNames { get; set; } = default;
    }

    public class TempStateVector<T> where T : class
    {
        internal TempStateVector() { }

        internal T? Target { get; set; } = null;

        internal StateVector Value { get; set; } = new StateVector();

        /// <summary>
        /// 记录该条件的名称
        /// </summary>
        public TempStateVector<T> SetName(string stateVectorName)
        {
            Value.Name = stateVectorName;
            return this;
        }

        /// <summary>
        /// 设置该Vector指向的State
        /// </summary>
        public TempStateVector<T> SetTarget(State state)
        {
            Value.State = state;
            return this;
        }

        /// <summary>
        /// 记录具体的条件
        /// </summary>
        public TempStateVector<T> SetCondition(Expression<Func<T, bool>> condition)
        {
            var compiledCondition = condition.Compile();

            Func<dynamic, bool> dynamicCondition = item =>
            {
                if (item is T typedItem)
                {
                    return compiledCondition(typedItem);
                }
                return false;
            };

            Value.Condition = dynamicCondition;

            return this;
        }

        /// <summary>
        /// 记录过渡效果相关的参数
        /// </summary>
        public TempStateVector<T> SetTransferParams(double transitionTime = 0, bool isQueue = false, bool isLast = true, bool isUnique = false, int? frameRate = default, double waitTime = 0.008, ICollection<string>? protectNames = default)
        {
            TransferParams tempdata = new TransferParams(transitionTime, isQueue, isLast, isUnique, frameRate, waitTime, protectNames);
            Value.TransferParams = tempdata;
            return this;
        }

        /// <summary>
        /// 输出StateVector
        /// </summary>
        public StateVector ToStateVector()
        {
            return Value;
        }
    }
}
