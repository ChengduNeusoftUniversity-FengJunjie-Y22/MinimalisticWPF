﻿using System;
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

        internal static void Scan()
        {
            Pages = Array.Empty<IPageChanging?>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IPageChanging))))
                                .ToArray();
            Pages = types.Select(x => Activator.CreateInstance(x) as IPageChanging)
                .Where(x => x != null)
                .ToArray();
        }

        public static object? Find(Type pageType)
        {
            return Pages.FirstOrDefault(x => x?.GetPage()?.GetType() == pageType);
        }

        public static object? Find(string pageName)
        {
            return Pages.FirstOrDefault(x => x?.GetPageName() == pageName)?.GetPage();
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
