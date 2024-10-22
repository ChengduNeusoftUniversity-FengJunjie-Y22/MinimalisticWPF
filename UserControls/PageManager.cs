using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MinimalisticWPF
{
    public static class PageManager
    {
        public static IPageChanging?[] Pages { get; private set; } = Array.Empty<IPageChanging?>();

        public static void Scan()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IPageChanging))))
                                .ToArray();
            Pages = types.Select(x => Activator.CreateInstance(x) as IPageChanging)
                .Where(x => x != null)
                .ToArray();
        }

        public static object? Find(Type pageType, int? index = null)
        {
            return index == null ? Pages.FirstOrDefault(x => x?.GetType() == pageType) : index > 0 && index < Pages.Length ? Pages[(int)index]?.GetPage() : null;
        }

        public static object? Find(string pageName)
        {
            return Pages.FirstOrDefault(x => x?.PageName == pageName)?.GetPage();
        }

        public static object? Find(int pageIndex)
        {
            if (pageIndex > 0 && pageIndex < Pages.Length)
            {
                return Pages[pageIndex]?.GetPage();
            }
            return null;
        }
    }
}
