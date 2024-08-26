using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
        public static TempStoryBoard<T> StateMachineTransfer<T>(this T element) where T : FrameworkElement, new()
        {
            TempStoryBoard<T> tempStoryBoard = new TempStoryBoard<T>(element);
            return tempStoryBoard;
        }

        /// <summary>
        /// 尝试为对象的DataContext加载状态机
        /// </summary>
        /// <typeparam name="T">DataContext的真实类型</typeparam>
        public static StateMachine StateMachineLoading<T>(this FrameworkElement source, T viewModel) where T : class
        {
            var vectorInterface = viewModel as IConditionalTransfer<T>;
            //检查datacontext是否是T类型的IConditionalTransfer<T>对象
            if (viewModel == null) throw new Exception($"The [ DataContext ] Is Not A [ {nameof(T)} ]");
            if (vectorInterface == null) throw new Exception($"The [ {nameof(T)} ] Is Not A [ {nameof(IConditionalTransfer<T>)} ]");

            var StateFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.FieldType == typeof(State)).ToArray();
            //反射ViewModel中所有定义的静态State对象
            var StateVectorField = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(x => x.FieldType == typeof(StateVector<T>));
            //反射ViewModel中首个定义的静态StateVector<T>对象

            var States = StateFields.Select(x => (State?)x.GetValue(viewModel)).ToArray();
            //尝试从ViewModel获取具体的State
            var StateVector = (StateVector<T>?)StateVectorField?.GetValue(viewModel);
            //尝试从ViewModel获取具体的StateVector<T>

            //尝试激活状态机系统与条件切换系统
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
                StateVector.Target = viewModel;
                vectorInterface.StateVector = StateVector;
                vectorInterface.StateVector.Machine = machine;
            }

            return machine;
        }
    }

    public class TempStoryBoard<T> where T : FrameworkElement, new()
    {
        public static List<Tuple<StateMachine, object>> MachinePool = new List<Tuple<StateMachine, object>>();

        internal TempStoryBoard(T target)
        {
            Target = target;

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

        internal StateMachine Machine { get; set; }

        List<Tuple<PropertyInfo, object?>> States = new List<Tuple<PropertyInfo, object?>>();

        TransferParams TransferParams { get; set; } = new TransferParams();

        public TempStoryBoard<T> Add(Expression<Func<T, double>> propertyLambda, double newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return this;
                }
                States.Add(Tuple.Create(property, (object?)newValue));
            }
            return this;
        }
        public TempStoryBoard<T> Add(Expression<Func<T, Brush>> propertyLambda, Brush newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Brush))
                {
                    return this;
                }
                States.Add(Tuple.Create(property, (object?)newValue));
            }
            return this;
        }
        public TempStoryBoard<T> Add(Expression<Func<T, Point>> propertyLambda, Point newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Brush))
                {
                    return this;
                }
                States.Add(Tuple.Create(property, (object?)newValue));
            }
            return this;
        }

        public TempStoryBoard<T> Set(Action<TransferParams>? modifyParams = null)
        {
            modifyParams?.Invoke(TransferParams);
            return this;
        }

        public T Start()
        {
            T sta = new T();
            foreach (var item in States)
            {
                item.Item1.SetValue(sta, item.Item2);
            }
            State temp = State.FromObject(sta)
                .SetName("temp")
                .ToState();
            Machine.States.Add(temp);
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
                            x.ProtectNames = TransferParams.ProtectNames;
                        });
            }
            return Target;
        }
    }
}
