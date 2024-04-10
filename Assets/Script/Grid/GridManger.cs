using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using Script.Functions;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;
using Grid = Script.Grid.Grid;
using Types = Script.BattalionData.BattalionTypes;

public class GridManger : MonoBehaviour
{
    [SerializeField] private GameObject pfGameObject;

    [SerializeField] private int width;
    [SerializeField] private int heigth;
    [SerializeField] private float cellsize;
    [SerializeField] private bool isEditor;

    [Tooltip("目标对象")] [SerializeField] private GameObject _gameObject; //目标对象

    private Transform aimTransform;

    [Tooltip("开启后，将根据目标对象的大小，自动设置网格的长宽")] [SerializeField]
    private bool isAutoSet;

    public Grid Grid { get; set; }

    public float Cellsize
    {
        get => cellsize;
        set => cellsize = value;
    }

    public BattalionData.BattalionTypes Types { get; set; } = BattalionData.BattalionTypes.Infantry;

    public BattalionData.Dirs Dirs { get; set; } = BattalionData.Dirs.Up;

    public float GetRotationAngle(BattalionData.Dirs dirs)
    {
        switch (dirs)
        {
            default:
            case BattalionData.Dirs.Up: return 0f;
            case BattalionData.Dirs.Right: return 270f;
            case BattalionData.Dirs.Down: return 180f;
            case BattalionData.Dirs.Left: return 90f;
        }
    }

    public Vector2Int GetRotationOffset(BattalionData.Dirs dirs)
    {
        switch (dirs)
        {
            default:
            case BattalionData.Dirs.Up: return new Vector2Int(0, 0);
            case BattalionData.Dirs.Right: return new Vector2Int(0, 1);
            case BattalionData.Dirs.Down: return new Vector2Int(1, 1);
            case BattalionData.Dirs.Left: return new Vector2Int(1, 0);
        }
    }

    /// <summary>
    /// 相邻检测
    /// </summary>
    /// <param name="go">被检测的 battalion</param>
    /// <returns></returns>
    public List<GameObject> DetectAdjacentCellProperties(GameObject go)
    {
        BattalionData.Dirs godirs = go.GetComponent<Battalion>().BattalionData.Dir;
        Vector3 goVector3 = go.transform.position;
        Vector2Int rotationOffset = GetRotationOffset(godirs);
        Vector3 placeObjectWorldPosition = goVector3 -
                                           new Vector3(rotationOffset.x, rotationOffset.y, 0) * (cellsize * 2)
                                           + new Vector3(cellsize, cellsize, 0);
        if (godirs == BattalionData.Dirs.Down)
        {
            placeObjectWorldPosition += new Vector3(-cellsize, -cellsize);
        }

        Grid.GetXY(placeObjectWorldPosition, out int x, out int y);
        //debug文本内容
        string Debugtext = "相邻如下：\n";
        //相邻的对象
        List<GameObject> gameObjectList = new List<GameObject>();
        //
        GameObject gameObjectSelf = Grid.GetBattalion(x, y);
        List<(int X, int Y)> BattalionXY;
        if (gameObjectSelf is not null)
        {
            BattalionXY = gameObjectSelf.GetComponent<Battalion>().BattalionXY;
        }
        else
        {
            BattalionXY = new List<(int X, int Y)>();
            Debug.Log(x+","+y+"gameObjectSelf is null");
        }
        GameObject checkGameobject;
        foreach (var XYs in BattalionXY)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            checkGameobject = Grid.GetBattalion(XYs.X + 1, XYs.Y);

            if (checkGameobject != gameObjectSelf && checkGameobject is not null)
            {
                if (gameObjects.Count == 0)
                {
                    gameObjects.Add(checkGameobject);
                    gameObjectList.Add(checkGameobject);
                    
                    //debug文本内容
                    Debugtext += checkGameobject.GetComponent<Battalion>().BattalionData.BattalionName + XYs.X + "," +
                                 XYs.Y + "\n";
                }

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    if (checkGameobject == gameObjects[i]) continue;
                    gameObjects.Add(checkGameobject);
                    gameObjectList.Add(checkGameobject);
                    //debug文本内容
                    Debugtext += checkGameobject.GetComponent<Battalion>().BattalionData.BattalionName + XYs.X + "," +
                                 XYs.Y + "\n";
                }
            }

