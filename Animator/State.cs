using System.Reflection;
using System.Dynamic;

namespace MinimalisticWPF
{
    /// <summary>
    /// 表示Target的一个状态
    /// </summary>
    public class State
    {
        internal State(object Target)
        {
            ObjType = Target.GetType();
            PropertyInfo[] Properties = ObjType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                            .Where(x => x.CanWrite && x.CanRead && x.PropertyType == typeof(double))
                                            .ToArray();

            foreach (PropertyInfo propertyInfo in Properties)
            {
                DoubleValues.Add(propertyInfo.Name, (double)propertyInfo.GetValue(Target));
            }
        }

        /// <summary>
        /// 状态的名称
        /// </summary>
        public string StateName { get; internal set; } = string.Empty;

        public Type ObjType { get; internal set; }

        public Dictionary<string, double> DoubleValues { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// 获取该状态下,指定属性的具体值
        /// </summary>
        /// <param name="propertyName">属性的名称</param>
        /// <returns>double 具体值</returns>
        /// <exception cref="ArgumentException"></exception>
        public double this[string propertyName]
        {
            get
            {
                if (!DoubleValues.TryGetValue(propertyName, out _))
                {
                    throw new ArgumentException($"There is no property State value named [ {propertyName} ] in the state named [ {StateName} ]");
                }

                return DoubleValues[propertyName];
            }
        }

        /// <summary>
        /// 记录一个实例对象为State
        /// </summary>
        public static TempState<T> FromObject<T>(T Target) where T : class
        {
            TempState<T> result = new TempState<T>(Target);
            return result;
        }
    }

    public class TempState<T>
    {
        internal TempState(T target) { Value = target; }

        internal T Value { get; set; }

        internal string Name { get; set; } = string.Empty;
    }
}
