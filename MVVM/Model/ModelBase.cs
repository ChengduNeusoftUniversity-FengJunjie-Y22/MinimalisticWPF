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

        public virtual Thickness FixedBorderThickness { get; set; } = new Thickness(1);

        public virtual Brush FixedBorderBrush { get; set; } = Brushes.White;

        public virtual CornerRadius CornerRadius { get; set; } = new CornerRadius(10);
      
        public virtual Brush ActualBackground { get; set; } = "#1e1e1e".ToBrush();

        public virtual double ActualBackgroundOpacity { get; set; } = 1;

        public virtual double Height { get; set; } = 80;

        public virtual double Width { get; set; } = 280;
    }
}
