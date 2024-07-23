using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace MinimalisticWPF
{
    /// <summary>
    /// 直方图中，用于存储节点数据元素的结构
    /// </summary>
    public class HistogramXElement
    {
        public HistogramXElement() { }

        /// <summary>
        /// 节点名称，唯一
        /// </summary>
        public string NodeName { get; set; } = string.Empty;

        /// <summary>
        /// 节点值,允许多个值元素
        /// </summary>
        public List<double> Value { get; set; } = new List<double>();
    }
}
