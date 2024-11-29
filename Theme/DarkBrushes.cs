using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class DarkBrushes
    {
        public static Brush Default = Brushes.Transparent;

        public static Brush H1 = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        public static Brush H2 = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        public static Brush H3 = new SolidColorBrush(Color.FromRgb(180, 180, 180));
        public static Brush H4 = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        public static Brush H5 = new SolidColorBrush(Color.FromRgb(100, 100, 100));

        public static Brush P1 = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        public static Brush P2 = new SolidColorBrush(Color.FromRgb(180, 180, 180));
        public static Brush P3 = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        public static Brush P4 = new SolidColorBrush(Color.FromRgb(120, 120, 120));
        public static Brush P5 = new SolidColorBrush(Color.FromRgb(80, 80, 80));

        public static Brush B1 = new SolidColorBrush(Color.FromRgb(50, 50, 50));
        public static Brush B2 = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        public static Brush B3 = new SolidColorBrush(Color.FromRgb(30, 30, 30));
        public static Brush B4 = new SolidColorBrush(Color.FromRgb(20, 20, 20));
        public static Brush B5 = new SolidColorBrush(Color.FromRgb(10, 10, 10));

        public static Brush E1 = new SolidColorBrush(Color.FromRgb(100, 100, 100));
        public static Brush E2 = new SolidColorBrush(Color.FromRgb(80, 80, 80));
        public static Brush E3 = new SolidColorBrush(Color.FromRgb(60, 60, 60));
        public static Brush E4 = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        public static Brush E5 = new SolidColorBrush(Color.FromRgb(20, 20, 20));

        public static Brush Select(DarkBrushPackage brushenum)
        {
            switch (brushenum)
            {
                case DarkBrushPackage.Default:
                    return Default;
                case DarkBrushPackage.H1:
                    return H1;
                case DarkBrushPackage.H2:
                    return H2;
                case DarkBrushPackage.H3:
                    return H3;
                case DarkBrushPackage.H4:
                    return H4;
                case DarkBrushPackage.H5:
                    return H5;
                case DarkBrushPackage.P1:
                    return P1;
                case DarkBrushPackage.P2:
                    return P2;
                case DarkBrushPackage.P3:
                    return P3;
                case DarkBrushPackage.P4:
                    return P4;
                case DarkBrushPackage.P5:
                    return P5;
                case DarkBrushPackage.B1:
                    return B1;
                case DarkBrushPackage.B2:
                    return B2;
                case DarkBrushPackage.B3:
                    return B3;
                case DarkBrushPackage.B4:
                    return B4;
                case DarkBrushPackage.B5:
                    return B5;
                case DarkBrushPackage.E1:
                    return E1;
                case DarkBrushPackage.E2:
                    return E2;
                case DarkBrushPackage.E3:
                    return E3;
                case DarkBrushPackage.E4:
                    return E4;
                case DarkBrushPackage.E5:
                    return E5;
                default:
                    throw new ArgumentOutOfRangeException(nameof(brushenum), brushenum, null);
            }
        }
    }
}
