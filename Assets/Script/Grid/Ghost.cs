using Script;
using Script.Functions;
using UnityEngine;
using UnityEngine.UI;
using Grid = Script.Grid.Grid;

public class Ghost : MonoBehaviour
{
    [SerializeField] private GameObject ghost;
    private GridManger gm;
    private Grid _grid;

    private void Start()
    {
        
        _grid = gameObject.GetComponent<GridManger>().Grid;
        gm = gameObject.GetComponent<GridManger>();
    }

    private void LateUpdate()
    {
        ghost.GetComponent<Image>().sprite = gm.Types switch
        {
            BattalionData.BattalionTypes.Infantry => Resources.Load<Sprite>("BattalionsImage/步兵营"),
            BattalionData.BattalionTypes.Artillery => Resources.Load<Sprite>("BattalionsImage/炮兵营"),
            BattalionData.BattalionTypes.Armor => Resources.Load<Sprite>("BattalionsImage/装甲营"),
            _ => ghost.GetComponent<Image>().sprite
        };
        _grid.GetXY(Functions.GetMouseWorldPosition(), out int x, out int y);
        Vector2Int rotationOffset = gm.GetRotationOffset(gm.Dirs);
        Vector3 placeObjectWorldPosition = _grid.GetWorldPosition(x, y) +
                                           new Vector3(rotationOffset.x, rotationOffset.y, 0) * gm.Cellsize;
        ghost.transform.position = placeObjectWorldPosition;
        ghost.transform.rotation = Quaternion.Euler(0, 0, gm.GetRotationAngle(gm.Dirs));
    }
}