using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class ExtensionForAnyClass
    {
        /// <summary>
        /// 开始创建基于StateMachine的过渡效果
        /// </summary>
        /// <param name="element"></param>
        public static TransitionBoard<T> Transition<T>(this T element) where T : class
        {
            TransitionBoard<T> tempStoryBoard = new TransitionBoard<T>(element);
            return tempStoryBoard;
        }

        /// <summary>
        /// 尝试为对象的DataContext加载状态机
        /// </summary>
        /// <typeparam name="T">DataContext的真实类型</typeparam>
        public static StateMachine StateMachineLoading<T>(this FrameworkElement source, T viewModel) where T : class
        {
            var vectorInterface = viewModel as IConditionalTransition<T> ?? throw new ArgumentException($"The [ {nameof(T)} ] Is Not A [ {nameof(IConditionalTransition<T>)} ]");
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
        /// 条件语句 ( 必填 )
        /// </summary>
        /// <param name="target"></param>
        /// <param name="condition">条件语句</param>
        /// <returns></returns>
        public static bool IsSatisfy<T>(this T target, Expression<Func<T, bool>> condition) where T : class
        {
            var checker = condition.Compile();
            if (checker == null) return false;
            return checker(target);
        }
        /// <summary>
        /// 条件语句 ( 必填 )
        /// <para>若符合条件则执行TransitionBoard过渡 ( 可空 ) </para>
        /// <para>TransitionBoard过渡效果参数 ( 可空 ) </para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="condition">条件语句</param>
        /// <param name="transfer">TransitionBoard过渡</param>
        /// <param name="set">细节参数</param>
        public static bool IsSatisfy<T>(this T target, Expression<Func<T, bool>> condition, TransitionBoard<T>? transfer, Action<TransitionParams>? set) where T : class
        {
            var checker = condition.Compile();
            if (checker == null) return false;
            var result = checker(target);
            if (result) target.BeginTransition(transfer, set);
            return result;
        }
        /// <summary>
        /// 条件语句 ( 必填 )
        /// <para>若符合条件则执行State过渡 ( 可空 ) </para>
        /// <para>State过渡效果参数 ( 可空 ) </para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="condition">条件语句</param>
        /// <param name="state">State过渡</param>
        /// <param name="set">细节参数</param>
        public static bool IsSatisfy<T>(this T target, Expression<Func<T, bool>> condition, State? state, Action<TransitionParams>? set = default) where T : class
        {
            var checker = condition.Compile();
            if (checker == null) return false;
            var result = checker(target);
            if (result) target.BeginTransition(state, set);
            return result;
        }

        /// <summary>
        /// 启动TransitionBoard过渡
        /// <para>注意 : 由此方法启动的动画必定截断正在执行的过渡</para>
        /// </summary>
        public static void BeginTransition<T>(this T source, TransitionBoard<T>? transfer, Action<TransitionParams>? set = default) where T : class
        {
            if (transfer != null)
            {
                if (set != null)
                {
                    transfer.TransitionParams = set;
                }

                if (transfer.IsStatic)
                {
                    transfer.Start(source);
                }
                else
                {
                    transfer.Start(source);
                }
            }
        }
        /// <summary>
        /// 启动State过渡
        /// <para>注意 : 由此方法启动的动画必定截断正在执行的过渡</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="state">待过渡到的State</param>
        /// <param name="set">过渡细节</param>
        public static void BeginTransition<T>(this T source, State? state, Action<TransitionParams>? set) where T : class
        {
            if (state == null) return;
            if (state.ActualType != typeof(T)) throw new ArgumentException("State does not match the type of the object");
            var machine = source.FindStateMachine();
            if (machine == null)
            {
                var newMachine = StateMachine.Create(source).SetStates();
                TransitionBoard<T>.MachinePool.Add(source, newMachine);
                newMachine.States.Add(state);
                newMachine.Transition(state.StateName, set);
            }
            else
            {
                machine.ReSet();
                machine.States.Add(state);
                machine.Transition(state.StateName, set);
            }
        }

        /// <summary>
        /// 尝试找出系统中管理该对象过渡效果的状态机实例
        /// <para>第一优先级 : TransitionBoard 对象池（字典）内存储的状态机</para>
        /// <para>第二优先级 : 实例对象自身包含的属性</para>
        /// <para>第三优先级 : 若为一个FrameworkElement，尝试从其DataContext中获取状态机</para>
        /// </summary>
        public static StateMachine? FindStateMachine<T>(this T source) where T : class
        {
            TransitionBoard<T>.MachinePool.TryGetValue(source, out var machineA);
            if (machineA != null) return machineA;


            var Incodebehind = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.CanRead && x.CanWrite && x.PropertyType == typeof(StateMachine));
            if (Incodebehind != null)
            {
                return Incodebehind.GetValue(source) as StateMachine;
            }

            if (source is FrameworkElement element)
            {
                if (element.DataContext == null) return null;
                var Inviewmodel = element.DataContext.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(x => x.CanRead && x.CanWrite && x.PropertyType == typeof(StateMachine));
                if (Inviewmodel != null)
                {
                    return Inviewmodel.GetValue(element.DataContext) as StateMachine;
                }
            }

            return null;
        }
    }

    public static class Transition
    {
        private static string _tempName = "temp";
        /// <summary>
        /// TransitionBoard中,State的临时名称,默认TransitionBoard使用名为"temp"的临时State
        /// </summary>
        public static string TempName
        {
            get => _tempName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _tempName = value;
                }
            }
        }
        public static TransitionBoard<T> CreateBoardFromObject<T>(T target) where T : class
        {
            return new TransitionBoard<T>(target);
        }
        public static TransitionBoard<T> CreateBoardFromType<T>() where T : class
        {
            return new TransitionBoard<T>() { IsStatic = true };
        }
    }

    public class TransitionBoard<T> where T : class
    {
        internal static Dictionary<object, StateMachine> MachinePool { get; set; } = new Dictionary<object, StateMachine>();

        internal TransitionBoard() { }
        internal TransitionBoard(T target)
        {
            Target = target;

            MachinePool.TryGetValue(target, out var temp);
            if (temp != null)
            {
                temp.States.Clear();
                temp.Interpreters.Clear();
                temp.Interpreter?.Interrupt();
                Machine = temp;
                return;
            }

            Machine = StateMachine.Create(target).SetStates();
            MachinePool.Add(Target, Machine);
        }
        internal bool IsStatic { get; set; } = false;
        internal T? Target { get; set; }
        internal StateMachine? Machine { get; set; }
        public State TempState { get; internal set; } = new State() { StateName = Transition.TempName };
        public Action<TransitionParams>? TransitionParams { get; internal set; }

        /// <summary>
        /// 设置指定属性过渡后的最终值
        /// </summary>
        public TransitionBoard<T> SetProperty(Expression<Func<T, double>> propertyLambda, double newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return this;
                }
                TempState.Values.Add(property.Name, (object?)newValue);
            }
            return this;
        }
        /// <summary>
        /// 设置指定属性过渡后的最终值
        /// </summary>
        public TransitionBoard<T> SetProperty(Expression<Func<T, Brush>> propertyLambda, Brush newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Brush))
                {
                    return this;
                }
                TempState.Values.Add(property.Name, (object?)newValue);
            }
            return this;
        }
        /// <summary>
        /// 设置指定属性过渡后的最终值
        /// </summary>
        public TransitionBoard<T> SetProperty(Expression<Func<T, Transform>> propertyLambda, params Transform[] newValue)
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
                TempState.Values.Add(property.Name, (object?)result);
            }
            return this;
        }
        /// <summary>
        /// 设置指定属性过渡后的最终值
        /// </summary>
        public TransitionBoard<T> SetProperty(Expression<Func<T, Point>> propertyLambda, Point newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Point))
                {
                    return this;
                }
                TempState.Values.Add(property.Name, (object?)newValue);
            }
            return this;
        }
        /// <summary>
        /// 设置指定属性过渡后的最终值
        /// </summary>
        public TransitionBoard<T> SetProperty(Expression<Func<T, CornerRadius>> propertyLambda, CornerRadius newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(CornerRadius))
                {
                    return this;
                }
                TempState.Values.Add(property.Name, (object?)newValue);
            }
            return this;
        }
        /// <summary>
        /// 设置指定属性过渡后的最终值
        /// </summary>
        public TransitionBoard<T> SetProperty(Expression<Func<T, Thickness>> propertyLambda, Thickness newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Thickness))
                {
                    return this;
                }
                TempState.Values.Add(property.Name, (object?)newValue);
            }
            return this;
        }
        /// <summary>
        /// 设置指定属性过渡后的最终值
        /// </summary>
        public TransitionBoard<T> SetProperty(Expression<Func<T, ILinearInterpolation>> propertyLambda, ILinearInterpolation newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || !typeof(ILinearInterpolation).IsAssignableFrom(property.PropertyType))
                {
                    return this;
                }
                TempState.Values.Add(property.Name, (object?)newValue);
            }
            return this;
        }
        /// <summary>
        /// 设置本次过渡的参数
        /// </summary>
        public TransitionBoard<T> SetParams(Action<TransitionParams>? modifyParams = null)
        {
            TransitionParams = modifyParams;
            return this;
        }
        /// <summary>
        /// 反射特定对象[全部]受支持的属性，注意会覆盖SetProperty步骤（反之则不会）
        /// </summary>
        /// <param name="reflected">被反射的对象实例</param>
        public TransitionBoard<T> ReflectAny(T reflected)
        {
            TempState = new State(reflected, Array.Empty<string>(), Array.Empty<string>());
            TempState.StateName = Transition.TempName;
            return this;
        }
        /// <summary>
        /// 反射特定对象[部分]受支持的属性，注意会覆盖SetProperty步骤（反之则不会）
        /// </summary>
        /// <param name="reflected">被反射的对象实例</param>
        /// <param name="blackList">黑名单,不参与反射记录的属性</param>
        public TransitionBoard<T> ReflectExcept(T reflected, params Expression<Func<T, string>>[] blackList)
        {
            var propertyNames = blackList.Select(p => ((MemberExpression)p.Body).Member.Name).ToArray();
            TempState = new State(reflected, Array.Empty<string>(), propertyNames);
            TempState.StateName = Transition.TempName;
            return this;
        }
        /// <summary>
        /// 启动过渡,内置目标对象
        /// </summary>
        public void Start()
        {
            if (IsStatic) throw new InvalidOperationException("This method cannot be used under Type-based creation, instead use an overloaded version of the Start () method");
            if (Machine == null) throw new ArgumentException("StateMachine instance lost");
            if (Target == null) throw new ArgumentException("Target object instance lost");
            Machine.ReSet();
            Machine.States.Add(TempState);
            Machine.Transition(Transition.TempName, TransitionParams);
        }
        /// <summary>
        /// 启动过渡,不是内置目标对象
        /// </summary>
        /// <param name="target">新的目标对象</param>
        public void Start(T target)
        {
            MachinePool.TryGetValue(target, out var temp);
            if (temp != null)
            {
                Machine = temp;
            }
            else
            {
                Machine = StateMachine.Create(target);
                MachinePool.Add(target, Machine);
            }
            Machine.ReSet();
            Machine.States.Add(TempState);
            Machine.Transition(Transition.TempName, TransitionParams);
        }
    }
}
