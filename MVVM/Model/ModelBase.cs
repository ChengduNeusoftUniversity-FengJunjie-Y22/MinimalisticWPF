using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public abstract class ModelBase
    {
        public virtual Brush FixedTransparent { get; set; } = Brushes.Transparent;
        public virtual Thickness FixedNoThickness { get; set; } = new Thickness(0);
        public virtual CornerRadius FixedNoCornerRadius { get; set; } = new CornerRadius(0);
        public virtual string Text { get; set; } = string.Empty;
        public virtual Brush TextBrush { get; set; } = Brushes.Transparent;
        public virtual double TextSize { get; set; } = 30;
        public virtual double FontSizeConvertRate { get; set; } = 0.7;
        public virtual Thickness EdgeThickness { get; set; } = new Thickness(1);
        public virtual Brush EdgeBrush { get; set; } = Brushes.Transparent;
        public virtual CornerRadius CornerRadius { get; set; } = new CornerRadius(0);
        public virtual Brush BackBrush { get; set; } = Brushes.Transparent;
        public virtual Brush HoverBackground { get; set; } = Brushes.Transparent;
        public virtual Brush HoverTextBrush { get; set; } = Brushes.Transparent;
        public virtual Brush HoverEdgeBrush { get; set; } = Brushes.Transparent;
        public virtual double HoverBackgroundOpacity { get; set; } = 0;
        public virtual double HoverGlobalOpacity { get; set; } = 0;
        public virtual double Height { get; set; } = double.NaN;
        public virtual double Width { get; set; } = double.NaN;
    }
}
