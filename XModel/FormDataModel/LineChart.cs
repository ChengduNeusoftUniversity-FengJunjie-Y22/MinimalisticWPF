using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    /// <summary>
    /// 折线图结构
    /// </summary>
    public class LineChart : FormBase
    {
        public LineChart() { }

        /// <summary>
        /// 折线绘制区域的宽度
        /// </summary>
        public double Width { get; set; } = 0;

        /// <summary>
        /// 折线绘制区域呃高度
        /// </summary>
        public double Height { get; set; } = 0;

        /// <summary>
        /// 数据组
        /// </summary>
        public List<LineChartXElement> DataGroup { get; set; } = new List<LineChartXElement>();

        /// <summary>
        /// 实际折线坐标点组
        /// </summary>
        public List<PointCollection> PointGroups { get; set; } = new List<PointCollection>();

        /// <summary>
        /// 数据是否可用
        /// </summary>
        public bool IsEnable
        {
            get => DataGroup.Count <= AxisTicks_X ? true : false;
        }

        /// <summary>
        /// 重新排列DataGroup中每个LineChartXElement在X轴上的索引
        /// </summary>
        public void UpdateDataIndex()
        {
            for (int i = 0; i < DataGroup.Count; i++)
            {
                DataGroup[i].XIndex = i;
            }
        }

        /// <summary>
        /// 更新折线坐标点组
        /// </summary>
        /// <param name="Root">折线图所依附的画板</param>
        public void UpdatePointBySize(FrameworkElement Root)
        {
            Width = Root.ActualWidth;
            Height = Root.ActualHeight;

            UpdateDataIndex();

            PointGroups.Clear();

            foreach (LineChartXElement item in DataGroup)
            {
                item.UpdatePointBySize(this);
            }

            for (int i = 0; i < FindMaxCount(); i++)
            {
                PointCollection points = new PointCollection();
                foreach (LineChartXElement item in DataGroup)
                {
                    if (item.Value.Count > i)
                    {
                        points.Add(item.Point[i]);
                    }
                }
                PointGroups.Add(points);
            }
        }

        /// <summary>
        /// 寻找X轴节点数据组中，最大数据组的元素数
        /// </summary>
        public int FindMaxCount()
        {
            int result = 0;

            foreach (LineChartXElement item in DataGroup)
            {
                if (item.Value.Count > result)
                {
                    result = item.Value.Count;
                }
            }

            return result;
        }
    }
}
