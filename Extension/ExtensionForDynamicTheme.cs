using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        public static Dictionary<Type, Dictionary<Type, State>> ThemeValues { get; internal set; } = new Dictionary<Type, Dictionary<Type, State>>();

        public static Type[] Assemblies { get; } = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IThemeAttribute).IsAssignableFrom(t) && typeof(Attribute).IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();

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

        private static void GenerateValue<T>(T target, StateMachine machine) where T : class
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
