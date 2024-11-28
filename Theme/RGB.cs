using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public struct RGB
    {
        public RGB() { R = 0; G = 0; B = 0; A = 255; }
        public RGB(int r, int g, int b)
        {
            R = Math.Clamp(r, 0, 255);
            G = Math.Clamp(g, 0, 255);
            B = Math.Clamp(b, 0, 255);
            A = 255;
        }
        public RGB(int r, int g, int b, int a)
        {
            R = Math.Clamp(r, 0, 255);
            G = Math.Clamp(g, 0, 255);
            B = Math.Clamp(b, 0, 255);
            A = Math.Clamp(a, 0, 255);
        }

        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }

        public Color Color => Color.FromArgb((byte)A, (byte)R, (byte)G, (byte)B);
        public SolidColorBrush SolidColorBrush => new(Color);
        public Brush Brush => SolidColorBrush;

        public static RGB FromColor(Color color)
        {
            return new RGB(color.R, color.G, color.B, color.A);
        }
        public static RGB FromBrush(Brush brush)
        {
            var color = (Color)ColorConverter.ConvertFromString(brush.ToString());
            return new RGB(color.R, color.G, color.B, color.A);
        }

        public RGB Scale(double rateR, double rateG, double rateB, double rateA)
        {
            int newR = Math.Clamp((int)(R * rateR), 0, 255);
            int newG = Math.Clamp((int)(G * rateG), 0, 255);
            int newB = Math.Clamp((int)(B * rateB), 0, 255);
            int newA = Math.Clamp((int)(A * rateA), 0, 255);
            return new RGB(newR, newG, newB, newA);
        }
        public RGB Scale(double rateRGB, double rateA)
        {
            int newR = Math.Clamp((int)(R * rateRGB), 0, 255);
            int newG = Math.Clamp((int)(G * rateRGB), 0, 255);
            int newB = Math.Clamp((int)(B * rateRGB), 0, 255);
            int newA = Math.Clamp((int)(A * rateA), 0, 255);
            return new RGB(newR, newG, newB, newA);
        }
        public RGB Scale(double rateRGBA)
        {
            int newR = Math.Clamp((int)(R * rateRGBA), 0, 255);
            int newG = Math.Clamp((int)(G * rateRGBA), 0, 255);
            int newB = Math.Clamp((int)(B * rateRGBA), 0, 255);
            int newA = Math.Clamp((int)(A * rateRGBA), 0, 255);
            return new RGB(newR, newG, newB, newA);
        }

        public RGB Delta(double deltaR, double deltaG, double deltaB, double deltaA)
        {
            int newR = Math.Clamp((int)(R + deltaR), 0, 255);
            int newG = Math.Clamp((int)(G + deltaG), 0, 255);
            int newB = Math.Clamp((int)(B + deltaB), 0, 255);
            int newA = Math.Clamp((int)(A + deltaA), 0, 255);
            return new RGB(newR, newG, newB, newA);
        }
        public RGB Delta(double deltaRGB, double deltaA)
        {
            int newR = Math.Clamp((int)(R + deltaRGB), 0, 255);
            int newG = Math.Clamp((int)(G + deltaRGB), 0, 255);
            int newB = Math.Clamp((int)(B + deltaRGB), 0, 255);
            int newA = Math.Clamp((int)(A + deltaA), 0, 255);
            return new RGB(newR, newG, newB, newA);
        }
        public RGB Delta(double deltaRGBA)
        {
            int newR = Math.Clamp((int)(R + deltaRGBA), 0, 255);
            int newG = Math.Clamp((int)(G + deltaRGBA), 0, 255);
            int newB = Math.Clamp((int)(B + deltaRGBA), 0, 255);
            int newA = Math.Clamp((int)(A + deltaRGBA), 0, 255);
            return new RGB(newR, newG, newB, newA);
        }
    }
}
