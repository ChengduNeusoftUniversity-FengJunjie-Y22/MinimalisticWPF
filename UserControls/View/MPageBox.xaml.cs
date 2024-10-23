using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinimalisticWPF
{
    public partial class MPageBox : UserControl
    {
        public MPageBox()
        {
            InitializeComponent();
        }

        public void Navigate(Type pageType)
        {
            UpdateSource(PageManager.Find(pageType));
        }
        public void Navigate(string pageName)
        {
            UpdateSource(PageManager.Find(pageName));
        }
        public void Navigate(int pageIndex)
        {
            UpdateSource(PageManager.Find(pageIndex));
        }

        private void UpdateSource(object? data)
        {
            if (data == null)
            {
                CurrentPage.Child = null;
                return;
            }
            var page = data as UIElement;
            var method = page as IPageNavigate;
            var size = method?.GetPageSize() ?? new Size(Width, Height);
            Width = size.Width;
            Height = size.Height;
            CurrentPage.Child = page;
        }
    }
}
