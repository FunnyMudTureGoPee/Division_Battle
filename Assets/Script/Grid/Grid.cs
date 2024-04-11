using System.Collections.Generic;
using UnityEngine;

namespace Script.Grid
{
    public class Grid
    {
        private int width;
        private int height;
        private float cellsize; //单元格大小
        private Vector3 originPosition;
        private int[,] gridArray;
        private string[,] gridStrings;
        private TextMesh[,] debugTextArray;
        private GameObject[,] battalions; //放置在格子上的对象


        private GameObject parent;

        public GameObject tip = Resources.Load<GameObject>("Text_tip"); //临时

        public Grid(int width, int height, float cellsize, GameObject parent, Vector3 originPosition)
        {
            this.width = width;
            this.height = height;
            this.cellsize = cellsize;
            this.originPosition = originPosition;
            this.parent = parent;

            gridArray = new int[this.width, this.height];
            debugTextArray = new TextMesh[this.width, this.height];
            gridStrings = new string[this.width, this.height];
            battalions = new GameObject[this.width, this.height];

            GameObject debugTips = new GameObject("DebugTips");

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = Functions.Functions.CreateWorldText(x + "," + y,debugTips.transform,
                        GetWorldPosition(x, y) + new Vector3(cellsize, cellsize) * 0.5f, 40, Color.white,
                        TextAnchor.MiddleLeft);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
        }

        public GameObject[,] Battalions
        {
            get => battalions;
            set => battalions = value;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * cellsize + originPosition + parent.transform.position;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition - parent.transform.position).x / cellsize);
            y = Mathf.FloorToInt((worldPosition - originPosition - parent.transform.position).y / cellsize);
            if (x < 0 || x > width)
            {
                x = 0;
            }

            if (y < 0 || y > height)
            {
                y = 0;
            }
        }

        public bool IsOnGrid(Vector3 worldPosition)
        {
           int x = Mathf.FloorToInt((worldPosition - originPosition - parent.transform.position).x / cellsize);
           int y = Mathf.FloorToInt((worldPosition - originPosition - parent.transform.position).y / cellsize);
            if (x < 0 || x > width)
            {
                return false;
            }

            if (y < 0 || y > height)
            {
                return false;
            }

            return true;
        }

        public void SetValue(int x, int y, int value)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                gridArray[x, y] = value;
                debugTextArray[x, y].text = gridArray[x, y].ToString();
            }
        }

        public void SetValue(Vector3 worldPosition, int value)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            Debug.Log(x + "." + y);
            SetValue(x, y, value);
        }

        public void RenewGrid(int x, int y)
        {
            for (int i = width; i < x; i++)
            {
                for (int j = height; j < y; j++)
                {
                }
            }
        }

        public GameObject GetBattalion(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return battalions[x, y];
            }
            else
            {
                return null;
            }
        }

        public void SetBattalion(int x, int y, GameObject battalion)
        {
            battalions[x, y] = battalion;
        }


        public int GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return gridArray[x, y];
            }
            else
            {
                return 0;
            }
        }

        public TextMesh[,] DebugTextArray
        {
            get => debugTextArray;
            set => debugTextArray = value;
        }

        /// <summary>
        /// 放置一个营
        /// </summary>
        /// <param name="x">网格x坐标</param>
        /// <param name="y">网格y坐标</param>
        /// <param name="battalion">营</param>
        /// <returns>是否可以放置</returns>
        public bool AddBattalion(int x, int y, GameObject battalion, out List<(int X, int Y)> list)
        {
            int[,] ints = battalion.GetComponent<Battalion.Battalion>().BattalionData.Ints;
            int dx = ints.GetLength(1);
            int dy = ints.GetLength(0);
            list = new List<(int X, int Y)>();
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                switch (battalion.GetComponent<Battalion.Battalion>().BattalionData.Dir.GetHashCode())
                {
                    case 0:
                        //up
                        if (!(x + dx < width && y + dy < height))
                        {
                            return false;
                        }

                        for (int i = 0; i < dx; i++)
                        {
                            for (int j = 0; j < dy; j++)
                            {
                                if (ints[j, i] == 1)
                                {
                                    if (!(battalions[x + i, y + j] is null))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < dx; i++)
                        {
                            for (int j = 0; j < dy; j++)
                            {
                                if (ints[j, i] == 1)
                                {
                                    list.Add((x + i, y + j));
                                    battalions[x + i, y + j] = battalion;
                                    debugTextArray[x + i, y + j].color = Color.red;
                                }
                            }
                        }

                        break;
                    case 1:
                        //right
                        if (!(x + dy < width && y - dx > 0)) return false;
                        for (int i = 0; i < dx; i++)
                        {
                            for (int j = 0; j < dy; j++)
                            {
                                if (ints[j, i] == 1)
                                {
                                    if (!(battalions[x + j, y - i] is null))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < dx; i++)
                        {
                            for (int j = 0; j < dy; j++)
                            {
                                if (ints[j, i] == 1)
                                {
                                    list.Add((x + j, y - i));
                                    battalions[x + j, y - i] = battalion;
                                    debugTextArray[x + j, y - i].color = Color.yellow;
                                }
                            }
                        }

                        break;
                    case 2:
                        //down
                        if (!(x - dx > 0 && y - dy > 0)) return false;
                        for (int i = 0; i < dx; i++)
                        {
                            for (int j = 0; j < dy; j++)
                            {
                                if (ints[j, i] == 1)
                                {
                                    if (!(battalions[x - i, y - j] is null))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < dx; i++)
                        {
                            for (int j = 0; j < dy; j++)
                            {
                                if (ints[j, i] == 1)
                                {
                                    list.Add((x - i, y - j));
                                    battalions[x - i, y - j] = battalion;
                                    debugTextArray[x - i, y - j].color = Color.green;
                                }
                            }
                        }

                        break;
                    case 3:
                        //left
                        if (!(x - dy > 0 && y + dx < height)) return false;
                        for (int i = 0; i < dx; i++)
                        {
                            for (int j = 0; j < dy; j++)
                            {
                                if (ints[j, i] == 1)
                                {
                                    if (!(battalions[x - j, y + i] is null))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < dx; i++)
                        {
                            for (int j = 0; j < dy; j++)
                            {
                                if (ints[j, i] == 1)
                                {
                                    list.Add((x - j, y + i));
                                    battalions[x - j, y + i] = battalion;
                                    debugTextArray[x - j, y + i].color = Color.blue;
                                }
                            }
                        }

                        break;
                }

                return true;
            }

            return false;
        }

        private void InitGrid(int x, int y)
        {
            battalions[x, y] = null;
            debugTextArray[x, y].color = Color.white;
        }

        public void RemoveBattalion(GameObject g)
        {
            List<(int X, int Y)> list = g.GetComponent<Battalion.Battalion>().BattalionXY;
            foreach (var XYs in list)
            {
                InitGrid(XYs.X, XYs.Y);
            }
        }
        
        
    }
}