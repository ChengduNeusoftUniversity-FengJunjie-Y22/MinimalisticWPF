using System.Reflection;
using System.Dynamic;
using System.Linq.Expressions;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF
{
    public class State
    {
        internal State() { }
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
        public Type? ActualType { get; internal set; } = default;
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
        /// 具备额外的反射开销,可自动记录对象当前所有满足条件的属性及其值到Values中
        /// </summary>
        public static ObjectTempState<T> FromObject<T>(T Target) where T : class
        {
            ObjectTempState<T> result = new ObjectTempState<T>(Target);
            return result;
        }
        /// <summary>
        /// 不具备额外的反射开销,直接为Values添加键值对
        /// </summary>
        public static TypeTempState<T> FromType<T>() where T : class
        {
            TypeTempState<T> state = new TypeTempState<T>();
            return state;
        }
    }

    public class ObjectTempState<T>
    {
        internal ObjectTempState(T target) { Value = target; }

        internal T Value { get; set; }
        internal string Name { get; set; } = string.Empty;
        internal List<string> WhiteList { get; set; } = new List<string>();

        /// <summary>
        /// 记录该状态的名称
        /// </summary>
        public ObjectTempState<T> SetName(string stateName)
        {
            Name = stateName;
            return this;
        }

        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, double>> propertyLambda,
            double newValue)
        {
            if (Value == null)
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
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, Brush>> propertyLambda,
            Brush newValue)
        {
            if (Value == null)
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
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, Transform>> propertyLambda,
            Transform newValue)
        {


            if (Value == null)
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
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, Point>> propertyLambda,
            Point newValue)
        {
            if (Value == null)
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
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, CornerRadius>> propertyLambda,
            CornerRadius newValue)
        {
            if (Value == null)
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
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, Thickness>> propertyLambda,
            Thickness newValue)
        {
            if (Value == null)
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
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, ILinearInterpolation>> propertyLambda,
            ILinearInterpolation newValue)
        {
            if (Value == null)
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
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }


        /// <summary>
        /// 完成记录
        /// </summary>
        /// <param name="IsWhiteList">是否启用白名单</param>
        /// <returns>State</returns>
        public State ToState(bool IsWhiteList = true)
        {
            if (!IsWhiteList) WhiteList.Clear();

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

    public class TypeTempState<T>
    {
        internal TypeTempState() { Target = new State(); }

        internal State Target { get; set; }

        public TypeTempState<T> SetName(string name)
        {
            Target.StateName = name;
            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TypeTempState<T> SetProperty(
            Expression<Func<T, double>> propertyLambda,
            double newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.Values.Add(property.Name, newValue);
                }
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TypeTempState<T> SetProperty(
            Expression<Func<T, Brush>> propertyLambda,
            Brush newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Brush))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.Values.Add(property.Name, newValue);
                }
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TypeTempState<T> SetProperty(
            Expression<Func<T, CornerRadius>> propertyLambda,
            CornerRadius newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(CornerRadius))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.Values.Add(property.Name, newValue);
                }
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TypeTempState<T> SetProperty(
            Expression<Func<T, Thickness>> propertyLambda,
            Thickness newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Thickness))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.Values.Add(property.Name, newValue);
                }
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TypeTempState<T> SetProperty(
            Expression<Func<T, Transform>> propertyLambda,
            Transform newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Transform))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.Values.Add(property.Name, newValue);
                }
            }

            return this;
        }
        /// <summary>
        /// 记录新状态对应的属性值
        /// </summary>
        public TypeTempState<T> SetProperty(
            Expression<Func<T, ILinearInterpolation>> propertyLambda,
            ILinearInterpolation newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(ILinearInterpolation))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.Values.Add(property.Name, newValue);
                }
            }

            return this;
        }

        /// <summary>
        /// 记录完成
        /// </summary>
        public State ToState()
        {
            return Target;
        }
    }
}
