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
        /// 获取当前类型在对象池中的信号量
        /// </summary>
        /// <param name="source"></param>
        /// <returns>
        /// -1 代表此类型不受Pool管理
        /// </returns>
        public static int GetPoolSemaphore(this Type source)
        {
            if (Pool.Source.TryGetValue(source, out var pool))
            {
                return pool?.Count ?? 0;
            }
            return -1;
        }
    }
}
