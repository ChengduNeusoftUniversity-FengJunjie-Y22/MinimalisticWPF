using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    internal class TransitionMeta : ITransitionMeta, IMergeableTransition
    {
        internal TransitionMeta(TransitionParams transitionParams, List<List<Tuple<PropertyInfo, List<object?>>>> tuples)
        {
            TransitionParams = transitionParams;
            FrameSequence = tuples;
        }
        public TransitionParams TransitionParams { get; private set; }
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; private set; }
        public TransitionMeta? Parse(object target)
        {
            if (target is ITransitionMeta meta)
            {
                return new TransitionMeta(meta.TransitionParams, meta.FrameSequence);
            }
            return null;
        }
        public void Merge(params ITransitionMeta[] meta)
        {
            ICollection<ITransitionMeta> original = [this, .. meta];
            var fps = original.Max(a => a.TransitionParams.FrameRate);
            var priority = original.Max(a => a.TransitionParams.UIPriority);
            var isBegin = original.Any(a => a.TransitionParams.IsBeginInvoke);
        }
    }
}
