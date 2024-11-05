using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public static class ExtensionForDynamicTheme
    {
        /// <summary>
        /// 类型 => [ 主题 => 状态机动画数据 ]
        /// </summary>
        public static Dictionary<Type, Dictionary<Type, State>> ThemeValues { get; internal set; } = new Dictionary<Type, Dictionary<Type, State>>();

        /// <summary>
        /// 可全局切换主题的对象
        /// </summary>
        public static List<object> InstanceHosts { get; internal set; } = new List<object>(64);

        /// <summary>
        /// 程序集中用于标注主题的全部自定义特性
        /// </summary>
        public static Type[] Assemblies { get; } = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IThemeAttribute).IsAssignableFrom(t) && typeof(Attribute).IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();

        /// <summary>
        /// 令全局主题切换可作用至该实例
        /// </summary>
        /// <param name="source"></param>
        public static T AwakeDynamicTheme<T>(T source) where T : class
        {
            if (!ThemeValues.TryGetValue(typeof(T), out _))
            {
                if (TransitionBoard<T>.MachinePool.TryGetValue(source, out var machine))
                {
                    GenerateValue(source, machine);
                }
                else
                {
                    var newMachine = StateMachine.Create(source);
                    TransitionBoard<T>.MachinePool.Add(source, newMachine);
                    GenerateValue(source, newMachine);
                }
            }
            InstanceHosts.Add(source);
            return source;
        }

        /// <summary>
        /// 应用指定类型的主题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="attributeType">主题特性类型</param>
        /// <param name="paramAction">过渡参数构造</param>
        /// <returns></returns>
        public static T ApplyTheme<T>(this T source, Type attributeType, Action<TransitionParams>? paramAction = null) where T : class
        {
            if (ThemeValues.TryGetValue(typeof(T), out var dictionary))
            {
                if (dictionary.TryGetValue(attributeType, out var state))
                {
                    source.BeginTransition(state, paramAction ?? TransitionParams.Theme);
                }
            }
            else
            {
                if (TransitionBoard<T>.MachinePool.TryGetValue(source, out var machine))
                {
                    GenerateValue(source, machine);
                }
                else
                {
                    var newMachine = StateMachine.Create(source);
                    TransitionBoard<T>.MachinePool.Add(source, newMachine);
                    GenerateValue(source, newMachine);
                }
                ApplyTheme(source, attributeType, paramAction);
            }

            return source;
        }

        private static void GenerateValue<T>(T target, StateMachine machine)where T : class
        {
            var dic = new Dictionary<Type, State>();
            foreach (var type in Assemblies)
            {
                var state = State.FromType<T>().ToState();
                AddPropertiesWithAttribute(state, machine.DoubleProperties, type);
                AddPropertiesWithAttribute(state, machine.BrushProperties, type);
                AddPropertiesWithAttribute(state, machine.TransformProperties, type);
                AddPropertiesWithAttribute(state, machine.CornerRadiusProperties, type);
                AddPropertiesWithAttribute(state, machine.ThicknessProperties, type);
                AddPropertiesWithAttribute(state, machine.PointProperties, type);
                AddPropertiesWithAttribute(state, machine.ILinearInterpolationProperties, type);
                state.StateName = $"{typeof(T).FullName}_{type.FullName}_DynamicTheme";
                dic.Add(type, state);
            }
            ThemeValues.Add(typeof(T), dic);
        }

        private static void AddPropertiesWithAttribute(State state, IEnumerable<PropertyInfo> properties, Type attributeType)
        {
            foreach (var info in properties)
            {
                var inter = info.GetCustomAttribute(attributeType, true) as IThemeAttribute;
                if (inter != null)
                {
                    state.AddProperty(info.Name, inter.Target);
                }
            }
        }
    }
}
