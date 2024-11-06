using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public class NotificationViewModel : ViewModelBase<NotificationViewModel, NotificationModel>
    {
        public NotificationViewModel() { this.AsGlobalTheme(); }

        public string Title
        {
            get => Model.Title;
            set
            {
                Model.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }
}
