using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public interface ITransitionMeta
    {
        public TransitionParams TransitionParams { get; }
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; }
    }
}
