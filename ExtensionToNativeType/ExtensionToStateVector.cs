using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    public static class ExtensionToStateVector
    {
        /// <summary>
        /// 名称
        /// </summary>
        public static TempStateVector SetName(this TempStateVector source, string stateVectorName)
        {
            source.Value.Name = stateVectorName;
            return source;
        }

        /// <summary>
        /// 条件判断式
        /// </summary>
        public static TempStateVector SetConditions<T>(this TempStateVector source, Expression<Func<T, bool>> condition) where T : class
        {
            var compiledCondition = condition.Compile();

            Func<dynamic, bool> dynamicCondition = item =>
            {
                if (item is T typedItem)
                {
                    return compiledCondition(typedItem);
                }
                return false;
            };

            source.Value.Condition = dynamicCondition;

            return source;
        }

        /// <summary>
        /// 过渡效果参数
        /// </summary>
        public static TempStateVector SetTransferParams(this TempStateVector source, double transitionTime = 0, bool isQueue = false, bool isLast = true, bool isUnique = false, int? frameRate = default, double waitTime = 0.008, ICollection<string>? protectNames = default)
        {
            TransferParams tempdata = new TransferParams(transitionTime, isQueue, isLast, isUnique, frameRate, waitTime, protectNames);
            source.Value.TransferParams = tempdata;
            return source;
        }

        /// <summary>
        /// 输出StateVector
        /// </summary>
        public static StateVector ToStateVector(this TempStateVector source)
        {
            return source.Value;
        }
    }
}
