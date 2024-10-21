using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public interface ITransitionBoard
    {
        Action<TransitionParams>? TransitionParams { get; set; }
        List<List<Tuple<PropertyInfo, List<object?>>>>? Preload { get; set; }
    }
}
