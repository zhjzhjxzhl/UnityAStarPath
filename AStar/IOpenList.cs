using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStart;


/// <summary>
/// IOpenList Astart中openlist 的抽象类，为了方便使用二叉堆替换数组
/// <para>Author: zhaojun jun.zhao@ifreeteam.com</para>
/// <para>Date: 2014/5/26 17:17:56</para>
/// <para>$Id$</para>
/// </summary>

namespace AStar
{
    interface IOpenList<T> :IEnumerable<T>,IEnumerator<T> where T : MapNode
    {
        /// <summary>
        /// 添加一个元素
        /// </summary>
        /// <param name="t"></param>
        void push(T t);
        /// <summary>
        /// 找到最小的，将其移除并返回
        /// </summary>
        /// <returns></returns>
        T getSmallest();
        /// <summary>
        /// 获取当前元素的个数
        /// </summary>
        /// <returns></returns>
        int getCount();
        /// <summary>
        /// 获取t的索引
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int indexOf(T t);

        /// <summary>
        /// 将二项堆中的t的位置重新设定
        /// </summary>
        /// <param name="t"></param>
        void rePostion(T t);
    }
}
