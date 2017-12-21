using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStart;


/// <summary>
/// Map 注释
/// <para>Author: zhaojun jun.zhao@ifreeteam.com</para>
/// <para>Date: 2014/5/26 17:16:18</para>
/// <para>$Id$</para>
/// </summary>

namespace AStar
{
    class Map
    {
        /// <summary>
        /// 地图的不可通过值，如果设为此值，则不可通过
        /// </summary>
        public const int MAPNODE_NO_PASSABLE_VALUE = int.MaxValue;
        /// <summary>
        /// 地图数据.
        /// </summary>
        private MapNode[,] _mapdata = null;
        /// <summary>
        /// 地图的行的宽度
        /// </summary>
        private int _width = 0;
        /// <summary>
        /// 地图的行数
        /// </summary>
        private int _height = 0;

        public int width
        {
            get
            {
                return _width;
            }
        }
        public int height
        {
            get
            {
                return _height;
            }
        }

        /// <summary>
        /// 初始化地图的长宽
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Map(int width, int height)
        {
            _mapdata = new MapNode[height,width];
            
            _width = width;
            _height = height;
            for (int h = 0; h < _height; h++)
            {
                for (int w = 0; w < _width; w++)
                {
                    _mapdata[h, w] = new MapNode();
                }
            }
        }

        /// <summary>
        /// 所有节点的weight。如果weight 为int。max则不可通过
        /// </summary>
        /// <param name="weight"></param>
        public void installData(int [,]datas)
        {
            for (int h = 0; h < _height; h++)
            {
                for (int w = 0; w < _width; w++)
                {
                    _mapdata[h, w].weight = datas[h, w];
                    _mapdata[h, w].x = w;
                    _mapdata[h, w].y = h;
                }
            }
        }

        /// <summary>
        /// 获取某个位置的节点是否可通过
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool isPassAble(int x, int y)
        {
            return _mapdata[y,x].isPassAble();
        }

        #region runtimeVars
        private IOpenList<MapNode> _open = null;
        private List<MapNode> _closed = null;
        private MapNode _current = null;
        #endregion

        #region forVisualDebug
        public IOpenList<MapNode> Open
        {
            get
            {
                return _open;
            }
        }
        public List<MapNode> Close
        {
            get
            {
                return _closed;
            }
        }
        #endregion

        private List<MapNode> findPath(MapNode start, MapNode end, bool eightDirection = false, bool useBinaryHeap = true)
        {
            if (useBinaryHeap)
            {
                _open = new BinaryHeapOpenList<MapNode>();
            }
            else
            {
                _open = new ListOpenList<MapNode>();
            }
            _closed = new List<MapNode>();
            List<MapNode> result = new List<MapNode>();
            start.parentNode = null;
            //设置或者不设置为0，问题不大。唯一的可能问题是寻路多次之后有可能溢出
            start.g = 0;
            start.h = 0;
            start.f = 0;
            //-----------------------
            if (end == null)
            {
                return result;
            }
            if (!end.isPassAble())
            {
                return result;
            }
            end.parentNode = null;
            _open.push(start);
            while ((_open.getCount() > 0) && (_closed.IndexOf(end) == -1))
            {
                //if (_open is ListOpenList<MapNode>)
                //{
                //    (_open as ListOpenList<MapNode>).printSelf();
                //}
                _current = _open.getSmallest();
                checkAround(_current, end, eightDirection);
                _closed.Add(_current);
            }
           
            MapNode c = end;
            while (c != null)
            {
                result.Insert(0, c);
                c = c.parentNode;
            }
            if (result.Count == 1)
            {
                result.Clear();
                result.Add(start);
            }
            return result;
        }

        //四方向数组
        private int[] _xd = new int[]{0,-1,0,1};
		private int[] _yd = new int[]{-1,0,1,0};
        //把方向数组
		private int[] _xde = new int[]{0,-1,-1,-1,0,1,1,1};
        private int[] _yde = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };

        private void checkAround(MapNode node, MapNode goalNode, bool eightDirection)
        {
            if (eightDirection)
            {
                for (int e = 7; e >= 0; e--)
                {
                    int _xe = node.x + _xde[e];
                    int _ye = node.y + _yde[e];
                    if ((_xe < 0) || (_ye < 0) || (_xe >= _width) || (_ye >= _height))
                    {
                        continue;
                    }
                    MapNode testNodee = _mapdata[_ye,_xe];
                    if (_closed.IndexOf(testNodee) != -1)
                    {
                        //已经在关闭列表
                        continue;
                    }
                    if (!testNodee.isPassAble())
                    {
                        //如果不可通过
                        _closed.Add(testNodee);
                        testNodee.parentNode = node;
                        continue;
                    }
                    if (_open.indexOf(testNodee) != -1)
                    {
                        //已经在开放列表，判断f的值是不是更好
                        if ((node.g + Mathf.Sqrt(_xde[e] * _xde[e] + _yde[e] * _yde[e])) < testNodee.g)
                        {
                            testNodee.g = node.g +/* testNodee.weight;*/ Mathf.Sqrt(_xde[e] * _xde[e] + _yde[e] * _yde[e])*testNodee.weight;
                            testNodee.parentNode = node;
                            testNodee.f = testNodee.g + testNodee.h;
                            ////此处是否应该在二项堆里重新排序.
                            _open.rePostion(testNodee);
                        }
                        continue;
                    }
                    else
                    {
                        testNodee.g = node.g + /*testNodee.weight;*/Mathf.Sqrt(_xde[e] * _xde[e] + _yde[e] * _yde[e])*testNodee.weight;
                        testNodee.h = Mathf.Abs(goalNode.x - testNodee.x) + Mathf.Abs(goalNode.y - testNodee.y);
                        testNodee.f = testNodee.g + testNodee.h;
                        testNodee.parentNode = node;
                        _open.push(testNodee);
                        continue;
                    }
                }
            }
            else
            {
                for (int i = 3; i >= 0; i--)
                {
                    int _x = node.x+_xd[i];
					int _y = node.y+_yd[i];
					if((_x<0)||(_y<0)||(_x>=_width)||(_y>=_height))
					{
						continue;
					}
					MapNode testNode = _mapdata[_y,_x];
					if(_closed.IndexOf(testNode) != -1)
					{
                        //已在关闭列表，不处理
						continue;
					}
					if(!testNode.isPassAble())
					{
                        //不可通过，加入关闭列表
						_closed.Add(testNode);
						testNode.parentNode = node;
						continue;
					}
					if(_open.indexOf(testNode) != -1)
					{
						//已经在开放列表，判断新的g值是不是更好
						if((node.g+testNode.weight)<testNode.g)
						{
							testNode.g = node.g+testNode.weight;
                            testNode.f = testNode.g + testNode.h;
                            ////此处是否应该在二项堆里重新排序.
                            _open.rePostion(testNode);
							testNode.parentNode = node;
						}
						continue;
					}else
					{
                        //加入开放列表
                        testNode.g = node.g + testNode.weight;
                        testNode.h = Mathf.Abs(goalNode.x - testNode.x) + Mathf.Abs(goalNode.y - testNode.y);
						testNode.f = testNode.g+testNode.h;
						testNode.parentNode = node;
						_open.push(testNode);
						continue;
					}
                }
            }
        }

