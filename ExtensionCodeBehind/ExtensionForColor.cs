using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class ExtensionForColor
    {
        public static RGB ToRGB(this Color color)
        {
            return RGB.FromColor(color);
        }
        public static RGB ToRGB<T>(this T brush) where T : Brush
        {
            return RGB.FromBrush(brush);
        }
    }
}
