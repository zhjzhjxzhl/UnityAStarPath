using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStart;
using System.Diagnostics;


/// <summary>
/// Terrain 注释
/// <para>Author: zhaojun jun.zhao@ifreeteam.com</para>
/// <para>Date: 2014/5/27 9:34:29</para>
/// <para>$Id$</para>
/// </summary>

namespace AStar.TestB
{
    class Terrain : MonoBehaviour
    {
        private int[,] _terrain = null;
        public int width;
        public int height;

        public GameObject start;
        public GameObject end;

        public Map map;
        /// <summary>
        /// 构建一个随机地图。
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="passabePre"></param>
        public void buildTerrain(int width,int height,float passabePre)
        {
            _terrain = new int[height,width];
            this.width = width;
            this.height = height;
            System.Random ran = new System.Random(1);
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    //随机地图
                    _terrain[h, w] = (ran.NextDouble() <= passabePre) ? 1 : int.MaxValue;
                    //构建特殊地图，专门用来测试方向性的
                    //_terrain[h, w] = (w==50&&h>10&&(h != 30) && (h != 60))?int.MaxValue:1;//
                    //构建无障碍地图
                    //_terrain[h, w] = 1;
                }
            }
            map = new Map(width, height);
            map.installData(_terrain);
        }

        public void OnDrawGizmos()
        {
            return;
            transform.DetachChildren();
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    if (_terrain[h, w] == int.MaxValue)
                    {
                        Gizmos.color = new Color(0, 0, 0);
                    }
                    else
                    {
                        Gizmos.color = new Color(1, 1, 1);
                    }
                    Gizmos.DrawCube(new Vector3(w + 0.5f, h + 0.5f, 1.0f), new Vector3(1,1,0.2f));
                }
            }
        }

        private UnityEngine.Object obj;
        public void showTerrain()
        {
            while (transform.childCount > 0)
            {
                Transform trans = transform.GetChild(transform.childCount - 1);
                trans.parent = null;
                Destroy(trans.gameObject);
            }
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    GameObject go = Instantiate(obj) as GameObject;
                    go.isStatic = true;
                    MeshRenderer mr = go.GetComponent<MeshRenderer>();
                    
                    if (_terrain[h, w] == int.MaxValue)
                    {
                        mr.material.color = new Color(0,0,0);
                    }
                    else
                    {
                        mr.material.color = new Color(1, 1, 1);
                    }
                    go.transform.parent = transform;
                    go.transform.localPosition = new Vector3(w + 0.5f, h + 0.5f, 1.0f);
                }
            }
        }

        void Start()
        {
            obj = Resources.Load("BaseTerrain");
        }

        public void buildMap()
        {
            int size = int.Parse(GameObject.Find("Lb_MapSize").GetComponent<UILabel>().text);
            float precent = float.Parse(GameObject.Find("Lb_passPre").GetComponent<UILabel>().text);

            buildTerrain(size, size, precent);
            showTerrain();
        }
        private bool back = false;
        public void findPathBack()
        {
            back = true;
            findPath();
            back = false;
        }
        public void findPath()
        {
            if (start != null && end != null)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                //int t1 = System.Environment.TickCount;
                //UnityEngine.Debug.Log("start t is "+t1);
                List<MapNode> path;
                if (_inspire)
                {
                    if (!back)
                    {

                        path = map.findPathByXY((int)start.transform.localPosition.x, (int)start.transform.localPosition.y,
                        (int)end.transform.localPosition.x, (int)end.transform.localPosition.y, _eightDirectory, _binaryHeap);
                    }
                    else
                    {
                        path = map.findPathByXY((int)end.transform.localPosition.x, (int)end.transform.localPosition.y,
                        (int)start.transform.localPosition.x, (int)start.transform.localPosition.y, _eightDirectory, _binaryHeap);
                    }
                }
                else
                {
                    if (!back)
                    {

                        path = map.findPathByXYNoInspire((int)start.transform.localPosition.x, (int)start.transform.localPosition.y,
                        (int)end.transform.localPosition.x, (int)end.transform.localPosition.y, _eightDirectory);
                    }
                    else
                    {
                        path = map.findPathByXYNoInspire((int)end.transform.localPosition.x, (int)end.transform.localPosition.y,
                        (int)start.transform.localPosition.x, (int)start.transform.localPosition.y, _eightDirectory);
                    }
                }
                //int del = System.Environment.TickCount - t1;
                //UnityEngine.Debug.Log("   cost tick :" + del);
                watch.Stop();
                GameObject.Find("Label_costTime").GetComponent<UILabel>().text = "time: " + watch.ElapsedMilliseconds + " ms";
                UnityEngine.Debug.Log("ms is :" + watch.ElapsedMilliseconds);
                
                if (path.Count > 1)
                {
                    GameObject pathLayer = GameObject.Find("PathLayer");
                    while (pathLayer.transform.childCount > 0)
                    {
                        Transform trans = pathLayer.transform.GetChild(0);
                        trans.parent = null;
                        Destroy(trans.gameObject);
                    }
                    foreach (MapNode mn in path)
                    {
                        GameObject go = Instantiate(obj) as GameObject;
                        MeshRenderer mr = go.GetComponent<MeshRenderer>();
                        mr.material.color = new Color(0, 0.5f, 0);
                        go.transform.parent = pathLayer.transform;
                        go.transform.localPosition = new Vector3(mn.x + 0.5f, mn.y + 0.5f, -2.0f);
                    }
                }
                showOpenClose();
            }
        }

        public void showOpenClose()
        {
            GameObject ocLayer = GameObject.Find("OpenClosed");
            while (ocLayer.transform.childCount > 0)
            {
                Transform trans = ocLayer.transform.GetChild(0);
                trans.parent = null;
                Destroy(trans.gameObject);
            }
            int i = 0;
            foreach (MapNode mn in map.Open)
            {
                GameObject go = Instantiate(obj) as GameObject;
                MeshRenderer mr = go.GetComponent<MeshRenderer>();
                mr.material.color = new Color(0.5f, 0.5f, 0);
                go.transform.parent = ocLayer.transform;
                go.transform.localPosition = new Vector3(mn.x + 0.5f, mn.y + 0.5f, -2.0f);
                i++;
            }
            UnityEngine.Debug.Log("open num "+i);
            foreach (MapNode mn in map.Close)
            {
                if (!mn.isPassAble())
                {
                    continue;
                }
                GameObject go = Instantiate(obj) as GameObject;
                MeshRenderer mr = go.GetComponent<MeshRenderer>();
                mr.material.color = new Color(0.5f, 0, 0);
                go.transform.parent = ocLayer.transform;
                go.transform.localPosition = new Vector3(mn.x + 0.5f, mn.y + 0.5f, -2.0f);
            }
        }

        private bool _eightDirectory = false;
        private bool _binaryHeap = false;
        private bool _inspire = false;

        public void OnCheckBoxChange()
        {
            GameObject go = GameObject.Find("CB_EightDirection");
            UIToggle tog = go.GetComponent<UIToggle>();
            _eightDirectory = tog.value;
            go = GameObject.Find("CB_BinaryHeap");
            tog = go.GetComponent<UIToggle>();
            _binaryHeap = tog.value;
            go = GameObject.Find("CB_Inspire");
            tog = go.GetComponent<UIToggle>();
            _inspire = tog.value;

        }
        
    }
}
