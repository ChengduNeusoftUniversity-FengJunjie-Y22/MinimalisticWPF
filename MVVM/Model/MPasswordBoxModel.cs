using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class MPasswordBoxModel : ModelWithTextBase
    {
        public MPasswordBoxModel() { }

        public override double Height { get; set; } = 80;

        public override double Width { get; set; } = 380;

        public string TruePassword { get; set; } = string.Empty;

        public string UIPassword { get; set; } = string.Empty;

        public string ReplacingCharacters { get; set; } = "·";

        public Brush PasswordStrengthColor { get; set; } = Brushes.White;
    }
}