        /// <summary>
        /// 根据节点的x,y和其他参数，返回一条路径
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="eightDirection"></param>
        /// <param name="useBinaryHeap"></param>
        /// <returns></returns>
        public List<MapNode> findPathByXY(int startX,int startY,int endX,int endY,bool eightDirection = false,bool useBinaryHeap = true)
		{
            //Debug.Log(startX+" " +startY+" "+ endX+" "+endY);
			List<MapNode> result = new List<MapNode>();
			if(startX == endX && startY == endY)
			{
                result.Add(_mapdata[startY,startX]);
				return result;
			}
			
			
			result = this.findPath(_mapdata[startY,startX],_mapdata[endY,endX],eightDirection,useBinaryHeap);
			if(result.Count < 2)
			{
				//找不到路
			}
			
			return result;
        }

        #region noInspire
        /// <summary>
        /// 对比测试用的非启发算法，不要使用
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="eightDirection"></param>
        /// <param name="useBinaryHeap"></param>
        /// <returns></returns>
        public List<MapNode> findPathByXYNoInspire(int startX, int startY, int endX, int endY, bool eightDirection = false)
        {
            //Debug.Log(startX + " " + startY + " " + endX + " " + endY);
            List<MapNode> result = new List<MapNode>();
            if (startX == endX && startY == endY)
            {
                result.Add(_mapdata[startY, startX]);
                return result;
            }
            MapNode start = _mapdata[startY, startX];
            MapNode end = _mapdata[endY, endX];

            _open = new ListOpenList<MapNode>() ;
            _open.push(start);
            start.parentNode = null;
            start.g = start.h = start.f = 0;
            end.parentNode = null;
            _closed = new List<MapNode>();
            while (_open.getCount() > 0 && (_closed.IndexOf(end) == -1))
            {

                //不去是用启发算法去找一个可能最优的,而是随便找一个来计算
                MapNode mn = (_open as ListOpenList<MapNode>).pop();
                //检查周围
                if (!eightDirection)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int _x = mn.x + _xd[i];
                        int _y = mn.y + _yd[i];
                        if (_x < 0 || _y < 0 || (_x >= _width) || (_y >= _height))
                        {
                            continue;
                        }
                        MapNode testN = _mapdata[_y,_x];
                        if(_closed.IndexOf(testN) != -1)
                        {
                            continue;
                        }
                        if (!testN.isPassAble())
                        {
                            _closed.Add(testN);
                            testN.parentNode = mn;
                            continue;
                        }
                        if (_open.indexOf(testN) != -1)
                        {
                            if (testN.g > mn.g + testN.weight)
                            {
                                testN.g = mn.g + testN.weight;
                                testN.parentNode = mn;
                            }
                            continue;
                        }
                        else
                        {
                            _open.push(testN);
                            testN.parentNode = mn;
                            testN.g = mn.g + testN.weight;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        int _x = mn.x + _xde[i];
                        int _y = mn.y + _yde[i];
                        if (_x < 0 || _y < 0 || (_x >= _width) || (_y >= _height))
                        {
                            continue;
                        }
                         MapNode testN = _mapdata[_y,_x];
                        if(_closed.IndexOf(testN) != -1)
                        {
                            continue;
                        }
                        if (!testN.isPassAble())
                        {
                            _closed.Add(testN);
                            testN.parentNode = mn;
                            continue;
                        }
                        if (_open.indexOf(testN) != -1)
                        {
                            if (testN.g > mn.g + testN.weight)
                            {
                                testN.g = mn.g + testN.weight;
                                testN.parentNode = mn;
                            }
                            continue;
                        }
                        else
                        {
                            _open.push(testN);
                            testN.parentNode = mn;
                            testN.g = mn.g + testN.weight;
                        }
                    }
                }
                //加入closed
                _closed.Add(mn);
            }

            MapNode c = end;
            while (c != null)
            {
                result.Insert(0, c);
                c = c.parentNode;
            }
            if (result.Count == 1)
            {
                result.Clear();
                result.Add(start);
            }

            if (result.Count < 2)
            {
                //找不到路
            }

            return result;
        }
        #endregion
    }
}
