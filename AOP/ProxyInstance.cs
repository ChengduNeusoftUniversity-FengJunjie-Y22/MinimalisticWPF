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
    /// 编写 [ 切面逻辑 ] 时使用
    /// </summary>
    /// <param name="args">在覆写方法时,由args顺序获取本次传给方法的参数值</param>
    /// <param name="lastResult">上一个方法返回的值</param>
    /// <param name="result">方法执行结束后,由result获取本次方法返回值</param>
    /// <returns>
    /// <para>[ getter ] 覆写时 , 需要返回新的getter逻辑返回的值</para>
    /// <para>[ method ] 覆写时 , 需要返回新的method逻辑返回的值</para>
    /// <para>[ 扩展 ] 而非覆写时,返回null即可</para>
    /// </returns>
    public delegate object? ProxyHandler(object?[]? args, object? lastResult);

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

        internal Dictionary<string, Tuple<ProxyHandler?, ProxyHandler?, ProxyHandler?>> GetterActions { get; set; } = new Dictionary<string, Tuple<ProxyHandler?, ProxyHandler?, ProxyHandler?>>();
        internal Dictionary<string, Tuple<ProxyHandler?, ProxyHandler?, ProxyHandler?>> SetterActions { get; set; } = new Dictionary<string, Tuple<ProxyHandler?, ProxyHandler?, ProxyHandler?>>();
        internal Dictionary<string, Tuple<ProxyHandler?, ProxyHandler?, ProxyHandler?>> MethodActions { get; set; } = new Dictionary<string, Tuple<ProxyHandler?, ProxyHandler?, ProxyHandler?>>();

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            var Name = targetMethod?.Name ?? string.Empty;

            if (Name == string.Empty) return null;

            if (Name.StartsWith("get_"))
            {
                GetterActions.TryGetValue(Name, out var actions);
                var R0 = actions?.Item1?.Invoke(args, null);
                var R1 = actions?.Item2 == null ? _targetType?.GetMethod(Name)?.Invoke(_target, args) : actions.Item2.Invoke(args, R0);
                actions?.Item3?.Invoke(args, R1);
                return R1;
            }
            else if (Name.StartsWith("set_"))
            {
                SetterActions.TryGetValue(Name, out var actions);
                var R0 = actions?.Item1?.Invoke(args, null);
                var R1 = actions?.Item2 == null ? _targetType?.GetMethod(Name)?.Invoke(_target, args) : actions.Item2.Invoke(args, R0);
                actions?.Item3?.Invoke(args, R1);
                return R1;
            }
            else
            {
                MethodActions.TryGetValue(Name, out var actions);
                var R0 = actions?.Item1?.Invoke(args, null);
                var R1 = actions?.Item2 == null ? _targetType?.GetMethod(Name)?.Invoke(_target, args) : actions.Item2.Invoke(args, R0);
                actions?.Item3?.Invoke(args, R1);
                return R1;
            }
        }
    }
}
