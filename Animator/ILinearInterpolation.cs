using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    /// <summary>
    /// 使得自定义类型支持状态机过渡
    /// </summary>
    public interface ILinearInterpolation
    {
        /// <summary>
        /// 当前具体实现类实例
        /// </summary>
        object Current { get; set; }

        /// <summary>
        /// 计算线性插值
        /// </summary>
        /// <param name="current">当前状态</param>
        /// <param name="target">最终状态</param>
        /// <param name="steps">插值数量</param>
        List<object?> Interpolate(object? current, object? target, double steps);

        /// <summary>
        /// double插值
        /// </summary>
        public static List<object?> DoubleComputing(object? start, object? end, int steps)
        {
            List<object?> result = new List<object?>();

            var d1 = start as double?;
            var d2 = end as double?;
            d1 = d1 == null || d1 == double.NaN ? 0 : d1;
            d2 = d2 == null || d2 == double.NaN ? 0 : d2;
            var delta = d2 - d1;

            for (var i = 0; i <= steps; i++)
            {
                var t = (double)(i + 1) / steps;
                result.Add(d1 + t * delta);
            }

            return result;
        }
        public static List<object?> BrushComputing(object? start, object? end, int steps)
        {
            List<object?> result = new List<object?>();

            var color1 = (start as SolidColorBrush)?.Color;
            var color2 = (end as SolidColorBrush)?.Color;
            color1 = color1 ?? new Color();
            color2 = color2 ?? new Color();

            for (var i = 0; i <= steps; i++)
            {
                var t = (double)(i + 1) / steps;
                var r = (byte)(color1.Value.R + t * (color2.Value.R - color1.Value.R));
                var g = (byte)(color1.Value.G + t * (color2.Value.G - color1.Value.G));
                var b = (byte)(color1.Value.B + t * (color2.Value.B - color1.Value.B));
                result.Add(new SolidColorBrush(Color.FromRgb(r, g, b)));
            }

            return result;
        }
        public static List<object?> TransformComputing(object? start, object? end, int steps)
        {
            List<object?> result = new List<object?>();

            Matrix matrix1 = ((Transform)(start ?? new TransformGroup())).Value;
            Matrix matrix2 = ((Transform)(end ?? new TransformGroup())).Value;

            for (int i = 0; i <= steps; i++)
            {
                var t = (i + 1) / steps;

                double m11 = matrix1.M11 + t * (matrix2.M11 - matrix1.M11);
                double m12 = matrix1.M12 + t * (matrix2.M12 - matrix1.M12);
                double m21 = matrix1.M21 + t * (matrix2.M21 - matrix1.M21);
                double m22 = matrix1.M22 + t * (matrix2.M22 - matrix1.M22);
                double offsetX = matrix1.OffsetX + t * (matrix2.OffsetX - matrix1.OffsetX);
                double offsetY = matrix1.OffsetY + t * (matrix2.OffsetY - matrix1.OffsetY);

                var interpolatedMatrixStr = $"{m11},{m12},{m21},{m22},{offsetX},{offsetY}";
                var transform = Transform.Parse(interpolatedMatrixStr);
                result.Add(transform);
            }

            return result;
        }
        public static List<object?> PointComputing(object? start, object? end, int steps)
        {
            List<object?> result = new List<object?>();



            return result;
        }
        public static List<object?> ThicknessComputing(object? start, object? end, int steps)
        {
            List<object?> result = new List<object?>();



            return result;
        }
        public static List<object?> CornerRadiusComputing(object? start, object? end, int steps)
        {
            List<object?> result = new List<object?>();



            return result;
        }
        private static Color InterpolateColor(Color colorA, Color colorB, double ratio)
        {
            byte r = (byte)(colorA.R + (colorB.R - colorA.R) * ratio);
            byte g = (byte)(colorA.G + (colorB.G - colorA.G) * ratio);
            byte b = (byte)(colorA.B + (colorB.B - colorA.B) * ratio);
            byte a = (byte)(colorA.A + (colorB.A - colorA.A) * ratio);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
