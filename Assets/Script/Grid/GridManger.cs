using System.Collections;
using System.Collections.Generic;
using Script;
using Script.Functions;
using Unity.Mathematics;
using UnityEngine;
using Grid = Script.Grid.Grid;

public class GridManger : MonoBehaviour
{
    [SerializeField] private GameObject pfGameObject;

    [SerializeField] private int width;
    [SerializeField] private int heigth;
    [SerializeField] private float cellsize;

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

            Grid = new Script.Grid.Grid(width, heigth, cellsize, _gameObject, new(-540, -360));
        }
        else
        {
            Grid = new Script.Grid.Grid(width, heigth, cellsize, _gameObject, new(0, 0));
        }
    }

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Grid.GetXY(Functions.GetMouseWorldPosition(), out int x, out int y);

            Vector2Int rotationOffset = GetRotationOffset(Dirs);
            Vector3 placeObjectWorldPosition = Grid.GetWorldPosition(x, y) +
                                               new Vector3(rotationOffset.x, rotationOffset.y, 0) * cellsize;
            GameObject gameObject = Instantiate(
                pfGameObject,
                placeObjectWorldPosition,
                Quaternion.Euler(0, 0, GetRotationAngle(Dirs)),
                aimTransform);
            gameObject.GetComponent<Battalion>().BattalionData = new BattalionData(gameObject, 123,
                Types, Dirs);
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

        if (Input.GetMouseButtonDown(1))
        {
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
            Grid.GetXY(Functions.GetMouseWorldPosition(), out int x, out int y);
            GameObject g = Grid.GetBattalion(x, y);
            if (g is not null) Grid.RemoveBattalion(g);
            Destroy(g);
        }
    }
}