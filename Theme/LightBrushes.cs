using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class LightBrushes
    {
        public static Brush Default = Brushes.Transparent;

        public static Brush H1 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public static Brush H2 = new SolidColorBrush(Color.FromRgb(50, 50, 50));
        public static Brush H3 = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        public static Brush H4 = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        public static Brush H5 = new SolidColorBrush(Color.FromRgb(200, 200, 200));

        public static Brush P1 = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        public static Brush P2 = new SolidColorBrush(Color.FromRgb(120, 120, 120));
        public static Brush P3 = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        public static Brush P4 = new SolidColorBrush(Color.FromRgb(180, 180, 180));
        public static Brush P5 = new SolidColorBrush(Color.FromRgb(200, 200, 200));

        public static Brush B1 = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        public static Brush B2 = new SolidColorBrush(Color.FromRgb(230, 230, 230));
        public static Brush B3 = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        public static Brush B4 = new SolidColorBrush(Color.FromRgb(210, 210, 210));
        public static Brush B5 = new SolidColorBrush(Color.FromRgb(200, 200, 200));

        public static Brush E1 = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        public static Brush E2 = new SolidColorBrush(Color.FromRgb(180, 180, 180));
        public static Brush E3 = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        public static Brush E4 = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        public static Brush E5 = new SolidColorBrush(Color.FromRgb(240, 240, 240));

        public static Brush Select(LightBrushPackage brushenum)
        {
            switch (brushenum)
            {
                case LightBrushPackage.Default:
                    return Default;
                case LightBrushPackage.H1:
                    return H1;
                case LightBrushPackage.H2:
                    return H2;
                case LightBrushPackage.H3:
                    return H3;
                case LightBrushPackage.H4:
                    return H4;
                case LightBrushPackage.H5:
                    return H5;
                case LightBrushPackage.P1:
                    return P1;
                case LightBrushPackage.P2:
                    return P2;
                case LightBrushPackage.P3:
                    return P3;
                case LightBrushPackage.P4:
                    return P4;
                case LightBrushPackage.P5:
                    return P5;
                case LightBrushPackage.B1:
                    return B1;
                case LightBrushPackage.B2:
                    return B2;
                case LightBrushPackage.B3:
                    return B3;
                case LightBrushPackage.B4:
                    return B4;
                case LightBrushPackage.B5:
                    return B5;
                case LightBrushPackage.E1:
                    return E1;
                case LightBrushPackage.E2:
                    return E2;
                case LightBrushPackage.E3:
                    return E3;
                case LightBrushPackage.E4:
                    return E4;
                case LightBrushPackage.E5:
                    return E5;
                default:
                    throw new ArgumentOutOfRangeException(nameof(brushenum), brushenum, null);
            }
        }
    }
}
