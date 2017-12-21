using System.Collections;

/// <summary>
/// MapNode 保存地图节点的基本信息
///     getWeight() 通过代价 在普通地图中都是1.通过设定为某个值，标示为不可通过.默认为int.max
///     x,y如果是方形地图。否则地图应提供一个遍历相邻节点的方法getAroundNext().
/// 保存寻路的时候的临时信息
///     g 起始节点到目前节点的消耗
///     h 当前节点到目标节点的预测消耗
///     f 当前节点的计算总消耗
///     
///     parentNode 当前节点的寻路父节点
/// <para>Author: zhaojun jun.zhao@ifreeteam.com</para>
/// <para>Date: 2014/5/26 16:59:30</para>
/// <para>$Id$</para>
/// </summary>
namespace AStart
{
    public class MapNode
    {
        private int _weight = 1;
        /// <summary>
        /// 通过的代价，默认是1
        /// </summary>
        public int weight
        {
            set
            {
                _weight = value;
            }
            get
            {
                return _weight;
            }
        }
        /// <summary>
        /// 当前节点在地图上的X.列索引
        /// </summary>
        public int x;
        /// <summary>
        /// 当前节点在地图上的y值。行索引
        /// </summary>
        public int y;
        #region runtimeVar
        /// <summary>
        /// 起始节点到当前节点的消耗
        /// </summary>
        public float g = 0;
        /// <summary>
        /// 当前节点到目标节点的预测消耗
        /// </summary>
        public float h = 0;
        /// <summary>
        /// 当前节点的总消耗
        /// </summary>
        public float f = 0;
        /// <summary>
        /// 当前节点寻路中的父节点
        /// </summary>
        public MapNode parentNode = null;
        #endregion

        /// <summary>
        /// 当前节点是否可以通过
        /// </summary>
        /// <returns></returns>
        public virtual bool isPassAble()
        {
            return (_weight != int.MaxValue);
        }

    }
}
