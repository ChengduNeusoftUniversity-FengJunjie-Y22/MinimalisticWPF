using System.Reflection;
using System.Dynamic;
using System.Linq.Expressions;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF
{
    public class State
    {
        internal State(object Target, ICollection<string> WhileList)
        {
            ActualType = Target.GetType();
            PropertyInfo[] Properties = ActualType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.CanRead)
                .ToArray();//所有可用属性
            PropertyInfo[] DoubleProperties = Properties.Where(x => x.PropertyType == typeof(double) && WhileList.Contains(x.Name))
                .ToArray();//筛选Double属性
            PropertyInfo[] BrushProperties = Properties.Where(x => x.PropertyType == typeof(Brush) && WhileList.Contains(x.Name))
                .ToArray();//筛选Brush属性
            PropertyInfo[] TransformProperties = Properties.Where(x => x.PropertyType == typeof(Transform) && WhileList.Contains(x.Name))
                .ToArray();//筛选Transform属性
            PropertyInfo[] PointProperties = Properties.Where(x => x.PropertyType == typeof(Point) && WhileList.Contains(x.Name))
                .ToArray();//筛选Point属性
            PropertyInfo[] CornerRadiusProperties = Properties.Where(x => x.PropertyType == typeof(CornerRadius) && WhileList.Contains(x.Name))
                .ToArray();//筛选CornerRadius属性
            PropertyInfo[] ThicknessProperties = Properties.Where(x => x.PropertyType == typeof(Thickness) && WhileList.Contains(x.Name))
                .ToArray();//筛选Thickness属性
            PropertyInfo[] ILinearInterpolationProperties = Properties.Where(x => typeof(ILinearInterpolation).IsAssignableFrom(x.PropertyType) && WhileList.Contains(x.Name))
                .ToArray();//筛选ILinearInterpolation接口支持的属性

            foreach (PropertyInfo propertyInfo in DoubleProperties)
            {
                Values.Add(propertyInfo.Name, propertyInfo.GetValue(Target));
            }
            foreach (PropertyInfo propertyInfo in BrushProperties)
            {
                Values.Add(propertyInfo.Name, propertyInfo.GetValue(Target));
            }
            foreach (PropertyInfo propertyInfo in TransformProperties)
            {
                Values.Add(propertyInfo.Name, propertyInfo.GetValue(Target));
            }
            foreach (PropertyInfo propertyInfo in PointProperties)
            {
                Values.Add(propertyInfo.Name, propertyInfo.GetValue(Target));
            }
            foreach (PropertyInfo propertyInfo in CornerRadiusProperties)
            {
                Values.Add(propertyInfo.Name, propertyInfo.GetValue(Target));
            }
            foreach (PropertyInfo propertyInfo in ThicknessProperties)
            {
                Values.Add(propertyInfo.Name, propertyInfo.GetValue(Target));
            }
            foreach (PropertyInfo propertyInfo in ILinearInterpolationProperties)
            {
                Values.Add(propertyInfo.Name, propertyInfo.GetValue(Target));
            }
        }

        /// <summary>
        /// 状态的名称
        /// </summary>
        public string StateName { get; internal set; } = string.Empty;
        /// <summary>
        /// 记录状态对象的真实类型
        /// </summary>
        public Type ActualType { get; internal set; }
        /// <summary>
        /// 记录的状态值
        /// </summary>
        public Dictionary<string, object?> Values { get; internal set; } = new Dictionary<string, object?>();
        /// <summary>
        /// 获取该状态下,指定属性的具体值
        /// </summary>
        /// <param name="propertyName">属性的名称</param>
        /// <returns>double 具体值</returns>
        /// <exception cref="ArgumentException"></exception>
        public object? this[string propertyName]
        {
            get
            {
                if (!Values.TryGetValue(propertyName, out _))
                {
                    throw new ArgumentException($"There is no property State value named [ {propertyName} ] in the state named [ {StateName} ]");
                }

                return Values[propertyName];
            }
        }
        /// <summary>
        /// 开始记录指定对象的状态
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
        internal List<string> WhiteList { get; set; } = new List<string>();

        /// <summary>
        /// 记录该状态的名称
        /// </summary>
        public TempState<T> SetName(string stateName)
        {
            Name = stateName;
            return this;
        }

        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TempState<T> SetProperty(
            Expression<Func<T, double>> propertyLambda,
            double newValue)
        {
            var compiledLambda = propertyLambda.Compile();
            var obj = Value;

            if (obj == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return this;
                }
                property.SetValue(obj, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TempState<T> SetProperty(
            Expression<Func<T, Brush>> propertyLambda,
            Brush newValue)
        {
            var compiledLambda = propertyLambda.Compile();
            var obj = Value;

            if (obj == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Brush))
                {
                    return this;
                }
                property.SetValue(obj, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TempState<T> SetProperty(
            Expression<Func<T, Transform>> propertyLambda,
            Transform newValue)
        {
            var compiledLambda = propertyLambda.Compile();
            var obj = Value;

            if (obj == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Transform))
                {
                    return this;
                }
                property.SetValue(obj, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TempState<T> SetProperty(
            Expression<Func<T, Point>> propertyLambda,
            Point newValue)
        {
            var compiledLambda = propertyLambda.Compile();
            var obj = Value;

            if (obj == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Point))
                {
                    return this;
                }
                property.SetValue(obj, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TempState<T> SetProperty(
            Expression<Func<T, CornerRadius>> propertyLambda,
            CornerRadius newValue)
        {
            var compiledLambda = propertyLambda.Compile();
            var obj = Value;

            if (obj == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(CornerRadius))
                {
                    return this;
                }
                property.SetValue(obj, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TempState<T> SetProperty(
            Expression<Func<T, Thickness>> propertyLambda,
            Thickness newValue)
        {
            var compiledLambda = propertyLambda.Compile();
            var obj = Value;

            if (obj == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Thickness))
                {
                    return this;
                }
                property.SetValue(obj, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TempState<T> SetProperty(
            Expression<Func<T, ILinearInterpolation>> propertyLambda,
            ILinearInterpolation newValue)
        {
            var compiledLambda = propertyLambda.Compile();
            var obj = Value;

            if (obj == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(ILinearInterpolation))
                {
                    return this;
                }
                property.SetValue(obj, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }


        /// <summary>
        /// 记录完毕
        /// </summary>
        public State ToState()
        {
            State result = new State(Value, WhiteList);
            result.StateName = Name;
            return result;
        }
        internal State ToState(ICollection<string> whiteList)
        {
            State result = new State(Value, whiteList);
            result.StateName = Name;
            return result;
        }
    }
}
