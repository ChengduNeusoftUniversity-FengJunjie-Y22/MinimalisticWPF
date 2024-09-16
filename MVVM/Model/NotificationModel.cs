using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.MVVM.Model
{
    public class NotificationModel : ModelBase
    {
        public NotificationModel() { }

        public string Title { get; set; } = string.Empty;
    }
}
