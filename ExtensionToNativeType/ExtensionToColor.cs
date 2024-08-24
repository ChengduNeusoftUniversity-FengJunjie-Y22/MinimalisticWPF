using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class ExtensionToBrush
    {
        public static Color Lerp(this Color startColor, Color endColor, double t)
        {
            // 计算颜色的插值
            byte r = (byte)(startColor.R + (endColor.R - startColor.R) * t);
            byte g = (byte)(startColor.G + (endColor.G - startColor.G) * t);
            byte b = (byte)(startColor.B + (endColor.B - startColor.B) * t);
            byte a = (byte)(startColor.A + (endColor.A - startColor.A) * t);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
