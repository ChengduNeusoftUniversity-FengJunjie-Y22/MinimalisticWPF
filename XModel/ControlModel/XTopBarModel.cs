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

        public string Title { get; set; } = string.Empty;

        public SolidColorBrush TitleColor { get; set; } = Brushes.Transparent;

        public SolidColorBrush ControlColor { get; set; } = Brushes.Transparent;

        public SolidColorBrush ControlHoverColor { get; set; } = Brushes.Transparent;
    }
}