            checkGameobject = Grid.GetBattalion(XYs.X - 1, XYs.Y);
            if (checkGameobject != gameObjectSelf && checkGameobject is not null)
            {
                if (gameObjects.Count == 0)
                {
                    gameObjects.Add(checkGameobject);
                    gameObjectList.Add(checkGameobject);
                    //debug文本内容
                    Debugtext += checkGameobject.GetComponent<Battalion>().BattalionData.BattalionName + XYs.X + "," +
                                 XYs.Y + "\n";
                }

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    if (checkGameobject != gameObjects[i])
                    {
                        gameObjects.Add(checkGameobject);
                        gameObjectList.Add(checkGameobject);
                        //debug文本内容
                        Debugtext += checkGameobject.GetComponent<Battalion>().BattalionData.BattalionName + XYs.X +
                                     "," +
                                     XYs.Y + "\n";
                    }
                }
            }

            checkGameobject = Grid.GetBattalion(XYs.X, XYs.Y - 1);
            if (checkGameobject != gameObjectSelf && checkGameobject is not null)
            {
                if (gameObjects.Count == 0)
                {
                    gameObjects.Add(checkGameobject);
                    gameObjectList.Add(checkGameobject);
                    //debug文本内容
                    Debugtext += checkGameobject.GetComponent<Battalion>().BattalionData.BattalionName + XYs.X + "," +
                                 XYs.Y + "\n";
                }

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    if (checkGameobject != gameObjects[i])
                    {
                        gameObjects.Add(checkGameobject);
                        gameObjectList.Add(checkGameobject);
                        //debug文本内容
                        Debugtext += checkGameobject.GetComponent<Battalion>().BattalionData.BattalionName + XYs.X +
                                     "," +
                                     XYs.Y + "\n";
                    }
                }
            }

            checkGameobject = Grid.GetBattalion(XYs.X, XYs.Y + 1);
            if (checkGameobject != gameObjectSelf && checkGameobject is not null)
            {
                if (gameObjects.Count == 0)
                {
                    gameObjects.Add(checkGameobject);
                    gameObjectList.Add(checkGameobject);
                    //debug文本内容
                    Debugtext += checkGameobject.GetComponent<Battalion>().BattalionData.BattalionName + XYs.X + "," +
                                 XYs.Y + "\n";
                }

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    if (checkGameobject != gameObjects[i])
                    {
                        gameObjects.Add(checkGameobject);
                        gameObjectList.Add(checkGameobject);
                        //debug文本内容
                        Debugtext += checkGameobject.GetComponent<Battalion>().BattalionData.BattalionName + XYs.X +
                                     "," +
                                     XYs.Y + "\n";
                    }
                }
            }
        }

        return gameObjectList;
    }

    private void Awake()
    {
        aimTransform = _gameObject.transform.Find("BattalionList");
        if (_gameObject)
        {
            if (isAutoSet)
            {
                width = (int)(_gameObject.GetComponent<RectTransform>().rect.width / cellsize);
                heigth = (int)(_gameObject.GetComponent<RectTransform>().rect.height / cellsize);
            }

            Grid = new Script.Grid.Grid(width, heigth, cellsize, _gameObject,
                new Vector3(-_gameObject.GetComponent<RectTransform>().rect.width / 2,
                    -_gameObject.GetComponent<RectTransform>().rect.height / 2, 0));
        }
        else
        {
            Grid = new Script.Grid.Grid(width, heigth, cellsize, _gameObject,
                new Vector3(-_gameObject.GetComponent<RectTransform>().rect.width / 2,
                    -_gameObject.GetComponent<RectTransform>().rect.height / 2, 0));
        }
    }

    private void Update()
    {
        if (isEditor)
        {
            Editor();
        }
        
    }

    public void SwitchEditingStatus()
    {
        isEditor = !isEditor;
    }
    
    /// <summary>
    /// 编辑部队编成
    /// </summary>
    private void Editor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Grid.IsOnGrid(Functions.GetMouseWorldPosition()) is false)
            {
                return;
            }

            Grid.GetXY(Functions.GetMouseWorldPosition(), out int x, out int y);

            CreatBattalion(x, y,Dirs,Types);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Grid.IsOnGrid(Functions.GetMouseWorldPosition()) is false)
            {
                return;
            }

            switch (Types.GetHashCode())
            {
                case 0:
                    Types = BattalionData.BattalionTypes.Artillery;
                    break;
                case 1:
                    Types = BattalionData.BattalionTypes.Armor;
                    break;
                case 2:
                    Types = BattalionData.BattalionTypes.Infantry;
                    break;
            }

            Functions.CreateTip(Types.ToString(), Functions.GetMouseWorldPosition(), 1f);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (Dirs.GetHashCode())
            {
                case 0:
                    Dirs = BattalionData.Dirs.Left;
                    Functions.CreateTip("Left", Functions.GetMouseWorldPosition(), 1f);
                    break;
                case 1:
                    Dirs = BattalionData.Dirs.Up;
                    Functions.CreateTip("Up", Functions.GetMouseWorldPosition(), 1f);
                    break;
                case 2:
                    Dirs = BattalionData.Dirs.Right;
                    Functions.CreateTip("Right", Functions.GetMouseWorldPosition(), 1f);
                    break;
                case 3:
                    Dirs = BattalionData.Dirs.Down;
                    Functions.CreateTip("Down", Functions.GetMouseWorldPosition(), 1f);
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (Dirs.GetHashCode())
            {
                case 0:
                    Dirs = BattalionData.Dirs.Right;
                    Functions.CreateTip("Right", Functions.GetMouseWorldPosition(), 1f);
                    break;
                case 1:
                    Dirs = BattalionData.Dirs.Down;
                    Functions.CreateTip("Down", Functions.GetMouseWorldPosition(), 1f);
                    break;
                case 2:
                    Dirs = BattalionData.Dirs.Left;
                    Functions.CreateTip("Left", Functions.GetMouseWorldPosition(), 1f);
                    break;
                case 3:
                    Dirs = BattalionData.Dirs.Up;
                    Functions.CreateTip("Up", Functions.GetMouseWorldPosition(), 1f);
                    break;
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            if (Grid.IsOnGrid(Functions.GetMouseWorldPosition()) is false)
            {
                return;
            }

            Grid.GetXY(Functions.GetMouseWorldPosition(), out int x, out int y);
            GameObject g = Grid.GetBattalion(x, y);
            if (g is not null) Grid.RemoveBattalion(g);
            Destroy(g);
        }
    }
    /// <summary>
    /// 创建一个营
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    /// <param name="dirs">方向</param>
    /// <param name="types">类型</param>
    public void CreatBattalion(int x, int y,BattalionData.Dirs dirs,Types types)
    {
        Vector2Int rotationOffset = GetRotationOffset(dirs);
        Vector3 placeObjectWorldPosition = Grid.GetWorldPosition(x, y) +
                                           new Vector3(rotationOffset.x, rotationOffset.y, 0) * cellsize;
        GameObject gameObject = Instantiate(
            pfGameObject,
            placeObjectWorldPosition,
            Quaternion.Euler(0, 0, GetRotationAngle(dirs)),
            aimTransform);
        gameObject.GetComponent<Battalion>().BattalionData = new BattalionData(gameObject, 123,
            types, dirs);
        if (!Grid.AddBattalion(x, y, gameObject, out List<(int X, int Y)> list))
        {
            Destroy(gameObject);
            Functions.CreateTip("无法放置", Functions.GetMouseWorldPosition(), 2f);
        }

        if (list is not null)
        {
            gameObject.GetComponent<Battalion>().BattalionXY = list;
        }
    }

    public void Type2Infantry()
    {
        SwitchType(Types.Infantry);
    }

    public void Type2Artillery()
    {
        SwitchType(Types.Artillery);
    }

    public void Type2Armor()
    {
        SwitchType(Types.Armor);
    }

    public void SwitchType(Types types)
    {
        switch (types.GetHashCode())
        {
            case 0:
                Types = BattalionData.BattalionTypes.Infantry;
                break;
            case 1:
                Types = BattalionData.BattalionTypes.Artillery;
                break;
            case 2:
                Types = BattalionData.BattalionTypes.Armor;
                break;
        }
    }
}