using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public abstract class ModelWithTextBase : ModelBase
    {
        public double FontSizeConvertRate { get; set; } = 0.7;

        public double FontSize { get; set; } = 56;

        public string Text { get; set; } = "MButton";

        public Brush Foreground { get; set; } = Brushes.White;
    }
}
