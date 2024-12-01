using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public sealed class LightBrushes : IThemeBrushes
    {
        internal LightBrushes() { }
        public static LightBrushes Selector { get; } = new LightBrushes();

        public static Brush Default = Brushes.Transparent;

        public static Brush H1 = new RGB(0, 0, 0, 255).Brush;
        public static Brush H2 = new RGB(0, 0, 0, 200).Brush;
        public static Brush H3 = new RGB(0, 0, 0, 155).Brush;
        public static Brush H4 = new RGB(0, 0, 0, 100).Brush;
        public static Brush H5 = new RGB(0, 0, 0, 55).Brush;

        public static Brush P1 = new RGB(100, 100, 100, 255).Brush;
        public static Brush P2 = new RGB(120, 120, 120, 255).Brush;
        public static Brush P3 = new RGB(150, 150, 150, 255).Brush;
        public static Brush P4 = new RGB(180, 180, 180, 255).Brush;
        public static Brush P5 = new RGB(200, 200, 200, 255).Brush;

        public static Brush B1 = new RGB(0, 0, 0, 0).Brush;
        public static Brush B2 = new RGB(230, 230, 230, 255).Brush;
        public static Brush B3 = new RGB(220, 220, 220, 255).Brush;
        public static Brush B4 = new RGB(210, 210, 210, 255).Brush;
        public static Brush B5 = new RGB(200, 200, 200, 255).Brush;

        public static Brush E1 = new RGB(150, 150, 150, 255).Brush;
        public static Brush E2 = new RGB(180, 180, 180, 255).Brush;
        public static Brush E3 = new RGB(200, 200, 200, 255).Brush;
        public static Brush E4 = new RGB(220, 220, 220, 255).Brush;
        public static Brush E5 = new RGB(240, 240, 240, 255).Brush;

        public static Brush F1 = Brushes.Violet;
        public static Brush F2 = new RGB(0, 0, 0, 80).Brush;
        public static Brush F3 = new RGB(0, 0, 0, 60).Brush;
        public static Brush F4 = new RGB(0, 0, 0, 40).Brush;
        public static Brush F5 = new RGB(0, 0, 0, 20).Brush;

        public Brush Select(BrushTags brushenum)
        {
            switch (brushenum)
            {
                case BrushTags.Default:
                    return Default;
                case BrushTags.H1:
                    return H1;
                case BrushTags.H2:
                    return H2;
                case BrushTags.H3:
                    return H3;
                case BrushTags.H4:
                    return H4;
                case BrushTags.H5:
                    return H5;
                case BrushTags.P1:
                    return P1;
                case BrushTags.P2:
                    return P2;
                case BrushTags.P3:
                    return P3;
                case BrushTags.P4:
                    return P4;
                case BrushTags.P5:
                    return P5;
                case BrushTags.B1:
                    return B1;
                case BrushTags.B2:
                    return B2;
                case BrushTags.B3:
                    return B3;
                case BrushTags.B4:
                    return B4;
                case BrushTags.B5:
                    return B5;
                case BrushTags.E1:
                    return E1;
                case BrushTags.E2:
                    return E2;
                case BrushTags.E3:
                    return E3;
                case BrushTags.E4:
                    return E4;
                case BrushTags.E5:
                    return E5;
                case BrushTags.F1:
                    return F1;
                case BrushTags.F2:
                    return F2;
                case BrushTags.F3:
                    return F3;
                case BrushTags.F4:
                    return F4;
                case BrushTags.F5:
                    return F5;
                default:
                    throw new ArgumentOutOfRangeException(nameof(brushenum), brushenum, null);
            }
        }
    }
}
