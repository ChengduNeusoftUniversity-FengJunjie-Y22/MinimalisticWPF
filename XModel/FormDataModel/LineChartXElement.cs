using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MinimalisticWPF
{
    /// <summary>
    /// 折线图中，用于存放节点元素的数据结构
    /// </summary>
    public class LineChartXElement
    {
        public LineChartXElement() { }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; } = string.Empty;

        /// <summary>
        /// 节点在X轴上的索引，指定坐标原点为0
        /// </summary>
        public int XIndex { get; set; } = 0;

        /// <summary>
        /// 节点值，允许多个值对象
        /// </summary>
        public List<double> Value { get; set; } = new List<double>();

        /// <summary>
        /// 实际折点
        /// </summary>
        public List<System.Windows.Point> Point { get; set; } = new List<System.Windows.Point>();

        /// <summary>
        /// 底部点
        /// </summary>
        public List<System.Windows.Point> Bottom { get; set; } = new List<System.Windows.Point>();

        /// <summary>
        /// 更新折线坐标点组
        /// </summary>
        /// <param name="Root">折线节点元素所依附的折线图</param>
        public void UpdatePointBySize(LineChart Root)
        {
            double Rate_Y = Root.Height / Root.Maximum_Y;
            double Rate_X = Root.Width / (Root.AxisTicks_X - 1);

            foreach (double value in Value)
            {
                Point.Add(new System.Windows.Point(XIndex * Rate_X, Root.Height - value * Rate_Y));
                Bottom.Add(new System.Windows.Point(XIndex * Rate_X, Root.Height));
            }
        }
    }
}
