using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

public enum StateRecordModes
{
    Object,
    Type
}

namespace MinimalisticWPF
{
    public static class ExtensionToFrameworkElement
    {
        /// <summary>
        /// 按比率应用为父级元素的尺寸
        /// </summary>
        public static T ToParentSize<T>(this T element, double rate = 1) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            element.Width = Parent.Width * rate;
            element.Height = Parent.Height * rate;
            return element;
        }

        /// <summary>
        /// 按比率应用为父级元素的高度
        /// </summary>
        public static T ToParentHeight<T>(this T element, double rate = 1) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            element.Height = Parent.Height * rate;
            return element;
        }

        /// <summary>
        /// 按比率应用为父级元素的宽度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static T ToParentWidth<T>(this T element, double rate = 1) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            element.Width = Parent.Width * rate;
            return element;
        }

        /// <summary>
        /// 开始创建基于StateMachine的过渡效果
        /// </summary>
        /// <param name="mode">记录状态值的过程是否具备额外的反射开销</param>
        public static TempTransfer<T> StateMachineTransfer<T>(this T element, StateRecordModes mode = StateRecordModes.Type) where T : class, new()
        {
            TempTransfer<T> tempStoryBoard = new TempTransfer<T>(element, mode);
            return tempStoryBoard;
        }

        /// <summary>
        /// 尝试为对象的DataContext加载状态机
        /// </summary>
        /// <typeparam name="T">DataContext的真实类型</typeparam>
        public static StateMachine StateMachineLoading<T>(this FrameworkElement source, T viewModel) where T : class
        {
            var vectorInterface = viewModel as IConditionalTransfer<T> ?? throw new ArgumentException($"The [ {nameof(T)} ] Is Not A [ {nameof(IConditionalTransfer<T>)} ]");
            if (viewModel == null) throw new ArgumentException($"The [ DataContext ] Is Not A [ {nameof(T)} ]");

            var StateFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.FieldType == typeof(State)).ToArray();
            var StateProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType == typeof(State)).ToArray();
            var StateVectorField = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x => x.PropertyType == typeof(StateVector<T>));

            var FieldStates = StateFields.Select(x => (State?)x.GetValue(viewModel)).ToArray();
            var PropertyStates = StateProperties.Select(x => (State?)x.GetValue(viewModel)).ToArray();
            var States = PropertyStates.Concat(FieldStates);
            var StateVector = (StateVector<T>?)StateVectorField?.GetValue(viewModel);

            var machine = StateMachine.Create(viewModel);
            vectorInterface.StateMachine = machine;
            if (States != null)
            {
                foreach (var state in States)
                {
                    if (state != null)
                    {
                        vectorInterface.StateMachine.States.Add(state);
                    }
                }
            }
            if (StateVector != null)
            {
                vectorInterface.StateVector = StateVector;
            }

            return machine;
        }

        /// <summary>
        /// 对特定对象判断单个条件并自动执行过渡效果
        /// </summary>
        /// <param name="target">目标实例</param>
        /// <param name="condition">条件语句</param>
        public static bool IsSatisfy<T>(this T target, Expression<Func<T, bool>> condition)
        {
            var checker = condition.Compile();
            if (checker == null) return false;
            return checker(target);
        }
    }

    public class TempTransfer<T> where T : class, new()
    {
        public static List<Tuple<StateMachine, object>> MachinePool { get; internal set; } = new List<Tuple<StateMachine, object>>();

        internal TempTransfer(T target, StateRecordModes mode)
        {
            Target = target;
            Mode = mode;

            var temp = MachinePool.FirstOrDefault(x => x.Item2 == target);
            if (temp != null)
            {
                temp.Item1.States.Clear();
                temp.Item1.Interpreters.Clear();
                temp.Item1.Interpreter?.Interrupt();
                Machine = temp.Item1;
                return;
            }

            Machine = StateMachine.Create(target).SetStates();
            MachinePool.Add(Tuple.Create(Machine, (object)Target));
        }

        internal T Target { get; set; }

        StateRecordModes Mode { get; set; } = StateRecordModes.Type;

        List<string> WhiteList { get; set; } = new List<string>();

        internal StateMachine Machine { get; set; }

        List<Tuple<PropertyInfo, object?>> States = new List<Tuple<PropertyInfo, object?>>();

        TransferParams TransferParams { get; set; } = new TransferParams();

        /// <summary>
        /// 添加新状态值
        /// </summary>
        public TempTransfer<T> Add(Expression<Func<T, double>> propertyLambda, double newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return this;
                }
                States.Add(Tuple.Create(property, (object?)newValue));
                WhiteList.Add(property.Name);
            }
            return this;
        }
        /// <summary>
        /// 添加新状态值
        /// </summary>
        public TempTransfer<T> Add(Expression<Func<T, Brush>> propertyLambda, Brush newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Brush))
                {
                    return this;
                }
                States.Add(Tuple.Create(property, (object?)newValue));
                WhiteList.Add(property.Name);
            }
            return this;
        }
        /// <summary>
        /// 添加新状态值
        /// </summary>
        public TempTransfer<T> Add(Expression<Func<T, Transform>> propertyLambda, params Transform[] newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Transform) || newValue.Length == 0)
                {
                    return this;
                }
                var value = newValue.Select(t => t.Value).Aggregate(Matrix.Identity, (acc, matrix) => acc * matrix);
                var interpolatedMatrixStr = $"{value.M11},{value.M12},{value.M21},{value.M22},{value.OffsetX},{value.OffsetY}";
                var result = Transform.Parse(interpolatedMatrixStr);
                States.Add(Tuple.Create(property, (object?)result));
                WhiteList.Add(property.Name);
            }
            return this;
        }
        /// <summary>
        /// 添加新状态值
        /// </summary>
        public TempTransfer<T> Add(Expression<Func<T, Point>> propertyLambda, Point newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Point))
                {
                    return this;
                }
                States.Add(Tuple.Create(property, (object?)newValue));
                WhiteList.Add(property.Name);
            }
            return this;
        }
        /// <summary>
        /// 添加新状态值
        /// </summary>
        public TempTransfer<T> Add(Expression<Func<T, CornerRadius>> propertyLambda, CornerRadius newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(CornerRadius))
                {
                    return this;
                }
                States.Add(Tuple.Create(property, (object?)newValue));
                WhiteList.Add(property.Name);
            }
            return this;
        }
        /// <summary>
        /// 添加新状态值
        /// </summary>
        public TempTransfer<T> Add(Expression<Func<T, Thickness>> propertyLambda, Thickness newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Thickness))
                {
                    return this;
                }
                States.Add(Tuple.Create(property, (object?)newValue));
                WhiteList.Add(property.Name);
            }
            return this;
        }
        /// <summary>
        /// 添加新状态值
        /// </summary>
        public TempTransfer<T> Add(Expression<Func<T, ILinearInterpolation>> propertyLambda, ILinearInterpolation newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || !typeof(ILinearInterpolation).IsAssignableFrom(property.PropertyType))
                {
                    return this;
                }
                States.Add(Tuple.Create(property, (object?)newValue));
                WhiteList.Add(property.Name);
            }
            return this;
        }
        /// <summary>
        /// 设置本次过渡的参数
        /// </summary>
        public TempTransfer<T> Set(Action<TransferParams>? modifyParams = null)
        {
            modifyParams?.Invoke(TransferParams);
            return this;
        }
        /// <summary>
        /// 启动过渡
        /// </summary>
        /// <param name="IsWhiteList">反射过程是否启用白名单</param>
        public T Start(bool IsWhiteList = true)
        {
            if (Mode == StateRecordModes.Type)
            {
                State temp = new State();
                temp.StateName = "temp";
                temp.ActualType = typeof(T);
                foreach (var item in States)
                {
                    if (temp.Values.ContainsKey(item.Item1.Name))
                    {
                        temp.Values[item.Item1.Name] = item.Item2;
                    }
                    else
                    {
                        temp.Values.Add(item.Item1.Name, item.Item2);
                    }
                }

                Machine.States.Add(temp);

                SendMessageToMachine();

                return Target;
            }
            else
            {
                if (!IsWhiteList) WhiteList.Clear();

                var sta = new T();

                foreach (var item in States)
                {
                    item.Item1.SetValue(sta, item.Item2);
                }
                State temp = State.FromObject(sta)
                    .SetName("temp")
                    .ToState(WhiteList);

                Machine.States.Add(temp);

                SendMessageToMachine();

                return Target;
            }
        }
        void SendMessageToMachine()
        {
            if (TransferParams == null)
            {
                Machine.Transfer("temp", (x) => x.Duration = 0);
            }
            else
            {
                Machine.Transfer("temp",
                        (x) =>
                        {
                            x.Duration = TransferParams.Duration;
                            x.IsQueue = TransferParams.IsQueue;
                            x.IsLast = TransferParams.IsLast;
                            x.IsUnique = TransferParams.IsUnique;
                            x.FrameRate = TransferParams.FrameRate;
                            x.WaitTime = TransferParams.WaitTime;
                            x.Start = TransferParams.Start;
                            x.Update = TransferParams.Update;
                            x.Completed = TransferParams.Completed;
                            x.LateUpdate = TransferParams.LateUpdate;
                            x.Acceleration = TransferParams.Acceleration;
                        });
            }
        }
    }
}
