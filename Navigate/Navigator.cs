using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MinimalisticWPF
{
    public static class Navigator
    {
        public static bool IsInitialized { get; private set; } = false;
        public static Dictionary<Type, Navigable> NavigableAttributes { get; private set; } = new Dictionary<Type, Navigable>();
        public static Dictionary<Type, object> Singletons { get; private set; } = new Dictionary<Type, object>();

        internal static void Scan()
        {
            if (!IsInitialized)
            {
                AttributeRead();
                IsInitialized = !IsInitialized;
            }
        }
        private static void AttributeRead()
        {
            var result = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Select(t => new
                {
                    Type = t,
                    Attribute = t.GetCustomAttribute(typeof(Navigable), true) as Navigable
                })
                .Where(x => x.Attribute != null)
                .ToArray();
            foreach (var item in result)
            {
                if (NavigableAttributes.ContainsKey(item.Type)) continue;

                NavigableAttributes.Add(item.Type, item.Attribute);
            }
        }
        public static object? GetInstance(Type pageType, params object?[]? value)
        {
            Scan();
            if (NavigableAttributes.TryGetValue(pageType, out var result))
            {
                switch (result.Mode)
                {
                    case ConstructionModes.Singleton:
                        if (Singletons.TryGetValue(pageType, out var page))
                        {
                            return page;
                        }
                        else
                        {
                            var newPage = Activator.CreateInstance(pageType, value);
                            if (newPage != null)
                            {
                                Singletons.Add(pageType, newPage);
                            }
                            return newPage;
                        }
                    case ConstructionModes.Refresh:
                        return Activator.CreateInstance(pageType, value);
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
