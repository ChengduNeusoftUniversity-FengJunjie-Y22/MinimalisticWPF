using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MinimalisticWPF
{
    public interface IExecutableTransition
    {
        public void Start();
        public void Stop(bool IsUnsafeStoped = false);
    }
}
