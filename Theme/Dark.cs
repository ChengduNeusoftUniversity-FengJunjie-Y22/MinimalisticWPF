using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Dark : Attribute, IThemeAttribute
    {
        public Dark(params object?[] param)
        {
            Parameters = param;
        }
        public Dark(DarkBrushPackage brushenum)
        {
            BrushPackage = brushenum;
            ispackagebrush = true;
        }

        private bool ispackagebrush = false;
        public object?[]? Parameters { get; set; }
        public DarkBrushPackage BrushPackage { get; set; }

        public object? Value => ispackagebrush ? DarkBrushes.Select(BrushPackage) : null;
    }
}
