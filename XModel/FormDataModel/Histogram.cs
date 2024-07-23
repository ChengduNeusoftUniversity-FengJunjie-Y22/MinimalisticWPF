using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// 直方图结构
    /// </summary>
    public class Histogram : FormBase
    {
        public Histogram() { }

        /// <summary>
        /// 在X轴上的每个节点处，用一个数据组来表示该节点的全部信息
        /// </summary>
        public List<HistogramXElement> DataGroup { get; set; } = new List<HistogramXElement>();

        /// <summary>
        /// 反映直方图X轴元素数是否超出[X轴刻度数-2]
        /// </summary>
        public bool IsEnable
        {
            get => DataGroup.Count <= AxisTicks_X - 2 ? true : false;
        }
    }
}
