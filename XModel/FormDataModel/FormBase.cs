using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// 表单结构的基类
    /// </summary>
    public abstract class FormBase : ISerializableObject
    {
        public SerializeModes SerializeMode { get; set; } = SerializeModes.Xml;
        public string AbsolutePath { get; set; } = "Default";

        /// <summary>
        /// X轴单位
        /// </summary>
        public string Unit_X { get; set; } = string.Empty;
        /// <summary>
        /// Y轴单位
        /// </summary>
        public string Unit_Y { get; set; } = string.Empty;

        /// <summary>
        /// X轴标题
        /// </summary>
        public string Title_X { get; set; } = string.Empty;
        /// <summary>
        /// Y轴标题
        /// </summary>
        public string Title_Y { get; set; } = string.Empty;

        /// <summary>
        /// X轴最小数
        /// </summary>
        public double Minimum_X { get; set; } = 0;
        /// <summary>
        /// Y轴最小数
        /// </summary>
        public double Minimum_Y { get; set; } = 0;

        /// <summary>
        /// X轴最大值
        /// </summary>
        public double Maximum_X { get; set; } = 0;
        /// <summary>
        /// Y轴最大值
        /// </summary>
        public double Maximum_Y { get; set; } = 0;

        /// <summary>
        /// X轴刻度线数量
        /// </summary>
        public int AxisTicks_X { get; set; } = 2;
        /// <summary>
        /// Y轴刻度线数量
        /// </summary>
        public int AxisTicks_Y { get; set; } = 2;
    }
}
