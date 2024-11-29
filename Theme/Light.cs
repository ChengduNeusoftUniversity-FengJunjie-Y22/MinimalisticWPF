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
        public Light(LightBrushPackage brushenum)
        {
            BrushPackage = brushenum;
            ispackagebrush = true;
        }

        private bool ispackagebrush = false;
        public object?[]? Parameters { get; set; }
        public LightBrushPackage BrushPackage { get; set; }

        public object? Value => ispackagebrush ? LightBrushes.Select(BrushPackage) : null;
    }
}
