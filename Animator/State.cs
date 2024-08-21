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
        public State(string stateName, object Target)
        {
            StateName = stateName;

            Type type = Target.GetType();
            PropertyInfo[] Properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in Properties)
            {
                if (propertyInfo.PropertyType == typeof(double))
                {
                    DoubleValues.Add(propertyInfo.Name, (double)propertyInfo.GetValue(Target));
                }
            }
        }

        /// <summary>
        /// 状态的名称
        /// </summary>
        public string StateName { get; set; } = string.Empty;

        internal Dictionary<string, double> DoubleValues { get; set; } = new Dictionary<string, double>();

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
    }
}
