using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MinimalisticWPF
{
    /// <summary>
    /// 覆写返回值行为逻辑
    /// </summary>
    public delegate object? ProxyReturnHandler(object?[]? args);

    /// <summary>
    /// 代理中间件,存储代理行为
    /// </summary>
    public class ProxyInstance : DispatchProxy
    {
        internal static int _id = 0;
        public static Dictionary<int, ProxyInstance> ProxyInstances { get; internal set; } = new Dictionary<int, ProxyInstance>();
        public static Dictionary<object, int> ProxyIDs { get; internal set; } = new Dictionary<object, int>();

        public ProxyInstance() { _localid = _id; _id++; ProxyInstances.Add(_localid, this); }

        internal object? _target;
        internal Type? _targetType;
        internal int _localid = 0;

        internal Dictionary<string, Tuple<Action?, ProxyReturnHandler?, Action?>> GetterActions { get; set; } = new Dictionary<string, Tuple<Action?, ProxyReturnHandler?, Action?>>();
        internal Dictionary<string, Tuple<Action?, Action?, Action?>> SetterActions { get; set; } = new Dictionary<string, Tuple<Action?, Action?, Action?>>();
        internal Dictionary<string, Tuple<Action?, ProxyReturnHandler?, Action?>> MethodActions { get; set; } = new Dictionary<string, Tuple<Action?, ProxyReturnHandler?, Action?>>();

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            var Name = targetMethod?.Name ?? string.Empty;

            if (Name == string.Empty) return null;

            if (Name.StartsWith("get_"))
            {
                GetterActions.TryGetValue(Name, out var actions);
                actions?.Item1?.Invoke();
                var result = actions?.Item2 == null ? _targetType?.GetMethod(Name)?.Invoke(_target, args) : actions.Item2.Invoke(args);
                actions?.Item3?.Invoke();
                return result;
            }
            else if (Name.StartsWith("set_"))
            {
                SetterActions.TryGetValue(Name, out var actions);
                actions?.Item1?.Invoke();
                var result = actions?.Item2 == null ? _targetType?.GetMethod(Name)?.Invoke(_target, args) : null;
                actions?.Item3?.Invoke();
                return result;
            }
            else
            {
                MethodActions.TryGetValue(Name, out var actions);
                actions?.Item1?.Invoke();
                var result = actions?.Item2 == null ? _targetType?.GetMethod(Name)?.Invoke(_target, args) : actions.Item2.Invoke(args);
                actions?.Item3?.Invoke();
                return result;
            }
        }
    }
}
