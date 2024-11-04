using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public enum Themes
    {
        Dark,
        Light
    }

    public static class Theme
    {
        public static List<ThemeControlBase> ThemeControlableInstances { get; internal set; } = new List<ThemeControlBase>();

        private static Themes _now = Themes.Dark;
        public static Themes Now
        {
            set
            {
                if (value == _now) return;
                _now = value;

                switch (value)
                {
                    case Themes.Dark:
                        ToDark();
                        break;
                    case Themes.Light:
                        ToLight();
                        break;
                }
            }
        }

        internal static void ToDark()
        {

        }

        internal static void ToLight()
        {

        }
    }
}
