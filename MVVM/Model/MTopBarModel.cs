using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public class MTopBarModel : ModelBase
    {
        public MTopBarModel() { }

        public ImageSource? Icon { get; set; } = null;
    }
}
