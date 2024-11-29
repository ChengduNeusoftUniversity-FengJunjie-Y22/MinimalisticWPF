using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public interface IThemeAttribute
    {
        object?[]? Parameters { get; set; }
        object? Value { get; }
    }
}
