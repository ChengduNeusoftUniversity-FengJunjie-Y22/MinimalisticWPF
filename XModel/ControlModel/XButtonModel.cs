using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class XButtonModel
    {
        public XButtonModel() { }

        internal SolidColorBrush FixedTransparent { get; set; } = Brushes.Transparent;

        public string Text { get; set; } = string.Empty;

        public SolidColorBrush TextColor { get; set; } = Brushes.Transparent;

        public SolidColorBrush TextHoverColor { get; set; } = Brushes.Transparent;

        public SolidColorBrush BorderHoverBrush { get; set; } = Brushes.Transparent;

        public SolidColorBrush BorderBrush { get; set; } = Brushes.Transparent;

        public SolidColorBrush Background { get; set; } = Brushes.Transparent;

        public SolidColorBrush AnimationFill { get; set; } = Brushes.Transparent;

        public Thickness BorderThickness { get; set; } = new Thickness(1);

        public CornerRadius CornerRadius { get; set; } = new CornerRadius(5);
    }
}
