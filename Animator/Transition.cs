using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public static class Transition
    {
        private static string _tempName = "temp";
        /// <summary>
        /// TransitionBoard中,State的临时名称
        /// </summary>
        public static string TempName
        {
            get => _tempName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _tempName = value;
                }
            }
        }
        /// <summary>
        /// 创建动画板,基于 [ 实例 ],占用更多资源
        /// </summary>
        public static TransitionBoard<T> CreateBoardFromObject<T>(T target) where T : class
        {
            var result = new TransitionBoard<T>(target);
            result.IsStatic = true;
            return result;
        }
        /// <summary>
        /// 创建动画板,基于 [ 类型 ],占用少量资源
        /// </summary>
        public static TransitionBoard<T> CreateBoardFromType<T>() where T : class
        {
            return new TransitionBoard<T>() { IsStatic = true };
        }
    }
}
