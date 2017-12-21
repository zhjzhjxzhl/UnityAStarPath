using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using AStart;


/// <summary>
/// BinaryHeapOpenList  二叉堆，用来优化寻路的。在格子较多(>50*50)，可通过区域较大(>85%)的情况下，提升较为明显，否者普通数组即可，无需二叉树寻路。
/// 构建一个以f为比较的二叉堆，如果前一个(父节点)比现在的f大，则现在的和前一个交换位置
/// <para>Author: zhaojun jun.zhao@ifreeteam.com</para>
/// <para>Date: 2014/5/26 17:19:01</para>
/// <para>$Id$</para>
/// </summary>

namespace AStar
{
    class BinaryHeapOpenList<T> : IOpenList<T> where T : MapNode,new()
    {
        private List<T> _data = new List<T>();

        public BinaryHeapOpenList()
        {
            //先加入一项,因为用数组维护二叉堆的时候，索引为0的元素，是不会用到的。因为0*2 == 0.不容易计算，如果使用0会带来计算上的复杂度
            _data.Add(new T());
        }
        /// <summary>
        /// 加入一个元素。将元素加入到数组末尾。然后依次和 位置/2   位置的元素对比，依次交换,直到停止
        /// 算法复杂度 log(2,n)
        /// </summary>
        /// <param name="t"></param>
        public void push(T t)
        {
            _data.Add(t);
            T temp;
            int length = _data.Count - 1;
            //元素当前位置
            int j = length;
            //要比较的位置
            int i = (int)(j / 2);
            while (i > 0)
            {
                //此处的等于号参考list的等于号
                if (_data[i].f >= _data[j].f)
                {
                    //如果前一个比现在的大，则前移
                    temp = _data[i];
                    _data[i] = _data[j];
                    _data[j] = temp;
                    j = i;
                    i = (int)(j / 2);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 1、先将第一个最小的，取出。然后将最后一个元素（不一定是最大的，但是一定比第一个大)，放置到最前面。
        /// 依次比较自己和child。如果child比自己小，则和child交换。
        /// 如果两个child都比自己大，则完成退出。
        /// </summary>
        /// <returns></returns>
        public T getSmallest()
        {
            //第一个保存下来，用作返回
            T smallest = _data[1];
            //重新维护这个列表，将最后一个移动到第一位
            _data[1] = _data[_data.Count - 1];
            _data.RemoveAt(_data.Count - 1);

            T temp;
            int length = _data.Count;
            int i = 1;
            int j = i * 2;

            while (j < length)
            {
                if (j + 1 < length)
                {
                    //两个子节点都存在，优先和较小的换位
                    if (_data[j].f < _data[j + 1].f)
                    {
                        //由于是从最后拿过来的项，所以尽量让其向后
                        if (_data[i].f >= _data[j].f)
                        {
                            temp = _data[j];
                            _data[j] = _data[i];
                            _data[i] = temp;
                            i = j;
                            j = i * 2;
                        }
                        else if (_data[i].f >= _data[j + 1].f)
                        {
                            temp = _data[j + 1];
                            _data[j + 1] = _data[i];
                            _data[i] = temp;
                            i = j + 1;
                            j = i * 2;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_data[i].f >= _data[j + 1].f)
                        {
                            temp = _data[j + 1];
                            _data[j + 1] = _data[i];
                            _data[i] = temp;
                            i = j + 1;
                            j = i * 2;
                        }
                        else if (_data[i].f >= _data[j].f)
                        {
                            temp = _data[j];
                            _data[j] = _data[i];
                            _data[i] = temp;
                            i = j;
                            j = i * 2;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (_data[i].f >= _data[j].f)
                    {
                        temp = _data[j];
                        _data[j] = _data[i];
                        _data[i] = temp;
                    }
                    break;
                }
            }
            return smallest;
        }

        public int getCount()
        {
            return _data.Count -1 ;
        }

        public int indexOf(T t)
        {
            return _data.IndexOf(t);
        }

        public void rePostion(T t)
        {
            int index = _data.IndexOf(t);
            int pindex = (int)(index / 2);
            T temp;
            while (pindex > 0)
            {
                if (_data[pindex].f >= _data[index].f)
                {
                    temp = _data[pindex];
                    _data[pindex] = _data[index];
                    _data[index] = temp;
                    index = pindex;
                    pindex = (int)(index / 2);
                }
                else
                {
                    break;
                }
            }
            return;
        }

        //---------------------------------------------------------------------------

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)this;
        }

        private int _index =-1;
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
