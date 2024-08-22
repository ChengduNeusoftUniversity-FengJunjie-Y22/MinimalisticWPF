using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

namespace MinimalisticWPF
{
    /// <summary>
    /// 表示Target的一个状态
    /// </summary>
    public class State
    {
        internal State(string stateName, object Target)
        {
            StateName = stateName;

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
        public static State Creat(object Target)
        {
            State result = new State(string.Empty, Target);
            return result;
        }

        /// <summary>
        /// 为该State起个名字
        /// </summary>
        public State SetName(string stateName)
        {
            StateName = stateName;
            return this;
        }

        /// <summary>
        /// 依据属性名和double值,修改该状态对应的信息
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="newValue">新的状态值</param>
        public State SetValue(string propertyName, double newValue)
        {
            if (DoubleValues.TryGetValue(propertyName, out _))
            {
                DoubleValues[propertyName] = newValue;
            }
            return this;
        }
    }
}
