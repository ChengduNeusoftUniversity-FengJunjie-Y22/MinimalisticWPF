using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class DynamicTheme
    {
        /// <summary>
        /// [ 全局 ] 应用主题 , 需要 object.AsGlobalTheme() 激活对象以在全局生效
        /// </summary>
        /// <param name="attributeType">主题特性类型</param>
        /// <param name="paramAction">过渡参数构造</param>
        /// <param name="windowBack">主窗口背景色</param>
        public static void GlobalApply(Type attributeType, Action<TransitionParams>? paramAction = null, Brush? windowBack = default)
        {
            foreach (var item in ExtensionForDynamicTheme.InstanceHosts)
            {
                item.ApplyTheme(attributeType, paramAction);
            }
            Application.Current.MainWindow.Transition()
                .SetProperty(x => x.Background, windowBack ?? Application.Current.MainWindow.Background)
                .SetParams(paramAction ?? TransitionParams.Theme)
                .Start();
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
