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
    [Tooltip("目标对象")]
    [SerializeField] private GameObject _gameObject;//目标对象

    private Transform aimTransform;
    [Tooltip("开启后，将根据目标对象的大小，自动设置网格的长宽")][SerializeField] private bool isAutoSet;
    private Grid grid;

    private BattalionData.BattalionTypes _types =BattalionData.BattalionTypes.Infantry;
    private BattalionData.Dirs _dirs=BattalionData.Dirs.Up;
    
    private void Start()
    {
        aimTransform= _gameObject.transform.Find("BattalionList");
        if (_gameObject)
        {
            if (isAutoSet)
            {
                width = (int)(_gameObject.GetComponent<RectTransform>().rect.width / cellsize);
                heigth = (int)(_gameObject.GetComponent<RectTransform>().rect.height / cellsize);
            }
            grid = new Script.Grid.Grid(width, heigth,cellsize,_gameObject,new (-540,-360));
        }
        else
        {
            grid = new Script.Grid.Grid(width, heigth,cellsize,_gameObject, new (0,0));
        }
        
    }
    
    public static float GetRotationAngle(BattalionData.Dirs dirs)
    {
        switch (dirs)
        {
            default:
            case BattalionData.Dirs.Up : return 0f;
            case BattalionData.Dirs.Right: return 270f;
            case BattalionData.Dirs.Down: return 180f;
            case BattalionData.Dirs.Left : return 90f;
        }
    }

    public Vector2Int GetRotationOffset(BattalionData.Dirs dirs)
    {
        switch (dirs)
        {
            default:
            case BattalionData.Dirs.Up : return new Vector2Int(0,0);
            case BattalionData.Dirs.Right: return new Vector2Int(0,1);
            case BattalionData.Dirs.Down: return new Vector2Int(1,1);
            case BattalionData.Dirs.Left : return new Vector2Int(1,0);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.GetXY(Functions.GetMouseWorldPosition(),out int x,out int y);

            Vector2Int rotationOffset = GetRotationOffset(_dirs);
            Vector3 placeObjectWorldPosition = grid.GetWorldPosition(x, y) +new Vector3(rotationOffset.x,rotationOffset.y,0)*cellsize;
            GameObject gameObject = Instantiate(
                pfGameObject, 
                placeObjectWorldPosition, 
                Quaternion.Euler(0,0,GetRotationAngle(_dirs)), 
                aimTransform);
            gameObject.GetComponent<Battalion>().BattalionData = new BattalionData(gameObject,123,
                _types, _dirs);
            if (!grid.AddBattalion(x,y,gameObject,out List<(int X, int Y)> list))
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
            switch (_types.GetHashCode())
            {
                case 0: _types = BattalionData.BattalionTypes.Artillery;
                    break;
                case 1: _types = BattalionData.BattalionTypes.Armor;
                    break;
                case 2: _types = BattalionData.BattalionTypes.Infantry;
                    break;
                
            }
            Functions.CreateTip(_types.ToString(),Functions.GetMouseWorldPosition(),1f);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (_dirs.GetHashCode())
            {
                case 0: _dirs = BattalionData.Dirs.Left;
                    Functions.CreateTip("Left",Functions.GetMouseWorldPosition(),1f);
                    break;
                case 1: _dirs = BattalionData.Dirs.Up;
                    Functions.CreateTip("Up",Functions.GetMouseWorldPosition(),1f);
                    break;
                case 2: _dirs = BattalionData.Dirs.Right;
                    Functions.CreateTip("Right",Functions.GetMouseWorldPosition(),1f);
                    break;
                case 3: _dirs = BattalionData.Dirs.Down;
                    Functions.CreateTip("Down",Functions.GetMouseWorldPosition(),1f);
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (_dirs.GetHashCode())
            {
                case 0: _dirs = BattalionData.Dirs.Right;
                    Functions.CreateTip("Right",Functions.GetMouseWorldPosition(),1f);
                    break;
                case 1: _dirs = BattalionData.Dirs.Down;
                    Functions.CreateTip("Down",Functions.GetMouseWorldPosition(),1f);
                    break;
                case 2: _dirs = BattalionData.Dirs.Left;
                    Functions.CreateTip("Left",Functions.GetMouseWorldPosition(),1f);
                    break;
                case 3: _dirs = BattalionData.Dirs.Up;
                    Functions.CreateTip("Up",Functions.GetMouseWorldPosition(),1f);
                    break;
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            grid.GetXY(Functions.GetMouseWorldPosition(),out int x,out int y);
            GameObject g = grid.GetBattalion(x, y);
            if(g is not null) grid.RemoveBattalion(g);
            Destroy(g);
        }
    }
}
