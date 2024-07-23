using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class XTopBarModel
    {
        public XTopBarModel() { }

        public string Title { get; internal set; } = string.Empty;

        public SolidColorBrush TitleColor { get; internal set; } = Brushes.Transparent;

        public SolidColorBrush ControlColor { get; internal set; } = Brushes.Transparent;

        public SolidColorBrush ControlHoverColor { get; internal set; } = Brushes.Transparent;
    }
}
