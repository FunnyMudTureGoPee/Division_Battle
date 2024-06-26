using Script.Battalion;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Grid
{
    public class Ghost : MonoBehaviour
    {
        [SerializeField] private GameObject ghost;
        private DivisionManger gm;
        private Grid _grid;

        private void Start()
        {
        
            _grid = gameObject.GetComponent<DivisionManger>().Grid;
            gm = gameObject.GetComponent<DivisionManger>();
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
            _grid.GetXY(Functions.Functions.GetMouseWorldPosition(), out int x, out int y);
            Vector2Int rotationOffset = gm.GetRotationOffset(gm.Dirs);
            Vector3 placeObjectWorldPosition = _grid.GetWorldPosition(x, y) +
                                               new Vector3(rotationOffset.x, rotationOffset.y, 0) * gm.Cellsize;
            ghost.transform.position = placeObjectWorldPosition;
            ghost.transform.rotation = Quaternion.Euler(0, 0, gm.GetRotationAngle(gm.Dirs));
        }
    }
}