using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class Light : Attribute, IThemeAttribute
    {
        public Light(params object?[] param)
        {
            Parameters = param;
        }
        public Light(BrushTags noselected)
        {
            BrushPackage = noselected;
            ispackagebrush = true;
        }
        public Light(BrushTags noselected, BrushTags selected)
        {
            BrushPackage = noselected;
            HoverBrushPackage = selected;
            ispackagebrush = true;
            ishoverpackagebrush = true;
        }

        private bool ispackagebrush = false;
        private bool ishoverpackagebrush = false;

        internal BrushTags BrushPackage { get; set; }
        internal BrushTags HoverBrushPackage { get; set; }

        public object?[]? Parameters { get; set; }
        public object? Value => ispackagebrush ? LightBrushes.Selector.Select(BrushPackage) : null;
        public object? FocusValue => ishoverpackagebrush ? LightBrushes.Selector.Select(HoverBrushPackage) : null;
    }
}
