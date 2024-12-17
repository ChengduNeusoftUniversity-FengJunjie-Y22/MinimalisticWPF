using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public interface IMergeableTransition
    {
        /// <summary>
        /// Merge transitions according to the built-in priorities and algorithms
        /// </summary>
        public void Merge(params ITransitionMeta[] meta);
    }
}
