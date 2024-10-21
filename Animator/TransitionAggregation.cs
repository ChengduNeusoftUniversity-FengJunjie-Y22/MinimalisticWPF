using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// 聚合动画解释器
    /// <para>[聚合]使得您可在一条时间线上组合多个过渡效果,这使得多个动画效果无需创建多个Task</para>
    /// </summary>
    public class TransitionAggregation
    {
        public TransitionAggregation() { }

        public List<Tuple<int, List<Tuple<object, List<Tuple<PropertyInfo, List<object?>>>>>, int>> Data { get; internal set; } = new List<Tuple<int, List<Tuple<object, List<Tuple<PropertyInfo, List<object?>>>>>, int>>();
    }
}
