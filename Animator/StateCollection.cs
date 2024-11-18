using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MinimalisticWPF
{
    /// <summary>
    /// 状态的合集
    /// <para>特性 :</para>
    /// <para>1.可将StateName作为索引,查询集合中指定的State</para>
    /// <para>2.Add()方法会替换同名State以确保唯一性</para>
    /// </summary>
    public class StateCollection : ICollection<State>
    {
        private List<State> _nodes = new List<State>();

        public State this[int index] { get => _nodes[index < _nodes.Count && index > -1 ? index : throw new ArgumentOutOfRangeException($"Index value [ {index} ] is out of collection range")]; }

        /// <summary>
        /// 通过状态名称访问状态信息
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>State 状态信息</returns>
        /// <exception cref="ArgumentException"></exception>
        public State this[string stateName]
        {
            get
            {
                State? result = _nodes.FirstOrDefault(x => x.StateName == stateName);

                if (result == null)
                {
                    throw new ArgumentException($"There is no State named [ {stateName} ] in the collection");
                }

                return result;
            }
        }

        public int Count => _nodes.Count;

        private int _suffix = 0;
        public int BoardSuffix
        {
            get
            {
                if (_suffix < 50)
                {
                    _suffix++;
                    return _suffix;
                }
                if (_suffix == 50)
                {
                    _suffix = 0;
                    return _suffix;
                }
                return -1;
            }
        }

        public bool IsReadOnly => false;

        public void Add(State item)
        {
            var existingNode = _nodes.FirstOrDefault(x => x.StateName == item.StateName);

            if (existingNode != null)
            {
                if (!ReferenceEquals(existingNode, item))
                {
                    _nodes.Remove(existingNode);
                    _nodes.Add(item);
                }
            }
            else
            {
                _nodes.Add(item);
            }
        }
        public void Clear()
        {
            _nodes.Clear();
        }
        public bool Contains(State item)
        {
            return _nodes.Contains(item);
        }
        public void CopyTo(State[] array, int arrayIndex)
        {
            _nodes.CopyTo(array, arrayIndex);
        }
        public bool Remove(State item)
        {
            return _nodes.Remove(item);
        }
        public IEnumerator<State> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
    }
}
