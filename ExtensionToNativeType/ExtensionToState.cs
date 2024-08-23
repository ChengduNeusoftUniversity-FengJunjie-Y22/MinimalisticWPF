using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public static class ExtensionToState
    {
        /// <summary>
        /// 名称
        /// </summary>
        public static TempState<T> SetName<T>(this TempState<T> source, string stateName) where T : class
        {
            source.Name = stateName;
            return source;
        }

        /// <summary>
        /// 状态值
        /// </summary>
        public static TempState<T> SetProperty<T>(
        this TempState<T> source,
        Expression<Func<T, double>> propertyLambda,
        double newValue) where T : class
        {
            var compiledLambda = propertyLambda.Compile();
            var obj = source.Value;

            if (obj == null)
            {
                return source;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return source;
                }

                property.SetValue(obj, newValue);
            }

            return source;
        }

        /// <summary>
        /// 输出State
        /// </summary>
        public static State ToState<T>(this TempState<T> source) where T : class
        {
            State result = new State(source.Value);
            result.StateName = source.Name;
            return result;
        }
    }
}
