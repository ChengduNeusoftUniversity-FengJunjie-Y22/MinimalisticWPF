using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class XButtonModel
    {
        public XButtonModel() { }

        public string Text { get; internal set; } = string.Empty;

        public SolidColorBrush TextColor { get; internal set; } = Brushes.Transparent;

        public SolidColorBrush TextHoverColor { get; internal set; } = Brushes.Transparent;

        public SolidColorBrush Background { get; internal set; } = Brushes.Transparent;
    }
}
