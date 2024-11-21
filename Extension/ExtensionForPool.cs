using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public static class ExtensionForPool
    {
        /// <summary>
        /// 获取当前类型在对象池中的总资源数
        /// </summary>
        /// <param name="source"></param>
        /// <returns>
        /// int 小于0表示对象池未能正确加载此类型
        /// </returns>
        public static int GetPoolSemaphore(this Type source)
        {
            Pool.Awake();
            return (Pool.FetchQueue.TryGetValue(source, out var pool) ? pool.Count : -1) + (Pool.DisposeQueue.TryGetValue(source, out var disc) ? disc.Count : -1);
        }
    }
}
