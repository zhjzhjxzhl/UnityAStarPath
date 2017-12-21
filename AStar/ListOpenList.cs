using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using AStart;


/// <summary>
/// ListOpenList 
/// <para>Author: zhaojun jun.zhao@ifreeteam.com</para>
/// <para>Date: 2014/5/26 17:18:14</para>
/// <para>$Id$</para>
/// </summary>

namespace AStar
{
    class ListOpenList<T> : IOpenList<T> where T : MapNode
    {
        private List<T> _data = new List<T>();

        public void push(T t)
        {
            _data.Add(t);
        }
        /// <summary>
        /// 打印自己
        /// </summary>
        public void printSelf()
        {
            string tt = "";
            foreach (T _t in _data)
            {
                tt += _t.f+"("+_t.x+","+_t.y+")" + ",";
            }
            Debug.Log(tt);
        }

        public T getSmallest()
        {
            if (_data.Count == 0)
            {
                return null;
            }
            T smallest = _data[0];
            int smallestIndex = 0;
            for (int i = 1; i < _data.Count; i++)
            {
                //这个地方的比较，如果使用大于等于，在效率上会比使用大于较优化一点
                //如果使用等号，则尽量取后加入openlist的点，这些点在f值相同的情况下，会比之前加入的优。
                //这是由于A Star的启发性，后面的点将消耗更少次的计算。
                //此处的优化，在100X100的随机地图上，带来的效率优化甚至有十几倍，甚至超越Binary Heap的优化。
                //这个等号，将会极大的降低openlist的大小。因为后加入的点周围的点，可能更多的是closed的。
                if (smallest.f >= _data[i].f)
                {
                    smallest = _data[i];
                    smallestIndex = i;
                }
            }
            _data.RemoveAt(smallestIndex);
            return smallest;
        }

        public int getCount()
        {
            return _data.Count;
        }

        public int indexOf(T t)
        {
            return _data.IndexOf(t);
        }

        public void rePostion(T t)
        {
            //数组查找，每次都是线性遍历，不需要做这个事情
            return;
        }

        // -----------------------------------------一下是测试对比用的-------------------------//
        //取出最后一个，用在非启发算法上
        public T pop()
        {
            if (_data.Count > 0)
            {
                T t = _data[0];
                _data.RemoveAt(0);
                return t;
            }
            return null;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)this;
        }

        private int _index = -1;
        public T Current
        {
            get 
            {
                return _data[_index];
            }
        }

        public bool MoveNext()
        {
            _index++; 
            if (_index < _data.Count)
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _index = -1;
        }

        public void Dispose()
        {
           
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)this;
        }
        object System.Collections.IEnumerator.Current
        {
            get { return _data[_index]; }
        }
    }
}
