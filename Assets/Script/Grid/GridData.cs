using System;
using System.Collections.Generic;
using Script.Battalion;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Grid
{
    [Serializable]
    public struct battalionData
    {
        public int x, y;
        public BattalionData.Dirs dirs;
        public BattalionData.BattalionTypes types;
        public int id;

        
        public battalionData(int x, int y, BattalionData.Dirs dirs, BattalionData.BattalionTypes types, int id)
        {
            this.x = x;
            this.y = y;
            this.dirs = dirs;
            this.types = types;
            this.id = id;
        }
    }

    [Serializable]
    public class GridData
    {
        public string name;
        public int level;
        public List<battalionData> battalionDatas = new List<battalionData>();
        public int width, high;

        public GridData()
        {
        }

        public GridData(string name, int level, GameObject[,] battalions)
        {
            this.name = name;
            this.level = level;
            width = battalions.GetLength(0);
            high = battalions.GetLength(1);

            Array2List(battalions);
        }

        private void Array2List(GameObject[,] battalions)
        {
            List<GameObject> checkObjects = new List<GameObject>();
            for (var i0 = 0; i0 < battalions.GetLength(0); i0++)
            for (var i1 = 0; i1 < battalions.GetLength(1); i1++)
            {
                var battalionData = battalions[i0, i1];
                if (battalionData is null) continue;
                int offset_x, offset_y;
                if (checkObjects.Count == 0)
                {
                    
                    checkObjects.Add(battalionData);
                    offset_x = GetOffset(battalionData.GetComponent<Battalion.Battalion>().BattalionData.Dir,
                        battalionData.GetComponent<Battalion.Battalion>().BattalionData.BattalionType).x;
                    offset_y = GetOffset(battalionData.GetComponent<Battalion.Battalion>().BattalionData.Dir,
                        battalionData.GetComponent<Battalion.Battalion>().BattalionData.BattalionType).y;
                    battalionDatas.Add(new battalionData(i0+offset_x, i1+offset_y,
                        battalionData.GetComponent<Battalion.Battalion>().BattalionData.Dir,
                        battalionData.GetComponent<Battalion.Battalion>().BattalionData.BattalionType,
                        battalionData.GetComponent<Battalion.Battalion>().BattalionData.ID));
                }

                bool b = true;
                foreach (var checkObject in checkObjects)
                {
                    if (battalionData == checkObject)
                    {
                        b = false;
                    }
                }

                if (!b) continue;
                checkObjects.Add(battalionData);
                offset_x = GetOffset(battalionData.GetComponent<Battalion.Battalion>().BattalionData.Dir,
                    battalionData.GetComponent<Battalion.Battalion>().BattalionData.BattalionType).x;
                offset_y = GetOffset(battalionData.GetComponent<Battalion.Battalion>().BattalionData.Dir,
                    battalionData.GetComponent<Battalion.Battalion>().BattalionData.BattalionType).y;
                battalionDatas.Add(new battalionData(i0+offset_x, i1+offset_y,
                    battalionData.GetComponent<Battalion.Battalion>().BattalionData.Dir,
                    battalionData.GetComponent<Battalion.Battalion>().BattalionData.BattalionType,
                    battalionData.GetComponent<Battalion.Battalion>().BattalionData.ID));
            }
        }

        private (int x, int y) GetOffset(BattalionData.Dirs dir, BattalionData.BattalionTypes type)
        {
            int offset_x=0, offset_y=0;
            switch (type)
            {
                case BattalionData.BattalionTypes.Infantry:
                    switch (dir)
                    {
                        case BattalionData.Dirs.Up:
                            break;
                        case BattalionData.Dirs.Right:
                            offset_y += 1;
                            break;
                        case BattalionData.Dirs.Down:
                            offset_x += 1;
                            offset_y += 2;
                            break;
                        case BattalionData.Dirs.Left:
                            offset_x += 2;
                            break;
                    }
                    break;
                case BattalionData.BattalionTypes.Artillery:
                    switch (dir)
                    {
                        case BattalionData.Dirs.Up:
                            offset_y -= 1;
                            break;
                        case BattalionData.Dirs.Right:
                            offset_y += 2;
                            break;
                        case BattalionData.Dirs.Down:
                            offset_x += 3;
                            offset_y += 1;
                            break;
                        case BattalionData.Dirs.Left:
                            offset_x += 2;
                            offset_y -= 1;
                            break;
                    }
                    break;
                case BattalionData.BattalionTypes.Armor:
                    switch (dir)
                    {
                        case BattalionData.Dirs.Up:
                            break;
                        case BattalionData.Dirs.Right:
                            offset_y += 3;
                            break;
                        case BattalionData.Dirs.Down:
                            offset_x += 3;
                            offset_y += 1;
                            break;
                        case BattalionData.Dirs.Left:
                            offset_x += 2;
                            offset_y -= 1;
                            break;
                    }
                    break;
            }

            return (offset_x, offset_y);
        }
    }
}