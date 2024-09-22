using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

public enum ProgressShapes
{
    Ring,
    Strip
}

namespace MinimalisticWPF
{
    public class MProgressBarModel : ModelBase
    {
        public MProgressBarModel() { }

        public ProgressShapes Type { get; set; } = ProgressShapes.Ring;

        public double Value { get; set; } = 0;
        public double StripValue { get; set; } = 0;

        public double StartAngle { get; set; } = 0;

        public double BaseEndAngle { get; set; } = 360;
        public double FillEndAngle { get; set; } = 0;

        public Brush FillBrush { get; set; } = Brushes.Cyan;
        public Brush BaseColor { get; set; } = Brushes.Gray;

        public double Thickness { get; set; } = 5;

        public double RingOpacity { get; set; } = 1;
        public double StripOpacity { get; set; } = 0;
    }
}
