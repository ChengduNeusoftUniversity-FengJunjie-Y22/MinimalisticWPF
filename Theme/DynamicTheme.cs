using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public static class DynamicTheme
    {
        /// <summary>
        /// [ 全局 ] 应用主题 , 需要 object.AwakeDynamicTheme() 激活对象以在全局生效
        /// </summary>
        /// <param name="attributeType">主题特性类型</param>
        /// <param name="paramAction">过渡参数构造</param>
        public static void GlobalApply(Type attributeType, Action<TransitionParams>? paramAction = null)
        {
            foreach (var item in ExtensionForDynamicTheme.InstanceHosts)
            {
                item.ApplyTheme(attributeType, paramAction);
            }
        }

        /// <summary>
        /// [ 局部 ] 应用主题 , 无需激活
        /// </summary>
        /// <param name="attributeType">主题特性类型</param>
        /// <param name="paramAction">过渡参数构造</param>
        /// <param name="targets">目标实例</param>
        public static void PartialApply(Type attributeType, Action<TransitionParams>? paramAction = null, params object[] targets)
        {
            foreach (var item in targets)
            {
                item.ApplyTheme(attributeType, paramAction);
            }
        }
    }
}
