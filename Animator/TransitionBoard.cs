using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF
{
    public class TransitionBoard<T> where T : class
    {
        /// <summary>
        /// 对于任意使用Board启动动画的对象实例,全局只允许存在一台StateMachine用于为其加载过渡效果
        /// </summary>
        public static Dictionary<object, StateMachine> MachinePool { get; internal set; } = new Dictionary<object, StateMachine>();

        internal TransitionBoard() { }
        internal TransitionBoard(T target)
        {
            Target = target;

            MachinePool.TryGetValue(target, out var temp);
            if (temp != null)
            {
                Machine = temp;
                return;
            }

            Machine = StateMachine.Create(target).SetStates();
            MachinePool.Add(Target, Machine);
        }
        /// <summary>
        /// 是否由Transition静态方法创建,若为true,代表该TransitionBoard无法直接使用Start(),需要改用AnyClass.BeginTransition()
        /// </summary>
        public bool IsStatic { get; internal set; } = false;
        internal T? Target { get; set; }
        internal StateMachine? Machine { get; set; }
        public State TempState { get; internal set; } = new State() { StateName = Transition.TempName };
        public Action<TransitionParams>? TransitionParams { get; set; }
        public List<List<Tuple<PropertyInfo, List<object?>>>>? Preload { get; set; }

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
                TempState.AddProperty(property.Name, (object?)newValue);
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
                TempState.AddProperty(property.Name, (object?)newValue);
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
                TempState.AddProperty(property.Name, (object?)result);
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
                TempState.AddProperty(property.Name, (object?)newValue);
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
                TempState.AddProperty(property.Name, (object?)newValue);
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
                TempState.AddProperty(property.Name, (object?)newValue);
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
                TempState.AddProperty(property.Name, (object?)newValue);
            }
            return this;
        }
        /// <summary>
        /// 设置本次过渡的参数
        /// </summary>
        public TransitionBoard<T> SetParams(Action<TransitionParams> modifyParams)
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
        /// 预载帧数据以降低状态机调度动画执行时的计算量,内置目标时( IsStatic == false )
        /// </summary>
        public TransitionBoard<T> PreLoad()
        {
            var par = new TransitionParams();
            TransitionParams?.Invoke(par);
            Preload = StateMachine.PreloadFrames(Target, TempState, par);
            return this;
        }
        /// <summary>
        /// 预载帧数据以降低状态机调度动画执行时的计算量,不是内置目标时( IsStatic == true )
        /// </summary>
        public TransitionBoard<T> PreLoad(T target)
        {
            var par = new TransitionParams();
            TransitionParams?.Invoke(par);
            Preload = StateMachine.PreloadFrames(target, TempState, par);
            return this;
        }
        /// <summary>
        /// 启动过渡,内置目标对象( IsStatic == false )
        /// </summary>
        public void Start()
        {
            if (IsStatic) throw new InvalidOperationException("This method cannot be used under Type-based creation, instead use an overloaded version of the Start () method");
            if (Machine == null) throw new ArgumentException("StateMachine instance lost");
            if (Target == null) throw new ArgumentException("Target object instance lost");
            Machine.ReSet();
            TempState.StateName = Transition.TempName + "NonStatic";
            Machine.States.Add(TempState);
            Machine.Transition(TempState.StateName, TransitionParams, Preload);
        }
        /// <summary>
        /// 启动过渡,不是内置目标对象( IsStatic == true )
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
            TempState.StateName = Transition.TempName + Machine.States.BoardSuffix;
            Machine.States.Add(TempState);
            Machine.Transition(TempState.StateName, TransitionParams, Preload);
        }
    }
}
